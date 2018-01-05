#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import sys
import shutil
import subprocess

__author__ = "sen"

def main(argv):
    if len(argv) != 3:
        print "args failed"
        return
    else:
        pathDst = argv[1]
        pathDst = pathDst.replace("\\", "/")

        pull = argv[2] == '0'

        if os.path.exists(pathDst):
            if os.path.isdir(pathDst):
                shutil.rmtree(pathDst)

            if os.path.isfile(pathDst):
                return

        protoDir = os.path.join(sys.path[0],"AL-proto")
        
        cmd = "git clone http://git."
        cwd = sys.path[0]
        if pull:        
            cmd = "git pull"
            cwd = protoDir

        try:
            retcode = subprocess.check_call(cmd, cwd = cwd)
            if retcode == 0:
                copyRes(protoDir,pathDst)
        except Exception, e:
            pass
        

def copyRes(path,toPath):
    path = path.replace("\\", "/")
    toPath = toPath.replace("\\", "/")

    for f in os.listdir(path):
        if os.path.isdir(os.path.join(path, f)):
            if f != ".git":
                shutil.copytree(os.path.join(path, f), os.path.join(toPath, f))


if __name__ == '__main__':
	main(sys.argv)