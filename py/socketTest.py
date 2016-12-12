#!/usr/bin/env python
# -*- coding: utf-8 -*-

#IP地址对应的实际上是计算机的网络接口
#IP协议负责把数据从一台计算机通过网络发送到另一台计算机。数据被分割成一小块一小块，然后通过IP包发送出去。由于互联网链路复杂，两台计算机之间经常有多条线路，因此，路由器就负责决定如何把一个IP包转发出去。
#IP包的特点是按块发送，途径多个路由，但不保证能到达，也不保证顺序到达。

#TCP协议负责在两台计算机之间建立可靠连接，保证数据包按顺序到达。
#TCP协议会通过握手建立连接，然后，对每个IP包编号，确保对方按顺序收到，如果包丢掉了，就自动重发。

#每个网络程序都向操作系统申请唯一的端口号，两个进程在两台计算机之间建立网络连接就需要各自的IP地址和各自的端口号。
#一个Socket需要知道目标计算机的IP地址和端口号，再指定协议类型即可。
import socket

#AF_INET指定使用IPv4协议，如果要用更先进的IPv6，就指定为AF_INET6。SOCK_STREAM指定使用面向流的TCP协议
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('www.sina.com.cn', 80))  #地址和端口号 80端口是Web服务的标准端口  SMTP服务是25端口，FTP服务是21端口

s.send('GET / HTTP/1.1\r\nHost: www.sina.com.cn\r\nConnection: close\r\n\r\n') #发送数据:

#接收数据
dataBuffer = []
while True:
	d = s.recv(1024)
	if d:
		dataBuffer.append(d)
	else:
		break
data = "".join(dataBuffer)

s.close()

header,html = data.split('\r\n\r\n', 1)
print header

with open('sina.html', 'wb') as f:
    f.write(html)