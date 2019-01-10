using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    class UiUtil
    {
        //删除所有的子控件 
        static public void ClearAllChild(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 清除子节点。
        /// </summary>
        /// <param name="t">父节点。</param>
        public static void ClearAllChildImmediate(Transform t)
        {
            while (t.childCount > 0)
            {
                UnityEngine.Object.DestroyImmediate(t.GetChild(0).gameObject);
            }
        }

        /// <summary>
        /// 获取网络字符串，要去掉最后一个0字节。
        /// </summary>
        /// <param name="data">字符串列表。</param>
        /// <returns>字符串。</returns>
        public static string GetNetString(List<byte> data)
        {
            byte[] bytes = data.ToArray();
            return GetNetString(bytes);
        }

        public static string GetNetString(byte[] bytes)
        {
            int len = bytes.Length;
            for (int i=0; i<bytes.Length; ++i)
            {
                if (bytes[i] == 0)
                {
                    len = i;
                    break;
                }
            }
            return len <= 0 ? string.Empty : Encoding.UTF8.GetString(bytes, 0, len);
        }

        public static string GetNetString(sbyte[] cbytes)
        {
            byte[] bytes = new byte[cbytes.Length];
            int len = cbytes.Length;
            for (int i = 0; i < cbytes.Length; ++i)
            {
                bytes[i] = (byte)cbytes[i];
                if (cbytes[i] == 0)
                {
                    len = i;
                    break;
                }
            }
            return len <= 0 ? string.Empty : Encoding.UTF8.GetString(bytes, 0, len);
        }

        public static string GetTimeString(int sec, bool showhour = false)
        {
            sec = Math.Max(sec, 0);
            int h = sec / 3600;
            int m = (showhour ? (sec % 3600) : sec) / 60;       //如果显示小时就要进行3600取余
            int s = sec % 60;
            string str = showhour ? string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s) : string.Format("{0:D2}:{1:D2}", m, s);
            return str;
        }

        /// <summary>
        /// 获取当前时间字符串。
        /// </summary>
        /// <returns>当前时间字符串。</returns>
        public static string GetCurTimeString()
        {
            DateTime now = DateTime.Now;
            return string.Format("{0}:{1:D2}", now.Hour, now.Minute);
        }

        /// <summary>
        /// 品质颜色
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static string GetQualitycolor(int quality)
        {
            string rgbStr = "ffffff";
            switch (quality)
            {
                case 0:
                    rgbStr = "fff4bf";
                    break;
                case 1:
                    rgbStr = "1fd7ff";
                    break;
                case 2:
                    rgbStr = "ff4ef5";
                    break;
                case 3:
                    rgbStr = "ff7109";
                    break;
                case 4:
                    rgbStr = "ff0000";
                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
                default:
                    rgbStr = "ffffff";
                    break;
            }
            return rgbStr;
        }

        /// <summary>
        /// 设置图片变灰。
        /// </summary>
        /// <param name="img">图像对象。</param>
        /// <param name="grey">是否变灰。</param>
        public static void SetImageGrey(Image img, bool grey)
        {
            img.material = grey ? AtlasSpriteManager.Instance.UIGrey : null;
        }

        /// <summary>
        /// 设置图片变灰。
        /// </summary>
        /// <param name="img">图像对象。</param>
        /// <param name="grey">是否变灰。</param>
        public static void SetRawImageGrey(RawImage img, bool grey)
        {
            img.material = grey ? AtlasSpriteManager.Instance.UIGrey : null;
        }

        public static int GetCharLength(string str)
        {
            if (str == null)
                return 0;
            return str.Length; 
        }

        public static void SetParentGrey(Transform trs, bool grey)
        {
            Image[] imgs = trs.GetComponentsInChildren<Image>();
            Text[] txts = trs.GetComponentsInChildren<Text>();

            if(imgs != null)
            {
                foreach(var v in imgs)
                {
                    v.material = grey ? AtlasSpriteManager.Instance.UIGrey : null;
                }
            }
            if (txts != null)
            {
                foreach (var v in txts)
                {
                    v.material = grey ? AtlasSpriteManager.Instance.UIGrey : null;
                }
            } 
        }


        public static int GetTextCurPrefWidht(Text text)
        {
            if (text == null)
                return 0; 
            return (int)text.preferredWidth;
        }

        public static int GetTextCurPrefHeight(Text text)
        {
            if (text == null)
                return 0;
            return (int)text.preferredHeight;
        }

        /// <summary>
        /// 创建子成员
        /// </summary>
        /// <param name="itemPrefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject CreatItem(GameObject itemPrefab,GameObject parent)
        {
            GameObject p = null;
            p = UnityEngine.Object.Instantiate(itemPrefab);
            if(parent != null)
            {
                p.transform.SetParent(parent.transform);
            }
            p.transform.localPosition = Vector3.zero;
            p.transform.localScale = Vector3.one;
            return p;
        }

        private static DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>毫秒级时间。</returns>  
        public static long ConvertDateTimeToUnix(DateTime time)
        {
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        /// <summary>
        /// 客户端与服务器的时间差。客户端时间加上此值等于服务器时间。
        /// </summary>
        public static long CSTimeOffset = 0;

        /// <summary>
        /// 获取当前的时间戳。
        /// </summary>
        /// <returns>秒级时间。</returns>
        public static long GetNowTimeStamp()
        {
            return ConvertDateTimeToUnix(DateTime.Now)/1000 + CSTimeOffset;
        }

        /// <summary>
        /// 获取事件描述文本。显示详细时间
        /// </summary>
        /// <param name="stamp">时间戳。</param>
        /// <returns>y.m.d h:m:s格式的字符串。</returns>
        public static string GetDateTimeString(long stamp)
        {
            DateTime dt = startTime.AddSeconds(stamp);
            return string.Format("{0}.{1:D2}.{2:D2} {3:D2}:{4:D2}:{5:D2}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        /// <summary>
        /// 显示年月日
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public static string GetDateTimeStrigYMD(long stamp)
        {
            DateTime dt = startTime.AddSeconds(stamp);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", dt.Year, dt.Month, dt.Day);
        }
        /// <summary>
        /// 只显示当天时间
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public static string GetDateTimeStringSimple(long stamp)
        {
            DateTime dt = startTime.AddSeconds(stamp);
            return string.Format("{0:D2}:{1:D2}:{2:D2}", dt.Hour, dt.Minute, dt.Second);
        }

        /// <summary>
        /// 初始化类。
        /// </summary>
        static UiUtil()
        {
            HexToDec.Add('0', 0);
            HexToDec.Add('1', 1);
            HexToDec.Add('2', 2);
            HexToDec.Add('3', 3);
            HexToDec.Add('4', 4);
            HexToDec.Add('5', 5);
            HexToDec.Add('6', 6);
            HexToDec.Add('7', 7);
            HexToDec.Add('8', 8);
            HexToDec.Add('9', 9);
            HexToDec.Add('a', 10);
            HexToDec.Add('b', 11);
            HexToDec.Add('c', 12);
            HexToDec.Add('d', 13);
            HexToDec.Add('e', 14);
            HexToDec.Add('f', 15);
            HexToDec.Add('A', 10);
            HexToDec.Add('B', 11);
            HexToDec.Add('C', 12);
            HexToDec.Add('D', 13);
            HexToDec.Add('E', 14);
            HexToDec.Add('F', 15);
        }

        /// <summary>
        /// 十六进制字符到数字的映射。
        /// </summary>
        private static Dictionary<char, int> HexToDec = new Dictionary<char, int>();

        /// <summary>
        /// 数字到十六进制字符的映射。
        /// </summary>
        private static char[] DecToHex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// 将字符串转换为颜色值。
        /// </summary>
        /// <param name="str">转换的十六进制字符串，只能是6RGB位或者8位ARGB。</param>
        /// <param name="def">默认颜色值。</param>
        /// <returns>颜色值。</returns>
        public static Color ToColor(string str, Color def)
        {
            int[] values = new int[8];
            if (str.Length != 6 && str.Length != 8)
            {
                return def;
            }

            //默认A通道是FF
            values[6] = 15;
            values[7] = 15;
            for (int i = 0; i < str.Length; ++i)
            {
                int v;
                if (!HexToDec.TryGetValue(str[i], out v))
                {
                    return def;
                }
                values[i] = v;
            }

            //生成颜色
            int r = values[0] * 16 + values[1];
            int g = values[2] * 16 + values[3];
            int b = values[4] * 16 + values[5];
            int a = values[6] * 16 + values[7];
            Color c = new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
            return c;
        }

        /// <summary>
        /// 将颜色值转换为RGBA格式的字符串。
        /// </summary>
        /// <param name="c">颜色值。</param>
        /// <returns>大写RGBA格式字符串。</returns>
        public static string ToHexString(Color c)
        {
            int r = (int)(c.r * 255);
            int g = (int)(c.g * 255);
            int b = (int)(c.b * 255);
            int a = (int)(c.a * 255);
            char[] chars = new char[8];
            chars[0] = DecToHex[r / 16];
            chars[1] = DecToHex[r % 16];
            chars[2] = DecToHex[g / 16];
            chars[3] = DecToHex[g % 16];
            chars[4] = DecToHex[b / 16];
            chars[5] = DecToHex[b % 16];
            chars[6] = DecToHex[a / 16];
            chars[7] = DecToHex[a % 16];
            string str = new string(chars);
            return str;
        }

        /// <summary>
        /// 设置文本颜色。
        /// </summary>
        /// <param name="txt">文本对象。</param>
        public static void SetTextColor(Text txt, float r, float g, float b, float a = 1)
        {
            txt.color = new Color(r, g, b, a);
        }

        /// <summary>
        /// 设置图像颜色。
        /// </summary>
        /// <param name="txt">图像对象。</param>
        public static void SetImageColor(Image img, float r, float g, float b, float a = 1)
        {
            img.color = new Color(r, g, b, a);
        }

        /// <summary>
        /// 设置控件位置。
        /// </summary>
        /// <param name="rt">控件对象。</param>
        public static void SetAnchoredPosition(RectTransform rt, float x, float y)
        {
            rt.anchoredPosition = new Vector2(x, y);
        }

        /// <summary>
        /// 设置控件旋转。
        /// </summary>
        /// <param name="rt">控件对象。</param>
        /// <param name="x">X旋转量。</param>
        /// <param name="y">Y旋转量。</param>
        /// <param name="z">Z旋转量。</param>
        public static void SetRotate(RectTransform rt, float x, float y, float z)
        {
            rt.localRotation = Quaternion.Euler(x, y, z);
        }

        /// <summary>
        /// 获取控件高度。(考虑尺寸适配)
        /// </summary>
        /// <param name="rt">控件对象。</param>
        /// <returns>控件高度。</returns>
        public static float GetHeight(RectTransform rt)
        {
            float h = 0;
            float top = 0, bottom = 0;
            while (rt != null)
            {
                if (rt.anchorMax.y > rt.anchorMin.y)
                {
                    top += rt.offsetMax.y;
                    bottom += rt.offsetMin.y;
                }
                else
                {
                    Canvas c = rt.GetComponent<Canvas>();
                    if (c != null)
                    {
                        //画布尺寸
                        h = c.pixelRect.height / c.scaleFactor;
                    }
                    else
                    {
                        h = rt.rect.height;
                    }                    
                    break;
                }
                rt = rt.parent as RectTransform;
            }

            float ret = h - top - bottom;
            return ret;
        }

        /// <summary>
        /// 获取字符串长度，Lua层用。
        /// </summary>
        /// <param name="str">字符串内容。</param>
        /// <returns>字符串长度。</returns>
        public static int GetStringLength(string str)
        {
            return str.Length;
        }

        /// <summary>
        /// 获取字符串宽度。
        /// </summary>
        /// <param name="str">要获取宽度的字符串。</param>
        /// <returns>符串宽度。(英文字符串算1个，中文字符算两个)</returns>
        public static int GetStringWidth(string str)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);
            int len = data.Length;
            return len;
        }

        /// <summary>
        /// 获取text组件的高度
        /// </summary>
        /// <param name="tt"></param>
        /// <returns></returns>
        public static float GetTextHeight(Text tt)
        {
            if(tt == null)
            {
                return 0;
            }
            float s = tt.rectTransform.localScale.y;
            return tt.preferredHeight * s;
        }

        public static List<int> SplitNumber(string str, string sp)
        {
            List<int> ret = new List<int>();
            string[] data = str.Split(sp.ToCharArray());
            for (int i=0; i< data.Length; ++i)
            {
                int n;
                if (int.TryParse(data[i], out n))
                {
                    ret.Add(n);
                }
            }
            
            return ret;
        }

        /// <summary>
        /// 获取摄像机旋转角度。
        /// </summary>
        /// <returns>摄像机旋转角度。</returns>
        public static float GetCameraRotate()
        {
            if (CoreEntry.gCameraMgr.MainCamera == null)
            {
                return 0;
            }
            CameraFollow cf = CoreEntry.gCameraMgr.MainCamera.GetComponent<CameraFollow>();
            if (cf == null)
            {
                return 0;
            }
            return cf.m_rotationAngle;
        }

        public static Color IntToColor(int val)
        {
            float inv = 1f / 255f;
            Color c = Color.black;
            c.r = inv * ((val >> 24) & 0xFF);
            c.g = inv * ((val >> 16) & 0xFF);
            c.b = inv * ((val >> 8) & 0xFF);
            c.a = inv * (val & 0xFF);
            return c;
        }

        /// <summary>
        /// 数字转字符串，Lua的数字转字符串会保留小数点后面的0，C#的不会。
        /// </summary>
        /// <param name="num">数字。</param>
        /// <param name="w">固定最小宽度。</param>
        /// <returns>字符串。</returns>
        public static string NumberToString(double num, int w=0)
        {
            string str = num.ToString();
            int len = w - str.Length;
            if (len > 0)
            {
                str = new string(' ', len) + str;
            }
            return str;
        }
        
        /// <summary>
        /// 对齐数字。
        /// </summary>
        /// <param name="num">要对其的数值。</param>
        /// <param name="align">要整除的数。</param>
        /// <param name="min">最小值，为0则忽略。</param>
        /// <returns>对齐后的数字。</returns>
        public static int AlignNumber(int num, int align, int min = 0)
        {
            int y = num % align;
            if (y > 0)
            {
                num += align - y;
            }
            if (min > 0)
            {
                num = Math.Max(num, min);
            }
            return num;
        }

        /// <summary>
        /// 将字符串普通空格替换为不换行空格。
        /// </summary>
        /// <param name="str">要替换的字符串。</param>
        /// <returns>替换后的字符串</returns>
        public static string NoBreakingSpace(string str)
        {
            return str.Replace(" ", "\u00A0");
        }

        /// <summary>
        /// 测试操作。
        /// </summary>
        public static void DoTest()
        {
        }
    }
}

