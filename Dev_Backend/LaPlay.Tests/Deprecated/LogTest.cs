using Xunit;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using LaPlay.Sources.Log;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Moq;

namespace Deprecated
{
    public class LogTest
    {
        public class LogTestTools
        {
            public static Boolean SameLists(List<String> listOne,List<String> listTwo)
            {
                if (listOne.Count != listTwo.Count) return false;

                List<String> listOneSorted = (from item in listOne orderby item select item).Cast<String>().ToList();
                List<String> listTwoeSorted = (from item in listTwo orderby item select item).Cast<String>().ToList();
                
                List<Boolean> oneByOneComparison = Enumerable.Range(0, listOne.Count).Select(index => listOneSorted.ElementAt(index) == listTwoeSorted.ElementAt(index)).Distinct().ToList();
                
                return !oneByOneComparison.Contains(false);
            }

            private static string generateRandomString(int length)
            {
                Random _random = new Random();

                return string.Join("", Enumerable.Range(1, length).Select(i => (char)(_random.Next(0, 65535))).ToList());
            }

            public static List<Thread> prepareThreadsForLogStress(ILog log, ConcurrentBag<dynamic> concurrentBag, Int32 threadsNumber, Int32 durationInMiliseconds, Int32 randomStringLength)
            {
                return Enumerable.Range(1, threadsNumber).Select(i =>
                    new Thread(() => {
                        String threadId = Guid.NewGuid().ToString();
                        Stopwatch stopWatch = new Stopwatch();
                        
                        stopWatch.Start();
                        while (stopWatch.Elapsed < TimeSpan.FromMilliseconds(durationInMiliseconds))
                        {
                            String randomString = generateRandomString(randomStringLength);

                            //Exclude line return character
                            while(randomString.Contains((char)13)) 
                            {
                                randomString.Replace(((char)13), generateRandomString(1)[0]);
                            }

                            String logLine = threadId + " " + randomString;
                            Console.WriteLine("trying to log");
                            log.Developpement(logLine);
                            concurrentBag.Add(logLine);
                            Console.WriteLine("OK !");

                            //var logLine = new {ThreadId = threadId, Content = generateRandomString(randomStringLength)}; 
                            //log.Developpement(logLine.ThreadId + " " + logLine.Content);
                            //concurrentBag.Add(logLine);
                        }
                        stopWatch.Stop();
                    })
                ).ToList();
            }

            [Fact]
            public void sameLists_ShouldReturnTrueOneEmptyLists()
            {
                Assert.True(SameLists(new List<String>(),new List<String>()));
            }

            [Fact]
            public void sameLists_ShouldReturnTrueOneSameLists()
            {
                Assert.True(SameLists(new List<String>(){"a", "c", "b"}, new List<String>(){"b", "a", "c"}));
            }

            [Fact]
            public void sameLists_ShouldReturnFalseOnListsWithDifferentSizes()
            {
                Assert.False(SameLists(new List<String>(){"a", "b"}, new List<String>(){"b", "a", "c"}));
            }

            [Fact]
            public void sameLists_ShouldReturnFalseOnDifferentLists()
            {
                Assert.False(SameLists(new List<String>(){"a", "b", "d"}, new List<String>(){"b", "a", "c"}));
            }

            [Fact]
            public void generateRandomString_shouldSucceed()
            {
                //Prepare sample and statistics
                Int32 randomCharactersNumber = 1000000;
                String randomCharacters = LogTestTools.generateRandomString(randomCharactersNumber);
                Double averageASCIICharacterCode = randomCharacters.Select(character => (int)character).Average();

                //Verify number corespond with what was asked
                Assert.Equal(randomCharactersNumber, randomCharacters.Length);

                //Verify that all Unicode charaters are generater
                Assert.Equal(65535, randomCharacters.Distinct().Count());

                //Verify that the average is +/- 10 % around 32767 : the middle of the unicode code range [0;65535]
                Assert.True(0.9 * 32767 <= averageASCIICharacterCode && averageASCIICharacterCode <= 1.1 * 32767);
            }

