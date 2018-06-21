using System;
using System.Collections.Generic;

namespace LaPlay.Api.Sources.Log
{
    public interface ILog
    {
        void info(String log);
        void warning(String log);
        void debug(String log);
    }
}