#!/usr/bin/env python
# -*- coding: utf-8 -*-

import socket
#UDP协议不需要建立连接，只需对方的IP地址和端口号，就可以直接发数据包。速度快但是能不能到达就不知道了

s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
for data in ['Michael', 'Tracy', 'Sarah']:
    # 发送数据:
    s.sendto(data, ('127.0.0.1', 9999))
    # 接收数据:
    print s.recv(1024)
s.close()

