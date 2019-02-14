#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import sys
import json
import shutil
import time

__author__ = "qingsen"

curDir = ""
channels = []
filterFile = [".meta", ".manifest"]

def ReadConfig():
    config = os.path.join(curDir,"Config/config.json")
    if not os.path.exists(config):
        print u"config.json 配置文件不存在"
        return False
    
    print u"读取配置文件： " + config

    global channels
    with open(config, "r") as configFile:
        table = json.load(configFile)
        channels = table["channels"]

        return True

def RenameOriginalRes(targetDir):
    for f in os.listdir(targetDir):
        child = os.path.join(targetDir,f)
        if os.path.isdir(child):
            RenameOriginalRes(child)
        else:
            ftype = os.path.splitext(child)[-1]
            if not ftype in filterFile:
                dirs = child.replace(curDir + "\\","").replace("\\","/").split("/")
                print u"重命名资源：" + child

                for channel in channels:
                    fileName = ""
                    for i, d in enumerate(dirs): 
                        if i != len(dirs)-1:
                            fileName += "%s_%s/" % (d, channel)
                        else:
                            if "." in d:
                                fileName += "%s_%s.%s" % (d.split(".")[0], channel, d.split(".")[1]) 
                            else:
                                fileName += d

                    topath = os.path.join(curDir,fileName)
                    if not os.path.exists(os.path.dirname(topath)):
                        os.makedirs(os.path.dirname(topath))

                    if not os.path.exists(topath):
                        shutil.copyfile(child,topath)

def  del_file(path):
    for i in os.listdir(path): 
        path_file = os.path.join(path,i)  
        if os.path.isfile(path_file):
            os.remove(path_file)
        else:
            del_file(path_file)

if __name__ == '__main__':
    curDir = sys.path[0]

    startTime = time.time()
    if ReadConfig():
        for channel in channels:
            channelDi = "OriginalRes_%s" % (channel)
            channelDir = os.path.join(curDir,channelDi)
            
            if os.path.exists(channelDir):
                del_file(channelDir)

        targetDir = os.path.join(curDir,"OriginalRes")
        RenameOriginalRes(targetDir)
      
    print u"资源复制重命名完毕,耗时%d秒-----------" % (time.time()-startTime)
	