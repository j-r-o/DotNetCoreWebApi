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

using LaPlay.Infrastructure.Shell;

namespace LaPlay.Infrastructure.Synchronization
{
    public class SynchronizationAdapterTest
    {
        [Fact]
        public void Synchronize_ShouldSucceed()
        {
            SynchronizationAdapter synchronizationAdapter = new SynchronizationAdapter(new LinuxAdapter());

            synchronizationAdapter.Synchronize("/media/sf_D_DRIVE/Apps/7-Zip/", "/media/sf_D_DRIVE/Apps/Audacity/");
        }
    }
}