#!/usr/bin/env python
# -*- coding: utf-8 -*-


class UrlManager(object):
    def __int__(self):
        self.unserch_urls = set()
        self.serched_urls = set()

    def addNewUrl(self, url):
        if url is None:
            return

        if url not in self.unserch_urls and url not in self.serched_urls:
            self.unserch_urls.add(url)

    def addNewUrls(self, urls):
        if urls is None or len(urls) == 0:
            return
        for url in urls:
            self.addNewUrl(url)

    def hasNewUrl(self):
        return  len(self.unserch_urls) > 0

    def getNewUrl(self):
        new_url =  self.unserch_urls.pop()
        self.serched_urls.add(new_url)
        return  new_url

