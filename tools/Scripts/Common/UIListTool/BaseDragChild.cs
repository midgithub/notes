using XLua;
﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIDragScrollView))]
[Hotfix]
public abstract class BaseDragChild : MonoBehaviour {

    Transform tran;
    public Transform mTran { get {
        if (tran == null)
            tran = transform;
        return tran;
    } }
    public int mIndex { get; set; }
    public int mDataIndex { get; set; }


    public abstract void Init(object data);

    public virtual void UpdateData(object data) { }
}

