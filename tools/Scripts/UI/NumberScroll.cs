/**
* @file     : NumberScroll.cs
* @brief    : 数字滚动组件
* @details  : 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using UnityEngine.UI;
using System;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class NumberScroll : BaseMeshEffect
    {
        public enum ScrollType
        {
            /// <summary>
            /// 数字计数。
            /// </summary>
            Count,

            /// <summary>
            /// 逐位改变。
            /// </summary>
            PetBit,
        }
        
        /// <summary>
        /// 当前显示的值。
        /// </summary>
        protected long mValue;

        /// <summary>
        /// 之前的值。
        /// </summary>
        protected long mOldValue;

        /// <summary>
        /// 当前显示的值。
        /// </summary>
        protected long mShowValue;

        /// <summary>
        /// 滚动计数。
        /// </summary>
        protected float mScrollCount;

        /// <summary>
        /// 显示的文本组建。
        /// </summary>
        public Text TextShow;

        /// <summary>
        /// 滚动时间。
        /// </summary>
        public float ScrollTime = 0.5f;

        /// <summary>
        /// 滚动类型。
        /// </summary>
        public ScrollType Type = NumberScroll.ScrollType.Count;

        /// <summary>
        /// 获取或设置当前显示的值。
        /// </summary>
        public long Value
        {
            get { return mValue; }
            set
            {
                SetValue(value);
            }
        }

        protected override void Awake()
        {
            if (TextShow == null)
            {
                TextShow = GetComponent<Text>();
            }            
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (Type != ScrollType.PetBit)
            {
                return;
            }
        }

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="value">数字值。</param>
        /// <param name="ani">是否带滚动动画。</param>
        public virtual void SetValue(long value, bool ani=true)
        {
            if (ani)
            {
                mOldValue = mValue;
                mValue = value;
                mScrollCount = Math.Max(0.1f, ScrollTime);
            }
            else
            {
                mValue = value;
                mOldValue = mValue;
                mShowValue = mValue;
                TextShow.text = mShowValue.ToString();
            }
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (mScrollCount <= 0)
            {
                return;
            }

            float time = Math.Max(0.1f, ScrollTime);
            mScrollCount = Mathf.Max(0, mScrollCount - Time.deltaTime);
            long value =(long)Mathf.Lerp(mValue, mOldValue, mScrollCount / time);
            if (value != mShowValue)
            {
                mShowValue = value;
                TextShow.text = mShowValue.ToString();
            }            
        }
    }
}

