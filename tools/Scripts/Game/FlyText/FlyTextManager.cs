/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : a
* @date     : 2014-xx-xx
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;


//flyType 文字类型  parma参数
[CSharpCallLua]
public delegate string FlyTextMapping(int flyType,string param);



namespace SG
{
    /// <summary>
    /// 飘字类型。
    /// </summary>
    public enum FlyTextType
    {
        MonsterNormal = 0,//怪物受到伤害
        PlayerNormal,//玩家受到伤害
        Crit,//暴击
        AddHP,//加血
        Buff,//Buff
        Special,//Special
        Huanling,//幻灵伤害
        Mochong,//魔宠伤害
        VipEquip,//王者伤害 文字 
        VipEquipNum,//王者伤害 数字
        Max,
    }

    public enum EFFECTMASK
    {
        MISS = 0x0001,//闪避
        CRIT = 0x0002,//暴击
        BLOCKED = 0x0004,//格挡
        KILL = 0x008,//击杀
        IMMUNITY = 0x0010,//免疫
        KNOCKBACK = 0x0020,//击退
        STIFF = 0x0040,//硬直
        RAMPAGE = 0x0080,//狂暴
        REBIRTH = 0x0100,//重生
        IGNORE = 0x0200,//免疫伤害
        SUPER = 0x0400,//卓越一击
        REALM = 0x0800,//境界压制
        REFLEX = 0x1000,//反弹伤害
        IGDEF = 0x2000,//无视一击
        PULL = 0x4000,//拉近
        HORSE = 0x8000,//打下马
        DIAMONDS = 0X800000,//钻石一击
        KING = 0x1000000,//王者一击
        SUPREME = 0x2000000,//至尊一击
    }

[Hotfix]
    class FlyTextInfo
    {
        public FlyTextType flyType;
        public GameObject owner;
        public string text;
        public int isGood;
    }

    /// <summary>
    /// 飘字管理。
    /// </summary>
[Hotfix]
    public class FlyTextManager : IModule
    {
        //----------- 每个管理器必须写的方法 ----------
        public override bool LoadSrv(IModuleServer IModuleSrv)
        {
            ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
            moduleSrv.GFlyTextMgr = this;

            return true;
        }

