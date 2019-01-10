/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

[Hotfix]
    public class DropMgr : IModule
    {
        //----------- 每个管理器必须写的方法 ----------
        public override bool LoadSrv(IModuleServer IModuleSrv)
        {
            ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
            moduleSrv.GDropMgr = this;

            return true;
        }

        public override void InitializeSrv()
        {
            m_ResourceLoader = CoreEntry.gResLoader;
            m_EventMgr = CoreEntry.gEventMgr;

            m_EventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnSceneLoaded);
            m_EventMgr.AddListener(GameEvent.GE_OBJ_ITEM_ENTER, OnItemEnter);
            m_EventMgr.AddListener(GameEvent.GE_OBJ_ITEM_LEAVE, OnItemLeave);
            m_EventMgr.AddListener(GameEvent.GE_ITEM_PICKUP, OnItemPickUp);
            m_EventMgr.AddListener(GameEvent.GE_TELEPORT, onTeleport);
            pickMaxDistance = ConfigManager.Instance.Consts.GetValue<float>(420, "fval");
        }

        private void onTeleport(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sTeleportResult telResult = parameter.objParameter as MsgData_sTeleportResult;
            int result = parameter.intParameter1;
            if (result == 0) {
                DropItemInfo dropItem;
                for (int i = 0; i < m_DropItemList.Count; i++)
                {
                    dropItem = m_DropItemList[i];
                    ReleaseDropItem(dropItem);
                    m_DropItemList.RemoveAt(i);
                }
            }

        }

        //private void onChangePos(GameEvent ge, EventParameter parameter)
        //{
        //    MsgData_sChangePos result = parameter.objParameter as MsgData_sChangePos;
        //    DropItemInfo dropItem;
        //    for (int i = 0; i < m_DropItemList.Count; i++)
        //    {
        //        dropItem = m_DropItemList[i];
        //        ReleaseDropItem(dropItem);
        //        m_DropItemList.RemoveAt(i);
        //    }
        //}

        public static IModule Newer(GameObject go)
        {
            IModule module = go.AddComponent<DropMgr>();

            return module;
        }
        //-------------------------------------------

        //float pickTimeInterval = 0.3f;
        //float lastPickTime = 0f;
        float pickMaxDistance = 3.0f;
        void Update()
        {
            //if (Time.time - lastPickTime > pickTimeInterval)
            //{
                if (m_DropItemList.Count > 0)
                {
                    m_PickItems.Clear();
                    for (int i = 0; i < m_DropItemList.Count; i++)
                    {
                        DropItemInfo item = m_DropItemList[i];
                        if (! hasAddedItems.Contains(item.serverID))
                        {
                            if (CanPick(item))
                            {
                                m_PickItems.Add(item.serverID);
                                hasAddedItems.Add(item.serverID);
                            }
                        }
                    }

                    SendPickItem();
                }

                //lastPickTime = Time.time;
            //}
        }

        void OnDestroy()
        {
            m_EventMgr.RemoveListener(GameEvent.GE_AFTER_LOADSCENE, OnSceneLoaded);
            m_EventMgr.RemoveListener(GameEvent.GE_OBJ_ITEM_ENTER, OnItemEnter);
            m_EventMgr.RemoveListener(GameEvent.GE_OBJ_ITEM_LEAVE, OnItemLeave);
            m_EventMgr.RemoveListener(GameEvent.GE_ITEM_PICKUP, OnItemPickUp);
        }

