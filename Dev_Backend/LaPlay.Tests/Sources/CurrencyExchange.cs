using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;
using System.Net;

using Xunit;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Moq;
using System.Security.Cryptography;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using LaPlay.Infrastructure.Shell;

namespace LaPlay.Infrastructure.Synchronization
{
    public class CurrencyExchange
    {
        // RATE LOG                 #################################################
            private Queue<Double> AUDToEURHistory = new Queue<Double>();
        // CREDIT : CURRENT / TRACKER   #############################################
            Double AUDCurrentCredit = 9500;
            Double EURCurrentCredit = 0;

            Double AUDPreviousCredit = 9500;
            Double EURPreviousCredit = 0;

        // UTILS                    #################################################

                public string RunCommand(String command)
                {
                    var escapedArgs = command.Replace("\"", "\\\"");

                    var process = new Process()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $"-c \"{escapedArgs}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };
                    process.Start();
                    string result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return result;
                }
                
        // CURRENT AUD >  EUR RATE  #################################################
        // RATE LOG                 #################################################

                private Double liveAUDToEUR()
                {
                    String rate = RunCommand("curl -k https://api.ofx.com/PublicSite.ApiService/OFX/spotrate/Individual/AUD/EUR/1000?format=json");
                    JObject _JsonData = JObject.Parse(rate);
                    String InterbankRate = _JsonData["InterbankRate"].ToString();
                    return Convert.ToDouble(InterbankRate.Substring(0, 6));

                    // Int32 rnd = new Random().Next(1, 3);
                    // Double fluctuation = (rnd == 1 ? 0.001 : -0.001);
                    // Double newRate = AUDToEURHistory.Last() + fluctuation;
                    // AUDToEURHistory.Enqueue(newRate);
                }

                private void logRate(Double AudToEur)
                {
                    Console.WriteLine("###" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   NEW RATE : " + AudToEur + " / " + 1/AudToEur + "   ###");
                    AUDToEURHistory.Enqueue(AudToEur);
                    if(AUDToEURHistory.Count() > 60 * 10) AUDToEURHistory.Dequeue();
                }

        // RATE VARIATION ANALYSIS  #################################################

                //Average on 10 seconds, 1 minute ago
                private Double AUDToEURAvg(Int32 secsAgo, Int32 intervalSecs)
                {
                    return Enumerable.Range(secsAgo, intervalSecs).Select(i => AUDToEURHistory.ElementAt(i)).Average();
                }
            
                private void statStability100()
                {
                    //(AUDToEURAvg_m100_m90.Invoke() - AUDToEURAvg_m10_0.Invoke()) / AUDToEURAvg_m10_0.Invoke();
                }

        // BUYING CONDITIONS


                Double currentAudCreditInEur => AUDCurrentCredit * AUDToEURHistory.Last();
                Double currentEurCreditInAud => EURCurrentCredit / AUDToEURHistory.Last();

                Boolean changeCurrentEurToAudWouldGenerateProfif => AUDPreviousCredit < currentEurCreditInAud;
                Boolean changeCurrentAudToEurWouldGenerateProfif => EURPreviousCredit < currentAudCreditInEur;


        [Fact]
        public void exchangeRateSimulator()
        {
            //initializeAUDToEURHistory();

            Action convertToEUR = () => {
                AUDPreviousCredit = AUDCurrentCredit;
                EURCurrentCredit = currentAudCreditInEur;
                AUDCurrentCredit = 0;
            };

            Action convertToAUD = () => {
                EURPreviousCredit = EURCurrentCredit;
                AUDCurrentCredit = currentEurCreditInAud;
                EURCurrentCredit = 0;
            };

            Int32 NoTransactionCounter = 0;

            for(Int32 i = 0; i < 1000; i++)
            {
                logRate(liveAUDToEUR());

                if      (changeCurrentEurToAudWouldGenerateProfif)  convertToAUD.Invoke();
                else if (changeCurrentAudToEurWouldGenerateProfif)  convertToEUR.Invoke();
                else
                {
                    NoTransactionCounter +=1;

                    if(NoTransactionCounter == 10)
                    {
                        if(EURCurrentCredit == 0) convertToEUR.Invoke();
                        else convertToAUD.Invoke();

                        Console.WriteLine("Sacrifice trade");

                        NoTransactionCounter = 0;
                    }
                }

                Console.WriteLine("AUDCurrentCredit : " + AUDCurrentCredit + " Value in EUR " + currentAudCreditInEur);
                Console.WriteLine("AUDPreviousCredit : " + AUDPreviousCredit);
                Console.WriteLine("EURCurrentCredit : " + EURCurrentCredit + " Value in AUD " + currentEurCreditInAud);
                Console.WriteLine("EURPreviousCredit : " + EURPreviousCredit);
                //Console.WriteLine("updateStability100 : " + updateStability100);

                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}