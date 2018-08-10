using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using LaPlay.Infrastructure.Shell;
using Newtonsoft.Json;

namespace LaPlay.Infrastructure.Synchronization
{
    public class SynchronizationAdapter// : ISynchronizationContract
    {
        private IShellContract _Shell;

        public SynchronizationAdapter(IShellContract shell)
        {
            _Shell = shell;
        }

        public void ListFiles1(String location)
        { 
            var files = from file in Directory.EnumerateFileSystemEntries(location, "*.*", SearchOption.AllDirectories)
                        from line in File.ReadLines(file)
                        select new
                        {
                            File = file,
                            Line = line
                        };

             try{
                Console.WriteLine("{0} files found.", files.Count().ToString());
            }
            finally{}

            foreach (var f in files)
            {
                try{
                Console.WriteLine("{0}\t{1}", f.File, f.Line);
                }
                finally{}
            }
        }

        public List<Tuple<String, Int64, DateTime, String, String>> ListFiles(String path)
        {
            Dictionary<String, String> linuxFileTypes = new Dictionary<String, String> {{"-", "regularFile"}, {"d", "directory"}, {"c", "characterDeviceFile"}, {"b", "blockDeviceFile"}, {"s", "localSocketFile"}, {"p", "namedPipe"}, {"l", "symbolicLink"}};

            return _Shell.RunCommand("tree " + path + " -a -D -f -i -p -s --timefmt \"%F %T\"").Split('\n')
                .Where(line => line.StartsWith("["))
                .Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$"))
                .Select(match => Tuple.Create(
                    linuxFileTypes[match.Groups[1].Value],
                    Convert.ToInt64(match.Groups[2].Value),
                    DateTime.ParseExact(match.Groups[3].Value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                    match.Groups[4].Value,
                    match.Groups[4].Value.Substring(path.Length))
                ).ToList();
        }

        public void Synchronize(String mainPath, String mirrorPath)
        {
            //liste 1 :  mainPath | mainSize | mainModificationDate | #NotInMirror
            //liste 2 :  mirrorPath | mirrorSize | mirrorModificationDate | #NotInMain | #OutDated

            //copy/replace files from list 1 which are not in mirror or outdated
            //delete files from list 2 which are not in main

            List<Tuple<String, Int64, DateTime, String, String>> mainFiles = ListFiles(mainPath);
            List<Tuple<String, Int64, DateTime, String, String>> mirrorFiles = ListFiles(mirrorPath);


            Stack<Tuple<String, Int64, DateTime, String, String>> mainFilesStack = new Stack<Tuple<String, Int64, DateTime, String, String>>(ListFiles(mainPath));
            Stack<Tuple<String, Int64, DateTime, String, String>> mirrorFilesStack = new Stack<Tuple<String, Int64, DateTime, String, String>>(ListFiles(mirrorPath));

            Tuple<String, Int64, DateTime, String, String> mainFile;
            Tuple<String, Int64, DateTime, String, String> mirrorFile;

            List<Tuple<Tuple<String, Int64, DateTime, String, String>, Tuple<String, Int64, DateTime, String, String>>> fullJoin = new List<Tuple<Tuple<String, Int64, DateTime, String, String>, Tuple<String, Int64, DateTime, String, String>>>();

            while(mainFilesStack.Any() || mirrorFilesStack.Any())
            {
                mainFile = (String.Compare(mainFilesStack.FirstOrDefault()?.Item5, mirrorFilesStack.FirstOrDefault()?.Item5) <= 0) ? mainFilesStack.Pop() : null;
                mirrorFile = (String.Compare(mirrorFilesStack.FirstOrDefault()?.Item5, mirrorFilesStack.FirstOrDefault()?.Item5) <= 0) ? mirrorFilesStack.Pop() : null;

                fullJoin.Add(Tuple.Create(mainFile, mirrorFile));
            }


            /*



            /a/a/b.txt              /a/a/f.txt
            /a/a/f.txt              /a/a/b.txt
            /a/a/b.txt              /a/a/f.txt





            var left =  from mainFile in mainFiles
                        join mirrorFile in mirrorFiles on mainFile.Item5.ToLower() equals mirrorFile.Item5.ToLower() into joinResult
                        from mi in joinResult.DefaultIfEmpty()
                        where mi == null
                        select Tuple.Create(mainFile, mi);

            var join =  from mainFile in mainFiles
                        join mirrorFile in mirrorFiles on mainFile.Item5.ToLower() equals mirrorFile.Item5.ToLower() into joinResult
                        from mi in joinResult
                        select Tuple.Create(mainFile, mi);

            var rigth = from mirrorFile in mirrorFiles
                        join mainFile in mainFiles on mirrorFile.Item5.ToLower() equals mainFile.Item5.ToLower() into joinResult
                        from ma in joinResult.DefaultIfEmpty()
                        where ma == null
                        select Tuple.Create(ma, mirrorFile);

            var fullJoin = left.Concat(join).Concat(rigth);
            */

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
        }

        public void a()
        {
            new Thread(() => Synchronize("/home/julien.rocco", "/tmp"));                        
        }


    }
}