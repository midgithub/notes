using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine.UI;

[Hotfix]
public class HeadTypeLogo : MonoBehaviour {

    [SerializeField]
    GameObject logoPrefab;
    [SerializeField]
    GameObject grid;

    public static HeadTypeLogo Instance;
    
    /// <summary>
    /// key: serverId  value:头顶标识
    /// </summary>
    public Dictionary<long, HeadLogoItem> serverByScripts = new Dictionary<long, HeadLogoItem>();

    void Awake()
    {
        Instance = this;
    }
	// Use this for initialization
	void Start () {
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_TaskUpdate, OnTaskEventUpdate);
       // CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_SceneCreatModelUpdate, OnModelEventUpdate);
        CreatLogo();
    }
    void CreatLogo()
    {
        serverByScripts.Clear();
        List<ActorObj> alls = CoreEntry.gActorMgr.GetAllNPCActors();

        foreach (var item in alls)
        {
            if(item != null)
            {
                Transform pointNode = item.transform.Find("E_top");
                GameObject p = Instantiate(logoPrefab) as GameObject;
                p.SetActive(true);
                p.gameObject.transform.SetParent(pointNode);
                p.gameObject.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
                p.gameObject.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                p.transform.SetParent(grid.transform);
                p.GetComponent<HeadLogoItem>().Set(item, item.ConfigID, item.ServerID);
                //调整方向
                //            index++;
                serverByScripts[item.ServerID] = p.GetComponent<HeadLogoItem>();
            }
        }
    }
      
   
    /// <summary>
    /// 任务通知刷新头顶icon
    /// </summary>
    /// <param name="ge"></param>
    /// <param name="parameter"></param>
    void OnTaskEventUpdate(GameEvent ge, EventParameter parameter)
    {
        foreach (var item in serverByScripts)
        {
            item.Value.RefreshShowState();
        }
    }

    /// <summary>
    /// 场景模型增加通知
    /// </summary>
    /// <param name="obj"></param>
    public void OnModelEventUpdate(ActorObj obj)
    {
//        ActorObj obj = parameter.objParameter as ActorObj;
        if (obj is NpcObj)
        {
            if (!serverByScripts.ContainsKey(obj.ServerID))
            {
                Transform pointNode = obj.transform.Find("E_top");
                GameObject p = Instantiate(logoPrefab) as GameObject;
                p.SetActive(true);
                p.gameObject.transform.SetParent(pointNode);
                p.gameObject.transform.GetComponent<RectTransform>().position = pointNode.position;
                p.gameObject.transform.GetComponent<RectTransform>().localScale = Vector3.one;
                p.transform.SetParent(grid.transform);
                p.transform.SetParent(grid.transform);
                HeadLogoItem head = p.GetComponent<HeadLogoItem>();
                head.Set(obj, obj.ConfigID, obj.ServerID);
                serverByScripts[obj.ServerID] = head;
            }else
            {
                serverByScripts[obj.ServerID].gameObject.SetActive(true);
                serverByScripts[obj.ServerID].Set(obj, obj.ConfigID, obj.ServerID);
            }
        }        
    }
    /// <summary>
    /// 场景模型销毁通知
    /// </summary>
    /// <param name="obj"></param>
    public void OnModelRemoveUpdate(ActorObj obj)
    {
        if(serverByScripts.ContainsKey(obj.ServerID))
        {
//            serverByScripts[obj.ServerID].Remove();
//            serverByScripts.Remove(obj.ServerID);
            serverByScripts[obj.ServerID].gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CC_TaskUpdate, OnTaskEventUpdate);
        //  CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CC_TaskUpdate, OnModelEventUpdate);
    }
}

