using UnityEngine;
using System.Collections;
using XLua;
using System;
using SG;
using UnityEngine.UI;
using System.Text;

[LuaCallCSharp]
[Hotfix]
public class LuaBehaviour : MonoBehaviour
{
    private static int CurIndex = 0;

    public string LuaPath;
    public InjectionObj[] Injections;

    protected bool m_Init = false;                //是否初始化过了
    protected int m_Index = 0;
    protected LuaTable scriptEnv;
    protected Action luaAwake;
    protected Action luaStart;
    protected Action luaUpdate;
    protected Action luaOnEnable;
    protected Action luaOnDisable;
    protected Action luaOnDestroy;

    public int Index
    {
        get { return m_Index; }
    }

    protected virtual void Awake()
    {
        InitLua();
        if (luaAwake != null)
        {
#if UNITY_EDITOR
            //ObjectTranslator.CurLuaBehaviour = this;
#endif
            luaAwake();
        }
    }

    public string GetPath()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(name);
        Transform t = transform.parent;
        while (t != null)
        {
            sb.Insert(0, t.name + ".");
            t = t.parent;
        }
        return sb.ToString();
    }

    public LuaTable InitLua()
    {
        if (m_Init)
        {
            return scriptEnv;
        }
        if (string.IsNullOrEmpty(LuaPath))
        {
            return null;
        }
        if (string.IsNullOrEmpty(LuaPath.Trim()))
        {
            return null;
        }
        m_Init = true;
        LuaPath.Replace("\\", "/");
        TextAsset luaScript = CoreEntry.gResLoader.LoadTextAsset(LuaPath, SenLib.AssetType.Txt);
        if (luaScript == null)
        {
            LogMgr.LogWarning("luaScript is null. Path:" + LuaPath);
            return null;
        }

        LuaEnv luaG = LuaMgr.Instance.GetLuaEnv();

        scriptEnv = luaG.NewTable();

        LuaTable meta = luaG.NewTable();
        meta.Set("__index", luaG.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();
        meta = null;

        scriptEnv.Set("self", this);
        if (Injections != null)
        {
            foreach (var injection in Injections)
            {
                if (!string.IsNullOrEmpty(injection.name.Trim()) && injection.value != null)
                {
                    scriptEnv.Set(injection.name, injection.value);
                }
            }
        }

        var pathName = LuaPath.Replace("/", ".").Replace("\\", ".").Replace(".lua", string.Empty);
        pathName = "@Lua." + pathName;
        luaG.DoString(luaScript.text, pathName, scriptEnv);

        scriptEnv.Get("Awake", out luaAwake);
        scriptEnv.Get("Start", out luaStart);
        scriptEnv.Get("Update", out luaUpdate);
        scriptEnv.Get("OnEnable", out luaOnEnable);
        scriptEnv.Get("OnDisable", out luaOnDisable);
        scriptEnv.Get("OnDestroy", out luaOnDestroy);
        m_Index = CurIndex++;
        LuaMgr.AddLua(this);
        return scriptEnv;
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        if (luaStart != null)
        {
#if UNITY_EDITOR
            //ObjectTranslator.CurLuaBehaviour = this;
#endif
            luaStart();
        }
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (luaUpdate != null)
        {
#if UNITY_EDITOR
           //ObjectTranslator.CurLuaBehaviour = this;
#endif
            luaUpdate();
        }
    }

    protected virtual void OnEnable()
    {
        if (luaOnEnable != null)
        {
#if UNITY_EDITOR
            //ObjectTranslator.CurLuaBehaviour = this;
#endif
            luaOnEnable();
        }
    }
    protected virtual void OnDisable()
    {
        if (luaOnDisable != null)
        {
#if UNITY_EDITOR
            //ObjectTranslator.CurLuaBehaviour = this;
#endif
            luaOnDisable();
        }
    }
    protected virtual void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
#if UNITY_EDITOR
            //ObjectTranslator.CurLuaBehaviour = this;
#endif
            luaOnDestroy();
            luaOnDestroy = null;
        }

#if UNITY_EDITOR
        Button[] btns = GetComponentsInChildren<Button>(true);
        foreach (Button btn in btns)
        {
            btn.onClick.RemoveAllListeners();
            int n = btn.onClick.GetPersistentEventCount();
            for (int i=0;i<n;++i)
            {
                btn.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
            }
            btn.onClick.Invoke();
        }
#endif

        luaAwake = null;
        luaStart = null;
        luaUpdate = null;
        luaOnEnable = null;
        luaOnDisable = null;
        if (scriptEnv != null)
        {
            scriptEnv.Dispose();
            scriptEnv = null;
        }
        Injections = null;
        mCellCallBack = null;
        LuaMgr.RemoveLua(this);
    }

    public LuaTable GetEnvTable()
    {
        return scriptEnv;
    }

    ////////////////////////////////Scroll//////////////////////////////////////////

    [CSharpCallLua]
    public delegate void VoidDelegate(GameObject go, int idx);
    public VoidDelegate mCellCallBack;
    void ScrollCellIndex(int idx)
    {
        if (null != mCellCallBack)
        {
            mCellCallBack(this.gameObject, idx);
        }
    }

    ////////////////////////////////Scroll//////////////////////////////////////////
}

