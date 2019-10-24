import os
import string
import sys
import tarfile
import shutil, re, tempfile
from pack import remove_folder, create_folder, sed_inplace
from sys import platform

if __name__ == "__main__":
    if platform == "linux" or platform == "linux2":
        LINUX_PERMISSION = 'sudo '
    else:
        LINUX_PERMISSION = ''

    if len(sys.argv) < 2:
        print("Please insert version. Example python tct_pack.py r1_rc1")
        exit(1)
    else:
        version_name=sys.argv[1]

    remove_folder('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/mobile')
    remove_folder('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/tv')
    create_folder('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/mobile')
    remove_folder('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/pkg_infos')
    create_folder('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/pkg_infos')

    #os.system("sudo python pack.py auto Alarm")

    # Pack TV suites
    f = open("tvlist", 'r')
    lines = f.readlines()
    for line in lines:
        if(len(line)>1 and not line.startswith('#')):
            os.system(LINUX_PERMISSION+' python pack.py '+line)
    f.close()

    sed_inplace('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/pkg_infos/mobile_pkg_info.xml','profile="mobile"', 'profile="tv"' )

    sed_inplace('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/pkg_infos/mobile_pkg_info.xml','mobile/', 'tv/' )

    os.rename('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/pkg_infos/mobile_pkg_info.xml','../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/pkg_infos/tv_pkg_info.xml')

    os.rename('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/mobile','../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/tv')

    create_folder('../../release/csharp-tct_4.0_dotnet/csharp-tct_4.0_dotnet/package/mobile')

    os.system(LINUX_PERMISSION+' python pack.py '+'all')
    src = '../../release/csharp-tct_4.0_dotnet/'
    dst = '../../release/csharp-tct_4.0_' + version_name

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

    os.rename(dst + '/csharp-tct_4.0_dotnet/',dst +'/csharp-tct_4.0_' + version_name)

    try:
        os.remove('../../release/csharp-tct_4.0_' + version_name+'.tar.gz')
    except OSError:
        pass
    try:
        os.remove('../../release/csharp-tct_4.0_' + version_name+'.tar')
    except OSError:
        pass

    if platform == "linux" or platform == "linux2":
        os.system('sudo chmod -R 777 '+ '../../release/csharp-tct_4.0_' + version_name)

    #changing file format
#    for root, dirs, files in os.walk(dst):
#        for file in files:
#            p=os.path.join(root,file)
#            if ('.sh' in p) or ('.py' in p):
#                text = open(os.path.abspath(p), 'rb').read().replace('\r\n', '\n')
#                open(os.path.abspath(p), 'wb').write(text)

    with tarfile.open('../../release/csharp-tct_4.0_' + version_name+'.tar.gz', "w:gz") as tar:
        tar.add(dst, arcname=os.path.basename(dst))
    remove_folder('../../release/csharp-tct_4.0_' + version_name)
