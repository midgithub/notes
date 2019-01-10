
using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using SG;
using XLua;

namespace SG
{
    public enum PVP_TYPE
    {
        PVP_TYPE_NONE = -1, //默认值，初始站位用
        PVP_TYPE_NOPE = 0,
        PVP_TYPE_ARENA = 1,
        PVP_TYPE_REAL_TIME = 2, // KOF
        PVP_TYPE_YUANZHENG = 3,
        PVP_TYPE_DUOBAO = 4,//夺宝
        PVP_TYPE_TEAM_BOSS = 5, // 组队BOSS
        PVP_TYPE_PLAYGROUND = 6,
        PVP_TYPE_TAKE_LOVE = 7,//横刀夺爱
        PVP_TYPE_WORLDBOSS_DOUBLEATK = 10,
        PVP_TYPE_COMPETE = 11,//切磋武艺
        PVP_TYPE_GUILDBOSS = 12,//军团boss
        PVP_TYPE_MOBA = 13,
        PVP_TYPE_GUILDWAR = 14,//军团战
    }

    //游戏逻辑管理器    
[Hotfix]
    public class GameLogicMgr : MonoBehaviour
    {
        private GameDBMgr m_gameDBMgr = null;
        private EventMgr m_eventMgr = null;
        //private ResourceLoader m_ResourceLoader;
        private SceneMgr m_sceneMgr = null;
        //private ObjectPoolManager m_ObjPoolMgr = null;

        public bool m_bScaleTime = false;

        /// <summary>
        ///音效声音大小
        /// </summary>
        public float SoundVolume = 0.5f;


        //float m_sendRatio = 0.05f;

        //是否按区域加载    
        public bool m_isLoadZone = true;

        ////add by lzp
        ////自动打怪
        //private bool m_AutoFight = false;
        public bool GuidScence
        {
            get
            {
                return false;
            }
        }

        //当前场景 刷怪 组ID
        private int m_curMonsterGroupId = 0;
        public int CurMonsterGroupId
        {
            get { return m_curMonsterGroupId; }
            set { m_curMonsterGroupId = value; }
        }
        
        //mod by Alex 20150702 pvp类型
        private PVP_TYPE m_PvpType = PVP_TYPE.PVP_TYPE_NOPE;
        public PVP_TYPE PvpType
        {
            get { return m_PvpType; }
            set
            {
                m_PvpType = value;
            }
        }

        //当前怪物波数
        private int m_curMonsterWaveSeq = 0;
        public int CurMonsterWaveSeq
        {
            get { return m_curMonsterWaveSeq; }
            set
            {
                if (value != m_curMonsterWaveSeq)
                {
                    m_curMonsterWaveSeq = value;

                    EventParameter ge = EventParameter.Get();
                    ge.intParameter = m_curMonsterWaveSeq;
                    ge.intParameter1 = GetWaveMonsterCount();
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FuBen_WaveChange, ge);
                }

            }
        }


        private bool m_bWaveLoop = false;
        //private int m_loopReferSeq = 0;

        //private bool m_bCurWaveLoaded = false;

        private bool m_bNeedCheckZone = false;

        private WaveTriggerDesc m_curWaveTriggerInfo = new WaveTriggerDesc();

        private List<SpawnMonster> m_TurbulenceSpawnMonsters = new List<SpawnMonster>();


        public GameObject m_CheckZoneObj = null;
        public ActorObj m_CheckZoneActorBase = null;

