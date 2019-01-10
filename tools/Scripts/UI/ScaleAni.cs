/**
* @file     : ScaleAni.cs
* @brief    : 缩放动画曲线
* @details  : 
* @author   : 
* @date     : 
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class ScaleAni : MonoBehaviour
    {
        public AnimationCurve CurveX = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public AnimationCurve CurveY = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        public bool PlayOnAwake;                            //唤醒时是否播放
        public RectTransform Target;

        private float _count = 0;                 //计时
        private float _duration = 0;                //动画时长
        private bool _playing = false;              //是否播放中

        /// <summary>
        /// 获取动画时长。
        /// </summary>
        public float Duration
        {
            get { return _duration; }
        }

        void Awake()
        {
            if (Target == null)
            {
                Target = transform as RectTransform;
            }           
            if (PlayOnAwake)
            {
                Play();
            } 
        }

        public void Play()
        {
            this.enabled = true;
            _count = 0;
            _duration = CurveX.length > 0 ? CurveX.keys[CurveX.keys.Length - 1].time : 0;
            _duration = Mathf.Max(_duration, CurveY.length > 0 ? CurveY.keys[CurveY.keys.Length - 1].time : 0);
            _playing = true;
            Evaluate(0);
        }

        // Update is called once per frame
        void Update()
        {
            if (!_playing)
            {
                this.enabled = false;
                return;
            }

            _count += Time.deltaTime;
            Evaluate(_count);
            if (_count >= _duration)
            {
                _playing = false;                
            }
        }

        private void Evaluate(float t)
        {
            float sx = CurveX.Evaluate(t);
            float sy = CurveY.Evaluate(t);
            Target.localScale = new Vector3(sx, sy, 1);
        }
    }
}

