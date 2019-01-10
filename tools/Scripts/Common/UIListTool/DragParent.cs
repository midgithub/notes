using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class DragParent : MonoBehaviour {

    public UIGrid mGrid;

    public UIScrollView mUIScrollView;
    Transform uiScrollviewTran;
    public Transform mUIScrollviewTran
    {
        get
        {
            if (uiScrollviewTran == null)
                uiScrollviewTran = mUIScrollView.transform;
            return uiScrollviewTran;
        }
    }

    float uiScrollViewSideY = 0;
    float uiScrollViewSideX = 0;
    UIPanel mUIPanel ;
    public float mUIScrollViewSideY
    {
        get
        {
            if (mUIPanel == null)
            {
                mUIPanel = mUIScrollView.GetComponent<UIPanel>();
                uiScrollViewSideY = mUIPanel.GetViewSize().y;
            }
            return uiScrollViewSideY;
        }
    }
    public float mUIScrollViewSideX
    {
        get
        {
            if (uiScrollViewSideX == 0)
            {
                mUIPanel = mUIScrollView.GetComponent<UIPanel>();
                uiScrollViewSideX = mUIPanel.GetViewSize().x;
            }
            return uiScrollViewSideX;
        }
    }


    public List<BaseDragChild> mActiveDragChildGroup = new List<BaseDragChild>();
    Queue<BaseDragChild> mDisableDragChildGroup = new Queue<BaseDragChild>();
    Dictionary<int, object> mDataDic = new Dictionary<int, object>();

    GameObject mChildPrefab;

    //int CurrentNum = 0;
    int mWidthNum = 1;
    int mPosWidth = 0;
    int mPosHeight = 0;

    int mPosYCount = 0;

    float DisableItemDis = 0;
    float ActiveItemDis = 0;

    bool IsHorizontal = false;
    bool IsDisableHide = false;
    void Awake()
    {
        if (mUIScrollView!=null)
            mUIScrollView = GetComponent<UIScrollView>();
        mUIScrollView.SetDragCallBack(DragCallBack);
    }
    public void Init<T>(GameObject childPrefab, int defaultNum, List<T> dataGroup, int posWidth, int posHeight, int widthNum = 1,bool isSync = true,bool horizontal = false,bool isDisableHide = false)
    {
        IsHorizontal = horizontal;
        IsDisableHide = isDisableHide;
        if (mChildPrefab != childPrefab)
        {
            mActiveDragChildGroup.ApplyAllItem(C => Destroy(C.gameObject));
            mDisableDragChildGroup.ApplyAllItem(C => Destroy(C.gameObject));
            mActiveDragChildGroup.Clear();
            mDisableDragChildGroup.Clear();
            mChildPrefab = childPrefab;
        }
        else
        {
            mActiveDragChildGroup.ApplyAllItem(C => {
                //C.gameObject.SetActive(false);
                SetItemShowEnable(C.transform, false);
                mDisableDragChildGroup.Enqueue(C);
            });
            mActiveDragChildGroup.Clear();
        }
        mDataDic.Clear();

        mWidthNum = widthNum;
        mPosWidth = posWidth;
        mPosHeight = posHeight;
        mPosYCount = defaultNum / widthNum;
        if (IsHorizontal)
        {
            ActiveItemDis = mUIScrollViewSideX * 0.5f + posWidth * 1;
            DisableItemDis = ActiveItemDis + posWidth;
        }
        else
        {
            ActiveItemDis = mUIScrollViewSideY * 0.5f + mPosHeight * 1;
            DisableItemDis = ActiveItemDis + mPosHeight ;
        }
        for (int i = 0; i < dataGroup.Count; i++)
            mDataDic[i] = dataGroup[i];

        mUIScrollView.ResetPosition();

        StartCoroutine(ShowItem(defaultNum,dataGroup,isSync));
    }

    public void UpdateShowData<T>(List<T> dataGroup)
    {
        if (mDataDic.Count != dataGroup.Count)
        {
            SG.LogMgr.LogError("无法刷新数据，新数据长度不一样");
            return;
        }
        mDataDic.Clear();
        for (int i = 0; i < dataGroup.Count; i++)
            mDataDic[i] = dataGroup[i];
        for(int i=0;i<mActiveDragChildGroup.Count;i++)
        {
            var child = mActiveDragChildGroup[i];
            child.UpdateData(mDataDic[child.mDataIndex]);
        }
    }

    void SetItemShowEnable(Transform go, bool isShow)
    {
        if(IsDisableHide)
            go.gameObject.SetActive(isShow);
        else
            go.SetRenderActive(isShow);
    }
    IEnumerator ShowItem<T>(int defaultNum, List<T> dataGroup,bool isSync)
    {
        for (int i = 0; i < dataGroup.Count; i++)
        {
            if (i < (defaultNum))
            {
                BaseDragChild itemChild = GetBaseDragChildByDisableGroup();
                SetItemPos(itemChild, i);
                itemChild.Init(dataGroup[i]);
                itemChild.mDataIndex = i;
                SetItemShowEnable(itemChild.transform,true);
                mActiveDragChildGroup.Add(itemChild);
            }
            if(i==0)
                mUIScrollView.ResetPosition();
            if (isSync)
            {
                yield return null;
            }
            if(i == defaultNum)
                mUIScrollView.ResetPosition();
        } 
    }

    BaseDragChild GetBaseDragChildByDisableGroup()
    {
        BaseDragChild getChildItem;
        if (mDisableDragChildGroup.Count <= 0)
        {
            getChildItem = NGUITools.AddChild(mGrid.gameObject, mChildPrefab).GetComponent<BaseDragChild>();
            getChildItem.gameObject.SetActive(true);
            SetItemShowEnable(getChildItem.transform, false);
        }
        else
        {
            getChildItem = mDisableDragChildGroup.Dequeue();
        }
        return getChildItem;
    }

    void SetItemPos(BaseDragChild dragChild,int itemDataIndex)
    {
        int posXindex = itemDataIndex % mWidthNum;
        int posYindex = itemDataIndex / mWidthNum;// +(posXindex > 0 ? 1 : 0);
        dragChild.transform.localPosition = new Vector3(posXindex * mPosWidth, -posYindex * mPosHeight);
        dragChild.mIndex = itemDataIndex;
    }

    void DragCallBack()
    {
        if (mUIScrollView != null && (mUIScrollView.currentMomentum.y != 0 || mUIScrollView.currentMomentum.x != 0))
        {
            //Debug.Log(mUIScrollView.currentMomentum);
            CheckMoveItem();
        }
    }

    void CheckMoveItem()
    {

        float upPos = -999999;
        float downPos = 99999;
        BaseDragChild upChild = null;
        BaseDragChild downChild = null;
        float leftPos = 999999;
        float rightPos = -99999;
        BaseDragChild leftChild = null;
        BaseDragChild rightChild = null;

        BaseDragChild centerChild = getCenterPosChild();
        if (centerChild == null)
            return;
        for (int i = mActiveDragChildGroup.Count - 1; i >= 0; i--)
        {
            var child = mActiveDragChildGroup[i];
            float distance = 0;
            bool isDisable = IsDisable(child, centerChild, ref distance);
            if (isDisable)
            {
                mActiveDragChildGroup.RemoveAt(i);
                //child.gameObject.SetActive(false);
                SetItemShowEnable(child.transform, false);
                mDisableDragChildGroup.Enqueue(child);
            }
            float childPosY = child.mTran.position.y;
            float childPosX = child.mTran.position.x;
            if (!IsHorizontal)
            {
                if (childPosY > upPos)
                {
                    upPos = childPosY;
                    upChild = child;
                }
                if (childPosY < downPos)
                {
                    downPos = childPosY;
                    downChild = child;
                }
            }
            else
            {
                if (childPosX < leftPos)
                {
                    leftPos = childPosX;
                    leftChild = child;
                }
                if (childPosX > rightPos)
                {
                    rightPos = childPosX;
                    rightChild = child;
                }
            }
        }
        if (!IsHorizontal)
        {
            if (IsActiveNew(upChild, centerChild, GetItemType.Up))
                OnMoveItem(GetItemType.Up);
            if (IsActiveNew(downChild, centerChild, GetItemType.Down))
                OnMoveItem(GetItemType.Down);
        }
        else
        {
            if (IsActiveNew(leftChild, centerChild, GetItemType.Left))
                OnMoveItem(GetItemType.Left);
            if (IsActiveNew(rightChild, centerChild, GetItemType.Right))
                OnMoveItem(GetItemType.Right);
        }
    }

    BaseDragChild getCenterPosChild()
    {
        float minDis = 999;
        BaseDragChild getChild = null;
        for (int i = 0; i < mActiveDragChildGroup.Count; i++)
        {
            var child = mActiveDragChildGroup[i];
            float pos1 = IsHorizontal ? child.mTran.localPosition.x : child.mTran.localPosition.y;
            float pos2 = IsHorizontal ? mUIScrollviewTran.localPosition.x : mUIScrollviewTran.localPosition.y;
            float distance = Mathf.Abs(Mathf.Abs(pos1) - Mathf.Abs(pos2));
            if (distance <= minDis)
            {
                minDis = distance;
                getChild = child;
            }
        }
        return getChild;
    }

    bool IsDisable(BaseDragChild dragChild, BaseDragChild centerChild, ref float distance)
    {
        bool isDisable= false;
        float pos1 = IsHorizontal ? dragChild.mTran.localPosition.x : dragChild.mTran.localPosition.y;
        float pos2 = IsHorizontal ? centerChild.mTran.localPosition.x : centerChild.mTran.localPosition.y;
        distance = Mathf.Abs(Mathf.Abs(pos2) - Mathf.Abs(pos1));
        if (distance > DisableItemDis)
            isDisable= true;
        return isDisable;
    }

    bool IsActiveNew(BaseDragChild dragChild, BaseDragChild centerChild,GetItemType targetMoveType)
    {
        bool isActiveNew = false;
        if (dragChild != null && centerChild != null)
        {
            float pos1 = IsHorizontal ? dragChild.mTran.localPosition.x : dragChild.mTran.localPosition.y;
            float pos2 = IsHorizontal ? centerChild.mTran.localPosition.x : centerChild.mTran.localPosition.y;
            float distance = Mathf.Abs(Mathf.Abs(pos1) - Mathf.Abs(pos2));
            if (distance < TargetActiveItemDis(targetMoveType))
                isActiveNew = true;
            
        }
        return isActiveNew;
    }

    float TargetActiveItemDis(GetItemType targetMoveType)
    {
        float getDis = ActiveItemDis;
        switch (targetMoveType)
        {
            case GetItemType.Up:
                if (mUIScrollView.currentMomentum.y < 0)
                    getDis += mPosHeight * 3;
                else
                    getDis -= mPosHeight;
                break;
            case GetItemType.Down:
                if (mUIScrollView.currentMomentum.y > 0)
                    getDis += mPosHeight * 3;
                else
                    getDis -= mPosHeight;
                break;
            case GetItemType.Left:
                if (mUIScrollView.currentMomentum.x > 0)
                    getDis += mPosWidth * 3;
                else
                    getDis -= mPosWidth;
                break;
            case GetItemType.Right:
                if (mUIScrollView.currentMomentum.x < 0)
                    getDis += mPosWidth * 3;
                else
                    getDis -= mPosWidth;
                break;
        }
        if(IsHorizontal)
            DisableItemDis = ActiveItemDis + mPosWidth;
        else
            DisableItemDis = ActiveItemDis + mPosHeight;
        return getDis;
    }

    void OnMoveItem(GetItemType getType)
    {
        if (mDisableDragChildGroup.Count == 0)
            return;
        var showIndex = GetNextShowItemIndex(getType);
        bool isUp = getType == GetItemType.Up || getType == GetItemType.Left;
        for (int i = 0; i < showIndex.Count; i++)
        {
            var child = GetBaseDragChildByDisableGroup();// mDisableDragChildGroup.Dequeue();
            SetItemShowEnable(child.transform, true);
            float newPosY = transform.localPosition.y + (isUp ? mPosYCount * mPosHeight : -mPosYCount * mPosHeight);
            child.transform.localPosition = new Vector3(i * mPosWidth, newPosY);
            SetItemPos(child, showIndex[i]);
            child.Init(mDataDic[showIndex[i]]);
            child.mDataIndex = showIndex[i];
            mActiveDragChildGroup.Add(child);
        }
    }

    List<int> GetIndexGroup = new List<int>();
    List<int> GetNextShowItemIndex(GetItemType getType)//一次只获取一行
    {
        bool getMin = getType == GetItemType.Left || getType == GetItemType.Up;
        GetIndexGroup.Clear();
        int getIndex = getMin ? 9999 : 0;
        for (int i = 0; i < mActiveDragChildGroup.Count; i++)
        {
            var child = mActiveDragChildGroup[i];
            getIndex = getMin ? (child.mIndex < getIndex ? child.mIndex : getIndex) : (child.mIndex > getIndex ? child.mIndex : getIndex);
        }
        getIndex = getMin ? (getIndex - 1) : (getIndex + 1);
        if (getMin)
        {
            while (getIndex >= 0 && (GetIndexGroup.Count < (getType == GetItemType.Up?mWidthNum:1)))
            {
                GetIndexGroup.Add(getIndex);
                getIndex--;
            }
        }
        else
        {
            int dataCount = mDataDic.Count;
            while (getIndex <= (dataCount - 1) && (GetIndexGroup.Count < (getType == GetItemType.Up ?mWidthNum:1)))
            {
                GetIndexGroup.Add(getIndex);
                getIndex++;
            }
        }
        return GetIndexGroup;
    }

    public enum GetItemType
    {
        Up,
        Down,
        Left,
        Right,
    }

}

