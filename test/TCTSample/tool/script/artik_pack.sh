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

sudo rm -rf ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile
sudo rm -rf ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/tv
sudo rm -rf ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/wearable
sudo rm -rf ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/common_iot
sudo mkdir ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile


########################### Auto packaging ###########################
sudo python pack.py auto Alarm
sudo python pack.py auto Applications
sudo python pack.py auto AudioIO
sudo python pack.py auto Bluetooth
sudo python pack.py auto Camera
sudo python pack.py auto DataControl
sudo python pack.py auto Device
sudo python pack.py auto Download
sudo python pack.py auto Information
sudo python pack.py auto Inputmethodmanager
sudo python pack.py auto IoTConnectivityClient
sudo python pack.py auto IoTConnectivityServer
sudo python pack.py auto Log
sudo python pack.py auto MediaCodec
sudo python pack.py auto Mediacontent
sudo python pack.py auto MediaKey
sudo python pack.py auto MediaPlayer
sudo python pack.py auto Metadata
sudo python pack.py auto Mime
sudo python pack.py auto Multimedia
sudo python pack.py auto MultimediaUtil
sudo python pack.py auto Network
sudo python pack.py auto Notifications
sudo python pack.py auto NUI
sudo python pack.py auto Packagemanager
sudo python pack.py auto Privilege
sudo python pack.py auto Recorder
sudo python pack.py auto Securerepository
sudo python pack.py auto Sensor
sudo python pack.py auto StreamRecorder
sudo python pack.py auto System
sudo python pack.py auto Systemsettings
sudo python pack.py auto Tracer


########################### Manual packaging ###########################

sudo python pack.py manual Alarm
sudo python pack.py manual Applications
sudo python pack.py manual AudioIO
sudo python pack.py manual Bluetooth
sudo python pack.py manual Camera
sudo python pack.py manual DataControl
sudo python pack.py manual Device
sudo python pack.py manual Download
sudo python pack.py manual Information
sudo python pack.py manual Inputmethodmanager
sudo python pack.py manual IoTConnectivityClient
sudo python pack.py manual IoTConnectivityServer
sudo python pack.py manual Log
sudo python pack.py manual MediaCodec
sudo python pack.py manual Mediacontent
sudo python pack.py manual MediaKey
sudo python pack.py manual MediaPlayer
sudo python pack.py manual Metadata
sudo python pack.py manual Mime
sudo python pack.py manual Multimedia
sudo python pack.py manual MultimediaUtil
sudo python pack.py manual Network
sudo python pack.py manual Notifications
sudo python pack.py manual NUI
sudo python pack.py manual Packagemanager
sudo python pack.py manual Privilege
sudo python pack.py manual Recorder
sudo python pack.py manual Securerepository
sudo python pack.py manual Sensor
sudo python pack.py manual StreamRecorder
sudo python pack.py manual System
sudo python pack.py manual Systemsettings
sudo python pack.py manual Tracer


#sudo sed -i 's/profile="mobile"/profile="common_iot"/' ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml
#sudo sed -i 's#mobile/#common_iot/#' ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml

#sudo mv ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/common_iot_pkg_info.xml
#sudo mv ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/common_iot

sudo chmod -R 777 ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/

#sudo rm -f ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml
sudo rm -f ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/tv_pkg_info.xml
sudo rm -f ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/wearable_pkg_info.xml
sudo rm -f ../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/common_iot_pkg_info.xml


daily_headed="csharp-tct_5.5_$(date +%Y%m%d)_headed"
sudo rm -rf ../../release/$daily_headed 
sudo cp -r ../../release/csharp-tct_5.5_dotnet/ ../../release/$daily_headed
sudo mv ../../release/$daily_headed/csharp-tct_5.5_dotnet ../../release/$daily_headed/$daily_headed

# copy healthcheck and inst.py files for artik profile
sudo cp -r artik_template/healthcheck.ini ../../release/$daily_headed/$daily_headed/tools/healthcheck.ini
sudo cp -r artik_template/inst.py ../../release/$daily_headed/$daily_headed/resource/tct-testconfig/inst.py

sudo chmod -R 777 ../../release/$daily_headed
cd ../../release
pwd
sudo tar -zcvf "$daily_headed.tar.gz" $daily_headed
sudo chmod -R 777 "$daily_headed.tar.gz"
sudo rm -rf $daily_headed

