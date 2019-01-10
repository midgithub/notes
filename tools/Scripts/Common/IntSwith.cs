using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class IntSwith{

    public int TempValue;
    public int RandomValue = 0;
    int MOD = 2;

    public IntSwith()
    {
 
    }

    public IntSwith(int v)
    {
        Set(v);
    }

    public void SetTempValue(int tempValue,int randomValue)
    {
        this.TempValue = tempValue;
        this.RandomValue = randomValue;
    }

    public void Set(int mValue)
    {
        RandomValue = Random.Range(9, 9999);
        int add = mValue + RandomValue;
        bool isCount = (add % MOD) == 0;
        TempValue = add * (isCount ? -1 : 1);
    }

    public int Get()
    {
        int num = 0;
        if (TempValue != 0 || RandomValue != 0)
        {
            bool isCount = TempValue % 2 == 0;
            num = TempValue * (isCount ? -1 : 1);
            num -= RandomValue;
        }
        return num;
    }


}

