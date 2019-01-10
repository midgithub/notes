/**
* @file     : MulScenesPathFinder
* @brief    : 多场景/跨场景寻路
* @details  : 多场景/跨场景寻路
* @author   : CW
* @date     : 2017-07-10
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;

namespace SG
{
[Hotfix]
    public class MulScenesPathFinder 
    {
        private static MulScenesPathFinder _instance = null;
        public static MulScenesPathFinder Instance
        {
            get
            {
                if (null == _instance)
                {
                    return _instance = new MulScenesPathFinder();
                }
                return _instance;
            }
        }

        private bool bUserMul;
        private int curMapID;
        private int curIndex;
        private int targetMapID;
        private Vector3 targetPostion;
        private List<int> mMapIDs = new List<int>();
        private List<SceneEntityConfig> mPortals = new List<SceneEntityConfig>();

        private MulScenesPathFinder()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_LOADSCENE_FINISH, OnSceneLoaded);
        }

        /// <summary>
        /// 寻路目标ID
        /// </summary>
        public int TargetMapID
        {
            get { return targetMapID; }
        }

        public Vector3 TargetPosition
        {
            get { return targetPostion; }
        }
        /// <summary>
        /// 跨场景寻路
        /// </summary>
        /// <param name="destMapID">目的地图ID</param>
        /// <param name="destPosition">目的位置</param>
        public void StartPathFinder(int destMapID, Vector3 destPosition)
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_JOYSTICK_DOWN, OnJoyStickDown);
            bUserMul = false;
            curMapID = MapMgr.Instance.EnterMapId;
            targetMapID = destMapID;
            targetPostion = destPosition;

            ActorObj ag = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
            if (destMapID == curMapID)//同一个地图直接走过去
            {
                ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
                actor.MoveToPos(destPosition);

                
                ag.ReqRideHorse();

                return;
            }

            if (AnalysePath(curMapID, destMapID))
            {
                CoreEntry.gEventMgr.AddListener(GameEvent.GE_JOYSTICK_DOWN, OnJoyStickDown);//开始跨场景寻路才需要简体摇杆按下时间，判断是否打断

                curIndex = 0;

                ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
                SceneEntityConfig curPortal = mPortals[curIndex];

                bUserMul = actor.MoveToPos(curPortal.position);
                ag.ReqRideHorse();
            }
        }

        /// <summary>
        /// 分析从起始场景到目的场景是否可达
        /// </summary>
        /// <param name="startMapID"></param>
        /// <param name="destMapID"></param>
        /// <returns>是否可达</returns>
        public bool AnalysePath(int startMapID, int destMapID)
        {
            HashSet<int> closeMapSet = new HashSet<int>();
            List<MapNodeInfo> allNodes = new List<MapNodeInfo>();
            Queue<MapNodeInfo> checkNodes = new Queue<MapNodeInfo>();

            MapNodeInfo curNode = new MapNodeInfo();
            curNode.id = startMapID;
            curNode.parent = null;
            curNode.portalCfg = null;

            allNodes.Add(curNode);
            checkNodes.Enqueue(curNode);

            curNode = null;
            while(checkNodes.Count > 0)
            {
                curNode = checkNodes.Dequeue();
                closeMapSet.Add(curNode.id);

                if(curNode.id == destMapID)
                {
                    break;
                }

                SceneEntitySet portals = GetPortalByMapID(curNode.id);
                if(null != portals)
                {
                    for(int i = 0;i < portals.entityList.Count;i++)
                    {
                        SceneEntityConfig portal = portals.entityList[i];
                        LuaTable portalCfg = ConfigManager.Instance.Map.GetPortalConfig(portal.configID);
                        if(null == portalCfg)
                        {
                            continue;
                        }
                        if(closeMapSet.Contains(portalCfg.Get<int>("targetMap")))
                        {
                            continue;
                        }

                        MapNodeInfo mapNodeInfo = new MapNodeInfo();
                        mapNodeInfo.id = portalCfg.Get<int>("targetMap");
                        mapNodeInfo.parent = curNode;
                        mapNodeInfo.portalCfg = portal;

                        checkNodes.Enqueue(mapNodeInfo);
                        allNodes.Add(mapNodeInfo);

                    }
                }
            }

            mMapIDs.Clear();
            mPortals.Clear();

            if(null != curNode && curNode.id == destMapID)
            {
                mMapIDs.Add(curNode.id);
                mPortals.Add(curNode.portalCfg);

                MapNodeInfo nextNode = curNode.parent;
                while (null != nextNode)
                {
                    mMapIDs.Add(nextNode.id);
                    if (nextNode.portalCfg != null)
                    {
                        mPortals.Add(nextNode.portalCfg);
                    }

                    nextNode = nextNode.parent;
                }

                mMapIDs.Reverse();
                mPortals.Reverse();

                return true;
            }

            return false;
        }

        /// <summary>
        /// 根据地图ID获取传送门配置信息
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        private SceneEntitySet GetPortalByMapID(int mapID)
        {
            List<SceneEntitySet> entities = CoreEntry.gGameDBMgr.GetEnityConfigInfo(mapID);
            if(null == entities)
            {
                return null;
            }
            
            for(int i = 0;i < entities.Count;i++)
            {
                if(entities[i].type == EntityConfigType.ECT_PORTAL)
                {
                    return entities[i];
                }
            }

            return null;
        }

        private void OnSceneLoaded(GameEvent ge, EventParameter paremeter)
        {
            if (bUserMul)
            {
                curMapID = MapMgr.Instance.EnterMapId;
                curIndex++;
                if (curIndex < mPortals.Count)//中间场景，走到传送门位置
                {
                    ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
                    if (null != actor)
                    {
                        SceneEntityConfig curPortal = mPortals[curIndex];
                        bUserMul = actor.MoveToPos(curPortal.position);
                    }
                }
                else
                {
                    if (curMapID == targetMapID)//目的场景，走到终点
                    {
                        ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
                        if (null != actor)
                        {
                            actor.MoveToPos(targetPostion);
                        }
                    }

                    bUserMul = false;
                    CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_JOYSTICK_DOWN, OnJoyStickDown);
                }

                //if (TaskMgr.LastClickTaskID > 0)
                //{
                //    AutoAIRunner.Init();
                //    AutoAIRunner.SetAutoTask(true);
                //}
            }
        }

        private void OnJoyStickDown(GameEvent ge, EventParameter parameter)
        {
            bUserMul = false;//切换场景后不再自动寻路
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_JOYSTICK_DOWN, OnJoyStickDown);
        }

        public void CancelMove()
        {
            bUserMul = false;
        }
    }

[Hotfix]
    class MapNodeInfo
    {
        public int id;
        public SceneEntityConfig portalCfg;
        public MapNodeInfo parent;
    }
}

