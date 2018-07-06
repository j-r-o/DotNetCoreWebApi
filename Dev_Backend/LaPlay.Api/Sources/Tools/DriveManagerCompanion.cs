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

        /// <summary>
        /// 
        /// </summary>
        public void deletePartition(String partition)
        {
            //Parameter must match "sd[a-z][1-9]"

            //unmount all partitions of drive
            _bashRunner.RunCommand("sudo umount -f /dev/" + partition);

            // this simulates manual inputs to fdisk
            // A blank line (commented as "default" will send a empty line terminated with a newline to take the fdisk default.
            _bashRunner.RunCommand("("
                                  +"echo o # clear the in memory partition table"
                                  +"echo w # write the partition table and quit"
                                  +"echo q # quit"
                                  +") | sudo fdisk /dev/" + partition);
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