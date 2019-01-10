using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using System;
using XLua;
[System.Serializable]
public class InjectionObj
{
    public string name;
    public GameObject value;
}


[LuaCallCSharp]

public class PanelBase : LuaBehaviour
{

    //protected PanelManager mPanelMgr = null;

    //public PanelManager PanelMgr
    //{
    //    get { return mPanelMgr; }
    //    set { mPanelMgr = value; }
    //}

    string mPanelName;
    public string PanelName
    {
        get { return mPanelName; }
        set { mPanelName = value; }
    }


    protected Canvas m_Canvas = null;

    public Canvas Canvas
    {
        get { return m_Canvas; }
        set { m_Canvas = value; }
    }


    public MainPanelUICfg mCfg = null;

    private int m_Layer = 0;
    private int m_DefaultSortOrder = 0;
    private int m_SortOrder = 0;

    public int Layer
    {
        get { return m_Layer; }
        set
        {
            m_Layer = value;
            if (m_Canvas != null)
            {
                m_Canvas.planeDistance = m_Layer * 200;
            }
        }
    }
    public int DefaultSortOrder
    {
        get { return m_DefaultSortOrder; }
        set { m_DefaultSortOrder = value; }
    }
    public int SortOrder
    {
        get { return m_SortOrder; }
        set
        {
            m_SortOrder = value;
            if (m_Canvas != null)
            {
                m_Canvas.overrideSorting = true;
                m_Canvas.sortingOrder = value;
            }
        }
    }

    protected Dictionary<string, GameObject> subObjects = new Dictionary<string, GameObject>();


    //上次打开的时间
    protected float m_lastShowTime = 0;
    public float LastShowTime
    {
        get { return m_lastShowTime; }
        set { m_lastShowTime = value; }
    }



    // 是否显示
    protected bool isShown;
    public bool IsShown { get { return isShown; } }



    // 自动释放时,  增加 延迟 m_inMemoryTime  秒后才释放
    protected float m_inMemoryTime = 0;

    public float InMemoryTime
    {
        get { return m_inMemoryTime; }
        set { m_inMemoryTime = value; }
    }

    protected Action luaOnShow;
    protected Action luaOnHide;
    
    protected Action AniCallFun;//全屏对话框动画结束回调
    
    protected override void Awake()
    {
        base.Awake();
        if (scriptEnv != null)
        {
            scriptEnv.Get("OnShow", out luaOnShow);
            scriptEnv.Get("OnHide", out luaOnHide);
            scriptEnv.Get("AniCallFun", out AniCallFun);
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void OnDestroy()
    {
        Hide();

#if UNITY_EDITOR
        ToggleText[] tts = GetComponentsInChildren<ToggleText>(true);
        foreach (ToggleText tt in tts)
        {
            tt.action = null;
        }
#endif
        luaOnShow = null;
        luaOnHide = null;
        AniCallFun = null;

        base.OnDestroy();        
    }

    //AniCallFun
    public void AnimationDialogCallBack()
    {
        if (AniCallFun != null)
        {
            AniCallFun();
        }
    }
    Vector2 GroupPos = new Vector2(-10000,0);

    public virtual void Show()
    {
        if (isShown)
        {
            return;
        }
        
        if (mCfg.type == 2)
        {
            //CoreEntry.gAudioMgr.PlayUISound(900001);
        }
        if (mCfg != null && mCfg.AnimationType == 2)
        {
            Transform ToggleGroup = this.transform.DeepFindChild("ToggleGroup");
            if (ToggleGroup != null)
            {
                if (GroupPos.x == -10000)
                {
                    RectTransform rectTrs = ToggleGroup.GetComponent<RectTransform>();
                    if (rectTrs != null)
                    {
                        GroupPos = rectTrs.anchoredPosition;
                    }
                }
                if (GroupPos.x != -10000)
                {
                    UIAnimationMgr.Instance.PlayAni(ToggleGroup.gameObject, null, 20, GroupPos.x, GroupPos.y);
                }
            }
        }


        LastShowTime = Time.time;
        isShown = true;        
        int tmpLayer = LayerMask.NameToLayer("UI");
        GameObject obj = this.gameObject;
        CommonTools.SetLayer(obj, tmpLayer);
        if (!obj.activeSelf)
        {
            obj.SetActive(true);
        }
        //gameObject.gameObject.SetActive(true);//.transform.SetRenderActive(true);
        UnityEngine.UI.GraphicRaycaster gray = this.GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (gray != null)
        {
            gray.enabled = true;
        }
        if (luaOnShow != null)
        {
            luaOnShow();
        }
        OnShow();

        if (!mCfg.ignoreguide)
        {
            EventParameter ep = EventParameter.Get(mPanelName);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UI_OPEN, ep);
        }        
    }

    public virtual void Hide()
    {
        if (!isShown || mCfg == null)       //好奇怪，有时候mCfg会是null
        {
            return;
        }
        
        if (mCfg.type == 2)
        {
            //CoreEntry.gAudioMgr.PlayUISound(900002);
        }


        LastShowTime = Time.time;
        isShown = false;
        try
        {
            int tmpLayer = LayerMask.NameToLayer("outui");
            CommonTools.SetLayer(this.gameObject, tmpLayer);
            UnityEngine.UI.GraphicRaycaster gray = this.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (gray != null)
            {
                gray.enabled = false;
            }
            //gameObject.gameObject.SetActive(false);
            if (luaOnHide != null)
            {
                luaOnHide();
            }
            OnHide();
        }
        catch (System.Exception ex)
        {
            LogMgr.UnityLog(ex.ToString());
        }

        if (!mCfg.ignoreguide)
        {
            EventParameter ep = EventParameter.Get(mPanelName);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UI_CLOSE, ep);
        }
    }

    public virtual void OnShow() { }
    public virtual void OnHide() { }


    //返回上一个界面
    public virtual void OnReturnPrePanel(bool hasEffect = true)
    {
        LogMgr.UnityLog("Error! 没有实现OnReturnPrePanel");

    } 

    /// ///////////////////////////////////////
    /// 
     
    public virtual void OnMsg(params object[] param)
    { 
    }

    protected void AddSubObject<T>(string name) where T : Component
    {
        GameObject obj = transform.FindChild(name).gameObject;//GameObject.Find(name);
        if (null != obj)
        {
            subObjects.Add(name, obj);
        }
    }

    protected T FindSubObject<T>(string name) where T : Component
    {
        if (subObjects.ContainsKey(name))
        {
            GameObject obj = subObjects[name];
            T t = obj.GetComponent<T>();

            return t;
        }

        return default(T);
    }

    public void ConnectStart()
    {
        Invoke("ConnectTimeOut", 10);
    }

    public void ConnectTimeOut()
    {
    }
    public void ConnectSuccess()
    {
        CancelInvoke("ConnectTimeOut");
    }

    public void Close()
    {
        if (mCfg != null)
        {
            MainPanelMgr.Instance.Close(mCfg.panelName);
        }        
    }

    /// <summary>
    /// 获取引导控件。
    /// </summary>
    /// <param name="name">控件名称。</param>
    /// <returns>控件对象。</returns>
    public GuideWidget GetGuideWidget(string name)
    {
        GuideWidget[] widgets = GetComponentsInChildren<GuideWidget>();
        for (int i=0; i<widgets.Length; ++i)
        {
            if (widgets[i].gameObject.activeSelf && widgets[i].Name.CompareTo(name) == 0)
            {
                return widgets[i];
            }
        }
        return null;
    }
}

