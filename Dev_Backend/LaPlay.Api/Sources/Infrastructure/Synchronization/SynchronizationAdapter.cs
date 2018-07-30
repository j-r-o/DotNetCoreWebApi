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
        public void ListFiles(String location)
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
    }
}