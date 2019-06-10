# coding:utf8

import os
import sys
import requests
from Crypto.Cipher import AES
from video_spider import lessonshelper

reload(sys)
sys.setdefaultencoding('utf-8')

tsfiles = []

def download(url, topath):
    global tsfiles
    tsfiles = []

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
            key = "1234567812345678" #res.content
            # print "key：", key

        if "EXTINF" in line:  # 找ts地址并下载
            unknow = False
            pd_url = url.rsplit("/", 1)[0] + "/" + file_line[index + 1]  # 拼出ts片段的URL

            # print u"下载文件： " + pd_url
            res = requests.get(pd_url)
            c_fule_name = file_line[index + 1].rsplit("/", 1)[-1].replace('output', '')
            tsfiles.append(c_fule_name)

            if len(key):  # AES 解密
                cryptor = AES.new(key, AES.MODE_CBC, key)   #key 16 倍数
                with open(os.path.join(topath, c_fule_name), 'ab') as f:
                    f.write(cryptor.decrypt(res.content))
            else:
                with open(os.path.join(topath, c_fule_name), 'ab') as f:
                    f.write(res.content)
                    f.flush()
    if unknow:
        raise BaseException(u"未找到对应的下载链接")


def merge_file(path, filename):
    if len(tsfiles) == 0:
        print u'未下载任何 ts 文件'
        return

    print u'合并文件： %s ...' % (filename)

    os.chdir(path)
    cmd = '+'.join(tsfiles)
    cmd = 'copy /b ' + cmd + " new.tmp"
    os.system(cmd)  # 命令太长合并不了
    os.system('del /Q *.ts')
    os.rename("new.tmp", filename)

