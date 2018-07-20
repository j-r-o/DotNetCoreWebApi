using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;

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
    }

    // inotifywait -mr -e modify,attrib,moved_to,moved_from,create,delete --format '{"f":"%w", "f":"%f", "f":"%e", "t":"%T"}' --timefmt '%F %T' /tmp

    // dd if=/dev/urandom of=1GB.bin bs=64M count=16 iflag=fullblock

    // lsyncd -delay 0 -nodaemon -rsync /tmp/rsync/a /tmp/rsync/b

    // cat * > merged-file2
}