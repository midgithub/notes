#!/usr/bin/env python
# -*- coding: utf-8 -*-

def main():
	print "\n"
	#raw_input 返回的都是字符串
	num = raw_input("输入数字:".decode('utf-8').encode('gbk'))  #避免乱码 
	if num.isdigit():
		num = int(num)
		a=2
		while a < num :
			if (num%a) == 0:
				break
			a = a + 1

		if num == a :
			print u"为素数"
		elif num == 0 or num == 1:
			print u"不是素数"
		else:
			print u"=%dx%d 不是素数" % (a,num/a)
	elif num.lower() == "xu1fan":
		print "xu1fan, rui sou sou xi dou xi la, sou la xi xi xi xi la xi la sou"
	else:
		print u"输入错误"

	main()

if __name__ == '__main__':
	print u"判断一个数是否为素数"
	main()
	