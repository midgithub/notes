using XLua;
﻿using UnityEngine;
using System.Collections;


namespace SG
{

[Hotfix]
    public class TimerBasedBuffEffect : BuffEffect
    {
        protected ACTOR_STATE RelativeState = ACTOR_STATE.AS_NONE;

        public override void ReplaceBuff()
        {
            if (owner != null)
            {
                owner.OnResetTimerState(RelativeState);
            }
        }
    }

}

