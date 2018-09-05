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
using System.Security.Cryptography;

using LaPlay.Infrastructure.Shell;

namespace LaPlay.Infrastructure.Synchronization
{
    public class SynchronizationAdapterTest
    {
        
        [Fact]
        public void dev()
        {
            var p = RandomFileGenerator.CreateRandomFileStructure(2, 10);
            p.Sort((a,b) => a.path.CompareTo(b.path));
            p.ForEach(d => Console.WriteLine(d.type + " " + d.bytes + " " + d.modifiedOn + " " + d.path));
        }

        private static class RandomFileGenerator
        {
            public static char GenerateRandomChar() {return (char) new Random().Next(65, 91);}

            public static String GenerateRandomString(Int32 length)
            {
                return String.Concat(Enumerable.Range(0, length + 1).Select(letter => GenerateRandomChar()));
            }

            public static Char GenerateRandomFileType()
            {
                //generate a file 80% of the time 
                return "------------------------dcbspl"[new Random().Next(0, 31)];
            }

            public static DateTime GenerateRandomDateTime()
            {
                return DateTime.MinValue.Add(TimeSpan.FromTicks((Int64) Math.Round(new Random().NextDouble() * DateTime.MaxValue.Ticks)));
            }

            public static String GenerateRandomPath(Int32 depth)
            {
                return String.Join('/', Enumerable.Range(1, new Random().Next(1, depth + 1)).Select(path => GenerateRandomString(8)));
            }

            public static SynchronizationAdapter.LSFile GenerateRandomLSFile(Int32 pathDepth, Char forceFileType = '0')
            {
                String type = (forceFileType.Equals('0') ? GenerateRandomFileType() : forceFileType).ToString();
                String rigths = "rwxrwxrwx";
                String size = new Random().Next().ToString();
                String date = GenerateRandomDateTime().ToString("yyyy-MM-dd HH:mm:ss");
                String path = GenerateRandomPath(pathDepth) + (type.Equals("directory") ? "" : "/file." + type);

                return new SynchronizationAdapter.LSFile(String.Concat("[", type, rigths, " ", size, " ", date, "]  ", path));
            }

            public static List<SynchronizationAdapter.LSFile> GenerateParentDirectoriesForLSFile(SynchronizationAdapter.LSFile lsFile)
            {
                String[] pathElements = lsFile.path.Split('/');

                List<SynchronizationAdapter.LSFile> parentDirectories = new List<SynchronizationAdapter.LSFile>();

                Enumerable.Range(0, pathElements.Length - 1).ToList().ForEach(i => {

                     String treeCommandResultLine = "[drwxrwxrwx 4096 "
                                             + lsFile.modifiedOn.ToString("yyyy-MM-dd HH:mm:ss") + "]  "
                                             + String.Join('/', Enumerable.Range(0, i + 1).Select(j => pathElements[j]));

                    parentDirectories.Add(new SynchronizationAdapter.LSFile(treeCommandResultLine));
                });

                return parentDirectories;
            }
            
            public static List<SynchronizationAdapter.LSFile> CreateRandomFileStructure(Int32 deepth, Int32 fileCount)
            {
                List<SynchronizationAdapter.LSFile> lsfiles = Enumerable.Range(0, fileCount).Select(i => GenerateRandomLSFile(deepth)).ToList();

                List<SynchronizationAdapter.LSFile> parents = new List<SynchronizationAdapter.LSFile>();

                lsfiles.ForEach(lsFile => {

                    List<SynchronizationAdapter.LSFile> tmpParents = RandomFileGenerator.GenerateParentDirectoriesForLSFile(lsFile);

                    tmpParents.ForEach(tp => {
                        if(parents.Where(p => p.path == tp.path).Count() == 0)
                            parents.Add(tp);
                    });
                });

                parents.ForEach(p => lsfiles.Add(p));

                return lsfiles;
            }
        }



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
        public void ListFiles_ShouldSucceed()
        {
            List<String> expectedResult = new LinuxAdapter().RunCommand("tree /etc  -a -D -f -i -p -s --timefmt \"%F %T\"").Split("\n").ToList();
            System.Text.RegularExpressions.Match counts = Regex.Match(expectedResult.ElementAt(expectedResult.Count() - 2), "([0-9]*).*, ([0-9]*).*");
            Int32 expectedDirectoryCount = Convert.ToInt32(counts.Groups[1].Value);
            Int32 expectedFileCount = Convert.ToInt32(counts.Groups[2].Value);

            SynchronizationAdapter synchronizationAdapter = new SynchronizationAdapter(new LinuxAdapter());

            List<SynchronizationAdapter.LSFile> files = synchronizationAdapter.ListFiles("/etc");

            Assert.True(expectedDirectoryCount + expectedFileCount == files.Count());
        }
 
