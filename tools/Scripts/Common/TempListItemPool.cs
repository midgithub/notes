using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class TempListItemPool<T>where T :Component
{

    public List<T> ActiveItemGroup = new List<T>();
    Queue<T> DisableItemGroup = new Queue<T>();


    public T GetActiveItem(GameObject grid, GameObject prefab,bool isSetActive = true)
    {
        T getObj = null;
        if (DisableItemGroup.Count > 0)
        {
            getObj = DisableItemGroup.Dequeue();
            getObj.transform.parent = grid.transform;
            getObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            getObj = NGUITools.AddChild(grid, prefab).GetComponent<T>();
        }
        if (isSetActive)
            getObj.gameObject.SetActive(true);
        else
            getObj.transform.SetRenderActive(true);
        ActiveItemGroup.Add(getObj);
        return getObj;
    }
    /// <summary>
    /// 回收子对象
    /// </summary>
    /// <param name="isDisable"></param>
    public void CleanUpActiveItem(bool isDisable = true)
    {
        for (int i = 0; i < ActiveItemGroup.Count; i++)
        {
            if (isDisable)
                ActiveItemGroup[i].gameObject.SetActive(false);
            else
                ActiveItemGroup[i].transform.SetRenderActive(false);
            DisableItemGroup.Enqueue(ActiveItemGroup[i]);
        }
        ActiveItemGroup.Clear();
    }

    public void RecoverActiveItem(T item,bool isDisable = true)
    {
        if (ActiveItemGroup.Contains(item))
            ActiveItemGroup.Remove(item);
        if (isDisable)
            item.gameObject.SetActive(false);
        else
            item.transform.SetRenderActive(false);
        DisableItemGroup.Enqueue(item);
    }
}

