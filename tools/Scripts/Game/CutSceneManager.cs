/**
* @file     : CutSceneManager.cs
* @brief    : 过场管理
* @details  : 
* @author   : XuXiang
* @date     : 2017-11-27 17:07
*/

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using WellFired;
using XLua;

namespace SG
{
[Hotfix]
    public class CutSceneManager
    {
        /// <summary>
        /// 过场UI路径。
        /// </summary>
        public static string UI_PATH = "UI/Prefabs/Common/TipCutScene";

        public const int TRIGGER_TIME_ONCE = 1;                         //首次触发
        public const int TRIGGER_TIME_EVERYTIME = 2;                    //每次触发

        public const int TRIGGER_TYPE_SCENE = 1;                        //场景触发
        public const int TRIGGER_TYPE_TASK = 2;                         //任务触发
        public const int TRIGGER_TYPE_MONSTER = 3;                      //怪物触发
        public const int TRIGGER_TYPE_DUNGEON = 4;                      //副本触发

        public const int SCENE_TYPE_ENTER = 1;                          //进入场景
        public const int SCENE_TYPE_POSITION = 2;                       //抵达坐标
        public const int TASK_TYPE_ACCEPT = 1;                          //接收任务
        public const int TASK_TYPE_COMPLETE = 2;                        //完成任务
        public const int TASK_TYPE_APPLY = 3;                           //提交任务
        public const int MONSTER_TYPE_BORN = 1;                         //怪物出生
        public const int MONSTER_TYPE_DIE = 2;                          //怪物死亡
        public const int DUNGEON_TYPE_ENTER = 1;                        //进入副本
        public const int DUNGEON_TYPE_COMPLETE = 2;                     //完成副本

        //单例。
        private static CutSceneManager _instance = null;