#region 事件
        private void OnSceneLoaded(GameEvent ge, EventParameter parameter)
        {
            InitDropObjPool();
            InitDropInfoData();
        }
        private void OnItemEnter(GameEvent ge, EventParameter parameter)
        {
            MsgData_sSceneObjectEnterItem itemStruct = parameter.objParameter as MsgData_sSceneObjectEnterItem;
            if (null == itemStruct)
            {
                return;
            }

            ShowDropItem(itemStruct);
        }
        private void OnItemLeave(GameEvent ge, EventParameter parameter)
        {
            long serverID = parameter.longParameter;

            ReleaseDropItem(serverID);
        }
        private void OnItemPickUp(GameEvent ge, EventParameter parameter)
        {
            MsgData_sPickUpItem data = parameter.msgParameter as MsgData_sPickUpItem;
            if (null == data)
            {
                return;
            }

            for (int i = 0; i < data.Data.Count; i++)
            {
                PickUpItem_Result result = data.Data[i];
                if (0 == result.Result)//0:成功
                {
                    hasAddedItems.Remove(result.ID);
                    PickDropItem(result.ID);
                }
                else if (-1 == result.Result)//-1:物品不存在
                {
                    hasAddedItems.Remove(result.ID);
                    //LogMgr.UnityError("物品不存在 " + result.ID);

                }
                else if (-2 == result.Result)//-2:不属于该玩家
                {
                    UITips.ShowTips("该物品不属于您");
                }
                else if (-3 == result.Result)//-3:背包已满
                {
                    UITips.ShowTips("背包已满,拾取失败");
                }
            }
        }
#endregion

#region 初始化
        /// <summary>
        /// 初始化掉落对象池
        /// </summary>
        public void InitDropObjPool()
        {
            m_DropItemList.Clear();
            m_UnusedDropItemList.Clear();

            GameObject DropPoolRoot = new GameObject("DropPoolRoot");

            m_itemEftPoolDict = new Dictionary<string, GameObjectPool>();
            for (int i = 0; i < m_ItemEftFalls.Length; i++)
            {
                GameObject itemObj = CreateDropObj(m_ItemEftFalls[i], m_itemEftStay);
                if (itemObj != null)
                {
                    itemObj.transform.SetParent(DropPoolRoot.transform);
                    m_itemEftPoolDict.Add(m_ItemEftFalls[i], new GameObjectPool(itemObj, DropPoolRoot, 2, 2));
                }
            }

            GameObject moneyObj = CreateDropObj(m_moneyEftFall, m_moneyEftStay);
            if (moneyObj != null)
            {
                moneyObj.transform.SetParent(DropPoolRoot.transform);
                m_moneyEftObjectPool = new GameObjectPool(moneyObj, DropPoolRoot, 10, 5);
            }

            //GameObject pickflyObj = (GameObject)UnityEngine.Object.Instantiate(m_ResourceLoader.LoadResource("Effect/ui/DropPickFly"));
            //if (pickflyObj != null)
            //{
            //    pickflyObj.transform.SetParent(DropPoolRoot.transform);
            //    pickflyObj.AddComponent<DropPickFly>();
            //    pickflyObj.SetActive(false);
            //    m_pickEftObjectPool = new GameObjectPool(pickflyObj, DropPoolRoot, 10, 20);
            //}

            foreach (KeyValuePair<int, string> kv in QualitEftDict)
            {
                GameObject effectObj = CoreEntry.gGameObjPoolMgr.Instantiate(kv.Value);
                if (null != effectObj)
                {
                    CoreEntry.gGameObjPoolMgr.Destroy(effectObj);
                }
            }
        }

        /// <summary>
        /// 构建掉落物体
        /// </summary>
        /// <param name="fallName"></param>
        /// <param name="stayName"></param>
        /// <returns></returns>
        private GameObject CreateDropObj(string fallName, string stayName)
        {
            GameObject dropItemObj = new GameObject();
            dropItemObj.name = "dropItemObj";

            GameObject fallObj = (GameObject)UnityEngine.Object.Instantiate(m_ResourceLoader.LoadResource(stayName));
            fallObj.name = "dropItem";
            fallObj.SetActive(true);
            fallObj.transform.SetParent(dropItemObj.transform);
            fallObj.transform.localPosition = new Vector3(0f, 0.3f, 0f);

            if (!string.IsNullOrEmpty(fallName))
            {
                GameObject stayObj = (GameObject)UnityEngine.Object.Instantiate(m_ResourceLoader.LoadResource(fallName));
                if (null != stayObj)
                {
                    stayObj.name = "fallObj";
                    stayObj.SetActive(false);
                    stayObj.transform.SetParent(dropItemObj.transform);
                }
            }

            dropItemObj.AddComponent<DropItem>();
            dropItemObj.SetActive(false);
            return dropItemObj;
        }

        /// <summary>
        /// 初始化话掉落数据
        /// </summary>
        private void InitDropInfoData()
        {
            //掉落数据表

        }
