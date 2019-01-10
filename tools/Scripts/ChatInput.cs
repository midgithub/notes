using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using System.Collections.Generic;
using System.Text;

namespace SG
{
    [RequireComponent(typeof(InputField))]
    [LuaCallCSharp]
[Hotfix]
    public class ChatInput : MonoBehaviour
    {
[Hotfix]
        class ExtraInfo
        {
            public ExtraInfo(int start, int end, string data)
            {
                StartIndex = start;
                EndIndex = end;
                ExtraData = data;
            }

            /// <summary>
            /// 额外信息的起始索引。
            /// </summary>
            public int StartIndex { get; set; }

            /// <summary>
            /// 额外信息的结束索引。
            /// </summary>
            public int EndIndex { get; set; }

            /// <summary>
            /// 额外数据。
            /// </summary>
            public string ExtraData { get; set; }
        }

        private string m_LastText = string.Empty;

        private List<ExtraInfo> m_SendInfo = new List<ExtraInfo>();


        public void OnValueChange(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                m_LastText = string.Empty;
                m_SendInfo.Clear();
                return;
            }
            if (m_LastText.CompareTo(text) == 0)
            {
                return;
            }

            //找到有发生改变的字符索引
            int index = 0;
            int n = Mathf.Min(m_LastText.Length, text.Length);
            for (int i=0; i<n; ++i)
            {
                char c1 = m_LastText[i];
                char c2 = text[i];
                if (c1 != c2)
                {
                    index = i;
                    break;
                }
                else
                {
                    index += 1;
                }
            }

            //删掉被修改的替换段，调整索引
            int offset = text.Length - m_LastText.Length;
            for (int i=0; i<m_SendInfo.Count; ++i)
            {
                ExtraInfo info = m_SendInfo[i];
                if (offset == 0)
                {
                    //修改
                    if (index >= info.StartIndex && index < info.EndIndex)
                    {
                        m_SendInfo.RemoveAt(i);
                        break;
                    }
                }
                else if (offset > 0)
                {
                    //添加文本
                    if (index > info.StartIndex)
                    {
                        if (index < info.EndIndex)
                        {
                            m_SendInfo.RemoveAt(i);
                            --i;            //删了一个，要补回来
                        }
                    }
                    else
                    {
                        info.StartIndex += offset;
                        info.EndIndex += offset;
                    }
                }
                else if (offset < 0)
                {
                    //删除文本
                    if (index >= info.StartIndex)
                    {
                        if (index < info.EndIndex)
                        {
                            m_SendInfo.RemoveAt(i);
                            --i;            //删了一个，要补回来
                        }
                    }
                    else if ((index - offset) >= info.StartIndex)
                    {
                        if ((index - offset) < info.EndIndex)
                        {
                            m_SendInfo.RemoveAt(i);
                            --i;            //删了一个，要补回来
                        }
                    }
                    else
                    {
                        info.StartIndex += offset;
                        info.EndIndex += offset;
                    }
                }
            }

            m_LastText = text;
        }

        /// <summary>
        /// 添加额外数据。
        /// </summary>
        /// <param name="show">显示的文本。</param>
        /// <param name="data">额外数据。</param>
        public void AddExtraInfo(string show, string data)
        {
            InputField input = GetComponent<InputField>();
            m_SendInfo.Add(new ExtraInfo(m_LastText.Length, m_LastText.Length + show.Length, data));
            m_LastText = m_LastText + show;
            input.text = m_LastText;
        }

        /// <summary>
        /// 获取发送的文本。
        /// </summary>
        /// <returns>发送的文本。</returns>
        public string GetSendText()
        {
            //替换字符串
            StringBuilder sb = new StringBuilder();
            int index = 0;
            for (int i = 0; i<m_SendInfo.Count; ++i)
            {
                ExtraInfo info = m_SendInfo[i];
                if (index < info.StartIndex)
                {
                    sb.Append(m_LastText.Substring(index, info.StartIndex - index));
                }
                sb.Append(info.ExtraData);
                index = info.EndIndex;
            }
            if (index < m_LastText.Length)
            {
                sb.Append(m_LastText.Substring(index, m_LastText.Length - index));
            }
            return sb.ToString();
        }
    }
}

