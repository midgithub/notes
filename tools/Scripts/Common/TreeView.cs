using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SG;

[Hotfix]
public class TreeView : MonoBehaviour
{
    List<GameObject> _treeViewItems = new List<GameObject>();
    public GameObject grid;
    public int gridSpaceY; //grid1
    public int gridItemHeight;  //grid1 item
    public int itemGridSpaceY;          //grid2
    public int itemGridItemHeight;  //grid2 item
   // public int VerticalItemSpace;  //横

    bool bfinish = false;   //是否读取完

    /// <summary>
    /// scrollview 总高
    /// </summary>
    int _yIndex = 0;

    [HideInInspector]
    /// <summary>
    /// 当前展开的item
    /// </summary>
    public TreeViewItem curItem;

    /// <summary>
    /// 刷新树形菜单
    /// </summary>
    public void RefreshTreeView()
    {
        _yIndex = -gridItemHeight/2;
        //复制一份菜单
        List<GameObject> _treeViewItemsClone = new List<GameObject>(_treeViewItems);

        //用复制的菜单进行刷新计算
        for (int i = 0; i < _treeViewItemsClone.Count; i++)
        {
            //已经计算过或者不需要计算位置的元素
            if (_treeViewItemsClone[i] == null || !_treeViewItemsClone[i].activeSelf)
            {
                continue;
            }
            TreeViewItem tvi = _treeViewItemsClone[i].GetComponent<TreeViewItem>();
            _treeViewItemsClone[i].GetComponent<RectTransform>().localPosition = new Vector3(0, _yIndex, 0);
            _yIndex += (-(gridSpaceY + gridItemHeight));

            //如果子元素是展开的，继续向下刷新
            if (tvi.IsExpanding)
            {
                curItem = tvi;
                RefreshTreeViewChild(tvi);
            }
            _treeViewItemsClone[i] = null;
        }

        //重新计算滚动视野的区域
        //float x = 0;
        float y = Mathf.Abs(_yIndex) + gridItemHeight;
        transform.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(0, y);
        //清空复制的菜单
        _treeViewItemsClone.Clear();
    }
    /// <summary>
    /// 刷新元素的所有子元素
    /// </summary>
    void RefreshTreeViewChild(TreeViewItem tvi)
    {
        for (int i = 0; i < tvi.GetChildrenNumber(); i++)
        {          
            _yIndex += (-(itemGridSpaceY + itemGridItemHeight));
            ////如果子元素是展开的，继续向下刷新
            //if (tvi.GetChildrenByIndex(i).IsExpanding)
            //{
            //    RefreshTreeViewChild(tvi.GetChildrenByIndex(i));
            //}

            //int index = _treeViewItemsClone.IndexOf(tvi.GetChildrenByIndex(i).gameObject);
            //if (index >= 0)
            //{
            //    _treeViewItemsClone[index] = null;
            //}
        }
    }

    /// <summary>
    /// 外部调用。 默认展开下拉表的第几个item  , 索引从0 开始
    /// </summary>
    /// <param name="index"></param>
    public void OpenIndex(int index)
    {
        if(!bfinish)
        {
            EmbaChildList();
        }
        if(_treeViewItems.Count <=index )
        {
            LogMgr.LogError("参数传递错误,下拉表的成员个数小于传入参数--> " + index);
            return;
        }
        TreeViewItem tt = _treeViewItems[index].GetComponent<TreeViewItem>();
        CheckItemState(tt);
    }

    /// <summary>
    /// 读取所有成员信息
    /// </summary>
    public void EmbaChildList()
    {
        foreach (Transform item in grid.transform)
        {
           TreeViewItem tt = item.gameObject.GetComponent<TreeViewItem>();
            tt.HideViewChild();  //隐藏所有子类
            tt.transform.GetComponent<Button>().onClick.AddListener(delegate () {
                CheckItemState(tt);
            });
            _treeViewItems.Add(item.gameObject);
        }
        bfinish = true;
    }
    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="tt"></param>
    void CheckItemState(TreeViewItem tt)
    {
        if(curItem != null && curItem != tt)
        {
            curItem.HideViewChild();
//            curItem = null;
        }
        if(tt.IsExpanding)
        {           
            tt.HideViewChild();
        }
        else
        {
            tt.ShowViewChild();
        }
        RefreshTreeView();
    }
    /*
    public void HideViewChild(TreeViewItem tt)
    {
        tt.IsExpanding = false;
        foreach (Transform item in tt.grid.transform)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void ShowViewChild(TreeViewItem tt)
    {
        tt.IsExpanding = true;
        foreach (Transform item in tt.grid.transform)
        {
            item.gameObject.SetActive(true);
        }
    }
    */
}

