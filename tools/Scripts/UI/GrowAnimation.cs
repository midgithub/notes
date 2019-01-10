using XLua;
﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
    /// <summary>
    /// 增长动画。
    /// </summary>
[Hotfix]
	public class GrowAnimation : MonoBehaviour
	{
        public Image ShowImage;                             //显示用的图像

        private float _fullTime;                            //长满次数
        private float _start;                               //起始比例
        private float _end;                                 //结束比例
        private float _count;                               //动画计数
        private float _countMax;                            //计数上限
        private Action<int> _funllCall;                     //经验满调用
        private int _fullCount;                             //满经验计数

        /// <summary>
        /// 唤醒。
        /// </summary>
        protected void Awake()
		{
			if (ShowImage == null)
            {
                ShowImage = this.GetComponent<Image>();
            }
		}
        
        /// <summary>
        /// 更新。
        /// </summary>
		public void Update ()
		{
	        if (_count > 0)
            {
                _count -= Time.deltaTime;
                float to = _fullTime > 0 ? 1 : _end;
                ShowImage.fillAmount = _start + (to - _start) * (_countMax - _count) / _countMax;
                if (_count <= 0)
                {
                    if (_fullTime > 0)
                    {
                        --_fullTime;
                        _start = 0;
                        _count = _fullTime > 0 ? 1 : 0.5f;  //最后剩余0.5秒
                        _countMax = _count;
                        ++_fullCount;
                        if (_funllCall != null)
                        {
                            _funllCall(_fullCount);
                        }
                    }
                    else
                    {
                        _count = 0;                        
                        if (_funllCall != null)
                        {
                            ++_fullCount;
                            if (_end >= 1)
                            {
                                _funllCall(_fullCount);
                            }
                            _funllCall(0);
                        }
                    }
                }
            }
		}

        /// <summary>
        /// 设置比例。
        /// </summary>
        /// <param name="p">显示比例。</param>
        public void SetProgress(float p)
        {
            if (ShowImage == null)
            {
                return;
            }
            ShowImage.fillAmount = p;
        }

        /// <summary>
        /// 显示增长动画。
        /// </summary>
        /// <param name="p"></param>
        /// <param name="full"></param>
        public void ShowGrowAnimation(float p, int full = 0, Action<int> fullcall = null)
        {
            if (ShowImage == null)
            {
                return;
            }
                        
            _fullTime = full;
            _start = full == 0 ? Math.Min(p, ShowImage.fillAmount) : ShowImage.fillAmount;
            _end = p;
            _count = _fullTime > 0 ? 0.5f : 1;      //起始加满0.5秒
            _countMax = _count;
            _funllCall = fullcall;
            _fullCount = 0;
        }
    }
}


