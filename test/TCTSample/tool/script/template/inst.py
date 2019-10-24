#!/usr/bin/env python
import os
import shutil
import glob
import time
import sys
import subprocess
import string
from optparse import OptionParser, make_option
import ConfigParser

PKG_NAMES = ["REPLACE"]
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PKG_NAME = os.path.basename(SCRIPT_DIR)
PARAMETERS = None
#XW_ENV = "export DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/5000/dbus/user_bus_socket"
TCT_CONFIG_FILE = "/opt/tools/TCT_CONFIG"
tct_parser = ConfigParser.ConfigParser()
tct_parser.read(TCT_CONFIG_FILE)
SRC_DIR = tct_parser.get('DEVICE', 'DEVICE_SUITE_TARGET_30')
PKG_SRC_DIR = "%s/tct/opt/%s" % (SRC_DIR, PKG_NAME)


def doCMD(cmd):
    # Do not need handle timeout in this short script, let tool do it
    print "-->> \"%s\"" % cmd
    output = []
    cmd_return_code = 1
    cmd_proc = subprocess.Popen(
        cmd, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, shell=True)

    while True:
        output_line = cmd_proc.stdout.readline().strip("\r\n")
        cmd_return_code = cmd_proc.poll()
        if output_line == '' and cmd_return_code != None:
            break
        sys.stdout.write("%s\n" % output_line)
        sys.stdout.flush()
        output.append(output_line)

    return (cmd_return_code, output)


def updateCMD(cmd=None):
    if "pkgcmd" in cmd:
        cmd = "su - %s -c '%s'" % (PARAMETERS.user, cmd)
#  Due to permission issue in tizen 5.5 ver
#    elif "copy_res" in cmd: 
#        cmd = "su - %s -c '%s;%s'" % (PARAMETERS.user, XW_ENV, cmd)
    elif "app_launcher" in cmd:
        cmd = "su - %s -c '%s'" % (PARAMETERS.user, cmd)
    return cmd
def getUSERID():
    if PARAMETERS.mode == "SDB":
        cmd = "sdb -s %s shell id -u %s" % (
            PARAMETERS.device, PARAMETERS.user)
    else:
        cmd = "ssh %s \"id -u %s\"" % (
            PARAMETERS.device, PARAMETERS.user )
    return doCMD(cmd)




def getPKGID(pkg_name=None):

    if pkg_name.endswith("-1.0.0"):
        pkg_name = pkg_name.replace("-1.0.0","")
    elif pkg_name.endswith("-1.0.0-arm"):
        pkg_name = pkg_name.replace("-1.0.0-arm","")


    if PARAMETERS.mode == "SDB":
        cmd = "sdb -s %s shell %s" % (
            PARAMETERS.device, updateCMD('pkgcmd -l'))
    else:
        cmd = "ssh %s \"%s\"" % (
            PARAMETERS.device, updateCMD('pkgcmd -l'))

    (return_code, output) = doCMD(cmd)
    if return_code != 0:
        return None

    test_pkg_id = None
    for line in output:
        if line.find("[" + pkg_name + "]") != -1:
            pkgidIndex = line.split().index("pkgid")
            test_pkg_id = line.split()[pkgidIndex+1].strip("[]")
            break
    return test_pkg_id


def doRemoteCMD(cmd=None):
    if PARAMETERS.mode == "SDB":
        cmd = "sdb -s %s shell %s" % (PARAMETERS.device, updateCMD(cmd))
    else:
        cmd = "ssh %s \"%s\"" % (PARAMETERS.device, updateCMD(cmd))

    return doCMD(cmd)


def doRemoteCopy(src=None, dest=None):
    if PARAMETERS.mode == "SDB":
        cmd_prefix = "sdb -s %s push" % PARAMETERS.device
        cmd = "%s %s %s" % (cmd_prefix, src, dest)
    else:
        cmd = "scp -r %s %s:/%s" % (src, PARAMETERS.device, dest)

    (return_code, output) = doCMD(cmd)
    doRemoteCMD("sync")

    if return_code != 0:
        return True
    else:
        return False


