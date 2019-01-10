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
	// 实现自定义的CRC算法.
[Hotfix]
    public static class CRC
    {
		private static ushort CRC16_POLYNOMIAL = 0x1021;	// crc_16校验方式的多项式.
		private static uint[] g_ulTable = new uint[256];
		private static bool has_inited = false;

		/*
		public static CRC()
		{
			g_ulTable = new uint[256];
			has_inited = false;
		}
		*/
		public static void CRC16Init()
		{
			uint nRemainder;
			int n,m;
			if( has_inited == true) return;
			for (n=0;n<256;n++)
			{
				nRemainder = (uint)n << 8;
				for(m=8;m>0;m--)
				{
					if((nRemainder & 0x8000) != 0)
					{
						nRemainder = (nRemainder << 1) ^ CRC16_POLYNOMIAL;
					}else
					{
						nRemainder = (nRemainder << 1);
					}
				}
				g_ulTable[n] = nRemainder;
			}
			has_inited = true;
		}
		public static uint CRCBitReflect(uint ulData,int nBits)
		{
			uint ulResult = 0;
			int n;
			for(n=0;n<nBits;n++)
			{
				if((ulData & 0x00000001)!=0)
				{
					ulResult |= (uint)(1 << ((nBits -1) -n));
				}
				ulData = (ulData >> 1);
			}
			return ulResult;
		}
		public static uint CRC16Calc(byte[] pucData, uint nBytes)
		{
			uint nRemainder = 0x0000;
			uint nRet;
			byte index;
			uint n;
			if(has_inited == false)
			{
				CRC16Init();
			}
			for(n=0;n<nBytes;n++)
			{
				uint indextemp = CRCBitReflect(pucData[n],8) ^ (nRemainder >> 8);
				index = (byte)(indextemp & 0xFF);
				nRemainder = (uint)g_ulTable[index] ^ (nRemainder << 8);
				//Debug.LogError("CRC16Calc nRemainder:" + nRemainder);
			}
			nRet = (uint)CRCBitReflect(nRemainder, 16) ^ 0x0000;

			//Debug.LogError("CRC16Calc nRet:" + nRet);

			return nRet;
		}

	}
}

