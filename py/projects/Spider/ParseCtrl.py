#!/usr/bin/env python
# -*- coding: utf-8 -*-

from bs4 import BeautifulSoup
import re

class HtmlParser(object):
    def getNewUrls(self, url, soup):
        new_urls = set()
        # /view/123.htm
        links = soup.find_all("a",href = re.compile(r"/view/\d+\.html"))
        for link in links:
            new_url = link["href"]
            new_full_url = urlparse.urljoin(url,new_url)
            new_urls.add(new_full_url)

        return  new_urls

    def getNewData(self, url, soup):
        res_data = {}
        return  res_data

    def parse(self, url, cont):
        if url is None or cont is None:
            return

        soup = BeautifulSoup(cont,"html.parser",from_encoding="utf-8")
        new_urls = self.getNewUrls(url,soup)
        new_data = self.getNewData(url,soup)
        return  new_urls,new_data