        public override void InitializeSrv()
        {
#if true
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FLYTEXT, OnFlyTextEvent);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_SKILLEFFECT, OnSkillEffect);
#endif
            InitFlyText();
        }

        public static IModule Newer(GameObject go)
        {
            IModule module = go.AddComponent<FlyTextManager>();

            return module;
        }
        //-------------------------------------------

        private const string BIND_NAME = "E_top";

        private static Dictionary<FlyTextType, string> m_FlyTextPath = new Dictionary<FlyTextType, string>()
        {
            {FlyTextType.MonsterNormal, "UI/Prefabs/FlyText/FirstRes/FlyText_Monster"},
            {FlyTextType.PlayerNormal, "UI/Prefabs/FlyText/FirstRes/FlyText_Player"},
            {FlyTextType.Crit, "UI/Prefabs/FlyText/FirstRes/FlyText_Crit"},
            {FlyTextType.AddHP, "UI/Prefabs/FlyText/FlyText_HP"},
            {FlyTextType.Buff, "UI/Prefabs/FlyText/FirstRes/FlyText_Miss"},
            {FlyTextType.Special, "UI/Prefabs/FlyText/FlyText_Special"},
            {FlyTextType.Huanling, "UI/Prefabs/FlyText/FlyText_Huanling"},
            {FlyTextType.Mochong, "UI/Prefabs/FlyText/FlyText_Mochong"},
            {FlyTextType.VipEquip, "UI/Prefabs/FlyText/FlyText_VipEquip"},
            {FlyTextType.VipEquipNum, "UI/Prefabs/FlyText/FlyText_VipEquipNum"},

        };

        private Camera uiCamera;

        private List<FlyText>[] m_FlyTextList = new List<FlyText>[(int)FlyTextType.Max];
        private List<FlyText>[] m_CachedList = new List<FlyText>[(int)FlyTextType.Max];

        private Queue<FlyTextInfo> mPlayerInfo = new Queue<FlyTextInfo>();
        private Queue<FlyTextInfo> mProcessInfo = new Queue<FlyTextInfo>();
        private float mIntvlTime = 0.08f;
        private float mLastTime = 0.0f;
        private float mPlayerIntvlTime = 0.1f;
        private float mPlayerLastTime = 0.0f;

        /// <summary>
        /// 飘字Root
        /// </summary>
        private Transform m_FlyTextRoot;
        public Transform FlyTextRoot
        {
            get
            {
                if (m_FlyTextRoot == null)
                {
                    string path = "UI/Prefabs/FlyText/FirstRes/FlyTextRoot";
                    GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
                    if(prefab!=null)
                    {
                        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
                        obj.name = "FlyTextRoot";
                        obj.SetActive(true);
                        Canvas canvas = obj.GetComponent<Canvas>();
                        canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
                        canvas.sortingOrder = 502;
                        GameObject.DontDestroyOnLoad(obj);
                        m_FlyTextRoot = obj.transform;
                    }

                }

                return m_FlyTextRoot;
            }
        }

        private void InitFlyText()
        {
            for (int i = 0; i < (int)FlyTextType.Max; i++)
            {
                m_FlyTextList[i] = new List<FlyText>();
                m_CachedList[i] = new List<FlyText>();
            }

            mProcessInfo.Clear();
            mPlayerInfo.Clear();
        }

        private void OnSkillEffect(GameEvent ge, EventParameter parameter)
        {
            MsgData_sCastEffect data = parameter.msgParameter as MsgData_sCastEffect;

            ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(data.TargetID);
            if (null == actor)
            {
                return;
            }

            bool isPlayer = actor.mActorType == ActorType.AT_LOCAL_PLAYER;
            string text = string.Empty;
            int flag = data.Flags;
            int damage = (int)data.Damage;
            damage = CheckHP(actor, damage);
            FlyTextType type;

            //幻灵或者魔宠技能伤害
            LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(data.SkillID);
            if (null != skillCfg)
            {
                if ((int)SkillShowType.ST_MAGICKEY == skillCfg.Get<int>("showtype"))
                {
                    type = FlyTextType.Mochong;
                    if (damage == 0)
                    {
                        return;
                    }
                    else if (damage < 0)
                    {
                        text = string.Format("魔宠 +{0}", -damage);
                    }
                    else
                    {
                        text = string.Format("魔宠 -{0}", damage);
                    }

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    return;
                }
                else if ((int)SkillShowType.ST_HUANLING == skillCfg.Get<int>("showtype"))
                {
                    type = FlyTextType.Huanling;
                    if (damage == 0)
                    {
                        return;
                    }
                    else if (damage < 0)
                    {
                        text = string.Format("幻灵 +{0}", -damage);
                    }
                    else
                    {
                        text = string.Format("幻灵 -{0}", damage);
                    }

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    return;
                }
            }

            if (0 == flag)
            {
                if (damage == 0)
                {
                    return;
                }
                if (damage < 0)
                {
                    text = string.Format("+{0}", -damage);
                    type = FlyTextType.AddHP;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);

                    return;
                }

                text = string.Format("{0}", damage);
                type = FlyTextType.Max;

                if (actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    type = FlyTextType.PlayerNormal;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);
                }
                else
                {
                    type = FlyTextType.MonsterNormal;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);
                }

                return;
            }

            bool showDamage = false;
            if (CheckMask(flag, EFFECTMASK.SUPER))
            {
                if (!isPlayer && damage != 0)//玩家不显示
                {
                    showDamage = true;

                    text = string.Format("卓越一击 {0}", damage);
                    type = FlyTextType.Special;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);
                }
            }
            if (CheckMask(flag, EFFECTMASK.IGDEF))
            {
                if (!isPlayer && damage != 0)//玩家不显示
                {
                    showDamage = true;

                    text = string.Format("无视一击 {0}", damage);
                    type = FlyTextType.Special;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);
                }
            }
            if (CheckMask(flag, EFFECTMASK.CRIT))
            {
                if (damage != 0)
                {
                    showDamage = true;

                    text = string.Format("暴击 {0}", damage);
                    type = FlyTextType.Crit;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);
                }
            }
            if (CheckMask(flag, EFFECTMASK.IGNORE))//免疫伤害
            {
                text = string.Format("无");
                type = FlyTextType.Buff;

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                //ShowText(actor.gameObject, text, type);
            }
            if (CheckMask(flag, EFFECTMASK.MISS))
            {
                text = string.Format("闪避");
                type = FlyTextType.Buff;

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                //ShowText(actor.gameObject, text, type);
            }
            if (CheckMask(flag, EFFECTMASK.BLOCKED))
            {
                text = string.Format("格挡");
                type = FlyTextType.Buff;

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                //ShowText(actor.gameObject, text, type);
            }
            if (CheckMask(flag, EFFECTMASK.IMMUNITY))
            {
                text = string.Format("免疫");
                type = FlyTextType.Buff;

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                //ShowText(actor.gameObject, text, type);
            }
            if (CheckMask(flag, EFFECTMASK.REFLEX))
            {
                text = string.Format("反弹伤害");
                type = FlyTextType.Special;

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                //ShowText(actor.gameObject, text, type);
            }
            if (CheckMask(flag, EFFECTMASK.RAMPAGE))
            {
                text = string.Format("狂暴");
                type = FlyTextType.Buff;

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                //ShowText(actor.gameObject, text, type);
            }
            if (CheckMask(flag, EFFECTMASK.REALM))
            {
                if (!isPlayer)//玩家不显示
                {
                    text = string.Format("境界压制");
                    type = FlyTextType.Special;

                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    //ShowText(actor.gameObject, text, type);
                }
            }
            if (CheckMask(flag, EFFECTMASK.KILL))
            {
                if (!showDamage)
                {
                    showDamage = true;

                    if (damage < 0)
                    {
                        text = string.Format("+{0}", -damage);
                        type = FlyTextType.AddHP;

                        AddProcessInfo(isPlayer, actor.gameObject, text, type);
                        //ShowText(actor.gameObject, text, type);
                    }
                    else if (damage > 0)
                    {
                        text = string.Format("{0}", damage);

                        if (actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                        {
                            type = FlyTextType.PlayerNormal;

                            AddProcessInfo(isPlayer, actor.gameObject, text, type);
                            //ShowText(actor.gameObject, text, type);
                        }
                        else
                        {
                            type = FlyTextType.MonsterNormal;

                            AddProcessInfo(isPlayer, actor.gameObject, text, type);
                            //ShowText(actor.gameObject, text, type);
                        }
                    }
                }
            }
            if (CheckMask(flag, EFFECTMASK.PULL))
            {

            }
            if (CheckMask(flag, EFFECTMASK.KNOCKBACK))
            {
                if (!showDamage)
                {
                    showDamage = true;
                    if (damage < 0)
                    {
                        text = string.Format("+{0}", -damage);
                        type = FlyTextType.AddHP;

                        AddProcessInfo(isPlayer, actor.gameObject, text, type);
                    }
                    else if (damage > 0)
                    {
                        text = string.Format("{0}", damage);
                        if (actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                        {
                            type = FlyTextType.PlayerNormal;

                            AddProcessInfo(isPlayer, actor.gameObject, text, type);
                            //ShowText(actor.gameObject, text, type);
                        }
                        else
                        {
                            type = FlyTextType.MonsterNormal;

                            AddProcessInfo(isPlayer, actor.gameObject, text, type);
                            //ShowText(actor.gameObject, text, type);
                        }
                    }
                }
            }
            if (CheckMask(flag, EFFECTMASK.STIFF))
            {

            }
            if (CheckMask(flag, EFFECTMASK.REBIRTH))
            {

            }
            if (CheckMask(flag, EFFECTMASK.HORSE))
            {

            }
            if (CheckMask(flag, EFFECTMASK.DIAMONDS))
            {
                text = string.Format("钻石一击");
                type = FlyTextType.VipEquip;
                FlyTextMapping fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<FlyTextMapping>("Common.FlyTextMapping");
                if (fun != null)
                {
                    text = fun((int)FlyTextType.VipEquip, "1");
                }

                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                if (damage > 0)
                {
                    text = string.Format("{0}", damage);
                    type = FlyTextType.VipEquipNum;
                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                }
            }

            if (CheckMask(flag, EFFECTMASK.KING))
            {
                text = string.Format("王者二鸡");
                type = FlyTextType.VipEquip;

                FlyTextMapping fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<FlyTextMapping>("Common.FlyTextMapping");
                if (fun != null)
                {
                    text = fun((int)FlyTextType.VipEquip,"2");
                }
                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                if(damage > 0)
                {
                    type = FlyTextType.VipEquipNum;
                    text = string.Format("{0}", damage);
                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                }

            }
            if (CheckMask(flag, EFFECTMASK.SUPREME))
            {
                text = string.Format("至尊三机");
                type = FlyTextType.VipEquip;
                FlyTextMapping fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<FlyTextMapping>("Common.FlyTextMapping");
                if (fun != null)
                {
                    text = fun((int)FlyTextType.VipEquip, "3");
                }
 
                AddProcessInfo(isPlayer, actor.gameObject, text, type);
                if (damage > 0)
                {
                    type = FlyTextType.VipEquipNum;
                    text = string.Format("{0}", damage);
                    AddProcessInfo(isPlayer, actor.gameObject, text, type);
                }

            }
        }

        private bool CheckMask(int flag, EFFECTMASK type)
        {
            return (flag & (int)type) != 0;
        }

        private int CheckHP(ActorObj actor, int damage)
        {
            if (null != actor.mBaseAttr)
            {
                if (damage > 0 && damage > actor.mBaseAttr.CurHP)
                {
                    if (actor.mBaseAttr.CurHP > 0)
                    {
                        return (int)actor.mBaseAttr.CurHP;
                    }
                    else
                    {
                        return damage;
                    }
                }
                else if (damage < 0 && damage < actor.mBaseAttr.CurHP - actor.mBaseAttr.MaxHP)
                {
                    return (int)(actor.mBaseAttr.CurHP - actor.mBaseAttr.MaxHP);
                }
            }

            return damage;
        }

        private void OnFlyTextEvent(GameEvent ge, EventParameter parameter)
        {
            GameObject owner = parameter.goParameter;
            FlyTextType ftype = (FlyTextType)parameter.intParameter;
            int value = parameter.intParameter1;
            int isGood = parameter.intParameter2;
            string txt = string.Empty;
            if (ftype == FlyTextType.Crit)
            {
                if (0 == value)
                {
                    return;
                }

                txt = string.Format("暴击 {0}", value);
            }
            else if (ftype == FlyTextType.Buff)
            {
                txt = (string)parameter.objParameter;
            }
            else if (ftype == FlyTextType.MonsterNormal)
            {
                if (0 == value)
                {
                    return;
                }

                txt = string.Format("{0}", value);
            }
            else if (ftype == FlyTextType.PlayerNormal)
            {
                if (0 == value)
                {
                    return;
                }

                txt = string.Format("{0}", value);
            }
            else if (ftype == FlyTextType.AddHP)
            {
                if (0 == value)
                {
                    return;
                }

                txt = string.Format("+{0}", -value);
            }
            else if (ftype == FlyTextType.Special)
            {
                txt = (string)parameter.objParameter;
            }

            ShowText(owner, txt, ftype, isGood);
        }

        private void ShowText(GameObject owner, string text, FlyTextType type, int isGood = 0)
        {
            Transform parent = FlyTextRoot;
            if (null == parent)
            {
                return;
            }

            string path = m_FlyTextPath[type];
            List<FlyText> activeList = m_FlyTextList[(int)type];
            List<FlyText> cacheList = m_CachedList[(int)type];

            int cachedCnt = cacheList.Count;
            FlyText ft = null;
            if (cachedCnt > 0)
            {
                ft = cacheList[cachedCnt - 1];
                cacheList.RemoveAt(cachedCnt - 1);
                if(ft!=null)
                {
                    ft.AniStart = Time.realtimeSinceStartup;

                    ft.transform.localPosition = Vector3.zero;
                    //ft.gameObject.SetActive(true);
                    ft.gameObject.transform.SetRenderActive(true);
                    activeList.Add(ft);
                }

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
                ft = obj.GetComponent<FlyText>();
                if (ft != null)
                {
                    ft.AniStart = Time.realtimeSinceStartup;

                    activeList.Add(ft);
                }
            }

            if (null != ft)
            {
                ft.Init(owner, text, type, isGood);
            }
        }

        void Update()
        {
            DoProcessInfo();
            UpdateFlyText();
        }

        private void UpdateFlyText()
        {
            float realTime = Time.realtimeSinceStartup;
            for (int i = 0; i < (int)FlyTextType.Max; ++i)
            {
                List<FlyText> showList = m_FlyTextList[i];
                List<FlyText> cacheList = m_CachedList[i];
                for (int j=0; j< showList.Count;)
                {
                    FlyText ft = showList[j];
                    if (UpdateFlyText(ft, realTime))
                    {
                        showList.RemoveAt(j);
                        cacheList.Add(ft);
                        //ft.gameObject.SetActive(false);
                        ft.gameObject.transform.SetRenderActive(false);
                    }
                    else
                    {
                        ++j;
                    }
                }
            }
        }

        /// <summary>
        /// 更新票字。
        /// </summary>
        /// <param name="ft">飘字对象。</param>
        /// <param name="realTime">当前时间。</param>
        /// <returns>是否结束。</returns>
        private bool UpdateFlyText(FlyText ft, float realTime)
        {
            float curTime = realTime - ft.AniStart;
            float x = 0, y = 0;
            ft.GetAnimationPos(curTime, ref x, ref y);
            ft.UpdateBindPosition();
            if (ft.Inverse)
            {
                ft.ShowText.transform.localPosition = new Vector3(ft.StartPosition.x - x, ft.StartPosition.y + y, 0f);
            }
            else
            {
                ft.ShowText.transform.localPosition = new Vector3(ft.StartPosition.x + x, ft.StartPosition.y + y, 0f);
            }
            Color c = ft.ShowText.color;
            c.a = ft.GetAnimationAlpha(curTime);
            ft.ShowText.color = c;
            x = ft.GetAnimationScale(curTime);
            ft.ShowText.transform.localScale = new Vector3(x, x, x);

            float maxt = ft.AniCurve.GetAnimationTime();
            return curTime > maxt;
        }

        private void AddProcessInfo(bool isPlayer, GameObject owner, string text, FlyTextType type, int isGood = 0)
        {
            FlyTextInfo info = new FlyTextInfo();
            info.owner = owner;
            info.text = text;
            info.flyType = type;
            info.isGood = isGood;

            if (isPlayer)
            {
                //ShowText(info.owner, info.text, info.flyType, info.isGood);
                mPlayerInfo.Enqueue(info);
            }
            else
            {
                ShowText(info.owner, info.text, info.flyType, info.isGood);
                //mProcessInfo.Enqueue(info);
            }
        }

        private void DoProcessInfo()
        {
            if (Time.realtimeSinceStartup - mPlayerLastTime > mPlayerIntvlTime)
            {
                if (mPlayerInfo.Count > 0)
                {
                    FlyTextInfo info = mPlayerInfo.Dequeue();

                    ShowText(info.owner, info.text, info.flyType, info.isGood);

                    mPlayerLastTime = Time.realtimeSinceStartup;
                }
            }

            if (Time.realtimeSinceStartup - mLastTime > mIntvlTime)
            {
                if (mProcessInfo.Count > 0)
                {
                    FlyTextInfo info = mProcessInfo.Dequeue();

                    ShowText(info.owner, info.text, info.flyType, info.isGood);

                    mLastTime = Time.realtimeSinceStartup;
                }
            }
        }
    }
}

