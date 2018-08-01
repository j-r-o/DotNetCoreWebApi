using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;
using Xunit;
using System.Text.RegularExpressions;
using System.Linq;

using Newtonsoft.Json;



namespace LaPlay.Infrastructure.Shell
{
    public class LinuxAdapterTest
    {

        [Fact]
        public void RunCommand_ShouldSucceed()
        {
            Func<String, String> FileType = typeChar =>
            {
                if (typeChar.Equals("-")) return "regularFile";
                if (typeChar.Equals("d")) return "directory";
                if (typeChar.Equals("c")) return "characterDeviceFile";
                if (typeChar.Equals("b")) return "blockDeviceFile";
                if (typeChar.Equals("s")) return "localSocketFile";
                if (typeChar.Equals("p")) return "namedPipe";
                if (typeChar.Equals("l")) return "symbolicLink";

                throw new Exception("Unexpected file type");
            };

            LinuxAdapter linuxAdapter = new LinuxAdapter();

            String process = linuxAdapter.RunCommand("tree /home/julien.rocco -a -D -f -i -p -s --timefmt \"%F %T\"");

            var lines2 = process.Split('\n').Select(l => {
                var match = Regex.Match(l, "^\\[(.)......... *([0-9]*) (....-..-.. ..:..:..)]  (.*)$");
                if(match.Success)
                {
                    return new {path = match.Groups[4].Value,
                                type = FileType.Invoke(match.Groups[1].Value),
                                modifiedOn = DateTime.ParseExact(match.Groups[3].Value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                                modifiedOnS = match.Groups[3].Value,
                                bytes = match.Groups[2].Value};
                }
                return null;
            }).Where(d => d != null);

            String json = JsonConvert.SerializeObject(lines2);

            using(StreamWriter fsb2 = new StreamWriter("b2.txt", false))
            {
                fsb2.Write(json);
            }
            
            Console.WriteLine(lines2.Count());
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