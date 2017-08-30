# coding:utf8
import mysql.connector  #安装 mysql-connector-python-2.1.5-py2.7-winx64.msi

conn = mysql.connector.connect(user='root', password='102261013028', database='db_sen', use_unicode=True)
cursor = conn.cursor() # 创建游标
cursor.execute('create table user (id varchar(20) primary key, name varchar(20))')
cursor.execute('insert into user (id, name) values (%s, %s)', ['1', 'Michael'])
print cursor.rowcount

conn.commit()
cursor.close()

cursor = conn.cursor()
cursor.execute('select * from user where id = %s', ('1',))
values = cursor.fetchall()
print values
print cursor.close()
conn.close()
