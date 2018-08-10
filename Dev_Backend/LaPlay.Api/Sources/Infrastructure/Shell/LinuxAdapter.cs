using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

using Newtonsoft.Json;

namespace LaPlay.Infrastructure.Shell
{
    public class LinuxAdapter : IShellContract
    {
        public string RunCommand(String command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        public Process RunNeverEndingCommand(String command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            
            return process;
        }

        public List<FileInfo> ListFiles(String path)
        {
            Dictionary<String, String> linuxFileTypes = new Dictionary<String, String> {{"-", "regularFile"}, {"d", "directory"}, {"c", "characterDeviceFile"}, {"b", "blockDeviceFile"}, {"s", "localSocketFile"}, {"p", "namedPipe"}, {"l", "symbolicLink"}};

            RunCommand("tree " + path + " -a -D -f -i -p -s --timefmt \"%F %T\"").Split('\n')
            .Where(line => ((String)line).StartsWith("["))
            .Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$"))
            .Select(matche => new System.IO.FileInfo(matche.Groups[4].Value));

            //=========================
            var treeCommandWatch = System.Diagnostics.Stopwatch.StartNew();
            
            List<String> files = RunCommand("tree " + path + " -a -D -f -i -p -s --timefmt \"%F %T\"").Split('\n').Where(line => ((String)line).StartsWith("[")).ToList();
            
            treeCommandWatch.Stop();
            Console.WriteLine("#" + treeCommandWatch.ElapsedMilliseconds + "#");

            //=========================
            var regexWatch = System.Diagnostics.Stopwatch.StartNew();

            List<Match> matches = new List<Match>(files.Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$")));

            regexWatch.Stop();
            Console.WriteLine("#" + regexWatch.ElapsedMilliseconds + "#");

            //=========================
            var fileInfoWatch = System.Diagnostics.Stopwatch.StartNew();

            List<FileInfo> fileInfos = new List<FileInfo>(matches.Select(matche => new System.IO.FileInfo(matche.Groups[4].Value)));

            fileInfoWatch.Stop();
            Console.WriteLine("#" + fileInfoWatch.ElapsedMilliseconds + "#");
            
            //=========================
            var allInOneWatch = System.Diagnostics.Stopwatch.StartNew();

            List<FileInfo> res = new List<FileInfo> (files
                .Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$"))
                .Select(matche => new System.IO.FileInfo(matche.Groups[4].Value)));

            allInOneWatch.Stop();
            Console.WriteLine("#" + allInOneWatch.ElapsedMilliseconds + "#");

            Console.WriteLine(">> " + files.Count + "#");
           
            return res;

            // return new List<dynamic>(
            //     files
            //     .Select(file => Regex.Match(file, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$"))
            //     .Select(matches => new
            //     {
            //         path = matches.Groups[4].Value,
            //         type = matches.Groups[2].Value,
            //         modifiedOn = DateTime.ParseExact(matches.Groups[3].Value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
            //         bytes = linuxFileTypes[matches.Groups[1].Value]
                    
            //     }));
            
            /*
            String sj = JsonConvert.SerializeObject(a);

            using(StreamWriter s = new StreamWriter("sj.txt", false))
            {
                s.Write(sj);
            }

            lines2.GroupBy(l => l.type).Count();
            Console.WriteLine("");

            String json = JsonConvert.SerializeObject(lines2);

            using(StreamWriter fsb2 = new StreamWriter("b2.txt", false))
            {
                fsb2.Write(json);
            }
            
            Console.WriteLine(lines2.Count());
            Console.WriteLine("#");
             */
        }
    }

    // inotifywait -mr -e modify,attrib,moved_to,moved_from,create,delete --format '{"f":"%w", "f":"%f", "f":"%e", "t":"%T"}' --timefmt '%F %T' /tmp

}