            [Fact]
            public void prepareThreadsForLogStress_shouldSucceed()
            {
                var mockedLog = new Mock<ILog>();
                ConcurrentBag<String> mockedLogFile = new ConcurrentBag<String>();
                mockedLog.Setup(log => log.Developpement(It.IsAny<String>())).Callback((String logLine) => {mockedLogFile.Add(logLine);});
                ConcurrentBag<dynamic> threadWritings = new ConcurrentBag<dynamic>();

                List<Thread> threads = prepareThreadsForLogStress(mockedLog.Object, threadWritings, 1000, 10, 1000);

                threads.ForEach(thread => thread.Start());
                threads.ForEach(thread => thread.Join());

                //Verify that number of log calls corresponds the sum of all threads calls 
                Assert.True(mockedLogFile.Count == threadWritings.Count);
                Console.WriteLine(mockedLogFile.Count + " - " + threadWritings.Count);

                List<String> sortedThreadWritings = (from tw in threadWritings orderby tw select tw).Cast<String>().ToList();
                List<String> sortedMockedLogFileLines = (from mlf in mockedLogFile orderby mlf select mlf).Cast<String>().ToList();

                List<Boolean> comparisons = Enumerable.Range(0, sortedThreadWritings.Count - 1).Select(i => sortedThreadWritings.ElementAt(i) == sortedMockedLogFileLines.ElementAt(i)).Distinct().Cast<Boolean>().ToList();

                //Verify that mocked log content is the same what the threads send
                Assert.True(!comparisons.Contains(false));
            }
        }

        //dotnet test --filter "FullyQualifiedName=LaPlay.Sources.Log.LogTest.productionDeveloppement_shouldLogOnOwnLevel"
        [Fact]
        public void productionDeveloppement_shouldLogOnOwnLevel()
        {
            Console.WriteLine("========================");
            Log log = new Log(Log.Level.Developpement);

            ConcurrentBag<dynamic> threadWritings = new ConcurrentBag<dynamic>();

            List<Thread> threads =  LogTestTools.prepareThreadsForLogStress(log, threadWritings, 10, 10, 50);

            Console.WriteLine("#1#");
            threads.ForEach(thread => thread.Start());
            Console.WriteLine("#2#");

            //threads.Select(thread => thread.ThreadState.Equals



            threads.ForEach(thread => Console.WriteLine(thread.ThreadState));

            threads.ForEach(thread => thread.Join());
            threads.ForEach(thread => Console.WriteLine(thread.ThreadState));

            threads.ForEach(thread => thread.Join());
            Console.WriteLine(threadWritings.Count);
            Console.WriteLine("###");
            threadWritings.Select(logLine => Console.WriteLine(logLine));

            //List<string> logLines = new List<string>(File.ReadAllLines("log.txt"));

            //Console.WriteLine(logLines.Count + " - " + threadWritings.Count);
        }

        //dotnet test --filter "FullyQualifiedName=LaPlay.Sources.Log.LogTest.aaabbbccc"

        [Fact]
        public void aaabbbccc()
        {
            Console.WriteLine("#");

        }

        /*
        [Fact]
        public void infoWarningDebug_shouldLogOnOwnLevel()
        {
            Int32 randomStringLength = 100;

            Func<Log, String> debugLog = (Log log) => {String logEntry = generateRandomString(randomStringLength); log.Developpement(logEntry); return logEntry;};
            Func<Log, String> infoLog = (Log log) => {String logEntry = generateRandomString(randomStringLength); log.Production(logEntry); return logEntry;};

            List<dynamic> levelsToTest = new List<dynamic>()
            {
                new {threadsLog = new ConcurrentBag<dynamic>(), fileLog = new Log(Log.Level.Developpement), infoAction = debugLog}//,
                //new {threadsLog = new ConcurrentBag<dynamic>(), fileLog = new Log("Warning"), infoAction = warningLog},
                //new {threadsLog = new ConcurrentBag<dynamic>(), fileLog = new Log("Debug"), infoAction = debugLog}
            };

            levelsToTest.ForEach(level => {

                List<Thread> threads = Enumerable.Range(1, 1024).Select(i =>

                    new Thread(() => {

                        String threadId = Guid.NewGuid().ToString();
                        Stopwatch stopWatch = new Stopwatch();
                        
                        stopWatch.Start();
                        
                        String sa;
                        while (stopWatch.Elapsed < TimeSpan.FromMilliseconds(10000)) {

                            var logLine = new {ThreadId = threadId, Content = generateRandomString(randomStringLength)}; 
                            //sa = level.infoAction.Invoke(level.fileLog);
                            //Console.WriteLine(sa);
                            level.threadsLog.Add(logLine);

                            try{
                            //level.threadsLog.Add(s);
                            }
                            catch(Exception e){
                                Console.WriteLine(e.ToString());
                            }
                        
                            //level.threadsLog.Add(level.infoAction.Invoke(level.fileLog));
                        }

                        stopWatch.Stop();
                    })
                ).ToList();

                threads.ForEach(thread => thread.Start());
                threads.ForEach(thread => thread.Join());
            });

            levelsToTest.ForEach(level => {Console.WriteLine("concurrentbag htreadsLog size : " + level.threadsLog.Count);});

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
