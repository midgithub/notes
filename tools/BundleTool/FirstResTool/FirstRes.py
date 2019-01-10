#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import sys
import json
import shutil
import codecs
import time

__author__ = "qingsen"

curDir = ""
configDir = ""
firstDir = ""
dicRes = {}
minutes = []
platforms = ["android"]
curPlat = "android"
copys = []


def ReadRecords():
    dicRes.clear()
    num = 0
    path = configDir + curPlat
    for f in os.listdir(path):
        if os.path.isfile(os.path.join(path,f)):
            fileType = os.path.splitext(f)[-1]
            if fileType == ".log" :
                num = num + 1
                print u"读取首包记录文件：" + os.path.join(path,f).replace("\\","/")
                with codecs.open(os.path.join(path,f).replace("\\","/"), "r", "utf-8-sig") as recordFile:
                    for line in recordFile.readlines(): 
                        lis = line.strip().split(":")
                        minu = lis[0].replace("minute","")
                        #print "minute: %d  res: %s" % (int(minu),lis[1])
                        if not "Lua_Bundles" in lis[1]:
                            if not (int(minu) in dicRes):
                                dicRes[int(minu)] = []

                            if not (lis[1] in dicRes[int(minu)]):    
                                dicRes[int(minu)].append(lis[1])

    if 0 == num:
        print u"首包资源记录文件不存在"

def ReadConfig():
    global minutes
    minutes = []

    config = configDir + curPlat + "/config.txt"
    if not os.path.exists(config):
        print u"config.txt 配置文件不存在"
        return False
    
    print u"读取筛选时间配置文件：" + config
    with codecs.open(config, "r", "utf-8-sig") as configFile:
        timestr = configFile.read().strip() 
        minutes = timestr.split(",")

def CopyFirsRes():
    global copys
    copys = []

    lastMinute = 0
    maxMinute = 0
    for minute in minutes: 
        for minu in dicRes: 
            if (minu > lastMinute) and (minu <= int(minute)):
                for res in dicRes[minu]: 
                    if not (res in copys):
                        copys.append(res)
                        maxMinute = int(minute)
                        CopyFile(res, lastMinute, minute)

            elif minu > int(minute) :  
                lastMinute = int(minute)   
                break  

    for res in copys:
        CopyFile(res, 0, maxMinute)

def CopyFile(res, start, end):
    resDir = curDir.replace("FirstResTool", curPlat + "/").replace("\\","/")
    destinyFile = resDir + res
    if not os.path.exists(destinyFile):
        raise ValueError('res not exist: ' + destinyFile)
    else:
        toDir = "%s/%d-%d" % (firstDir + curPlat, start+1, int(end))
        topath = os.path.join(toDir,res).replace("\\","/")
        if not os.path.exists(os.path.dirname(topath)):
            os.makedirs(os.path.dirname(topath))

        print u"拷贝首包文件：" + res
        if not os.path.exists(topath):
            shutil.copyfile(destinyFile,topath)

def ClearDir():
    for plat in platforms: 
        if os.path.exists(firstDir + plat):
            shutil.rmtree(firstDir + plat)
        os.mkdir(firstDir + plat)

if __name__ == '__main__':
    curDir = sys.path[0]
    print curDir

    configDir = curDir + "/Config/"
    configDir = configDir.replace("\\","/")

    firstDir = curDir.replace("FirstResTool","") + "/FirstRes_"
    firstDir = firstDir.replace("\\","/")

    ClearDir()
    for plat in platforms: 
        curPlat = plat
        ReadRecords()
        ReadConfig()
        CopyFirsRes()

    print u"首包资源处理完毕-----------"
	