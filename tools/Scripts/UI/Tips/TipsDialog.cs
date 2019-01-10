/**
* @file     : TipsDialog.cs
* @brief    : TipsDialog
* @details  : TipsDialog
* @author   : CW
* @date     : 2017-06-26
*/
using XLua;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using SG;

[Hotfix]
public class TipsDialog : PanelBase 
{
    public Text mContextText;
    public Text mOKText;
    public Button mOKButton;
    public Text mCancelText;
    public Button mCancelButton;
    public Text mMidleText;
    public Button mMidleButton;

    private Action mOKAction;
    private Action mCancelAction;
    private Action mButtonAction;

    // Use this for initialization
    protected override void Start () 
    {
        mOKButton.onClick.AddListener(OnClickOK);
        mCancelButton.onClick.AddListener(OnClickCanel);
        mMidleButton.onClick.AddListener(OnClickMidle);
	}

    public void DoSet(string text, string okText, string cancelText, Action okAction, Action cancelAction)
    {
        mContextText.text = text;
        mOKText.text = okText;
        mCancelText.text = cancelText;

        mOKAction = okAction;
        mCancelAction = cancelAction;

        mOKButton.gameObject.SetActive(true);
        mCancelButton.gameObject.SetActive(true);
        mMidleButton.gameObject.SetActive(false);
    }

    public void DoSet(string text, string buttonText, Action buttonAction)
    {
        mContextText.text = text;
        mMidleText.text = buttonText;

        mButtonAction = buttonAction;

        mOKButton.gameObject.SetActive(false);
        mCancelButton.gameObject.SetActive(false);
        mMidleButton.gameObject.SetActive(true);
    }

    private void OnClickOK()
    {
        SDKMgr.Instance.TrackGameOptLog(3, 1);
        Hide();
        if (null == mOKAction)
            return;

        mOKAction();
    }

    private void OnClickCanel()
    {
        SDKMgr.Instance.TrackGameOptLog(3, 2);
        Hide();
        if (null == mCancelAction)
            return;

        mCancelAction();
    }

    private void OnClickMidle()
    {
        Hide();
        if (null == mButtonAction)
            return;

        mButtonAction();
    }

    public static void SetSimpleTipsDialog(string text, Action clickAction = null)
    {
        SetSimpleTipsDialog(text, "确定", clickAction);
    }

    public static void SetSimpleTipsDialog(string text, string buttonText, Action clickAction = null)
    {
        TipsDialog tipDialog = MainPanelMgr.Instance.ShowDialog("TipsDialog") as TipsDialog;
        if (null == tipDialog)
            return;

        tipDialog.DoSet(text, buttonText, clickAction);
    }

    public static void SetTipsDialog(string text, Action okAction = null, Action cancelAction = null)
    {
        SetTipsDialog(text, "确定", "取消", okAction, cancelAction);
    }

    public static void SetTipsDialog(string text, string okText, string cancelText, Action okAction = null, Action cancelAction = null)
    {
        TipsDialog tipDialog = MainPanelMgr.Instance.ShowDialog("TipsDialog") as TipsDialog;
        if (null == tipDialog)
            return;

        tipDialog.DoSet(text, okText, cancelText, okAction, cancelAction);
    }
}

