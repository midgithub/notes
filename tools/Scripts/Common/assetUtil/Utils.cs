using XLua;
﻿using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
 
namespace SG
{
[Hotfix]
    public class Utils
    {
       
        /// <summary>
        /// 按照规则严格进行分割
        /// </summary>
        /// <param name="str">原始字符</param>
        /// <param name="nTypeList">字符串类型</param>
        /// <param name="regix">规则词，只有一个</param>
        /// <returns>返回分割的词</returns>
        public static string[] MySplit(string str, string[] nTypeList, string regix)
        {

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            String[] content = new String[nTypeList.Length];
            int nIndex = 0;
            int nstartPos = 0;
            while (nstartPos <= str.Length)
            {
                int nsPos = str.IndexOf(regix, nstartPos);
                if (nsPos < 0)
                {
                    String lastdataString = str.Substring(nstartPos);
                    if (string.IsNullOrEmpty(lastdataString) && nTypeList[nIndex].ToLower() != "string")
                    {
                        content[nIndex++] = "--";
                    }
                    else
                    {
                        content[nIndex++] = lastdataString;
                    }
                    break;
                }
                else
                {
                    if (nstartPos == nsPos)
                    {
                        if (nTypeList[nIndex].ToLower() != "string")
                        {
                            content[nIndex++] = "--";
                        }
                        else
                        {
                            content[nIndex++] = "";
                        }
                    }
                    else
                    {
                        content[nIndex++] = str.Substring(nstartPos, nsPos - nstartPos);
                    }
                    nstartPos = nsPos + 1;
                }
            }

            return content;

        }

        // 复制一个obj并绑定到父节点
        public static GameObject BindObjToParent(GameObject resObject, GameObject parentObject, string name = null)
        {
            GameObject newObj = GameObject.Instantiate(resObject) as GameObject;
            newObj.transform.parent = parentObject.transform;
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
            if (null != name)
            {
                newObj.name = name;
            }
            return newObj;
        }

        public static Color GetColorByString(string strColor)
        {
            int r = 0;
            int g = 0;
            int b = 0;
            if (strColor.Length == 6)
            {
                string strR = strColor[0].ToString() + strColor[1].ToString();
                string strG = strColor[2].ToString() + strColor[3].ToString();
                string strB = strColor[4].ToString() + strColor[5].ToString();
                r = Convert.ToInt32(strR, 16);
                g = Convert.ToInt32(strG, 16);
                b = Convert.ToInt32(strB, 16);
            }
            return new Color((float)r / 255, (float)g / 255, (float)b / 255);
        }
        
     
     
        // 将时间间隔格式化对应标签上
        public static void SetTimeDiffToLabel(UILabel label, int timeDiff)
        {
            if (timeDiff <= 0)
            {
                label.text = "00:00:00";
            }
            else
            {
                label.text = string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", timeDiff / 3600, (timeDiff % 3600) / 60, timeDiff % 60);
            }
        }

        public static string GetTimeDiffFormatString(int timeDiff)
        {
            if (timeDiff <= 0)
            {
                return "00:00:00";
            }
            else
            {
                return string.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", timeDiff / 3600, (timeDiff % 3600) / 60, timeDiff % 60);
            }
        }

        // 清除GRID所有子ITEM
        public static void CleanGrid(GameObject grid)
        {
            for (int i = 0, count = grid.transform.childCount; i < count; i++)
            {
                GameObject.Destroy(grid.transform.GetChild(i).gameObject);
            }

            grid.transform.DetachChildren();
        }

      

        //public static GameObject LoadUIItem(GameObject parent, string name, UIPathData uiData)
        //{
        //    GameObject resObj = ResourceManager.LoadResource(uiData.path) as GameObject;
        //    GameObject curItem = Utils.BindObjToParent(resObj, parent);
        //    curItem.name = name;
        //    return curItem;
        //}
 

        public static Quaternion DirServerToClient(float rad)
        {
            return Quaternion.Euler(0, 90.0f - rad * 180.0f / Mathf.PI, 0);
        }

        public static float DirClientToServer(Quaternion rotate)
        {
            return Mathf.PI * 0.5f - rotate.eulerAngles.y * Mathf.PI / 180.0f;
        }

        //转化到0-2PI范围内
        public static float NormaliseDirection(float fDirection)
        {
            float _2PI = (float)(Math.PI * 2);
            float fRetValue = fDirection;

            if (fRetValue >= _2PI)
            {
                fRetValue -= ((float)((int)(fDirection / _2PI)) * _2PI);
                fRetValue = (fRetValue > 0.0F) ? fRetValue : 0.0f;
                fRetValue = (fRetValue < _2PI) ? fRetValue : _2PI;
            }
            else if (fRetValue < 0)
            {
                fRetValue += ((float)((int)(-fDirection / _2PI) + 1) * _2PI);
                fRetValue = (fRetValue > 0.0F) ? fRetValue : 0.0f;
                fRetValue = (fRetValue < _2PI) ? fRetValue : _2PI;
            }
            return fRetValue;
        }

        //获取当前平台StreamingAsset路径
        public static string GetStreamingAssetPath()
        {
            string strStreamingPath = "";
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                strStreamingPath = Application.dataPath + "/Raw";
            }
            else
            {
                strStreamingPath = Application.streamingAssetsPath;
            }

            return strStreamingPath;
        }
 

