using Xunit;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using LaPlay.Api.Sources.Log;
using System.Text.RegularExpressions;

namespace LaPlay.Test.Sources.Log
{
    public class Log
    {

        private string generateRandomString(int length)
        {
            Random _random = new Random();

            return string.Join("", Enumerable.Range(1,length).Select(i => (char)(_random.Next(0, 255))).ToList());
        }

        [Fact]
        public void generateRandomString_shouldReturnRandomStringWithExpectedLength()
        {
            Assert.Equal(1024, generateRandomString(1024).Length);

            
            Console.WriteLine("###########");
            Console.WriteLine(generateRandomString(1024).Length);
            Console.WriteLine(Encoding.ASCII.GetBytes(generateRandomString(1024)).Select(x => (int)x).Average());


            string s = generateRandomString(10);

            Console.WriteLine(s);
            Console.WriteLine(String.Join(", ", s.Select(x => (int)x)));
            Console.WriteLine(s.Select(x => (int)x).Average());
            Console.WriteLine(String.Join(", ", s.Select(x => (int)x).Select(x => (char)x)));
            Console.WriteLine(s.Select(x => (int)x).Select(x => (char)x).Select(x => (int)x).Average());

            Console.WriteLine("###########");
        }

        private void testLogFile(){

            var v = new { Amount = 108, Message = "Hello" }; 

            string path = @"Log.txt";

            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);

                string allLines = string.Join("", readText);
                Console.WriteLine("Log file has " + readText.Length + " lines");


                 string pattern = @"\d+";
                MatchCollection matches = Regex.Matches(allLines, pattern);
                // matches.Count

                Console.Write(allLines.Length);
            }
        }

        private void threadMission(object o){

            LaPlay.Api.Sources.Log.Log l = (LaPlay.Api.Sources.Log.Log)o; 

            String g = Guid.NewGuid().ToString();
            //g = i.ToString();

            int wrintings = 0;

            Stopwatch s = new Stopwatch();
            s.Start();
            while (s.Elapsed < TimeSpan.FromMilliseconds(10)) 
            //for(int j =0; j<1000; j++)
            {
                wrintings++;

                l.debug(g + " #######################################################");
                //l.debug(Thread.CurrentThread.Name + " #######################################################");
            }

            s.Stop();
            l.debug(g + " wrote " + wrintings + " times");
            Console.Write(g + " wrote " + wrintings + " times" + "\r\n");
        }

        [Fact]
        public void ReturnOK()
        {

            LaPlay.Api.Sources.Log.Log l = new LaPlay.Api.Sources.Log.Log();

            //Enumerable.Range(1,10).Select(i => l.debug(i.ToString()));
            List<Thread> threads = new List<Thread>();

            //List<Thread> threads = Enumerable.Range(1, 1)
             //   .Select(i => new Thread(() => {
            for(int i = 0; i < 10 ; i++)
            {                    
                //threads.Add(new Thread(() => {/*code here */}));
                threads.Add(new Thread(new ParameterizedThreadStart(threadMission)));
            }

            l.debug(threads.Count() + " threads ready");
            threads.ForEach(t => {t.Start(l);});
            threads.ForEach(t => {t.Name = "sdf";});
            l.debug(threads.Count() + " threads running");


            //threads.ForEach(t => Console.Write(t.ThreadState + "\r\n"));

            l.debug("+1");
            //threads.ForEach(t => t.IsBackground = true);
            threads.ForEach(t => t.Join());

            //threads.ForEach(t => Console.Write(t.ThreadState + "\r\n"));

            l.debug("+2");
            Console.Write(l.getReaderWriterLockSlim() + "\r\n");
            l.debug("+3");
            l.debug(threads.Count() + " threads");
            l.debug("+5");
            l.debug(threads.Count() + " threads finished");
            l.debug("+6");

            testLogFile();

            /*

            Thread[] t = new Thread[10];

            for (int tNum = 0; tNum < 10; tNum++) {
            int n = tNum; // new line
            t[tNum] = new Thread(() => {
                Thread.Sleep(new Random().Next(20));
                Console.Write(n + " "); // changed line
            });
            t[tNum].Start();
        }
        // wait for the threads to finish
        for (int tNum = 0; tNum < 10; tNum++) {
            t[tNum].Join();
        }

            Thread t = new Thread(new ThreadStart(MyThreadMethod));

            

            // Assert
            Assert.Equal("OK OK", responseString);
            */
        }
    }
}
