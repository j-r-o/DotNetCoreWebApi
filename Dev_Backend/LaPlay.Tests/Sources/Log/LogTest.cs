using Xunit;
<<<<<<< HEAD
using System;
=======
>>>>>>> develop
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using System.Text;
using System.IO;
using System.Diagnostics;
using LaPlay.Sources.Log;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace LaPlay.Sources.Log
{
    public class LogTest
    {

        private string generateRandomString(int length)
        {
            Random _random = new Random();

            return string.Join("", Enumerable.Range(1,length).Select(i => (char)(_random.Next(0, 255))).ToList());
        }

        [Fact]
<<<<<<< HEAD
        public void generateRandomString_shouldSucceed()
        {
            //Prepare sample and statistics
            Int32 randomCharactersNumber = 1000000;
            String randomCharacters = generateRandomString(randomCharactersNumber);
            Double averageASCIICharacterCode = randomCharacters.Select(character => (int)character).Average();

            //Verify number corespond with what was asked
            Assert.Equal(randomCharactersNumber, randomCharacters.Length);

            //Verify that all ASCII charaters are generater
            Assert.Equal(255, randomCharacters.Distinct().Count());

            //Verify that the average is +/- 10 % around 127 : the middle of the ASCII code range [0;255]
            Assert.True(0.9 * 127 <= averageASCIICharacterCode && averageASCIICharacterCode <= 1.1 * 127);
        }

        [Fact]
        public void infoWarningDebug_shouldLogOnOwnLevel()
        {
            Func<Log, String> debugLog = (Log log) => {String logEntry = generateRandomString(10000); log.debug(logEntry); return logEntry;};
            Func<Log, String> infoLog = (Log log) => {String logEntry = generateRandomString(10000); log.info(logEntry); return logEntry;};
            Func<Log, String> warningLog = (Log log) => {String logEntry = generateRandomString(10000); log.warning(logEntry); return logEntry;};

            List<dynamic> levelsToTest = new List<dynamic>()
            {
                new {threadsLog = new ConcurrentBag<dynamic>(), fileLog = new Log("Info"), infoAction = infoLog}//,
                //new {threadsLog = new ConcurrentBag<dynamic>(), fileLog = new Log("Warning"), infoAction = warningLog},
                //new {threadsLog = new ConcurrentBag<dynamic>(), fileLog = new Log("Debug"), infoAction = debugLog}
            };

            levelsToTest.ForEach(level => {

                List<Thread> threads = Enumerable.Range(1, 10).Select(i =>

                    new Thread(() => {
                        
                        Stopwatch s = new Stopwatch();
                        s.Start();
                        Console.Write("thread running");
                        while (s.Elapsed < TimeSpan.FromMilliseconds(10)) {
                        
                            level.threadsLog.Add(level.infoAction.Invoke(level.fileLog));
                        }

                        s.Stop();
                    })
                ).ToList();

                threads.ForEach(thread => thread.Start());
                threads.ForEach(thread => thread.Join());
            });

            levelsToTest.ForEach(level => {Console.WriteLine(level.threadsLog.Lenght());});

            // Assert.True(verifyLogContentWithThreadsResult)
        }
/*
        [Fact]
        public void info_shouldNotLogOnNonInfoLevel()
        {
            log = new Log("None");

            Action logInfoAction = () => log.info(generateRandomString(10000));

            floodAndVerify(10, logInfoAction);
        }

         [Fact]
        public void warning_shouldLogOnWarningLevel()
        {
            log = new Log("Warning");

            Action logInfoAction = () => log.info(generateRandomString(10000));

            floodAndVerify(10, logInfoAction);
        }

         [Fact]
        public void warning_shouldNotLogOnNonWarningLevel()
        {
            log = new Log("None");

            Action logInfoAction = () => log.info(generateRandomString(10000));

            floodAndVerify(10, logInfoAction);
        }

         [Fact]
        public void debug_shouldLogOnDebugLevel()
        {
            Action logInfoAction = () => log.info(generateRandomString(10000));

            floodAndVerify(10, logInfoAction);
        }

         [Fact]
        public void debug_shouldNotLogOnNonDebugLevel()
        {
            log = new Log("None");

            Action logInfoAction = () => log.info(generateRandomString(10000));

            floodAndVerify(10, logInfoAction);
        }


            new Thread (() => MyFunction(param1, param2))


            Func<Int32, Func<string, int>, Thread> callLoopThread = (durationInSeconds, Func<string, int> method) =>
            {
                int sum = int32A + int32B;
                return sum;
            };




            //FloodThread(duration) : a thread that calls info() for a specific duration - it return all the written text as a list

            //Flood : a loop that starts X FloodThread

            //Verify that the text written by the threads is in the log file
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
                //threads.Add(new Thread(() => {code here}));
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
        }
*/
    }
}