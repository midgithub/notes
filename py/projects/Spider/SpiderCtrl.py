#!/usr/bin/env python
# -*- coding: utf-8 -*-
from Spider import DataCtrl
from Spider import DownLoadCtrl
from Spider import ParseCtrl
from Spider import UrlCtrl


class Spider(object):
    def __int__(self):
        self.url_ctrl = UrlCtrl.UrlManager()
        self.downloader = DownLoadCtrl.HtmlDownloader()
        self.parser = ParseCtrl.HtmlParser()
        self.data = DataCtrl.HtmlData()

    def start(self, url):
        count = 0
        self.url_ctrl.addNewUrl(url)
        while self.url_ctrl.hasNewUrl():
            try:
                new_url = self.url_ctrl.getNewUrl()
                count = count + 1
                print "serch %d :%s" % (count, new_url)

                html_cont = self.downloader.download(new_url)
                new_urls, new_data = self.parser.parse(new_url, html_cont)
                self.url_ctrl.addNewUrls(new_urls)
                self.data.addData(new_data)

                if count == 1000:
                    break
            except:
                print  "serch fail"

        self.data.outputData()

if __name__ == '__main__':
    root_url = ""
    spider = Spider()
    spider.start(root_url)