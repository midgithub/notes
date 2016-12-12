#!/usr/bin/env python
# -*- coding: utf-8 -*-

#第一行注释是为了告诉Linux/OS X系统，这是一个Python可执行程序，Windows系统会忽略这个注释；
#第二行注释是为了告诉Python解释器(运行python就是启动CPython解释器 解释型语言,运行速度慢,不能加密)，按照UTF-8编码读取源代码，否则，你在源代码中写的中文输出可能会有乱码。

# cmd C:\Python27\Scripts 执行pip install packname 或 easy_install packname
import functools
import sys   #import sys as "别名"
import os
#sys.path.append("..") 添加要搜索的目录

try:
    import cStringIO as StringIO
except ImportError: # 导入失败会捕获到ImportError
    import StringIO

#模块名就是文件名，可以用包来组织模块(避免模块名冲突)
#每一个包目录下面都会有一个__init__.py的文件，这个文件是必须存在的，否则，Python就把这个目录当成普通目录，而不是一个包
#__init__.py可以是空文件，也可以有Python代码，因为__init__.py本身就是一个模块，而它的模块名就是包名。

#作用域
#类似__xxx__这样的变量是特殊变量，可以被直接引用，但是有特殊用途
#类似_xxx和__xxx这样的函数或变量就是非公开的（private）

__author__ = "qingsen"


#  my test start
name = raw_input("enter your name:")
print type(name)   # <type "str"> if type(name) == type("abc"); isinstance(name,str)  isinstance(x, (int, float))
if name == "sen" :
	print "qingsen"
elif name == "ping" :
	print "ily ping"
else:
	print "unknow"

bol = True or False # or,and,not

a = "ABC"
b = a
a = "DEF"
print "a="+a+"  b="+b # a = "DEF"  b = "ABC"

a = 10/3  #3
a = 10.0/3  #3.333....

print ord("A")  #to Unicode num  65
print chr(65)   #to Unicode char A
print u"中文"

str1 = "i and %s %d" % ("you",37)
print str1


#列表 (可以改变内容的数组)  从 0 开始
#是一种有序的集合，可以随时添加和删除其中的元素 []
aList = [1,2,3,4]
print sum(aList) #10
a1 = aList[2:] #子集：[3,4]
a2 = aList[:2] #子集：[1,2]
#切片运算符 [:] 是左闭合,右开
aList[1] = 5 # [1,5,3,4]
print aList
del aList[1] 
# list.remove(obj) list.append(obj)  list.extend(seq)  list.insert(index,obj) list.pop(index) 
#for i, value in enumerate(['A', 'B', 'C']):  enumerate函数可以把一个list变成索引-元素对
#    print i, value


#元祖 测试  (不可以改变内容的数组：指向不变)  ()
aTuple = ("name1","name2","name3","name4")
# aTuple[1] = "name4" 报错
for value in aTuple:  #内建函数 sorted,reversed,enumerate,   zip(list1,list2)同时迭代两个序列
	print value
for index in range(len(aTuple)):  #range() 函数返回[0,1,2,3] 序列
	print index,aTuple[index]
for index,value in enumerate(aTuple):
	print "%d %s " % (index,value)
print range(3)  #[0,1,2]  star默认0，step默认1
print range(1,6) #[1,2,3,4,5]
print range(1,6,2) #[1,3,5]               


#字典 {} table （key:value） key要用双引号
aDict = {"name":"sen","id":37}
for key in aDict:    #for value in d.itervalues()   for k, v in d.iteritems()
	print key,aDict[key]
r = aDict.get("grad",-1)  #get方法，如果key不存在，可以返回None，或者自己指定的value
                          #aDict.pop(key)
print r

s = set([1, 1, 2, 2, 3, 3]) #set和dict类似,也是一组key的集合,但不存储value,由于key不能重复，所以在set中，没有重复的key。重复元素在set中自动被过滤
print s
#s.add(key)  s.remove(key)

#不变对象来说（如str），调用对象自身的任意方法，也不会改变该对象自身的内容。相反，这些方法会创建新的对象并返回，这样，就保证了不可变对象本身永远是不可变的。
#>>> isinstance('abc', Iterable) # isinstance str是否可迭代
#True
 #isinstance(123, Iterable) # 整数是否可迭代
#False

#列表生成式
print[x * x for x in range(1, 11)] #[1,4,9,...]
print[x * x for x in range(1, 11) if x % 2 == 0]

#一边循环一边计算的机制，称为生成器（Generator）
#generator保存的是算法，每次调用next()，就计算出下一个元素的值，直到计算到最后一个元素，没有更多的元素时，抛出StopIteration的错误。
g = (x * x for x in range(1, 11))
print g.next() #1
print g.next() #4

