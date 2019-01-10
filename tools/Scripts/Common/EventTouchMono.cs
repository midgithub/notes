using UnityEngine;
using SG;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XLua;

[Hotfix]
public class EventTouchMono : MonoBehaviour
{

    DateTime dt = DateTime.Now;
    bool bMove = true;

    LayerMask mClickMask = ~((1 << LayerMask.NameToLayer("ignore")) | (1 << LayerMask.NameToLayer("tree")));
    public float thetaSpeed = 120.0f;
    public float phiSpeed = 120.0f;
    public float moveSpeed = 10.0f;
    public float zoomSpeed = 3.0f;

    public float phiBoundMin = -90;
    public float phiBoundMax = 90f;

    protected Vector3 oldPostion1 = Vector3.zero;
    protected Vector3 oldPostion2 = Vector3.zero;


    bool bCollectFinish = true;

    bool cancelAuto = false;

    void Update()
    {
        if (CoreEntry.gCameraMgr.MainCamera == null)
        {
            return;
        }
        if (CoreEntry.gJoystickMgr.IsTouch())
        {
            cancelAuto = false;
            //   ChangeMouseTexture(TargetType.none);
            return;
        }
        if (targetObj != null)
        {
            if (!NpcMgr.Instance.bInArea(targetObj.gameObject.transform.position, 10))
            {
                targetObj.SelectNpc(targetObj, false);
                targetObj = null;
            }
        }
        if ((DateTime.Now - dt).TotalSeconds < 1)
        {
            return;
        }
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    cancelAuto = false;
                    return;
                }
            }
        }
        //Button ss;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
#else
        int count = Input.touchCount;
        if (count <= 0)
            return;
        Touch touch = Input.GetTouch(0);
        if (TouchPhase.Began == touch.phase)
#endif
        {

#if IPHONE || ANDROID
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                cancelAuto = false;
                return;
            }		
#else
            if (EventSystem.current.IsPointerOverGameObject())
            {
                cancelAuto = false;
                //过滤穿透
                return;
            }
