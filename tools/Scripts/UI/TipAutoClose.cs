/**
* @file     : TipAutoClose.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;

namespace SG
{
    /// <summary>
    /// 自动关闭弹窗。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class TipAutoClose : PanelBase
    {
        public delegate string GetTimerNoteDelegate(int sec);

        public static void Show(int sec, string title, GetTimerNoteDelegate notecall)
        {
            TipAutoClose tipDialog = MainPanelMgr.Instance.ShowDialog("TipAutoClose") as TipAutoClose;
            if (null == tipDialog)
            {
                return;
            }

            tipDialog.Init(sec, title, notecall);
        }

        //public Text Title;                                  //标题
        public Text Note;                                   //描述

        public float m_Duration;                            //持续时间
        public float m_Count;                               //计时
        public int m_LastSecond;                            //上次秒数
        public GetTimerNoteDelegate m_NoteCall;             //获取描述回调

        public void Init(int sec, string title, GetTimerNoteDelegate notecall)
        {
            m_Duration = sec;
            m_Count = m_Duration;
            m_LastSecond = (int)m_Count;
            m_NoteCall = notecall;
            //Title.text = title;
            Note.text = m_NoteCall == null ? string.Empty : m_NoteCall(m_LastSecond);
        }

        protected override void Update()
        {
            base.Update();
            if (m_Count > 0)
            {
                m_Count -= Time.deltaTime;
                int sec = (int)m_Count;
                if (m_LastSecond != sec)
                {
                    m_LastSecond = sec;
                    Note.text = m_NoteCall == null ? string.Empty : m_NoteCall(m_LastSecond);
                    PlayerData.Instance.ReviceHideLessTime = m_LastSecond;
                }
                if (m_Count <= 0)
                {
                    MainPanelMgr.Instance.HideDialog("TipAutoClose");
                    PlayerData.Instance.ReviceHideLessTime = 0;
                }
            }
        }
    }
}