def fib(x):
	n,a,b=0,0,1
	while n < x :
		print "step:%d" % (n+1)
		yield b
		a,b = b,a+b
		n = n +1
f = fib(5) #每次调用next()的时候执行，遇到yield语句返回，再次执行时从上次返回的yield语句处继续执行。
f.next()
for n in fib(5):
	print n

def fun1():   #数据类型转换函数 int() str()
	pass  #无操作

def myAbs(x):
	if not isinstance(x,(int,float)):  #isinstance(obj1,obj2) obj1是否继承 obj2(tuple,dict,int,float)
		raise TypeError("bad operand type")  #通过raise显示地引发异常。一旦执行了raise语句，raise后面的语句将不能执行。
	if x >= 0 :
		return x ,"myAbs"
	else:
		return -x ,"myAbs"

r1,r2 = myAbs(-2) #函数可以返回多个值
print r1,r2
R = myAbs(-2)
print R # （2，"myAbs"） 返回多值其实就是返回一个tuple

def power(x,n=2):  #默认参数  默认参数必须指向不变对象！
	r = 1 
	while n>0:
		n = n -1
		r = r*x
	return r 

def sum(*prams):  #可以传入任意个参数
	s = 0
	for value in prams:
		s = s + value
	return s

print sum(1,2)
print sum(*(1,2,3)) #记得传参数时 *把list或tuple的元素变成可变参数传进去

  #*args是可变参数，args接收的是一个tuple；
  #**kw是关键字参数，kw接收的是一个dict

#注：由于abs函数实际上是定义在__builtin__模块中的
#所以要让修改abs变量的指向在其它模块也生效，要用__builtin__.abs = 10。

#map map()函数接收两个参数，一个是函数，一个是序列，map将传入的函数依次作用到序列的每个元素，并把结果作为新的list返回。
def fun2(x):
	return x*x
print map(fun2,range(4)) #[0,1,4,9]
print map(str,range(4))

#reduce(f, [x1, x2, x3, x4]) = f(f(f(x1, x2), x3), x4) 函数必须接收两个参数
def sum1(x,y):
	return x+y
print reduce(sum1,[1,2,3,4,5]) #求和 15

#filter()把传入的函数依次作用于每个元素，然后根据返回值是True还是False决定保留还是丢弃该元素。
def is_odd(x):
	return x%2 == 0
filter(is_odd,[1,2,3,4,5,6]) #[2,4,6]

def sort(x,y):
	if x > y:
		return 1  #降序
	elif x < y:
		return -1
	else: 
		return 0
print sorted([1,5,3,2,6,4],sort)

#匿名函数有个限制，就是只能有一个表达式，不用写return，返回值就是该表达式的结果。
f1 = lambda x: x*x

#在代码运行期间动态增加功能的方式，称之为“装饰器”（Decorator）。
#decorator就是一个返回函数的高阶函数

def log(func):
	def f(*args,**kw):
		print "call func %s()" % (func.__name__) #函数对象有一个__name__属性
		return func(*args,**kw)
	return f

@log  #@语法，把decorator置于函数的定义处
def func3():
	print "log 2015-11-17"

func3() #func3变量指向了新的函数(log 的返回函数 f)

#偏函数  把一个函数的某些参数给固定住
print int("10",2) #"10"是个2进制的值，请转换为10进制的值
myInt = functools.partial(int,base=2) #base 要写
print myInt("100")
print myInt("100",base=10) #base 要写




#OOP
#可以动态地给一个实例变量绑定属性,方法(对其它实例无作用)
#__slots__变量，来限制该类能添加的属性，对继承的子类是不起作用
#属性以__开头，就变成了一个私有属性
#不能直接访问__name是因为Python解释器对外把__name变量改成了_MyClass__name
class MyClass(object):
	act = "doAct" #实例可以访问到，但是方法里不能访问
	#self.attr = 0  这里不能用self. 来定义属性
	#__slots__ = ("attr1")
	def __init__(self,name): 
		self.doInit()
		self.__name = name
		self.__id = 37
	def __str__(self):
		return "MyClass instance"

	def __getattr__(self, attr): #在没有找到属性的情况下，才调用__getattr__
        if attr=='age':
            return lambda: 25
    def __call__(self):
        print('My name is %s.' % self.__name)  #实例本身上调用 如 c1()

	def doInit(self):
		print "init"
	def getName(self):  #第一个参数一定要写self
		#这里的self.__name == _MyClass__name，所以子类调用该接口会报没这个属性错误
		print "call getName " + self.__name,self.__id 
	def setName(self,name):
		self.__name = name
		print "newName" + self.__name

c1 = MyClass("c1")
print c1 #MyClass instance  因为print 调用的是该类的  __str__()方法
print c1.act
c1.getName()
c1.attr1 = "attr1"


class Child(MyClass):
	def __init__(self, name):
		self.__name = name

