using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using XLua;
using UnityEngine.UI;

[LuaCallCSharp]

public class PanelManager :MonoBehaviour
{
    public static int MaxCacheNumber = 4;               //最大缓存的UI数量

    public string uUIRootPath = @"UI/Prefabs/root/FirstRes/uUIRoot";
    private string root3DPrefabPath = "UI/Prefabs/root/FirstRes/UIRoot3D";

    protected Dictionary<string, MainPanelUICfg> mPrefabMap = new Dictionary<string, MainPanelUICfg>();
    protected List<MainPanelUICfg> mPreloadList = new List<MainPanelUICfg>();
    protected Dictionary<string, int> m_showTimesMap = new Dictionary<string, int>();
    protected Dictionary<string, PanelBase> panelDic = new Dictionary<string, PanelBase>();
    protected List<PanelBase> mShowDlgList = new List<PanelBase>();
    
    public Camera uiCamera { get; private set; }
    public GameObject uUIRootObj;
    private GameObject uiCameraObj;
    private GameObject mUIRoot3d;

    public UnityEngine.GameObject UIRoot3d
    {
        get { return mUIRoot3d; }
        set { mUIRoot3d = value; }
    }

   
    protected Stack<string> m_PanelNameStack = new Stack<string>();
    public Stack<string> PanelNameStack
    {
        get { return m_PanelNameStack; }
        set { m_PanelNameStack = value; }
    }

    public static int MaxSortOrder { get { return 1000; } }
    public static int GetLayerSortOrder(int layer)
    {
        return (MaxSortOrder - layer * 100);
    }


    public virtual void Awake()
    { 
    }

    public virtual void Init()
    {
        mUIRoot3d = GameObject.Find("UIRoot3D");
        if (mUIRoot3d == null)
        {
            GameObject uiRoot2DPrefab = CoreEntry.gResLoader.Load(root3DPrefabPath) as GameObject;
            if(uiRoot2DPrefab== null)return;
            mUIRoot3d = GameObject.Instantiate(uiRoot2DPrefab) as GameObject;
            mUIRoot3d.name = "UIRoot3D";
            mUIRoot3d.transform.position = new Vector3(0, 1000, 0);

            GameObject.DontDestroyOnLoad(mUIRoot3d);
        }

        if (uUIRootObj == null)
        {
            GameObject rootPrefab = (GameObject)CoreEntry.gResLoader.Load(uUIRootPath, typeof(GameObject));
            if(rootPrefab == null)return;
            uUIRootObj = GameObject.Instantiate(rootPrefab);
            uUIRootObj.transform.parent = null;
            uUIRootObj.SetActive(true);
            //uiCamera = rootPrefab.GetComponentInChildren<Camera>();  
            GameObject.DontDestroyOnLoad(uUIRootObj);
            //InvokeRepeating("CheckDestroyMemery", 1, 1);              //目前太多功能写在UI层了，暂时不清理隐藏的UI，由切换场景负责清理
        } 
    }

