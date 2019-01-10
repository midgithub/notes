using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
namespace SG
{
    /// <summary>
    /// 序列帧动画。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
	public class SpriteAnimation : MonoBehaviour
    {
        public string SpriteName;                           //贴图名称
        public int StartIndex;                              //起始序号
        public int Number;                                  //贴图数量
        public Sprite[] Frames;                             //动画帧序列。
        public float FrameGap;                              //帧间隔
        public float CycleGap;                              //周期间隔
        public float Delay;                                 //延迟时间
        public bool PlayOnAwake;                            //唤醒时是否播放
        public bool Loop;                                   //是否循环
        public bool LoopGapShow;                            //周期间隔是否停留最后一帧
        public bool AutoNativeSize = true;

        private Image Show;                                  //显示动画的图像

        private int _index;                                 //当前帧索引
        private int _state;                                 //动画状态 0未播放 1延迟 2播放中 3周期间隔
        private float _stateCount;                          //状态计数


        private bool isPlaying = true;

        //spName : "QualityEffect:QE3_"
        public void ReLoad(string spName)
        {
            if (SpriteName != spName)
            {
                SpriteName = spName;
                Frames = null;
                Load();
            }

        }

        /// <summary>
        /// 唤醒。
        /// </summary>
        protected void Awake()
        {
            SetShowObj();
            Load();
            if (PlayOnAwake)
            {
                Play();
            }
        }


        public void Load()
        {
            if ((Frames == null || Frames.Length <= 0) && !string.IsNullOrEmpty(SpriteName) && Number > 0)
            {
                Frames = new Sprite[Number];
                for (int i = 0; i < Number; ++i)
                {
                    Frames[i] = AtlasSpriteManager.Instance.GetSprite(string.Format("{0}{1:D2}", SpriteName, StartIndex + i));
                }
            }          
        }


        /// <summary>
        /// 更新。
        /// </summary>
		void Update()
        {
            if (!isPlaying)
                return;
            if (Frames == null)
            {
                return;
            }
            if (_index >= Frames.Length)
                return;
            _stateCount += Time.deltaTime;
            if (_state == 1)
            {
                if (_stateCount >= Delay)
                {
                    _index = 0;
                    _state = 2;
                    _stateCount = 0;
                    if (_index < Frames.Length)
                    {
                        Show.enabled = true;
                        Show.sprite = Frames[_index];
                        if (AutoNativeSize)
                        {
                            Show.SetNativeSize();
                        }
                    }
                }
            }
            else if (_state == 2)
            {
                if (_stateCount >= FrameGap)
                {
                    _stateCount -= FrameGap;
                    ++_index;
                    if (_index < Frames.Length)
                    {
                        Show.sprite = Frames[_index];
                        if (AutoNativeSize)
                        {
                            Show.SetNativeSize();
                        }
                    }
                    else
                    {
                        if (CycleGap > 0)
                        {
                            _state = 3;
                            _stateCount = 0;
                            Show.enabled = LoopGapShow;
                        }
                        else
                        {
                            if (Loop)
                            {
                                _index = 0;
                                Show.sprite = Frames[_index];
                                if (AutoNativeSize)
                                {
                                    Show.SetNativeSize();
                                }
                            }
                            else
                            {
                                _state = 0;
                                Show.enabled = false;
                            }
                        }
                    }
                }
            }
            else if (_state == 3)
            {
                if (_stateCount >= CycleGap)
                {
                    Show.enabled = true;
                    if (Loop)
                    {
                        _index = 0;
                        _state = 2;
                        _stateCount = 0;
                        Show.sprite = Frames[_index];
                        if (AutoNativeSize)
                        {
                            Show.SetNativeSize();
                        }
                    }
                    else
                    {
                        _state = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 播放。
        /// </summary>
        public void Play()
        {
            isPlaying = true;
            _state = 1;
            _stateCount = 0;
            _index = 0;
            SetShowObj();

            if (Show)
            {
                Show.enabled = true;
            }
        }
        private void SetShowObj()
        {
            if (Show == null)
            {
                Show = GetComponent<Image>();
            }
        }
        public void Stop()
        {
            isPlaying = false;
            SetShowObj();

            if (Show)
            {
                _state = 1;
                _stateCount = 0;
                Show.enabled = false;
            }
        }
    }
}


