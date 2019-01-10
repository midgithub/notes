using XLua;
﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine ;
using System.Net.Sockets;
using System.Net ;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SG  
{
	// 实现公共算法
[Hotfix]
    public static class NetCommon
    {
		public static string GBK2UTF8(string input)
		{
            Encoding gbk = Encoding.GetEncoding("gb2312");
            Encoding utf8 = Encoding.UTF8;

            byte[] temp = utf8.GetBytes(input);
            LogMgr.UnityLog("temp:" + BitConverter.ToString(temp));
            string output = gbk.GetString(temp);
            return output;

            //return input;

			/*
			byte[] temp = gbk.GetBytes(input);
			LogMgr.UnityError("temp:"+BitConverter.ToString (temp));
			byte[] temp1 = Encoding.Convert(gbk,utf8,temp);
			LogMgr.UnityError("temp1:"+BitConverter.ToString (temp1));
			string output = gbk.GetString(temp1);

			return output;
			*/
		}
		public static string UTF82GBK(string input)
		{

            //Encoding gbk = Encoding.GetEncoding("gb2312");
            //Encoding utf8 = Encoding.UTF8;

            //byte[] temp = gbk.GetBytes(input);
            //LogMgr.UnityLog("temp:"+BitConverter.ToString (temp));
            //string output = utf8.GetString(temp);
            //return output;
            return input;

			/*
			byte[] temp = utf8.GetBytes(input);
			LogMgr.UnityError("temp:"+BitConverter.ToString (temp));
			byte[] temp1 = Encoding.Convert(utf8,gbk,temp);
			LogMgr.UnityError("temp1:"+BitConverter.ToString (temp1));
			string output = utf8.GetString(temp1);
			
			return output;
			*/
		}
		public static string GetMd5Hash(string input) 
		{ 
			MD5 md5Hash = MD5.Create();

			// Convert the input string to a byte array and compute the hash. 
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input)); 
			
			// Create a new Stringbuilder to collect the bytes 
			// and create a string. 
			StringBuilder sBuilder = new StringBuilder(); 
			
			// Loop through each byte of the hashed data  
			// and format each one as a hexadecimal string. 
			for (int i = 0; i < data.Length; i++) 
			{ 
				sBuilder.Append(data[i].ToString("x2")); 
			} 
			
			// Return the hexadecimal string. 
			return sBuilder.ToString(); 
		} 
		// 返回结构体的大小
		public static int Sizeof(Type type)
		{
			return Marshal.SizeOf(type);
		}
		//将Byte转换为结构体类型
		public static byte[] StructToBytes(object structObj,int size)
		{
			byte[] bytes = new byte[size];
			IntPtr structPtr = Marshal.AllocHGlobal(size);
			//将结构体拷到分配好的内存空间
			Marshal.StructureToPtr(structObj, structPtr, false);
			//从内存空间拷贝到byte 数组
			Marshal.Copy(structPtr, bytes, 0, size);
			//释放内存空间
			Marshal.FreeHGlobal(structPtr);
			return bytes;
			
		}
		//将Byte转换为结构体类型
		public static object ByteToStruct(byte[] bytes, Type type)
		{
			int size = Marshal.SizeOf(type);
			if (size > bytes.Length)
			{
				return null;
			}
			//分配结构体内存空间
			IntPtr structPtr = Marshal.AllocHGlobal(size);
			//将byte数组拷贝到分配好的内存空间
			Marshal.Copy(bytes, 0, structPtr, size);
			//将内存空间转换为目标结构体
			object obj = Marshal.PtrToStructure(structPtr, type);
			//释放内存空间
			Marshal.FreeHGlobal(structPtr);
			return obj;
		}
	}
}

