
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using UnityEngine.SceneManagement;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class MapMgr
    {
        public static string mainCityMapName //= "cs_map_zhucheng";  
        {
            get
            {
                string strtmp = "cs_zc_03";
                return strtmp;
            }
        }

        static MapMgr mInstance;
        public static MapMgr Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new MapMgr();
                }
                return mInstance;
            }
        }


        bool m_inMainCity = false;
        public bool InMainCity
        {
            get 
            {
                return m_inMainCity; 
            }
            set { m_inMainCity = value; }
        }

        private int m_EnterMapId;

        public int EnterMapId
        {
            get { return m_EnterMapId; }
            set { m_EnterMapId = value; }
        }


        /// <summary>
        /// 分线。
        /// </summary>
        private int m_Line;

        /// <summary>
        /// 获取或设置当前所在分线。
        /// </summary>
        public int Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        private Vector2 m_PlayerInitPos;
        public Vector2 PlayerInitPos
        {
            get { return m_PlayerInitPos; }
            set { m_PlayerInitPos = value; }
        }

        private Vector3 m_PlayerInitDir;
        public Vector3 PlayerInitDir
        {
            get { return m_PlayerInitDir; }
        }

        public Dictionary<string, Texture2D> m_TextureSet = new Dictionary<string, Texture2D>();

        public MapMgr()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_ENTER_SCENE, GE_SC_ENTER_SCENE);            
        }

        public enum MapType
        {
            Map_None = -2,
            Map_Login = -1,//登录
            Map_SelectRole = 0,//选择角色
            Map_Common = 1,//普通场景
            Map_CrossServer = 2,//跨服务器场景
        }

        private MapType m_curMapType;
        public MapType CurMapType
        {
            get { return m_curMapType; }
        }
        private MapType m_lastMapType;
        public MapType LastMapType
        {
            get { return m_lastMapType; }
        }

        public int enterType = -1;

        private int m_EnterMapType;
        /// <summary>
        /// 地图刷怪类型
        /// </summary>
        public int EnterMapType
        {
            get
            {
                return m_EnterMapType;
            }
            set
            {
                m_EnterMapType = value;
            }
        }

        public int EnterType
        {
            get;
            set;
        }

        public Texture2D GetTexture2D(string strKey)
        {
            if(m_TextureSet.ContainsKey(strKey))
            {
                return m_TextureSet[strKey];
            }
            return null;
        }

        public void AddTexture2D(string strKey, Texture2D value)
        {
            if (m_TextureSet.ContainsKey(strKey))
            {
                m_TextureSet[strKey] = value;
            }
            else
            {
                m_TextureSet.Add(strKey, value);
            }
        }

        public bool FirstLoad;
        private void GE_SC_ENTER_SCENE(GameEvent ge, EventParameter parameter)
        {
            if (FirstLoad)
            {
                SDKMgr.Instance.TrackGameLog("6000", "进入loading");
            }

            MsgData_sEnterScene data = parameter.msgParameter as MsgData_sEnterScene;
            m_Line = data.LineID;
            m_PlayerInitPos = new Vector2((float)data.PosX, (float)data.PosY);
            m_PlayerInitDir = new Vector3(0f, (float)data.Dir, 0f);
            m_EnterMapId = data.MapID;
            enterType = data.EnterType;
            DoEnterScene();
        }
      
        private void DoEnterScene()
        {
            LuaTable mapConfig = ConfigManager.Instance.Map.GetMapConfig(m_EnterMapId);
            if (mapConfig == null)
            {
                return;
            }
            m_EnterMapType = mapConfig.Get<int>("monsterType");
            m_lastMapType = m_curMapType;
            m_curMapType = GetMapType(mapConfig.Get<int>("type"));

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BEGIN_LOADSCENE, null);

            MainRole.Instance.mapid = m_EnterMapId;
            //CoreEntry.SetGamePage(CoreEntry.GAME_PAGE.GAME);
            CoreEntry.CoreInit();
            CoreEntry.enterFight();
            CoreEntry.InFightScene = false;
            MapMgr.Instance.InMainCity = true;


            LoadingWindowImpl.LoadScene(mapConfig.Get<string>("res"));
        }

        public void Init()
        {
            m_curMapType = MapType.Map_None;
            m_lastMapType = MapType.Map_None;
        }

        /// <summary>
        /// 进入场景
        /// </summary>
        /// <param name="id"></param>
        public void DoEnterScene(int id)
        {
            m_EnterMapId = id;

            DoEnterScene();
        }

        /// <summary>
        /// 进入初始化场景
        /// </summary>
        public void EnterInitScene()
        {
            SceneManager.LoadScene("Init");
            //CoreEntry.gSceneMgr.LoadScene("Scene/allMap/ui/Init");
        }
        /// <summary>
        /// 进入更新资源场景
        /// </summary>
        public void EnterUpdateScene()
        {
            //CoreEntry.gResLoader.LoadLevel("UpdateScene");
            //CoreEntry.gSceneMgr.LoadScene("Scene/allMap/ui/Init");
            SceneManager.LoadScene("Init");
        }

        /// <summary>
        /// 登出场景
        /// </summary>
        public void EnterLoginOutScene()
        {
            m_lastMapType = m_curMapType;
            m_curMapType = MapType.Map_Login;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, null);

            Scene cur = SceneManager.GetActiveScene();
            if (!cur.name.Equals("LoginUI"))
            {
                SceneManager.LoadScene("LoginOut");
            }
            else
            {
                EnterLoginScene();
            }
        }

        /// <summary>
        /// 进入登录场景
        /// </summary>
        public void EnterLoginScene()
        {
            m_lastMapType = m_curMapType;
            m_curMapType = MapType.Map_Login;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, null);

            MainPanelMgr.gotoPanel("LoginPanel");
            CoreEntry.gSceneMgr.LoadScene("Scene/allMap/ui/LoginUI");

            EnterType = 0;
            FirstLoad = true;
        }

        /// <summary>
        /// 进入角色选择场景
        /// </summary>
        public void EnterRoleSelectScene()
        {
            m_lastMapType = m_curMapType;
            m_curMapType = MapType.Map_SelectRole;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BEGIN_LOADSCENE_CREATE_ROLE, null);
            MainPanelMgr.gotoPanel("UIRoleSelect");
            CoreEntry.gSceneMgr.LoadScene("Scene/allMap/ui/RoleUI");

            EnterType = 0;
            FirstLoad = true;
        }

        private MapType GetMapType(int type)
        {
            if (type == (int)ESceneType.SCENE_TYPE_CROSS_ARENA || type == (int)ESceneType.SCENE_TYPE_CROSS_BOSS ||
                type == (int)ESceneType.SCENE_TYPE_CROSS_CITYWAR || type == (int)ESceneType.SCENE_TYPE_CROSS_PRE_ARENA ||
                type == (int)ESceneType.SCENE_TYPE_CROSS_PVP || type == (int)ESceneType.SCENE_TYPE_CROSS_PVP3 ||
                type == (int)ESceneType.SCENE_TYPE_CROSS_QUEST || type == (int)ESceneType.SCENE_TYPE_CROSS_TAOTAI ||
                type == (int)ESceneType.SCENE_TYPE_CROSS_TASK || type == (int)ESceneType.SCENE_TYPE_CROSS_TEAM_DUPL 
                || type == (int)ESceneType.SCENE_TYPE_CROSS_WAR 
                || type == (int)ESceneType.SCENT_TYPE_CROSS_FLAG
                || type == (int)ESceneType.SCENE_TYPE_CROSS_ARTIFACT)
            {
                return MapType.Map_CrossServer;
            }
            else
            {
                return MapType.Map_Common;
            }
        }

        public int GetCurSceneID()
        {
            if (m_curMapType == MapType.Map_None || m_curMapType == MapType.Map_Login || m_curMapType == MapType.Map_SelectRole)
            {
                return 0;
            }

            return m_EnterMapId;
        }
        /// <summary>
        /// 获取当前地图刷怪类型(配置表monsterType值)
        /// </summary>
        /// <returns></returns>
        public int GetCurMapType()
        {
            if (m_curMapType == MapType.Map_None || m_curMapType == MapType.Map_Login || m_curMapType == MapType.Map_SelectRole)
            {
                return 0;
            }
            return m_EnterMapType;
        }

        //是否跨服场景中
        public bool IsCrossMap()
        {
            return CurMapType == MapType.Map_CrossServer;
        }

    }
}