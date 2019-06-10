# coding:utf8
from datetime import datetime
import time


def get_cur_time_str():
    return time.strftime('%Y-%m-%d %H:%M:%S')


def str_to_timetuple(timestr):
    return time.mktime(datetime.strptime(timestr, "%Y-%m-%d %H:%M:%S").timetuple())