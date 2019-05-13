using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using XLua;
using Bundle;
using System;

namespace SG
{

[Hotfix]
    public class LoadingWindowImpl : MonoBehaviour
    {
        public Sprite[] BackSprite;   //兼容外网保留

        public Text TipInfo;
        public Slider LoadingProgress;
        public Transform EffectTran;
        public string EffectName;
        public MeshRenderer EffectMR;
        public Image BackImage;

        public static string strNextLevel;

        public GameObject sloder;
        public Text _tip;

        //过审开关 0/关，1/开
        int IS_REVIEW = 0;
        string[] _strtip = new string[4] { "组队秘境可以获取大量的资源！",
        "建议优先完成主线任务！","任务都做完了可以打boss获取大量装备","跨服战场可以随意PK的"};


        public static void LoadScene(string strLevel)
        {
            strNextLevel = strLevel;
            CoreEntry.gSceneMgr.LoadScene("Scene/allMap/ui/loadingscene"); //SceneManager.LoadScene("loadingscene");                         //改成空场景过渡 减少最大内存消耗

            MainPanelMgr.Instance.Release();
            MainPanelMgr.Instance.ShowPanel("PanelLoadingWindow");
        }

        // Use this for initialization
        void Start()
        {
            IS_REVIEW = ClientSetting.Instance.GetIntValue("IS_REVIEW");
            UISwitchScene.IS_REVIEW = IS_REVIEW;
            SetIS_REVIEW();

            LoadingProgress.value = 0;
            if (null == EffectMR)
            {
                Transform effect = EffectTran.Find(EffectName);
                if (null != effect)
                {
                    EffectMR = effect.GetComponent<MeshRenderer>();
                }
            }
            if (null != EffectMR)
            {
                EffectMR.material.SetTextureOffset("_mask", Vector2.zero);
            }

            SetBackSprite();

            //sen
            ClearScene();

            _wait = new WaitForEndOfFrame();
            LoadModule.Instance.LoadScene(strNextLevel, delegate (object data)
            {
                _realLoad = true;
            }, true);
            StartCoroutine(loadScene());
        }

        private void ClearScene()
        {
            AtlasSpriteManager.Instance.ClearCache();
            CoreEntry.gSceneMgr.ClearPools(MapMgr.Instance.GetCurSceneID());
            CoreEntry.gObjPoolMgr.ReleaseObjectPool();
            CoreEntry.gGameObjPoolMgr.ClearPool();
            FlyAttrManager.CloseAllFlyAttr();

            CoreEntry.gResLoader.ClearPrefabs();

            bool cache = true;
            Scene cur = SceneManager.GetActiveScene();
            if (cur.name.Equals("RoleUI"))
            {
                cache = false;
            }
            LoadModule.Instance.Clear(cache);
        }

        void SetIS_REVIEW()
        {
            if (IS_REVIEW == 0)
            {
                sloder.SetActive(true);
            }
            else
            {
                sloder.SetActive(false);
                StartCoroutine(SetTip());
            }
        }

        IEnumerator SetTip()
        {
            _tip.text = _strtip[UnityEngine.Random.Range(0, _strtip.Length)];
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SetTip());
            yield break;
        }

        void OnDestroy()
        {
            BackImage.sprite = null;
            MainPanelMgr.Instance.Init();
      
            if(ArenaMgr.Instance.IsArenaScene == false)
            {
                MainPanelMgr.Instance.ShowDialog("JoystickPanel");
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FRIST_UI_LOADED, null);
        }

        void OnDisable()
        {
            LevelLoader.OnAsyncLoading -= GE_LOADSCENE_PROGRESS;
        }

        void OnEnable()
        {
            LevelLoader.OnAsyncLoading += GE_LOADSCENE_PROGRESS;
            CoreEntry.gTimeMgr.TimeScale = 1;
        }

        void SetBackSprite()
        {
            CommonTools.SetLoadImage(BackImage, "Bg_loading", 1);
        }

        void GE_LOADSCENE_PROGRESS(AsyncOperation oprate, float step)
        {
            _sceneOperat = oprate;
            _sceneStep = step;
        }

        void loadsceneFinished()
        {
            CoreEntry.lastLoadSceneCompleteTime = Time.time;
            Bundle.AssetBundleLoadManager.Instance.ReleaseAllAssetBundle();
            CancelInvoke();
            MainPanelMgr.Instance.Release();
        }

        //sen
        private AsyncOperation _sceneOperat;
        private float _sceneStep;

        private WaitForEndOfFrame _wait;
        private bool _realLoad = false;

        //注意这里返回值一定是 IEnumerator    
        IEnumerator loadScene()
        {
            int displayProgress = 0;
            int toProgress = 0;

            int randPro = UnityEngine.Random.Range(40, 70);

            while (!_realLoad)
            {
                while (displayProgress < randPro)    //预加载
                {
                    ++displayProgress;
                    SetProgressBar(displayProgress);
                    yield return _wait;
                }
                yield return _wait;
            }

            CoreEntry.gSceneMgr.InitPools(MapMgr.Instance.GetCurSceneID());
            if (ModuleServer.MS.GSkillCastMgr != null)
            {
                ModuleServer.MS.GSkillCastMgr.Disable();
            }

            while (_sceneStep < 0.90f)
            {
                toProgress = (int)(_sceneStep * 100);
                while (displayProgress < toProgress)
                {
                    ++displayProgress;
                    SetProgressBar(displayProgress);
                    yield return _wait;
                }
                yield return _wait;
            }

            MainPanelMgr.ClearTexture2D();
            yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue(string.Format("Bg_loading{0}", UnityEngine.Random.Range(0, 4))), "Bg_loading"));

            toProgress = 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetProgressBar(displayProgress);
                yield return _wait;
            }

            if (null != _sceneOperat)
            {
                _sceneOperat.allowSceneActivation = true;
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_LOADSCENE_FINISH, EventParameter.Get(1));
            SDKMgr.Instance.addBatchDataEvent(1, SG.Account.Instance.ServerId, "10230", " 加载场景完成" + strNextLevel);

            loadsceneFinished();
        }

        private void SetProgressBar(int displayProgress)
        {
            float floatParameter = displayProgress / 100.0f;
            LoadingProgress.value = floatParameter;
            if (null == EffectMR)
            {
                Transform effect = EffectTran.Find(EffectName);
                if (null != effect)
                {
                    EffectMR = effect.GetComponent<MeshRenderer>();
                }
            }
            if (null != EffectMR)
            {
                Vector2 offset = new Vector2(-1f + floatParameter * 1.1f, 0f);
                EffectMR.material.SetTextureOffset("_mask", offset);
            }
        }
    }
}