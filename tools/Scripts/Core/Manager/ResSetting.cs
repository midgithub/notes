using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;

[Hotfix]
public class ResSetting : ClientSetting
{
    private static ResSetting instance = null;

    public static ResSetting Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new ResSetting();
            }
            return instance; 
        }
    }

    public ResSetting()
    {
        ReLoadClientSettingData();
    }

    public override void ReLoadClientSettingData()
    {
        mData = ClientSetting.Instance.ConfigData;
    }

}

