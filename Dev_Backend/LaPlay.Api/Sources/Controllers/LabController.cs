using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

//using LaPlay.Sources.Labo;

namespace LaPlay.Api.Sources.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabController : ControllerBase
    {
        //private Labo _lab;

        public LabController(){
            //_lab = new Labo(log);
        }

        // api/lab/logBench
        [HttpGet("logBench")]
        public ActionResult<long> logBench()
        {
            List<String> logs = new List<string>();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            Random r = new Random();

            for(int i = 0; i < 100000; i++){

                logs.Add(new string(Enumerable.Repeat(chars, 500).Select(s => s[r.Next(s.Length)]).ToArray()));
            }

            for(int i = 0; i < 100000; i++){

                //_log.Developpement(logs[i]);
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            System.IO.File.WriteAllLines(@"./WriteLines.txt", logs);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds / 1000;

            return elapsedMs;
        }

        // api/lab/test
        [HttpGet("drives")]
        public ActionResult<string> drives()
        {
            //return _lab.drives();
            return null;
        }

        // api/lab/bash
        [HttpGet("bash")]
        public ActionResult<string> ok()
        {
            string cmd = "lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8";

            // LaPlay.Api.Sources.Tools.Tools.Bash("lsblk -o \"NAME,MAJ:MIN,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID\"");
            //Console.WriteLine(_lab.Bash(cmd));

            //return (_lab.Bash(cmd));
            return null;
        }
    }
}