		public static void CheckTargetPath(string targetPath)
		{
            targetPath = targetPath.Replace('\\', '/');

            int dotPos = targetPath.LastIndexOf('.');
            int lastPathPos = targetPath.LastIndexOf('/');

            if (dotPos > 0 && lastPathPos < dotPos)
            {
                targetPath = targetPath.Substring(0, lastPathPos);
            }
			if(Directory.Exists(targetPath))
			{
				return;
			}

			
			string[] subPath = targetPath.Split('/');
			string curCheckPath = "";
			int subContentSize = subPath.Length;
			for(int i=0; i<subContentSize; i++)
			{
				curCheckPath += subPath[i] + '/';
				if(!Directory.Exists(curCheckPath))
				{
					Directory.CreateDirectory(curCheckPath);
				}
			}
		}

        public static void DeleteFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] strTemp;
            //先删除该目录下的文件
            strTemp = System.IO.Directory.GetFiles(path);
            foreach (string str in strTemp)
            {
                System.IO.File.Delete(str);
            }
            //删除子目录，递归
            strTemp = System.IO.Directory.GetDirectories(path);
            foreach (string str in strTemp)
            {
                DeleteFolder(str);
            }

            System.IO.Directory.Delete(path);
        }

        // 拷贝一个路径下所有的文件，不包含子目录
        public static void CopyPathFile(string srcPath, string distPath)
        {
            if (!Directory.Exists(srcPath))
            {
                return;
            }

            Utils.CheckTargetPath(distPath);

            string[] strLocalFile = System.IO.Directory.GetFiles(srcPath);
            foreach (string curFile in strLocalFile)
            {
                System.IO.File.Copy(curFile, distPath + "/" + Path.GetFileName(curFile), true);
            }
        }

        // 获取MD5
        public static string GetMD5Hash(string pathName)
        {

            string strResult = "";
            string strHashData = "";
            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;
            System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            try
            {
                oFileStream = new System.IO.FileStream(pathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);
                oFileStream.Close();
                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                LogMgr.UnityLog("read md5 file error :" + pathName + " e: " + ex.ToString());
            }

            return strResult;

        }

        public static DateTime UnixTimestamp2LocalTime(long timestamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dtDateTime.AddSeconds(timestamp).ToLocalTime();
        }

        public static long LocalTime2UnixTimestamp(DateTime dateTime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.Subtract(sTime)).TotalSeconds;
        }

        /// <summary>
        /// 将字节数组拷贝到另一个数组中。
        /// </summary>
        /// <param name="src">源数组。</param>
        /// <param name="srcindex">数据源起始索引。</param>
        /// <param name="dst">目标数组。</param>
        /// <param name="dstindex">目标数组起始索引。</param>
        /// <param name="len">拷贝的数据长度。</param>
        public static void CopyTo(byte[] src, int srcindex, byte[] dst, int dstindex, int len)
        {
            for (int i = 0; i < len; ++i)
            {
                dst[dstindex + i] = src[srcindex + i];
            }
        }

        /// <summary>
        /// 写入一个无符号整数。
        /// </summary>
        /// <param name="src">要写入的字节数组。</param>
        /// <param name="index">要写入的起始位置。</param>
        /// <param name="value">要写入的值。</param>
        public static void WriteUInt64(byte[] src, int index, ulong value)
        {
            src[index + 7] = (byte)((value >> 56) & 0xFF);
            src[index + 6] = (byte)((value >> 48) & 0xFF);
            src[index + 5] = (byte)((value >> 40) & 0xFF);
            src[index + 4] = (byte)((value >> 32) & 0xFF);
            src[index + 3] = (byte)((value >> 24) & 0xFF);
            src[index + 2] = (byte)((value >> 16) & 0xFF);
            src[index + 1] = (byte)((value >> 8) & 0xFF);
            src[index + 0] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// 写入一个无符号整数。
        /// </summary>
        /// <param name="src">要写入的字节数组。</param>
        /// <param name="index">要写入的起始位置。</param>
        /// <param name="value">要写入的值。</param>
        public static void WriteUInt32(byte[] src, int index, uint value)
        {
            src[index + 3] = (byte)((value >> 24) & 0xFF);                  //最高位
            src[index + 2] = (byte)((value >> 16) & 0xFF);                  //次高位            
            src[index + 1] = (byte)((value >> 8) & 0xFF);                   //次低位
            src[index + 0] = (byte)(value & 0xFF);                          //最低位
        }

        /// <summary>
        /// 写入一个无符号整数。
        /// </summary>
        /// <param name="src">要写入的字节数组。</param>
        /// <param name="index">要写入的起始位置。</param>
        /// <param name="value">要写入的值。</param>
        public static void WriteUInt16(byte[] src, int index, ushort value)
        {   
            src[index + 1] = (byte)((value >> 8) & 0xFF);                   //次低位
            src[index + 0] = (byte)(value & 0xFF);                          //最低位
        }

        /// <summary>
        /// 写入一个浮点数。
        /// </summary>
        /// <param name="src">要写入的字节数组。</param>
        /// <param name="index">要写入的起始位置。</param>
        /// <param name="value">要写入的值。</param>
        public static void WriteFloat(byte[] src, int index, float value)
        {
            BitConverter.GetBytes(value).CopyTo(src, index);
        }

        /// <summary>
        /// 写入一个浮点数。
        /// </summary>
        /// <param name="src">要写入的字节数组。</param>
        /// <param name="index">要写入的起始位置。</param>
        /// <param name="value">要写入的值。</param>
        public static void WriteDouble(byte[] src, int index, double value)
        {
            BitConverter.GetBytes(value).CopyTo(src, index);
        }     
    }
}

