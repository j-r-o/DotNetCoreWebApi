using System;
using System.Collections.Generic;

namespace LaPlay.Infrastructure.Synchronization
{
    public interface ISynchronizationContract
    {
        List<String> SynchronizedFiles();
        
        List<String> NotSynchronizedFiles();

        List<String> AllFiles();
    }
}