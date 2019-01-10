using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class Common_Base : BaseUITransformShow {

    public T FindComponent<T>(string name) where T : Component
    {
        Transform tr = transform.Find(name);
        if (tr != null)
        {
            return tr.GetComponent<T>();
        }

        return null;
    }
    /// <summary>
    /// 在子节点内搜查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T FindComponentDeepSearch<T>(string name) where T : Component
    {
        T[] tr = transform.GetComponentsInChildren<T>();
        for (int i = 0; i < tr.Length; ++i)
        {
            if (tr[i].name.CompareTo(name) == 0)
            {
                return tr[i];
            }
        }

        return null;
    }

    public T GetARequiredComponent<T>(GameObject obj) where T : Component
    {
        T t = null;
        if (obj != null)
        {
            t = obj.GetComponent<T>();
            if (t == null)
            {
                t = obj.AddComponent<T>();
            }
        }

        return t;
    }

    public UIPanel FindPanel(Transform t)
    {
        while (t != null)
        {
            UIPanel panel = t.GetComponent<UIPanel>();
            if (panel != null)
            {
                return panel;
            }

            if (t.parent != null)
            {
                t = t.parent;
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}

