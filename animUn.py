#!user/bin/env python
# -*- coding: utf-8 -*-

#分解 由cocosAnimation 打包成的大图
import sys
import os
import xml.etree.ElementTree as ET
from PIL import Image

__author__ = "qingsen"

def getImgsInfo(plistFile):
    images = []

    root = ET.fromstring(open(plistFile,'r').read())
    frames = root[0][1]
    for imageName in frames.findall("key"):  #findall :当前标签 所有子key标签
        name = imageName.text
        pos = imageName.text.find("/")

        if pos >= 0 :
            name = imageName.text[pos+1:len(imageName.text)]
        print name
        d = {}
        d["imageName"] = name
        images.append(d)
    print len(images) 
    i = 0
    for info in frames.findall("dict"): 
        images[i]["width"] = int(info[1].text)
        images[i]["height"] = int(info[3].text)
        images[i]["x"] = int(info[9].text)
        images[i]["y"] = int(info[11].text)
        i = i + 1
    return images

def decompos(plistFile,pngFile):
    bigImage = Image.open(pngFile)
    images= getImgsInfo(plistFile)

    fileName = plistFile.replace(".plist","")
    for image in images:  
        box = (image["x"],image["y"],image["x"]+image["width"],image["y"]+image["height"])
        childImg = bigImage.crop(box)

        if not os.path.isdir(fileName):
            os.mkdir(fileName)
        outfile = (fileName+'/' + image["imageName"])
        print outfile
        childImg.save(outfile) 
        

def main():
    # python decompos.py 文件名 （会在同级目录下生成同名的文件夹放分解后的小图）
    args = sys.argv
    if len(args) == 2:
        filename = args[1]
        plistFile = filename + '.plist'
        pngFile = filename + '.png'
        if (os.path.exists(plistFile) and os.path.exists(pngFile)):
            decompos(plistFile,pngFile)
        else:
            print u"文件缺失"
    else:
        print u"输入要分解的文件名"

if __name__ == '__main__':
    main()