        [Fact]
        public void FullJoin_ShouldSucceed()
        {
            Func<String> randomPath = () => String.Join("/", Enumerable.Range(1, new Random().Next(1, 10)).Select(i => (char) new Random().Next(65,90)));
            Func<SynchronizationAdapter.LSFile> randomPathLSFile = () => new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  " + randomPath.Invoke());

            List<SynchronizationAdapter.LSFile> mainFiles = new List<SynchronizationAdapter.LSFile>();
            List<SynchronizationAdapter.LSFile> mirrorFiles = new List<SynchronizationAdapter.LSFile>();

            List<SynchronizationAdapter.LSFile> leftFilesOnly = Enumerable.Range(1, new Random().Next(1, 10)).Select(i => randomPathLSFile.Invoke()).ToList();
            List<SynchronizationAdapter.LSFile> joinFilesOnly = Enumerable.Range(1, new Random().Next(1, 10)).Select(i => randomPathLSFile.Invoke()).ToList();
            List<SynchronizationAdapter.LSFile> rigthFilesOnly = Enumerable.Range(1, new Random().Next(1, 10)).Select(i => randomPathLSFile.Invoke()).ToList();

            leftFilesOnly.ForEach(file => mainFiles.Add(file));
            joinFilesOnly.ForEach(file => mainFiles.Add(file));
            joinFilesOnly.ForEach(file => mirrorFiles.Add(file));
            rigthFilesOnly.ForEach(file => mirrorFiles.Add(file));

            List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>> fullJoinResult = new SynchronizationAdapter(null).FullJoin(mainFiles, mirrorFiles);

            Assert.True(fullJoinResult.Count() == leftFilesOnly.Count() + joinFilesOnly.Count() + rigthFilesOnly.Count());

            Assert.True(fullJoinResult
                        .Where(line => line.Item1 != null && line.Item2 == null)
                        .Select(line => line.Item1.path)
                        .Intersect(leftFilesOnly.Select(file => file.path))
                        .Count() == leftFilesOnly.Count());

            Assert.True(fullJoinResult
                        .Where(line => line.Item1 != null && line.Item2 != null)
                        .Select(line => line.Item1.path)
                        .Intersect(joinFilesOnly.Select(file => file.path))
                        .Count() == joinFilesOnly.Count());

            Assert.True(fullJoinResult
                        .Where(line => line.Item1 == null && line.Item2 != null)
                        .Select(line => line.Item2.path)
                        .Intersect(rigthFilesOnly.Select(file => file.path))
                        .Count() == rigthFilesOnly.Count());
        }

        [Fact]
        public void CopyToMirror_ShouldSucceed()
        {
            Func<String, byte[]> SHA512 = (b) => SHA256Managed.Create().ComputeHash(File.ReadAllBytes(b));

            String map = "/tmp/a1";
            String maf = map + "/file.txt";
            String mip = "/tmp/a2";

            Directory.CreateDirectory(map);

            using(StreamWriter debug = new StreamWriter(maf, false))
            {
                debug.WriteLine(Guid.NewGuid().ToString());
            }

            SynchronizationAdapter.LSFile file = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  " + maf);

            new SynchronizationAdapter(new LinuxAdapter()).CopyToMirror(map, mip, file);

            Assert.True(Enumerable.SequenceEqual(SHA512.Invoke(map + "/file.txt"), SHA512.Invoke(mip + "/file.txt")));

            Directory.Delete(map, true);
            Directory.Delete(mip, true);
        }

        [Fact]
        public void DeleteFileOrDirectory_ShouldSucceed()
        {
            String map = "/tmp/LaPlayTest/a";
            String maf = map + "/file.txt";

            Directory.CreateDirectory(map);

            using(StreamWriter debug = new StreamWriter(maf, false))
            {
                debug.WriteLine(Guid.NewGuid().ToString());
            }

            new SynchronizationAdapter(new LinuxAdapter()).DeleteFileOrDirectory(map);

            Assert.True(Directory.Exists(map) == false);
        }

        [Fact]
        public void FilterNewFiles_ShouldSucceed()
        {
            List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>> mockedComparisonResult = new List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>>();

            SynchronizationAdapter.LSFile dir = new SynchronizationAdapter.LSFile("[d--------- 4096 0001-01-01 00:00:00]  /tmp/dir");
            SynchronizationAdapter.LSFile leftOnlyFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftOnlyFile");
            SynchronizationAdapter.LSFile nullFile = null;
            SynchronizationAdapter.LSFile leftAndRightSameDateFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftAndRightSameDateFile");
            SynchronizationAdapter.LSFile leftUpdatedFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:01]  /tmp/dir/LeftAndRightUpdatedFile");
            SynchronizationAdapter.LSFile rightUpdatedFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftAndRightUpdatedFile");
            SynchronizationAdapter.LSFile rigthOnlyFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:01]  /tmp/dir/RigthOnlyFile");

            mockedComparisonResult.Add(Tuple.Create(dir, dir));
            mockedComparisonResult.Add(Tuple.Create(leftOnlyFile, nullFile));
            mockedComparisonResult.Add(Tuple.Create(leftAndRightSameDateFile, leftAndRightSameDateFile));
            mockedComparisonResult.Add(Tuple.Create(leftUpdatedFile, rightUpdatedFile));
            mockedComparisonResult.Add(Tuple.Create(nullFile, rigthOnlyFile));

