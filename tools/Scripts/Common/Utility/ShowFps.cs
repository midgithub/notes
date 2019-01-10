/**
* @file     : ShowFps.cs
* @brief    : 显示帧数
* @details  : 显示帧数
* @author   : 
* @date     : 2014-12-09
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class ShowFps : MonoBehaviour
    {
        public static float UpdateInterval = 0.5F;

        private float f_LastInterval;
        private int i_Frames = 0;
        private float f_Fps = 0;

        void Start()
        {
            f_LastInterval = Time.realtimeSinceStartup;
            i_Frames = 0;
        }

        void Update()
        {
            if (!SG.LogMgr.GMSwitch)
                return;
            ++i_Frames;

            if (Time.realtimeSinceStartup > f_LastInterval + UpdateInterval)
            {
                int old = (int)(f_Fps * 10);
                f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);
                i_Frames = 0;
                f_LastInterval = Time.realtimeSinceStartup;

                //是否通知更新
                int cur = (int)(f_Fps * 10);
                if (old != cur)
                {
                    EventParameter ep = EventParameter.Get();
                    ep.floatParameter = f_Fps;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FPS, ep);
                }                
            }
        }
    }
}

