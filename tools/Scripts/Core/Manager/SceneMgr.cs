/**
* @file     : SceneMgr.cs
* @brief    : 
* @details  : 场景管理器
* @author   : 
* @date     : 2014-9-18 16:52
*/

using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SG
{

[Hotfix]
    public class SceneMgr : MonoBehaviour
    {
        //场景中的破碎物件
        List<GameObject> m_brokedObjList;

        //当前开放的区域
        private int m_curOpenZoneID = 0;

        public int curOpenZoneID
        {
            get { return m_curOpenZoneID; }
        }

        public GameObject[] brokedObjArray
        {
            get { return m_brokedObjList.ToArray(); }
        }


        //增加场景的BOSS的技能系数
        public float m_fBossSkillNum = 1.0f;

        private AudioSource m_audioSource = null;

        void Start()
        {
            m_brokedObjList = new List<GameObject>();
        }

		public void OnLevelWasLoaded(int level)
        {
            Scene loadedScene = SceneManager.GetActiveScene();

            if (!loadedScene.name.Equals("loadingscene") && !loadedScene.name.Equals("LoginOut"))
            {
                int curSceneId = MapMgr.Instance.GetCurSceneID();     //第一个场景ID

                if (curSceneId > 0)
                {
                    CoreEntry.InitUI();

                    if (ModuleServer.MS.GSkillCastMgr != null)
                    {
                        ModuleServer.MS.GSkillCastMgr.Enable();
                    }

                    MainPanelMgr.Instance.uiCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");

                    LoadSceneObjects(curSceneId);
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_AFTER_LOADSCENE, EventParameter.Get(curSceneId));
                }
            }
        }
      
        //关门
        public void CloseDoor()
        {
           
        }

        //主角是否在电梯上面
        public bool IsPlayerOnElevator()
        {
            return false;
        }

        public void RemoveBroked(GameObject brokedObj)
        {
            m_brokedObjList.Remove(brokedObj);
        }

        #region load scene objects
        public void ClearPools(int mapID)
        {
            if (mapID > 0)
            {
                CoreEntry.gActorMgr.ClearActor();
                CoreEntry.gEntityMgr.ClearObjs();
            }
        }

        public void InitPools(int mapID)
        {
            if(mapID > 0)
            {
                //CoreEntry.gActorMgr.ClearActor();
                //CoreEntry.gEntityMgr.ClearObjs();

                CoreEntry.gObjPoolMgr.initObjectPool();
                SceneDataMgr.Instance.LoadSceneData(mapID);
            }
        }
        /// <summary>
        /// 加载场景物体
        /// </summary>
        /// <param name="mapId">地图ID</param>
        private void LoadSceneObjects(int mapID)
        {
            CoreEntry.gCurrentMapDesc = ConfigManager.Instance.Map.GetMapConfig(mapID);
            CoreEntry.gSceneLoader.LoadPlayer();

            StartCoroutine(LoadAudio());
        }

        IEnumerator LoadAudio()
        {
            m_audioSource = null;
            GameObject aObj = GameObject.Find("Audio1");
            if (aObj != null)
            {
                m_audioSource = aObj.GetComponent<AudioSource>();
            }
            if (m_audioSource != null)
            {
                CoreEntry.g_CurrentSceneMusic = m_audioSource;
                //控制战斗场景的音乐
                m_audioSource.mute = !CoreEntry.cfg_bMusicToggle;
                if (!m_audioSource.mute)
                {
                    m_audioSource.Stop();
                }
            }

            yield return new WaitForSeconds(0.1f);
            CoreEntry.bMusicToggle = CoreEntry.cfg_bMusicToggle;
        }
        #endregion


        //同步加载场景
        public void LoadScene(string sceneName,bool async = false)
        {
            LoadModule.Instance.LoadScene(sceneName, null, async);
        }


        public bool SceneLoading
        {
            private set;
            get;
        }
        public void PreloadScene(string sceneName)
        {
            SceneLoading = true;
            LoadModule.Instance.LoadScene(sceneName, (o) =>{
                SceneLoading = false;
            }, true, false);
        }
    }
}