if __name__ == '__main__':
    download_path = os.getcwd() + "\download"
    if not os.path.exists(download_path):
        os.mkdir(download_path)

    urls = {
        "http://cdnaliyunv.zhongye.net/201901/94d4eca4-9457-4be1-a83c-9d0d9eddfcca/5dc4cfb0-beef-4045-ad38-5fcccb8f8bef/output.m3u8": u"考研英语二/导学规划课/导学课（1）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201902/24236eb0-db9b-40f0-8190-dd1e7b5746c4/a53068f7-10dd-418a-aa99-3a1e8f3ef83f/output.m3u8": u"考研英语二/导学规划课/导学课（2）（刘建波）",

        "http://cdnaliyunv.zhongye.net/201811/2d005474-f7c8-4401-b184-e643672cdcf5/48bd46e0-0367-4060-92e8-80bcb6284163/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 词汇：导学（1）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201811/4ef6ea3b-5015-4bb4-ad48-84200d6ea60a/9948a388-9aa0-494b-9d38-5440a64d2f16/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 词汇：导学（2）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201811/9ec740b5-9474-4fef-abd3-59f25efb0e09/b1505bc3-2191-4f35-8ac2-8cae904032b5/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 词汇：导学（3）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/7e8e13ec-edcf-4ef5-ab4d-ea03fc8c1da6/e168be81-c0c6-4dc2-8af1-ab6113e72aed/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：社会生活（1）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201811/8d560a9b-82f6-43b5-b645-b74446342b89/9e31354c-12b1-4821-a002-8f8febba5c66/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：社会生活（2）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/4ad7058a-b36a-42c2-8b03-9bb2cfb83dc5/718c41ff-261b-4585-8cad-0f1a01761aea/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：社会生活（3）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201811/21c4c914-5456-4e94-bd01-359400b92011/05e0b636-d81b-4466-94c4-601ef171139d/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：社会生活（4）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/33a42fe8-dced-4551-bd99-925b8e5d56f5/930ae58b-8e20-4449-95e9-0fa6011676c9/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：社会生活（5）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201903/bf7edc73-341c-47d5-a069-512e8722c9a7/2da9e312-0e4b-4c8c-9aa8-bb1c26411760/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：社会生活（6）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/5f64df04-52d3-4e43-9b67-0f5670bc1308/a4969de4-433c-48f1-9814-28d8d64d72d4/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：文化教育（1）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/4a6fa8e3-592f-4da0-ac5c-237fdb4a1596/6d2f10e9-2c95-4262-86a1-0b3d7dce1b65/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：文化教育（2）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201904/8a2cdd41-4d3c-4cd0-9a77-3fc4532d0210/43837fd6-7d3c-407a-9dcf-5d027954b1e5/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：文化教育（3）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201811/86e8cd77-9936-4654-b7ee-7c754ce9d97a/8ba523ce-2305-4cc3-bdee-ca02330251e6/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：经济科技（1）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/1a517706-3f98-4ea4-894e-67b6da1bd0f6/72acc200-0ee1-4cd8-ad67-99c509de09f5/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：经济科技（2）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201904/9aae6f11-4326-4807-a319-ec7bc6f116a2/51d23ceb-4b24-4d5f-9709-3bdb6a73973f/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：经济科技（3）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/7b117b61-ab16-4d55-8034-49ed3ca89a0b/61270902-961d-4778-9989-905605181ab5/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：法律政治（1）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201801/b5d8f945-1a8a-4152-b638-55124d10e2f1/99789d52-5e53-4c7f-919a-77767d6baa15/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：法律政治（2）（苗嘉）",
        "http://cdnaliyunv.zhongye.net/201811/56d21a6b-4fd9-482e-9c0b-ef6684edf4b2/50f43a16-5b37-4642-b014-89bd4e4c7182/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：法律政治（3）（苗嘉）",

        "http://cdnaliyunv.zhongye.net/201901/55148563-c494-46fa-8285-0ea416008f51/6e2e59f9-5eef-4373-aa1c-13684cb4b01f/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（1）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201901/60ac375f-9ad3-4bf6-8947-d287e44884fb/57134434-2fe3-4ad6-8786-d8731cdf353e/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（2）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201901/ed4987b6-a1c5-4f2a-8649-eadf45e53478/c9aec37f-da61-4575-a29f-773bf5e95e14/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（3）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201901/6af618e6-3417-4f7a-9fee-f7f54db4ec02/7fc64abe-ce68-4ce7-9fdd-de633bd3a5ad/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（4）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201901/69027d0a-fb11-4d50-b3ed-84e4e6977855/924636bf-a3b5-4f33-b0d9-7e33cc364967/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（5）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201901/8250a98c-9b83-44b8-af8d-1bbe02f6252f/d3a999f7-93f0-49d8-abab-4bb21b4f82f2/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（6）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201901/a1c45583-8af9-484e-a810-f66837d1dfbc/7f4341e3-e769-4397-ae13-16ecaee5efe9/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（7）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201903/aaacf300-8a7d-415e-aed2-bbefe0debe56/2d65b19b-6429-4ce6-bed8-e22dec9c62f1/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（8）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201903/dec73341-c230-44a1-b6d9-cd7b44baff07/cfef59a4-4650-4922-9e1f-95ad39186699/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（9）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201903/c7285afc-7c5e-4b0e-ac95-92e1ad0c0c74/e78086d1-65f7-4051-8b3b-37d1525f05c4/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（10）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201903/b3f8bd54-3f78-4f21-826b-743d3f68d7bc/68f36552-8457-4077-9822-6e8e2a7e3d67/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（11）（刘建波）",
        "http://cdnaliyunv.zhongye.net/201903/9ca08fb3-c97e-4789-a7ff-03e372585379/e0cad08e-f03d-4f11-a82c-a8ab3ce3da0a/output.m3u8": u"考研英语二/基础精讲课/2020考研 统考英语 核心词汇：语境词汇（12）（刘建波）",
    }

    count = 0
    for k, v in urls.iteritems():
        count = count + 1
        print u'正在下载第 %d 文件： %s ...' % (count, v)

        target_dir = download_path
        sub_path = v.split('/')
        for sub in sub_path:
            target_dir = os.path.join(target_dir, sub)

        if os.path.exists(target_dir + '.mp4'):
            continue

        target_dir = os.path.dirname(target_dir)
        if not os.path.exists(target_dir):
            os.makedirs(target_dir)

        download(k, target_dir)
        merge_file(target_dir, sub_path[-1] + '.mp4')