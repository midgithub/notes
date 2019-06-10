# coding:utf8

import os
import sys
import lesson
import io


class LessonHelper(object):
    def __init__(self):
        self.lessons = {}

    def init(self):
        curDir = sys.path[0]
        configDir = curDir + "/config/"
        self.readRecords(configDir)
        #self.writelessons(configDir)

    def readRecords(self, path):
        for f in os.listdir(path):
            if os.path.isfile(os.path.join(path, f)):
                fileType = os.path.splitext(f)[-1]
                if fileType == ".txt":
                    print u"读取记录文件：" + os.path.join(path, f).replace("\\", "/")
                    with io.open(os.path.join(path, f).replace("\\", "/"), "r", encoding='utf-8') as recordFile:
                        lines = recordFile.readlines()

                        for i in range(len(lines) / 15):
                            lesson_obj = lesson.Lesson()
                            start = 0 + 15 * i
                            end = 15 * (i + 1)
                            for line in range(start, end):
                                linecontent = lines[line]
                                if 'LessonUrl' in linecontent:
                                    valuepos = linecontent.find(':')
                                    value = linecontent[valuepos + 3:-2]
                                    lesson_obj.set_lessonUrl(value)
                                elif 'MidPathUrl' in linecontent:
                                    valuepos = linecontent.find(':')
                                    value = linecontent[valuepos + 3:-2]
                                    lesson_obj.set_midPathUrl(value)
                                elif 'LessonName' in linecontent:
                                    valuepos = linecontent.find(':')
                                    value = linecontent[valuepos + 3:-2]
                                    lesson_obj.set_lessonName(value)
                                elif 'LessonTypeName' in linecontent:
                                    valuepos = linecontent.find(':')
                                    value = linecontent[valuepos + 3:-2]
                                    lesson_obj.set_lessonTypeName(value)

                            uid = lesson_obj.get_lessonUid()
                            if uid not in self.lessons:
                                self.lessons[uid] = lesson_obj

    def writelessons(self, path):
        with open(os.path.join(path, "lessons.log"), "w") as difResFile:
            for k, v in self.lessons.iteritems():
                difResFile.write(str(v).encode("gbk") + '\n')

    def getlessons(self):
        return self.lessons