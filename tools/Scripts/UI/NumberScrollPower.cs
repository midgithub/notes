/**
* @file     : NumberScrollPower.cs
* @brief    : 战力数字滚动组件,100转123456
* @details  : 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using UnityEngine.UI;
using System;

[CSharpCallLua]
public delegate string PowerToText(float v);
[CSharpCallLua]
public delegate string ShowItemTips(int v);
namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class NumberScrollPower : NumberScroll
    {
        
        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="value">数字值。</param>
        /// <param name="ani">是否带滚动动画。</param>
        public override void SetValue(long value, bool ani=true)
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
                TextShow.text = GetPowerText(mShowValue);
            }
        }

        public string GetPowerText(float v)
        {
            PowerToText fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<PowerToText>("Common.PowerToText");
            if(fun != null)
            {
                return fun(v);
            }

            return Math.Floor(v).ToString();
        }
        // Update is called once per frame
        public override void Update()
        {
            if (mScrollCount <= 0)
            {
                return;
            }

            float time = Math.Max(0.1f, ScrollTime);
            mScrollCount = Mathf.Max(0, mScrollCount - Time.deltaTime);
            int value = (int)Mathf.Lerp(mValue, mOldValue, mScrollCount / time);
            if (value != mShowValue)
            {
                mShowValue = value;
                TextShow.text = GetPowerText(mShowValue);
            }
        }
    }
}

