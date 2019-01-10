using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using XLua;

namespace SG
{

    [LuaCallCSharp]
    public class CoreEntry : SingletonMonoBehaviour<CoreEntry>
    {
        public enum GAME_PAGE
        {
            LOGIN = 0,
            ACTOR = 1,
            GAME = 2,
        }


        //private static GAME_PAGE m_gamePage = GAME_PAGE.LOGIN;

        private static bool m_bInitUI = false;

        public static bool m_bUseDistort = false;

        public static bool m_bUseExEff = false;  //是否使用简化特效


        static public bool GetInitUI()
        {
            return m_bInitUI;
        }
        //暂停游戏，以触发剧情  add by lzp 
        private static bool gameStart = true;

        public static bool GameStart
        {
            get { return CoreEntry.gameStart; }
            set
            {
                //    Debug.LogError("GameStart: " + value); 
                CoreEntry.gameStart = value;
            }
        }

        //add by Alex 20150327 记录进入场景前的页面名称,因为我们有很多个入口进入战斗(打擂台,打副本.........)
        public static string PanelNameBeforeEnterGame = "PanelChooseStage"; //给默认值防止出错
                                                                            //add by Alex 20150416 系统设置里的音效和音乐开关
        public static AudioSource g_CurrentSceneMusic;
        public static bool cfg_bMusicToggle = true;
       
        public static bool cfg_bEaxToggle = true;
        public static bool cfg_bQualityToggle = true;
        public static Dictionary<int, bool> cfg_bPushToggles = new Dictionary<int, bool>();


        public static bool bMusicToggle
        {
            get
            {
                return cfg_bMusicToggle;
            }
            set
            {
                cfg_bMusicToggle = value;

                GameObject aObj = GameObject.Find("Audio1");
                AudioSource aud = null;
                if (aObj != null)
                {
                    aud = aObj.GetComponent<AudioSource>();
                }
                 
                if(aud != null)
                {
                    aud.mute = !cfg_bMusicToggle;
                    if (cfg_bMusicToggle)
                    {
                        aud.volume = 1.0f;
                        aud.Play();
                    }
                    else
                    {
                        aud.Stop();
                    }
                }

            }
        }

        private static bool cfg_bModelShowToggle = true;
        public static bool bModelShow
        {
            get { return cfg_bModelShowToggle; }
            set
            {
                if (cfg_bModelShowToggle == value)
                {
                    return;
                }

                cfg_bModelShowToggle = value;

                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SETTING_ACTORDISPLAYNUM, null);
            }
        }

        public CoreEntry()
        {
            return;
        }


        public static float lastLoadSceneCompleteTime = 0;
        static bool m_bLoadSceneComplete = true;
        public static bool bLoadSceneComplete
        {
            get { return m_bLoadSceneComplete; }
            set { m_bLoadSceneComplete = value; }
        }

        /////////////////////////////////////////////////

        //凡是全局管理器，Awake函数中，建议不要引用跟其他管理器，初始化的顺序无法保证


        private static LogMgr m_logMgr = null;
        public static LogMgr gLogMgr
        {
            get
            {
                if (ReferenceEquals(null, m_logMgr))
                {
                    m_logMgr = CoreRootObj.AddComponent<LogMgr>();
                }
                return m_logMgr;
            }
            set { m_logMgr = value; }
        }

        /// <summary>
        /// 给外界借用StartCoroutine
        /// </summary>
        public static MonoBehaviour Helper
        {
            get
            {
                return gTimeMgr;
            }
        }

        private static GameLogicMgr m_gameMgr = null;
        public static GameLogicMgr gGameMgr
        {
            get
            {
                if (ReferenceEquals(null, m_gameMgr))
                {
                    m_gameMgr = CoreRootObj.AddComponent<GameLogicMgr>();

                }
                return m_gameMgr;
            }
            set { m_gameMgr = value; }
        }

        private static AutoAIMgr m_autoAIMgr = null;

