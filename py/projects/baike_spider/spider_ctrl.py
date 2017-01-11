# coding:utf8

# alt+enter 快捷创建类或方法
from baike_spider import download_ctrl
from baike_spider import html_parse
from baike_spider import output_data
from baike_spider import url_ctrl


class SpiderCtrl(object):
    def __init__(self):
        self.obj_urlctrl = url_ctrl.UrlCtrl()
        self.obj_dowloader = download_ctrl.DownloadCtrl()
        self.obj_parser = html_parse.HtmlParser()
        self.obj_outputer = output_data.DataOutputer()

    def start(self, url):
        count = 0
        self.obj_urlctrl.add_new_url(url)
        while self.obj_urlctrl.has_new_url():
            try:
                new_url = self.obj_urlctrl.get_new_url()
                count = count + 1
                print "serch %d : %s" % (count, new_url)
                html_cont = self.obj_dowloader.download(new_url)
                new_urls, html_data = self.obj_parser.parse(new_url, html_cont)
                self.obj_urlctrl.add_new_urls(new_urls)
                self.obj_outputer.add_data(html_data)

                if count == 100:
                    break
            except:
                print "serch fail"

        self.obj_outputer.output_data()

if __name__ == '__main__':
    root_url = "http://baike.baidu.com/link?url=N5MECsMS25HMN7WYZ9FX-9c75dGR_Wfv-361CkfxzwswTFFCQX3EclIwQggO9TEfZhgUDcPHSDIMyLUhxVhk4K"
    obj_spider = SpiderCtrl()
    obj_spider.start(root_url)