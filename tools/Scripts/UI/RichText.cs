/**
* @file     : RichText.cs
* @brief    : 用于支持图片和超链接的文本控件
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-17 17:15
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;
using XLua;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class RichText : Text, IPointerClickHandler
    {
        /// <summary>
        /// 超链接信息类
        /// </summary>
[Hotfix]
        private class HrefInfo
        {
            public int startIndex;

            public int endIndex;

            public string param;

            public Color color;

            public readonly List<Rect> boxes = new List<Rect>();
        }

        //实例化调用
        protected override void Awake()
        {
            base.Awake();
            mImages.Clear();
            mIVStartIndex.Clear();
            mIVEndIndex.Clear();
            mHrefInfos.Clear();
            mUnderLines.Clear();
            UiUtil.ClearAllChildImmediate(transform);
            alignByGeometry = true;
        }

        /// <summary>
        /// 获取图像信息的正则表达式
        /// </summary>
        private static readonly Regex ImageRegex = new Regex(@"<quad name=(.+?) />", RegexOptions.Singleline);

        /// <summary>
        /// 图像列表。
        /// </summary>
        private List<Image> mImages = new List<Image>();

        /// <summary>
        /// 图片的起始顶点的索引
        /// </summary>
        private List<int> mIVStartIndex = new List<int>();

        /// <summary>
        /// 图片的最后一个顶点的索引
        /// </summary>
        private List<int> mIVEndIndex = new List<int>();

        /// <summary>
        /// 显示文本宽度。
        /// </summary>
        private float mTextWidth;

        public float TextWidth
        {
            get { return mTextWidth; }
        }

        /// <summary>
        /// 显示文本高度。
        /// </summary>
        private float mTextHeight;

        public float TextHeight
        {
            get { return mTextHeight; }
        }

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            UpdateQuadImage();
        }

        /// <summary>
        /// 更新Quad标记的图像。
        /// </summary>
        protected void UpdateQuadImage()
        {
            mOutputText = GetOutputText();
            mIVStartIndex.Clear();
            mIVEndIndex.Clear();
            mImages.RemoveAll(image => image == null);
            mUnderLines.RemoveAll(image => image == null);


#if UNITY_EDITOR

            //非运行时的编辑器状态不生成
            if (!Application.isPlaying)
            {
                return;
            }

#endif

            foreach (Match match in ImageRegex.Matches(text))
            {
                //算好图片所在的点点索引范围
                var picIndex = match.Index + match.Length - 1;
                mIVStartIndex.Add(match.Index * 6);
                mIVEndIndex.Add(picIndex * 6 + 5);

                //补够图像控件
                if (mImages.Count < mIVStartIndex.Count)
                {
                    mImages.Add(CreateImage());
                }

                //初始化图像
                string spriteName = match.Groups[1].Value.Trim();
                Image img = mImages[mIVStartIndex.Count - 1];
                if (img.name.CompareTo(spriteName) != 0)
                {
                    img.sprite = AtlasSpriteManager.Instance.GetSprite(spriteName);
                    img.name = spriteName;
                }
                img.rectTransform.sizeDelta = new Vector2(fontSize, fontSize);
                img.gameObject.SetActive(true);
            }

            //多余的图像隐藏
            for (int i = mIVStartIndex.Count; i < mImages.Count; i++)
            {
                mImages[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 创建图像控件。
        /// </summary>
        /// <returns></returns>
        public Image CreateImage()
        {
            var resources = new DefaultControls.Resources();
            var go = DefaultControls.CreateImage(resources);
            var rt = go.transform as RectTransform;
            go.layer = gameObject.layer;
            rt.SetParent(rectTransform);
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;            
            return go.GetComponent<Image>();
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            var orignText = m_Text;
            m_Text = mOutputText;
            base.OnPopulateMesh(toFill);
            m_Text = orignText;

#if UNITY_EDITOR

            //非运行时的编辑器状态不生成
            if (!Application.isPlaying)
            {
                return;
            }

#endif

            List<UIVertex> verts = new List<UIVertex>();            
            toFill.GetUIVertexStream(verts);
            RefreshImage(verts);
            RefreshHrefBoxes(verts);
            toFill.Clear();
            toFill.AddUIVertexTriangleStream(verts);
            Invoke("RefreshUnderLine", 0);
        }

        /// <summary>
        /// 刷新超链接点击盒子。
        /// </summary>
        /// <param name="verts">顶点数组。</param>
        private void RefreshHrefBoxes(List<UIVertex> verts)
        {
            // 处理超链接包围框
            foreach (var hrefInfo in mHrefInfos)
            {
                //清除掉原来的
                hrefInfo.boxes.Clear();
                if (hrefInfo.startIndex < verts.Count)
                {
                    // 将超链接里面的文本顶点索引坐标加入到包围框
                    Bounds bounds = new Bounds(verts[hrefInfo.startIndex].position, Vector3.zero);
                    int n = Math.Min(verts.Count, hrefInfo.endIndex + 1);
                    hrefInfo.color = color;
                    for (int i = hrefInfo.startIndex; i < n; ++i)
                    {
                        Vector3 pos = verts[i].position;
                        if (i % 6 == 0 && pos.y < bounds.min.y) // 换行重新添加包围框，每个字符6个点
                        {
                            hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                            bounds = new Bounds(pos, Vector3.zero);
                        }
                        else
                        {
                            bounds.Encapsulate(pos); // 扩展包围框
                        }    

                        //下划线跟着文本变色
                        if (verts[i].color != color)
                        {
                            hrefInfo.color = verts[i].color;
                        }                    
                    }                    
                    hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                }
            }
        }

        /// <summary>
        /// 刷新图像。
        /// </summary>
        /// <param name="verts"></param>
        private void RefreshImage(List<UIVertex> verts)
        {
            if (verts.Count <= 0)
            {
                mTextWidth = fontSize;
                mTextHeight = fontSize;
                return;
            }

            //逐个图像处理
            float minx = verts[0].position.x;
            float maxx = verts[0].position.x;
            float miny = verts[0].position.y;
            float maxy = verts[0].position.y;
            for (var i = 0; i < mIVStartIndex.Count; i++)
            {
                var startIndex = mIVStartIndex[i];
                if (startIndex >= verts.Count)
                {
                    break;
                }

                //调整图像位置
                var rt = mImages[i].rectTransform;
                var size = rt.sizeDelta;
                Vector3 pos = verts[startIndex].position;           //字符矩形的左上角坐标
                rt.anchoredPosition = new Vector2(pos.x + size.x / 2, pos.y - size.y / 2);
                maxx = Math.Max(maxx, pos.x + size.x);

                //清除quad标签对应的内容
                var endIndex = mIVEndIndex[i];
                for (int j = startIndex + 1; j <= endIndex && j < verts.Count; ++j)
                {
                    verts[j] = verts[startIndex];
                }
            }

            //查找最高和最低点            
            for (int i = 1; i < verts.Count; ++i)
            {
                Vector3 pos = verts[i].position;
                Vector2 uv = verts[i].uv0;
                if (Math.Abs(uv.x) + Math.Abs(uv.y) > 0)
                {
                    minx = Math.Min(minx, pos.x);
                    maxx = Math.Max(maxx, pos.x);
                    miny = Math.Min(miny, pos.y);
                    maxy = Math.Max(maxy, pos.y);
                }                
            }

            mTextWidth = Math.Max(fontSize, maxx - minx + 2);
            mTextHeight = Math.Max(fontSize, maxy - miny + 2);
        }
        
        /// <summary>
        /// 刷新下划线。
        /// </summary>
        public void RefreshUnderLine()
        {
            int n = 0;
            for (int i=0; i< mHrefInfos.Count; ++i)
            {
                List<Rect> boxes = mHrefInfos[i].boxes;
                for (int j=0; j<boxes.Count; ++j)
                {
                    //不足图像控件
                    if (n >= mUnderLines.Count)
                    {
                        Image img = CreateImage();
                        img.name = "line";                        
                        mUnderLines.Add(img);
                    }

                    //设置下划线位置和尺寸在超链接Box的底部
                    Rect r = boxes[j];
                    RectTransform imgrt = mUnderLines[n++].transform as RectTransform;
                    imgrt.gameObject.SetActive(true);
                    float xl = r.min.x;
                    float xr = r.max.x;
                    float y = r.min.y;
                    imgrt.anchoredPosition = new Vector2((xl + xr) / 2, y - 0.5f);
                    imgrt.sizeDelta = new Vector2(xr - xl, 1);
                    imgrt.GetComponent<Image>().color = mHrefInfos[i].color;
                }
            }

            //隐藏多余的下划线
            for (int i=n; i< mUnderLines.Count; ++i)
            {
                mUnderLines[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 超链接信息列表
        /// </summary>
        private List<HrefInfo> mHrefInfos = new List<HrefInfo>();

        /// <summary>
        /// 超链接正则
        /// </summary>
        private static readonly Regex HrefRegex = new Regex(@"<href param=(.*?)>(.*?)</href>", RegexOptions.Singleline);

        /// <summary>
        /// 下划线列表。
        /// </summary>
        private List<Image> mUnderLines = new List<Image>();

        /// <summary>
        /// 超链接点击事件。
        /// </summary>
        [Serializable]
[Hotfix]
        public class HrefClickEvent : UnityEvent<string> { }

        /// <summary>
        /// 超链接点击事件。
        /// </summary>
        public HrefClickEvent OnHrefClick = new HrefClickEvent();

        /// <summary>
        /// 用于设置给控件的文本。
        /// </summary>
        private string mOutputText;

        /// <summary>
        /// 获取超链接解析后的文本。
        /// </summary>
        /// <returns>传给控件显示的文本。</returns>
        protected string GetOutputText()
        {
            StringBuilder sb = new StringBuilder();
            string txt = text;
            mHrefInfos.Clear();
            var indexText = 0;
            foreach (Match match in HrefRegex.Matches(text))
            {
                sb.Append(txt.Substring(indexText, match.Index - indexText));
                //sb.Append("<color=#0000FF>");  // 超链接颜色

                var hrefInfo = new HrefInfo();

                hrefInfo.startIndex = sb.Length * 6; // 超链接里的文本起始顶点索引
                hrefInfo.endIndex = (sb.Length + match.Groups[2].Length - 1) * 6 + 5;
                hrefInfo.param = match.Groups[1].Value.Trim();
                hrefInfo.color = this.color;
                mHrefInfos.Add(hrefInfo);

                sb.Append(match.Groups[2].Value);
                //sb.Append("</color>");
                indexText = match.Index + match.Length;
            }
            sb.Append(txt.Substring(indexText, txt.Length - indexText));
            return sb.ToString();
        }

        /// <summary>
        /// 点击事件检测是否点击到超链接文本
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);
            foreach (var hrefInfo in mHrefInfos)
            {
                var boxes = hrefInfo.boxes;
                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(lp))
                    {
                        OnHrefClick.Invoke(hrefInfo.param);
                        return;
                    }
                }
            }
        }

        public override string text
        {
            get { return base.text; }
            set
            {
                base.text = value;
                Rebuild(CanvasUpdate.PreRender);
            }
        }
    }
}

