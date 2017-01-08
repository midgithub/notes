# coding:utf8
import urllib2


class DownloadCtrl(object):
    def download(self, url):
        if url is None:
            return

        response = urllib2.urlopen(url)
        if response.getcode() != 200:
            return None

        return response.read()
