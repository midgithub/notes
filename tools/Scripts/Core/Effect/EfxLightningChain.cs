using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
public class EfxLightningChain : MonoBehaviour {
    struct LinePlayData
    {
        public GameObject frist;
        public GameObject last ;
        public GameObject line ;
        public float startTime ;
        public float endTime ;

    }

    //动作对应的特效
    LightningChainCell lightningCell = null;
    List<GameObject> list = null;
    List<LinePlayData> listPlay = new List<LinePlayData>();

    void OnEnable()
    {
        list = null;

        //Init(targetList2);
    }

    void OnDisable()
    {
        list = null;
    }

    void ClearAll()
    {
        for (int i = 0; i < listPlay.Count; i++)
        {
            //Destroy(listPlay[i].line);
            CoreEntry.gGameObjPoolMgr.Destroy(listPlay[i].line);
        }
        listPlay.Clear();
    }

	// Update is called once per frame
	void LateUpdate ()
    {
        for (int i = 0; i < listPlay.Count; i++)
        {
            // 结束
            if (Time.time - listPlay[i].endTime > 0 && listPlay[i].line.activeInHierarchy )
            {
                listPlay[i].line.SetActive(false);
            }
            else
            if (Time.time - listPlay[i].startTime > 0)
            {
                if (Time.time - listPlay[i].endTime < 0 && !listPlay[i].line.activeInHierarchy)
                {
                    // 开始显示,伤害判定
                    listPlay[i].line.SetActive(true);
                    if (lightningCell!=null)
                        lightningCell.CalculateDamage(listPlay[i].last);
                }
                // 更新坐标
                ActorObj actorBase1 = listPlay[i].frist.GetComponent<ActorObj>();
                ActorObj actorBase2 = listPlay[i].last.GetComponent<ActorObj>();
                //位置改为胸口挂点
                Vector3 actorPos1 = actorBase1.ChestSocket.position;// new Vector3(listPlay[i].frist.transform.position.x, listPlay[i].frist.transform.position.y + actorObj1.GetColliderRadius() * 1.6f, listPlay[i].frist.transform.position.z); ;
                Vector3 actorPos2 = actorBase2.ChestSocket.position;// new Vector3(listPlay[i].last.transform.position.x, listPlay[i].last.transform.position.y + actorObj2.GetColliderRadius() * 1.6f, listPlay[i].last.transform.position.z); ;

                listPlay[i].line.transform.parent   = transform;
                listPlay[i].line.transform.position = actorPos1;
                listPlay[i].line.transform.rotation = Quaternion.identity;

                LineRenderer line = listPlay[i].line.GetComponent<LineRenderer>();
                if (line == null)
                    continue;
                line.SetVertexCount(2);
                line.SetPosition(0, Vector3.zero);
                line.SetPosition(1, actorPos2 - actorPos1);
            }
        }

	}

//    public void Init(GameObject[] list)
    public void Init(LightningChainCell cell, List<GameObject> list2)
    {
        lightningCell = cell;
        list = list2;
        ClearAll();

        for (int i = 0; i < list.Count-1; i++)
        {
            LinePlayData playdata;
            playdata.frist = list[i];
            playdata.last = list[i + 1];
            //playdata.line = Instantiate(CoreEntry.gResLoader.LoadResource("Effect/scence/fx_shandian")) as GameObject;//CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/scence/fx_shandian");
            playdata.line = CoreEntry.gGameObjPoolMgr.InstantiateEffect("Effect/scence/fx_shandian");
            playdata.startTime = Time.time + i*0.3f;
            playdata.endTime   = Time.time + i*0.3f + 0.75f;
            if (playdata.line != null)
            {
                playdata.line.SetActive(false);
            }
            listPlay.Add(playdata);
        }

    }

    //脱离Obj，原地播放
    public void DetachObject()
    {
        ClearAll();
    }

}

}