#endif

            //UnityEngine.UI.GraphicRaycaster gray = this.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            Ray ray = CoreEntry.gCameraMgr.MainCamera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;
            //TargetType curTargetType = TargetType.none;  //初始化点击光标显示类型

            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, mClickMask))
            {
                //                LogMgr.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到

                GameObject gameObj = hitInfo.collider.gameObject;
                if (!gameObj.CompareTag("Untagged"))
                {
                    cancelAuto = false;
                    //LogMgr.UnityLog("click object name is " + gameObj.name);
                    //LogMgr.UnityLog("gameObj.tag   " + gameObj.tag);    //obj  tag层
                    if (gameObj.tag.Equals("npc") || gameObj.tag.Equals("collect")
                        || gameObj.tag.Equals("monster") || gameObj.tag.Equals("player") || gameObj.tag.Equals("boss"))
                    {
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_MyTakeAction, EventParameter.Get());

                        //int distance = 0;
                        //int campId = 0;
                        if (gameObj.tag.Equals("npc"))
                        {
                            //curTargetType = TargetType.handerType1;
                            int cid = gameObj.GetComponent<NpcObj>().ConfigID;
                            //npcConfig cf = npcExcel.GetRecord<npcConfig>(cid);
                            LuaTable cfg = ConfigManager.Instance.Actor.GetNPCConfig(cid);
                            if (cfg == null)
                            {
                                LogMgr.LogError("NPC配置表中不存在 resid:  " + cid);
                                return;
                            }
                            if (cfg.Get<int>("open_dialog") < 1)   //该NPC不能进行对话
                            {
                                return;
                            }
                            //distance = cfg.Get<int>("open_dis");
                            //campId = cfg.Get<int>("camp");
                        }
                        if (gameObj.tag.Equals("collect"))
                        {
                            //curTargetType = TargetType.handerType1;
                            CollectionType mType = gameObj.GetComponent<CollectionObj>().Type;
                            if (mType == CollectionType.ME || mType == CollectionType.FRIEND)
                            {
                                //公会战判断。 己方军旗
                                return;
                            }
                            int cid = gameObj.GetComponent<CollectionObj>().ConfigID;
                            long serverId = gameObj.GetComponent<CollectionObj>().ServerID;
                            LuaTable cfg = ConfigManager.Instance.Map.GetCollectionConfig(cid);
                            if (cfg == null)
                            {
                                LogMgr.LogError("采集配置表中不存在 resid:  " + cid);
                                return;
                            }
                            if(cfg.Get<int>("click") != 1)
                            {
                                return;
                            }

                            if(NpcMgr.Instance.bInArea(gameObj.transform.position, cfg.Get<int>("distance")) == false)
                            {
                                Debug.Log("距离不够");
                                return;
                            }

                            if (bCollectFinish)
                            {
                                int num = cfg.Get<int>("cast_time");
                                bCollectFinish = false;

                                MsgData_cStructDef rsp = new MsgData_cStructDef();
                                rsp.cID = serverId;
                                if (rsp.cID > 0)
                                {
                                    LogMgr.Log("发送采集请求rsp.cID   " + rsp.cID);
                                    CoreEntry.netMgr.send((Int16)NetMsgDef.C_STRUCT_DEF, rsp);
                                }
                                //TaskMgr.Instance.Send_CS_CollectData(cid);
                                UITips.ShowSliderProgress(cfg.Get<string>("tips"), num, () => {
                                    bCollectFinish = true;
                                });
                                gameObj.GetComponent<CollectionObj>().SelectTarget();


                            }/*
                            Debug.LogError("采集cid  "+ cid);
                            UITips.ShowSliderProgress("采集中", cfg.Get<int>("cast_time")/1000,() => {
                                Debug.LogError("11111");
                            });
                            */
                        }
                        if (gameObj.tag.Equals("monster") || gameObj.tag.Equals("boss") || gameObj.tag.Equals("collect"))
                        {
                            ActorObj monster = gameObj.GetComponent<ActorObj>();
                            ActorObj mainplayer = CoreEntry.gActorMgr.MainPlayer;
                            if (monster != null)
                            {
                                if (mainplayer.m_SelectTargetObject == monster)
                                {
                                    LogMgr.DebugLog("点击选中同一个目标");
                                    CoreEntry.gAutoAIMgr.AutoFight = true;
                                }
                                else
                                {
                                    if((monster is MonsterObj) && monster.IsSameCamp())
                                    {
                                        //同阵营的怪物不能被选中！
                                    }else
                                    {
                                        mainplayer.SelectTarget(monster);
                                    }
                                }
                            }
                            return;
                        }

                        //PK选择目标
                        if (gameObj.tag.Equals("player"))
                        {
                            ActorObj mainplayer = CoreEntry.gActorMgr.MainPlayer;
                            OtherPlayer player = gameObj.GetComponent<OtherPlayer>();
                            if (player != null)
                            {
                                mainplayer.SelectTarget(player);
                                return;
                            }
                        }

                        dt = DateTime.Now;  //过滤多次发送。
                        //     if (TaskMgr.Instance.bInArea(hitInfo.point,distance))   //cf.open_dis 对话半径

                        if (NpcMgr.Instance.bInArea(hitInfo.point, 3))
                        {
                            if (gameObj.tag.Equals("npc"))
                            {
                                targetObj = gameObj.GetComponent<ActorObj>();
                                NpcMgr.Instance.OpenNpcTk(targetObj.ConfigID);
                                //LuaTable camp = ConfigManager.Instance.Common.GetCampConfig(campId);

                                //ActorObj mainplayer = CoreEntry.gActorMgr.MainPlayer;

                                //if (targetObj != null)
                                //{
                                //    if (mainplayer.m_SelectTargetObject == targetObj)
                                //    {
                                //        LogMgr.LogError("点击选中同一个目标");
                                //        CoreEntry.gAutoAIMgr.AutoFight = true;
                                //    }
                                //    else
                                //    {
                                //        if ((targetObj is ActorObj) && targetObj.IsSameCamp())
                                //        {
                                //            //同阵营的怪物不能被选中！
                                //        }
                                //        else
                                //        {
                                //            mainplayer.SelectTarget(targetObj);
                                //        }
                                //    }
                                //}

                               targetObj.SetTarget(targetObj);
                               targetObj.SelectNpc(targetObj, true);
                            }
                            else if (gameObj.tag.Equals("collect"))
                            {

                            }
                            else if (gameObj.tag.Equals("player"))
                            {

                            }
                            else
                            {

                            }

                        }
                        else
                        {
                            if (bMove)
                            {
                                Vector3 go = hitInfo.point;
                                LogMgr.Log("距离不够，点击移动");
                                TaskMgr.Instance.MoveToPos(go);
                                //                               bMove = false;
                            }
                        }
                    }
                }
                else
                {
                    if (gameObj.layer == LayerMask.NameToLayer("ground") && CoreEntry.gJoystickMgr.IsShow && !ArenaMgr.Instance.IsArenaFight)
                    {
                        int mapid = MapMgr.Instance.EnterMapId;
                        var info = ConfigManager.Instance.Map.GetMapConfig(mapid);
                        if (info != null && info.GetInPath<int>("type") == 42)//如果是竞技场 不给人物行走
                        {
                            return;
                        }
                        Vector3 pos = hitInfo.point;
                        ActorObj mainActor = CoreEntry.gActorMgr.MainPlayer;
                        if (!SceneDataMgr.Instance.IsBlocked(pos) && null != mainActor)
                        {
                            if (mainActor.GetCurState() == ACTOR_STATE.AS_STAND ||
                                mainActor.GetCurState() == ACTOR_STATE.AS_RUN ||
                                mainActor.GetCurState() == ACTOR_STATE.AS_NONE)
                            {
                                if (mainActor.AutoPathFind && !cancelAuto)
                                {
                                    cancelAuto = true;
                                    UITips.ShowTips("再次点击取消自动寻路");

                                    return;
                                }

                                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_MyTakeAction, EventParameter.Get());

                                //CoreEntry.gGameMgr.AutoFight = false;
                                TaskMgr.bRunAndTasking = false;
                                CoreEntry.gActorMgr.MainPlayer.ShowMoveArrow(pos);
                                CoreEntry.gActorMgr.MainPlayer.MoveToPos(pos);
                            }
                        }
                    }
                }
                //targetType = curTargetType;
            }

        }

        //    ChangeMouseTexture(targetType);    //更改鼠标光标
    }

    /// <summary>
    /// 触碰的obj
    /// </summary>
    ActorObj targetObj;



    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    ///// <summary>
    ///// 触碰obj 的鼠标光标显示类型
    ///// </summary>
    //TargetType targetType = TargetType.none;

    void ChangeMouseTexture(TargetType type)
    {
        Texture2D pic = ChooseSelectFx(type);
        Cursor.SetCursor(pic, Vector2.zero, CursorMode.Auto);
    }


    /// <summary>
    /// 设置光标图片
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    Texture2D ChooseSelectFx(TargetType type)
    {
        Texture2D pic = null;
        Sprite sprite = null;
        switch (type)
        {
            case TargetType.none:
                break;
            case TargetType.handerType1:
                sprite = AtlasSpriteManager.Instance.GetSprite("drop:2");
                pic = sprite.texture;
                break;
            case TargetType.handerType2:
                break;
            default:
                break;
        }
        return pic;
    }

    /// <summary>
    /// 触碰类型
    /// </summary>
    enum TargetType
    {
        none,
        handerType1, //鼠标图片显示 - 采集类型
        handerType2,
    }

[Hotfix]
    public class CheckUGUI : MonoBehaviour
    {



        public static bool CheckGuiRaycastObjects(EventSystem eventSystem, GraphicRaycaster graphicRayCaster)

        {

            PointerEventData eventData = new PointerEventData(eventSystem);

#if UNITY_EDITOR

            eventData.pressPosition = Input.mousePosition;

            eventData.position = Input.mousePosition;

#endif

#if UNITY_ANDROID || UNITY_IPHONE

            if (Input.touchCount > 0)

            {

                eventData.pressPosition = Input.GetTouch(0).position;

                eventData.position = Input.GetTouch(0).position;

            }

#endif

            List<RaycastResult> list = new List<RaycastResult>();

            graphicRayCaster.Raycast(eventData, list);

            return list.Count > 0;

        }        
    }
}

