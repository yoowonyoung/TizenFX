#!/bin/bash

# tct-toolkit path
# {repository_path}/tct-toolkit/tct-tools/
TOOL_KIT='/home/hoon/share/repository/tct-tools/tct-toolkit/tct-tools/'

# tool path
# {repository_path}/tools/tct_5.5/
TOOLS='/home/hoon/share/repository/tct-tools/tools/tct_5.5/'


# tct path
# {repository_path}/api/release/chsarp-tct_5.5_dotnet
TCT_PATH='/home/hoon/share/repository/public/csharp/net5.5/api/release/csharp-tct_5.5_dotnet'


echo "Getting latest tools in spin repository."
cd $TOOL_KIT
git reset --hard
git pull

cd $TOOLS
git reset --hard
git pull


echo "Updating resource folder"
rsync -avz --exclude=tct-testconfig --exclude=tct-testconfig-3.0.zip $TOOLS/resource/ $TCT_PATH/csharp-tct_5.5_dotnet/resource/
# remove tizen-web-ui-fw and tinyweb folder
rm -rf $TCT_PATH/csharp-tct_5.5_dotnet/resource/tizen-web-ui-fw/
rm -rf $TCT_PATH/csharp-tct_5.5_dotnet/resource/tinyweb/

echo "Updating tools folder"
rsync -avz --exclude=healthcheck.ini --exclude=manager $TOOLS/tools/ $TCT_PATH/csharp-tct_5.5_dotnet/tools/


echo "Updating tct-toolkit"
rm -rf $TCT_PATH/tct-tools_r4_rc1/*
sudo cp -r $TOOL_KIT/* $TCT_PATH/tct-tools_r4_rc1/



echo "Changing folder permission"
sudo chmod -R 777 $TCT_PATH/csharp-tct_5.5_dotnet/
sudo chmod -R 777 $TCT_PATH/tct-tools_r4_rc1/


cd $TCT_PATH
cd ../../tool/script


sudo python auto_binary.py
