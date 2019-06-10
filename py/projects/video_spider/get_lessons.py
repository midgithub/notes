# coding:utf8

import os
import sys

reload(sys)
sys.setdefaultencoding('utf-8')

lessons = {}

def readRecords(path):
    for f in os.listdir(path):
        if os.path.isfile(os.path.join(path,f)):
            fileType = os.path.splitext(f)[-1]
            if fileType == ".txt":
                print u"读取首包记录文件：" + os.path.join(path,f).replace("\\","/")
                with open(os.path.join(path,f).replace("\\","/"), "r") as recordFile:
                    lines = recordFile.readlines()

                    for i in range(len(lines)/15):
                        print '第 %d 个 视频对象' % (i+1)
                        lesson_obj = {}
                        start = 0+15*i
                        end = 15*(i+1)
                        for line in range(start, end):
                            linecontent = lines[line]
                            if 'LessonUrl' in linecontent:
                                valuepos = linecontent.find(':')
                                value = linecontent[valuepos+3:-2]
                                lesson_obj['LessonUrl'] = value
                            elif 'MidPathUrl' in linecontent:
                                valuepos = linecontent.find(':')
                                value = linecontent[valuepos + 3:-2]
                                lesson_obj['MidPathUrl'] = value
                            elif 'LessonName' in linecontent:
                                valuepos = linecontent.find(':')
                                value = linecontent[valuepos + 3:-2]
                                lesson_obj['LessonName'] = value.decode("gb2312")
                                print lesson_obj['LessonName']
                            elif 'LessonTypeName' in linecontent:
                                valuepos = linecontent.find(':')
                                value = linecontent[valuepos + 3:-2]
                                lesson_obj['LessonTypeName'] = value.decode("gb2312")
                                print lesson_obj['LessonTypeName']

                        uid = lesson_obj['LessonUrl']+'/'+lesson_obj['MidPathUrl']
                        if uid not in lessons:
                            lessons[uid] = lesson_obj

def writelessons(path):
    with open(os.path.join(path, "lessons.log"), "w") as difResFile:
        for k, v in lessons.iteritems():
            difResFile.write(str(v).encode("gb2312") + '\n')

if __name__ == '__main__':
    curDir = sys.path[0]

    configDir = curDir + "/config/"
    readRecords(configDir)
    writelessons(configDir)