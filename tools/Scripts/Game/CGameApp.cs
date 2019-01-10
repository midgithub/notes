using UnityEngine;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class CGameApp
{
    // 游戏管理器
    private static CGameApp _singleton = null;

    private string m_sDataPath = "";
    private string m_sWWWDataPath = "";

    private string m_sPersistentDataPath = "";
    private string m_sWWWPersistentDataPath = "";

    public static CGameApp Instance
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = new CGameApp();
                _singleton.Init();
            }
            return _singleton;
        }
    }

    public void Init()
    {
        InitPath();
    }

    private void InitPath()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN 
        m_sDataPath = Application.dataPath + "/StreamingAssets/";
#elif UNITY_IOS  // IOS
        m_sDataPath = Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
       m_sDataPath = Application.dataPath + "!assets/";
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN 
        m_sWWWDataPath = "file://" + m_sDataPath;
#elif UNITY_IOS  // IOS
        m_sWWWDataPath = "file://" + m_sDataPath;
#elif UNITY_ANDROID  // Android
        m_sWWWDataPath  = "jar:file://" + m_sDataPath;
#endif

        m_sPersistentDataPath = Application.persistentDataPath + "/";

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        m_sWWWPersistentDataPath = "file://../" + Application.persistentDataPath + "/";
#else
        m_sWWWPersistentDataPath = "file://" + Application.persistentDataPath + "/";
#endif

    }

    // 普通方式读取文件
    public string DataPath
    {
        get
        {
            return m_sDataPath;
        }
    }

    // WWW方式读取文件
    public string WWWDataPath
    {
        get
        {
            return m_sWWWDataPath;
        }
    }

    // 普通方式读取文件
    public string PersistentDataPath
    {
        get
        {
            return m_sPersistentDataPath;
        }
    }

    // WWW方式读取文件
    public string WWWPersistentDataPath
    {
        get
        {
            return m_sWWWPersistentDataPath;
        }
    }
}