def uninstPKGs():
    action_status = True
    doRemoteCMD("sh /home/owner/share/res/copy_res.sh -u")
    doRemoteCMD("rm -rf /home/owner/share/res")

    for root, dirs, files in os.walk(SCRIPT_DIR):
        print "uninstPKGs ################################"
        for file in files:
            print ("file : %s" % (file))
            if file.endswith(".wgt"):
                pkg_id = getPKGID(os.path.basename(os.path.splitext(file)[0]))
                if not pkg_id:
                    action_status = False
                    print ("wgt pkg_id is None. file name is : %s " % (file))
                    continue

                print ("wgt pkg_id : %s" % (pkg_id))
                (return_code, output) = doRemoteCMD(
                    "pkgcmd -u -t wgt -q -n %s" % pkg_id)
                for line in output:
                    if "Failure" in line:
                        action_status = False
                        break
            elif file.endswith(".tpk"):
                pkg_id = getPKGID(os.path.basename(os.path.splitext(file)[0]))
                if not pkg_id:
                    print ("tpk pkg_id is None. file name is : %s " % (file))
                    action_status = False
                    continue

                print ("tpk pkg_id : %s" % (pkg_id))
                (return_code, output) = doRemoteCMD(
                    "pkgcmd -u -t tpk -q -n %s" % pkg_id)

                for line in output:
                    if "Failure" in line:
                        action_status = False
                        break

    (return_code, output) = doRemoteCMD(
        "rm -rf %s" % PKG_SRC_DIR)

    if return_code != 0:
        action_status = False

    return action_status


def askpolicyremoving():
    for root, dirs, files in os.walk(SCRIPT_DIR):
        for file in files:
            if file.endswith(".wgt"):
                pkg_id = getPKGID(os.path.basename(os.path.splitext(file)[0]))
            elif file.endswith(".tpk"):
                pkg_id = getPKGID(os.path.basename(os.path.splitext(file)[0]))

    print ("pkg_id : %s" % (pkg_id))
    print (os.getcwd())
    print (os.path.dirname(os.path.realpath(__file__)) )
    if not doRemoteCopy("%s/askpolicy.sh" % SCRIPT_DIR, "%s" % (SRC_DIR)):
        action_status = False
    if PARAMETERS.mode == "SDB":
        cmd = "sdb -s %s shell .%s/askpolicy.sh %s" % (PARAMETERS.device,
        SRC_DIR, pkg_id)
    return doCMD(cmd)


def checkPlatformArch(config_key):
    (return_code, output) = doRemoteCMD("cat /etc/config/model-config.xml | grep %s" % (config_key))
    output_len = len(output)

    if output_len == 0:
        return False

    for out in output:
        if config_key in out:
            if 'true' in out:
                return True
    return False

def checkProfile():
    config_key = 'tizen.org/feature/profile'
    (return_code, output) = doRemoteCMD("cat /etc/config/model-config.xml | grep %s" % (config_key))
    output_len = len(output)

    #'mobile/wearable/tv'
    for out in output:
        if config_key in out:
            if 'mobile' in out:
                return 'mobile'
            elif 'wearable' in out:
                return 'wearable'
            elif 'tv' in out:
                return 'tv'

