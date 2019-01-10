using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class PlatformListener : MonoBehaviour {

	private static PlatformListener m_instance = null;

	public static PlatformListener Instance()
	{
		return m_instance;
	}
	void Awake()
	{
		//为gameManager赋值，所有的其他操作都要放在后面
		if (null != m_instance)
		{
			Destroy(this.gameObject);
		}		
		m_instance = this;
		DontDestroyOnLoad(this.gameObject);
	}

    //void OnCYUserLogin(string param)
    //{

    //    //param = "{\"uid\":\"514001613706505\",\"token\":\"9f38e276d3d758b5\",\"deviceid\":\"10001\",\"userip\":\"10001\",\"type\":\"c\",\"appid\":\"101005256\"},4001,10001"
    //    //Debug.Log(param);
    //    string keyValidateInfo = "\"validateInfo\":";

    //    int validateInfoPos = param.IndexOf(keyValidateInfo) + keyValidateInfo.Length + 1;
    //    string strValidateInfo = param.Substring(validateInfoPos, param.IndexOf("\"", validateInfoPos) - validateInfoPos);
		
    //    int opCodePos = param.LastIndexOf(",") + 1;
    //    string opCode = param.Substring(opCodePos);
		
    //    int channelPos = param.LastIndexOf("},") + 2;
    //    string stringChannel = param.Substring(channelPos, opCodePos - 1 - channelPos);

    //    LoginData.accountData.SetCYData(strValidateInfo);
    //    MessageBoxLogic.OpenWaitBox(1003, 15, 0);
    //    NetManager.SendUserLogin(LoginData.Ret_Login);
    //}

	void OnCYPayResult(string param)
	{
		bool isPaySuccess = false;
		int nSuccess = 0;
		if(int.TryParse(param, out nSuccess))
		{
			isPaySuccess = nSuccess > 0;
		}

		Debug.Log("============unity pay result :" + isPaySuccess.ToString());
	}

    //void OnLoginTimeOut()
    //{
    //    NetWorkLogic.GetMe().DisconnectServer();
    //    MessageBoxLogic.OpenOKBox(1005, 1000);
    //}
}

