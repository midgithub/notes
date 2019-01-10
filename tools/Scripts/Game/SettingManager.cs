using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using SG;
using System.Text.RegularExpressions;



//画面品质
[LuaCallCSharp]
public enum EnumPic
{
    LOW =1,
    MID = 2,
    HEIGHT = 3,
}

[LuaCallCSharp]
[Hotfix]
public class SettingManager 
{
    private static SettingManager m_instance = null;
    public static SettingManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SettingManager();
            }
            return m_instance;
        }
    }


    //private int width = 0;

    //private int height = 0;

    //private int scaleWidth = 0;

    //private int scaleHeight = 0;
#pragma warning disable 0414

    private int ScreenWidth = 1136;

    private int ScreenHeight = 640;

    //1低，2中,3高
    public int PicSetting
    {
        get
        { 
            return PlayerPrefs.GetInt("PicSetting", 2);
        }
        set {
            PlayerPrefs.SetInt("PicSetting", value);
            GameGraphicSetting.IsLowQuality = (value == 1);
            EventParameter param = new EventParameter();
            param.intParameter = value;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SETTING_CHANGE, param);
        }
    }

    public int ActorDisplayNum
    {
        get
        {
            int num = PlayerPrefs.GetInt("ActorDisplayNum", 0);
            if (0 != num)
            {
                return num;
            }
            
            string param = ConfigManager.Instance.Consts.GetValue<string>(449, "param");
            if (string.IsNullOrEmpty(param))
            {
                return 1;
            }

            string[] pms = param.Split('#');
            int pic = PicSetting;
            if (pic < 1 || null == pms || pms.Length < pic)
            {
                return 1;
            }

            string[] pms2 = pms[pic - 1].Split(',');
            if (null == pms2 || pms2.Length < 3)
            {
                return 1;
            }

            if(!int.TryParse(pms2[2], out num))
            {
                num = 1;
            }

            return num;
        }
        set
        {
            PlayerPrefs.SetInt("ActorDisplayNum", value);
        }
    }

    public void SetScreenResolution()
    {
        int CurSettingLevel = GetPhoneQuality() - 1;

        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

        if (CurSettingLevel == 0)
        {
            setDesignContentScale(0);
            Shader.globalMaximumLOD = 500;
        }
        else if (CurSettingLevel == 1)
        {
            setDesignContentScale(1);
            Shader.globalMaximumLOD = 1000;
        }
        else
        {
            setDesignContentScale(2);
            Shader.globalMaximumLOD = 1000;
        }
    }
    /// <summary>
    /// 优化安卓手机显示
    /// </summary>
    public void setDesignContentScale(int level)
    { 

        #region MyRegion
        //if (scaleWidth == 0 && scaleHeight == 0)
        //{
        //    width = Screen.currentResolution.width;
        //    height = Screen.currentResolution.height;
        //    float s1 = (float)designWidth / (float)designHeight;
        //    float s2 = (float)width / (float)height;
        //    if (s1 < s2)
        //    {
        //        designWidth = (int)Mathf.FloorToInt(designHeight * s2);
        //    }
        //    else if (s1 > s2)
        //    {
        //        designHeight = (int)Mathf.FloorToInt(designWidth / s2);
        //    }
        //    float contentScale = (float)designWidth / (float)width;
        //    if (contentScale < 1.0f)
        //    {
        //        scaleWidth = designWidth;
        //        scaleHeight = designHeight;
        //    }
        //}
        //if (scaleWidth > 0 && scaleHeight > 0)
        //{
        //    if (scaleWidth % 2 == 0)
        //    {
        //        //scaleWidth += 1;
        //    }
        //    else
        //    {
        //        scaleWidth -= 1;
        //    }
        //    Screen.SetResolution(scaleWidth, scaleHeight, true);
        //}
        #endregion
#if UNITY_ANDROID 
            if (level == 0)
            { 
                Screen.SetResolution(Mathf.CeilToInt(ScreenWidth * 0.75f), Mathf.CeilToInt(ScreenHeight * 0.75f), true);
            } 
            else
            {
                Screen.SetResolution(ScreenWidth, ScreenHeight, true);
            } 
#elif UNITY_IOS
            //if (level == 0||level == 1)
            //{
            //    Screen.SetResolution(1280,720,true);
            //}
            //else
            //{
//                 if(ScreenWidth>1920)
//                 {
//                     Screen.SetResolution(Mathf.CeilToInt(ScreenWidth * 0.8f), Mathf.CeilToInt(ScreenHeight * 0.8f), true);
//                 }
//                 else
//                 {
//                     Screen.SetResolution(ScreenWidth, ScreenHeight, true);
//                 }
            //}
#endif
    }



