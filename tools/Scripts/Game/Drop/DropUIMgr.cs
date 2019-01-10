/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

namespace SG
{
[Hotfix]
    public class DropUIMgr : IModule
    {
        private Transform mDropTextRoot;
        public Transform DropTextRoot
        {
            get
            {
                if (mDropTextRoot == null)
                {
                    string path = "UI/Prefabs/Drop/FirstRes/DropUIRoot";
                    GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
                    if(prefab != null)
                    {
                        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
                        obj.name = "DropTextRoot";
                        obj.SetActive(true);
                        Canvas canvas = obj.GetComponent<Canvas>();
                        canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
                        canvas.sortingOrder = 500;
                        GameObject.DontDestroyOnLoad(obj);
                        mDropTextRoot = obj.transform;
                    }
                }
                return mDropTextRoot;
            }
        }

        private Transform mDropFlyRoot;
        public Transform DropFlyRoot
        {
            get
            {
                if (mDropFlyRoot == null)
                {
                    string path = "UI/Prefabs/Drop/FirstRes/DropUIRoot";
                    GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
                    if(prefab!= null)
                    {
                        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
                        obj.name = "DropFlyRoot";
                        obj.SetActive(true);
                        Canvas canvas = obj.GetComponent<Canvas>();
                        canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
                        canvas.sortingOrder = 700;
                        GameObject.DontDestroyOnLoad(obj);
                        mDropFlyRoot = obj.transform;
                    }

                }

                return mDropFlyRoot;
            }
        }

        private Camera uiCamera = null;

        //掉落UI
        public Vector3 DropTextOffset = new Vector3(0f, 0.1f, 0f);

[Hotfix]
        protected class DropText
        {
            public GameObject dropItemObj;
            public GameObject rootObj;
            public Image textBg;
            public Text text;
        }

        private List<DropText> mDropTextList = new List<DropText>();
        private List<DropText> mUnusedList = new List<DropText>();

        private static float f = 1 / 255.0f;
        private List<Color> mTextColor = new List<Color> 
        { 
            new Color(255 * f, 255 * f, 255* f),
			new Color(32 * f, 235 * f, 109 * f),
            new Color(52 * f, 206 * f, 239 * f),
            new Color(181 * f, 127 * f, 255 * f),
            new Color(232 * f, 192 * f, 31 * f),
			new Color(255 * f, 252 * f, 0f),
            new Color(255 * f, 0f, 0f),
			new Color(255 * f, 0f, 0f),
        };
        private List<Color> mOutlineColor = new List<Color> 
        { 
            new Color(34 * f, 34 * f, 34* f),
			new Color(4 * f, 24 * f, 2* f),
            new Color(0f, 38 * f, 108 * f),
            new Color(17 * f, 5 * f, 31 * f),
            new Color(49 * f, 17 * f, 0f),
			new Color(255 * f, 252 * f, 0f),
            new Color(27 * f, 0f, 0f),
			new Color(27 * f, 0f, 0f),
        };

        private List<Color> mTipsTextColor = new List<Color> 
        { 
            new Color(255f, 255 * f, 255* f),
			new Color(32 * f, 235 * f, 109 * f),
            new Color(52 * f, 206 * f, 239 * f),
            new Color(181 * f, 127 * f, 255 * f),
            new Color(232 * f, 192 * f, 31 * f),
			new Color(255 * f, 252 * f, 0f),
            new Color(255 * f, 0f, 0f),
			new Color(255 * f, 0f, 0f),
        };
        private List<Color> mTipsOutlineColor = new List<Color> 
        { 
            new Color(34 * f, 34 * f, 34* f),
			new Color(4 * f, 24 * f, 2* f),
            new Color(0f, 38 * f, 108 * f),
            new Color(17 * f, 5 * f, 31 * f),
            new Color(49 * f, 17 * f, 0f),
			new Color(255 * f, 252 * f, 0f),
            new Color(27 * f, 0f, 0f),
			new Color(27 * f, 0f, 0f),
        };

        // 拾取UI
        private static string mDropFlyItemPath = "UI/Prefabs/Drop/FirstRes/DropFlyItem";
        private List<DropFlyItem> mDropFlyItemList = new List<DropFlyItem>();
        private List<DropFlyItem> mCachedFlyItemList = new List<DropFlyItem>();

        private static string mDropGoldFlyItemPath = "UI/Prefabs/Drop/FirstRes/DropGoldFlyItem";
        private List<DropGoldFlyItem> mDropGoldFlyItemList = new List<DropGoldFlyItem>();
        private List<DropGoldFlyItem> mCachedGoldFlyItemList = new List<DropGoldFlyItem>();

