# coding:utf8

import os
import requests
from Crypto.Cipher import AES


class LessonDownloader(object):
    def __init__(self, todir, lesson):
        self.lesson = lesson
        self.tsfiles = []
        self.todir = todir

    def checkdownload(self):
        target_dir = self.todir
        fullname = os.path.join(target_dir, self.lesson.get_full_name())
        if os.path.exists(fullname):
            print u'文件： %s 已下载...' % (fullname)
            return False

        print u'正在下载文件： %s ...' % (fullname)
        target_dir = os.path.dirname(fullname)
        if not os.path.exists(target_dir):
            os.makedirs(target_dir)

        self.todir = target_dir
        return True

    def startdownload(self):
        if self.checkdownload() is False:
            return

        url = self.lesson.get_lesson_download_url()
        all_content = requests.get(url).text  # 获取第一层M3U8文件内容
        if "#EXTM3U" not in all_content:
            raise BaseException(u"非M3U8的链接")

        if "EXT-X-STREAM-INF" in all_content:  # 第一层
            file_line = all_content.split("\n")
            for line in file_line:
                if '.m3u8' in line:
                    url = url.rsplit("/", 1)[0] + "/" + line  # 拼出第二层m3u8的URL
                    all_content = requests.get(url).text

        file_line = all_content.split("\n")

        unknow = True
        key = ""
        merge = True
        for index, line in enumerate(file_line):  # 第二层
            if "#EXT-X-KEY" in line:  # 找解密Key
                method_pos = line.find("METHOD")
                comma_pos = line.find(",")
                method = line[method_pos:comma_pos].split('=')[1]
                # print "Decode Method：", method

                uri_pos = line.find("URI")
                quotation_mark_pos = line.rfind('"')
                key_path = line[uri_pos:quotation_mark_pos].split('"')[1]

                key_url = url.rsplit("/", 1)[0] + key_path  # 拼出key解密密钥URL
                res = requests.get(key_url)
                key = "1234567812345678"  # res.content  #在线m3u8测试
                # print "key：", key

            if "EXTINF" in line:  # 找ts地址并下载
                unknow = False
                pd_url = url.rsplit("/", 1)[0] + "/" + file_line[index + 1]  # 拼出ts片段的URL

                # print u"下载文件： " + pd_url
                res = requests.get(pd_url)
                c_fule_name = file_line[index + 1].rsplit("/", 1)[-1].replace('output', '')
                self.tsfiles.append(c_fule_name)

                try:
                    if len(key):  # AES 解密
                        cryptor = AES.new(key, AES.MODE_CBC, key)  # key 16 倍数
                        with open(os.path.join(self.todir, c_fule_name), 'ab') as f:
                            f.write(cryptor.decrypt(res.content))
                    else:
                        with open(os.path.join(self.todir, c_fule_name), 'ab') as f:
                            f.write(res.content)
                            f.flush()
                except Exception, e:
                    merge = False
                    print u'发生了一个错误: %s %s' % (self.lesson.get_full_name(), c_fule_name)
                    break
                else:
                    pass
                finally:
                    pass
        if unknow:
            raise BaseException(u"未找到对应的下载链接")

        self.merge_file(merge)

    def merge_file(self, merge):
        if len(self.tsfiles) == 0:
            print u'未下载任何 ts 文件'
            return

        os.chdir(self.todir)
        if merge is False:
            os.system('del /Q *.ts')
            return

        print u'合并文件： %s ...' % (self.lesson.get_lessonName())

        cmd = '+'.join(self.tsfiles)
        cmd = 'copy /b ' + cmd + " new.tmp"
        os.system(cmd)  # 命令太长合并不了
        os.system('del /Q *.ts')
        os.rename("new.tmp", self.lesson.get_lessonName() + '.mp4')
