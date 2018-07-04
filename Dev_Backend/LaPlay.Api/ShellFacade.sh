#!/bin/bash

#================================   HELPERS   =================================

#=================================   CORE    ==================================

	createPartitionOnDrive() {

		drivePath=$1
		validDrive=$(echo $drivePath | grep -c -e '^/dev/sd[a-z]$')
		drive=$(echo $drivePath | grep -o 'sd[a-z]$')
		partitionCount=$(grep -c -o $drive'[0-9]' /proc/partitions)

		if [ $validDrive -ne 1 ]
		then
		    echo ERROR: $drivePath' is not a valid drive path' 
		    exit 1 # terminate and indicate error
		fi

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
			) | fdisk $1
		#) | sudo fdisk $1

		blkid : trouver uuid
		mkfs.ntfs /dev/sdb1 -q : formater partition rapide
		=> pleinds d'options, voir man

		créer repertoire de montage
		ajouter à /etc/fstab
				
	}

	listPartitions() {
		lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8
	}

	deletePartitionsOnDrive() {
		# this simulates manual inputs to fdisk
		# A blank line (commented as "default" will send a empty line terminated with a newline to take the fdisk default.
		(
			echo o # clear the in memory partition table
			echo w # write the partition table and quit
			echo q # quit"
			) | fdisk $1
		#) | sudo fdisk $1
	}

	createPartitionMountPoint() {
		echo "createPartitionMountPoint $1"
	}

	listPartitionMountPoints() {
		lsblk -J -o NAME,RM,SIZE,RO,FSTYPE,MOUNTPOINT,UUID,LABEL,PARTLABEL,PARTUUID,TYPE,MAJ:MIN -I 8
	}

	deletePartitionMountPoint() {
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
		echo "	ex : ./ShellFacade -f deletePartitionsOnDrive -p /dev/sda1"
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
		listPartitions			) listPartitions;;
		deletePartitionsOnDrive		) deletePartitionsOnDrive $parameters;;
		createPartitionMountPoint	) createPartitionMountPoint $parameters;;
		listPartitionMountPoints	) listPartitionMountPoints;;
		deletePartitionMountPoint	) deletePartitionMountPoint $parameters;;
		*				) usage;; 
	esac