#!/usr/bin/env python
# -*- coding: utf-8 -*-

import socket
import threading
import time

#服务器要能够区分一个Socket连接是和哪个客户端绑定的。
#唯一 Socket依赖4项：服务器地址、服务器端口、客户端地址、客户端端口。
#服务器 同时响应多个客户端的请求，所以每个连接都需要一个新的进程或者新的线程来处理

print 'thread %s is running...' % threading.current_thread().name

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# 监听端口:
s.bind(('127.0.0.1', 9999)) #127.0.0.1表示本机地址，如果绑定到这个地址，客户端必须同时在本机运行才能连接，外部的计算机无法连接进来。

s.listen(5) #始监听端口 指定等待连接的最大数量
print "waiting for client request connect"

def tcplink(sock, addr):
    #print 'Accept new connection from %s:%s...' % addr
    sock.send('Welcome!')

    while True: #等待客户端数据
        data = sock.recv(1024)
        time.sleep(1)  
        if data == 'exit' or not data:
            break
        sock.send('Hello, %s!' % data)

    sock.close()
    print 'Connection from %s:%s closed.' % addr

while True:
	#client 请求连接
	sock,addr = s.accept()
	t = threading.Thread(target=tcplink, name="new thread", args=(sock, addr)) # 创建新线程来处理TCP连接:
    
	t.start()
	
    





