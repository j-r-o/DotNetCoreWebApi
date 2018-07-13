using Xunit;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Moq;

namespace LaPlay.Tools
{
    public class DriveManagerCompanionTest
    {
        //dotnet test --filter "FullyQualifiedName=LaPlay.Api.Sources.Tools.DriveManagerCompanionTest.deletePartition_shouldSucceed"
        [Fact]
        public void deletePartition_shouldSucceed()
        {
            //Log log = new Log(Log.Level.Developpement);
            //BashRunner bashRunner = new BashRunner();

            //DriveManagerCompanion driveManagerCompanion = new DriveManagerCompanion(log, bashRunner);
            //driveManagerCompanion.deletePartition("sdb");
        }
    }
}