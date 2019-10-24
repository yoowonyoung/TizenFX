#!/bin/bash
PATH=/bin:/usr/bin:/sbin:/usr/sbin
for i in `grep -r "0xA" /var/cynara/db/_ | grep $1`
do
    CLIENT=`echo $i | cut -d ";" -f1`
    USER=`echo $i | cut -d ";" -f2`
    PRIVILEGE=`echo $i | cut -d ";" -f3`
    #echo "cyad --erase=\"\" -r=no -c $CLIENT -u $USER -p $PRIVILEGE"
    cyad --erase="" -r=no -c $CLIENT -u $USER -p $PRIVILEGE
done
