using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;

using LaPlay.Sources.Log;

namespace LaPlay.Api.Sources.Tools
{
    public class DriveManagerCompanion
    {
        private readonly ILog _logs;
        private readonly IBashRunner _bashRunner;

        public DriveManagerCompanion(ILog logs, IBashRunner bashRunner)
        {
            _logs = logs;
            _bashRunner = bashRunner;
        }

        
        //list drive and partitions (-I 8 : filter MAJ)
        //lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8

        //convert command result : from spaces separated values to coma separated values
        //df -h | tr -s ' ' | cut -d ' ' -f 1- --output-delimiter "," 



        public void createPartition(String drive)
        {
            
        }

        public void listPartitions()
        {

        }

        public void deletePartition(String partition)
        {

        }

        public void createPartitionMountPoint(String partition, String mountFolderName)
        {

        }

        public void listPartitionMountPoints()
        {

        }

        public void deletePartitionMountPoint(String partition, String mountFolderName)
        {

        }
    }
}