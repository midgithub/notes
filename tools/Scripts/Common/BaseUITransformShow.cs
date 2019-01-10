using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class BaseUITransformShow : MonoBehaviour {
    public enum PanelRenderStatus { None,Active,Disable}


    Dictionary<int, List<Transform>> TempObjGroup = new Dictionary<int, List<Transform>>();
    List<UIPanel> mPanelGroup = new List<UIPanel>();
    List<UIWidget> mWidgetGroup = new List<UIWidget>();
    //List<GameObject> GetObjGroup = new List<GameObject>();

    public PanelRenderStatus mPanelRenderStatus = PanelRenderStatus.None;

    public bool IsRenderActive { get { return mPanelRenderStatus== PanelRenderStatus.Active; } }

    public void SetRendereActive(bool flag)
    {
        if (!IsSetRender(flag))
            return;
        if (flag && !gameObject.activeSelf)
            gameObject.SetActive(true);
        transform.SetRenderActive(flag);
        StopCoroutine("SetUIUpdateActiveForFarm");
        if(gameObject.activeSelf)
            StartCoroutine(SetUIUpdateActiveForFarm(flag));

        mPanelRenderStatus = flag ? PanelRenderStatus.Active : PanelRenderStatus.Disable;

        if (flag)
            OnRenderEnable();
        else
            OnRenderDisable();
    }

    bool IsSetRender(bool flag)
    {
        bool isSet = false;
        switch (mPanelRenderStatus)
        {
            case PanelRenderStatus.None:
                isSet = true;
                break;
            case PanelRenderStatus.Active:
                isSet = flag == false;
                break;
            case PanelRenderStatus.Disable:
                isSet = flag == true;
                break;
        }
        return isSet;
    }

    public virtual void OnRenderEnable()
    {
    }

    public virtual void OnRenderDisable()
    { 
    }

    IEnumerator SetUIUpdateActiveForFarm(bool flag)
    {
        if (!flag)
            yield return StartCoroutine(GetComponentGroup());
        for (int i = 0; i < mPanelGroup.Count; i++)
        {
            if (mPanelGroup[i] != null)
                mPanelGroup[i].enabled = flag;
        }
        for (int i = 0; i < mWidgetGroup.Count; i++)
        {
            if (mWidgetGroup[i] != null)
                mWidgetGroup[i].IsEnableUpdate = flag;
        }
        //Debug.Log("SetItemRendere");
    }

    IEnumerator GetComponentGroup()
    {
        if (mPanelGroup.Count == 0)
            yield return StartCoroutine(GetComponentGroupBySync(transform));
    }

    IEnumerator GetComponentGroupBySync(Transform parent)
    {
        int levelIndex = 1;
        TempObjGroup[levelIndex] = new List<Transform>();
        foreach (Transform child in parent)
        {
            TempObjGroup[levelIndex].Add(child);
        }
        bool flag = true;
        while (flag)
        {
            List<Transform> tranGroup = new List<Transform>();
            foreach (Transform child in TempObjGroup[levelIndex])
            {
                if (child!=null)
                    foreach (Transform childtran in child.transform)
                    {
                        tranGroup.Add(childtran);
                        UIPanel mPanel = childtran.GetComponent<UIPanel>();
                        UIWidget mWidget = childtran.GetComponent<UIWidget>();
                        if (mPanel != null && !mPanelGroup.Contains(mPanel))
                            mPanelGroup.Add(mPanel);
                        if (mWidget != null && !mWidgetGroup.Contains(mWidget))
                            mWidgetGroup.Add(mWidget);
                    }
            }
            levelIndex++;
            TempObjGroup[levelIndex] = tranGroup;
            yield return null;
            if (tranGroup.Count == 0)
                flag = false;
        }
    }

}

