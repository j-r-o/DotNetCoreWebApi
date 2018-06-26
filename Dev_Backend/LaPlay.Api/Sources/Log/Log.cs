using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace LaPlay.Sources.Log
{
    public class Log : ILog
    {
        private String          _logLevel;
        private FileStream      _logFile;
        private StreamWriter    _writer;

        private ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim();

        private ThreadSafeLog(String log)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            _writer.Write(log);
            _ReaderWriterLockSlim.ExitWriteLock();
        }

        public Log(String logLevel)
        {
            _logFile = new FileStream(@"log.txt", FileMode.OpenOrCreate);
            _writer = new StreamWriter(_logFile, Encoding.UTF8, 1048576, true); //1Mo buffer, default AutoFlush = false is faster then true
        }

        public void info(String log)
        {
            ThreadSafeLog(log);
        }

        public void warning(String log){
            ThreadSafeLog(log);
        }

        public void debug(String log){
            ThreadSafeLog(log);
        }
    }
}