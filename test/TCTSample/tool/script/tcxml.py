# tcxml.py

# testcase xml struct 
#      <testcase component="" execution_type="manual" id="Tizen.Applications.Tests.ApplicationTests.Run_MANUAL_TEST" priority="" purpose="">
#        <description>
#          <test_script_entry>
#          </test_script_entry>
#        </description>
#      </testcase>

import os
import string
import sys
from xml.etree.ElementTree import Element, SubElement, dump, parse, ElementTree, PI

PREFIX = 'Tizen.'
SUFFIX = '.Tests'
PROJECT_PATH = '../../tct-suite-vs/'

class Testcase:
    component=""
    execution_type=""
    id=""
    priority=""
    purpose=""
    test_script_entry = ""


def read_tc_list(filePath):
    """ Return the TC list in the file """
    class_name=""
    tc_list = []
    f = open(filePath, 'r')
    #print('[file] : '+filePath)
    lines = f.readlines()
    for i in range(len(lines)):
        #finding namespace
        if lines[i].find('namespace') > -1:
            namespace_line = lines[i].replace('{','')
            namespace_line_split = namespace_line.split()
            class_name = namespace_line_split[len(namespace_line_split)-1]
            #print('namespace : '+class_name)

        #finding test class name
        elif lines[i].find('[TestFixture]') > -1 :
            while 1:
                i += 1
                if lines[i].find('class') > -1 and lines[i].find(']') == -1:
                    if lines[i].find(':') == -1:
                      class_line = lines[i].replace('{','')
                      class_line_split = class_line.split()
                      class_name = class_name + '.' + class_line_split[len(class_line_split)-1]
                    else :
                      class_line = lines[i].replace('{','')
                      remove_parent_class = class_line.split(':')
                      class_line_split = remove_parent_class[0].split()
                      class_name = class_name + '.' + class_line_split[len(class_line_split)-1]
                    #print('namespace + class : ' + class_name)
                    break;    

        elif lines[i].find('[Test]') > 0 :
            tc_type = 'auto'
            if lines[i].find('//') == -1 :
                #print('[Test] : '+str(lines[i].find('[Test]')) + ', '+str(lines[i].find('//')))
                while 1:
                    i += 1
                    # check the testcase type
                    if lines[i].find('[Step') > -1:
                        tc_type = 'manual'
                    if lines[i].find('public') > -1:
                        tc = Testcase()
                        # split tc name
                        tc_line = lines[i].replace('{','')
                        tc_name = tc_line.split()
                        # set tc information
                        tc.id = class_name + '.' + tc_name[len(tc_name)-1].replace('()','')
                        #print('tc id : ' + tc.id)
                        tc.execution_type = tc_type
                        tc_list.append(tc)
                        break;
            elif lines[i].find('//') != -1 and lines[i].find('[Test]') < lines[i].find('//'):
                #print('[Test] : '+str(lines[i].find('[Test]')) + ', '+str(lines[i].find('//')))
                while 1:
                    i += 1
                    # check the testcase type
                    if lines[i].find('[Step') > -1:
                        tc_type = 'manual'
                    if lines[i].find('public') > -1:
                        tc = Testcase()
                        # split tc name
                        tc_line = lines[i].replace('{','')
                        tc_name = tc_line.split()
                        # set tc information
                        tc.id = class_name + '.' + tc_name[len(tc_name)-1].replace('()','')
                        #print('tc id : ' + tc.id)
                        tc.execution_type = tc_type
                        tc_list.append(tc)
                        break;
    f.close()

    return tc_list


def read_all_testfiles(path):
    """ Return all abspath of testfiles """
    res = []
    for root, dirs, files in os.walk(path):
        #rootpath = os.path.abspath(path)
        rootpath = os.path.join(os.path.abspath(path), '../'+root)
        for file in files:
            filepath = os.path.join(rootpath, file)
            if filepath.find('~') == -1:
                res.append(filepath)
                #print(filepath)

    return res


def indent(elem, level=0):
    i = "\n" + level*"  "
    if len(elem):
        if not elem.text or not elem.text.strip():
            elem.text = i + "  "
        if not elem.tail or not elem.tail.strip():
            elem.tail = i
        for elem in elem:
            indent(elem, level+1)
        if not elem.tail or not elem.tail.strip():
            elem.tail = i
    else:
        if level and (not elem.tail or not elem.tail.strip()):
            elem.tail = i



def write_xml(module_name , tc_list, dest):

    # Create 'fake' root node
    fake_root = Element(None)

    # Add desired processing instructions.  Repeat as necessary.
    pi = PI("xml-stylesheet", "type='text/xsl' href='./testcase.xsl'")
    pi.tail = "\n"
    fake_root.append(pi)

    root = Element('test_definition')
    suite_ele = SubElement(root, "suite")
    suite_ele.attrib["category"]=""
    suite_ele.attrib["extension"]=""
    suite_ele.attrib["name"]=PREFIX+module_name+SUFFIX
    suite_ele.attrib["type"]="native"

    set_ele = SubElement(suite_ele, "set")
    set_ele.attrib["type"]="js"
    set_ele.attrib["name"]=PREFIX+module_name+SUFFIX


    for tc in tc_list:
        # Create <testcase> element
        tc_ele = SubElement(set_ele, "testcase")
        # set testcase attribute
        tc_ele.attrib["component"] = tc.component
        tc_ele.attrib["execution_type"] = tc.execution_type
        tc_ele.attrib["id"] = tc.id
        tc_ele.attrib["priority"] = tc.priority
        tc_ele.attrib["purpose"] = tc.purpose
        # Create <description> element
        description_ele = SubElement(tc_ele, "description")
        # Create <test_script_entry> element
        test_script_entry_ele = SubElement(description_ele, "test_script_entry")
        # set text
        test_script_entry_ele.text = " "

    indent(root)

    # Add real root as last child of fake root
    fake_root.append(root)
    
    xml_tree = ElementTree(fake_root)
    xml_tree.write(dest, encoding="utf-8", xml_declaration=True)
