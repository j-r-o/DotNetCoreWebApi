using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Diagnostics;

//using LaPlay.Sources.Log;

namespace LaPlay.Tools
{
    public class DriveManagerCompanion
    {
        //private readonly ILog _logs;
        private readonly IBashRunner _bashRunner;

        public DriveManagerCompanion(/*ILog logs, */IBashRunner bashRunner)
        {
            //_logs = logs;
            _bashRunner = bashRunner;
        }

        public String listBlockDevices()
        {
            //list drives and partitions (-J : json output, -I 8 : disks and partitions only)
            String blockDevicesAsJson = _bashRunner.RunCommand("lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8");
            //dynamic data = Json.Decode(blockDevicesAsJson);
            return "";
        }

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

        public void createPartitionMountPoint(String partitionLabel, String mountFolderName)
        {
            //#ajouter à /etc/fstab avec le paramettre nofail pour que ca marche meme sans le disk
    		//# voir man fstab
    		//"LABEL=" + partitionLabel + "    /mnt/" + mountFolderName + "    ntfs-3g    default,nofail    0    2"
        }

        public void listPartitionMountPoints()
        {

        }

        public void deletePartitionMountPoint(String partitionLabel)
        {
            //targetFile="/home/julien.rocco/desktop/fstab"
    		//lineToDelete=$(grep -n "partitionLabel" $targetFile | cut -d : -f1)
    		//echo $lineToDelete
    		//#sed -e $lineToDelete'd' $targetFile
    		//sed -i -e 's/.*81164c31-2b99-4283-a5d9-2a942dd65e71.*//' $targetFile
        }
    }
}