#endregion

#region 掉落
        /// <summary>
        /// 展示掉落物品
        /// </summary>
        /// <param name="monsterID"></param>
        /// <param name="position"></param>
        private void ShowDropItem(MsgData_sSceneObjectEnterItem itemStruct)
        {
            if (itemStruct.Owner != CoreEntry.gActorMgr.MainPlayer.ServerID)
                return;
            Vector3 position = CommonTools.ServerPosToClient(itemStruct.PosX, itemStruct.PosY);
            position.y = CommonTools.GetTerrainHeight(position);

            LuaTable itemCfg = null;
            LuaTable equipCfg = null;

            itemCfg = ConfigManager.Instance.BagItem.GetItemConfig(itemStruct.ConfigID);
            if (null == itemCfg)
            {
                equipCfg = ConfigManager.Instance.BagItem.GetItemConfig(itemStruct.ConfigID);
            }

            if (null == itemCfg && null == equipCfg)
            {
                LogMgr.UnityError("config error:id " + itemStruct.ConfigID + " no config data");

                return;
            }

            string dmeshid = string.Empty;
            string itemName = string.Empty;
            int itemQuality = 0;
            if (null != itemCfg)
            {
                dmeshid = itemCfg.Get<string>("dmeshid");
                itemName = itemCfg.Get<string>("name");
                itemQuality = itemCfg.Get<int>("quality");
            }
            else
            {
                if (equipCfg != null)
                {
                    dmeshid = equipCfg.Get<string>("dmeshid");
                    itemName = equipCfg.Get<string>("name");
                    itemQuality = equipCfg.Get<int>("quality");
                }

            }

            DropItemInfo dropItem = null;
            if (m_UnusedDropItemList.Count > 0)
            {
                dropItem = m_UnusedDropItemList[m_UnusedDropItemList.Count - 1];
                m_UnusedDropItemList.RemoveAt(m_UnusedDropItemList.Count - 1);
            }
            else
            {
                dropItem = new DropItemInfo();
                dropItem.dropItemObj = null;
            }

            dropItem.cfgID = itemStruct.ConfigID;
            dropItem.serverID = itemStruct.Guid;
            dropItem.count = itemStruct.Count;
            dropItem.dropPosition = position;
            string[] strs = dmeshid.Split('#');
            if(strs == null || strs.Length != 2)
            {
                LogMgr.UnityError("item config error:" + itemStruct.ConfigID + ";" + dmeshid);

                return;
            }
            GenerateDropItem(dropItem.dropPosition, strs[1], itemQuality, ref dropItem);
            if(dropItem != null && dropItem.dropItem != null)
            {
                dropItem.dropItem.Init(dropItem.dropType, itemName, dropItem.count, strs[0], itemQuality);
                m_DropItemList.Add(dropItem);
            }
        }

        /// <summary>
        /// 生成掉落
        /// </summary>
        /// <param name="dropPos"></param>
        /// <param name="dropItem"></param>
        private void GenerateDropItem(Vector3 dropPos, string dropEft, int quality, ref DropItemInfo dropItem)
        {
            GameObjectPool objPool = null;
            GameObject qualityEftObj = null;
            dropItem.dropEft = dropEft;
            if (m_itemEftPoolDict.ContainsKey(dropEft))
            {
                dropItem.dropType = DROP_TYPE.DROPTYPE_COMMON;
                objPool = m_itemEftPoolDict[dropEft];

                if (QualitEftDict.ContainsKey(quality))
                {
                    qualityEftObj = CoreEntry.gGameObjPoolMgr.Instantiate(QualitEftDict[quality]);
                }
            }
            else
            {
                dropItem.dropType = DROP_TYPE.DROPTYPE_GOLD;
                objPool = m_moneyEftObjectPool;
            }

            GameObject obj = null;
            if (objPool != null)
            {
                obj = objPool.ObtainPrefabInstance();
                if (obj == null)
                    return;
                dropItem.dropItemObj = obj;
                dropItem.dropItem = obj.GetComponent<DropItem>();
                obj.transform.position = dropPos;
                dropItem.dropQualityEftObj = qualityEftObj;

                if (null != qualityEftObj)
                {
                    qualityEftObj.transform.SetParent(obj.transform);
                    qualityEftObj.transform.localPosition = Vector3.zero;
                    qualityEftObj.transform.localScale = Vector3.one;
                    qualityEftObj.transform.localRotation = Quaternion.identity;
                }
            }
        }
