# new_pakcage.py
# make new TCT package 

import os
import string
import sys
import shutil, re, tempfile
from pack import sed_inplace
from os import rename

PREFIX = 'Tizen.'
SUFFIX = '.Tests'
PROJECT_PATH = '../../tct-suite-vs/'
MODULE_NAME=""

PACKAGE_NAME = ''


class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    ERROR = '\033[91m'
    ENDC = '\033[0m'

    def disable(self):
        self.HEADER = ''
        self.OKBLUE = ''
        self.OKGREEN = ''
        self.WARNING = ''
        self.ERROR = ''
        self.ENDC = ''


if __name__ == "__main__":

    # sudo python new_pakcage.py {auto or manual} {module_name}
    if len(sys.argv) < 3:          
        print(bcolors.ERROR +"===> python new_pakcage.py {auto or manual} {module_name}" +bcolors.ENDC)
        exit(1)

    arg1 = sys.argv[1]
    arg2 = sys.argv[2]


    if arg1 != 'auto' and arg1 !='manual' and arg2 != 'auto' and arg2 !='manual' :
        print(bcolors.ERROR +"===> python new_pakcage.py {auto or manual} {module_name}" +bcolors.ENDC)
        exit(1)

    if arg2 == 'auto' or arg2 =='manual' :
        temp = arg1
        arg1 = arg2
        arg2 = temp

    MODULE_NAME = arg2
    TEMPLATE_PATH = ""

    if arg1 == 'auto':
        PACKAGE_NAME = PREFIX + MODULE_NAME + SUFFIX
        TEMPLATE_PATH = "Tizen.Newmodule.Tests"
    else :
        PACKAGE_NAME = PREFIX + MODULE_NAME +'.Manual' + SUFFIX
        TEMPLATE_PATH = "Tizen.Newmodule.Manual.Tests"


    if os.path.isdir(PROJECT_PATH+PACKAGE_NAME):
        print(bcolors.ERROR +"===> " + MODULE_NAME+" " + arg1 + " package already exists." + bcolors.ENDC)
        exit(1)

    # Copy project template
    shutil.copytree("template/"+TEMPLATE_PATH , PROJECT_PATH+PACKAGE_NAME)

    sed_inplace(PROJECT_PATH+PACKAGE_NAME+"/tizen-manifest.xml",TEMPLATE_PATH, PACKAGE_NAME)
    sed_inplace(PROJECT_PATH+PACKAGE_NAME+"/"+TEMPLATE_PATH+".csproj",TEMPLATE_PATH, PACKAGE_NAME)
    sed_inplace(PROJECT_PATH+PACKAGE_NAME+"/"+TEMPLATE_PATH+".sln",TEMPLATE_PATH, PACKAGE_NAME)

    rename(PROJECT_PATH+PACKAGE_NAME+"/"+TEMPLATE_PATH+".csproj",PROJECT_PATH+PACKAGE_NAME+"/"+PACKAGE_NAME+".csproj")
    rename(PROJECT_PATH+PACKAGE_NAME+"/"+TEMPLATE_PATH+".sln",PROJECT_PATH+PACKAGE_NAME+"/"+PACKAGE_NAME+".sln")
    rename(PROJECT_PATH+PACKAGE_NAME+"/shared/res/"+TEMPLATE_PATH+".png",PROJECT_PATH+PACKAGE_NAME+"/shared/res/"+PACKAGE_NAME+".png")
        


