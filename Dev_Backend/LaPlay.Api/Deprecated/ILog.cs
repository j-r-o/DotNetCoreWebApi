using System;
using System.Collections.Generic;

namespace Deprecated
{
    public interface ILog
    {
        void Production(String log);
        void Developpement(String log);
    }
}