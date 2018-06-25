#!/bin/bash



echo $text;

usage() {

    echo "Usage: [-d /dev/sd%]" 1>&2;
    exit;
}

partition() {

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
        #) | fdisk
    ) | sudo fdisk $1
}

while getopts ":d:" o; do
    case "${o}" in
        d)
            partition ${OPTARG}
            ;;
        *)
            usage
            ;;
    esac
done



