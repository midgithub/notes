# coding:utf8

class Lesson(object):
    def __init__(self):
        self.__lessonUrl = ''
        self.__midPathUrl = ''
        self.__lessonName = ''
        self.__lessonTypeName = ''

    def get_lessonUrl(self):
        return self.__lessonUrl

    def set_lessonUrl(self, url):
        self.__lessonUrl = url

    def get_midPathUrl(self):
        return self.__midPathUrl

    def set_midPathUrl(self, url):
        self.__midPathUrl = url

    def get_lessonName(self):
        return self.__lessonName

    def set_lessonName(self, name):
        self.__lessonName = name

    def get_lessonTypeName(self):
        return self.__lessonTypeName

    def set_lessonTypeName(self, name):
        self.__lessonTypeName = name

    def get_lessonUid(self):
        return self.__lessonUrl + '/' + self.__midPathUrl

    def get_lesson_download_url(self):
        return self.__lessonUrl + '/' + self.__midPathUrl + '/output.m3u8'

    def get_full_name(self):
        titles = self.__lessonTypeName.split('_')
        pathes = [titles[0], titles[2], titles[1], self.__lessonName]
        return '/'.join(pathes) + '.mp4'
