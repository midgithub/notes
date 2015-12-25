#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import time

def writeName(path):
	global num
	if not os.path.isdir(path) and not os.path.isfile(path):
		return false

	if os.path.isfile(path):
		dir_file = os.path.split(path)
		fileType = os.path.splitext(dir_file[-1])[-1]
		if fileType == ".lua" :
			with open("luaNames.txt","a") as fi:
				fi.write(dir_file[-1]+"\n")
				num = num +1
	elif os.path.isdir(path):
		for f in os.listdir(path):
			writeName(os.path.join(path,f))

num = 0
start = time.time()

path = "D:\work\qzc\\test"
path = path.replace("\\","/")
if os.path.exists(os.path.join(os.getcwd(),"luaNames.txt")):
	print "remove"
	os.remove("luaNames.txt")

writeName(path)

cost = time.time() - start

print u"lua文件数量：%d" % (num)
print u"消耗时间：%0.2f" % (cost)