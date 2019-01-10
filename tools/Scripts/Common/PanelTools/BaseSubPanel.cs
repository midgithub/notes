using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public abstract class BaseSubPanel<T> : BaseUITransformShow {

    protected T mParent;
    public virtual void InitParent(T parent)
    {
        mParent = parent;
    }
    public abstract void ShowPanel(params object[] param);

    public virtual void UpdatePanel(params object[] param)
    { 
    }

}

