using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;

using Xunit;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Moq;

namespace LaPlay.Infrastructure.Synchronization
{
    public class SynchronizationAdapterTest
    {
        [Fact]
        public void RunNeverEndingCommand_ShouldSucceed()
        {
            SynchronizationAdapter synchronizationAdapter = new SynchronizationAdapter();

            synchronizationAdapter.ListFiles("/home/julien.rocco/EclipseWorkspace/connect-resources-backend_INSTABLE");
        }
    }
}