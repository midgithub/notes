#  coding:utf8


class UrlCtrl(object):
    def __init__(self):
        self._new_urls = set()
        self._old_urls = set()

    def has_new_url(self):
        return len(self._new_urls) > 0

    def get_new_url(self):
        new_url = self._new_urls.pop()
        self._old_urls.add(new_url)
        return new_url

    def add_new_url(self, url):
        if url is None:
            return
        if url not in self._new_urls and url not in self._old_urls:
            self._new_urls.add(url)

    def add_new_urls(self, urls):
        if urls is None or len(urls) == 0:
            return
        for url in urls:
            self.add_new_url(url)