ch1 = Child("ch1")
print dir(ch1)  #列出属性和方法
ch1.setName("ch2")

class Fib(object):
    def __init__(self):
        self.a, self.b = 0, 1 # 初始化两个计数器a，b

    def __iter__(self):
        return self # 实例本身就是迭代对象，故返回自己

    def next(self):
        self.a, self.b = self.b, self.a + self.b # 计算下一个值
        if self.a > 100000: # 退出循环的条件
            raise StopIteration();
        return self.a # 返回下一个值

    def __getitem__(self, n):  #像list那样按照下标取出元素，需要实现__getitem__()方法
        a, b = 1, 1
        for x in range(n):
            a, b = b, a + b
        return a

for n in Fib():
	print n #1,1,2,3,5

#try 执行出错，则后续代码不会继续执行，直接跳转至except语句块处理，执行完except后，如果有finally语句块，则执行finally语句块
#当没有错误发生时，会自动执行else语句
try:
	pass
except ZeroDivisionError, e:
	raise ValueError('input error!') #抛出
else:
	assert n != 0, 'n is zero!' #假设为真，否则抛出AssertionError
finally:
	pass

try:
	fi = open("rtest.txt","r")
	#print fi.read() #如果执行这步.下面readlines 不执行
	print fi.readline().strip()
	for line in fi.readlines(): #readlines() 读取剩余的每行,是调用next()函数来实现遍历
		print "line:"+line.strip() # 把末尾的'\n'删掉
finally:
	if fi:
		fi.close()

#上面简写
with open("rtest.txt","r") as fi2:
    print fi2.read() #read()会一次性读取文件的全部内容，如果文件很大，内存就爆了
    				 #调用read(size)方法，每次最多读取size个字节的内容

#r:文件必须存在 w:先清空原本数据再写,不存在文件先创建文件,不可以读 a:同w,不过是续写
#f.closed  f.mode:访问模式 f.name:文件名称
#f.tell():文件读到哪个位置 f.seek(pos,isAbs):文件读取跳转到pos位置，isAbs是否相对当前(1:相对0:绝对)
with open("wtest.txt","w") as fi3: 
	print "tell:%d" % fi3.tell()
	fi3.seek(3,0)
	fi3.write("sen ilu") #写在缓冲区,少部分直接写进文件,close的时候写完,可以直接调用flush
	print "tell:%d" % fi3.tell()
	fi3.seek(1,1)
	fi3.write("ping")
	#fi3.writelines("ilu \nping") #传进seq

print os.getcwd() #当前Python脚本工作的目录路径
for f in os.listdir(os.getcwd()): #返回指定目录下的所有文件和文件夹名(不能读取文件夹里面的文件名)
	print f

#目录操作
import shutil
#os.mkdir("file") #创建新目录,该目录存在会报错
shutil.copyfile("shop.csb","file/shop.csb") #对象只能是文件,后面的文件(可以不存在,会创建新的)内容被前面替换
#shutil.copytree("files","newfile") #文件夹复制,newfile必须不存在
#os.rename(oldname,newname) #重命名文件（目录）
#shutil.move(oldpos,newpos) #移动文件（目录）

#os.remove("wtest.txt") #不能移除文件夹 ()
#shutil.rmtree("dir")   #空目录、有内容的目录都可以删

print os.path.basename("files/file1.txt")
print os.path.isdir("files")
print os.path.isfile("files/file1.txt")

dir_file = os.path.split("files/file1.txt") #路径的目录名和文件名分割
print dir_file[0],dir_file[1]
name_t = os.path.splitext(dir_file[1]) #文件名和类型分割
lis = dir_file[1].split(".")
print name_t[0],name_t[1],lis

#import hello2
try:
	import cPickle as pickle
except Exception, e:
	import pickle
d1 = dict(name="sen",age=24)
with open("pickle.txt","w") as fi4:
	fi4.write(pickle.dumps(d1)) #把任意对象序列化成一个str
with open("pickle.txt","r") as fi5:
	print pickle.load(fi5)  #反序列化出对象

import json
d1["telphone"] = "152"
with open("json.txt","w") as fi4:
	fi4.write(json.dumps(d1,indent=1)) 
with open("json.txt","r") as fi5:
	print json.load(fi5)  

def main():
	args = sys.argv
	if len(args) == 1:
		print 'welcome python world,%s' % (args[0]) #args[0] = "hello.py" 即文件名
	elif len(args) == 2:
		print 'welcome python world,%s' % (args[1]) #python hello.py sen, args[1] = sen
	else:
		print "too many args"

#命令行运行hello模块文件时，Python解释器把一个特殊变量__name__置为__main__，
#而如果在其他地方导入该hello模块时，if判断将失败，
if __name__ == '__main__':
	main()
	print "git add"
