using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class GameGraphicSetting
    {
        //true代表使用低画质配置
        static bool isLowQuality = false;

        public static bool IsLowQuality
        {
            get
            {
                return isLowQuality;
            }
            set
            {
                isLowQuality = value;

                EffectSetting.OnChange();
                MaterialSetting.OnChange();

                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_GameGraphicSetting, null);
            }
        }
    }
}

