# gen.py
# make package file

import os
import string
import sys
import shutil, re, tempfile
import zipfile
from xml.etree.ElementTree import Element, SubElement, dump, parse, ElementTree
from pkginfo_gen import PKGInfoGenerator
from tcxml import read_tc_list, read_all_testfiles, indent, write_xml

PREFIX = 'Tizen.'
SUFFIX = '.Tests'
PROJECT_PATH = '../../tct-suite-vs/'
MODULE_NAME=""

PACKAGE_NAME = ''
TPK_PATH = ''
RELEASE_PATH = ''
TCT='csharp-tct_5.5_dotnet'
SUCCESS = 0
FAILS = 0


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

class Testcase:
    component=""
    execution_type=""
    id=""
    priority=""
    purpose=""
    test_script_entry = ""


def sed_inplace(filename, pattern, repl):
    '''
    Perform the pure-Python equivalent of in-place `sed` substitution: e.g.,
    `sed -i -e 's/'${pattern}'/'${repl}' "${filename}"`.
    '''
    pattern_compiled = re.compile(pattern)

    with tempfile.NamedTemporaryFile(mode='w', delete=False) as tmp_file:
        with open(filename) as src_file:
            for line in src_file:
                tmp_file.write(pattern_compiled.sub(repl, line))

    shutil.copystat(filename, tmp_file.name)
    shutil.move(tmp_file.name, filename)


def zip(src, dst):
    zf = zipfile.ZipFile("%s.zip" % (dst), "w", zipfile.ZIP_DEFLATED)
    abs_src = os.path.abspath(src)
    for dirname, subdirs, files in os.walk(src):
        for filename in files:
            absname = os.path.abspath(os.path.join(dirname, filename))
            arcname = absname[len(abs_src) + 1:]
            #print 'zipping %s as %s' % (os.path.join(dirname, filename),arcname)
            zf.write(absname, arcname)
    zf.close()


def folder_list(path):
    res = []
    for name in os.listdir(path):
        if name.endswith('Tests'):
            if "ElmSharpWearable" in name :
                continue

            if "NUI.Wearable" in name :
                continue

            if "WatchfaceComplication" in name :
                continue

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



def create_folder(path):
    if not os.path.isdir(path):
        os.mkdir(path,0777)

def remove_folder(path):
    if os.path.isdir(path):
        shutil.rmtree(path)

def copy_helper_application(folder_path , dest):
    create_folder(dest)
    #print(dest)

    for root, dirs, files in os.walk(folder_path):
        #rootpath = os.path.abspath(path)
        rootpath = os.path.join(os.path.abspath(folder_path), '../'+root)
        for file in files:
            filepath = os.path.join(rootpath, file)
            if filepath.endswith('.tpk'):
                shutil.copy(filepath, dest)
                #print(filepath)
            elif filepath.endswith('.wgt'):
                shutil.copy(filepath, dest)
                #print(filepath)


def file_count(path):
    list_dir = []
    list_dir = os.listdir(path)
    count = 0
    for file in list_dir:
        count += 1

    return count

def print_result(success , fails):  
    print('')
    print('-------------------------------------')
    print(' packaging result ')
    print('  %d succeeded , %d failed ' % (success , fails) )
    print('-------------------------------------')
    print('')


