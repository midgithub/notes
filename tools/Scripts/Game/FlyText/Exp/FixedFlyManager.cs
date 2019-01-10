/**
* @file     : FixedFlyManager
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-07-17
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using XLua;

namespace SG
{
    public enum FixedFlyType
    {
        Exp = 0,//经验
        Mochong,//魔宠技能
        Huanling,//技能
        Max,
    }

[Hotfix]
    class FixedFlyInfo
    {
        public string text;
        public FixedFlyType type;
    }

[Hotfix]
    public class FixedFlyManager : IModule
    {
        private static Dictionary<FixedFlyType, string> m_FlyTextPath = new Dictionary<FixedFlyType, string>()
        {
            {FixedFlyType.Exp, "UI/Prefabs/FixedFlyText/FirstRes/Item_Exp"},
            {FixedFlyType.Mochong, "UI/Prefabs/FixedFlyText/Item_Mochong"},
            {FixedFlyType.Huanling, "UI/Prefabs/FixedFlyText/Item_Huanling"},
        };

        private Dictionary<FixedFlyType, List<FixedFlyItem>> m_FixedTextDict = new Dictionary<FixedFlyType, List<FixedFlyItem>>();
        private Dictionary<FixedFlyType, List<FixedFlyItem>> m_CachedDict = new Dictionary<FixedFlyType, List<FixedFlyItem>>();

        private Queue<string> mProcessText = new Queue<string>();
        private float mIntervalTime = 0.3f;
        private float mLastTime = 0.0f;

        private Queue<FixedFlyInfo> mSkillProcessText = new Queue<FixedFlyInfo>();
        private float mSkillIntervalTime = 0.3f;
        private float mSkillLastTime = 0.0f;

        private Transform mExpRoot;
        public Transform ExpRoot
        {
            get
            {
                if (mExpRoot == null)
                {
                    Transform rootTransform = ModuleServer.MS.GFlyTextMgr.FlyTextRoot;
                    mExpRoot = rootTransform.Find("ExpParent");
                }

                return mExpRoot;
            }
        }

        private Transform mSkillRoot;
        public Transform SkillRoot
        {
            get
            {
                if (mSkillRoot == null)
                {
                    Transform rootTransform = ModuleServer.MS.GFlyTextMgr.FlyTextRoot;
                    mSkillRoot = rootTransform.Find("SkillParent");
                }

                return mSkillRoot;
            }
        }

        //----------- 每个管理器必须写的方法 ----------
        public override bool LoadSrv(IModuleServer IModuleSrv)
        {
            ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
            moduleSrv.GFixedFlyMgr = this;

            return true;
        }

        public override void InitializeSrv()
        {
            //CoreEntry.gEventMgr.AddListener(GameEvent.GE_PLAYER_EXP, OnPlayerExp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_PALYER_JINGJIE_EXP, OnPlayerJingJieExp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FLY_SKILL, OnSkillFlyText);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AddExpInfo, OnAddExpInfo);
 

            m_FixedTextDict.Clear();
            m_CachedDict.Clear();
            for (int i = 0; i < (int)FixedFlyType.Max; i++)
            {
                FixedFlyType type = (FixedFlyType)i;
                m_FixedTextDict[type] = new List<FixedFlyItem>();
                m_CachedDict[type] = new List<FixedFlyItem>();
            }
        }


        public static IModule Newer(GameObject go)
        {
            IModule module = go.AddComponent<FixedFlyManager>();

            return module;
        }
        //-------------------------------------------

        void Update()
        {
            DoProcessText();

            float realTime = Time.realtimeSinceStartup;

            for (int i = 0; i < (int)FixedFlyType.Max; i++)
            {
                FixedFlyType type = (FixedFlyType)i;
                if (m_FixedTextDict.ContainsKey(type))
                {
                    List<FixedFlyItem> showList = m_FixedTextDict[type];
                    for (int j = 0; j < showList.Count; j++)
                    {
                        UpdateFixedText(showList[j], realTime);
                    }
                }
            }
        }

        private void DoProcessText()
        {
            if (Time.realtimeSinceStartup - mLastTime > mIntervalTime)
            {
                if (mProcessText.Count > 0)
                {
                    string text = mProcessText.Dequeue();

                    ShowFixedFly(FixedFlyType.Exp, text);

                    mLastTime = Time.realtimeSinceStartup;
                }
            }

            if (Time.realtimeSinceStartup - mSkillLastTime > mSkillIntervalTime)
            {
                if (mSkillProcessText.Count > 0)
                {
                    FixedFlyInfo info = mSkillProcessText.Dequeue();

                    ShowFixedFly(info.type, info.text);

                    mSkillLastTime = Time.realtimeSinceStartup;
                }
            }
        }

        private void UpdateFixedText(FixedFlyItem item , float realTime)
        {
            if (null == item)
            {
                return;
            }

            float curTime = realTime - item.AniStart;
            float x = 0, y = 0;
            item.GetAnimationPos(curTime, ref x, ref y);
            item.ShowText.transform.localPosition = new Vector3(x, y, 0f);
            Color c = item.ShowText.color;
            c.a = item.GetAnimationAlpha(curTime);
            item.ShowText.color = c;
            x = item.GetAnimationScale(curTime);
            item.ShowText.transform.localScale = new Vector3(x, x, x);

            x = item.GetAnimationTime();
            if (curTime > x)
            {
                Recycle(item);
            }
        }

        private void OnPlayerExp(GameEvent ge, EventParameter parameter)
        {
            int preLv = parameter.intParameter;
            long preExp = parameter.longParameter;
            int curLv = PlayerData.Instance.BaseAttr.Level;
            long curExp = PlayerData.Instance.BaseAttr.Exp;

            long expGain = curExp - preExp;
            for (int i = preLv; i < curLv; i++)
            {
                LuaTable lvCfg = ConfigManager.Instance.Actor.GetLevelUpConfig(i);
                if (lvCfg == null)
                {
                    LogMgr.ErrorLog("invalid level ,no config found ,level = {0}", i);
                    continue;
                }
                expGain += lvCfg.Get<int>("exp");
            }

            if (expGain > 0)
            {
                mProcessText.Enqueue(string.Format("经验 +{0}", NumberConvert(expGain)));
            }
        }
        private void OnAddExpInfo(GameEvent ge, EventParameter parameter)
        {
            MsgData_sAddExpInfo msg = parameter.msgParameter as MsgData_sAddExpInfo;
            if (msg == null) return;
            if(msg.exp > 0 )
            {
                if (msg.src == 277 && msg.rate > 0)
                {
                    mProcessText.Enqueue(string.Format("经验 +{0} （魔神加成{1}%）", NumberConvert((long)msg.exp),msg.rate*100));
                }
                else
                {
                    mProcessText.Enqueue(string.Format("经验 +{0}", NumberConvert((long)msg.exp)));
                }
            }

        }
            
        private void OnPlayerJingJieExp(GameEvent ge, EventParameter parameter)
        {
            int preLv = parameter.intParameter;
            long preExp = parameter.longParameter;
            int curLv = PlayerData.Instance.BaseAttr.JingJieLevel;
            long curExp = PlayerData.Instance.BaseAttr.JingJieExp;

            long expGain = curExp - preExp;
            for (int i = preLv; i < curLv; i++)
            {
                LuaTable cfg = ConfigManager.Instance.Actor.GetDianFengConfig(i);
                if (null == cfg)
                    continue;

                expGain += cfg.Get<int>("exp");
            }

            if (expGain > 0)
            {
                mProcessText.Enqueue(string.Format("境界经验 +{0}", NumberConvert(expGain)));
            }
        }

        private string NumberConvert(long num)
        {
            if (num < 1000000)
            {
                return num.ToString();
            }
            else if (num < 10 * 10000 * 10000)
            {
                return string.Format("{0}万", (long)(num * 0.0001));
            }
            else
            {
                return string.Format("{0}亿", (long)(num * 0.00000001));
            }
        }

        private void OnSkillFlyText(GameEvent ge, EventParameter parameter)
        {
            if (null == parameter)
            {
                return;
            }

            if (parameter.intParameter == (int)SkillShowType.ST_HUANLING)
            {
                FixedFlyInfo info = new FixedFlyInfo();
                info.type = FixedFlyType.Huanling;
                info.text = string.Format("幻灵·{0}", parameter.stringParameter);

                mSkillProcessText.Enqueue(info);
            }
            else if (parameter.intParameter == (int)SkillShowType.ST_MAGICKEY)
            {
                FixedFlyInfo info = new FixedFlyInfo();
                info.type = FixedFlyType.Mochong;
                info.text = string.Format("魔宠·{0}", parameter.stringParameter);

                mSkillProcessText.Enqueue(info);
            }
        }

        private void ShowFixedFly(FixedFlyType type, string text)
        {
            Transform parent = null;
            switch (type)
            {
                case FixedFlyType.Exp:
                    parent = ExpRoot;
                    break;
                case FixedFlyType.Huanling:
                case FixedFlyType.Mochong:
                    parent = SkillRoot;
                    break;
                default:
                    break;
            }
            if (null == parent)
            {
                return;
            }

            List<FixedFlyItem> activeList = m_FixedTextDict[type];
            List<FixedFlyItem> cacheList = m_CachedDict[type];
            string path = m_FlyTextPath[type];

            FixedFlyItem item = null;
            int cnt = cacheList.Count;
            if (cnt > 0)
            {
                item = cacheList[cnt - 1];
                cacheList.RemoveAt(cnt - 1);

                item.transform.localPosition = Vector3.zero;
                //item.gameObject.SetActive(true);
                item.gameObject.transform.SetRenderActive(true);
            }
            else
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
                if (null == prefab)
                    return;
                GameObject obj = Instantiate(prefab) as GameObject;
                if (null == obj)
                    return;

                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                item = obj.GetComponent<FixedFlyItem>();
            }

            if (null != item)
            {
                activeList.Add(item);
                item.Init(Time.realtimeSinceStartup, text, type);
            }
        }

        private void Recycle(FixedFlyItem item)
        {
            List<FixedFlyItem> activeList = m_FixedTextDict[item.mType];
            List<FixedFlyItem> cacheList = m_CachedDict[item.mType];

            item.Recycle();
            activeList.Remove(item);
            cacheList.Add(item);

            //item.gameObject.SetActive(false);
            item.gameObject.transform.SetRenderActive(false);
        }
    }
}

