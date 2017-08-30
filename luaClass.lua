Father={}
Father.numBer = 0  --类属性，所有对象共享  调用用类名：Role.numBer

--lua提供了一种更为便利的语法，即将点（.）替换为冒号（：），这样可以在定义和调用函数时隐藏参数。
function Father:new(name)  --Father.new(self,name)
  self.name = name
  self.lovenumber = 0
  self.__index=self
  Father.numBer = Father.numBer + 1

  opt = {}     
  --将新对象实例的元表指向Father，这样就可以以Father为模板了
  setmetatable(opt,self)
  --将Father的__index字段指向自己，以便新对象在找不到指定的key时可以被重定向，即访问Father拥有的key
  return opt
end

function Father:toString()
  print("I love my son!"..self.name)
end

function Father:Loving(v)
   self.lovenumber=self.lovenumber+v 
   return self.lovenumber
end

local f1=Father:new("jianjian")
print(f1:toString())
print(f1:Loving(100))
print(f1.name.."Father.numBer"..Father.numBer)

local f2=Father:new("baba")
print(f2:toString())
print(f2:Loving(200))
print(f1.name.."Father.numBer"..Father.numBer)

--单例
function Father:Instance()  
    if self.instance == nil then  
        self.instance = self:new()  
    end  
    return self.instance  
end 

local s1 = Father:Instance()  
local s2 = Father:Instance()  
if s1 == s2 then  
    print("两个对象是相同的实例")  
end  

--下面派生出Father的一个子类，此时的Son仍为Father的一个对象实例
Son=Father:new()
--重写Father中的toString方法，以实现自定义功能
function Son:toString()
   print("I love myself!")
end

--在执行下面的new方法时，table s的元表已经是Son了，而不是Father
s=Son:new()
print(s:toString()) --先在子类Son中找到该方法
print(s:Loving(50)) --子类中无该方法，则调用父类中该方法
--输出答案
--I love myself！
--50