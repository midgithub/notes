using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Runtime.InteropServices;
using SG;

[Hotfix]
public class GuangDianTongCall{

    string appid { get { return "1121851252"; } }  //"1121851252";
    string conv_type  { get { return "MOBILEAPP_ACTIVITE"; } }
    string advertiser_id { get { return "23942"; } }// "23942";// 广告主ID（必选）
    string app_type { get { return Application.platform == RuntimePlatform.Android ? "ANDROID" : "IOS"; } }// ANDROID转化应用类型（必选）
    //string uid = "10000";

    string encrypt_key { get { return "BAAAAAAAAAAAAF2G"; } }//广告主在广点通（e.qq.com）创建转化之后，系统会自动生成密钥。
    string sign_key { get { return "4408a197e5b7138c"; } }//广告主在广点通（e.qq.com）创建转化之后，系统会自动生成密钥。
    string conv_time { get { return GetConvTime(); } } // 转化发生时间
    //string client_ip{get{return "10.11.12.13";}} // 转化发生IP（可选） 激活发生用户的客户端IP
    string query_string { get { return string.Format("muid={0}&conv_time={1}", UrlEncode(muid, Encoding.UTF8), UrlEncode(conv_time, Encoding.UTF8)); } }
    string pageUrl { get { return string.Format("http://t.gdt.qq.com/conv/app/{0}/conv?{1}", appid, query_string); } }
    string encode_page { get { return UrlEncode(pageUrl, Encoding.UTF8); } }
    string property { get { return string.Format("{0}&GET&{1}", sign_key, encode_page); } }
    string signature { get { return Bundle.Md5Tool.Md5Sum(property); } }
    string base_data { get { return string.Format("{0}&sign={1}", query_string, signature); } }
    string data { get { return EncryptDecryptStr(base_data, encrypt_key); } }

    string attachment { get { return string.Format("conv_type={0}&app_type={1}&advertiser_id={2}",
        UrlEncode(conv_type, Encoding.UTF8), UrlEncode(app_type, Encoding.UTF8), UrlEncode(advertiser_id, Encoding.UTF8));
    }
    }

    string GetUrl { get { return string.Format("http://t.gdt.qq.com/conv/app/{0}/conv?v={1}&{2}",appid,UrlEncode(data, Encoding.UTF8),attachment); } }

    string muid//用户设备的IMEI 或idfa 进行MD5SUM 以后得到的32 位全小写MD5 表现字符串
    {
        get
        {
            return "aaa54abb8713b7b0f5b39f02bda98693";//ios
            //switch (Application.platform)
            //{
            //    case RuntimePlatform.IPhonePlayer:
            //        return IOSIDFA;
            //    default:
            //        return AndroidImei;
            //}
        }
    }
    string AndroidImei { get { return SystemInfo.deviceUniqueIdentifier.ToLower(); } }
    string IOSIDFA { get { return Bundle.Md5Tool.Md5Sum(mIDFA).ToLower(); } }
    string mIDFA = "";


    [DllImport("__Internal")]
    private static extern string _GetIDFA();
    public GuangDianTongCall()
    {
#if UNITY_IPHONE
        mIDFA = _GetIDFA();
#endif
    }

    string EncryptDecryptStr(string dataStr, string key)
    {
        List<byte> bytes = new List<byte>() ;
        int j = 0;
        for (int i = 0; i < dataStr.Length; i++)
        {
            bytes.Add((byte)(dataStr[i] ^ key[j]));
            j = (++j) % (key.Length);
        }
        return Convert.ToBase64String(bytes.ToArray());
    }

    string UserDataKey = "IsSendGuangDianTong";
    public void WWWRequestAtFirst()
    {
        if (PlayerPrefs.GetInt(UserDataKey, 0) != 0)
            return;
        MonoInstance.Instance.StartCoroutine(WWWRequest());
    }

    IEnumerator WWWRequest()
    {
        string url = GetUrl;
        WWW www = new WWW(url);

        LogMgr.Log("请求广点通:"+url);

        yield return www;
        if (!string.IsNullOrEmpty(www.error))
            LogMgr.LogError(www.error);
        else
        {
            LogMgr.Log("WWWRequestStatus:" + www.text);
            PlayerPrefs.SetInt(UserDataKey, 1);
        }
    }

    [ContextMenu("Test")]
    void Test()
    {
        //WWWRequestAtFirst();
        //return;
        //string url = GetUrl;
        LogMgr.Log(GetUrl);
    }

    private static string UrlEncode(string temp, Encoding encoding)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < temp.Length; i++)
        {
            string t = temp[i].ToString();
            string k = WWW.EscapeURL(t, encoding);
            if (t == k)
            {
                stringBuilder.Append(t);
            }
            else
            {
                stringBuilder.Append(k.ToUpper());
            }
        }
        return stringBuilder.ToString();
    }

    
        static string GetConvTime()
        {
            System.DateTime dt = System.DateTime.Now;
            System.TimeSpan ts = dt.ToUniversalTime() - new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return System.Convert.ToInt64(ts.TotalSeconds).ToString();
        }  



    //http://t.gdt.qq.com/conv/app/112233/conv?v=GRAaEGJVCFNFTRQXZw5UH0RQR0NsVF4GRUtJRGxZBBpEUkBEPUMNDBwPLwA2BgBERVFBRm1TXVVETVYXMwIAFwA6GgRiVF5NQ0heRW1FVEpSFhoTMVhWAkYdRxJqWFdKEgFKRT1WWVdBSRRCbFIATxJSQENuBw%3D%3D&conv_type=MOBILEAPP_ACTIVITE&app_type=ANDROID&advertiser_id=10000
    //http://t.gdt.qq.com/conv/app/112233/conv?v=GRAaEGJVCFNFTRQXZw5UH0RQR0NsVF4GRUtJRGxZBBpEUkBEPUMNDBwPLwA2BgBERVFBRm1TXVVETVYXMwIAFwA6GgRiVF5NQ0heRW1FVEpSFhoTMVhWAkYdRxJqWFdKEgFKRT1WWVdBSRRCbFIATxJSQENuBw%3D%3D&conv_type=MOBILEAPP_ACTIVITE&app_type=ANDROID&advertiser_id=10000
}

