using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[Hotfix]
public class TreeViewItem : MonoBehaviour {

    [HideInInspector]
    public bool IsExpanding = false;  //是否展开  

    public GameObject grid;

    public int GetChildrenNumber()
    {
        return grid.transform.childCount;
    }

    public TreeViewItem GetChildrenByIndex(int index)
    {
        return this;
    }
    public void HideViewChild()
    {
        if(this.gameObject == null)
        {
            return;
        }
        IsExpanding = false;
        foreach (Transform item in grid.transform)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void ShowViewChild()
    {
        if (this.gameObject == null)
        {
            return;
        }
        IsExpanding = true;
        foreach (Transform item in grid.transform)
        {
            item.gameObject.SetActive(true);
        }
    }
}

