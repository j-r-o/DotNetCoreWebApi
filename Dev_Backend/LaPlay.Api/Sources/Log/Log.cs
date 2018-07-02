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
        private Level           _level;
        private FileStream      _logFile;
        private StreamWriter    _writer;

        public enum Level
        {
            Production = 1,
            Developpement = 0,
        }

        private ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim();

        private void ThreadSafeLog(String log)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            _writer.WriteLine(log);

            //Console.WriteLine("# ThreadSafeLog #");

            _ReaderWriterLockSlim.ExitWriteLock();
        }

        public Log(Level level)
        {
            _level = level;
            _logFile = new FileStream(@"log.txt", FileMode.OpenOrCreate);
            //_writer = new StreamWriter(_logFile, Encoding.Unicode, 1048576, true); //1Mo buffer, default AutoFlush = false is faster then true
            _writer = new StreamWriter(_logFile, Encoding.Unicode, 1048576, true); //1Mo buffer, default AutoFlush = false is faster then true
        }

        public void Production(String log)
        {
            if (Level.Production <= _level) ThreadSafeLog(log);
        }

        public void Developpement(String log){
            if (Level.Developpement <= _level) ThreadSafeLog(log);
        }
    }
}