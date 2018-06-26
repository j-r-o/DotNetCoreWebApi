using System;
using System.Collections.Generic;

namespace LaPlay.Sources.Log
{
    public interface ILog
    {
        void Production(String log);
        void Developpement(String log);
    }
}