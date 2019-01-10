/**
* @file     : UITips
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-07-31
*/
using UnityEngine;
using System;
using System.Collections;
using XLua;
using SG;

[CSharpCallLua]
public delegate void ShowTips(string msg);
[CSharpCallLua]
public delegate void ShowHeroModelTips();
[CSharpCallLua]
public delegate void SetTipDialog(string title, string desc, Action okFunc, Action cancelFunc, string okText, string canelText);
[CSharpCallLua]
public delegate void SetTipSingDialog(string title, string desc, Action okFunc);

[CSharpCallLua]
public delegate void SetSliderCollect(string title, int desc, Action callback);
[CSharpCallLua]
public delegate void SwitchLine();

[Hotfix]
public class UITips
{
    public static void ShowTips(string msg)
    {
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        ShowTips fun = G.GetInPath<ShowTips>("Common.ShowTips");
        if (fun != null)
        {
            fun(msg);
        }
    }

    public static void ShowUnHeroModelShow()
    {
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        ShowHeroModelTips fun = G.GetInPath<ShowHeroModelTips>("Common.TipHeroModelShow");
        if (fun != null)
        {
            fun();
        }
    }


    public static void ShowNotice(string msg)
    {
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        ShowTips fun = G.GetInPath<ShowTips>("Common.ShowNotice");
        if (fun != null)
        {
            fun(msg);
        }
    }

    public static void ShowTipsDialog(string title, string desc, string okText, Action okFunc, string cancelText, Action cancelFunc)
    {
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        SetTipDialog fun = G.GetInPath<SetTipDialog>("Common.ShowTipDlg");
        if (fun != null)
        {
            fun(title, desc, okFunc, cancelFunc, okText, cancelText);
        }
    }
    public static void ShowTipSinginDlg(string title, string desc, Action okFunc)
    {
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        SetTipSingDialog fun = G.GetInPath<SetTipSingDialog>("Common.ShowTipSinginDlg");
        if (fun != null)
        {
            fun(title, desc, okFunc);
        }
    }

    public static void ShowSliderProgress(string title, int time, Action callback = null)
    {
        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        SetSliderCollect fun = G.GetInPath<SetSliderCollect>("Common.ShowSliderProgress");
        if (fun != null)
        {
            fun(title, time, callback);
        }
    }
    public static void ReconnectSwitchLine()
    {
        if (MapMgr.Instance.CurMapType == MapMgr.MapType.Map_None || MapMgr.Instance.CurMapType == MapMgr.MapType.Map_Login
            || MapMgr.Instance.CurMapType == MapMgr.MapType.Map_SelectRole)
        {
            return;
        }

        LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
        SwitchLine fun = G.GetInPath<SwitchLine>("Common.ReconnectSwitchLine");
        if (fun != null)
        {
            fun();
        }
    }
}

