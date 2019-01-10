/**
* @file     : EventToUI.cs
* @brief    : 通知界面逻辑事件
* @details  : 注册事件：RegisterEvent() 发送事件：SendEvent() 
* 			  设置参数：SetArg()，当前最多只支持Arg1 - Arg4四个参数 
* 			  获取参数：GetArg()，注意获取参数后需要强制类型转换，否则会编译报错
* @author   : 
* @date     : 2014-9-24
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

	public delegate void UIOnEvent();

	public enum UIEventArg
	{
		Arg1,
		Arg2,
		Arg3,
		Arg4,
        Arg5,
	}

	//public class EventUISrv {

	//	public virtual void OnEvent() {}
	//}

        /// <summary>
        /// 已废弃，不要再用
        /// </summary>
[Hotfix]
	public class EventToUI {

		private static Dictionary<string, List<UIOnEvent>> g_UIOnEventDict = new Dictionary<string, List<UIOnEvent>>();

		private static string g_strEvent;
		public static string sEvent { get{ return g_strEvent; } }

		private static Dictionary<UIEventArg, ValueArg> g_argDict = new Dictionary<UIEventArg, ValueArg>();

		public static void SetArg(UIEventArg key, bool value) 
		{
			ValueArg temp = new ValueArg();
			temp.SetValue(value);
			
			if (g_argDict.ContainsKey(key))
			{
				g_argDict[key] = temp;
			}
			else
			{
				g_argDict.Add(key, temp);
			}
		}

		public static void SetArg(UIEventArg key, int value) 
		{
			ValueArg temp = new ValueArg();
			temp.SetValue(value);
			
			if (g_argDict.ContainsKey(key))
			{
				g_argDict[key] = temp;
			}
			else
			{
				g_argDict.Add(key, temp);
			}
		}
		
		public static void SetArg(UIEventArg key, float value)
		{
			ValueArg temp = new ValueArg();
			temp.SetValue(value);
			
			if (g_argDict.ContainsKey(key))
			{
				g_argDict[key] = temp;
			}
			else
			{
				g_argDict.Add(key, temp);
			}
		}
		
		public static void SetArg(UIEventArg key, double value)
		{
			ValueArg temp = new ValueArg();
			temp.SetValue(value);
			
			if (g_argDict.ContainsKey(key))
			{
				g_argDict[key] = temp;
			}
			else
			{
				g_argDict.Add(key, temp);
			}
		}
		
		public static void SetArg(UIEventArg key, string value)
		{
			ValueArg temp = new ValueArg();
			temp.SetValue(value);
			
			if (g_argDict.ContainsKey(key))
			{
				g_argDict[key] = temp;
			}
			else
			{
				g_argDict.Add(key, temp);
			}
		}

		public static void SetArg(UIEventArg key, GameObject value)
		{
			ValueArg temp = new ValueArg();
			temp.SetValue(value);
			
			if (g_argDict.ContainsKey(key))
			{
				g_argDict[key] = temp;
			}
			else
			{
				g_argDict.Add(key, temp);
			}
		}
        public static void SetArg(UIEventArg key, object value)
        {
            ValueArg temp = new ValueArg();
            temp.SetValue(value);

            if (g_argDict.ContainsKey(key))
            {
                g_argDict[key] = temp;
            }
            else
            {
                g_argDict.Add(key, temp);
            }
        }
		
		public static object GetArg(UIEventArg key) 
		{  
			ValueArg tempArg;
			if (g_argDict.TryGetValue(key, out tempArg))
			{
				return tempArg.GetValue();
			}
			else
			{
				return "null";
			}  
		}
	
		public static void RegisterEvent(string sEvtName, UIOnEvent onEvent)
		{
			List<UIOnEvent> eventList;
			if (g_UIOnEventDict.TryGetValue(sEvtName, out eventList))
			{
				if (eventList.Contains(onEvent) == false)
				{
					eventList.Add(onEvent);
				}
			}
			else
			{
				eventList = new List<UIOnEvent>();
				eventList.Add(onEvent);
				g_UIOnEventDict.Add(sEvtName, eventList);
			}
		}

		public static void UnRegisterEvent(string sEvtName, UIOnEvent onEvent)
		{
			List<UIOnEvent> eventList;
			if (g_UIOnEventDict.TryGetValue(sEvtName, out eventList))
			{
				if (eventList.Contains(onEvent) == true)
				{
					eventList.Remove(onEvent);
				}

				if (eventList.Count <= 0)
				{
					g_UIOnEventDict.Remove (sEvtName);
				}
			}
		}

		// Use this for initialization
		public static void SendEvent(string sEvtName)
		{

            List<UIOnEvent> eventList;
            if (g_UIOnEventDict.TryGetValue(sEvtName, out eventList))
            {
                for(int i = 0 ; i < eventList.Count ; ++i )
                {
                    g_strEvent = sEvtName;
                    try
                    {
                        eventList[i]();
                    }
                    catch (System.Exception e)
                    {
                        LogMgr.UnityError(e.ToString()); 
                    }
                }
            }
		}
	}

};//End SG

