

#!/usr/bin/python
# encoding: utf-8 

import os,shutil,hashlib,time

fileexlist=['_dyb','_xy']
base_old_path = os.getcwd()
print 'base_old_path', base_old_path

def change_md5(filename,path):
	with open(os.path.join(path,filename),'rb') as f:
		md5obj = hashlib.md5()
		md5obj.update(f.read())
		hash = md5obj.hexdigest()
		print(hash,type(hash),' -- ',filename)
	with open(os.path.join(path,filename),'a') as f:
		#f.write("####&&&&%%%%")
		f.write("\n")
		f.close()
	with open(os.path.join(path,filename),'rb') as f:
		md5obj = hashlib.md5()
		md5obj.update(f.read())
		hash = md5obj.hexdigest()
		print(hash,type(hash),' -- ',filename)
	return hash

for flex in fileexlist:
	new_bundle_path = base_old_path + flex
	if (os.path.exists(new_bundle_path)):
		shutil.rmtree(new_bundle_path)
	shutil.copytree(base_old_path,new_bundle_path)
	lpath=[]
	for dirpath, dirnames, filenames in os.walk(new_bundle_path):
		if os.path.isdir(dirpath):
			lpath.append(dirpath)
			#print 'Directory', dirpath
		dirpathfull = dirpath +'/'

		for filename in filenames:
			#print ' File', filename
			portion = os.path.splitext(filename)
			if portion[1] == '.unity3d':
				#print ' File', portion[0]
				oldname=dirpathfull+filename
				#print ' oldname', oldname
				newname=dirpathfull+portion[0]+flex+'.unity3d'
				#print ' NewName', newname
				change_md5(filename,dirpath)
				os.rename(oldname,newname)

	lpath.sort(key = lambda i:len(i),reverse=True)
	
	for idir in lpath:
		newidir=idir+flex
		#print 'Directory', idir
		print 'New Directory', newidir
		os.rename(idir,newidir)
