using XLua;
﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine ;
using System.Net.Sockets;
using System.Net ;
using System.Threading;

namespace SG  
{
	// 网络模块上行数据加密模块,下行数据不编码.
[Hotfix]
    public class NetEncrypt
    {
		private int m_nSelfSalt;
		private int m_nTargetSalt;
		private int m_nKey;
		private byte[] m_sKeybuff;

		public NetEncrypt()
		{
			m_nSelfSalt = 0;
			m_nTargetSalt = 0;
			m_nKey = 0;
			m_sKeybuff = new byte[4];
		}
   		public void setSelfSalt(int salt)
		{
			m_nSelfSalt = salt;
		}
		public int getSelfSalt()
		{
			if(m_nSelfSalt == 0)
			{
				System.Random a = new System.Random(System.DateTime.Now.Millisecond);
				do{
					m_nSelfSalt = a.Next();
				}while(m_nSelfSalt == 0);
			}
			return m_nSelfSalt;
		}
		public void setTargetSalt(int salt)
		{
			m_nTargetSalt = salt;
			genKey();
		}
		public int getKey()
		{
			return m_nKey;
		}
		private void genKey()
		{
			LogMgr.UnityLog("genKey:" + m_nSelfSalt + " " + m_nTargetSalt);
			if(m_nSelfSalt != 0 && m_nTargetSalt != 0)
			{
				m_nKey = (m_nSelfSalt ^ m_nTargetSalt) + 3397;
				//BitConverter.GetBytes(m_nKey).CopyTo(m_sKeybuff,0);
                Utils.WriteUInt32(m_sKeybuff, 0, (uint)m_nKey);
				LogMgr.UnityLog("m_sKeybuff:" + BitConverter.ToString(m_sKeybuff));
			}
		}
		public ushort getKeyCRC()
		{
			return (ushort)(0XFFFF & CRC.CRC16Calc(m_sKeybuff,4));
		}
		public bool encode(byte[] pInBuff, int len, byte[] pOutBuff)
		{
			if(m_nSelfSalt != 0)
			{
				for(int i=0;i<len;i++)
				{
					pOutBuff[i] = (byte)(pInBuff[i] ^ m_sKeybuff[i&3]);
				}
				return true;
			}else
			{
				return false;
			}
		}
	}
}

