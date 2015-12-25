#!user/bin/env python
# -*- coding: utf-8 -*-

#文件复制,移除,重命名

import os
import shutil

def copyCsb(path):
	num = 0
	print u"复制最新csb文件："
	path = path.replace("\\","/")
	for f in os.listdir(path):
		if os.path.isfile(os.path.join(path,f)):
			fileType = os.path.splitext(f)[-1]
			if fileType == ".csb" :
				shutil.copyfile(os.path.join(path,f),os.path.join(toPath,f))
				num = num + 1
				print "  "+os.path.join(path,f)
				#重命名
				#os.rename(os.path.join(path,f),path+"/"+os.path.splitext(f)[0]+"sen."+fileType)

	print u"最新csb文件总数：%d" % (num)

def copyRes(path):
	num = 0
	print u"复制最新文件夹："
	path = path.replace("\\","/")
	for f in os.listdir(path):
		if os.path.isdir(os.path.join(path,f)):
			shutil.copytree(os.path.join(path,f),os.path.join(toPath,f))
			num = num + 1
			print "  "+os.path.join(path,f)
	print u"最新文件夹总数：%d" % (num)


def removeCsb(path):
	num = 0
	print u"移除旧csb文件："
	for f in os.listdir(path):
		if os.path.isfile(os.path.join(path,f)):
			fileType = os.path.splitext(f)[-1] 
			if fileType == ".csb" :
				os.remove(os.path.join(path,f))
				num = num + 1
				print "  "+os.path.join(path,f)
	print u"旧csb文件总数：%d" % (num)

def removeDir(path):
	num = 0
	print u"移除旧文件夹："
	for f in os.listdir(path):
		if os.path.isdir(os.path.join(path,f)):
			shutil.rmtree(os.path.join(path,f))
			num = num + 1
			print "  "+os.path.join(path,f)
	print u"旧文件夹总数：%d" % (num)


def removeRes(path):
	if os.path.exists(path):
		shutil.rmtree(path)
	os.mkdir(path)

toPath = "D:\work\qzc\\rbugs\\res" 
toPath = toPath.replace("\\","/")
# removeCsb(toPath)
# removeDir(toPath)
removeRes(toPath)

res = ["D:\work\qzc\qzc\\res","D:\work\qzc\qzc\cocosstudio"]
copyCsb(res[0])
copyRes(res[1])

