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
        public Sprite[] BackSprite;

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

            CoreEntry.gResLoader.ClearPrefabs();
            LoadModule.Instance.Clear();
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
            if (BackSprite.Length > 2) return;
            string[] bg_path = new string[3]
            {
                "UI/Atlas/RAW/Bg_loading2",
                "UI/Atlas/RAW/Bg_loading3",
                "UI/Atlas/RAW/Bg_loading4"
            };
            List<Sprite> splist = new List<Sprite>();
            splist.Add(BackSprite[0]);
            for (int i = 0; i < bg_path.Length; ++i)
            {
                UnityEngine.Object o = CoreEntry.gResLoader.LoadResource(bg_path[i]);
                if (o == null) continue;

                splist.Add((o as GameObject).GetComponent<Image>().sprite);

            }
            BackSprite = new Sprite[splist.Count];
            for (int i = 0; i < BackSprite.Length; ++i)
            {
                BackSprite[i] = splist[i];
                // Debug.LogError("===" + BackSprite[i].name);
            }

            if (BackSprite != null && BackSprite.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, BackSprite.Length);
                if (index < BackSprite.Length)
                {
                    string strBgImage = "Bg_loading" + index;
                    if(index == 0)
                    {
                        strBgImage = "Bg_loading";
                    }
                    if (BackImage)
                    {
                        if (BackImage)
                        {
                            Debug.Log("-------------------------------------------------" + strBgImage);
                            CommonTools.SetLoadImage(BackImage, ClientSetting.Instance.GetStringValue(strBgImage),1);
                        }
                    }
                    //BackImage.sprite = BackSprite[index];
                }
            }
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