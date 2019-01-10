using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SG;

namespace SG
{

    //游戏逻辑管理器    
[Hotfix]
    public class MonsterTips : MonoBehaviour
    {
        public GameObject GoSprite = null;
        public GameObject RotObj  = null;
        public GameObject GoObj = null;
        public GameObject tempPos = null;
        public GameObject tempMonsterPos = null;

        Vector3 monsterPos = new Vector3(0,0,0);
        bool m_active = false;

        static MonsterTips myself = null;
        protected float lastangle = 0;
        protected float lookAtDis = 3f; // 这个3f改大了会出错，可能超出了屏幕范围

        void Awake()
        {
            myself = this;

        }

        void OnEnable()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EVENT_MONSTER_TIPS, SetMonsterTips);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FuBen_exit, OnExitFuBen);
        }

        void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_EVENT_MONSTER_TIPS, SetMonsterTips);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_FuBen_exit, OnExitFuBen);
        }

        public void OnExitFuBen(GameEvent ge, EventParameter parameter)
        {
            m_active = false;
            GoSprite.SetActive(m_active);
            RotObj.SetActive(m_active);
            gameObject.SetActive(false);
        }

        static public void SetMonsterTips(GameEvent ge, EventParameter parameter)
        {
            myself.gameObject.SetActive(true);
            myself.m_active = parameter.intParameter != 0 ? true : false;
            OneZoneData zoneData = CoreEntry.gGameDBMgr.GetAimPosZoneData(CoreEntry.gGameMgr.CurMonsterGroupId, CoreEntry.gSceneMgr.curOpenZoneID);
            if (zoneData == null)
                return;
            if (myself.m_active == false)
            {
                myself.GoSprite.SetActive(myself.m_active);
                myself.RotObj.SetActive(myself.m_active);
            }

            if (myself.m_active)
            {
                //护送特殊逻辑
                int nTaskType = CoreEntry.gGameMgr.GetCurSceneTaskType();
                if (nTaskType == 1 && CoreEntry.gGameMgr.m_CheckZoneObj != null )
                {
                    //取得护送目标
                    myself.tempMonsterPos = CoreEntry.gGameMgr.m_CheckZoneObj;
                    myself.monsterPos = CoreEntry.gGameMgr.m_CheckZoneObj.transform.position;

                    myself.GoSprite.SetActive(true);
                    myself.RotObj.SetActive(true);
                }
                else
                {
                    myself.tempMonsterPos = null;
                    myself.monsterPos = (zoneData.endPos + zoneData.startPos) * 0.5f;

                    myself.GoSprite.SetActive(true);
                    myself.RotObj.SetActive(true);
                }
            }
        }

        public void Update()
        {
            if(m_active)
            {
                if (CoreEntry.gActorMgr != null && CoreEntry.gActorMgr.MainPlayer != null)
                {
                    int nTaskType = CoreEntry.gGameMgr.GetCurSceneTaskType();
                    if (nTaskType == 1 && CoreEntry.gGameMgr.m_CheckZoneObj != null)
                        tempMonsterPos = CoreEntry.gGameMgr.m_CheckZoneObj;

                    if (tempMonsterPos != null)
                        monsterPos = tempMonsterPos.transform.position;

                    Vector3 lookRot = monsterPos - CoreEntry.gActorMgr.MainPlayer.transform.position;
                    //lookRot.y = 0;
                    if (lookRot == Vector3.zero)
                    {
                        m_active = false;
                    }
                    else
                    {
                        if (CoreEntry.gCameraMgr.MainCamera == null)
                            return;
                        // 屏幕上的中心点位置对应到一个世界坐标上
                        Vector3 screenPosOrigin = CoreEntry.gCameraMgr.MainCamera.transform.position + CoreEntry.gCameraMgr.MainCamera.transform.forward * CoreEntry.gCameraMgr.MainCamera.farClipPlane;

                        lookRot.Normalize();
                        lookRot = screenPosOrigin + lookRot * lookAtDis;
                        //Debug.DrawLine(CoreEntry.gActorMgr.MainPlayer.transform.position, lookRot, Color.green);
                        //Vector3 screenPos = Camera.main.WorldToScreenPoint(lookRot);   

                        // 按角度旋转
                        lookRot = tempPos.transform.position - RotObj.transform.position; // 计算向量
                        float angle = Mathf.Atan2(lookRot.y, lookRot.x) * Mathf.Rad2Deg;  // 计算角度

                        if (Mathf.Abs(angle - lastangle)> 15f)
                        {
                            RotObj.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1f)); // 绕Z轴旋转
                        }
                        else
                        {
                            Quaternion rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1f)); // 绕Z轴旋转
                            RotObj.transform.rotation = Quaternion.Slerp(RotObj.transform.rotation, rotation, Mathf.Clamp01(5f * Time.deltaTime));
                        }

                        // Go图标的坐标设置
                        GoSprite.transform.position = GoObj.transform.position;

                        lastangle = angle;
                    }

                }
                else
                {
                    GoSprite.SetActive(false);
                    RotObj.SetActive(false);
                    m_active = false;
                }

            }
        }

      
    }

};//end SG

