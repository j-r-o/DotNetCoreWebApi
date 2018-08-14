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
       public class LSFile
        {
            Dictionary<String, String> linuxFileTypes = new Dictionary<String, String> {{"-", "regularFile"}, {"d", "directory"}, {"c", "characterDeviceFile"}, {"b", "blockDeviceFile"}, {"s", "localSocketFile"}, {"p", "namedPipe"}, {"l", "symbolicLink"}};

            public String type;
            public Int64 bytes;
            public DateTime modifiedOn;
            public String path;

            public LSFile(String treeCommandResultLine)
            {
                Match match = Regex.Match(treeCommandResultLine, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$");

                this.type = linuxFileTypes[match.Groups[1].Value];
                this.bytes = Convert.ToInt64(match.Groups[2].Value);
                this.modifiedOn = DateTime.ParseExact(match.Groups[3].Value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                this.path = match.Groups[4].Value;
            }
        }

        private IShellContract _Shell;

        public SynchronizationAdapter(IShellContract shell)
        {
            _Shell = shell;
        }

        public List<LSFile> ListFiles(String path)
        {
            return _Shell.RunCommand("tree " + path + " -a -D -f -i -p -s --timefmt \"%F %T\"").Split('\n')
                .Where(line => line.StartsWith("["))
                .Select(file => new LSFile(file))
                .ToList();
        }

        public List<Tuple<LSFile, LSFile>> FullJoin(List<LSFile> mainFiles, List<LSFile> mirrorFiles)
        {

            var left =  from mainFile in mainFiles
                        join mirrorFile in mirrorFiles on mainFile.path.ToLower() equals mirrorFile.path.ToLower() into joinResult
                        from mi in joinResult.DefaultIfEmpty()
                        where mi == null
                        select Tuple.Create(mainFile, mi);

            var join =  from mainFile in mainFiles
                        join mirrorFile in mirrorFiles on mainFile.path.ToLower() equals mirrorFile.path.ToLower() into joinResult
                        from mi in joinResult
                        select Tuple.Create(mainFile, mi);

            var rigth = from mirrorFile in mirrorFiles
                        join mainFile in mainFiles on mirrorFile.path.ToLower() equals mainFile.path.ToLower() into joinResult
                        from ma in joinResult.DefaultIfEmpty()
                        where ma == null
                        select Tuple.Create(ma, mirrorFile);

            return left.Concat(join).Concat(rigth).ToList();
        }

        public void CopyToMirror(String mainPath, String mirrorPath, LSFile file){ _Shell.RunCommand("cp -f " + file.path + " " + mirrorPath + file.path.Substring(mainPath.Length));}
        public void RemoveFromMirror(LSFile file){ _Shell.RunCommand("rm -d " + file.path);}
        public List<LSFile> FilterNewFiles(List<Tuple<LSFile, LSFile>> comparisonResult){ return comparisonResult.Where(comparison => comparison.Item2 == null).Select(comparison => comparison.Item1).ToList();}
        public List<LSFile> FilterUpdatedFiles(List<Tuple<LSFile, LSFile>> comparisonResult){ return comparisonResult.Where(comparison => comparison.Item1?.modifiedOn > comparison.Item2?.modifiedOn).Select(comparison => comparison.Item1).ToList();}
        public List<LSFile> FilterDeletedFiles(List<Tuple<LSFile, LSFile>> comparisonResult){ return comparisonResult.Where(comparison => comparison.Item1 == null).Select(comparison => comparison.Item1).ToList();}

        public void Synchronize(String mainPath, String mirrorPath)
        {
            var comparisonResult = FullJoin(ListFiles(mainPath), ListFiles(mirrorPath));

            FilterNewFiles(comparisonResult).ForEach(file => CopyToMirror(mainPath, mirrorPath, file));
            FilterUpdatedFiles(comparisonResult).ForEach(file => CopyToMirror(mainPath, mirrorPath, file));
            FilterDeletedFiles(comparisonResult).ForEach(file => RemoveFromMirror(file));
        }

        public void a()
        {
            new Thread(() => Synchronize("/home/julien.rocco", "/tmp"));                        
        }


    }
}