# coding:utf8
import urlparse
# 网页解析器
from bs4 import BeautifulSoup
import re


class HtmlParser(object):
    def parse(self, url, cont):
        if url is None or cont is None:
            return

        soup = BeautifulSoup(cont, "html.parser", from_encoding="utf-8")
        new_urls = self._get_new_urls(url, soup)
        new_data = self._get_new_data(url, soup)
        return new_urls, new_data

    def _get_new_urls(self, url, soup):
        new_urls = set()
        # /view/123.htm
        links = soup.find_all("a", href=re.compile(r"/view/\d+\.htm"))
        for link in links:
            new_url = link['href']
            new_full_url = urlparse.urljoin(url, new_url)
            new_urls.add(new_full_url)
        return new_urls

    def _get_new_data(self, url, soup):
        res_data = {}
        res_data['url'] = url

        # <dd class="lemmaWgt-lemmaTitle-title">
        title_node = soup.find('dd', class_="lemmaWgt-lemmaTitle-title").find("h1")
        res_data['title'] = title_node.get_text()

        # class ="lemma-summary" label-module="lemmaSummary" >
        summary_node = soup.find('div', class_="lemma-summary")
        res_data['summary'] = summary_node.get_text()

        return res_data
