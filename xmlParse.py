#!user/bin/env python
# -*- coding: utf-8 -*-

import xml.etree.ElementTree as ET

tree = ET.parse('test.xml')  #硬盘文件导入
print dir(tree) 

root = tree.getroot()   #根节点 第一层标签
#或者通过字符串导入:  root = ET.fromstring(open('test.xml','r').read())

#<data></data>   打印 data，{}
#<plist version="1.0"></plist>  打印 plist，{"version":"1.0"}
print root.tag,root.attrib  #根节点 有一个tag(data)以及一些列属性

for child in root:
	print child.tag,child.attrib  #子标签 和 其属性

#孩子节点是嵌套的，我们可以通过索引访问特定的孩子节点。
print root[0][1].text

for neighbor in root.iter("neighbor"):  #查找树 所有neighbor的标签
	print neighbor.attrib

#查找
for country in root.findall("country"):  #findall :当前标签 所有子country标签
	print country.attrib  #该标签 所有的属性 字典
	print country.find('rank').text  ##该标签 第一个子标签为rank 的文本内容
	print country.get('name')   #该标签 属性为“name”的值

# 添加修改
for rank in root.iter("rank"): 
	rank.text = str(int(rank.text) + 1)  #改变文本
	rank.set("updated","yes")  #添加或修改属性
tree.write("output.xml")  #根节点以上的内容没了

#移除
for country in root.findall("country"):
	if int(country.find('rank').text) > 50:
		root.remove(country)  
tree.write("output.xml")  #根节点以上的内容没了