        //游戏开始入口
        void Start()
        {
            //获取需要的管理器
            m_gameDBMgr = CoreEntry.gGameDBMgr;
            m_eventMgr = CoreEntry.gEventMgr;
            //m_ResourceLoader = CoreEntry.gResLoader;
            m_sceneMgr = CoreEntry.gSceneMgr;
            //m_ObjPoolMgr = CoreEntry.gObjPoolMgr;
            //m_skillComboMgr = CoreEntry.gSkillComboMgr;

            RegisterEvent();

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FuBen_exit, OnExitFuBen);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FuBen_loading_end, OnLoadingEnd);


            CoreEntry.gEventMgr.AddListener(GameEvent.GE_UI_PANEL_CONNECT_START, OnEventConnectStart);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_UI_PANEL_CONNECT_SUCCESS, OnEventConnectSuccess);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MSG_FUBEN_REQUIREAWARD, GE_MSG_FUBEN_REQUIREAWARD);


            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MSG_FUBEN_SYNRESULT, OnServerPushItemData);

        }

        public void OnServerPushItemData(GameEvent ge, EventParameter parameter)
        {
            LogMgr.UnityError("---------------OnServerPushItemData");
        }


        public void OnEventConnectStart(GameEvent ge, EventParameter parameter)
        {
            LogMgr.UnityLog("OnEventConnectStart");
            //MainPanelMgr.Instance.ShowDialog("WaitDialog");
            CancelInvoke("ConnectTimeOut");
            Invoke("ConnectTimeOut", 10);

        }
        public void GE_MSG_FUBEN_REQUIREAWARD(GameEvent ge, EventParameter parameter)
        {
            //   NetLogicGame.Instance.sendGetFubenAward();
        }

        public void SendFunbenFinsh()
        {

        }

        public void SendJiaoyanData()
        {

        }
        public void OnEventConnectSuccess(GameEvent ge, EventParameter parameter)
        {
            LogMgr.UnityLog("OnEventConnectSuccess");
            CancelInvoke("ConnectTimeOut");
        }

        public void ConnectTimeOut()
        {
            LogMgr.UnityLog("ConnectTimeOut");
        }

        public void OnExitFuBen(GameEvent ge, EventParameter parameter)
        {
            ClearScene();
            CancelInvoke();
        }

        public int GetCurSceneType()
        {
            if (MainRole.Instance.mapid == 1) return 0;

            LuaTable data = ConfigManager.Instance.Map.GetMapConfig(MainRole.Instance.mapid);
            if (data == null) return 0;

            return data.Get<int>("type");


        }

        //取得副本任务类型
        public int GetCurSceneTaskType()
        {
            return 0;
        }

        //add by lzp 19:29 2015/7/30 停止ai,重置所有怪物ACTOR_STATE
        public void pauseAllMonsterAction()
        {
            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllMonsterActors();
            for (int i = 0; i < actorList.Count; i++)
            {
                ActorObj actor = actorList[i];
                if (!actor.IsDeath())
                {
                    actor.ChangeState(ACTOR_STATE.AS_STAND);
                }
            }
        }

        public static int GetCurSceneID(string strLevelName)
        {
            if (strLevelName == "MainUI") return 0;

            if (strLevelName == "LoginUI") return 0;

            if (strLevelName == "RoleUI") return 0;

            int mapid = MainRole.Instance.mapid;
            LuaTable data = ConfigManager.Instance.Map.GetMapConfig(mapid);
            if (data == null)
            {
                return 0;
            }

            return mapid;
        }


        void Update()
        {

            if (m_bScaleTime)
            {
                CoreEntry.gTimeMgr.TimeScale = 0.2f;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    //AGE.ActionManager.Instance.PlayAction("Data/age/MoveAttack", true, false, m_playerObj);            
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressingButton(1);
                    //m_sendRatio = 0.05f;
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressingButton(2);
                }
                else if (Input.GetKeyDown(KeyCode.K))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressingButton(3);
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressingButton(4);
                }
                else if (Input.GetKeyUp(KeyCode.H))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressedButton(1);
                    //m_sendRatio = 0.05f;
                }
                else if (Input.GetKeyUp(KeyCode.J))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressedButton(2);
                }
                else if (Input.GetKeyUp(KeyCode.K))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressedButton(3);

                }
                else if (Input.GetKeyUp(KeyCode.L))
                {
                    if (ModuleServer.MS.GSkillCastMgr != null)
                        ModuleServer.MS.GSkillCastMgr.PressedButton(4);
                }
            }


            if (m_bNeedCheckZone && CoreEntry.gActorMgr.MainPlayer)
            {
                Vector3 pos = CoreEntry.gActorMgr.MainPlayer.transform.position;

                //yy 其他NPC也可以触发怪物
                if (m_CheckZoneObj != null)
                {
                    pos = m_CheckZoneObj.transform.position;
                }
                
                if (pos.x >= m_curWaveTriggerInfo.arg1 && pos.x <= m_curWaveTriggerInfo.arg4
                    && pos.z >= m_curWaveTriggerInfo.arg3 && pos.z <= m_curWaveTriggerInfo.arg6)
                {
                    m_bNeedCheckZone = false;
                    LoadMonster();

                    SetMonsterTips(false);

                }
            }
        }

        //出发下一区域 波数 , 要求必须是区域触发类型的怪
        public void TriggerNextWave()
        {

            m_bNeedCheckZone = false;
            SetMonsterTips(false);
            LoadMonster();

        }

        //注册事件
        void RegisterEvent()
        {
            m_eventMgr.AddListener(GameEvent.GE_BEFORE_LOADSCENE, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, EventFunction);
        }


        public void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_BEFORE_LOADSCENE:
                    break;

                case GameEvent.GE_AFTER_LOADSCENE:
