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
sortedDicRes = {}
configTable = {}
minutes = []
platforms = ["android"]
curPlat = "android"
copys = []
resDir = ""
maxMinute = 0

def ReadConfig():
    config = os.path.join(curDir,"Config/config.json")
    if not os.path.exists(config):
        print u"config.json 配置文件不存在"
        return False
    
    print u"read config:" + config

    global configTable
    global platforms
    with open(config, "r") as configFile:
        configTable = json.load(configFile)
        platforms = configTable["platforms"]

        return True

def ReadRecords():
    global sortedDicRes
    sortedDicRes = []

    dicRes = {}
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

    sortedDicRes = sorted(dicRes.items(), key=lambda d:d[0])

    if 0 == num:
        print u"首包资源记录文件不存在"

def CopyFirsRes():
    global maxMinute
    global copys
    copys = []

    lastMinute = 0
    for minute in minutes: 
        for item in sortedDicRes: 
            minu = item[0]
            if (minu > lastMinute) and (minu <= int(minute)):
                for res in item[1]: 
                    if not (res in copys):
                        copys.append(res)
                        maxMinute = int(minute)
                        CopyFile(res, lastMinute, minute)

            elif minu > int(minute) :  
                lastMinute = int(minute)   
                break  

    # for res in copys:
    #     CopyFile(res, 0, str(maxMinute))

def CopyOtherRes():
    allRes = []
    filterFile = [".meta", ".manifest"]

    for root,dirs,files in os.walk(resDir):
        if not "Lua_Bundles" in root:
            for file in files:
                filePath = os.path.join(root,file)
                ftype = os.path.splitext(filePath)[-1]
                if not ftype in filterFile:
                    allRes.append(filePath.replace(resDir,"").replace("\\","/")) 

    for res in allRes:
        if not (res in copys):
            CopyFile(res, maxMinute, "end")

def CopyFile(res, start, end):
    destinyFile = resDir + res
    if not os.path.exists(destinyFile):
        raise ValueError('res not exist: ' + destinyFile)
    else:
        toDir = "%s/%d-%s" % (firstDir + curPlat, start+1, end)
        topath = os.path.join(toDir,res).replace("\\","/")
        if not os.path.exists(os.path.dirname(topath)):
            os.makedirs(os.path.dirname(topath))

        print u"拷贝首包文件：" + destinyFile
        if not os.path.exists(topath):
            shutil.copyfile(destinyFile,topath)

def ClearDir():
    for plat in platforms: 
        if os.path.exists(firstDir + plat):
            shutil.rmtree(firstDir + plat)

if __name__ == '__main__':
    curDir = sys.path[0]

    configDir = curDir + "/Config/"
    configDir = configDir.replace("\\","/")

    firstDir = os.path.dirname(curDir)+"/FirstRes_"
    firstDir = firstDir.replace("\\","/")

    if ReadConfig():
        ClearDir()
        for plat in platforms: 
            curPlat = plat
            minutes = configTable[curPlat]
            print minutes

            resDir = (os.path.dirname(curDir) + "/" + curPlat + "/").replace("\\","/")
            ReadRecords()
            CopyFirsRes()
            CopyOtherRes()

    print u"首包资源处理完毕-----------"
	