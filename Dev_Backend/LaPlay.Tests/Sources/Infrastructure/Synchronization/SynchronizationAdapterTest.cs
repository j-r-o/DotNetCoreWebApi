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
        public void LSFile_ShouldConstructWithTreeCommandResultLine()
        {
            List<dynamic> files = new List<dynamic> {
                new {expectedIdenfication = "regularFile", typeChar="-", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"},
                new {expectedIdenfication = "directory", typeChar="d", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"},
                new {expectedIdenfication = "characterDeviceFile", typeChar="c", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"},
                new {expectedIdenfication = "blockDeviceFile", typeChar="b", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"},
                new {expectedIdenfication = "localSocketFile", typeChar="s", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"},
                new {expectedIdenfication = "namedPipe", typeChar="p", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"},
                new {expectedIdenfication = "symbolicLink", typeChar="l", bytes = new Random().Next(), date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), path = "/home/user/desktop"}
            };

            var filesWithLSFiles = files.Select(file => new{file = file, lsFile = new SynchronizationAdapter.LSFile("[" + file.typeChar + "rwxrwxrwx      " + file.bytes + " " + file.date + "]  " + file.path + "\n")});
            
            filesWithLSFiles.ToList().ForEach(file => {
                Assert.True(file.file.expectedIdenfication == file.lsFile.type);
                Assert.True(file.file.bytes == file.lsFile.bytes);
                Assert.True(file.file.date == file.lsFile.modifiedOn.ToString("yyyy-MM-dd HH:mm:ss"));
                Assert.True(file.file.path == file.lsFile.path);
            });
        }



        [Fact]
        public void Synchronize_ShouldSucceed()
        {

            Mock<IShellContract> mockedShellContract = new Mock<IShellContract>();
            
            mockedShellContract.Setup(m => m.RunCommand(It.IsAny<String>())).Returns(
                "/tmp\n" +
                "[drwxrwxr-x        4096 2018-08-14]  /tmp/a\n" +
                "[drwxrwxr-x       20480 2018-08-14]  /tmp/appInsights-node\n" +
                "[srwxrwxrwx           0 2018-08-13]  /tmp/.X11-unix/X0\n" +
                "[-rw-------         410 2018-08-13]  /tmp/.xfsm-ICE-9C4VNZ\n" +
                "[drwxrwxrwt        4096 2018-08-13]  /tmp/.XIM-unix\n" +
                "[drwxrwxr-x        4096 2018-08-14]  /tmp/Z\n" +
                "[-rw-rw-r--           0 2018-08-14]  /tmp/Z/a\n" +
                "[-rw-rw-r--           0 2018-08-14]  /tmp/Z/b\n" +
                "[-rw-rw-r--       69632 2018-08-14]  /tmp/Z/tree.txt\n" +
                "%T [error opening dir]\n" +
                "\n" +
                "31 directories, 728 files\n" +
                ""
            );

            SynchronizationAdapter synchronizationAdapter = new SynchronizationAdapter(mockedShellContract.Object);

            synchronizationAdapter.ListFiles("");

            synchronizationAdapter.Synchronize("/media/sf_D_DRIVE/Apps/7-Zip/", "/media/sf_D_DRIVE/Apps/Audacity/");

            /*
            String sj = JsonConvert.SerializeObject(fullJoin);

            using(StreamWriter s = new StreamWriter("sj.txt", false))
            {
                foreach(var b in fullJoin)
                {
                    s.WriteLine(b.Item1?.Item1.ToString() + " | " + b.Item2?.Item1.ToString());
                    s.WriteLine(b.Item1?.Item2.ToString() + " | " + b.Item2?.Item2.ToString());
                    s.WriteLine(b.Item1?.Item3.ToString() + " | " + b.Item2?.Item3.ToString());
                    s.WriteLine(b.Item1?.Item4.ToString() + " | " + b.Item2?.Item4.ToString());
                }
            }
            */
        }
    }
}