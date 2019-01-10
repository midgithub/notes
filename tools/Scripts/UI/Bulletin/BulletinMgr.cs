/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using UnityEngine;
using System.Collections;
using System.IO;
using XLua;
using Bundle;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class BulletinMgr
    {
        private static BulletinMgr _instance;
        public static BulletinMgr Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new BulletinMgr();
                }

                return _instance;
            }
        }

        private string mData;
        private bool isNew;
        private BulletinMgr()
        {
            mData = string.Empty;
            isNew = false;
        }

        public string ReadBulletin()
        {
            if(string.IsNullOrEmpty(mData))
            {
                ReadLocal();
            }

            return mData;
        }

        public void WriteBulletin(string content)
        {
            string md5Code = "";
            if (content.StartsWith(BundleCommon.BaseUrl + "\r\n"))
            {
                mData = content.Replace(BundleCommon.BaseUrl + "\r\n", "");
                md5Code = Md5Tool.Md5Sum(mData);
            }
            else if (content.StartsWith(BundleCommon.BaseUrl + "\n"))
            {
                mData = content.Replace(BundleCommon.BaseUrl + "\n", "");
                md5Code = Md5Tool.Md5Sum(mData);
            }
            else if (content.StartsWith(BundleCommon.BaseUrl + "\r"))
            {
                mData = content.Replace(BundleCommon.BaseUrl + "\r", "");
                md5Code = Md5Tool.Md5Sum(mData);
            }
            
            string oldCode = PlayerPrefs.GetString("NotieMd5", "");
            if(string.IsNullOrEmpty(md5Code))
            {
                isNew = false;
                PlayerPrefs.SetString("NotieMd5", "");
            }
            else
            {
                isNew = !md5Code.Equals(oldCode);
                PlayerPrefs.SetString("NotieMd5", md5Code);
            }
        }

        private void ReadLocal()
        {
            TextAsset txt = CoreEntry.gResLoader.LoadTextAsset("Data/Bulletin", LoadModule.AssetType.Csv) ;
            if (null != txt)
            {
                mData = txt.text;
                if (!string.IsNullOrEmpty(mData))
                {
                    string md5Code = Md5Tool.Md5Sum(mData);
                    string oldCode = PlayerPrefs.GetString("NotieMd5", "");
                    isNew = !md5Code.Equals(oldCode);
                    PlayerPrefs.SetString("NotieMd5", md5Code);
                }
                else
                {
                    isNew = false;
                    PlayerPrefs.SetString("NotieMd5", "");
                }
            }
            else
            {
                isNew = false;
                PlayerPrefs.SetString("NotieMd5", "");
            }
        }

        public bool IsNew()
        {
            if (string.IsNullOrEmpty(mData))
            {
                ReadLocal();
            }
            return isNew;
        }
    }
}