        public static AutoAIMgr gAutoAIMgr
        {
            get
            {
                if (m_autoAIMgr == null && m_coreRootObj != null)
                {
                    m_autoAIMgr = CoreRootObj.AddComponent<AutoAIMgr>();
                }
                return m_autoAIMgr;
            }
        }


        private static ActorMgr m_actorMgr = null;
        public static ActorMgr gActorMgr
        {
            get
            {
                if (ReferenceEquals(null, m_actorMgr))
                {
                    m_actorMgr = CoreRootObj.AddComponent<ActorMgr>();
                }

                return m_actorMgr;
            }
        }

        private static EntityMgr m_entityMgr = null;
        public static EntityMgr gEntityMgr
        {
            get
            {
                if (ReferenceEquals(null, m_entityMgr))
                {
                    m_entityMgr = CoreRootObj.AddComponent<EntityMgr>();
                }

                return m_entityMgr;
            }
        }

        private static SceneLoader m_sceneLoader = null;
        public static SceneLoader gSceneLoader
        {
            get
            {
                if (ReferenceEquals(null, m_sceneLoader))
                {
                    m_sceneLoader = CoreRootObj.AddComponent<SceneLoader>();
                }

                return m_sceneLoader;
            }
        }

        private static BuffMgr m_BuffMgr = null;
        public static BuffMgr gBuffMgr
        {
            get
            {
                if (ReferenceEquals(null, m_BuffMgr))
                {
                    m_BuffMgr = CoreRootObj.AddComponent<BuffMgr>();
                }

                return m_BuffMgr;
            }
        }



        private static CameraMgr m_cameraMgr = null;
        public static CameraMgr gCameraMgr
        {
            get
            {
                if (ReferenceEquals(null, m_cameraMgr))
                {
                    m_cameraMgr = CoreRootObj.AddComponent<CameraMgr>();
                }
                return m_cameraMgr;
            }
            set { m_cameraMgr = value; }
        }

        private static TimeMgr m_TimeMgr = null;
        public static TimeMgr gTimeMgr
        {
            get
            {
                if (ReferenceEquals(null, m_TimeMgr))
                {
                    m_TimeMgr = CoreRootObj.AddComponent<TimeMgr>();
                }
                return m_TimeMgr;
            }
            set { m_TimeMgr = value; }
        }



        private static AudioMgr m_AudioMgr = null;
        public static AudioMgr gAudioMgr
        {
            get
            {
                if (m_AudioMgr == null)
                {
                    m_AudioMgr = CoreRootObj.AddComponent<AudioMgr>();
                }
                return m_AudioMgr;
            }
        }


        private static GameDBMgr m_gameDBMgr = null;
        public static GameDBMgr gGameDBMgr
        {
            get
            {
                if (m_gameDBMgr == null)
                {
                    m_gameDBMgr = CoreRootObj.AddComponent<GameDBMgr>();

                }
                return m_gameDBMgr;
            }
        }


        private static SkillMgr m_skillMgr = null;
        public static SkillMgr gSkillMgr
        {
            get
            {
                if (m_skillMgr == null)
                {

                }
                return m_skillMgr;
            }
            set { m_skillMgr = value; }
        }


        //private static SkillComboMgr m_skillComboMgr = null;
        //public static SkillComboMgr gSkillComboMgr
        //{
        //    get
        //    {
        //        if (m_skillComboMgr == null)
        //        {

        //        }
        //        return m_skillComboMgr;
        //    }
        //    set { m_skillComboMgr = value; }
        //}



        private static BaseTool m_baseTool = null;
        public static BaseTool gBaseTool
        {
            get
            {
                if (ReferenceEquals(null, m_baseTool))
                {
                    m_baseTool = CoreRootObj.AddComponent<BaseTool>();
                }
                return m_baseTool;
            }

        }


        private static MainPanelMgr m_PanelManager = null;
        public static MainPanelMgr gMainPanelMgr
        {
            get
            {
                if (ReferenceEquals(null, m_PanelManager))
                {
                    m_PanelManager = CoreRootObj.AddComponent<MainPanelMgr>();
                }
                return m_PanelManager;
            }

        }


