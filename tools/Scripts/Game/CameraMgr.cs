using XLua;
﻿using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO; 
using System;


namespace SG
{
    //游戏逻辑管理器    
[Hotfix]
    public class CameraMgr : MonoBehaviour
    {
        private Camera mainCamera;
        public Camera MainCamera
        {
            get
            {
                if(null == mainCamera)
                {
                    mainCamera = Camera.main;
                }

                return mainCamera;
            }
        }

        int cameraSpeed = 8;
        float lastDistance = 12;
        float lastHeight = 12;

        IEnumerator _bossEnter(int resID)
        {
            //等待场景加载完才播boss 动画
            while (CoreEntry.bLoadSceneComplete != true)
            {
                LogMgr.UnityLog("等待场景加载完才播boss 动画 ");
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.1f);

            CoreEntry.gGameMgr.pauseGame();

            yield return new WaitForSeconds(0.5f);

            if (m_isCameraMoving == false)
            {


            }
            else
            {
                yield break;
            }

            yield return new WaitForSeconds(3.2f);
            CoreEntry.gGameMgr.resumeGame();

        }

        bool m_isCameraMoving = false;
        //镜头移动到目标 怪物 
        public void moveToTarget(int resID)
        {
            //StopAllCoroutines(); 
            StartCoroutine(_moveToTarget(resID));

        }

        //镜头移回到玩家身上
        public void moveToPlayer()
        {
            StartCoroutine(_moveToPlayer());
        }


        GameObject m_bossObj;
        public IEnumerator _moveToTarget(int resID)
        {
            //等待场景加载完才播boss 动画
            while (CoreEntry.bLoadSceneComplete != true)
            {
                LogMgr.UnityLog("_moveToTarget 等待场景加载完才播boss 动画 ");
                yield return new WaitForSeconds(0.1f);
            }


            ActorObj actorBase = CoreEntry.gActorMgr.GetActorByConfigID(resID);
            if (actorBase == null)
            {
                LogMgr.UnityError("找不到怪物ID：" + resID);
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_end, null);
                yield break;
            }
            IActorState state = actorBase.GetActorState(ACTOR_STATE.AS_ENTER);
            if (state != null)
            {
                //state.Reset();
                try
                {
                    EnterState eState = state as EnterState;
                    eState.EndEnterStateFromCamera(actorBase);
                }
                catch (Exception e)
                {
                    LogMgr.UnityError(e.ToString());  //状态没有初始化
                }

            }


            CameraFollow cameraFollow = null;
            if (null != MainCamera)
            {
                cameraFollow = MainCamera.GetComponent<CameraFollow>();
            }
            if (cameraFollow == null)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_end, null);
                yield break;
            }

            lastDistance = cameraFollow.m_distance;
            lastHeight = cameraFollow.m_height;

            CoreEntry.gGameMgr.pauseGame();

            yield return new WaitForSeconds(0.6f);


            //TweenAttr2.SetAttr fnSetHeight = new TweenAttr2.SetAttr(cameraFollow.SetHeight);
            //TweenAttr.SetAttr fnSetDistance = new TweenAttr.SetAttr(cameraFollow.SetDistance);


            float distance = cameraFollow.getDistance(m_bossObj.transform);
            if (distance < 10)
            {
                cameraSpeed = 4;
            }
            else if (distance < 20)
            {
                cameraSpeed = 6;
            }
            LogMgr.UnityLog("镜头速度调整为:" + cameraSpeed);

            yield return new WaitForSeconds(1.9f);

            //通知剧情， 镜头已经移动到目标
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_Focus, null);

        }

        public IEnumerator _moveToPlayer()
        {
            CoreEntry.gGameMgr.pauseGame();


            CameraFollow cameraFollow = null;
            if (null != MainCamera)
            {
                cameraFollow = MainCamera.GetComponent<CameraFollow>();
            }
            if (cameraFollow == null)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_end, null);
                m_isCameraMoving = false;
                CoreEntry.gGameMgr.resumeGame();
                yield break;
            }
            cameraFollow.m_target = CoreEntry.gActorMgr.MainPlayer.transform;

            yield return new WaitForSeconds(0.1f);

            float time = 0.5f;
            float count = 0;
            float height = cameraFollow.m_height;
            float distance = cameraFollow.m_distance;
            float angle = cameraFollow.m_rotationAngle;
            while (angle < -180) { angle += 360; }
            while (angle >= 180) { angle -= 360; }
            while (count < time)
            {
                float t = count / time;
                cameraFollow.SetHeight(Mathf.Lerp(height, CameraFollow.CurHeight, t));
                cameraFollow.SetDistance(Mathf.Lerp(distance, cameraFollow.m_initDistance, t));
                cameraFollow.SetRotationY(Mathf.Lerp(angle, CameraFollow.CurRotationAngle, t));
                count += Time.deltaTime;
                yield return null;
            }
            cameraFollow.SetHeight(CameraFollow.CurHeight);
            cameraFollow.SetDistance(cameraFollow.m_initDistance);
            cameraFollow.SetRotationY(CameraFollow.CurRotationAngle);

            yield return new WaitForSeconds(0.1f);
            CoreEntry.gGameMgr.resumeGame();

            //通知剧情， 镜头已经移回
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_end, null);
            m_bossObj = null;
            m_isCameraMoving = false;
        }
        

        //镜头移回到玩家身上
        public void FastMoveToTarget(int resID)
        {
            StartCoroutine(_FastMoveToTarget(resID));
        }

        GameObject m_targetObj;
        public IEnumerator _FastMoveToTarget(int cfgID)
        {
            ActorObj actorBase = CoreEntry.gActorMgr.GetActorByConfigID(cfgID);
            if (actorBase == null)
            {
                LogMgr.UnityError("找不到怪物ID：" + cfgID);
                yield break;
            }


            CameraFollow cameraFollow = null;
            if (null != MainCamera)
            {
                cameraFollow = MainCamera.GetComponent<CameraFollow>();
            }
            if (cameraFollow == null)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_end, null);
                yield break;
            }

            lastDistance = cameraFollow.m_distance;
            lastHeight = cameraFollow.m_height;

            CoreEntry.gGameMgr.pauseGame();

            yield return new WaitForSeconds(0.6f);


            //TweenAttr2.SetAttr fnSetHeight = new TweenAttr2.SetAttr(cameraFollow.SetHeight);
            //TweenAttr.SetAttr fnSetDistance = new TweenAttr.SetAttr(cameraFollow.SetDistance);

            float distance = cameraFollow.getDistance(actorBase.transform);
            if (distance < 10)
            {
                cameraSpeed = 4;
            }
            else if (distance < 20)
            {
                cameraSpeed = 6;
            }
            LogMgr.UnityLog("镜头速度调整为:" + cameraSpeed);

            //播放动作
            yield return new WaitForSeconds(1.9f);

            //通知剧情， 镜头已经移动到目标
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Camera_Focus, null);

        }
    }
}

