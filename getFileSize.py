#!user/bin/env python
# -*- coding: utf-8 -*-

import os

#打印当前目录下所有子文件文件的大小
__author__ = "qingsen"

def main():
	curPath = os.getcwd()
	print curPath

	for f in os.listdir(curPath):
		path = os.path.join(curPath,f)
		if os.path.isfile(path):
			print u"文件:%s 大小------------%d b" % (f,os.path.getsize(path))
		elif os.path.isdir(path): 
			size = 0
			for root, dirs, files in os.walk(path):  
			    size += sum([os.path.getsize(os.path.join(root, name)) for name in files])  
			print u"文件夹:%s 大小------------%d kb" % (f,size/1024.0) 

	os.system("pause")

if __name__ == '__main__':
	main()
		

