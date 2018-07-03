#!/bin/bash


freeSpaceToPartition() {

    # this simulates manual inputs to fdisk
    # A blank line (commented as "default" will send a empty line terminated with a newline to take the fdisk default.
    (
        echo o # clear the in memory partition table
        echo n # new partition
        echo p # primary partition
        echo   # default - partition number 1
        echo   # default - start at beginning of disk 
        echo   # default - finish at end of disk 
        echo w # write the partition table and quit
        echo q # quit"
        ) | fdisk
    #) | sudo fdisk $1
}

listPartitions() {
	echo "listPartitions"
}

deletePartition() {
	echo "deletePartition $1"
}

createPartitionMountPoint() {
	echo "createPartitionMountPoint $1"
}

listPartitionMountPoints() {
	echo "listPartitionMountPoints"
}

deletePartitionMountPoint() {
	echo "deletePartitionMountPoint $1"
}

usage() {
	echo "Usage: ./ShellFacade -f <fonction> [-p <parameters>]"
	echo "  -f  name of function to run"
	echo "  -p  parameters of the function"
	echo ""
	echo "Available functions and expected parameters:"
	echo ""
	echo "  freeSpaceToPartition(drive)"
	echo "    ex : ./ShellFacade -f freeSpaceToPartition -p /dev/sda"
	echo ""
	echo "  listPartitions"
	echo "    ex : ./ShellFacade -f listPartitions"
	echo ""
	echo "  deletePartition(path)"
	echo "    ex : ./ShellFacade -f deletePartition -p /dev/sda1"
	echo ""
	echo "  createPartitionMountPoint(path)"
	echo "    ex : ./ShellFacade -f createPartitionMountPoint -p /dev/sda1,/mnt/sda1"
	echo ""
	echo "  listPartitionMountPoints"
	echo "    ex : ./ShellFacade -f listPartitionMountPoints"
	echo ""
	echo "  deletePartitionMountPoint(path)";
	echo "    ex : ./ShellFacade -f deletePartitionMountPoint -p /mnt/sda1"
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
	freeSpaceToPartition		) freeSpaceToPartition $parameters;;
        listPartitions			) listPartitions;;
	deletePartition			) deletePartition $parameters;;
	createPartitionMountPoint	) createPartitionMountPoint $parameters;;
	listPartitionMountPoints	) listPartitionMountPoints;;
	deletePartitionMountPoint	) deletePartitionMountPoint $parameters;;
        *				) usage;; 
esac