def instPKGs():
    action_status = True
    (return_code, output) = doRemoteCMD(
        "mkdir -p /home/owner/share/res")
    (return_code, output) = doRemoteCMD(
        "mkdir -p %s" % PKG_SRC_DIR)
    if return_code != 0:
        action_status = False
    if PKG_NAMES[0] == "Tizen.Information.Tests":
        (return_code, output) = doRemoteCMD("cp /etc/config/model-config.xml /opt/usr/home/owner/share/res")

    if not doRemoteCopy(SCRIPT_DIR+"/res", "/home/owner/share/res/"):
        action_status = False
    (return_code, output) = doRemoteCMD("sh /home/owner/share/res/copy_res.sh -i")
    if return_code != 0:
        action_status = False

    for root, dirs, files in os.walk(SCRIPT_DIR):

        if (SCRIPT_DIR+"/res") in root :
            continue

        for file in files:
            sys.stdout.write("file : %s\n" % file)
            if file == "WgtPkgTest.wgt":
                if not doRemoteCopy(root + '/' + file, "%s/%s" % (SRC_DIR, file)):
                    action_status = False
                (return_code, output) = doRemoteCMD(
                    "pkgcmd -i -t wgt -q -p %s/%s" % (SRC_DIR, file))
                doRemoteCMD("rm -rf %s/%s" % (SRC_DIR, file))
                for line in output:
                    if "Failure" in line:
                        action_status = False
                        break
                #move on SDcard
                (return_code, output) = doRemoteCMD(
                    "pkgcmd -m -t wgt -T 1 -n JGvAAJHzxu" )
                for line in output:
                    if "Failure" in line:
                        action_status = False
                        break
            elif file.endswith(".wgt"):
                if not doRemoteCopy(root + '/' + file, "%s/%s" % (SRC_DIR, file)):
                    action_status = False
                (return_code, output) = doRemoteCMD(
                    "pkgcmd -i -t wgt -q -p %s/%s" % (SRC_DIR, file))
                doRemoteCMD("rm -rf %s/%s" % (SRC_DIR, file))
                for line in output:
                    if "Failure" in line:
                        action_status = False
                        break
            elif file.endswith(".tpk"):
                if not doRemoteCopy(root + '/' + file, "%s/%s" % (SRC_DIR, file)):
                    action_status = False

                if file.endswith("-x86.tpk"):
                    x86_ret = checkPlatformArch('tizen.org/feature/platform.core.cpu.arch.x86')

                    if x86_ret:
                        (return_code, output) = doRemoteCMD("pkgcmd -i -t tpk -q -p %s/%s" % (SRC_DIR, file))
                    else:
                        print("Skipped installation on this platform. file name is %s" % (file))

                elif file.endswith("-arm.tpk"):
                    armv7_ret = checkPlatformArch('tizen.org/feature/platform.core.cpu.arch.armv7')
                    armv6_ret = checkPlatformArch('tizen.org/feature/platform.core.cpu.arch.armv6')

                    if armv7_ret or armv6_ret:
                        (return_code, output) = doRemoteCMD("pkgcmd -i -t tpk -q -p %s/%s" % (SRC_DIR, file))
                    else:
                        print("Skipped installation on this platform. file name is %s" % (file))

                elif file == "org.tizen.example.IMESample.Tizen.Mobile-1.0.0.tpk":
                    check_profile = checkProfile()
                    if check_profile != 'wearable':
                        (return_code, output) = doRemoteCMD("pkgcmd -i -t tpk -q -p %s/%s" % (SRC_DIR, file))
                    else:
                        print("Skipped installation on this profile. file name is %s" % (file))

                elif file == "org.tizen.example.IMESample.Tizen.Wearable-1.0.0.tpk":
                    check_profile = checkProfile()
                    if check_profile == 'wearable':
                        (return_code, output) = doRemoteCMD("pkgcmd -i -t tpk -q -p %s/%s" % (SRC_DIR, file))
                    else:
                        print("Skipped installation on this profile. file name is %s" % (file))

                else:
                    (return_code, output) = doRemoteCMD(
                        "pkgcmd -i -t tpk -q -p %s/%s" % (SRC_DIR, file))

                if file == "org.tizen.MsgPortApp.Tizen-1.0.0.tpk":
                    (return_code, output) = doRemoteCMD("app_launcher -s %s" % ("org.tizen.MsgPortApp.Tizen"))
                elif file == "org.tizen.MsgPortApp1.Tizen-1.0.0.tpk":
                    (return_code, output) = doRemoteCMD("app_launcher -s %s" % ("org.tizen.MsgPortApp1.Tizen"))
                elif file.startswith("Tizen.Privilege.Tests"):
                    doCMD("sdb -s %s shell su -c %s" %(PARAMETERS.device,"\"cyad -s -k MANIFESTS_GLOBAL -c User::Pkg::Tizen.Privilege.Tests -u '*' -p http://tizen.org/privilege/contact.read -t DENY\""))
                doRemoteCMD("rm -rf %s/%s" % (SRC_DIR, file))
                for line in output:
                    if "Failure" in line:
                        action_status = False
                        break


    return action_status



def main():
    try:
        usage = "usage: inst.py -i"
        opts_parser = OptionParser(usage=usage)
        opts_parser.add_option(
            "-m", dest="mode", action="store", help="Specify mode")
        opts_parser.add_option(
            "-s", dest="device", action="store", help="Specify device")
        opts_parser.add_option(
            "-i", dest="binstpkg", action="store_true", help="Install package")
        opts_parser.add_option(
            "-u", dest="buninstpkg", action="store_true", help="Uninstall package")
        opts_parser.add_option(
            "-a", dest="user", action="store", help="User name")
        global PARAMETERS
        (PARAMETERS, args) = opts_parser.parse_args()
    except Exception, e:
        print "Got wrong option: %s, exit ..." % e
        sys.exit(1)

    if not PARAMETERS.user:
        PARAMETERS.user = "owner"
    if not PARAMETERS.mode:
        PARAMETERS.mode = "SDB"

    if PARAMETERS.mode == "SDB":
        if not PARAMETERS.device:
            (return_code, output) = doCMD("sdb devices")
            for line in output:
                if str.find(line, "\tdevice") != -1:
                    PARAMETERS.device = line.split("\t")[0]
                    break
    else:
        PARAMETERS.mode = "SSH"

    if not PARAMETERS.device:
        print "No device provided"
        sys.exit(1)

    user_info = getUSERID()
    re_code = user_info[0]
    if re_code == 0 :
        global XW_ENV
        userid = user_info[1][0]
        XW_ENV = "export DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/%s/dbus/user_bus_socket"%str(userid)
    else:
        print "[Error] cmd commands error : %s"%str(user_info[1])
        sys.exit(1)
    if PARAMETERS.binstpkg and PARAMETERS.buninstpkg:
        print "-i and -u are conflict"
        sys.exit(1)

    if PARAMETERS.buninstpkg:
        if not uninstPKGs():
            sys.exit(1)
    else:
        if not instPKGs():
            sys.exit(1)

if __name__ == "__main__":
    main()
    sys.exit(0)
