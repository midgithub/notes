using XLua;
using System;
using UnityEngine;
using System.Runtime.InteropServices;

[Hotfix]
public class PushManager : MonoBehaviour, IPush
{
    private static IPush m_Instance;
    private Action<bool, string> onStartFinish;
    private Action<bool, string> onStopFinish;

    public static IPush Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("[PushManager]").AddComponent<PushManager>();
            }
            return m_Instance;
        }
    }

    protected void Awake()
    {

        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);
        //Debug.LogError("=====Awake======"+ this.gameObject.name);
#if UNITY_IOS&& !UNITY_EDITOR
        //__PushUnity3DBridge_init(this.gameObject.name, "OnCallBack");
#elif UNITY_ANDROID&& !UNITY_EDITOR
        _androidPushManager.Call("init", this.gameObject.name, "OnCallBack");
#endif
    }


[Hotfix]
    private class MethodInfo
    {
        public string method;
        public bool success;
        public string msg;
    }

    protected void OnCallBack(string data)
    {
        //Debug.LogError("=====OnCallBack======" + data);

        var methodInfo = JsonUtility.FromJson<MethodInfo>(data);
        if (methodInfo.method == "OnStartFinish")
        {
            if (onStartFinish != null)
            {
                onStartFinish(methodInfo.success, methodInfo.msg);
            }
        }
        else if (methodInfo.method == "OnStopFinish")
        {
            if (onStopFinish != null)
            {
                onStopFinish(methodInfo.success, methodInfo.msg);
            }
        }
    }


    private static AndroidJavaObject _androidPushManager;

    private PushManager()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       try
        {
            var ajc = new AndroidJavaClass("com.hk.sdk.push.PushManager");
            _androidPushManager = ajc.CallStatic<AndroidJavaObject>("getInstance");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
#endif

    }

    public void SetAccessId(long accessId)
    {
        _androidPushManager.Call("setAccessId", accessId);
    }

    public long GetAccessId()
    {
        return _androidPushManager.Call<long>("getAccessId");
    }

    public void SetAccessKey(string accessKey)
    {
        _androidPushManager.Call("setAccessKey", accessKey);
    }

    public string GetAccessKey()
    {
        return _androidPushManager.Call<string>("getAccessKey");
    }

    public void SetMiPushAppId(string appId)
    {
        _androidPushManager.Call("setMiPushAppId", appId);
    }

    public void SetMiPushAppKey(string appkey)
    {
        _androidPushManager.Call("setMiPushAppKey", appkey);
    }

    public void SetMzPushAppId(string appId)
    {
        _androidPushManager.Call("setMzPushAppId", appId);
    }

    public void SetMzPushAppKey(string appKey)
    {
        _androidPushManager.Call("setMzPushAppKey", appKey);
    }

    public void SetInstallChannel(string channel)
    {
        _androidPushManager.Call("setInstallChannel", channel);
    }

    public string GetInstallChannel()
    {
        return _androidPushManager.Call<string>("getInstallChannel");
    }

    public void SetHuaweiDebug(bool isDebug)
    {
        _androidPushManager.Call("setHuaweiDebug", isDebug);
    }

    public void EnableDebug(bool isEnable)
    {
        _androidPushManager.Call("enableDebug", isEnable);
    }

    public string GetToken()
    {
        return _androidPushManager.Call<string>("getToken");
    }

    public void StartPushService(Action<bool, string> onFinish = null)
    {
        onStartFinish = onFinish;
        _androidPushManager.Call("registerPush");
    }

    public void StopPushService(Action<bool, string> onFinish = null)
    {
        onStopFinish = onFinish;
        _androidPushManager.Call("unregisterPush");
    }

    public void BindAccount(string account)
    {
        _androidPushManager.Call("bindAccount", account);
    }

    public void UnBindAccount(string account)
    {
        _androidPushManager.Call("delAccount", account);
    }

    public void BindTag(string tag)
    {
        _androidPushManager.Call("setTag", tag);
    }

    public void UnBindTag(string tag)
    {
        _androidPushManager.Call("deleteTag", tag);
    }
}