#endregion

#region 拾取

        /// <summary>
        /// 物品是否可拾取
        /// </summary>
        /// <param name="dropItem"></param>
        /// <returns></returns>
        private bool CanPick(DropItemInfo dropItem)
        {
            if (null == dropItem.dropItemObj)
            {
                return false;
            }

            DropItem item = dropItem.dropItemObj.GetComponent<DropItem>();
            if (null == item)
            {
                return false;
            }

            if (false == item.CanPick)
            {
                return false;
            }

            return CanPickByPosition(dropItem.dropPosition);
        }

        /// <summary>
        /// 掉落物品位置是否可以拾取
        /// </summary>
        /// <param name="dropPos"></param>
        /// <returns></returns>
        private bool CanPickByPosition(Vector3 dropPos)
        {
            ActorObj curActor = CoreEntry.gActorMgr.MainPlayer;
            if (null == curActor)
            {
                return false;
            }

            Vector3 actorPos = curActor.transform.position;
            actorPos.y = 0f;
            dropPos.y = 0;

            float distance = Vector3.Distance(actorPos, dropPos);

            return distance < pickMaxDistance;
        }

        /// <summary>
        /// 请求拾取
        /// </summary>
        private void SendPickItem()
        {
            if (m_PickItems.Count > 0)
            {
                NetLogicGame.Instance.SendReqPickUpItem(m_PickItems);
            }
        }

        /// <summary>
        /// 拾取物品
        /// </summary>
        /// <param name="serverID"></param>
        private void PickDropItem(long serverID)
        {
            DropItemInfo dropItem;
            for (int i = 0; i < m_DropItemList.Count; i++)
            {
                if (serverID == m_DropItemList[i].serverID)
                {
                    dropItem = m_DropItemList[i];

                    GetPickFlyPosition();
                    PlayPickEffect(dropItem);//播放拾取特效
                    PlayPickTips(dropItem);//播放提示效果
                    ReleaseDropItem(dropItem);

                    m_DropItemList.RemoveAt(i);

                    //拾取物品触发事件
                    EventParameter param = EventParameter.Get();
                    param.intParameter1 = dropItem.cfgID;
                    param.intParameter2 = dropItem.count;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Drop_Item, param);

                    break;
                }
            }

        }

        /// <summary>
        /// 获取拾取特效的位置
        /// </summary>
        private void GetPickFlyPosition()
        {
            if (null == m_BagTransform)
            {
                PanelBase panel = MainPanelMgr.Instance.GetLoadedPanel("UIMain");
                if (null != panel)
                {
                    Transform t = panel.transform.DeepFindChild("FunctionBarHolder");
                    if (null != t)
                    {
                        m_BagTransform = t.DeepFindChild("Bag");
                    }
                }
            }

            if (null == m_PickFlyTransform)
            {
                PanelBase panel = MainPanelMgr.Instance.GetLoadedPanel("UIMain");
                if (null != panel)
                {
                    Transform t = panel.transform.DeepFindChild("ChatInfoHolder");
                    if (null != t)
                    {
                        m_PickFlyTransform = t.DeepFindChild("Bag");
                    }
                }
            }
        }

        /// <summary>
        /// 拾取特效
        /// </summary>
        /// <param name="dropItem"></param>
        private void PlayPickEffect(DropItemInfo dropItem)
        {
            if (dropItem.dropType == DROP_TYPE.DROPTYPE_GOLD)
            {
                EventToUI.SetArg(UIEventArg.Arg1, dropItem.count);
                EventToUI.SetArg(UIEventArg.Arg2, dropItem.dropPosition);
                EventToUI.SendEvent("EU_SHOW_DROPGOLDGFLY");

                CoreEntry.gAudioMgr.PlayUISound(900014);

                return;
            }

            LuaTable itemCfg = null;
            LuaTable equipCfg = null;

            itemCfg = ConfigManager.Instance.BagItem.GetItemConfig(dropItem.cfgID);
            if (null == itemCfg)
            {
                equipCfg = ConfigManager.Instance.BagItem.GetItemConfig(dropItem.cfgID);
            }

            if (null == itemCfg && null == equipCfg)
            {
                return;
            }

            string dmeshid = string.Empty;
            string itemName = string.Empty;
            int quality = -1;
            if (null != itemCfg)
            {
                dmeshid = itemCfg.Get<string>("dmeshid");
                itemName = itemCfg.Get<string>("name");
                quality = itemCfg.Get<int>("quality");
            }
            else
            {
                if(equipCfg !=null)
                {
                    dmeshid = equipCfg.Get<string>("dmeshid");
                    itemName = equipCfg.Get<string>("name");
                    quality = equipCfg.Get<int>("quality");
                }

            }

            string[] strs = dmeshid.Split('#');
            if (strs == null || strs.Length != 2)
            {
                return;
            }

            EventToUI.SetArg(UIEventArg.Arg1, strs[0]);
            EventToUI.SetArg(UIEventArg.Arg2, dropItem.dropPosition);
            Transform t = PickFlyTarget;
            if (null != t)
            {
                EventToUI.SetArg(UIEventArg.Arg3, t.position);
            }
            else
            {
                EventToUI.SetArg(UIEventArg.Arg3, Vector3.zero);
            }
            EventToUI.SetArg(UIEventArg.Arg4, string.Format("{0} x{1}" , itemName, dropItem.count));
            EventToUI.SetArg(UIEventArg.Arg5, quality);
            EventToUI.SendEvent("EU_SHOW_DROPFLY");

            //if (m_pickEftObjectPool != null)
            //{
            //    GameObject pickFlyObj = m_pickEftObjectPool.ObtainPrefabInstance();
            //    if (pickFlyObj != null)
            //    {
            //        pickFlyObj.SetActive(true);
            //        DropPickFly pickFly = pickFlyObj.GetComponent<DropPickFly>();
            //        ActorObj po = CoreEntry.gActorMgr.GetPlayerActorByServerID(MainRole.Instance.serverID);
            //        //pickFly.Init(dropItem.dropPosition, po.gameObject, m_pickEftObjectPool);
            //        pickFly.Init(dropItem.dropPosition, m_BagUIPos, m_pickEftObjectPool);
            //    }
            //}
        }

        /// <summary>
        /// 根据服务器id回收，非自身拾取处理
        /// </summary>
        /// <param name="serverID"></param>
        private void ReleaseDropItem(long serverID)
        {
            DropItemInfo dropItem;
            for (int i = 0; i < m_DropItemList.Count; i++)
            {
                if (serverID == m_DropItemList[i].serverID)
                {
                    dropItem = m_DropItemList[i];

                    ReleaseDropItem(dropItem);

                    m_DropItemList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 回收掉落物品
        /// </summary>
        /// <param name="unusedDropItem"></param>
        private void ReleaseDropItem(DropItemInfo unusedDropItem)
        {
            EventToUI.SetArg(UIEventArg.Arg1, unusedDropItem.dropItemObj);
            EventToUI.SendEvent("EU_REMOVE_DROPITEMTEXT");

            GameObjectPool objPool = m_moneyEftObjectPool;
            if (unusedDropItem.dropType == DROP_TYPE.DROPTYPE_COMMON)
            {
                objPool = m_itemEftPoolDict[unusedDropItem.dropEft];
            }

            if (null != unusedDropItem.dropQualityEftObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(unusedDropItem.dropQualityEftObj);
            }

            if (unusedDropItem.dropItemObj != null)
            {
                objPool.RecyclePrefabInstance(unusedDropItem.dropItemObj);
                unusedDropItem.dropItemObj = null;
            }
            m_UnusedDropItemList.Add(unusedDropItem);
        }

        /// <summary>
        /// 播放拾取提示
        /// </summary>
        /// <param name="item"></param>
        private void PlayPickTips(DropItemInfo item)
        {

            return;
        }
#endregion

        struct ItemDisplayInfo
        {
            public int iID;			//道具ID号
            public string itemName; //道具名称
            public string itemFallEftName;//掉落特效名称
            public string itemStayEftName;//保持特效名称
            public string itemDropAnim;   //掉落动画
        }

        public enum DROP_TYPE
        {
            DROPTYPE_GOLD,
            DROPTYPE_COMMON,
        };

[Hotfix]
        public class DropItemInfo
        {
            public DROP_TYPE dropType;
            public string dropEft;
            public Vector3 dropPosition;
            public long serverID;
            public int cfgID;
            public int count;
            public GameObject dropItemObj;
            public DropItem dropItem;
            public GameObject dropQualityEftObj;
        }

        //当前掉落物品，用于监听拾取
        private List<DropItemInfo> m_DropItemList = new List<DropItemInfo>();
        private List<DropItemInfo> m_UnusedDropItemList = new List<DropItemInfo>();

        private List<long> m_PickItems = new List<long>();
        private List<long> hasAddedItems = new List<long>();

        //掉落物及拾取特效
        private string m_moneyEftFall = "Effect/skill/remain/fx_diaoluo_jinbi";
        private string m_moneyEftStay = "Effect/common/money_s";
        private static string[] m_ItemEftFalls = new string[] 
        {
            "Effect/skill/remain/fx_diaoluo_baoxiang",
            "Effect/skill/remain/fx_diaoluo_hujian",
            "Effect/skill/remain/fx_diaoluo_huwan",
            "Effect/skill/remain/fx_diaoluo_jiezhi",
            "Effect/skill/remain/fx_diaoluo_kuzi",
            "Effect/skill/remain/fx_diaoluo_longdan",
            "Effect/skill/remain/fx_diaoluo_peishi",
            "Effect/skill/remain/fx_diaoluo_wuqi",
            "Effect/skill/remain/fx_diaoluo_xianglian",
            "Effect/skill/remain/fx_diaoluo_xiezi",
            "Effect/skill/remain/fx_diaoluo_yaodai",
            "Effect/skill/remain/fx_diaoluo_yifu",
        };
        private string m_itemEftStay = "UI/Prefabs/Drop/FirstRes/DropItem";
        public static Dictionary<int, string> QualitEftDict = new Dictionary<int, string>()
        {
            {1, "Effect/common/fx_baoxiangpinzhi_lv"},
			{2, "Effect/common/fx_baoxiangpinzhi_lan"},
            {3, "Effect/common/fx_baoxiangpinzhi_zi"},
			{4, "Effect/common/fx_baoxiangpinzhi_cheng"},
            {5, "Effect/common/fx_baoxiangpinzhi_jin"},
            {6, "Effect/common/fx_baoxiangpinzhi_hong"},
        };
        GameObjectPool m_moneyEftObjectPool = null;
        Dictionary<string, GameObjectPool> m_itemEftPoolDict = null;
        //GameObjectPool m_pickEftObjectPool = null;
        #pragma warning disable 0414
        Vector3 m_BagUIPos = new Vector3(301, 377, 0);

        private Transform m_PickFlyTransform;
        public Transform PickFlyTarget
        {
            get 
            {
                if (CoreEntry.gJoystickMgr.IsShow)
                {
                    return m_PickFlyTransform;
                }
                else
                {
                    return m_BagTransform;
                }
            }
        }
        private Transform m_BagTransform;

        //事件管理器
        private EventMgr m_EventMgr;
        private ResourceLoader m_ResourceLoader;
    }

};//End SG

#pragma warning disable 0414

