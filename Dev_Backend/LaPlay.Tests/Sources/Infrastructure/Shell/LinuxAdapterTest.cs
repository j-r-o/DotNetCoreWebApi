using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;
using Xunit;
using System.Text.RegularExpressions;

namespace LaPlay.Infrastructure.Shell
{
    public class LinuxAdapterTest
    {

        [Fact]
        public void RunCommand_ShouldSucceed()
        {
            LinuxAdapter linuxAdapter = new LinuxAdapter();

            String process = linuxAdapter.RunCommand("tree /media/sf_D_DRIVE/VM -a -f -i -D -F -s --timefmt \"%F %T\"");

            String[] lines = process.Split('\n');

            List<String> lst = lines.OfType<String>().ToList();

            var json = from l in lines
                       select new { file = match.Groups[3].Value};


            FileStream fsb = new FileStream("b.txt", FileMode.Create);
            using(TextWriter tw = new StreamWriter(fsb))
            {
                foreach(String line in lines)
                {
                    var match = Regex.Match(line, "^\\[ *([0-9]*) (....-..-.. ..:..:..)]  (.*)$");
                        
                    if(match.Success)
                    {
                        string b = "{\"File\":\"" + match.Groups[3].Value + "\", \"ModifiedOn\":\"" + match.Groups[2].Value + "\", \"Bytes\":" + match.Groups[1].Value + "}";
                        tw.WriteLine(b);
                    }
                    //else tw.Write("!!! " + line);
                }
            }

            Console.WriteLine(lines.Length);
            Console.WriteLine("#");
            Console.WriteLine(process);
        }

        public void RunNeverEndingCommand_ShouldSucceed()
        {
            LinuxAdapter linuxAdapter = new LinuxAdapter();

            //Process process linuxAdapter.RunNeverEndingCommand("");

            
        }
    }
}