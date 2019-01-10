using XLua;
﻿using UnityEngine;
using System.Collections;


public class CommonUIButton : MonoBehaviour {
    public delegate void ButtonClickDelegate(object value);

    public UILabel mLabel;
    public UISprite mUiSprite;
    public Transform mUiSprite_2;

    object CallBackValue;
    ButtonClickDelegate ButtonClickCallBack;
    System.Action ButtonClickCallBackWithoutValue;
    public int mIndex = 0;

    void Awake()
    {
        UIEventListener.Get(this.gameObject).onClick = BtnClick;
    }
    void BtnClick(GameObject obj)
    {
        OnClick_();
    }
    public void SetClickCallBack(ButtonClickDelegate clickCallBack, object value)
    {
        this.ButtonClickCallBack = clickCallBack;
        CallBackValue = value;
    }

    public void SetClickCallBack(System.Action clickCallBack)
    {
        this.ButtonClickCallBackWithoutValue = clickCallBack;
    }

    void OnClick_()
    {
        if (ButtonClickCallBack != null)
        {
            ButtonClickCallBack(CallBackValue);
        }
        if (ButtonClickCallBackWithoutValue != null)
        {
            ButtonClickCallBackWithoutValue();
        }
    }

    public void SetLabel(string str)
    {
        mLabel.SetText(str);
    }

    public void SetSprite(string str)
    {
        mUiSprite.spriteName = str;
    }

}

