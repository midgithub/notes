using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
using UnityEngine.UI;
using System.Collections.Generic;

[Hotfix]
public class HeadLogoItem : MonoBehaviour
{


    public ActorObj obj;
    public int configId;
    public long serverId;
    Transform pointNode;

    [SerializeField]
    GameObject wenhao;
    [SerializeField]
    GameObject wenhao_gray;


    // Use this for initialization
    void Start()
    {

    }

    public void Set(ActorObj tt, int _id, long _sId)
    {
        obj = tt;
        configId = _id;
        serverId = _sId;
        pointNode = tt.transform.Find("E_top");
        this.gameObject.name = _id.ToString();
        //this.gameObject.transform.SetParent(pointNode);
        this.gameObject.transform.GetComponent<RectTransform>().position = pointNode.position;
        //   this.gameObject.transform.GetComponent<RectTransform>().scal = Vector3.one;
        RefreshShowState();

    }
    public void RefreshShowState()
    {
        if (TaskMgr.Instance.CheckNpcWithTaskState(configId) == 2)
        {
            wenhao.SetActive(true);
        }
        else
        {
            wenhao.SetActive(false);
        }
    }

    public void Remove()
    {
        DestroyImmediate(this);
    }
    private void Update()
    {
        if (this.gameObject.activeSelf && pointNode != null)
        {
            this.gameObject.transform.GetComponent<RectTransform>().position = pointNode.position;
        }
    }

    private void LateUpdate()
    {
        if (pointNode == null || CoreEntry.gCameraMgr.MainCamera == null)
        {
            return;
        }

        //Vector3 sv = Camera.main.WorldToScreenPoint(pointNode.position);
    }
}

