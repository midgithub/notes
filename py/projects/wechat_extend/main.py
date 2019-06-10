# coding:utf8
import sys
import time
import itchat
import requests as rq
from itchat.content import *
from wechat_extend import dbhelper
from wechat_extend import config

reload(sys)
sys.setdefaultencoding('utf-8')  #ascii' codec can't decode byte 0xe4 in position 0: ordinal not in range(128)

class Wechat(object):
    def __init__(self):
        self.obj_db_helper = dbhelper.DbHelper()

    def start(self):
        itchat.auto_login(hotReload=True, loginCallback=self.after_login, exitCallback=self.after_logout)
        itchat.run()

    def after_login(self):
        print("登录后调用")
        self.obj_db_helper.init()

    def after_logout(self):
        print("退出后调用")
        self.obj_db_helper.close()

    def get_result_from_robot(self,text):
        # 接口请求数据
        data = {
            "reqType": 0,
            "perception": {
                "inputText": {
                    "text": str(text)
                }
            },
            "userInfo": {
                "apiKey": "3350ab49a32645f9a5c395df13aa148d",
                "userId": "123"
            }
        }

        # 请求接口
        result = rq.post(config.tl_api_url, headers=config.tl_headers, json=data).json()
        return result

    def robot_reply(self, friend, text):
        result = self.get_result_from_robot(text)
        reply = result['results'][0]['values']['text']
        itchat.send_msg(reply, friend)
        print "TL 查询: " + text
        print "TL 回复: " + reply

    def receive_msg(self, msg):
        if 'UserName' not in msg['User']:  # 可能是公众号发的消息
            return

        my_info = itchat.search_friends()
        reply_content = msg['Content'].encode('utf8')
        my_reply = my_info['UserName'] == msg['FromUserName']  #msg['User']['UserName'] 不管对方或是我发的，都是对方的信息
        lastReplyInfo = self.obj_db_helper.get_last_reply_time(msg['User'])
        if my_reply is True:
            self.obj_db_helper.update_last_reply_time(msg['ToUserName'])

            for call in config.robot_calls:  #自动回复
                if call in reply_content:
                    self.robot_reply(msg['ToUserName'], reply_content.replace(call,''))
                    break
        else:
            for call in config.robot_calls:  # 自动回复
                if call in reply_content:
                    self.robot_reply(msg['FromUserName'], reply_content.replace(call, ''))
                    return

            if time.time() - lastReplyInfo[0] > config.robot_time:
                if lastReplyInfo[1] > 0:
                    if config.robot_auto_reply is True:
                        self.robot_reply(msg['FromUserName'], reply_content)
                else:
                    if msg['User']['RemarkName'] in config.girl_friend_name:
                        itchat.send_msg(u"小主人快来呀,你的女朋友找你来了", msg['FromUserName'])
                        self.obj_db_helper.set_robot_replying(msg['FromUserName'], '1')
                    else:
                        itchat.send_msg(u"小主人现在不在线哦,稍后会给你回消息,急事请电话联系!", msg['FromUserName'])
                    # itchat.send_image()

        # if msg['Type'] == TEXT:  # 文字
        #     print TEXT
        # elif msg['Type'] == MAP:  # 位置
        #     print MAP
        # elif msg['Type'] == CARD:  # 名片
        #     print CARD
        # elif msg['Type'] == SHARING:  # 分享
        #     print SHARING
        # elif msg['Type'] == PICTURE:  # 相册图片
        #     print PICTURE
        # elif msg['Type'] == RECORDING:  # 语音
        #     print RECORDING
        # elif msg['Type'] == VIDEO:  # 小视频
        #     print VIDEO
        # elif msg['Type'] == ATTACHMENT:  # 文件
        #     print ATTACHMENT

obj_wechat = None


@itchat.msg_register([TEXT, MAP, CARD, SHARING, PICTURE, RECORDING, ATTACHMENT, VIDEO], isFriendChat=True)
def receive_msg(msg):
    obj_wechat.receive_msg(msg)

if __name__ == '__main__':
    obj_wechat = Wechat()
    obj_wechat.start()