        private static LuaMgr m_LuaMgr = null;
        public static LuaMgr gLuaMgr
        {
            get
            {
                if (ReferenceEquals(null, m_LuaMgr))
                {
                    m_LuaMgr = CoreRootObj.AddComponent<LuaMgr>();
                }
                return m_LuaMgr;
            }

        }




        private static ResourceLoader m_rcLoader = null;
        public static ResourceLoader gResLoader
        {
            get
            {
                if (ReferenceEquals(null, m_rcLoader))
                {
                    m_rcLoader = CoreRootObj.AddComponent<ResourceLoader>();
                }
                return m_rcLoader;
            }
        }

        private static SceneObjMgr m_sceneObjMgr = null;
        public static SceneObjMgr gSceneObjMgr
        {
            get
            {
                if (ReferenceEquals(null, m_sceneObjMgr))
                {
                    m_sceneObjMgr = m_coreRootObj.AddComponent<SceneObjMgr>();
                }

                return m_sceneObjMgr;
            }
        }

        private static SceneMgr m_sceneMgr = null;
        public static SceneMgr gSceneMgr
        {
            get
            {
                if (m_sceneMgr == null && m_coreRootObj != null)
                {
                    m_sceneMgr = m_coreRootObj.AddComponent<SceneMgr>();
                }
                return m_sceneMgr;
            }
        }

        private static MorphMgr m_morphMgr = null;
        public static MorphMgr gMorphMgr
        {
            get
            {
                if (ReferenceEquals(null, m_morphMgr))
                {
                    m_morphMgr = m_coreRootObj.AddComponent<MorphMgr>();
                }

                return m_morphMgr;
            }
        }

        private static ObjectPoolManager m_objPoolMgr = null;
        public static ObjectPoolManager gObjPoolMgr
        {
            get { return m_objPoolMgr; }
            set { m_objPoolMgr = value; }
        }

        // 使用PoolManager的对象池
        private static GameObjPoolMgr m_GameObjPoolMgr = null;
        public static GameObjPoolMgr gGameObjPoolMgr
        {
            get
            {
                if (m_GameObjPoolMgr == null)
                {
                    m_GameObjPoolMgr = CoreRootObj.AddComponent<GameObjPoolMgr>();
                }
                return m_GameObjPoolMgr;
            }
        }

        private static BehaviacSystem m_behaviacSys = null;
        public static BehaviacSystem gBehaviacSystem
        {
            get { return m_behaviacSys; }
            set { m_behaviacSys = value; }
        }


        private static EventMgr m_eventMgr = null;
        public static EventMgr gEventMgr
        {
            get
            {
                //if ( m_gamePage == GAME_PAGE.LOGIN)
                //{
                //    // 指定预初始化以及登陆界面时，不处理，以满足三种情况的战斗逻辑启动（连网、不连网、直接场景启动）
                //}

                //解决多线程访问 的问题
                if (ReferenceEquals(null, m_eventMgr))
                {
                    m_eventMgr = CoreRootObj.AddComponent<EventMgr>();


                    ExceptionCatch except = CoreRootObj.GetComponent<ExceptionCatch>();
                    if (except == null)
                    {
                        CoreRootObj.AddComponent<ExceptionCatch>();
                    }
                }
                return m_eventMgr;
            }
        }


        private static NetMgr m_netMgr = null;

        public static NetMgr netMgr
        {
            get
            {
                //解决多线程访问 的问题
                if (ReferenceEquals(null, m_netMgr))
                {
                    m_netMgr = CoreRootObj.AddComponent<NetMgr>();


                }
                return m_netMgr;
            }
        }

        //public static void SetGamePage(GAME_PAGE page)
        //{
        //    //m_gamePage = page;
        //}


        static GameObject m_coreRootObj;