    /// <summary>
    /// 判断某个UI是否为预加载。
    /// </summary>
    /// <param name="pname">UI名称。</param>
    /// <returns>是否为预加载。</returns>
    public bool IsPreload(string pname)
    {
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(pname, out cfg))
        {
            return cfg.preload;
        }
        return false;
    }

    //
    public virtual void Release()
    {
        //只保留预加载的UI名称
        Stack<string> temp = new Stack<string>(m_PanelNameStack.Count);
        while (m_PanelNameStack.Count > 0)
        {
            string pname = m_PanelNameStack.Pop();
            if (IsPreload(pname))
            {
                temp.Push(pname);
            }
        }
        while (temp.Count > 0)
        {
            m_PanelNameStack.Push(temp.Pop());
        }

        //弹窗列表
        m_showTimesMap.Clear();
        for (int i=0; i< mShowDlgList.Count;)
        {
            if (!mShowDlgList[i].mCfg.preload)
            {
                mShowDlgList.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }

        //UI对象
        List<PanelBase> panels = new List<PanelBase>();
        var e = panelDic.GetEnumerator();
        while (e.MoveNext())
        {
            var v = e.Current.Value;
            if (v.mCfg.preload)
            {
                v.Hide();
                v.gameObject.SetActive(false);
                panels.Add(v);
            }
            else
            {
                Destroy(v.gameObject);
            }
        }
        e.Dispose();
        panelDic.Clear();
        for (int i=0; i<panels.Count; ++i)
        {
            PanelBase pb = panels[i];
            panelDic.Add(pb.PanelName, pb);            
        }
        panels.Clear();
        mCurPanelName = string.Empty;
    }

    public int layerHide = LayerMask.NameToLayer("outui");
    public void CheckDestroyMemery()
    {
        Dictionary<string, PanelBase>.Enumerator enumerator = panelDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var data = enumerator.Current;
            PanelBase panel = data.Value;

            int times = 0;
            if (m_showTimesMap.ContainsKey(data.Key))
            {
                times = m_showTimesMap[data.Key];
            }
            //不动态释放的界面......
            if (panel.PanelName.Equals("UIMain") || panel.mCfg.preload || m_PanelNameStack.Contains(panel.PanelName))
            {
                continue;
            }

            //使用次数多的界面 存在时间长, 使用次数少的界面 存在时间短
            if (Time.time - panel.LastShowTime > panel.InMemoryTime + times * 20 && (panel.gameObject.layer == layerHide))
            {
                panelDic.Remove(data.Key);
                GameObject.Destroy(panel.gameObject);
                break;
            }
        }
        enumerator.Dispose();
    }

    public void RegPanel(MainPanelUICfg mpCfg, string panelName)
    {
        if (!mPrefabMap.ContainsKey(panelName))
        {
            mPrefabMap.Add(panelName, mpCfg);
        }
    }

    /// <summary>
    /// 获取UI配置。
    /// </summary>
    /// <param name="name">UI名称。</param>
    /// <returns>UI配置。</returns>
    public MainPanelUICfg GetPanelConfig(string name)
    {
        MainPanelUICfg cfg;
        mPrefabMap.TryGetValue(name, out cfg);
        return cfg;
    }

    public void LoadPanel(MainPanelUICfg mpUI, string panelName)
    {
        GameObject panelPrefab = CoreEntry.gResLoader.Load(mpUI.prefabPath) as GameObject;
        if (panelPrefab == null)
        {
            LogMgr.UnityError("没有找到 界面路径:" + mpUI.prefabPath);
            return;
        }

        GameObject panelObj = GameObject.Instantiate(panelPrefab);
        if (panelObj.activeSelf)
        {
            LogMgr.LogError("保存该UI预设体时，名字左侧不能勾选保存！ name:{0}", panelName);
        }
        PanelBase panelBase = panelObj.GetComponent(typeof(PanelBase)) as PanelBase;
        panelBase.Canvas = panelBase.GetComponent<Canvas>();
        panelBase.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        panelBase.Canvas.worldCamera = uiCamera;
        panelBase.mCfg = mpUI;

        panelObj.transform.SetParent(uUIRootObj.transform);
        RectTransform rectTrans = panelObj.transform as RectTransform;
        rectTrans.localPosition = Vector3.zero;
        rectTrans.sizeDelta = new Vector2(0, 0);
        rectTrans.localScale = panelObj.transform.lossyScale;

        panelBase.DefaultSortOrder = GetLayerSortOrder((int)mpUI.enumLayerType);
        panelBase.Layer = (int)(mpUI.enumLayerType + 1);
        panelBase.SortOrder =  panelBase.DefaultSortOrder + mpUI.subTypeLayer;
        //panelBase.PanelMgr = this;
        panelBase.PanelName = panelName;
        panelObj.name = panelName;
        panelDic.Add(panelName, panelBase);
        panelObj.gameObject.SetActive(false);
    }
    public bool HasPanel(string panelName)
    {
        return mPrefabMap.ContainsKey(panelName);    
    }

    /// <summary>
    /// 判断某个UI是否显示。
    /// </summary>
    /// <param name="panelName">UI名称。</param>
    /// <returns>是否显示。</returns>
    public bool IsShow(string panelName)
    {
        PanelBase panel;
        if (panelDic.TryGetValue(panelName, out panel))
        {
            return panel.IsShown;
        }
        return false;
    }

    public void preLoad(string panelName)
    {
        if (!panelDic.ContainsKey(panelName))
        {
            if (mPrefabMap.ContainsKey(panelName))
            {
                LoadPanel(mPrefabMap[panelName], panelName);
            }
        } 
    }

    public PanelBase GetPanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            return panelDic[panelName];
        }
        else
        {
            if (mPrefabMap.ContainsKey(panelName))
            {
                LoadPanel(mPrefabMap[panelName], panelName);
                panelDic[panelName].gameObject.SetActive(true);
                return panelDic[panelName];
            }
            else
            { 
            }

        } 
        return null;
    }

    public PanelBase GetLoadedPanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            return panelDic[panelName];
        }

        return null;
    }

    public void ShowPanel(string panelName, bool hasEffect = true, System.Action finishCallBack = null)
    { 
        if(panelName == "LoginPanel")
        {
            SDKMgr.Instance.bLoginStatus = true;
        }

        if (uiCamera == null)
            uiCamera = uUIRootObj.GetComponentInChildren<Camera>();  
        if (m_showTimesMap.ContainsKey(panelName))
        {
            m_showTimesMap[panelName] += 1;
        }
        else
        {
            m_showTimesMap.Add(panelName, 1);
        }

         

        CoreEntry.gTimeMgr.TimeScale = 1f;  //重置时间对协助的影响

        if (!HasPanel(panelName))
        {
            LogMgr.UnityLog("has not pannel:" + panelName);
            return;
        }

        if (panelName.Equals(mCurPanelName))
        {
            return;
        }
        if (string.IsNullOrEmpty(mCurPanelName))
        {
            CurPanelName = panelName;
            GetPanel(panelName).Show();

            if (finishCallBack != null)
                finishCallBack();
            return;
        }

        string tempPanelName = mCurPanelName;

        CurPanelName = panelName;
       
      StartCoroutine(delayHide(panelName, tempPanelName, finishCallBack)); 
    }

    //
    IEnumerator delayHide(string showName, string hideName, System.Action finishCallBack = null)
    {
        LogMgr.DebugLog("------------------" + showName + "  start  --------------");
        if (!isLoadedPanel(showName))
        {
//             PanelBase waitDialog = GetPanel("WaitDialog");
//             waitDialog.Show(true);
            yield return new WaitForSeconds(0.001f);
        }

        PanelBase newPanel = GetPanel(showName);

        //yield return new WaitForSeconds(0.01f);

        PanelBase hidePanel = null;
        if (panelDic.ContainsKey(hideName))
        {
            hidePanel = panelDic[hideName];
        }

        if (hidePanel != null)
            hidePanel.Hide();
        if(newPanel!=null)
        newPanel.Show();

        //GetPanel("WaitDialog").Hide(true);
        LogMgr.DebugLog("------------------" + showName + "  end  --------------");
        //CoreEntry.gAudioMgr.PlayUISound(100048);

        if (finishCallBack != null)
            finishCallBack();

        yield return true;
    }
    public bool isLoadedPanel(string panelName)
    {
        return panelDic.ContainsKey(panelName);
    }
  
        //当前所在界面名
    protected string mCurPanelName;
    public string CurPanelName
    {
        get { return mCurPanelName; }
        set
        { 
            if (value.Equals(mCurPanelName))
            {
                return;
            }

            mCurPanelName = value;
            if (!string.IsNullOrEmpty(mCurPanelName))
            {
                m_PanelNameStack.Push(mCurPanelName);
                OnChangePanel();
            }
        }
    }

    public PanelBase getCurPanelObj()
    {
        return GetPanel(mCurPanelName);
    }


    public void OnChangePanel()
    {  
        List<PanelBase> tempList = new List<PanelBase>();

        for (int i = 0; i < mShowDlgList.Count; ++i)
        {
            if (mShowDlgList[i] != null)
            {
                tempList.Add(mShowDlgList[i]);
            }
        }

        for (int i = 0; i < tempList.Count; ++i)
        {
            if (tempList[i] != null)
            {
                tempList[i].Hide();
            }
        }
        mShowDlgList.Clear(); 
    }

    public string GetPrePanelName()
    {
        return m_PanelNameStack.ToArray()[1];
    }
    public bool ReturnPrePanel(bool hasEffect = true)
    { 
        if (m_PanelNameStack.Count == 0)
        {
            return false;
        }

        string lastPanelName = m_PanelNameStack.Pop();

        if (m_PanelNameStack.Count == 0)
        {
            return false;
        }
        while (!string.IsNullOrEmpty(mCurPanelName)
            && ( m_PanelNameStack.Count>0)
            && (lastPanelName.Equals("UIWaitDialog")
            || lastPanelName.Equals("PanelLoadingWindow")
            || lastPanelName.Equals("UIMain")
            || mCurPanelName.Equals(lastPanelName))) //防止返回到同一个界面
        {
            lastPanelName = m_PanelNameStack.Pop();
        }

        if ( lastPanelName.Equals("UIWaitDialog")
            || lastPanelName.Equals("PanelLoadingWindow") 
            || mCurPanelName.Equals(lastPanelName) )
        {
            return false;
        }

        string tempPanelName = mCurPanelName;
         
        if (lastPanelName == null)
        {
            return false;
        }

        CurPanelName = lastPanelName;

        OnChangePanel(); 

        PanelBase tmpPanel = GetPanel(tempPanelName);
        if (tmpPanel != null)
        {
            tmpPanel.Hide();
        }
        PanelBase panel = GetPanel(lastPanelName);
        if (panel != null)
        {
            panel.Show();
        }

        return true; 
    }

    public void HidePanel(string panelName)
    { 
        GetPanel(panelName).Hide();
        CheckMaxCache();
    }

    /// <summary>
    /// 检测最大缓存的UI。
    /// </summary>
    public void CheckMaxCache()
    {
        float mintime = 0;
        PanelBase lastpanel = null;
        var e = panelDic.GetEnumerator();
        int hidenumber = 0;
        while (e.MoveNext())
        {
            var panel = e.Current.Value;
            if (!panel.IsShown && !panel.mCfg.cache)
            {
                ++hidenumber;
                if (mintime < panel.LastShowTime)
                {
                    mintime = panel.LastShowTime;
                    lastpanel = panel;
                }                
            }
        }
        e.Dispose();
        if (hidenumber > MaxCacheNumber && lastpanel != null)
        {
            DestroyPanel(lastpanel.PanelName);
        }
    }

    public void DestroyPanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            var panel = panelDic[panelName];
            GameObject.Destroy(panel.gameObject);
            panelDic.Remove(panelName);
        }
    }

    public void HideAllPanel(bool disable)
    {
        List<PanelBase> allPanels = new List<PanelBase>();
        foreach (KeyValuePair<string, PanelBase> data in panelDic)
        {
            PanelBase panel = data.Value;
            allPanels.Add(panel);
        }
        for (int i = 0; i < allPanels.Count; i++)
        {
            PanelBase panel = allPanels[i];
            if (null != panel)
            {
                panel.Hide();
            }
        }
        mCurPanelName = "";
        CheckMaxCache();
    }

    public PanelBase ShowDialog(string panelName, bool hasEffect = true, int depth = 0)
    {
        if (!HasPanel(panelName))
        {
            LogMgr.UnityLog("has not pannel:" + panelName);
            return null;
        }

        if (m_showTimesMap.ContainsKey(panelName))
        {
            m_showTimesMap[panelName] += 1;
        }
        else
        {
            m_showTimesMap.Add(panelName, 1);
        }

        PanelBase dialog = GetPanel(panelName);
        //dialog.PanelMgr = this;
        mShowDlgList.Add(dialog);
        if(dialog!=null)
        {
            //已经显示出来的UI不再播动画
            if (!dialog.IsShown)
            {
                MainPanelUICfg cfg;
                if (mPrefabMap.TryGetValue(panelName, out cfg))
                {
                    if (cfg.AnimationType == 1)
                    {
                        UIAnimationMgr.Instance.PlayAnimation(dialog.gameObject, dialog.AnimationDialogCallBack);
                    }
                }
            }
            dialog.Show();
        }


        return dialog; 
    }

    public void HideAllDialog()
    { 
        foreach (var child in mShowDlgList)
        {
            if (child != null && child.gameObject != null && child.gameObject.activeSelf && child.name != "CommonTopBar")
            {
                child.gameObject.SetActive(false);
            }
        }
        CheckMaxCache();
    }

    public bool HasDialog()
    {
        bool flag = false;
        for (int i=0; i< mShowDlgList.Count; ++i)
        {
            PanelBase child = mShowDlgList[i];
            if (child != null && child.gameObject != null && child.gameObject.activeSelf && child.name != "CommonTopBar")
            { 
                flag = true;
                break;
            }
        }
        return flag;
    }

    public PanelBase GetDialog(string panelName)
    {
        PanelBase panel = null;
        for (int i = 0; i < mShowDlgList.Count; i++)
        {
            PanelBase child = mShowDlgList[i];
            if (child != null && child.gameObject != null && child.name.Equals(panelName))
            {
                panel = child;
            }                
        }
        return panel;

    }

    public void DestroyDialog(string panelName)
    {
        HideDialog(panelName);
        if (panelDic.ContainsKey(panelName))
        {
            var panel = panelDic[panelName];
            GameObject.Destroy(panel.gameObject);
            panelDic.Remove(panelName);
        }
    }

    public void HideDialog(string panelName, bool hasEffect = true)
    {
        if (!panelDic.ContainsKey(panelName))
        {
            LogMgr.UnityLog("has not pannel:" + panelName);
            return;
        }
        if (!HasPanel(panelName))
        {
            LogMgr.UnityLog("has not pannel:" + panelName);
            return;
        }
        PanelBase dialog = GetPanel(panelName);
        if (dialog == null)
            return;
        mShowDlgList.Remove(dialog);
        dialog.Hide();
        CheckMaxCache();
    }
  

    //////////////////////////////////////


    public  static Vector2 GetUIScreenSize(Canvas cas)
    {

        CanvasScaler fs = cas.GetComponent<CanvasScaler>();

        Vector2 screenSize = fs.referenceResolution;



        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        Vector2 vResloution = fs.referenceResolution;


        if (fs.matchWidthOrHeight == 0)
        {
            float Scale = screenWidth / vResloution.x;
            screenSize.y = screenHeight / Scale;
        }
        else
        {
            float Scale = screenHeight / vResloution.y;
            screenSize.x = screenWidth / Scale;
        }

        return screenSize;

    }

    /// <summary>
    /// 设置是否显示UI。
    /// </summary>
    /// <param name="v">是否显示。</param>
    public void SetUIVisible(bool v)
    {
        uiCamera.enabled = v;
        if (v)
        {
            CoreEntry.gCameraMgr.moveToPlayer();
        }
    }
}

