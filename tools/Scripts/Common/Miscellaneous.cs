using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
[LuaCallCSharp]
[Hotfix]
public static class Miscellaneous
{
#if UNITY_ANDROID
    private static AndroidJavaObject sContent = null;
#elif UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern float GetIosBatteryLevel();
#endif

    public static int GetBatteryLevel()
    {
        int power = 100;
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (sContent == null)
        {
            sContent = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (AndroidJavaClass bat = new AndroidJavaClass("com.hytime.foxpower.MiddleCS"))
		{  
           power = bat.CallStatic<int>("GetAndroidBatteryLevel",sContent);
		}

#elif UNITY_IPHONE
        power = Mathf.FloorToInt(GetIosBatteryLevel() * 100);        
#endif
        return power;
    }


    //获取品牌
    public static string GetBrand()
    {
        string  value = "";
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (sContent == null)
        {
            sContent = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (AndroidJavaClass bat = new AndroidJavaClass("com.hytime.foxpower.MiddleCS"))
		{  
           value = bat.CallStatic<string>("GetBrand");
		}

#elif UNITY_IPHONE      
#endif
        //Debug.Log(value);
        return value;
    }
    //获取手机型号
    public static string GetModel()
    {
        string value = "";
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (sContent == null)
        {
            sContent = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (AndroidJavaClass bat = new AndroidJavaClass("com.hytime.foxpower.MiddleCS"))
		{  
           value = bat.CallStatic<string>("GetModel");
		}

#elif UNITY_IPHONE      
#endif
        //Debug.Log(value);
        return value;
    }

    //secure android id 
    public static string GetSecureID()
    {
        string value = "";
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (sContent == null)
        {
            sContent = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (AndroidJavaClass bat = new AndroidJavaClass("com.hytime.foxpower.MiddleCS"))
		{  
           value = bat.CallStatic<string>("GetSecureID",sContent);
		}

#elif UNITY_IPHONE      
#endif
        //Debug.Log(value);
        return value;
    }


    //设备id
    public static string GetDeveiceId()
    {
        string value = "";
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (sContent == null)
        {
            sContent = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (AndroidJavaClass bat = new AndroidJavaClass("com.hytime.foxpower.MiddleCS"))
		{  
           value = bat.CallStatic<string>("GetDeveiceId",sContent);
		}

#elif UNITY_IPHONE      
#endif
        //Debug.Log(value);
        return value;
    }


    //获取mac
    public static string GetMac()
    {
        string value = "";
#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (sContent == null)
        {
            sContent = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        using (AndroidJavaClass bat = new AndroidJavaClass("com.hytime.foxpower.MiddleCS"))
		{  
           value = bat.CallStatic<string>("GetMac");
		}

#elif UNITY_IPHONE      
#endif
        //Debug.Log(value);
        return value;
    }
}

