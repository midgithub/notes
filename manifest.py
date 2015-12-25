#!user/bin/env python
# -*- coding: utf-8 -*-

import os
import json
import hashlib

#资源清单生成

dic = {
  "assets": {},
  "engineVersion": "3.6", 
  "packageUrl": "http://rbugs.v.pranagames.net:85/rbugs4/", 
  "remoteManifestUrl": "http://rbugs.v.pranagames.net:85/rbugs4/project.manifest", 
  "remoteVersionUrl": "http://rbugs.v.pranagames.net:85/rbugs4/version.manifest", 
  "searchPaths": [], 
  "version": "0.1.9.3020"
}

assets ={}

def md5Fils(path):
	path = path.replace("\\","/")
	if not os.path.isdir(path) and not os.path.isfile(path):
		return false

	if os.path.isfile(path):
		with open(path,"r") as curFile:
			fileStr = curFile.read()
	        m = hashlib.md5()
	        m.update(fileStr)

	        md5Str = str(m.hexdigest())
	        d = dict(md5 = md5Str)
	        key = path[len(os.getcwd())+1:-1]
	        assets[key] = d

	elif os.path.isdir(path):
		for f in os.listdir(path):
			md5Fils(os.path.join(path,f))
    			
def main():
	md5Fils(os.path.join(os.getcwd(),"res"))
	md5Fils(os.path.join(os.getcwd(),"src"))
	dic["assets"] = assets
	with open("project.manifest","w") as manifest: 
		manifest.write(json.dumps(dic,indent=1)) 
   

if __name__ == '__main__':
	main()