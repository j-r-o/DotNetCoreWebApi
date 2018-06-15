using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace LaPlay.Api.Sources.Log
{
    public class Log : ILog
    {
        private FileStream _logFile;
        private StreamWriter _writer;

        private ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();

        public Log()
        {
            //_logFile = File.Open(@"log.txt", FileMode.Append, FileAccess.Write);

            _logFile = new FileStream(@"log.txt", FileMode.OpenOrCreate);
            _writer = new StreamWriter(_logFile, Encoding.UTF8, 1024, true);
        }

        public void info(String log){
            _writer.Write(log);
        }

        public void warning(String log){
            _writer.Write(log);
        }

        public void debug(String log){
            rwls.EnterWriteLock();
            _writer.WriteLine(log);
            rwls.ExitWriteLock();
        }
    }
}