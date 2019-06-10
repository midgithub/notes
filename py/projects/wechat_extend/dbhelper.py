# coding:utf8
import mysql.connector
import utils

class DbHelper(object):
    def __init__(self):
        self.connect = mysql.connector.connect(user='root', password='102261013028', use_unicode=True)   #N-XdYJdgd6e&
        self.cursor = self.connect.cursor()  # 创建游标

    def init(self):
        try:
            self.cursor.execute('create database wechat')
        except:
            print 'Database wechat exists!'
        self.connect.database = 'wechat'

        try:
            self.cursor.execute('create table friends (id smallint primary key auto_increment , nickName varchar(100), remarkName varchar(100), uid varchar(100), lastReplyTime varchar(20), autoReplying varchar(5))')
        except:
            print 'The table friends exists!'

    def get_last_reply_time(self, friendInfo):
        self.cursor.execute('select * from friends where uid = %s', (friendInfo['UserName'],))
        friend = self.cursor.fetchone()    #不管针对数据库的查询有没有返回结果，必须要进行fetchxxx()，否则进行下一个其他的insert、create等待操作的时候就会报unread result found的异常了。
        if friend is not None:
            return (utils.str_to_timetuple(friend[4]), int(friend[5]))
        else:
            self.cursor.execute('insert into friends (id, nickName, remarkName, uid, lastReplyTime, autoReplying) values (%s, %s, %s, %s, %s, %s)', [0, friendInfo['NickName'], friendInfo['RemarkName'], friendInfo['UserName'], '2019-02-03 17:00:00', '0'])
            self.cursor.fetchone()
            self.connect.commit()
        return (0, 0)

    def update_last_reply_time(self, uid):
        timestr = utils.get_cur_time_str()
        self.cursor.execute('update friends set lastReplyTime = %s, autoReplying = %s where uid = %s', (timestr, '0', uid))
        self.cursor.fetchone()
        self.connect.commit()

    def set_robot_replying(self, uid, replying):
        self.cursor.execute('update friends set autoReplying = %s where uid = %s', (replying, uid))
        self.cursor.fetchone()
        self.connect.commit()

    def close(self):
        self.cursor.close()
        self.conn.close()