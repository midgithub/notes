using XLua;
﻿using UnityEngine;
using System.Collections;
using Bundle;
using UnityEngine.UI;

[Hotfix]
public class AssetUpdateUIMsgTips : MonoBehaviour {

    public Text DesLabel;
    public Text TipsLabel;
    public GameObject SureBtn;
    public GameObject CancelBtn;
    public GameObject RewardGrid;
    //AssetUpdateUI mAssetUpdateUI;
    //DownloadAssetSizeCaculate mSizeData;
    System.Action SureCallBack;

    private 

    void Start()
    {
        uGUI.UIEventListener.Get(SureBtn).onPointerClick = OnSureBtnClick;
        uGUI.UIEventListener.Get(CancelBtn).onPointerClick = OnCancelBtnClick;
    }

    public void ShowMsg(DownloadAssetSizeCaculate sizeData, AssetUpdateUI assetUpdateUI,System.Action sureCallBack)
    {
        //mSizeData = sizeData;
        //this.mAssetUpdateUI = assetUpdateUI;
        if(sizeData.MaxSize >= 1000)
        {
            RewardGrid.SetActive(true);
            DesLabel.transform.localPosition = new Vector3(0,70,0);
            string msgTxt = string.Format("需要下载{0}M更新文件，更新可获得", sizeData.AllFileSize);
            ShowByText(msgTxt, sureCallBack);
        }
        else
        {
            RewardGrid.SetActive(false);
            DesLabel.transform.localPosition = new Vector3(0, 20, 0);
            string msgTxt = string.Format("需要下载{0}M更新文件，是否继续？", sizeData.AllFileSize);
            ShowByText(msgTxt, sureCallBack);
        }
        //this.SureCallBack = sureCallBack;
        //gameObject.SetActive(true);
        
        //DesLabel.SetText();
    }

    public void ShowByText(string msg, System.Action callBack)
    {
        DesLabel.text = msg;
        this.SureCallBack = callBack;
        gameObject.SetActive(true);
    }

    void OnSureBtnClick(GameObject obj)
    {
        gameObject.SetActive(false);
        //mAssetUpdateUI.mAssetUpdateMgr.ConfirmUpdate(mSizeData);
        if (SureCallBack != null)
            SureCallBack();
    }

    void OnCancelBtnClick(GameObject go)
    {
        gameObject.SetActive(false);
        Application.Quit();
    }

}

