using System;
using System.Collections;
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
            LinuxAdapter linuxAdapter = new LinuxAdapter();
            linuxAdapter.ListFiles("/home/julien.rocco");
        }

        [Fact]
        public void RunNeverEndingCommand_ShouldSucceed()
        {
            LinuxAdapter linuxAdapter = new LinuxAdapter();
            
            List<FileInfo> a = linuxAdapter.ListFiles("/home/julien.rocco");

            String sj = JsonConvert.SerializeObject(a);

            using(StreamWriter s = new StreamWriter("sj.txt", false))
            {
                s.Write(sj);
            }

            
        }
    }
}