#!/bin/bash

#function myfunc()
#{
#    local  myresult='some value'
#    echo "$myresult"
#}

#result=$(myfunc)   # or result=`myfunc`
#echo $result



#================================   HELPERS   =================================

	listDiskPaths() {

		echo lsblk | grep "disk" | cut -d " " -f1
	}

	countDiskPartitions() {

		disk="$1"

		partitionCount=$(lsblk | grep "part" | cut -d " " -f1 | grep -c "$disk")

	}

#==============================   VALIDATORS   ================================

	drivePathMustBeReal() {

		drivePath="$1"

		#Return 1 if drivePath is in diskPaths, 0 otherwise
		echo $(listDiskPaths | grep -c -e "$drivePath")
	}

	driveMustHaveNoPartition() {

		echo "driveMustHaveNoPartition"
	}


#=================================   CORE    ==================================

	createPartitionOnDrive() {

		#Parameter must be somthing like "sd[a-z]"
		drivePath=$1

		#if [ $(drivePathMustBeReal $drivePath) -ne 1 ]
		#then
		#    echo ERROR: $drivePath' is not a valid drive path'
		#    exit 1 # terminate and indicate error
		#fi

		drive=$(echo $drivePath | grep -o 'sd[a-z]$')
		partitionCount=$(grep -c -o $drive'[0-9]' /proc/partitions)
		if [ $partitionCount -ne 0 ]
		then
		    echo ERROR: $drivePath' has '$partitionCount' partitions. Delete them first before partitionning this drive'
		    exit 1 # terminate and indicate error
		fi

		# this simulates manual inputs to fdisk
		# A blank line (commented as "default" will send a empty line terminated with a newline to take the fdisk default.
		(
			echo o # clear partition table and create a new empty DOS partition table
			echo n # new partition
			echo p # primary partition
			echo   # default - partition number 1
			echo   # default - start at beginning of disk
			echo   # default - finish at end of disk
			echo w # write the partition table and quit
			echo q # quit"
		) | fdisk "/dev/$drivePath"
		#) | sudo fdisk $drivePath

		#Format new partition (/dev/sd.1) as ntfs.
		newPartitionPath="/dev/"$drivePath"1"
		mkfs.ntfs $newPartitionPath --fast --verbose --with-uuid -L "TheLabel"

	}

	listPartitions() {

		echo $(drivePathMustBeReal /dev/sda)

		#lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8
	}

	deletePartitionsOnDrive() {

		#Parameter must match "sd[a-z][1-9]"
		drivePath=$1

		#unmount all partitions of drive
		umount "/dev/"$1

		# this simulates manual inputs to fdisk
		# A blank line (commented as "default" will send a empty line terminated with a newline to take the fdisk default.
		(
			echo o # clear the in memory partition table
			echo w # write the partition table and quit
			echo q # quit"
		) | fdisk "/dev/$1"
		#) | sudo fdisk $1
	}

	createPartitionMountPoint() {
		echo "createPartitionMountPoint $1"

		#Parameter must match "sd[a-z][1-9]"
		partitionName=$1

		partitionUUID=$(lsblk -o NAME,UUID | grep $partitionName | cut -d ' ' -f2)

		#Create mount directory
		mkdir -p /laPlayStorageSpace/$partitionUUID
		#set owner
		#set group
		#set rights

		#ajouter à /etc/fstab avec le paramettre nofail pour que ca marche meme sans le disk
		# voir man fstab
		UUID=$partitionUUID    /mnt/$partitionUUID    ntfs-3g    default,nofail    0    2

	}

	listPartitionMountPoints() {

		lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8
	}

	deletePartitionMountPoint() {

		targetFile="/home/julien.rocco/desktop/fstab"
		lineToDelete=$(grep -n "00000000-2b99-4283-a5d9-2a942dd65e71" $targetFile | cut -d : -f1)
		echo $lineToDelete
		#sed -e $lineToDelete'd' $targetFile
		sed -i -e 's/.*81164c31-2b99-4283-a5d9-2a942dd65e71.*//' $targetFile

		echo "deletePartitionMountPoint $1"
	}

#=================================   MENU   ===================================

	usage() {
		echo "Usage: ./ShellFacade -f <fonction> [-p <parameters>]"
		echo "  -f  name of function to run"
		echo "  -p  parameters of the function"
		echo ""
		echo "Available functions and expected parameters:"
		echo ""
		echo "  createPartitionOnDrive(drive)"
		echo "	ex : ./ShellFacade -f createPartitionOnDrive -p /dev/sda"
		echo ""
		echo "  listPartitions"
		echo "	ex : ./ShellFacade -f listPartitions"
		echo ""
		echo "  deletePartitionsOnDrive(path)"
		echo "	ex : ./ShellFacade -f deletePartitionsOnDrive -p /dev/sdb1"
		echo ""
		echo "  createPartitionMountPoint(path)"
		echo "	ex : ./ShellFacade -f createPartitionMountPoint -p /dev/sda1,/mnt/sda1"
		echo ""
		echo "  listPartitionMountPoints"
		echo "	ex : ./ShellFacade -f listPartitionMountPoints"
		echo ""
		echo "  deletePartitionMountPoint(path)";
		echo "	ex : ./ShellFacade -f deletePartitionMountPoint -p /mnt/sda1"
		exit;
	}

	while getopts ":f:p:" arg; do
		case $arg in
			f) function=${OPTARG};;
			p) parameters=${OPTARG};;
			*) usage;;
		esac
	done

	case $function in
		createPartitionOnDrive		) createPartitionOnDrive $parameters;;
		listPartitions				) listPartitions;;
		deletePartitionsOnDrive		) deletePartitionsOnDrive $parameters;;
		createPartitionMountPoint	) createPartitionMountPoint $parameters;;
		listPartitionMountPoints	) listPartitionMountPoints;;
		deletePartitionMountPoint	) deletePartitionMountPoint $parameters;;
		*							) usage;;
	esac

#sudo ./ShellFacade.sh -f createPartitionOnDrive -p sdb
# umount does not work if drive is busy
#sudo umount /dev/sdb?*
#sudo wipefs -a -f /dev/sdb
#lsblk
#sudo fdisk /dev/sdb



#convert command result : from spaces separated values to coma separated values
#df -h | tr -s ' ' | cut -d ' ' -f 1- --output-delimiter ","
