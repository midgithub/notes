using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using SG;

/*
[Hotfix]
public class PlatformHelper
{
	// 游戏大版本
	[DllImport("__Internal")]
	private static extern int _getGameVersion ();
	public static int GetGameVersion()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) 
			return _getGameVersion ();
		else
			return (int)VERSION.GameVersion;
	}

	// 程序版本
	[DllImport("__Internal")]
	private static extern int _getProgramVersion ();
	public static int GetProgramVersion()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) 
			return _getProgramVersion ();
		else
			return (int)VERSION.ProgramVersion;
	}

    // 是否可以使用GM指令
    [DllImport("__Internal")]
    private static extern bool _isEnableGM();
    public static bool IsEnableGM()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            return _isEnableGM();
        else
            return true;
        
    }

    // 是否可以自动锁屏
    [DllImport("__Internal")]
    private static extern void _setScreenCanAutoLock(bool bCanLock);
    public static void SetScreenCanAutoLock(bool bCanLock)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            _setScreenCanAutoLock(bCanLock);
    }


	// 资源更新地址
	[DllImport("__Internal")]
	private static extern string _getResDonwloadUrl();
	public static string GetResDonwloadUrl()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return _getResDonwloadUrl();
		return "";
	}

	// 是否开启资源更新
	[DllImport("__Internal")]
	private static extern bool _isEnableUpdate();
	public static bool IsEnableUpdate()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			return _isEnableUpdate();
		return false;
	}

	// 用户登录
	[DllImport("__Internal")]
	private static extern void _userLogin();
	public static void UserLogin()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_userLogin();
	}

    // 用户登出
	[DllImport("__Internal")]
	private static extern void _userLogout();
	public static void UserLogout()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_userLogout();
	}

	public enum ChannelType
	{
		NONE,
        IOS_UNKNOWN,
		IOS_TEST,
        IOS_TY,
        IOS_APPSTORE,
        TEST,
	}

    // 获取版本类型枚举
	[DllImport("__Internal")]
	private static extern string _getChannelString();
	public static ChannelType GetChannelType()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string strChannel = _getChannelString();
			if(strChannel == "IOSTest") 
			{
				return ChannelType.IOS_TEST;
			}
			else if(strChannel == "AppStore")
			{
                return ChannelType.IOS_APPSTORE;
			}
            else if (strChannel == "TY")
            {
                return ChannelType.IOS_TY;
            }

            return ChannelType.IOS_UNKNOWN;
		}

        return ChannelType.TEST;
	}

    // 获取服务器列表地址
	[DllImport("__Internal")]
	private static extern string _getServerlistUrl();
    public static string GetServerListUrl()
    {

		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(_getServerlistUrl() == "0")
			{
				LogMgr.UnityLog("server list url is 0, user local ipList");
				return "file://" + Application.streamingAssetsPath + "/IPList.txt";
			}
			return _getServerlistUrl();
		}

        //return "http://mrd.changyou.com/tianlong3D/IPList.txt";
        return "file://" + Application.streamingAssetsPath + "/IPList.txt";
    }

    // 统计日志：角色进入游戏
	[DllImport("__Internal")]
	private static extern void _roleEnterGame(string strAccountID,string strRoleType, string strRoleName, int RoleLevel);
    public static void RoleEnterGame(string strAccountID, string strRoleType, string strRoleName, int RoleLevel)
	{
		
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
            _roleEnterGame(strAccountID,strRoleType, strRoleName, RoleLevel);
		}

	}

    // 统计日志：角色进入游戏
    [DllImport("__Internal")]
    private static extern void _onAccountLogin(string strServerID, string strUserID);
    public static void OnAccountLogin(string strServerID, string strUserID)
    {

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _onAccountLogin(strServerID, strUserID);
        }

    }

    // 支付
	[DllImport("__Internal")]
	private static extern void _makePay(string strRoleID, string groupID);
	public static void MakePay(string strRoleID, string groupID)
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_makePay(strRoleID, groupID);
		}
	}

    // 获取包体更新地址
	[DllImport("__Internal")]
	private static extern string _getUpdateAppUrl();
    public static string GetUpdateAppUrl()
    {
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _getUpdateAppUrl();
		}
        return "";
    }
	
    // 获取设备唯一ID
	[DllImport("__Internal")]
	private static extern string _getDeviceUDID();
	public static string GetDeviceUDID()
	{
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _getDeviceUDID();
		}

		return "";
	}

    // 是否开启调试模式：左上角调试框，FPS
    [DllImport("__Internal")]
    private static extern bool _IsEnableDebugMode();
    public static bool IsEnableDebugMode()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return _IsEnableDebugMode();
        }
        return true;
    }

	[DllImport("__Internal")]
	private static extern string _getMediaChannel();
	public static string GetMediaChannel()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _getMediaChannel();
		}
		return "";
	}
	

}
//*/

