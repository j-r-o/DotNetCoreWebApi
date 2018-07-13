using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;

namespace LaPlay.Infrastructure.Shell
{
    public class DriveManager
    {
        public void listFreeSpaces()
        {
            //return new {Drive = "/dev/sda", FreeSpace = 12345698};
        }

        public void createPartitionWithFreeSpace(String drive)
        {
            //String createPartitionWithFreeSpace = "";

            //String bashResult = _bashRunner.RunCommand(createPartitionWithFreeSpace);

        }


        //list drive and partitions (-I 8 : filter MAJ)
        //lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8
        public void deletePartition(String partition)
        {

        }

        public void createMountPointForPartition(String partitionLabel, String mountFolderName)
        {
            

        }

        public void deleteMountPointForPartition(String partition, String mountFolderName)
        {

        }
    }
}
