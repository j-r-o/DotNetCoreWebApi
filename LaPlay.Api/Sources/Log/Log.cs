using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LaPlay.Api.Sources.Log
{
    public class Log : ILog
    {
        private FileStream _logFile;
        private StreamWriter _writer;

        public Log()
        {
            _logFile = File.Open(@"log.txt", FileMode.Append, FileAccess.Write);
            _writer = new StreamWriter(_logFile, Encoding.UTF8, 1024, true);
        }

        public void info(String log){
            _writer.Write(log);
        }

        public void warning(String log){
            _writer.Write(log);
        }

        public void debug(String log){
            _writer.Write(log);
        }
    }
}