        /// <summary>
        /// 获取单例。
        /// </summary>
        public static CutSceneManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CutSceneManager();
                    _instance.Init();
                }
                return _instance;
            }
        }



        /// <summary>
        /// 地图对应到动画。
        /// </summary>
        private Dictionary<int, List<LuaTable>> m_CacheMapAnimation = new Dictionary<int, List<LuaTable>>();

        /// <summary>
        /// 任务对应到动画。
        /// </summary>
        private Dictionary<int, List<LuaTable>> m_CacheTaskAnimation = new Dictionary<int, List<LuaTable>>();

        /// <summary>
        ///怪物对应到动画。
        /// </summary>
        private Dictionary<int, List<LuaTable>> m_CacheMonsterAnimation = new Dictionary<int, List<LuaTable>>();

        /// <summary>
        /// 副本对应到动画。
        /// </summary>
        private Dictionary<int, List<LuaTable>> m_CacheDungeonAnimation = new Dictionary<int, List<LuaTable>>();

        /// <summary>
        /// 上次播放到停止时间。
        /// </summary>
        private float m_LastPlayStopTime = 0;

        /// <summary>
        /// UI对象。
        /// </summary>
        private GameObject m_CacheUI;

        /// <summary>
        /// 当前动画ID。
        /// </summary>
        private int m_CurID;

        /// <summary>
        /// 获取当前动画编号。
        /// </summary>
        public int CurID
        {
            get { return m_CurID; }
        }

        /// <summary>
        /// 当前动画。
        /// </summary>
        private USSequencer m_CurAni;

        /// <summary>
        /// 添加动画编号。
        /// </summary>
        /// <param name="cache">动画缓存。</param>
        /// <param name="id">动画编号。</param>
        private static void AddAnimation(Dictionary<int, List<LuaTable>> cache, int id, LuaTable ani)
        {
            List<LuaTable> data;
            if (!cache.TryGetValue(id, out data))
            {
                data = new List<LuaTable>();
                cache.Add(id, data);
            }
            data.Add(ani);
        }

        public void Init()
        {
            //反向分析，建立动画关系
            foreach (var kvp in ConfigManager.Instance.Common.Animations)
            {
                LuaTable cfg = kvp.Value as LuaTable;
                if (cfg.Get<int>("sceneid") != 0)
                {
                    AddAnimation(m_CacheMapAnimation, cfg.Get<int>("sceneid"), cfg);
                }
                if (cfg.Get<int>("questid") != 0)
                {
                    AddAnimation(m_CacheTaskAnimation, cfg.Get<int>("questid"), cfg);
                }
                if (cfg.Get<int>("monsterid") != 0)
                {
                    AddAnimation(m_CacheMonsterAnimation, cfg.Get<int>("monsterid"), cfg);
                }
                if (cfg.Get<int>("dungeontype") != 0 && cfg.Get<int>("dungeonid") != 0)
                {
                    AddAnimation(m_CacheDungeonAnimation, cfg.Get<int>("dungeonid"), cfg);
                }
            }
        }
        
        /// <summary>
        /// 注册游戏事件。
        /// </summary>
        public void RegisterGameEventListener()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnGameEventEnterScene);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_POSITION, OnGameEventMovePosition);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MONSTER_LOADED, OnGameEventMonsterLoad);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MONSTER_DEATH, OnGameEventMonsterDeath);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_TaskUpdate, OnGameEventTaskUpdate);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnGameEventDungeonEnter);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_DungeonQuestUpdate, OnGameEventTaskDungeonUpdate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_RESLEVEL_END, OnGameEventResLevelEnd);
        }

        /// <summary>
        /// 角色进入场景。
        /// </summary>
        public void OnGameEventEnterScene(GameEvent ge, EventParameter parameter)
        {
            int map = MapMgr.Instance.EnterMapId;
            List<LuaTable> anis;
            if (!m_CacheMapAnimation.TryGetValue(map, out anis))
            {
                return;
            }

            for (int i = 0; i < anis.Count; ++i)
            {
                LuaTable cfg = anis[i];
                if (cfg.Get<int>("scene") == SCENE_TYPE_ENTER)
                {
                    TryPlayerCutScene(cfg);
                    break;
                }
            }
        }

        /// <summary>
        /// 角色移动到某个位置。
        /// </summary>
        public void OnGameEventMovePosition(GameEvent ge, EventParameter parameter)
        {
            int map = MapMgr.Instance.EnterMapId;
            List<LuaTable> anis;
            if (!m_CacheMapAnimation.TryGetValue(map, out anis))
            {
                return;
            }

            Vector3 pos = (Vector3)parameter.objParameter;
            for (int i=0; i< anis.Count; ++i)
            {
                LuaTable cfg = anis[i];
                if (cfg.Get<int>("scene") == SCENE_TYPE_POSITION)
                {
                    //判断位置
                    LuaTable poscfg = ConfigManager.Instance.Map.GetPositionConfig(cfg.Get<int>("positionid"));
                    List <int> posinfo = UiUtil.SplitNumber(poscfg.Get<string>("pos"), ",");
                    if (posinfo.Count >= 3)
                    {
                        float r = posinfo.Count >= 4 ? posinfo[3] : 3;      //默认3的范围
                        float x = pos.x - posinfo[1];
                        float z = pos.z - posinfo[2];
                        if (x * x + z * z <= r * r)
                        {
                            TryPlayerCutScene(cfg);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 角色进场。
        /// </summary>
        public void OnGameEventMonsterLoad(GameEvent ge, EventParameter parameter)
        {
            MonsterObj obj = parameter.objParameter as MonsterObj;
            List<LuaTable> anis;
            if (!m_CacheMonsterAnimation.TryGetValue(obj.ConfigID, out anis))
            {
                return;
            }

            int map = MapMgr.Instance.EnterMapId;
            for (int i = 0; i < anis.Count; ++i)
            {
                LuaTable cfg = anis[i];
                if (cfg.Get<int>("monster") == MONSTER_TYPE_BORN)
                {
                    int sceneid = cfg.Get<int>("sceneid");
                    if (sceneid == 0 || sceneid == map)
                    {
                        TryPlayerCutScene(cfg);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 角色死亡。
        /// </summary>
        public void OnGameEventMonsterDeath(GameEvent ge, EventParameter parameter)
        {
            ActorObj actor = parameter.goParameter.GetComponent<ActorObj>();
            MonsterObj monster = actor as MonsterObj;
            if (monster == null)
            {
                return;
            }
            
            List<LuaTable> anis;
            if (!m_CacheMonsterAnimation.TryGetValue(monster.ConfigID, out anis))
            {
                return;
            }

            int map = MapMgr.Instance.EnterMapId;
            for (int i = 0; i < anis.Count; ++i)
            {
                LuaTable cfg = anis[i];
                if (cfg.Get<int>("monster") == MONSTER_TYPE_DIE)
                {
                    int sceneid = cfg.Get<int>("sceneid");
                    if (sceneid == 0 || sceneid == map)
                    {
                        TryPlayerCutScene(cfg);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 任务更新。
        /// </summary>
        public void OnGameEventTaskUpdate(GameEvent ge, EventParameter parameter)
        {
            int id = parameter.intParameter1;
            List<LuaTable> anis;
            if (!m_CacheTaskAnimation.TryGetValue(id, out anis))
            {
                return;
            }

            //任务状态过滤
            TaskMgr.TaskEnum te = (TaskMgr.TaskEnum)parameter.intParameter;
            int state = te == TaskMgr.TaskEnum.add ? TASK_TYPE_ACCEPT : (te == TaskMgr.TaskEnum.finish ? TASK_TYPE_APPLY : 0);
            if (state == 0 && te == TaskMgr.TaskEnum.update)
            {
                MsgData_sQuestInfo info = parameter.msgParameter as MsgData_sQuestInfo;
                if (info.state == 2)
                {
                    state = TASK_TYPE_COMPLETE;
                }
            }
            if (state == 0)
            {
                return;
            }

            for (int i = 0; i < anis.Count; ++i)
            {
                LuaTable cfg = anis[i];
                if (cfg.Get<int>("quest") == state)
                {
                    TryPlayerCutScene(cfg);
                    break;
                }
            }
        }

        /// <summary>
        /// 获取副本编号。
        /// </summary>
        /// <param name="type">副本类型。</param>
        /// <returns>副本编号。</returns>
        public static int GetDungeonID(int type)
        {
            //4 = 活动地图
            //5 = 帮派活动
            //6 = 通天塔
            //7 = 限时活动
            //8 = 限时杀怪副本
            //9 = 北苍殿限时挑战
            //10 = 境界突破
            //11 = 任务副本
            //12 = 主宰之路
            //13 = 灵兽墓地
            //14 = 灵路试炼
            //15 = 奇遇副本
            //16 = 跨服PVP地图
            //17 = 转生地图
            //18 = 抢门地图
            //19 = 地宫炼狱
            //21 = 多宝圣地
            //22 = 深渊魔域
            //23 = 大唐奇遇
            //24 = 帮派组队
            //25 = 帮派战
            //26 = 跨服boss
            //27 = 跨服战场
            //28 = 跨服淘汰
            //30 = 宝塔秘境
            //31 = 幻灵仙域            
            //33 = 个人挑战boss
            //34 = 跨服擂台赛
            //35 = 跨服场景
            //36 = 跨服夺帅地图
            //39 = 神武副本
            //40 = 跨服组队副本            

            int ret = 0;
            switch (type)
            {
                case 20:
                    ret = BossFuBenDataMgr.Instance.EnterId;
                    break;
                case 32:    //32 = 资源副本
                    ret = PlayerData.Instance.ResLevelData.CurID;
                    break;
                case 41:    //41 = 任务副本
                    ret = TaskMgr.Instance.GetCurTaskFbId();
                    break;
                default:
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 进入副本。
        /// </summary>
        public void OnGameEventDungeonEnter(GameEvent ge, EventParameter parameter)
        {
            CheckDungeon(DUNGEON_TYPE_ENTER);
        }

        /// <summary>
        /// 更新任务副本。
        /// </summary>
        public void OnGameEventTaskDungeonUpdate(GameEvent ge, EventParameter parameter)
        {
            MsgData_sDungeonQuestStateUpdate resp = parameter.msgParameter as MsgData_sDungeonQuestStateUpdate;
            if (resp.state != 0)
            {
                return;
            }

            CheckDungeon(DUNGEON_TYPE_COMPLETE);
        }

        /// <summary>
        /// 资源副本结束。
        /// </summary>
        public void OnGameEventResLevelEnd(GameEvent ge, EventParameter parameter)
        {
            if (parameter.intParameter != 1)
            {
                return;
            }

            CheckDungeon(DUNGEON_TYPE_COMPLETE);
        }

        /// <summary>
        /// 检查副本触发。
        /// </summary>
        /// <param name="type">副本触发类型。</param>
        private void CheckDungeon(int type)
        {
            int map = MapMgr.Instance.EnterMapId;
            LuaTable mapcfg = ConfigManager.Instance.Map.GetMapConfig(map);
            int dunid = GetDungeonID(mapcfg.Get<int>("type"));
            int key = dunid * 1000 + mapcfg.Get<int>("type");       //副本类型目前都是两位
            List<LuaTable> anis;
            if (!m_CacheTaskAnimation.TryGetValue(key, out anis))
            {
                return;
            }

            for (int i = 0; i < anis.Count; ++i)
            {
                LuaTable cfg = anis[i];
                if (cfg.Get<int>("dungeontype") == type)
                {
                    TryPlayerCutScene(cfg);
                    break;
                }
            }
        }

        /// <summary>
        /// 已播放过到动画。
        /// </summary>
        private Dictionary<int, bool> m_PlayerAni = new Dictionary<int, bool>();

        /// <summary>
        /// 判断是否有播放过某动画。
        /// </summary>
        /// <param name="id">动画ID。</param>
        /// <returns>是否有播放过。</returns>
        public bool IsHavePlay(int id)
        {
            bool ret;
            if (m_PlayerAni.TryGetValue(id, out ret))
            {
                return ret;
            }

            string key = string.Format("CUT_SCENE_{0}", id);
            ret = RoleUtility.GetBool(key, false);
            m_PlayerAni.Add(id, ret);

            return ret;
        }

        /// <summary>
        /// 设置动画已经播放过。
        /// </summary>
        /// <param name="id">动画编号。</param>
        public void SetHavePlay(int id)
        {
            string key = string.Format("CUT_SCENE_{0}", id);
            m_PlayerAni[id] = true;
            RoleUtility.SetBool(key, true);
        }

        /// <summary>
        /// 跳过过场。
        /// </summary>
        public void SkipCutScene()
        {
            OnAniEnd(m_CurAni);
        }

        /// <summary>
        /// 播放过场。
        /// </summary>
        /// <param name="path">过场动画路径。</param>
        public void TryPlayerCutScene(LuaTable cfg)
        {
            //上一个动画还未结束
            if (Time.realtimeSinceStartup < m_LastPlayStopTime)
            {
                return;
            }

            //首次播放检查
            int id = cfg.Get<int>("id");
            if (cfg.Get<int>("time") == TRIGGER_TIME_ONCE)
            {
                if (IsHavePlay(id))
                {
                    return;
                }
                else
                {
                    SetHavePlay(id);
                }
            }

            //播放动画            
            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(cfg.Get<string>("animation"), typeof(GameObject));
            if (prefab == null)
            {
                LogMgr.LogError("加载过场动画失败 path:{0}", cfg.Get<string>("animation"));
                return;
            }
            GameObject obj = GameObject.Instantiate(prefab) as GameObject;
            USSequencer us = obj.GetComponent<USSequencer>();
            if (us == null)
            {
                LogMgr.LogError("播放场动画失败，未找到USSequencer组建 path:{0}", cfg.Get<string>("animation"));
                return;
            }            
            us.PlaybackFinished += OnAniEnd;
            m_CurID = id;
            OnAniStart(us);

            //保存结束时间，在次期间不再播动画
            float duration = 0;
            AutoDestroy ad = obj.GetComponent<AutoDestroy>();
            if (ad != null)
            {
                duration = ad.Delay;
            }
            m_LastPlayStopTime = Time.realtimeSinceStartup + duration;
#if UNITY_EDITOR
            LogMgr.Log("播放过场动画 duration:{0} path:{1}", duration, cfg.Get<string>("animation"));
#endif
        }

        private void OnAniStart(USSequencer us)
        {
            m_CurAni = us;
            m_CurAni.Play();
            PlayerData.Instance.RideData.SendChangeRideStateRequest(0);         //进入剧情动画需要下马
            SetJoystickEnable(false);   //播放过场动画开始时禁用摇杆
            OpenUI();
            CoreEntry.gAutoAIMgr.IsSuspend = true;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_StoryStart, EventParameter.Get());
        }

        private void OnAniEnd(USSequencer us)
        {
            if (m_CurAni == us && m_CurAni != null)
            {
                GameObject.Destroy(m_CurAni.gameObject, 0.1f);
                m_CurAni = null;
                m_CurID = 0;
                SetJoystickEnable(true);    //播放过场动画结束时启用摇杆
                CloseUI();
                CoreEntry.gAutoAIMgr.IsSuspend = false;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_StoryEnd, EventParameter.Get());                
            }
        }

        /// <summary>
        /// 设置摇杆的使用状态。
        /// </summary>
        /// <param name="enable">是否启用。</param>
        public static void SetJoystickEnable(bool enable)
        {
            EventParameter ep = EventParameter.Get(enable ? 0 : 1);
            ep.intParameter1 = 1;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_JOYSTICK_STATE, ep);
        }

        /// <summary>
        /// 打开过场UI。
        /// </summary>
        public void OpenUI()
        {
            if (m_CacheUI == null)
            {
                m_CacheUI = CommonTools.AddSubChild(null, UI_PATH);
            }
            m_CacheUI.SetActive(true);
        }

        /// <summary>
        /// 关闭过场UI。
        /// </summary>
        public void CloseUI()
        {
            if (m_CacheUI != null && m_CacheUI.activeSelf)
            {
                m_CacheUI.SetActive(false);
            }
        }
    }
}

