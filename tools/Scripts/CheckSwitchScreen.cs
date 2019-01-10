using UnityEngine;
using System.Collections;
using System;

public class CheckSwitchScreen : MonoBehaviour
{
    static CheckSwitchScreen _instance;
    public static CheckSwitchScreen instance { get { return _instance; } }

    [HideInInspector]
    public int cutoutWidth = 80;
    [HideInInspector]
    public int cutoutHeight = 80;
    [HideInInspector]
    public bool liuHai;

    static bool extra = false;
    void Awake()
    {
        liuHai = false;
        if (extra) Destroy(gameObject);
        else
        {
            extra = true;
            DontDestroyOnLoad(gameObject);
            _instance = this;
#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); //获得Context
                if (context != null)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.wywl.hsj.libhelper.ScreenAdaptation");
                    if(null != jc)
                    {
                        jc.CallStatic("StartScreenAdapation", context);     // 调用静态方法(java 方法名)
                    }
                }
                else
                {
                    Debug.LogError("CheckSwitchScreen context null ");
                }
            }
        catch (Exception ex)
        {
            Debug.LogError("CallJava " + ex.Message);
        }
#endif
        }
    }

    public void ScreenAdapation(string size)
    {
        //Debug.Log("Screen.orientation:" + Screen.orientation);
        Debug.Log("size:" + size);
        var s = size.Split('|');
        cutoutWidth = int.Parse(s[0]);
        cutoutHeight = int.Parse(s[1]);
        int width = Screen.width; ;
        int manualWidth = Screen.width;

        //Log("width:" + width);
        //offsetRate = cutoutHeight / width;
        //Log("offsetRate:" + offsetRate);
        liuHai = true;
        //screenOrientationKind = Screen.orientation;

        //初始化一次
        //InitView();
    }
}
