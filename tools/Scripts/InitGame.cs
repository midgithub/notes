using UnityEngine;
using System.Collections;
using SG;
using XLua;
using UnityEngine.UI;

/// <summary>
/// 出事场景的逻辑。
/// </summary>
[Hotfix]
public class InitGame : MonoBehaviour
{
    [CSharpCallLua]
    public delegate bool InitGameDelegate();

    [CSharpCallLua]
    public delegate float GetInitProgressDelegate();

    /// <summary>
    /// 初始化UI。
    /// </summary>
    private UIInitGame InitUI;

    private InitGameDelegate m_LuaInitCall;
    private GetInitProgressDelegate m_LuaGetInitProCall;


    private void Awake()
    {
        CoreEntry.gEventMgr.AddListener(SG.GameEvent.GE_THIRDPARTY_INIT, OnThirdPartyInit);
        GameObject partyUi = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Start/FirstRes/ThirdPartyPanel", null, false);
        this.enabled = false;
    }

    void OnDestroy()
    {
        CoreEntry.gEventMgr.RemoveListener(SG.GameEvent.GE_THIRDPARTY_INIT, OnThirdPartyInit);
    }

    public void OnThirdPartyInit(GameEvent ge, EventParameter paramter)
    {
        this.enabled = true;
    }

    // Use this for initialization
    void Start()
    {
        SDKMgr.Instance.TrackGameLog("3000", "开始加载lua配置");
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        UnityEngine.Debug.Log("0000000000000000000");
        if (G != null)
        {
            m_LuaInitCall = G.GetInPath<InitGameDelegate>("InitGame");
            UnityEngine.Debug.Log("1111111111111111111");
            m_LuaGetInitProCall = G.GetInPath<GetInitProgressDelegate>("GetInitProgress");
            UnityEngine.Debug.Log("2222222222222222222");
            GameObject go = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Start/FirstRes/UIInit", null, false);
            InitUI = go.transform.GetComponent<UIInitGame>();
            RawImage bg = go.transform.Find("Back").GetComponent<RawImage>();
            if (null != bg)
            {
                bg.texture = ThirdPartyEntry._textureBg;
            }

            InitUI.SetProgress(0);
            StartCoroutine(InitProcess());
        }
    }

    IEnumerator InitProcess()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float gap = 0.05f;
        float time = 0.0f;
        float lastTime = Time.realtimeSinceStartup;
        float configPercent = Random.Range(0.8f, 0.9f);
        if (AppConst.UseAssetBundle)
        {
            configPercent = Random.Range(0.3f, 0.5f);
        }

        while (!m_LuaInitCall())
        {
            time += Time.realtimeSinceStartup - lastTime;
            if (time > gap)
            {
                lastTime = Time.realtimeSinceStartup;
                time = 0;
                InitUI.SetProgress(m_LuaGetInitProCall() * configPercent);                
                yield return wait;
            }
        }
        m_LuaInitCall = null;
        m_LuaGetInitProCall = null;
        yield return wait;

        InitUI.SetLoadTip("游戏资源加载中......");
        float lastPercent = configPercent + (1 - configPercent) * 0.5f;
        float displayProgress = configPercent;
        CoreEntry.gSceneMgr.PreloadScene("Scene/allMap/ui/RoleUI");
        while (CoreEntry.gSceneMgr.SceneLoading)
        {
            while (displayProgress < lastPercent)    //预加载
            {
                displayProgress += 0.005f;
                InitUI.SetProgress(displayProgress);
                yield return wait;
            }
            yield return wait;
        }

        lastPercent = 0.99f;
        CoreEntry.gSceneMgr.PreloadScene("Scene/allMap/ui/LoginUI");
        while (CoreEntry.gSceneMgr.SceneLoading)
        {
            while (displayProgress < lastPercent)    //预加载
            {
                displayProgress += 0.01f;
                InitUI.SetProgress(displayProgress);
                yield return wait;
            }
            yield return wait;
        }

        InitEnd();
        InitUI.SetProgress(1);

        yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue("BackLogin"),string.Empty));
        yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue("LoginLogo2"),string.Empty));
        yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue(string.Format("Bg_loading{0}", UnityEngine.Random.Range(0, 4))), "Bg_loading"));

        MapMgr.Instance.EnterLoginScene();
    }

    /// <summary>
    /// 初始化结束。
    /// </summary>
    private void InitEnd()
    {
        SDKMgr.Instance.TrackGameLog("3001", "lua配置加载结束");
        MainRole.Instance.RegisterNetMsg();
        MainRole.Instance.handle = 0;

        MainPanelMgr.Instance.Release();
        SettingManager.Instance.SetScreenResolution();
    }
}