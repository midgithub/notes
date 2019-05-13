using UnityEngine;
using System.Collections;
using SG;

using XLua;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[LuaCallCSharp]
public class MainPanelUICfg
{
    public int id;///id
    public string panelName;///UI名
    public int type;        //类型
    public bool fullview;           //全屏显示
    public bool ignoreguide;     //是否忽略引导
    public bool cache;              //是否缓存
    public bool preload;              //是否预加载
    public string prefabPath;///uiprefab路径
    public int subTypeLayer;///子层级
    public int enumLayerType;///分类层级
    public int AnimationType;//动画类型 0,无动画 ,1 非全屏缩放动画

    ///
}

[LuaCallCSharp]
[Hotfix]
public class MainPanelMgr : PanelManager
{
    ////////////////////////////////////单件处理//////////////////////////////////////////////// 
    private static MainPanelMgr m_instance;

    private static string m_firstPanel = null;





    public static void gotoPanel(string panelName)
    {
        m_firstPanel = panelName;
        //Debug.LogError(m_firstPanel + " " + panelName);
    }


    public override void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as MainPanelMgr;
        }
        else
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    public void Update()
    {
        UpdateCamera();
    }

    public virtual void OnApplicationQuit()
    {
        m_instance = null;
    }

    protected virtual void OnDestroy()
    {
        if (m_instance == this)
        {
            LogMgr.UnityLog("[SingletonMonoBehaviour] destory singleton. type=" + typeof(MainPanelMgr).ToString());
            m_instance = null;
        }
    }

    public static MainPanelMgr Instance
    {
        get
        {
#if MANAGER_SELF_OBJ
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType(typeof(MainPanelMgr)) as MainPanelMgr;
                if (m_instance == null)
                {
                    LogMgr.UnityLog("Create Instance : " + typeof(MainPanelMgr).ToString());
                    m_instance = new GameObject(typeof(MainPanelMgr).ToString()).AddComponent<MainPanelMgr>();
                }
                GameObject.DontDestroyOnLoad(m_instance.gameObject);
            }

#else
            m_instance = CoreEntry.gMainPanelMgr;
#endif
            return m_instance;
        }
    }


    ////////////////////////////////////单件处理//////////////////////////////////////////////// 



    public override void Init()
    {
        base.Init();

        if (mPrefabMap.Count <= 0)
        {
            Dictionary<int, LuaTable> data = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_MainPanelUI");
            if (data == null)
            {
                LogMgr.LogError("请检查 MainPanelUIConfig ");
                return;
            }
            foreach (KeyValuePair<int, LuaTable> v in data)
            {
                LuaTable t = v.Value;
                MainPanelUICfg mpUI = new MainPanelUICfg();
                mpUI.id = t.Get<int>("id");
                mpUI.panelName = t.Get<string>("panelName");
                mpUI.type = t.Get<int>("type");
                mpUI.fullview = t.Get<bool>("fullview");
                mpUI.ignoreguide = t.Get<bool>("ignoreguide");
                mpUI.cache = t.Get<bool>("cache");
                mpUI.preload = t.Get<bool>("preload");
                mpUI.prefabPath = t.Get<string>("prefabPath");
                mpUI.subTypeLayer = t.Get<int>("subTypeLayer");
                mpUI.enumLayerType = t.Get<int>("enumLayerType");
                mpUI.AnimationType = t.Get<int>("AnimationType");
                if (mpUI.panelName.Length > 2)
                {
                    RegPanel(mpUI, mpUI.panelName);
                }

                if (mpUI.preload)
                {
                    mPreloadList.Add(mpUI);
                }
            }
        }        

        //预加载
        if (PlayerData.Instance.RoleID != 0)
        {
            for (int i = 0; i < mPreloadList.Count; ++i)
            {
                MainPanelUICfg cfg = mPreloadList[i];
                if (!panelDic.ContainsKey(cfg.panelName))
                {
                    LoadPanel(cfg, cfg.panelName);
                }
            }
        }
        
        showFirstPanel();
    }


    //显示第一个UI
    void showFirstPanel()
    {
        if (m_firstPanel != null)
        {
            if (MapMgr.Instance.CurMapType == MapMgr.MapType.Map_SelectRole)
            {
                ShowDialog("UIWaitDialog");
            }
            ShowPanel(m_firstPanel);

            m_firstPanel = null;
        }
        else
        {
            ShowPanel("UIMain");
        }

    }



    #region --------------------Add by XuXiang--------------------

    /// <summary>
    /// 页面跳转参数。
    /// </summary>
    private LuaTable m_JumpParam = null;

    /// <summary>
    /// 获取或设置页面跳转参数。
    /// </summary>
    public LuaTable JumpParam
    {
        get { return m_JumpParam; }
        set { m_JumpParam = value; }
    }

    /// <summary>
    /// 打开一个界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    /// <returns>打开的UI对象。</returns>
    public PanelBase Open(string name)
    {
       // Debug.LogError("=========打开一个界面。=====1="+ name);
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(name, out cfg))
        {
          //  Debug.LogError("=========打开一个界面。=====2=");
            if (cfg.type == 1)
            {
              //  Debug.LogError("=========打开一个界面。=====3=");
                ShowPanel(name);
                return GetPanel(name);
            }
            else if (cfg.type == 2)
            {
               // Debug.LogError("=========打开一个界面。=====4");
                CloseCurPanel();
                return ShowDialog(name);
            }
        }
        return null;
    }

    /// <summary>
    /// 异步打开一个界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    /// <param name="call">打开的UI对象。</param>
    public void OpenAsyn(string name, Action<PanelBase> call)
    {
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(name, out cfg))
        {
            MainPanelMgr.Instance.Open("UIWaitDialog");

            LoadModule.Instance.LoadPrefab(cfg.prefabPath, (o) =>
            {
                if (cfg.type == 1)
                {
                    ShowPanel(name, false, () => {
                        if (call != null)
                        {
                            call(GetPanel(name));
                        }
                        MainPanelMgr.Instance.Close("UIWaitDialog");
                    });

                }
                else if (cfg.type == 2)
                {
                    PanelBase panel = ShowDialog(name);
                    if (call != null)
                    {
                        call(panel);
                        MainPanelMgr.Instance.Close("UIWaitDialog");
                    }
                }
                else
                {
                    MainPanelMgr.Instance.Close("UIWaitDialog");
                    if (call != null)
                    {
                        call(null);
                    }
                }
            });
        } 
    }
 
    /// <summary>
    /// 关闭一个界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    public void Close(string name)
    {
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(name, out cfg))
        {
            if (cfg.type == 1)
            {
                HidePanel(name);
            }
            else if (cfg.type == 2)
            {
                HideDialog(name);
            }
        }
    }
    /// <summary>
    /// 关闭当前界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    public void CloseCurPanel()
    {
        LogMgr.DebugLog(CurPanelName);
        if (string.IsNullOrEmpty(CurPanelName)|| CurPanelName.Equals("UIMain")) return;
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(CurPanelName, out cfg))
        {
            if (cfg.type == 1)
            {
                HidePanel(CurPanelName);
            }
            else if (cfg.type == 2)
            {
                HideDialog(CurPanelName);
            }
        }
    }
    /// <summary>
    /// 获取当前显示的最顶层UI。
    /// </summary>
    /// <param name="topbar">是否包含TopBar</param>
    /// <returns>UI对象。</returns>
    public PanelBase GetTopUI(bool topbar = false)
    {
        PanelBase ret = null;
        var e = panelDic.GetEnumerator();
        while (e.MoveNext())
        {
            PanelBase panel = e.Current.Value;
            if (panel.IsShown && ((topbar && panel.PanelName == "CommonTopBar") || !panel.mCfg.ignoreguide))
            {
                if (ret == null || ret.Canvas.sortingOrder < panel.Canvas.sortingOrder)
                {
                    ret = panel;
                }
            }
        }
        e.Dispose();

        return ret;
    }

    /// <summary>
    /// 是否还有全屏显示的UI。
    /// </summary>
    public bool IsHaveFullView()
    {
        bool ret = false;
        var e = panelDic.GetEnumerator();
        while (e.MoveNext())
        {
            PanelBase panel = e.Current.Value;
            if (panel.IsShown && panel.mCfg.fullview)
            {
                ret = true;
                break;
            }
        }
        e.Dispose();

        return ret;
    }
    public  bool  bInsertStatus = false;
    public void UpdateCamera()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            if (IsShow("TipShowUI"))
            {
                bInsertStatus = false;
                Close("TipShowUI");
            }
            else
            {
                bInsertStatus = true;
                Open("TipShowUI");
            }

        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            LogMgr.UnityError("########### 主动关闭链接 ##########");
            CoreEntry.netMgr.Reconnect.needShowNotice = false;
            CoreEntry.netMgr.disconnect();

        }

        if (Input.GetKeyDown(KeyCode.C)) {

            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            string platform = G.GetInPath<string>("ModelManager.LoginModel.CurPlatform");

            LogMgr.UnityLog("platform: " + platform);

            Dictionary<int, LuaTable> m_levelFuncOpen = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.CommonConfig.LevelFuncOpen");

            Dictionary<int, LuaTable> m_resconfig = G.GetInPath<Dictionary<int, LuaTable>>("ConfigData.ResourcepackageConfig.Resourcepackage");
            Dictionary<int, LuaTable>.Enumerator iter = m_resconfig.GetEnumerator();
            foreach (var item in m_resconfig) {
                LuaTable tbl = item.Value;
                int levelDown = tbl.Get<int>("leveldown");
                int levelUp = tbl.Get<int>("levelup");
            }
        }

        UpdateMainPlayerPos();

        //UI摄像机处于飞激活状态
        if (uiCamera != null && !uiCamera.enabled)
        {
            UpdateCameraControl();
        }