def pack(module , cnt):  

    MODULE_NAME = module

    PACKAGE_NAME = PREFIX+MODULE_NAME+SUFFIX
    TPK_PATH = PROJECT_PATH+PACKAGE_NAME+'/bin/Debug/tizen60/'+PACKAGE_NAME+'-1.0.0.tpk'
    RELEASE_PATH = 'release/'

    # Check the module exist or not.
    if not os.path.isdir(PROJECT_PATH+PACKAGE_NAME+'/'): 
        print(bcolors.ERROR +  str(cnt+1) + "  please check the module name. "+bcolors.ENDC+MODULE_NAME+bcolors.ERROR+" package does not exist in tct-suite-vs."+bcolors.ENDC)
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
        global FAILS        
        FAILS += 1
        return

    if not os.path.isfile(TPK_PATH):
        print(bcolors.ERROR +  str(cnt+1) + "  You should build "+MODULE_NAME+" project before run this script." +bcolors.ENDC)
        FAILS += 1
        return 

    # Create release / tclist / bin folder.
    create_folder(RELEASE_PATH)
    create_folder(RELEASE_PATH+'tclist')
    create_folder('bin')

    # Remove bin/opt folder.
    remove_folder('bin/opt')


    # Create bin/opt and package folder.
    os.mkdir('bin/opt',0777)
    os.mkdir('bin/opt/'+PACKAGE_NAME,0777)

    # create new test.xml file
    file_list = read_all_testfiles(PROJECT_PATH+PACKAGE_NAME+'/testcase/')
    file_list.sort()   
    tc_list = []

    for file_path in file_list:
        # read only .cs(Testcases) files.
        if file_path.endswith('.cs'):
            tc_list.extend(read_tc_list(file_path))

    write_xml(MODULE_NAME,tc_list,'bin/opt/'+PACKAGE_NAME+'/tests.xml')

    # copy tests.xml to release/tclist folder.
    create_folder(RELEASE_PATH+'tclist/'+PACKAGE_NAME)
    shutil.copy('bin/opt/'+PACKAGE_NAME+'/tests.xml', RELEASE_PATH+'tclist/'+PACKAGE_NAME)

    # create new inst.py file
    shutil.copy('template/inst.py', 'bin/opt/'+PACKAGE_NAME)
    sed_inplace('bin/opt/'+PACKAGE_NAME+'/inst.py','REPLACE', PACKAGE_NAME )

    # copy Tizen.$1.Tests.tpk from tct-suite-vs package folder
    shutil.copy(TPK_PATH, 'bin/opt/'+PACKAGE_NAME)

    # copy helper applications
    if os.path.isdir(PROJECT_PATH+'Resource/'+PACKAGE_NAME):
        copy_helper_application(PROJECT_PATH+'Resource/'+PACKAGE_NAME , 'bin/opt/'+PACKAGE_NAME+'/apps')

    # copy askpolicy script
    # shutil.copy('template/askpolicy.sh', 'bin/opt/'+PACKAGE_NAME)

    # copy res folder
    if os.path.isdir(PROJECT_PATH+PACKAGE_NAME+'/res'):
        if file_count(PROJECT_PATH+PACKAGE_NAME+'/res') > 0:
            shutil.copytree(PROJECT_PATH+PACKAGE_NAME+'/res' , 'bin/opt/'+PACKAGE_NAME+'/res')

    # create zip 
    zip('bin', RELEASE_PATH+PACKAGE_NAME+'-5.5')
    if os.path.isfile(RELEASE_PATH+PACKAGE_NAME+'-5.5.zip'):
        print( str(cnt+1) + "  Created "+bcolors.OKGREEN + MODULE_NAME + bcolors.ENDC + " Packages.")
        global SUCCESS        
        SUCCESS += 1

    # copy zip file 
    create_folder('../../release/'+TCT+'/'+TCT+'/package/mobile/')
    shutil.copy(RELEASE_PATH+PACKAGE_NAME+'-5.5.zip', '../../release/'+TCT+'/'+TCT+'/package/mobile/')

    # If TCT is installed , replace package
    if os.path.isdir('/opt/tct/tizen_csharp_5.5'):
        shutil.copy(RELEASE_PATH+PACKAGE_NAME+'-5.5.zip', '/opt/tct/tizen_csharp_5.5/packages/mobile')

    # Remove bin folder.
    remove_folder('bin')




def print_error():
    print(" Please check command.")
    print("     guide  > sudo python pack.py {auto or manual} {module_name}")
    print("     example> sudo python pack.py auto Applications")
    print("     The fisrst letter of {module_name} is always capitalized.")
    print(" If you want to pack all package, you can use 'sudo python pack.py all'")

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
        pack_list = folder_list(PROJECT_PATH)
    else:
        MODULE = sys.argv[2]
        if sys.argv[1] == 'manual':  
            MODULE = MODULE +'.Manual'
        pack_list.append(MODULE)

    pack_list.sort()

    for i in range(len(pack_list)):
        pack(pack_list[i] , i)


    create_folder('../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/')
    if sys.argv[1] == 'all':
        pkg_info_gen = PKGInfoGenerator('all')
    else :
        pkg_info_gen = PKGInfoGenerator(PREFIX+MODULE+SUFFIX)
    pkg_info_gen.generate_pkg_info()
    if os.path.isdir('/opt/tct/tizen_csharp_5.5'):
        shutil.copy('../../release/'+TCT+'/'+TCT+'/package/pkg_infos/mobile_pkg_info.xml', '/opt/tct/tizen_csharp_5.5/packages/pkg_infos')

        
#    pkg_info_gen = PKGInfoGenerator()
#    pkg_info_gen.generate_pkg_info()

    print_result( SUCCESS , FAILS )