#region  获取手机高低配置

    //public class renderconfig
    //{
    //    int ShadowLevel = 0;
    //    int ShaderLOD = 151;
    //    int AntiAliasing = 0;
    //    int ScreenWidth = 960;
    //    int EffectLevel = 0;
    //    int FontEffect = 0;
    //    int PostEffect = 0;
    //    int pbr = 0;
    //    int WaterLevel = 0;
    //}

    public int GetPhoneQuality()
    {
        int quality =3;

#if UNITY_ANDROID
         quality = 1;

        int systemMemorySize = SystemInfo.systemMemorySize;
        string deviceModelName = SystemInfo.deviceModel.Trim();
        string graphicsDeviceName = SystemInfo.graphicsDeviceName.ToLower().Trim();//"adreno 530";//

        if (systemMemorySize > 2100) //2--3g
        {
            quality = 3;
        }
        else if (systemMemorySize > 1666) //2g
        {
            quality = 2;
        }
        else
        { // 1-2G
            quality = 1;
        }
        //Debug.Log(string.Format("debug qua 1:{0} {1} {2} {3}", quality,systemMemorySize, deviceModelName, graphicsDeviceName));
        quality = Mathf.Min(quality, deviceModelMatching(deviceModelName));
        //Debug.Log(string.Format("debug qua2:{0} {1} {2} {3}", quality, systemMemorySize, deviceModelName, graphicsDeviceName));
        quality = Mathf.Min(quality, GraphicMatching(graphicsDeviceName));
        //Debug.Log(string.Format("debug qua3:{0} {1} {2} {3}", quality, systemMemorySize, deviceModelName, graphicsDeviceName));
        if(quality <= 0)
        {
            quality = 1;
        }

#elif UNITY_IOS
        quality = 3;
#endif
        Debug.Log("phone qua:" + quality);
        return quality;
    }

    [CSharpCallLua]
    //type 
    public delegate int MatchDeviceQuality(int type, string param);

    //return 0获取失败
    public int GetDeviceQuality(int type, string param)
    {
        int quality = 0;
        MatchDeviceQuality fun = LuaMgr.Instance.GetLuaEnv().Global.Get<MatchDeviceQuality>("MatchDeviceQuality");
        if (fun != null)
        {
            quality = fun(type, param);
        }

        return quality;
    }

    public int deviceModelMatching(string devicename)
    {
        int quality = 3;
        //用devicename匹配lua配置表
        int tmpQua = GetDeviceQuality(1, devicename);
        if (tmpQua != 0)
        {
            quality = tmpQua;
        }
        return quality;
    }
    public int GraphicMatching(string graphicsDeviceName)
    {
        //默认最好品质
        int quality = 3;

        string[] graphicNames = { "Adreno", "Mali" };
        string graphicName = string.Empty;
        for (int i = 0; i < graphicNames.Length; i++)
        {
            int index  = graphicsDeviceName.ToLower().IndexOf(graphicNames[i].ToLower());
            if (index >= 0)
            {
                graphicName = graphicNames[i];
                break;
            }
        }

        string graphicAdapterName = string.Empty;
        string graphicAdapterFuzzyName = string.Empty;
        do
        {
            if (string.IsNullOrEmpty(graphicName))
                break;

            //String regexKey = @"(.*?[^a-zA-Z]|^)(mali)([^a-zA-Z].*?[\D]|[^a-zA-Z0-9].*)(?<ID>\d{3})([\D].*|$)";
            string regexKey = @"(.*?[^a-zA-Z]|^)(" + @graphicName.ToLower() + @")([^a-zA-Z].*?[\D]|[^a-zA-Z0-9]{0,1})(?<ID>\d{3})([\D].*|$)";
            Match ma = Regex.Match(graphicsDeviceName, regexKey, RegexOptions.IgnoreCase);
            if (ma == null || (ma.Success == false) || ma.Groups == null)
                break;

            graphicAdapterName = graphicName + ma.Groups["ID"];
            LogMgr.Log("GPU型号1:{0}", graphicAdapterName);
            graphicAdapterFuzzyName = graphicAdapterName.Substring(0, graphicAdapterName.Length - 2) + "XX";

        } while (false);

        //先用graphicAdapterName匹配lua配置
        //再用graphicAdapterFuzzyName匹配lua配置

        //找不到适配型号默认高配置
        if(string.IsNullOrEmpty(graphicAdapterName) && string.IsNullOrEmpty(graphicAdapterFuzzyName))
        {
            return quality;
        }

        int tmpQua = GetDeviceQuality(2, graphicAdapterName);
       
        if (tmpQua != 0)
        {
            quality = tmpQua;
        }
        else
        {
            tmpQua = GetDeviceQuality(2, graphicAdapterFuzzyName);
            if(tmpQua != 0)
            {
                quality = tmpQua;
            }
        }

        return quality;
    }

#endregion
}

