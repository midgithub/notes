using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using SG;

[Hotfix]
public class LoginOut : MonoBehaviour
{
    private UIInitGame InitUI;
    private RawImage _bgImg;

    void Start () {
        GameObject go = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Start/FirstRes/UIInit", null, false);
        InitUI = go.transform.GetComponent<UIInitGame>();

        _bgImg = go.transform.Find("Back").GetComponent<RawImage>();
        if (null != _bgImg)
        {
            CommonTools.SetLoadImage(_bgImg, "Bg_loading");
        }

        InitUI.SetProgress(0);

        ClearScene();
        StartCoroutine(InitProcess());
    }

    private void OnDestroy()
    {
        if (null != _bgImg)
        {
            _bgImg.texture = null;
        }
    }

    private void ClearScene()
    {
        MainPanelMgr.Instance.Release();

        AtlasSpriteManager.Instance.ClearCache();
        CoreEntry.gSceneMgr.ClearPools(MapMgr.Instance.GetCurSceneID());
        CoreEntry.gObjPoolMgr.ReleaseObjectPool();
        CoreEntry.gGameObjPoolMgr.ClearPool();
        FlyAttrManager.CloseAllFlyAttr();

        CoreEntry.gResLoader.ClearPrefabs();
        LoadModule.Instance.Clear();
    }

    IEnumerator InitProcess()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        InitUI.SetLoadTip("游戏资源加载中......");
        float lastPercent = Random.Range(0.5f, 0.7f);
        float displayProgress = 0f;
        CoreEntry.gSceneMgr.PreloadScene("Scene/allMap/ui/RoleUI");
        while (CoreEntry.gSceneMgr.SceneLoading)
        {
            while (displayProgress < lastPercent)    //预加载
            {
                displayProgress += 0.1f;
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
                displayProgress += 0.2f;
                InitUI.SetProgress(displayProgress);
                yield return wait;
            }
            yield return wait;
        }

        InitUI.SetProgress(1);

        yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue("BackLogin"), string.Empty));
        yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue("LoginLogo2"), string.Empty));
        yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue(string.Format("Bg_loading{0}", UnityEngine.Random.Range(0, 4))), "Bg_loading"));

        MapMgr.Instance.EnterLoginScene();
    }

}
