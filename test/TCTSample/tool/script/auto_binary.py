import os
import string
import sys
import errno
import datetime
import shutil, re, tempfile
import tarfile
from pack import remove_folder, create_folder, sed_inplace
from sys import platform

def is_binary(filename):
    with open(filename, 'rb') as f:
        for block in f:
            if '\0' in block:
                return True
    return False

if __name__ == "__main__":
    if platform == "linux" or platform == "linux2":
        LINUX_PERMISSION = 'sudo '
    else:
        LINUX_PERMISSION = ''

    # Get latest tcs.
#    os.system ('git reset --hard')
#    os.system ('git pull')

    remove_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile')
    remove_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/tv')
    remove_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/wearable')
    create_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile')
    remove_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos')
    create_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos')


    # pack tv package
    os.system(LINUX_PERMISSION+' python pack.py '+'all')

    # Copy mobile package folder to tv package.
    # set tv_pkg_info.xml
    sed_inplace('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','profile="mobile"', 'profile="tv"' )
    sed_inplace('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','mobile/', 'tv/' )
    os.rename('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/tv_pkg_info.xml')
    # rename mobile folder to tv folder
    os.rename('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile','../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/tv')


    # pack wearable package
    os.system(LINUX_PERMISSION+' python pack.py '+'all')
    os.system(LINUX_PERMISSION+' python pack.py '+'auto ElmSharpWearable')
    os.system(LINUX_PERMISSION+' python pack.py '+'manual ElmSharpWearable')
    os.system(LINUX_PERMISSION+' python pack.py '+'auto NUI.Wearable')
    os.system(LINUX_PERMISSION+' python pack.py '+'manual NUI.Wearable')
    os.system(LINUX_PERMISSION+' python pack.py '+'auto WatchfaceComplication')

    # Copy mobile package folder to wearable package.
    # set wearable_pkg_info.xml
    sed_inplace('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','profile="mobile"', 'profile="wearable"' )
    sed_inplace('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','mobile/', 'wearable/' )
    os.rename('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/wearable_pkg_info.xml')
    # rename mobile folder to wearable folder
    os.rename('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile','../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/wearable')


    # pack mobile package
    create_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile')
    os.system(LINUX_PERMISSION+' python pack.py '+'all')




    # Pack TV suites
    #f = open("tvlist", 'r')
    #lines = f.readlines()
    #for line in lines:
    #    if(len(line)>1 and not line.startswith('#')):
    #        os.system(LINUX_PERMISSION+' python pack.py '+line)
    #f.close()

    #sed_inplace('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','profile="mobile"', 'profile="tv"' )
    #sed_inplace('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','mobile/', 'tv/' )
    #os.rename('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml','../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/tv_pkg_info.xml')
    #os.rename('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile','../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/tv')
    #create_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile')
    #os.system(LINUX_PERMISSION+' python pack.py '+'all')


    date_time = datetime.datetime.today().strftime('%Y%m%d')
    src = '../../release/csharp-tct_5.5_dotnet/'
    dst = '../../release/csharp-tct_5.5_' + date_time
    if os.path.exists(dst):
        remove_folder(dst)
    else:
        os.makedirs(dst)

    for item in os.listdir(src):
        s = os.path.join(src, item)
        d = os.path.join(dst, item)
        if os.path.isdir(s):
            shutil.copytree(s, d, symlinks=False, ignore=None)
        else:
            shutil.copy2(s, d)

    os.rename(dst + '/csharp-tct_5.5_dotnet/',dst +'/csharp-tct_5.5_'+date_time)


    try:
        os.remove('../../release/csharp-tct_5.5_'+date_time+'.tar.gz')
    except OSError:
        pass
    try:
        os.remove('../../release/csharp-tct_5.5_'+date_time+'.tar')
    except OSError:
        pass

    if platform == "linux" or platform == "linux2":
        os.system('sudo chmod -R 777 '+ '../../release/csharp-tct_5.5_' + date_time)

    #changing file format
    for root, dirs, files in os.walk(dst):
        for file in files:
            p=os.path.join(root,file)
            if not(is_binary(os.path.abspath(p))):
                text = open(os.path.abspath(p), 'rb').read().replace('\r\n','\n')
                open(os.path.abspath(p), 'wb').write(text)

    with tarfile.open('../../release/csharp-tct_5.5_'+date_time+'.tar.gz', "w:gz") as tar:
        tar.add(dst, arcname=os.path.basename(dst))
    remove_folder('../../release/csharp-tct_5.5_' + date_time)