        public static GameObject CoreRootObj
        {
            get
            {

                if (ReferenceEquals(null, m_coreRootObj))
                {
                    m_coreRootObj = new GameObject("CoreRoot");
                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(m_coreRootObj);
                    }

                }
                return m_coreRootObj;
            }
        }



        private static SoundEffectManager soundEffectMgr = new SoundEffectManager();

        public static SoundEffectManager SoundEffectMgr
        {
            get { return CoreEntry.soundEffectMgr; }
        }

        private static GameEffectManager gameEffectManager = new GameEffectManager();

        public static GameEffectManager GameEffectMgr
        {
            get { return CoreEntry.gameEffectManager; }
        }

        private static MonsterPoolMgr monsertPoolMgr = null;
        public static MonsterPoolMgr gMonsterPoolMgr
        {
            get
            {
                if (null == monsertPoolMgr)
                {
                    monsertPoolMgr = new MonsterPoolMgr();
                }

                return monsertPoolMgr;
            }
        }

        private static JoyStickMgr joystickMgr = null;
        public static JoyStickMgr gJoystickMgr
        {
            get
            {
                if (ReferenceEquals(null, joystickMgr))
                {
                    joystickMgr = CoreRootObj.AddComponent<JoyStickMgr>();
                }

                return joystickMgr;
            }
        }

        private static Bundle.HttpDownloadManager httpDownloaderMgr = null;
        public static Bundle.HttpDownloadManager gHttpDownloadMgr
        {
            get
            {
                if (ReferenceEquals(null, httpDownloaderMgr))
                {
                    httpDownloaderMgr = CoreRootObj.AddComponent<Bundle.HttpDownloadManager>();
                }

                return httpDownloaderMgr;
            }
        }

        private static MainThreadDispatcher mainThreadDispatcher = null;
        public static MainThreadDispatcher gMainThreadDispatcher
        {
            get
            {
                if (ReferenceEquals(null, mainThreadDispatcher))
                {
                    mainThreadDispatcher = CoreRootObj.AddComponent<MainThreadDispatcher>();
                }

                return mainThreadDispatcher;
            }
        }

        private static EventTouchMono eventTouchMono = null;

        public override void Init()
        {
            //避免多次初始化
            //DontDestroyOnLoad(CoreRootObj);

            Shader.DisableKeyword("_FOG_ON");
            Shader.DisableKeyword("DISTORT_OFF");


            //byte[] byteConfig = NGUITools.Load("SoundConfig");
            //if (null != byteConfig)
            //{
            //    string configs = System.Text.Encoding.UTF8.GetString(byteConfig);
            //    string[] userInfo = configs.Split('|');
            //    //启动游戏时读取配置信息进行初始化
            //    CoreEntry.cfg_bMusicToggle = userInfo.Length > 0 ? bool.Parse(userInfo[0]) : true;
            //    CoreEntry.cfg_bEaxToggle = userInfo.Length > 1 ? bool.Parse(userInfo[1]) : true;
            //    CoreEntry.cfg_bQualityToggle = userInfo.Length > 2 ? bool.Parse(userInfo[2]) : true;
            //}

            //byteConfig = NGUITools.Load("PushConfig");
            //if (null != byteConfig)
            //{
            //    string configs = System.Text.Encoding.UTF8.GetString(byteConfig);
            //    LitJson.JsonData data = LitJson.JsonMapper.ToObject(configs);
            //    for (int i = 0; i < data.Count; i++)
            //    {
            //        cfg_bPushToggles[(int)data[i]["id"]] = (bool)data[i]["value"];
            //    }
            //}
            //else
            //{
            //    CsvTable tbl = CsvMgr.Instance.getTable("Push");
            //    foreach (int id in tbl.mIntMap.Keys)
            //    {
            //        cfg_bPushToggles[id] = true;
            //    }
            //}


            //(GameObject.FindObjectOfType(typeof(AudioSource)) as AudioSource).mute = !CoreEntry.cfg_bMusicToggle;


            gEventMgr.enabled = true;
            netMgr.enabled = true;
            gLogMgr.enabled = true;
            gResLoader.enabled = true;



            ApplicationFunction af = CoreRootObj.GetComponent<ApplicationFunction>();
            //增加应用响应对象
            if (af == null)
            {
                af = CoreRootObj.AddComponent<ApplicationFunction>();
                //UI限制帧数只在一开始处理，防止重复被调用
                SetUIFrameRate();
            }
            af.netMgr = netMgr;

            //MsgPush push = CoreRootObj.GetComponent<MsgPush>();
            //if( push == null)
            //{
            //    push = CoreRootObj.AddComponent<MsgPush>();
            //}

            ActorDisplayMgr.Instance.Init();
        }



        //BUG 
        protected override void OnDestroy()
        {
            //LogMgr.UnityError("___________不能释放这个对象"); 

        }

        public static void CoreInit()
        {


            //设置横屏显示
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;

            //所有全局组件挂在CoreRoot对象下，CoreRoot对象被设为加载场景时不被删除，保证它的全局存在
            //GameObject go = new GameObject("CoreRoot");



            if (m_baseTool == null)
                m_baseTool = CoreRootObj.AddComponent<BaseTool>();

            //    if (    m_teamMgr   ==null )
            //m_teamMgr = CoreRootObj.AddComponent<TeamMgr>();

            //         if (m_challengeTeamManager == null)
            //         m_challengeTeamManager = CoreRootObj.AddComponent<ChallengeTeamManager>();

            if (m_skillMgr == null)
                m_skillMgr = CoreRootObj.AddComponent<SkillMgr>();

            //if (m_skillComboMgr == null)
            //    m_skillComboMgr = CoreRootObj.AddComponent<SkillComboMgr>();

            if (m_gameDBMgr == null)
                m_gameDBMgr = CoreRootObj.AddComponent<GameDBMgr>();


            if (m_objPoolMgr == null)
                m_objPoolMgr = CoreRootObj.AddComponent<ObjectPoolManager>();

            if (m_GameObjPoolMgr == null)
                m_GameObjPoolMgr = CoreRootObj.AddComponent<GameObjPoolMgr>();

            if (m_behaviacSys == null)
                m_behaviacSys = CoreRootObj.AddComponent<BehaviacSystem>();

            if (m_gameMgr == null)
                m_gameMgr = CoreRootObj.AddComponent<GameLogicMgr>();

            if (eventTouchMono == null)
                eventTouchMono = CoreRootObj.AddComponent<EventTouchMono>();

            //        TaskMgr task = TaskMgr.Instance;
            //        NpcMgr npc = NpcMgr.Instance;
            //        DungeonMgr dungeon = DungeonMgr.Instance;
            if (null == m_sceneObjMgr)
                m_sceneObjMgr = CoreRootObj.AddComponent<SceneObjMgr>();
            MoveDispatcher.Instance.Init();

            if (null == m_morphMgr)
                m_morphMgr = CoreRootObj.AddComponent<MorphMgr>();

        }


        public static void InitUI()
        {
            if (m_bInitUI)
            {

                return;
            }

            m_bInitUI = true;


            ModuleServer.MS.Initialize();


        }

        //是否
        static bool m_inFightScene;
        public static bool InFightScene
        {
            get { return m_inFightScene; }
            set { m_inFightScene = value; }
        }


        public static void SetUIFrameRate()
        {
            //SetFrameRate(CsvMgr.GetGlobalConfig().TargetFrame);
        }

        public static void SetBattleFrameRate()
        {
            //SetFrameRate(CsvMgr.GetGlobalConfig().BattleTargetFrame);
        }


        public static void SetFrameRate(int rate)
        {
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Application.targetFrameRate = rate;
            }
        }


        public static void leaveFight()
        {

            //CoreEntry.GameStart = false; 


            if (m_gameMgr == null)
            {
                return;
            }

            InFightScene = false;


            //m_teamMgr.enabled = false ;
            m_skillMgr.enabled = false;
            //m_skillComboMgr.enabled = false;
            m_baseTool.enabled = false;
            m_gameDBMgr.enabled = false;
            m_objPoolMgr.enabled = false;
            m_behaviacSys.enabled = false;
            m_gameMgr.enabled = false;
            //m_challengeTeamManager.enabled = false;
            //退出要把这个标记设置一下
            //PvpPlayerDataSource.Instance.GongChengType = PvpPlayerDataSource.GONGCHENG_TYPE.NORMAL;

            //PlayerAgent playerAgent = m_teamMgr.MainPlayer.GetComponent<PlayerAgent>();
            //playerAgent.enabled = false;


            ////退出非竞技场才做自动战斗记录
            //if (CoreEntry.gGameMgr.PvpType != PVP_TYPE.PVP_TYPE_ARENA &&
            //    CoreEntry.gGameMgr.PvpType != PVP_TYPE.PVP_TYPE_TAKE_LOVE &&
            //    CoreEntry.gGameMgr.PvpType != PVP_TYPE.PVP_TYPE_COMPETE &&
            //    CoreEntry.gGameMgr.PvpType != PVP_TYPE.PVP_TYPE_DUOBAO &&
            //    CoreEntry.gGameMgr.PvpType != PVP_TYPE.PVP_TYPE_GUILDWAR)
            //{
            //    CoreEntry.gGameMgr.RecordAutoFight = CoreEntry.gGameMgr.AutoFight;
            //}

            m_bInitUI = false;

            //add by Alex 20150328 11:15 //不管离开了怎样的战斗，都不再是pvp状态
            CoreEntry.gGameMgr.PvpType = PVP_TYPE.PVP_TYPE_NOPE;

            //广播退出副本消息
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FuBen_exit, null);

            //退出战斗 恢复 缩放
            CoreEntry.gTimeMgr.GlobalScale = 1f;

            //FrameManager.FM.DestroyAllUI(); //干扰主城 返回

            CoreEntry.gObjPoolMgr.ReleaseObjectPool();

            SetUIFrameRate();
        }

        public static LuaTable gCurrentMapDesc;

        public static bool IsMobaGamePlay()
        {
            if (gCurrentMapDesc == null)
                return false;

            return false;
        }

        public static GameObject LastMainCameraObject = null;

        public static void RecordMainCameraObject()
        {
            if (CoreEntry.gCameraMgr.MainCamera != null)
            {
                LastMainCameraObject = CoreEntry.gCameraMgr.MainCamera.gameObject;
            }
        }

        public static Camera GetMainCamera()
        {
            if (CoreEntry.gCameraMgr.MainCamera != null)
            {
                return CoreEntry.gCameraMgr.MainCamera;
            }
            else if (LastMainCameraObject != null)
            {
                return LastMainCameraObject.GetComponent<Camera>();
            }

            return null;
        }

        public static void enterFight()
        {

            if (m_gameMgr == null)
            {
                return;
            }
            DungeonMgr.bLoadFirstUI = true;
            CoreEntry.gAutoAIMgr.AutoFight = false;

            InFightScene = true;

            //m_teamMgr.enabled = true;
            m_skillMgr.enabled = true;
            //m_skillComboMgr.enabled = true;
            m_baseTool.enabled = true;
            m_gameDBMgr.enabled = true;
            m_objPoolMgr.enabled = true;
            m_behaviacSys.enabled = true;
            m_gameMgr.enabled = true;
            //m_challengeTeamManager.enabled = true; 

            CoreEntry.gGameMgr.resumeGame();

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FuBen_enter, null);

            //进入战斗   缩放
            CoreEntry.gTimeMgr.GlobalScale = 1.0f;

            SetBattleFrameRate();


        }


        static public bool IsEditor()
        {
            // return false;
            return Application.platform == RuntimePlatform.WindowsEditor;
        }

        public bool NeedWaitingForReady()
        {
            return false;
        }

        public IEnumerator WaitingForReady()
        {
            if (gGameMgr != null)
            {
                //是否要等待网络玩家
                yield return StartCoroutine(gGameMgr.WaitingForRemotePlayers());
            }

        }
    }
}