        private static string mDropFlyTipItemPath = "UI/Prefabs/Drop/FirstRes/DropFlyTipItem";
        private List<DropFlyTipItem> mDropFlyTipItemList = new List<DropFlyTipItem>();
        private List<DropFlyTipItem> mCachedFlyTipItemList = new List<DropFlyTipItem>();
        private Queue<FlyTipInfo> mProcessFlyTips = new Queue<FlyTipInfo>();
        private float mLastTime = 0.0f;
        private float mIntervalTime = 0.8f;

        //----------- 每个管理器必须写的方法 ----------
        public override bool LoadSrv(IModuleServer IModuleSrv)
        {
            ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
            moduleSrv.GDropTextMgr = this;

            return true;
        }

        public override void InitializeSrv()
        {
            uiCamera = MainPanelMgr.Instance.uiCamera;

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnBeginSceneLoad);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE, OnBeginSceneLoad);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, OnBeginSceneLoad);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE_CREATE_ROLE, OnBeginSceneLoad);
            EventToUI.RegisterEvent("EU_ADD_DROPITEMTEXT", OnEvent);
            EventToUI.RegisterEvent("EU_REMOVE_DROPITEMTEXT", OnEvent);
            EventToUI.RegisterEvent("EU_SHOW_DROPFLY", OnEvent);
            EventToUI.RegisterEvent("EU_SHOW_DROPGOLDGFLY", OnEvent);
            EventToUI.RegisterEvent("EU_SHOW_DROPFLYTIP", OnEvent);
        }

        public static IModule Newer(GameObject go)
        {
            IModule module = go.AddComponent<DropUIMgr>();

            return module;
        }
        //-------------------------------------------
        void Update()
        {
            float realTime = Time.realtimeSinceStartup;
            for (int i = 0; i < mDropFlyItemList.Count; i++)
            {
                DropFlyItem item = mDropFlyItemList[i];
                if (null != item)
                {
                    item.UpdateItem(realTime);

                    if (realTime > item.GetStayTime())
                    {
                        RecycleDropFlyItem(item, true);
                    }
                }
            }
            for (int i = 0; i < mDropGoldFlyItemList.Count; i++)
            {
                DropGoldFlyItem item = mDropGoldFlyItemList[i];
                if (null != item)
                {
                    item.UpdateItem(realTime);

                    if (realTime > item.GetAnimationTime() + item.mAniStart)
                    {
                        RecycleDropGoldFlyItem(item);
                    }
                }
            }
            for (int i = 0; i < mDropFlyTipItemList.Count; i++)
            {
                DropFlyTipItem item = mDropFlyTipItemList[i];
                if (null != item)
                {
                    item.UpdateItem(realTime);

                    if (realTime > item.GetAnimationTime() + item.mAniStart)
                    {
                        RecycleDropFlyTipItem(item);
                    }
                }
            }

            if (Time.realtimeSinceStartup - mLastTime > mIntervalTime)
            {
                if (mProcessFlyTips.Count > 0)
                {
                    FlyTipInfo info = mProcessFlyTips.Dequeue();

                    ShowDropFlyTipItem(info.name, info.quality);

                    mLastTime = Time.realtimeSinceStartup;
                }
            }
        }

        void LateUpdate()
        {
            if (null != uiCamera)
            {
                for (int i = 0; i < mDropTextList.Count; i++)
                {
                    GameObject dropItemObj = mDropTextList[i].dropItemObj;
                    if (null != dropItemObj && null != CoreEntry.gCameraMgr.MainCamera)
                    {
                        Vector3 screenPos = CoreEntry.gCameraMgr.MainCamera.WorldToScreenPoint(dropItemObj.transform.position);
                        Vector3 worldPos = uiCamera.ScreenToWorldPoint(screenPos);

                        mDropTextList[i].rootObj.transform.position = worldPos + DropTextOffset;
                    }
                }
            }
        }

        private void OnEvent()
        {
            if (EventToUI.sEvent.CompareTo("EU_ADD_DROPITEMTEXT") == 0)
            {
                GameObject dropItemObj = (GameObject)EventToUI.GetArg(UIEventArg.Arg1);
                string text = (string)EventToUI.GetArg(UIEventArg.Arg2);
                int quality = (int)EventToUI.GetArg(UIEventArg.Arg3);
                if (-1 == quality)
                    return;

                Create(dropItemObj, text, quality);
            }
            else if (EventToUI.sEvent.CompareTo("EU_REMOVE_DROPITEMTEXT") == 0)
            {
                GameObject dropItemObj = (GameObject)EventToUI.GetArg(UIEventArg.Arg1);
                for (int i = 0; i < mDropTextList.Count; ++i)
                {
                    if (mDropTextList[i].dropItemObj == dropItemObj)
                    {
                        RecycleDropText(mDropTextList[i]);
                    }
                }
            }
            else if (EventToUI.sEvent.CompareTo("EU_SHOW_DROPFLY") == 0)
            {
                string icon = (string)EventToUI.GetArg(UIEventArg.Arg1);
                Vector3 from = (Vector3)EventToUI.GetArg(UIEventArg.Arg2);
                Vector3 to = (Vector3)EventToUI.GetArg(UIEventArg.Arg3);
                string itemName = (string)EventToUI.GetArg(UIEventArg.Arg4);
                int quality = (int)EventToUI.GetArg(UIEventArg.Arg5);

                ShowDropFlyItem(Time.realtimeSinceStartup, icon, from, to, itemName, quality);
            }
            else if (EventToUI.sEvent.CompareTo("EU_SHOW_DROPGOLDGFLY") == 0)
            {
                int count = (int)EventToUI.GetArg(UIEventArg.Arg1);
                Vector3 pos = (Vector3)EventToUI.GetArg(UIEventArg.Arg2);

                ShowDropGoldFlyItem(count, pos);
            }
            else if (EventToUI.sEvent.CompareTo("EU_SHOW_DROPFLYTIP") == 0)
            {
                FlyTipInfo info = new FlyTipInfo();

                info.name = (string)EventToUI.GetArg(UIEventArg.Arg1);
                info.quality = (int)EventToUI.GetArg(UIEventArg.Arg2);

                mProcessFlyTips.Enqueue(info);
            }
        }

        /// <summary>
        /// 删除残留内容
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnBeginSceneLoad(GameEvent ge, EventParameter parameter)
        {
            while (mDropTextList.Count > 0)
            {
                RecycleDropText(mDropTextList[0]);
            }

            while (mDropFlyItemList.Count > 0)
            {
                RecycleDropFlyItem(mDropFlyItemList[0]);
            }

            while (mDropGoldFlyItemList.Count > 0)
            {
                RecycleDropGoldFlyItem(mDropGoldFlyItemList[0]);
            }

            while (mDropFlyTipItemList.Count > 0)
            {
                RecycleDropFlyTipItem(mDropFlyTipItemList[0]);
            }
            mProcessFlyTips.Clear();
        }

        /// <summary>
        /// 掉落文本
        /// </summary>
        /// <param name="dropItemObj"></param>
        /// <param name="text"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        private DropText Create(GameObject dropItemObj, string text, int quality)
        {
            DropText dt = null;

            if (mUnusedList.Count > 0)
            {
                int index = mUnusedList.Count - 1;
                dt = mUnusedList[index];
                mUnusedList.RemoveAt(index);
                dt.dropItemObj = dropItemObj;
                dt.text.text = text;
                float f1 = System.Text.Encoding.Default.GetBytes(text).Length;
                dt.textBg.rectTransform.sizeDelta = new Vector2(f1 * 9f, 25f);
                dt.text.color = mTextColor[quality];
                Outline textOutline = dt.text.GetComponent<Outline>();
                textOutline.effectColor = mOutlineColor[quality];
                dt.rootObj.SetActive(true);

                mDropTextList.Add(dt);

                return dt;
            }

            GameObject dropGo = (GameObject)Instantiate(CoreEntry.gResLoader.LoadResource("UI/Prefabs/Drop/FirstRes/DropText"));
            if (null == dropGo)
            {
                return null;
            }

            dt = new DropText();
            dt.dropItemObj = dropItemObj;
            dt.rootObj = dropGo;
            dt.rootObj.transform.SetParent(DropTextRoot);
            dt.rootObj.transform.localScale = Vector3.one;
            dt.textBg = dropGo.transform.Find("Bg").GetComponent<Image>();
            dt.text = dropGo.transform.Find("Text").GetComponent<Text>();

            dt.text.text = text;
            float f2 = System.Text.Encoding.Default.GetBytes(text).Length;
            dt.textBg.rectTransform.sizeDelta = new Vector2(f2 * 9f, 25f);
            dt.text.color = mTextColor[quality];
            Outline txtOutline = dt.text.GetComponent<Outline>();
            txtOutline.effectColor = mOutlineColor[quality];

            mDropTextList.Add(dt);

            return dt;
        }

        /// <summary>
        /// 回收掉落文本
        /// </summary>
        /// <param name="dt"></param>
        private void RecycleDropText(DropText dt)
        {
            dt.rootObj.gameObject.SetActive(false);
            mDropTextList.Remove(dt);
            mUnusedList.Add(dt);
        }

        /// <summary>
        /// 显示拾取
        /// </summary>
        /// <param name="icon"></param>
        private void ShowDropFlyItem(float startTime, string icon, Vector3 startPos, Vector3 endPos, string itemName, int quality)
        {
            DropFlyItem item = null;
            int cnt = mCachedFlyItemList.Count;
            if (cnt > 0)
            {
                item = mCachedFlyItemList[cnt - 1];
                mCachedFlyItemList.RemoveAt(cnt - 1);

                //item.gameObject.SetActive(true);
                item.gameObject.transform.SetRenderActive(true);
            }
            else
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(mDropFlyItemPath, typeof(GameObject));
                if (null == prefab)
                    return;
                GameObject obj = Instantiate(prefab) as GameObject;
                if (null == obj)
                    return;

                obj.transform.SetParent(DropFlyRoot);
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                item = obj.GetComponent<DropFlyItem>();
            }

            if (null != item)
            {
                mDropFlyItemList.Add(item);

                item.Init(startTime, icon, startPos, endPos, itemName, quality);
            }
        }

        /// <summary>
        /// 回收拾取
        /// </summary>
        /// <param name="item"></param>
        private void RecycleDropFlyItem(DropFlyItem item, bool isDone = false)
        {
            //item.gameObject.SetActive(false);
            item.gameObject.transform.SetRenderActive(false);
            mDropFlyItemList.Remove(item);
            mCachedFlyItemList.Add(item);

            if (isDone)
            {
                DoBagTween();

                EventToUI.SetArg(UIEventArg.Arg1, item.mItemName);
                EventToUI.SetArg(UIEventArg.Arg2, item.mItemQuality);
                EventToUI.SendEvent("EU_SHOW_DROPFLYTIP");
            }
        }

        private void ShowDropGoldFlyItem(int count, Vector3 pos)
        {
            DropGoldFlyItem item = null;
            int cnt = mCachedGoldFlyItemList.Count;
            if (cnt > 0)
            {
                item = mCachedGoldFlyItemList[cnt - 1];
                mCachedGoldFlyItemList.RemoveAt(cnt - 1);

                //item.gameObject.SetActive(true);
                item.gameObject.transform.SetRenderActive(true);
            }
            else
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(mDropGoldFlyItemPath, typeof(GameObject));
                if (null == prefab)
                    return;
                GameObject obj = Instantiate(prefab) as GameObject;
                if (null == obj)
                    return;

                obj.transform.SetParent(DropFlyRoot);
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                item = obj.GetComponent<DropGoldFlyItem>();
            }

            if (null != item)
            {
                mDropGoldFlyItemList.Add(item);

                item.Init(Time.realtimeSinceStartup, count, pos);
            }
        }

        private void RecycleDropGoldFlyItem(DropGoldFlyItem item)
        {
            //item.gameObject.SetActive(false);
            item.gameObject.transform.SetRenderActive(false);
            mDropGoldFlyItemList.Remove(item);
            mCachedGoldFlyItemList.Add(item);
        }

        private void ShowDropFlyTipItem(string itemName, int quality)
        {
            DropFlyTipItem item = null;
            int cnt = mCachedFlyTipItemList.Count;
            if (cnt > 0)
            {
                item = mCachedFlyTipItemList[cnt - 1];
                mCachedFlyTipItemList.RemoveAt(cnt - 1);

                //item.gameObject.SetActive(true);
                item.gameObject.transform.SetRenderActive(true);
            }
            else
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(mDropFlyTipItemPath, typeof(GameObject));
                if (null == prefab)
                    return;
                GameObject obj = Instantiate(prefab) as GameObject;
                if (null == obj)
                    return;

                obj.transform.SetParent(DropFlyRoot.Find("CenterTopRoot"));
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                item = obj.GetComponent<DropFlyTipItem>();
            }

            if (null != item )
            {
                mDropFlyTipItemList.Add(item);
                if (quality < mTipsTextColor.Count && quality < mTipsOutlineColor.Count)
                {
                    item.Init(Time.realtimeSinceStartup, itemName, mTipsTextColor[quality], mTipsOutlineColor[quality]);
                }
            }
        }

        private void RecycleDropFlyTipItem(DropFlyTipItem item)
        {
            //item.gameObject.SetActive(false);
            item.gameObject.transform.SetRenderActive(false);
            mDropFlyTipItemList.Remove(item);
            mCachedFlyTipItemList.Add(item);
        }

        private void DoBagTween()
        {
            Transform bagTran = ModuleServer.MS.GDropMgr.PickFlyTarget;
            if (null == bagTran)
            {
                return;
            }

            if (!DOTween.IsTweening(bagTran))
            {
                Sequence tweenSeq = DOTween.Sequence();
                tweenSeq.Append(bagTran.DOScale(1.2f, 0.3f));
                tweenSeq.Append(bagTran.DOScale(1.0f, 0.3f));
            }
        }
    }

[Hotfix]
    class FlyTipInfo
    {
        public string name;
        public int quality;
    }
}

