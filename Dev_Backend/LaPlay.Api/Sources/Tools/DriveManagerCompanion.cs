using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;

using LaPlay.Sources.Log;

namespace LaPlay.Api.Sources.Tools
{
    public class DriveManager
    {
        private readonly ILog _logs;
        private readonly IBashRunner _bashRunner;

        public DriveManager(ILog logs, IBashRunner bashRunner)
        {
            _logs = logs;
            _bashRunner = bashRunner;
        }

        
        //list drive and partitions (-I 8 : filter MAJ)
        //lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8

        //convert command result : from spaces separated values to coma separated values
        //df -h | tr -s ' ' | cut -d ' ' -f 1- --output-delimiter "," 
        public void deletePartition(String partition)
        {

        }

        public void createMountPointForPartition(String partition, String mountFolderName)
        {

        }

        public void deleteMountPointForPartition(String partition, String mountFolderName)
        {

        }
    }
}