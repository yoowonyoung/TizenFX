import os
import string
import sys
import zipfile
import shutil
import platform

FILE_PATH = '../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/pkg_infos/mobile_pkg_info.xml'

OKGREEN = '\033[92m'
ENDC = '\033[0m'

class PKGInfoGenerator:
    # If option is 'all', all package generate.
    module = ''    
    def __init__(self , module):
        self.module = module

    def print_log(self, string):
        print OKGREEN + string + ENDC

    def generate_pkg_info(self):
        self.init_file()
        self.add_initial_tag()
        self.add_testsuite_tag()
        self.add_ending_tag()
        print("")
        #self.print_log("SUCCESSFULLY SAVED AT : " + os.path.abspath(FILE_PATH))

    def init_file(self):
        print("")
        print("-------------------------------------------------------")
        self.print_log("GENERATING PACKAGE INFORMATION")
        f = open(FILE_PATH, 'w+')
        f.write("")
        f.close()

    def write_file(self, string):
        f = open(FILE_PATH, 'a+')
        f.write(string)
        f.close()

    def add_initial_tag(self):
        self.write_file('<?xml version="1.0" encoding="UTF-8"?>\n')
        self.write_file('<ns3:testplan xmlns:ns3="http://www.example.org/plan/" xmlns="" profile="mobile">\n')

    def unzip(self, source_file, dest_path):
        with zipfile.ZipFile(source_file, 'r') as zf:
            zf.extractall(path=dest_path)
            zf.close()
 

    def add_testsuite_tag(self):
        path = '../../release/csharp-tct_5.5_dotnet/csharp-tct_5.5_dotnet/package/mobile/'
        for root, dirs, files in os.walk(path):
            rootpath = os.path.abspath(root)
            files.sort()
            for file in files:
                # Get the project name from tests.xml
                filepath = os.path.join(rootpath, file)
                split_type = ''
                if platform.system() == 'Windows':
                    split_type = '\\'
                else:
                    split_type = '/'

                splitwords = filepath.split(split_type)
                index = len(splitwords)
                project_name = splitwords[index-1]
                project_name = project_name.replace("-5.5.zip","")
                dest = filepath.replace("-5.5.zip","")
                #unzip
                self.unzip( filepath , dest )
                xml_path = dest +'/opt/'+ project_name+'/tests.xml'
                # Count the number of test cases from tests.xml
                grep_cmd = ''
                if platform.system() == 'Windows':
                    xml_path = xml_path.replace('/', '\\')
                    grep_cmd = 'findstr "<testcase" '+xml_path+' | find /c "<testcase" '
                else:
                    grep_cmd = 'grep -o "<testcase" ' + xml_path + ' | wc -l'
                output = os.popen(grep_cmd).read()
                splitwords = output.split("\n")
                tc_number = splitwords[0]
                # Check whether project is auto or manual from project name
                if "Manual" not in project_name: 
                   auto_manual = "auto"
                else:
                   auto_manual = "manual"               

                self.set_testsuite_tag(project_name, tc_number, auto_manual)
                # Remove Unzip folder
                shutil.rmtree(dest)
                  

    def set_testsuite_tag(self, project_name, tc_number, auto_manual):
        if self.module == 'all' :
            print("")
            print("PACKAGE_NAME : " + project_name)
            print('TC_NUMBER : ' +  tc_number )
            print("EXECUTION_TYPE : " + auto_manual)
        elif self.module == project_name:
            print("")
            print("PACKAGE_NAME : " + project_name)
            print("TC_NUMBER : " + tc_number)
            print("EXECUTION_TYPE : " + auto_manual)

        self.write_file('  <suite name="' + project_name + '" category="C# Device APIs">\n')
        if auto_manual == "auto":
            self.write_file('    <auto_tcn>' + tc_number + '</auto_tcn>\n')
            self.write_file('    <manual_tcn>0</manual_tcn>\n')
        else:
            self.write_file('    <auto_tcn>0</auto_tcn>\n')
            self.write_file('    <manual_tcn>' + tc_number + '</manual_tcn>\n')
        self.write_file('    <total_tcn>' + tc_number + '</total_tcn>\n')
        self.write_file('    <pkg_name>mobile/' + project_name + '-5.5.zip</pkg_name>\n')
        self.write_file('  </suite>\n')

    def add_ending_tag(self):
        self.write_file('</ns3:testplan>\n')

#if __name__ == "__main__":
#    obj = PKGInfoGenerator()
#    obj.generate_pkg_info()
