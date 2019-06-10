# coding:utf8

import os
import sys
from video_spider import lessonshelper
from video_spider import lessondownloader

reload(sys)
sys.setdefaultencoding('utf-8')


class Wideos(object):
    def __init__(self):
        self.obj_lesson_helper = lessonshelper.LessonHelper()

    def start(self, dir):
        self.obj_lesson_helper.init()
        lessons =  self.obj_lesson_helper.getlessons()

        count = 0
        for k, v in lessons.iteritems():
            count = count + 1
            print u'准备下第 %d 个文件' % (count)
            downloader = lessondownloader.LessonDownloader(dir, v)
            downloader.startdownload()


if __name__ == '__main__':
    download_path = os.getcwd() + "\download"
    if not os.path.exists(download_path):
        os.mkdir(download_path)

    obj_vido = Wideos()
    obj_vido.start(download_path)
