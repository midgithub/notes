@echo off

echo 这是个判断一个数是否为素数的脚本
:start
echo.
set /p x=输入判定整数:
set /a n=x-1
set i=1
if %x% == 1 (
	echo 不是素数
    goto start
    exit
)
if %x% == 2 (
	echo 是素数
    goto start
    exit
)
echo %x%|findstr /be "[0-9]*" >nul && goto isPrime || goto isU
exit

:isPrime
set /a i+=1
set /a is=%x%%%%i%
set /a z=%x%/%i%
if  %is% equ 0 (
    echo =%i%*%z%
    echo 不是素数
    goto start
)
if %i% equ %n% (
    echo 是素数 
    goto start
)
goto isPrime
exit

:isU
if /i %x% == Xu1Fan (  ::/i 忽略大小写
    echo  Xu1Fan 你还不错哦! rui sou sou xi dou xi la   sou la xi xi xi xi la xi la sou
    goto start
)else ( ::else要放)后面 ( 和else 有空格
    echo 输入错误 
    goto start
) 
exit
