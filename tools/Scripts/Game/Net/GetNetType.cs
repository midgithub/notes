using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Net.Sockets;

[Hotfix]
public class GetNetType
{
	public delegate void GetNetTypeDelegate(string serverIp,AddressFamily adf);
	GetNetTypeDelegate GetNetTypeCallBack;

	[DllImport("__Internal")]
	private static extern string _getIPv6(string mHost, string mPort);  
 
//"192.168.1.1&&ipv4"
  static string GetIPv6(string mHost, string mPort)
  {
#if UNITY_IPHONE && !UNITY_EDITOR
		string mIPv6 = _getIPv6(mHost, mPort);
		return mIPv6;
#else
    return mHost + "&&ipv4";
#endif
  }
	public static void GetNetTypeByIOS (string serverIp, string serverPorts, GetNetTypeDelegate callBack)
	{
		string newServerIP="";
		AddressFamily newAD = AddressFamily.InterNetwork;
		GetIOSNetType (serverIp, serverPorts, out newServerIP, out newAD);
		if (callBack != null)
			callBack (newServerIP, newAD);
	}	
  static void GetIOSNetType(string serverIp, string serverPorts,out String nServerIp, out AddressFamily nIPType)
  {
      String newServerIp = "";
      nServerIp = serverIp;
      AddressFamily newAddressFamily = AddressFamily.InterNetwork;
      getIPType(serverIp, serverPorts, out newServerIp, out newAddressFamily);
      if (!string.IsNullOrEmpty(newServerIp)) { nServerIp = newServerIp; }
      nIPType = newAddressFamily;
      //socketClient = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
      //Debug.Log("Socket AddressFamily :" + newAddressFamily.ToString() + "ServerIp:" + serverIp);
  }
  static void getIPType(String serverIp, String serverPorts, out String newServerIp, out AddressFamily mIPType)
    {
       mIPType = AddressFamily.InterNetwork;
       newServerIp = serverIp;
      try
      {
        string mIPv6 = GetIPv6(serverIp, serverPorts);
        if (!string.IsNullOrEmpty(mIPv6))
        {
          string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split(mIPv6, "&&");
          if (m_StrTemp != null && m_StrTemp.Length >= 2)
          {
            string IPType = m_StrTemp[1];
            if (IPType == "ipv6")
            {
              newServerIp = m_StrTemp[0];
              mIPType = AddressFamily.InterNetworkV6;
            }
          }
        }
      }
      catch (Exception e)
      {
        Debug.Log("GetIPv6 error:" + e);
      }
      
    }

}