#if UNITY_EDITOR
                    GameObject fxmaker = GameObject.Find("dzs_fxmaker");
                    if (fxmaker)
                    {
                        return;
                    }
#endif
                    
                    break;

                default:
                    break;
            }
        }

        WTC_TYPE GetNextWaveTrigType()
        {
            return SpawnWave.DEFAULT_TRIGTYPE;
        }

        void UpdateWaveTriggerInfo()
        {
        }

        //进入新的场景前清除上一个场景的数据
        private void ClearScene()
        {
            //进入场景后清除上一个场景的Actor列表
            CoreEntry.gActorMgr.ClearActor();

            m_TurbulenceSpawnMonsters.Clear();
        }

        public AudioSource m_audioSource = null;


        //进入副本 loadind界面关闭 时候触发
        void OnLoadingEnd(GameEvent ge, EventParameter parameter)
        {
            if (m_audioSource != null)
            {
                if (!m_audioSource.mute)
                {
                    m_audioSource.volume = 0.6f;
                    m_audioSource.Play();
                }
            }

            ModifySceneProperties(CoreEntry.gCurrentMapDesc);

        }
        
        //场景加载完(  这个mapID原本是表里的怪物组id不是地图id,  )
        void OnAfterLoadScene(int mapID)
        {


        }

        void ModifySceneProperties(LuaTable mapDesc)
        {
            //Debug.Break();
            if (mapDesc != null)
            {
                GameObject doorRoot = GameObject.Find("door");
                if (doorRoot != null)
                {
                    List<Door> doors = new List<Door>();

                    GameObject[] doorsObj = GameObject.FindGameObjectsWithTag("door"); //doorRoot.GetComponentsInChildren<Door>(true);
                    for (int i = 0; i < doorsObj.Length; ++i)
                    {
                        doors.Add(doorsObj[i].GetComponent<Door>());
                    }
                }
                else
                {
                    LogMgr.UnityError("can not find door or it is hidden");
                }
            }

        }

        //检测loading界面加载完，弹剧情界面 add by lzp 10:38 2015/7/1
        void DetectionLoadingEnd()
        {
            Invoke("DetectionLoadingEnd", 0.02f);
        }

        void OnRequestNanManAward(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sNanmanruqinAward data = parameter.msgParameter as MsgData_sNanmanruqinAward;

        }

        void StartDelayLoadMonster()
        {
        }

        void StartLoadMonster()
        {
        }

        public void KillAllMonster()
        {
        }

        //加载非主角
        void LoadMonster()
        {
            OneZoneData zoneData = m_gameDBMgr.GetAimPosZoneData(m_curMonsterGroupId, m_sceneMgr.curOpenZoneID);

            if (zoneData == null)
            {
                LogMgr.UnityError("Load zoneData error. mapid:" + m_curMonsterGroupId + " zoneid:" + m_sceneMgr.curOpenZoneID);
                return;
            }
            //add by lzp
            SetGuidSkill();

            StartLoadMonster();
            //所有的种怪
            SpawnMonster[] monsters = m_gameDBMgr.GetSpawnMonsterInfo(m_curMonsterGroupId);
            for (int i = 0; i < monsters.Length; ++i)
            {
                if (!monsters[i].enable)
                {
                    continue;
                }

                if (CurMonsterWaveSeq != monsters[i].waveSequence)
                {
                    continue;
                }

                Vector3 pos = monsters[i].pos;

                //是否当前区域怪
                if (zoneData != null && m_isLoadZone)
                {
                    if (pos.x < zoneData.startPos.x || pos.x > zoneData.endPos.x
                        || pos.z < zoneData.startPos.z || pos.z > zoneData.endPos.z)
                    {
                        continue;
                    }
                }

                //刷怪时间扰动,避免所有怪一起刷新动作太整齐的问题
                if (m_curWaveTriggerInfo.turbulenceTime > 0f)
                {
                    m_TurbulenceSpawnMonsters.Add(monsters[i]);
                }
                else
                {
                    SpawnActor(monsters[i]);
                }
            }

            //发送怪物被加载完了
            m_eventMgr.TriggerEvent(GameEvent.GE_EVENT_AFTER_MONSTER_LOADED, null);

            if (m_TurbulenceSpawnMonsters.Count > 0)
            {
                InvokeRepeating("TurbulenceSpawnMonster", Random.Range(0f, m_curWaveTriggerInfo.turbulenceTime / m_TurbulenceSpawnMonsters.Count),
                    m_curWaveTriggerInfo.turbulenceTime / m_TurbulenceSpawnMonsters.Count);
            }

            //m_bCurWaveLoaded = true;

            if (m_bWaveLoop == false && IsLoopMonsterAllDeath() == false)
            {
                m_bWaveLoop = true;
                TriggerLoopWave();
            }

            // 下一波怪在这波开始时延时刷新情况
            if (IsOpenZoneFinalWaveMonster() == false &&
                    (GetNextWaveTrigType() == WTC_TYPE.LASTWAVEBEGAN || GetNextWaveTrigType() == WTC_TYPE.ZONE))
            {
                CurMonsterWaveSeq++;
                UpdateWaveTriggerInfo();
                //m_bCurWaveLoaded = false;

                if (m_curWaveTriggerInfo.trigType == (int)WTC_TYPE.LASTWAVEBEGAN)
                {
                    if (m_curWaveTriggerInfo.arg1 > 0)
                    {
                        StartDelayLoadMonster();
                        Invoke("DelaySpawnMonster", m_curWaveTriggerInfo.arg1);
                    }
                    else
                    {
                        LoadMonster();
                    }
                }
                else if (m_curWaveTriggerInfo.trigType == (int)WTC_TYPE.ZONE)
                {
                    m_bNeedCheckZone = true;
                    SetMonsterTips(true);
                }
            }
        }

        private List<SpawnMonster> GetWaveSpawnMonsters(int zoneID, int waveSeq)
        {
            List<SpawnMonster> filterdMonsters = new List<SpawnMonster>();
            OneZoneData zoneData = m_gameDBMgr.GetAimPosZoneData(m_curMonsterGroupId, zoneID);
            SpawnMonster[] monsters = m_gameDBMgr.GetSpawnMonsterInfo(m_curMonsterGroupId);
            for (int i = 0; i < monsters.Length; ++i)
            {
                if (!monsters[i].enable)
                {
                    continue;
                }

                if (waveSeq != monsters[i].waveSequence)
                {
                    continue;
                }

                Vector3 pos = monsters[i].pos;

                //是否当前区域怪
                if (zoneData != null)
                {
                    if (pos.x < zoneData.startPos.x || pos.x > zoneData.endPos.x
                        || pos.z < zoneData.startPos.z || pos.z > zoneData.endPos.z)
                    {
                        continue;
                    }
                }

                filterdMonsters.Add(monsters[i]);
            }

            return filterdMonsters;
        }

        private SpawnMonster GetRandomSpawnMonster(List<SpawnMonster> monsters)
        {
            int index = Random.Range(0, monsters.Count);
            return monsters[index];
        }

        private Dictionary<int, int> GetNeedSupplyMonsters(List<SpawnMonster> totalMonsters)
        {
            return null;
        }

        private void TriggerLoopWave()
        {
        }

        public bool NoLoopForDebug = false;

        private void LoadLoopMonster()
        {

        }

        private void TurbulenceSpawnMonster()
        {
            if (m_TurbulenceSpawnMonsters.Count > 0)
            {
                SpawnActor(m_TurbulenceSpawnMonsters[0]);
                m_TurbulenceSpawnMonsters.RemoveAt(0);
            }
            else
            {
                CancelInvoke("TurbulenceSpawnMonster");
            }
        }
        
        GameObject SpawnActor(SpawnMonster spawnInfo)
        {
            GameObject obj = null;
            return obj;
        }
        
        IEnumerator OnBossEnter(int resID)
        {
            yield return StartCoroutine(CoreEntry.gCameraMgr._moveToTarget(resID));
            yield return StartCoroutine(CoreEntry.gCameraMgr._moveToPlayer());

        }

        public void PlayAllMonsterDeathAction()
        {
            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllMonsterActors();
            for (int i = 0; i < actorList.Count; ++i)
            {

                ActorObj actorBase = actorList[i];
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(actorBase.gameObject) == false)
                {
                    continue;
                }
                if (actorBase.mActorType == ActorType.AT_MONSTER
                    || actorBase.mActorType == ActorType.AT_BOSS
                    || actorBase.mActorType == ActorType.AT_MECHANICS)
                {
                    if (!actorBase.IsDeath())
                    {
                        actorBase.ChangeState(ACTOR_STATE.AS_DEATH);
                    }
                }
            }

        }

        public void pauseGame()
        {
            CoreEntry.GameStart = false;
            //CoreEntry.gTeamMgr.pauseAllPlayerAction();
            CoreEntry.gGameMgr.pauseAllMonsterAction();
        }

        public void resumeGame()
        {
            CoreEntry.GameStart = true;
        }
        /* 
         * 
         */
        public bool SpawnMonster(int MonsterResID, Vector3 position, out GameObject obj, bool isNpc = false)
        {
            //获取怪物信息表
            obj = null;
            return true;
        }

        public bool SpawnMonsterWithoutUsingObjectPool(int MonsterResID, Vector3 position, out GameObject obj, bool isNpc = false)
        {
            obj = null;
            return true;
        }

        //是否怪物全部死亡
        public bool IsMonsterAllDeath()
        {
            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllMonsterActors();
            for (int i = 0; i < actorList.Count; ++i)
            {
                ActorObj actorBase = actorList[i];
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(actorBase.gameObject) == false)
                {
                    continue;
                }

                //敌方阵营才算
                if (actorBase.m_TeamType == 3)
                {
                    if (actorBase.mActorType == ActorType.AT_MONSTER
                        || actorBase.mActorType == ActorType.AT_BOSS
                        || actorBase.mActorType == ActorType.AT_MECHANICS)
                    {
                        if (!actorBase.IsDeath())
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool HasMonsters()
        {
            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllMonsterActors();
            for (int i = 0; i < actorList.Count; ++i)
            {
                ActorObj actorBase = actorList[i];
                //敌方阵营才算
                if (actorBase.m_TeamType == 3)
                {
                    if (actorBase.mActorType == ActorType.AT_MONSTER
                        || actorBase.mActorType == ActorType.AT_BOSS
                        || actorBase.mActorType == ActorType.AT_MECHANICS)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //是否是最后一波怪
        bool IsOpenZoneFinalWaveMonster()
        {
            return false;
        }


        //总波数
        public int GetWaveMonsterCount()
        {
            int iTotalWave = 0;
            return iTotalWave;
        }

        public static bool checkValid(GameObject actors)
        {
            return true;
        }
        
        //HashSet<int> m_errorSet = new HashSet<int>(); 
        public void actorGameObjectListNullError(int index)
        {
            LogMgr.UnityError("actorGameObjectListNullError index:" + index);
            Invoke("DelayProcese_actorGameObjectListNullError", 0.001f);

        }

        public void AddObj(GameObject obj, ActorObj actorBase)
        {
        }

        public int CalculateMonstersNum()
        {
            return 0;
        }

        public void RemoveObj(GameObject obj, ActorObj actor)
        {
        }

        void Send_ALLMONSTER_DEATH()
        {
            m_eventMgr.TriggerEvent(GameEvent.GE_TASK_ALLMONSTER_DEATH, null);
        }

        bool IsLoopMonsterAllDeath()
        {
            return true;
        }

        void DelaySpawnMonster()
        {
            CancelInvoke("DelaySpawnMonster");
            LoadMonster();
        }

        public GameObject GetBossActorObject()
        {
            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllActors();
            for (int i = 0; i < actorList.Count; ++i)
            {
                //对IOS出现怪物不动 报错的异常  进行错误处理
                if (GameLogicMgr.checkValid(actorList[i].gameObject) == false)
                {
                    continue;
                }
                ActorObj actorBase = actorList[i];

                if (actorBase.mActorType == ActorType.AT_BOSS)
                {
                    return actorBase.gameObject;
                }
            }

            return null;
        }

        public ActorObj GetBossActorBase()
        {
            List<ActorObj> actorList = CoreEntry.gActorMgr.GetAllActors();
            for (int i = 0; i < actorList.Count; i++)
            {
                ActorObj actor = actorList[i];
                if (actor.mActorType == ActorType.AT_BOSS)
                    return actor;
            }

            return null;
        }

        //关闭场景渲染,用于检查是否为gpu瓶颈
        static bool bShowMesh = true;
        public void RenderMeshSwith()
        {
            bShowMesh = !bShowMesh;
            MeshRenderer[] meshList = FindObjectsOfType<MeshRenderer>();
            for (int i = 0; i < meshList.Length; i++)
            {
                meshList[i].enabled = bShowMesh;
            }

            SkinnedMeshRenderer[] skinMeshList = FindObjectsOfType<SkinnedMeshRenderer>();
            for (int i = 0; i < skinMeshList.Length; i++)
            {
                skinMeshList[i].enabled = bShowMesh;
            }

            ParticleSystem[] particleList = FindObjectsOfType<ParticleSystem>();
            for (int i = 0; i < particleList.Length; i++)
            {
                particleList[i].GetComponent<Renderer>().enabled = bShowMesh;
            }
        }

        public bool IsPvpState()
        {
            return m_PvpType != PVP_TYPE.PVP_TYPE_NOPE;
        }

        void SetMonsterTips(bool bA)
        {
            //技能释放成功
            EventParameter parameter = EventParameter.Get();
            parameter.intParameter = bA == true ? 1 : 0;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EVENT_MONSTER_TIPS, parameter);
        }
        //指引使用技能 add by lzp
        void SetGuidSkill()
        {

        }

        public IEnumerator WaitingForRemotePlayers()
        {
            yield return null;

        }

        public IEnumerator DelayExit()
        {
            yield return new WaitForSeconds(0.01f);

            MapMgr.Instance.EnterLoginOutScene();
        }

    }
}

