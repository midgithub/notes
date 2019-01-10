#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import sys
import json
import shutil
import time
import hashlib

__author__ = "qingsen"

curDir = ""
channel = "android"
apkVersion = "2.5.0"
resVersion = 1000
versionDirs = []
versionReses = {}
filterFile = [".meta", ".manifest"]
tool = ""

def ReadConfig():
    config = os.path.join(curDir,"Config/config.json")
    if not os.path.exists(config):
        print u"config.json 配置文件不存在"
        return False
    
    print u"读取配置文件： " + config

    global channel
    global apkVersion
    global resVersion
    with open(config, "r") as configFile:
        table = json.load(configFile)
        channel = table["channel"]
        apkVersion = table["apkVersion"]
        resVersion = table["resVersion"]

        return True

def ReadVersionDirs():
    global versionDirs
    versionDirs = []

    target = "Channels/%s/%s" % (channel, apkVersion)
    targetDir = os.path.join(curDir,target)
    for f in os.listdir(targetDir):
        child = os.path.join(targetDir,f)
        if os.path.isdir(child):
            if not "output" in f:
                versionDirs.append(int(f))
     
    versionDirs.sort() 

def ReadAllVersionRes():
    for version in versionDirs:
        target = "Channels/%s/%s/%d" % (channel, apkVersion, version)
        targetDir = os.path.join(curDir,target)
        versionReses[version] = {}
        ReadDirRes(targetDir,version)

def ReadDirRes(targetDir,curVersion):
    for f in os.listdir(targetDir):
        child = os.path.join(targetDir,f)
        if os.path.isdir(child):
            ReadDirRes(child,curVersion)
        else:
            ftype = os.path.splitext(child)[-1]
            if not ftype in filterFile:
                fileName = child.replace("\\","/").split(str(curVersion)+"/")[1]
                versionReses[curVersion][fileName] = GetFileMd5(child)
                # print curVersion,fileName,versionReses[curVersion][fileName]

def GetFileMd5(file):
    with open(file, "rb") as targetFile:
        content = targetFile.read()
        fmd5 = hashlib.md5(content)
        return fmd5.hexdigest()

def GetDifResWithPreVersion():
    for version in versionDirs:
        if version < resVersion:
            print u"比对 %s %s 资源版本 %d 和 %d" % (channel, apkVersion, version, resVersion)
            CompareRes(version, resVersion)

def CompareRes(minVersion,curVersion):
    target = "Channels/%s/%s/%d" % (channel, apkVersion, minVersion)
    minDir = os.path.join(curDir,target)
    if not os.path.exists(minDir): 
        print u"资源目录不存在： %s" % (minDir)
        return

    target = "Channels/%s/%s/%d" % (channel, apkVersion, curVersion)
    maxDir = os.path.join(curDir,target)
    if not os.path.exists(maxDir): 
        print u"资源目录不存在： %s" % (maxDir)
        return

    output = "Channels/%s/%s/output/%d/%d_%d" % (channel, apkVersion, curVersion, minVersion, curVersion)
    outputDir = os.path.join(curDir,output)
    if os.path.exists(outputDir):
        shutil.rmtree(outputDir)
    os.makedirs(outputDir)

    difRes = [] 
    count = 0   
    for res in versionReses[curVersion]:
        count = count + 1
        print u"比对文件： %s" % (res)

        maxMd5 = versionReses[curVersion][res]
        minMd5 = ""
        if res in versionReses[minVersion]:
            minMd5 = versionReses[minVersion][res]
        
        if maxMd5 != minMd5:
            difRes.append(res)
            topath = os.path.join(outputDir,res)
            if not os.path.exists(os.path.dirname(topath)):
                os.makedirs(os.path.dirname(topath))

            if not os.path.exists(topath):
                shutil.copyfile(os.path.join(maxDir,res),topath)

    print u"比对文件数量： %d" % (count)
    difRes.sort()
    with open(os.path.join(outputDir,"difRes.txt"), "w") as difResFile:
        for res in difRes:
            difResFile.write(res + '\n')
            # difResFile.writelines(difRes)

    zipDir = "Channels/%s/%s/output/%d" % (channel, apkVersion, curVersion)
    zipName = "%d_%d" % (minVersion, curVersion)
    cmd = "%s %d %s/ %s/ %s" % (tool, 1, outputDir, os.path.join(curDir,zipDir), zipName)
    cmd = cmd.replace("\\","/")
    os.system(cmd)

if __name__ == '__main__':
    curDir = sys.path[0]
    tool = "Tool/test.exe"
    tool = os.path.join(curDir,tool).replace("\\","/")

    startTime = time.time()
    if ReadConfig():
        ReadVersionDirs()
        if resVersion in versionDirs: 
            ReadAllVersionRes()
            GetDifResWithPreVersion()
        else:
            print u"没有配置 Channels/%s/%s/%d 对应的资源目录" % (channel, apkVersion, resVersion)

    print u"资源比对处理完毕,耗时%d秒-----------" % (time.time()-startTime)
	