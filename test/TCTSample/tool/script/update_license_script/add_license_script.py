# 1. Remove previous license script top of .cs files.
# 2. Add new license script top of .cs files.

import os
import sys
import shutil

PREFIX = 'Tizen.'
SUFFIX = '.Tests'
PROJECT_PATH = '../../../tct-suite-vs/'
MODULE_NAME=""
PACKAGE_NAME = ''
PATH = '../../../tct-suite-vs/'

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

def update_license_script(path):
    try:
        res = []
        for name in os.listdir(path):
            if name.endswith('.Tests'):
                print (bcolors.OKGREEN + 'Package : '+name + bcolors.ENDC)
                print ('    Updated file lists')
                search_tclist(path+name)
    except PermissionError:
        pass

    return res

def search_tclist(dirname):
    filenames = os.listdir(dirname)
    for filename in filenames:
        full_filename = os.path.join(dirname, filename)
        if os.path.isdir(full_filename):
            # except folder (obj, bin, Properties, res)
            if filename != 'obj' and filename != 'bin' and filename != 'Properties' and filename != 'res':
                search_tclist(full_filename)
        else:
            if filename.endswith('.cs'):
                print ("        "+filename)
                remove_previous_script(full_filename)
                add_new_script(full_filename)

def read_script():
    s = ""
    f = open("license_script", 'r')
    while True:
        line = f.readline()
        if not line: break
        s+=str(line)
    f.close()

    return s

def remove_previous_script(path):
    filename = path
    f = open(filename,'r')
    lines = f.readlines()
    f.close()

    f = open(filename,'w')
    flag = False
    for line in lines:
        # UTF8-BOM
        if line.startswith('using') or line.startswith('\xef\xbb\xbfusing'):
            flag = True
        if flag:
            f.write(line)

    f.close()

def add_new_script(path):
    filename = path
    f = open(filename,'r')
    lines = f.readlines()
    f.close()

    f = open(filename,'w')

    f.write(read_script())

    for line in lines:
        f.write(line)

    f.close()

def print_error():
    print(" Please check command.")
    print("     guide  > sudo python add_license_script.py {auto or manual} {module_name}")
    print("     example> sudo python add_license_script.py auto Applications")
    print(" If you want to update all package, you can use 'sudo python add_license_script.py all'")

def print_exist_package():
        print(bcolors.ERROR + "  please check the module name. "+bcolors.ENDC+MODULE_NAME+bcolors.ERROR+" package does not exist in tct-suite-vs."+bcolors.ENDC)
        print("  You can choose from below package list.")
        module_list = folder_list(PROJECT_PATH)
        module_list.sort()
        for module in module_list:
            temp = module
            if module.find(".Manual") > -1:
                temp = module.replace(".Manual" , "")
                temp = "manual " + temp
            else :
                temp = "auto " + temp
            print("      " + temp)

def folder_list(path):
    res = []
    for name in os.listdir(path):
        if name.endswith('Tests'):
            splitwords = name.split(".")
            splitwords.pop()
            splitwords.pop(0)
            folder_name = ''

            for splitword in splitwords:
                if folder_name == '':
                    folder_name += splitword
                else:
                    folder_name += '.' + splitword
            #print folder_name
            res.append(folder_name)
    return res


if __name__ == "__main__":
    pack_list = []

    if len(sys.argv) == 2:
        if(sys.argv[1] != 'all'):
            print_error()
            exit(1)
    elif len(sys.argv) < 3:
        print_error()
        exit(1)

    arg1 = sys.argv[1]

    if arg1 != 'auto' and arg1 !='manual' and arg1 !='all' :
        print_error()
        exit(1)

    if sys.argv[1] == 'all':
        update_license_script(PATH)
    else:
        MODULE = sys.argv[2]
        if sys.argv[1] == 'manual':
            MODULE = MODULE +'.Manual'
        PACKAGE_NAME = PREFIX+MODULE+SUFFIX
        PATH = PROJECT_PATH+PACKAGE_NAME
        if not os.path.isdir(PATH):
            print_exist_package()
            exit(1)
        print(bcolors.OKGREEN+'Package : '+PACKAGE_NAME + bcolors.ENDC)
        print ('    Updated file lists')
        search_tclist(PATH)

    print(bcolors.OKGREEN + '------------------------------------------')
    print('License script have updated successfully.')
    print('------------------------------------------'+bcolors.ENDC)
