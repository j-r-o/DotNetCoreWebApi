using Xunit;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using LaPlay.Api.Sources.Log;

namespace LaPlay.Test.Sources.Log
{
    public class Log
    {
        [Fact]
        public void ReturnOK()
        {

            LaPlay.Api.Sources.Log.Log l = new LaPlay.Api.Sources.Log.Log();

            List<Thread> threads = Enumerable.Range(1, 10)
                .Select(i => new Thread(() => {

                        Stopwatch s = new Stopwatch();
                        s.Start();
                        while (s.Elapsed < TimeSpan.FromMilliseconds(1/100)) 
                        {
                            l.debug("#");
                        }

                        s.Stop();
                }))
                .ToList();

            threads.ForEach(t => t.Start());

            threads.ForEach(t => t.Join());


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
