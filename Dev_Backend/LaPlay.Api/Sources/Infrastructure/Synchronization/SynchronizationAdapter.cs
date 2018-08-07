using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace LaPlay.Infrastructure.Synchronization
{
    public class SynchronizationAdapter// : ISynchronizationContract
    {
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

        public String[,] ListFiles(String path)
        {
            Dictionary<String, String> linuxFileTypes = new Dictionary<String, String> {{"-", "regularFile"}, {"d", "directory"}, {"c", "characterDeviceFile"}, {"b", "blockDeviceFile"}, {"s", "localSocketFile"}, {"p", "namedPipe"}, {"l", "symbolicLink"}};

            RunCommand("tree " + path + " -a -D -f -i -p -s --timefmt \"%F %T\"").Split('\n')
            .Where(line => ((String)line).StartsWith("["))
            .Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$"))
            .Select(matche => new System.IO.FileInfo(matche.Groups[4].Value))
            .Where(f => f)
            );

            String[,] aeaze;

            aeaze.


            RunCommand("tree " + path + " -a -D -f -i -p -s --timefmt \"%F %T\"").Split('\n')
                .Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$"))
                .Select(matches => new
                {
                    path = matches.Groups[4].Value,
                    type = matches.Groups[2].Value,
                    modifiedOn = DateTime.ParseExact(matches.Groups[3].Value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                    bytes = linuxFileTypes[matches.Groups[1].Value]
                    
                }));
    }
}