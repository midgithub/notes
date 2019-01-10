using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
using Bundle;
using UnityEngine.UI;

[Hotfix]
public class AssetUpdateUI : MonoBehaviour 
{
    public Slider mUISlider;
    public Text mUILabel;
    public AssetUpdateMgr mAssetUpdateMgr;

    public AssetUpdateUIMsgTips mAssetUpdateUIMsgTips;

    void Awake()
    {
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_DOWNLOAD_ASSET, OnUpdateProgress);
        mAssetUpdateUIMsgTips.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_DOWNLOAD_ASSET, OnUpdateProgress); 
    }

    void Start()
    {
        if (AssetBundleLoadManager.IsTestMode)
        {
            OnLogin();
        }
        else
        {
            mAssetUpdateMgr = Bundle.AssetUpdateMgr.Instance;
            mAssetUpdateMgr.BeginUpdate();
        }
    }

    [ContextMenu("ShowPath")]
    void ShowPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    void OnUpdateProgress(GameEvent ge, EventParameter parameter)
    {
        mUILabel.text = parameter.stringParameter;
        mUISlider.value = parameter.floatParameter;
        if (parameter.intParameter == 9999)
        {
            OnLogin();
        }
        else if (parameter.intParameter == 10000)
        {
            var sizeData = parameter.objParameter as DownloadAssetSizeCaculate;
            if(sizeData.MaxSize < 1000)   //小于3M直接更新
            {
                mAssetUpdateMgr.ConfirmUpdate();
            }
            else
            {
                mAssetUpdateUIMsgTips.ShowMsg(sizeData, this, () =>
                {
                    mAssetUpdateMgr.ConfirmUpdate();
                });
            }
        }
        else if (parameter.intParameter == 10001)
        {
            mAssetUpdateUIMsgTips.ShowByText("网络无法连接，请检查网络是否连接!", () =>
            {
                mAssetUpdateMgr.BeginUpdate();
            });
        }
        else if (parameter.intParameter == 20000)
        {
            mAssetUpdateUIMsgTips.ShowByText("网络无法连接，请检查网络是否连接!", () =>
            {
                mAssetUpdateMgr.GetRemoteUrl();
            });
        }
        else if (parameter.intParameter == 20001)
        {
            System.Action callback = parameter.objParameter as System.Action;
            mAssetUpdateUIMsgTips.ShowByText("网络不佳，是否继续下载剩余资源?", () =>
            {
                if (callback != null)
                    callback();
            });
        }
        else if (parameter.intParameter == 30000)
        {
            mAssetUpdateUIMsgTips.TipsLabel.gameObject.SetActive(false);
            mAssetUpdateUIMsgTips.ShowByText("客户端有新版本了，需要重新下载安装", () =>
            {
                string url = parameter.objParameter1.ToString();
                if (string.IsNullOrEmpty(url))
                    Application.Quit();
                else
                    Application.OpenURL(url);
            });
        }
        else if (parameter.intParameter == 40000)
        {
            mAssetUpdateUIMsgTips.TipsLabel.gameObject.SetActive(false);
            mAssetUpdateUIMsgTips.ShowByText("本次更新了重要资源，需要重新打开客户端", () =>
            {
                Application.Quit();
            });
        }
    }

    void OnLogin()
    {
        AssetBundleLoadManager.Instance.UpdateBundleConfigData();
        ClientSetting.Instance.ReLoadClientSettingData();
        
        CoreEntry.gSceneMgr.enabled = true;
        CoreEntry.gTimeMgr.TimeScale = 1.0f;
        CoreEntry.leaveFight();
        CoreEntry.gGameMgr.resumeGame();

        //MapMgr.Instance.EnterLoginScene();
        MapMgr.Instance.EnterInitScene();
        

    }

}

