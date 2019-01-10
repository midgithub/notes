using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using SG;

[Hotfix]
public class LoginOut : MonoBehaviour
{
    private UIInitGame InitUI;

    void Start () {
        GameObject go = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Start/FirstRes/UIInit", null, false);
        InitUI = go.transform.GetComponent<UIInitGame>();

        RawImage bg = go.transform.Find("Back").GetComponent<RawImage>();
        if (null != bg)
        {
            CommonTools.SetLoadImage(bg, ClientSetting.Instance.GetStringValue("InitBg"));
        }

        InitUI.SetProgress(0);

        ClearScene();
        StartCoroutine(InitProcess());
    }

    private void ClearScene()
    {
        AtlasSpriteManager.Instance.ClearCache();
        CoreEntry.gSceneMgr.ClearPools(MapMgr.Instance.GetCurSceneID());
        CoreEntry.gObjPoolMgr.ReleaseObjectPool();

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

        yield return new WaitForSeconds(0.1f);

        MapMgr.Instance.EnterLoginScene();
    }

}