            List<SynchronizationAdapter.LSFile> newFiles = new SynchronizationAdapter(new LinuxAdapter()).FilterNewFiles(mockedComparisonResult);

            Assert.True(newFiles.Count == 1);
            Assert.True(newFiles.Contains(leftOnlyFile));
        }

        [Fact]
        public void FilterUpdatedFiles_ShouldSucceed()
        {
            List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>> mockedComparisonResult = new List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>>();

            SynchronizationAdapter.LSFile dir = new SynchronizationAdapter.LSFile("[d--------- 4096 0001-01-01 00:00:00]  /tmp/dir");
            SynchronizationAdapter.LSFile leftOnlyFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftOnlyFile");
            SynchronizationAdapter.LSFile nullFile = null;
            SynchronizationAdapter.LSFile leftAndRightSameDateFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftAndRightSameDateFile");
            SynchronizationAdapter.LSFile leftUpdatedFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:01]  /tmp/dir/LeftAndRightUpdatedFile");
            SynchronizationAdapter.LSFile rightUpdatedFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftAndRightUpdatedFile");
            SynchronizationAdapter.LSFile rigthOnlyFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:01]  /tmp/dir/RigthOnlyFile");

            mockedComparisonResult.Add(Tuple.Create(dir, dir));
            mockedComparisonResult.Add(Tuple.Create(leftOnlyFile, nullFile));
            mockedComparisonResult.Add(Tuple.Create(leftAndRightSameDateFile, leftAndRightSameDateFile));
            mockedComparisonResult.Add(Tuple.Create(leftUpdatedFile, rightUpdatedFile));
            mockedComparisonResult.Add(Tuple.Create(nullFile, rigthOnlyFile));

            List<SynchronizationAdapter.LSFile> updatedFiles = new SynchronizationAdapter(new LinuxAdapter()).FilterUpdatedFiles(mockedComparisonResult);

            Assert.True(updatedFiles.Count == 1);
            Assert.True(updatedFiles.Contains(leftUpdatedFile));
        }

        [Fact]
        public void FilterDeletedFiles_ShouldSucceed()
        {
            List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>> mockedComparisonResult = new List<Tuple<SynchronizationAdapter.LSFile, SynchronizationAdapter.LSFile>>();

            SynchronizationAdapter.LSFile dir = new SynchronizationAdapter.LSFile("[d--------- 4096 0001-01-01 00:00:00]  /tmp/dir");
            SynchronizationAdapter.LSFile leftOnlyFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftOnlyFile");
            SynchronizationAdapter.LSFile nullFile = null;
            SynchronizationAdapter.LSFile leftAndRightSameDateFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftAndRightSameDateFile");
            SynchronizationAdapter.LSFile leftUpdatedFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:01]  /tmp/dir/LeftAndRightUpdatedFile");
            SynchronizationAdapter.LSFile rightUpdatedFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:00]  /tmp/dir/LeftAndRightUpdatedFile");
            SynchronizationAdapter.LSFile rigthOnlyFile = new SynchronizationAdapter.LSFile("[---------- 1024 0001-01-01 00:00:01]  /tmp/dir/RigthOnlyFile");

            mockedComparisonResult.Add(Tuple.Create(dir, dir));
            mockedComparisonResult.Add(Tuple.Create(leftOnlyFile, nullFile));
            mockedComparisonResult.Add(Tuple.Create(leftAndRightSameDateFile, leftAndRightSameDateFile));
            mockedComparisonResult.Add(Tuple.Create(leftUpdatedFile, rightUpdatedFile));
            mockedComparisonResult.Add(Tuple.Create(nullFile, rigthOnlyFile));

            List<SynchronizationAdapter.LSFile> deletedFiles = new SynchronizationAdapter(new LinuxAdapter()).FilterDeletedFiles(mockedComparisonResult);

            Assert.True(deletedFiles.Count == 1);
            Assert.True(deletedFiles.Contains(rigthOnlyFile));
        }

        [Fact]
        public void Synchronize_ShouldSucceedWithAnEmptyDirectory()
        {
            IShellContract shell = new LinuxAdapter();
            shell.RunCommand("mkdir -p /tmp/LaPlayTest/a/anEmptyDirectory");
            shell.RunCommand("mkdir -p /tmp/LaPlayTest/b");

            new SynchronizationAdapter(new LinuxAdapter()).Synchronize("/tmp/LaPlayTest/a", "/tmp/LaPlayTest/b");

            Assert.True(File.Exists("/tmp/LaPlayTest/b/anEmptyDirectory"));

            shell.RunCommand("rm -r /tmp/LaPlayTest");
        }

        public void Synchronize_ShouldSucceedWithAnyFileType()
        {
        }

        public void Synchronize_ShouldSucceedWithADeletedFile()
        {
        }

        public void Synchronize_ShouldSucceedWithANewFileStructure()
        {
        }

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