#endif

    }

    public void UpdateCameraControl()
    {
        //摄像机获取
        if (m_CF == null)
        {
            if (CoreEntry.gCameraMgr.MainCamera == null)
                return;
            m_CF = CoreEntry.gCameraMgr.MainCamera.GetComponent<CameraFollow>();
            if (m_CF == null)
            {
                return;
            }
        }
		
        UpdateCameraRotate();
        UpdateCameraFarNear();
        UpdateCameraFocus();
    
    }

    public void UpdateMainPlayerPos() {
        PlayerObj mainPlayer = CoreEntry.gActorMgr.MainPlayer;
        if (mainPlayer == null) return;
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor
            ) {

            if (Input.GetKey(KeyCode.W)) {

                Vector3 currPos = CoreEntry.gActorMgr.MainPlayer.GetPosition();
                mainPlayer.MoveToDir(CoreEntry.gCameraMgr.MainCamera.transform.forward);
            }

            if (Input.GetKey(KeyCode.S))
            {
                mainPlayer.MoveToDir(-CoreEntry.gCameraMgr.MainCamera.transform.forward);
            }

            if (Input.GetKey(KeyCode.A))
            {
                mainPlayer.MoveToDir(-CoreEntry.gCameraMgr.MainCamera.transform.right);
            }

            if (Input.GetKey(KeyCode.D))
            {
                mainPlayer.MoveToDir(CoreEntry.gCameraMgr.MainCamera.transform.right);
            }
            
        }
    }

    /// <summary>
    /// 镜头旋转。
    /// </summary>
    public void UpdateCameraRotate()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            m_Rotate = !m_Rotate;
        }
        if (m_Rotate)
        {
            if (Input.GetKey(KeyCode.PageUp))
            {
                m_RotateSpeed = Mathf.Clamp(m_RotateSpeed + RotateSpeedDelta * Time.deltaTime, -180, 180);
            }
            if (Input.GetKey(KeyCode.PageDown))
            {
                m_RotateSpeed = Mathf.Clamp(m_RotateSpeed - RotateSpeedDelta * Time.deltaTime, -180, 180);
            }
            m_CF.m_rotationAngle += m_RotateSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// 更新焦点。
    /// </summary>
    public void UpdateCameraFocus()
    {
        //主角锁定
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            if (m_CF != null)
            {
                m_CF.m_target = m_CF.m_target == null ? CoreEntry.gActorMgr.MainPlayer.transform : null;
            }
        }
        if(m_CF==null)
        {
            return;
        }
        //焦点移动
        if (m_CF.m_target == null)
        {
            float mx = 0;
            float my = 0;
            float mz = 0;
            if (Input.GetKey(KeyCode.Keypad8)) { my += m_FocusMoveSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Keypad2)) { my -= m_FocusMoveSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Keypad4)) { mx -= m_FocusMoveSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Keypad6)) { mx += m_FocusMoveSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Keypad1)) { mz += m_FocusMoveSpeed * Time.deltaTime; }
            if (Input.GetKey(KeyCode.Keypad3)) { mz -= m_FocusMoveSpeed * Time.deltaTime; }

            if (Math.Abs(mx) + Math.Abs(my) + Math.Abs(mz) > 0)
            {
                Transform t = m_CF.transform;
                Vector3 offset = t.right * mx + t.up * my + t.forward * mz;
                m_CF.mLastCameraPosition = m_CF.mLastCameraPosition + offset;
            }

            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                m_FocusMoveSpeed = Mathf.Clamp(m_FocusMoveSpeed + FocusMoveSpeedDelta, 1, 40);
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                m_FocusMoveSpeed = Mathf.Clamp(m_FocusMoveSpeed - FocusMoveSpeedDelta, 1, 40);
            }
        }
    }

    /// <summary>
    /// 拉近拉远
    /// </summary>
    public void UpdateCameraFarNear()
    {
        if (Input.GetKey(KeyCode.Home))
        {
            m_Rotate = false;
            m_CF.m_distance = Mathf.Clamp(m_CF.m_distance - m_FarNearSpeed * Time.deltaTime, 2, 50);
        }
        if (Input.GetKey(KeyCode.End))
        {
            m_Rotate = false;
            m_CF.m_distance = Mathf.Clamp(m_CF.m_distance + m_FarNearSpeed * Time.deltaTime, 2, 50);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            m_FarNearSpeed = Mathf.Clamp(m_FarNearSpeed + FarNearSpeedDelta, 1, 40);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            m_FarNearSpeed = Mathf.Clamp(m_FarNearSpeed - FarNearSpeedDelta, 1, 40);
        }
    }

    private CameraFollow m_CF;

    /// <summary>
    /// 是否环绕旋转。
    /// </summary>
    private bool m_Rotate = false;

    /// <summary>
    /// 环绕旋转速度。
    /// </summary>
    private float m_RotateSpeed = 45;

    /// <summary>
    /// 拉近拉远速度。
    /// </summary>
    private float m_FarNearSpeed = 10;

    /// <summary>
    /// 焦点移动速度。
    /// </summary>
    private float m_FocusMoveSpeed = 10;

    /// <summary>
    /// 旋转速度每秒变化量。
    /// </summary>
    private static float RotateSpeedDelta = 360;

    /// <summary>
    /// 拉远缩进速度。
    /// </summary>
    private static float FarNearSpeedDelta = 1;

    /// <summary>
    /// 焦点移动速度。
    /// </summary>
    private static float FocusMoveSpeedDelta = 1;

    #endregion
    private static Dictionary<string, Texture2D> m_TextureSet = new Dictionary<string, Texture2D>();
    public static Texture2D GetTexture2D(string strKey)
    {
        if (m_TextureSet.ContainsKey(strKey))
        {
            return m_TextureSet[strKey];
        }
        return null;
    }

    public static void ClearTexture2D()
    {
        m_TextureSet.Clear();
    }

    public static IEnumerator LoadStreamTexture(string strImage,string realName)
    {
        string urlImage = Application.streamingAssetsPath + "/" + strImage;
        if (Application.platform == RuntimePlatform.Android)
        {
            urlImage = Application.streamingAssetsPath + "/" + strImage;
        }
        else
        {
            urlImage = "file://" + Application.streamingAssetsPath + "/" + strImage;
        }

        Debug.Log("load path: " + urlImage);
        WWW www = new WWW(urlImage);
        yield return www;

        if (www != null && string.IsNullOrEmpty(www.error))
        {
            if (string.IsNullOrEmpty(realName))
            {
                m_TextureSet[strImage] = www.texture;
            }else
            {
                m_TextureSet[realName] = www.texture;
            }
        }
        else
        {
            Debug.LogError(www.error);
        }

        if (www.isDone)
        {
            www.Dispose();
        }
    }
}
