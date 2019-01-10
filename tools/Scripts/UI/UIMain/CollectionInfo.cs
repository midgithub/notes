using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SG;

[Hotfix]
public class CollectionInfo : MonoBehaviour
{

    public const string BIND_NAME = "E_top";                    //绑定节点名称

    public Text NameText;                                       //采集物名称
    public GameObject Owner;                                    //拥有的对象
    public Transform BindNode;                                  //绑定的节点
    public RectTransform SelfRT;

    void Awake()
    {
        SelfRT = transform as RectTransform;
    }

    public virtual void Init(GameObject owner)
    {
        if (owner == null)
        {
            return;
        }
        Owner = owner;
        SetBindNode(Owner, BIND_NAME);

        CollectionObj obj = Owner.GetComponent<CollectionObj>();
        if (obj == null)     //测试时
        {
            NameText.text = "test";
            return;
        }
        NameText.text = obj.Name;
    }

    //魔宠只有名称，参数传进来
    public virtual void Init(GameObject owner,string name)
    {
        if (owner == null)
        {
            return;
        }
        Owner = owner;
        SetBindNode(Owner, BIND_NAME);

        NameText.text = name;
    }
    public void SetBindNode(GameObject owner, string name)
    {
        if (owner == null)
        {
            return;
        }

        Owner = owner;
        BindNode = ActorObj.RecursiveFindChild(Owner.transform, name);
        if (BindNode == null)
        {
            LogMgr.UnityWarning(string.Format("{0} has no child {1}. Use self instead.", Owner.name, name));
            BindNode = Owner.transform;
        }
        UpdatePosition();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        if (BindNode == null || CoreEntry.gCameraMgr.MainCamera == null)
        {
            return;
        }

        Vector3 sv = CoreEntry.gCameraMgr.MainCamera.WorldToScreenPoint(BindNode.position);
        SelfRT.position = MainPanelMgr.Instance.uiCamera.ScreenToWorldPoint(sv);
    }
}

