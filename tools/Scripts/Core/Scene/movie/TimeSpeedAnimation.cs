using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class TimeSpeedAnimation : MonoBehaviour
{

    /// <summary>
    /// 时间缩放
    /// </summary>
    public float Speed = 0;

    /// <summary>
    /// 是否有效果
    /// </summary>
    public bool IsTakeEffect = false;
    
	// Update is called once per frame
	void Update () {

        if (IsTakeEffect)
        {
            if (Speed > 0)
            {
                SG.CoreEntry.gTimeMgr.TimeScale = Speed;

            }
        }
        else if (Speed != 1)
        {
            Speed = 1;
            SG.CoreEntry.gTimeMgr.TimeScale = Speed;
        }
	}
    
}

