#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import sys

__author__ = "sen"

def ModifyFileExt(path,ext):
    files = os.listdir(path)          #列出指定目录下的所有文件
    for f in files:
        curfile =os.path.join(path,f)
        if os.path.isdir(curfile):                 
            ModifyFileExt(curfile,ext)

        if os.path.isfile(curfile):  
            fname = os.path.splitext(f)[0]
            ftype = os.path.splitext(f)[-1]
            if ftype == ext:
                newname = os.path.join(path,fname) 
                print "modify file: "+curfile+" to " + newname
                os.rename(curfile,newname)          


if __name__ == '__main__':
	ModifyFileExt(sys.path[0],".txt")
	