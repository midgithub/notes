using UnityEngine;
using System;
using System.Collections.Generic;
using XLua;

namespace SG
{

    public enum SkillRangeType
    {
        SKILL_TARGET = 1,    //目标单体
        SKILL_TARGET_CIRCLE = 2,    //目标圆形范围
        SKILL_SELF_FUN = 3,     //自身原点锥形
        SKILL_SELF_RECT = 4,    //自身边中心矩形
        SKILL_SELF_CIRCLE = 5   //自身原点圆
    }


    public enum SkillType
    {
        SKILL_CHARGE = 1,    //冲锋
        SKILL_NORMAL = 2,   //普通攻击
        SKILL_BIGHIT = 3,    //重击
        SKILL_SMALLAOE = 4,  //小AOE
        SKILL_XULI = 5,      //蓄力
        SKILL_XUANFANZHAN = 6, //旋风斩
        SKILL_BIGAOE = 7,  //大AOE

    }



    public enum SkillFactionType
    {
        FACTION_SELF = 1,    //自己
        FACTION_FRIEND = 2,   //友方
        FACTION_ENEMY = 4,    //敌方
        FACTION_NEUTRAL = 8,  //中立

    }

    public enum BehitEfxPos
    {
        GROUND,             //地面

        FOOT,           //脚下

        BIP,            //腰

    }

    public enum SkillEffectType
    {
        COMON = 1,//普通通用技能效果
        CUSTOM = 2,//skill_customeffect定义效果（陷阱，光环和召唤）
        MOVE = 3//位移效果
    }

    //伤害参数    
[Hotfix]
    public class DamageParam
    {
        public int skillID;         //技能ID：如果是技能伤害

        public int Flags;            // 目标标记
        public int DamagaType;       // 伤害类型
        public int Damage;          // 目标伤害值
        public bool IsClient = true;          //是否是客户端本地计算


        public ActorObj attackActor;//攻击者
        public ActorObj behitActor;//被击对象

        //被击效果(采用替换的方式)
        public int weight;            //修正被击权重(被击的动作，位移)

        public bool isNotUseCurveMove = false;  //不使用美术位移    

        public float moveTime;        //修正小退时间
        public float moveScale;       //修正小退位移系数      

        public float backScale;       //修正击退位移系数          
        public float flyScale;        //修正击飞位移系数      
        public float skyTime;         //修正悬空时间
        public bool isUseDamageTransfer = false; // true表示这是一个伤害转移
        public void clean()
        {
            attackActor = null;
            behitActor = null;
        }
    }

[Hotfix]
    public class SkillMgr : MonoBehaviour
    {
        private EventMgr m_EventMgr;
        private BaseTool m_baseTool;
#if !PUBLISH_RELEASE
        // 无敌设置(GM 模块)
        public bool m_bMiaoGuai = false;

        //主角无伤害
        public bool m_bWuShang = false;
#endif
        //显示技能范围(GM 模块)
        public bool m_bShowSkillScope = false;


        //选中标记
        // #if UNITY_EDITOR    
        private int m_selectType = 0;        //选择类型 1攻击(红色) 2不可攻击(绿色)
        private GameObject m_beSelectTagObj = null;

        /// <summary>
        /// 获取选择标记。
        /// </summary>
        /// <param name="att">目标是否可攻击。</param>
        /// <returns>选择标记对象。</returns>
        public GameObject GetSelectTag(bool att)
        {
            int marktype = att ? 1 : 2;
            if (m_beSelectTagObj == null || m_selectType != marktype)
            {
                if (m_beSelectTagObj != null)
                {
                    Destroy(m_beSelectTagObj);
                    m_beSelectTagObj = null;
                }

                //加载新的选中标记
                string path = marktype == 1 ? "Effect/common/fx_select" : "Effect/common/fx_select_lv";
                m_selectType = marktype;
                m_beSelectTagObj = GameObject.Instantiate(CoreEntry.gResLoader.Load(path)) as GameObject;
            }

            return m_beSelectTagObj;
        }

        /// <summary>
        /// 隐藏选择标记。
        /// </summary>
        public void HideSelectTag()
        {
            if (m_beSelectTagObj != null)
            {
                m_beSelectTagObj.transform.SetParent(null);
                m_beSelectTagObj.SetActive(false);
            }
        }
        
        public GameObject m_beSelectTagFriend = null;
        
        // Use this for initialization
        void Start()
        {
            m_EventMgr = CoreEntry.gEventMgr;
            m_baseTool = CoreEntry.gBaseTool;

            //加载脚底选中特效
            m_beSelectTagFriend = GameObject.Instantiate(CoreEntry.gResLoader.Load("Effect/common/fx_select_lv")) as GameObject;
            if(m_beSelectTagFriend == null)return;
            m_beSelectTagFriend.transform.parent = null;
            m_beSelectTagFriend.transform.localPosition = Vector3.zero;
            m_beSelectTagFriend.SetActive(false);
            
            RegisterEvent();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //注册技能释放事件
        void RegisterEvent()
        {
            m_EventMgr.AddListener(GameEvent.GE_NOTIFY_CAST_SKILL, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_NOTIFY_SHOW_SKILL_SCOPE, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE, EventFunction);

            m_EventMgr.AddListener(GameEvent.GE_SC_SKILLEFFECT, EventFunction);

            m_EventMgr.AddListener(GameEvent.GE_SC_KNOCKBACK, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_SC_ADDBUFF, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_SC_UPDATEBUFF, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_SC_DELBUFF, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_SC_ADDBUFFList, EventFunction);

            m_EventMgr.AddListener(GameEvent.GE_SC_OTHER_CASTSKILL_BEGIN, EventFunction);
            m_EventMgr.AddListener(GameEvent.GE_SC_OTHER_CASTSKILL_END, EventFunction);

            m_EventMgr.AddListener(GameEvent.GE_SC_CAST_MOVE_EFFECRT, EventFunction);
            //m_EventMgr.AddListener(GameEvent.GE_NOTIFY_SKILL_DAMAGE, EventFunction);        
        }

        void OnDestroy()
        {
            RemoveEvent();
        }

        void RemoveEvent()
        {
            m_EventMgr.RemoveListener(GameEvent.GE_NOTIFY_CAST_SKILL, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_NOTIFY_SHOW_SKILL_SCOPE, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE, EventFunction);


            m_EventMgr.RemoveListener(GameEvent.GE_SC_SKILLEFFECT, EventFunction);

            m_EventMgr.RemoveListener(GameEvent.GE_SC_KNOCKBACK, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_SC_ADDBUFF, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_SC_ADDBUFFList, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_SC_UPDATEBUFF, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_SC_DELBUFF, EventFunction);


            m_EventMgr.RemoveListener(GameEvent.GE_SC_OTHER_CASTSKILL_BEGIN, EventFunction);
            m_EventMgr.RemoveListener(GameEvent.GE_SC_OTHER_CASTSKILL_END, EventFunction);

            m_EventMgr.RemoveListener(GameEvent.GE_SC_CAST_MOVE_EFFECRT, EventFunction);
            //m_EventMgr.AddListener(GameEvent.GE_NOTIFY_SKILL_DAMAGE, EventFunction);        
        }
        public void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_NOTIFY_CAST_SKILL:
                    {
                        //释放技能
                        int skillID = (int)parameter.intParameter;
                        GameObject actor = parameter.goParameter;

                        //调用actorbase释放技能
                        ActorObj actorObj = actor.GetComponent<ActorObj>();

                        //是否学习了该技能                
                        if (actorObj.IsHadLearnSkill(skillID))
                        {
                            //玩家释放技能前选择目标
                            if (actorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
                            {
                                //若没有目标则马上选择目标
                                CoreEntry.gActorMgr.MainPlayer.CheckSkillTarget();
                            }

                            actorObj.OnRunToAttack(skillID);
                            //actorObj.OnCastSkill(skillID); 
                        }
                    }
                    break;

                case GameEvent.GE_NOTIFY_SHOW_SKILL_SCOPE:
                    {
                        //释放技能
                        int skillID = (int)parameter.intParameter;
                        GameObject actor = parameter.goParameter;

                        //调用actorbase释放技能
                        ActorObj actorObj = actor.GetComponent<ActorObj>();

                        //是否学习了该技能                
                        if (actorObj.IsHadLearnSkill(skillID))
                        {
                            actorObj.OnShowSkillScope(skillID);
                            //actorObj.OnCastSkill(skillID); 
                        }
                    }
                    break;
                case GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE:
                    {
                        //释放技能
                        int skillID = (int)parameter.intParameter;
                        GameObject actor = parameter.goParameter;

                        //调用actorbase释放技能
                        ActorObj actorObj = actor.GetComponent<ActorObj>();

                        //是否学习了该技能                
                        if (actorObj.IsHadLearnSkill(skillID))
                        {
                            actorObj.OnHideSkillScope(skillID);
                            //actorObj.OnCastSkill(skillID); 
                        }
                    }
                    break;

                //
                case GameEvent.GE_SC_SKILLEFFECT:
                    {
                        MsgData_sCastEffect data = parameter.msgParameter as MsgData_sCastEffect;

                        if (data == null)
                            return;


                        //服务器这里只冒伤害数字
                        bool bIsMainPlayer = false;

                        BehitParam behitParam = new BehitParam();

                        DamageParam damageParam = new DamageParam();
                        damageParam.attackActor = CoreEntry.gActorMgr.GetActorByServerID(data.CasterID);
                        damageParam.behitActor = CoreEntry.gActorMgr.GetActorByServerID(data.TargetID);
                        damageParam.skillID = data.SkillID;
                        damageParam.Flags = data.Flags;
                        damageParam.DamagaType = data.DamagaType;
                        damageParam.Damage = (int)data.Damage;
                        damageParam.IsClient = false;


                        behitParam.damgageInfo = damageParam;
                        behitParam.displayType = (DamageDisplayType)data.DamagaType;
                        behitParam.hp = (int)data.Damage;


                        if (behitParam.damgageInfo.attackActor != null
                            && behitParam.damgageInfo.attackActor.mActorType == ActorType.AT_LOCAL_PLAYER)
                        {
                            bIsMainPlayer = true;
                        }

                        if (behitParam.damgageInfo.attackActor != null)
                        {
                            if (bIsMainPlayer)
                            {
                                behitParam.damgageInfo.attackActor.OnDamage((int)data.Damage, data.SkillID, bIsMainPlayer, behitParam);
                            }
                            else
                            {
                                //其他人要播受击动作
                                OnSkillDamage(damageParam);
                            }

                        }

                    }
                    break;


                case GameEvent.GE_SC_OTHER_CASTSKILL_BEGIN:
                    {
                        MsgData_sCastBegan data = parameter.msgParameter as MsgData_sCastBegan;
                        if (data == null )
                            return;
                        ActorObj attackObj = CoreEntry.gActorMgr.GetActorByServerID(data.CasterID);

                        if (attackObj == null)
                            return;

                        if (attackObj != null && attackObj.mActorType == ActorType.AT_LOCAL_PLAYER)
                        {
                            return;
                        }



                        //todo ，先强行同步位置
                        attackObj.SetServerPosition(new Vector2(data.CasterPosX, data.CasterPosY));
                        attackObj.OnCastSkill(data.SkillID, data.CasterID, data.TargetID, data.PosX, data.PosY);

                    }
                    break;


                case GameEvent.GE_SC_OTHER_CASTSKILL_END:
                    {
                        MsgData_sCastEnd data = parameter.msgParameter as MsgData_sCastEnd;
                        if (data == null )
                            return;
                        ActorObj attackObj = CoreEntry.gActorMgr.GetActorByServerID(data.CasterID);

                        if (attackObj == null)
                            return;

                        attackObj.SkillEnd(data.SkillID);


                    }
                    break;


                //hitback 
                case GameEvent.GE_SC_KNOCKBACK:
                    {
                        MsgData_sKnockBack data = parameter.msgParameter as MsgData_sKnockBack;
                        if (data == null )
                            return;
                        ActorObj beHitActor = CoreEntry.gActorMgr.GetActorByServerID(data.TargetID);
                        ActorObj attackActor = CoreEntry.gActorMgr.GetActorByServerID(data.CasterID);

                        if ( beHitActor == null || attackActor == null)
                            return;

                        beHitActor.OnHitBack(attackActor, data.MotionSpeed, data.PosX, data.PosY, data.MotionTime);
                    }
                    break;

                case GameEvent.GE_SC_ADDBUFFList:
                    {
                        MsgData_sAddBufferList data = parameter.msgParameter as MsgData_sAddBufferList;
                        if (data == null)
                            return;
                        ActorObj targetObj = CoreEntry.gActorMgr.GetActorByServerID(data.TargetID);
                        if (targetObj == null)
                            return;
                        for (int i = 0; i < data.BufferList.Count; i++)
                        {
                            MsgData_sBuffer _buff = data.BufferList[i];
                            BuffData buffdata = new BuffData();
                            buffdata.BufferInstanceID = _buff.BufferInstanceID;
                            buffdata.buffType = _buff.BufferTemplateID;
                            buffdata.Life = _buff.Life;
                            buffdata.Count = 1;
                            //buffdata.Param[0] = data.Param;
                            CoreEntry.gBuffMgr.AddBuff(buffdata, targetObj);
                        }

                    }
                    break;
                case GameEvent.GE_SC_ADDBUFF:
                    {
                        MsgData_sAddBuffer data = parameter.msgParameter as MsgData_sAddBuffer;

                        if (data == null )
                            return;

                        /*
                        if (data.BufferTemplateID == 24100001 || data.BufferTemplateID == 24100002)
                        {
                            Debug.LogError("target目标 "+ data.TargetID +"  添加 iBuffId = " + data.BufferTemplateID);
                        }
                        */
                        // Debug.LogError("target目标 " + data.TargetID + "  添加 iBuffId = " + data.BufferTemplateID);
                        ActorObj targetObj = CoreEntry.gActorMgr.GetActorByServerID(data.TargetID);

                        if (targetObj == null)
                            return;

                        BuffData buffdata = new BuffData();
                        buffdata.BufferInstanceID = data.BufferInstanceID;
                        buffdata.buffType = data.BufferTemplateID;
                        buffdata.Life = data.Life;
                        buffdata.Count = 1;

                        buffdata.Param[0] = data.Param;
                        //LogMgr.LogError("SkillMgr AddBuff " + buffdata.buffType);
                        CoreEntry.gBuffMgr.AddBuff(buffdata, targetObj);

                    }
                    break;

                case GameEvent.GE_SC_UPDATEBUFF:
                    {
                        MsgData_sUpdateBuffer data = parameter.msgParameter as MsgData_sUpdateBuffer;

                        if (data == null)
                            return;

                        BuffData buffdata = new BuffData();
                        buffdata.BufferInstanceID = data.BufferInstanceID;
                        buffdata.Life = data.Life;
                        buffdata.Count = data.Count;
                        buffdata.Param = data.Param;
                        //LogMgr.LogError("SkillMgr UpdateBuff " + buffdata.buffType);
                        CoreEntry.gBuffMgr.UpdateBuff(buffdata);

                    }
                    break;

                case GameEvent.GE_SC_DELBUFF:
                    {
                        MsgData_sDelBuffer data = parameter.msgParameter as MsgData_sDelBuffer;
                        if (data == null)
                            return;
                        if (data.BufferTemplateID == 24100001 || data.BufferTemplateID == 24100002)
                        {
                            //LogMgr.DebugLog("target目标 "+ data.TargetID + "  删除 iBuffId = " + data.BufferTemplateID);
                        }

                        //LogMgr.LogError("SkillMgr DelBuff " + data.BufferTemplateID);
                        //  if(data.BufferInstanceID)
                        ActorObj targetObj = CoreEntry.gActorMgr.GetActorByServerID(data.TargetID);

                        if (targetObj != null)
                        {
                            //LogMgr.LogError("SKillMgr CoreEntry.gBuffMgr.RemoveBuff " + data.BufferTemplateID);
                            CoreEntry.gBuffMgr.RemoveBuff(data.BufferInstanceID, targetObj);
                            if (targetObj.ServerID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                            {
                                EventParameter msg = new EventParameter();
                                msg.longParameter = data.BufferTemplateID;
                                //Debug.LogError("删除BUFF  " + data.BufferTemplateID);
                                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DELEBUFF, msg);
                            }

                        }

                    }
                    break;

                case GameEvent.GE_SC_CAST_MOVE_EFFECRT:
                    {
                        MsgData_sCastMoveEffect data = parameter.msgParameter as MsgData_sCastMoveEffect;
                        if (null == data)
                        {
                            return;
                        }

                        if (data.caseterID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                        {
                            return;
                        }

                        ActorObj casterObj = CoreEntry.gActorMgr.GetActorByServerID(data.caseterID);
                        if (null == casterObj)
                        {
                            return;
                        }

                        LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(data.skillID);
                        if (null == skill_action)
                        {
                            return;
                        }

                        float posX = (float)data.posX;
                        float posZ = (float)data.posY;
                        Vector3 destPos = new Vector3(posX, CommonTools.GetTerrainHeight(new Vector2(posX, posZ)), posZ);

                        AnimationCurveData curveData = casterObj.GetComponent<AnimationCurveData>();
                        if (null == curveData)
                        {
                            LogMgr.LogError(casterObj.gameObject.name + " has no AnimationCurveBase, skill id:" + data.skillID);

                            return;
                        }

                        casterObj.UseCurveData3(skill_action.Get<string>("animation"), destPos, null);
                    }
                    break;
                default:
                    break;
            }
        }

        Dictionary<int, int> m_damageMap = new Dictionary<int, int>();

        public void Log()
        {
            // 只在windows开发环境下运行才输出日志
            if (Application.platform != RuntimePlatform.WindowsEditor)
                return;

            m_damageMap.Clear();

        }

        //技能命中，获取被击的信息，发给被击对象做表现
        public void OnSkillDamage(DamageParam damageParam)
        {
            if (ArenaMgr.Instance.IsArenaFight)
            {
                ArenaMgr.Instance.OnSkillDamage(damageParam);

                return;
            }

            //攻击者的信息                    
            ActorObj attackActorObj = damageParam.attackActor;
            ActorObj behitActorObj = damageParam.behitActor;

            if (null == attackActorObj || null == behitActorObj)
            {
                return;
            }

            if (behitActorObj.IsDeath())
            {
                return;
            }


            BehitParam behitParam = new BehitParam();


            behitParam.displayType = DamageDisplayType.DDT_NORMAL;
            behitParam.hp = damageParam.Damage;

#if !PUBLISH_RELEASE
            if (m_bMiaoGuai)
            {
                if (attackActorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    behitParam.hp += 1000000;
                }
            }
            else
            {
                if (m_bWuShang && attackActorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    behitParam.hp = 1;
                }
            }
#endif

            //被击效果修正
            behitParam.damgageInfo = damageParam;

#if !PUBLISH_RELEASE
            if (m_bWuShang && behitActorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                behitParam.hp = 1;
            }
#endif


            //给技能释放者发送回复信息

            // 发送被击消息给被击者
            //  if (behitParam.hp != 0)
            {
                behitActorObj.OnSkillBeHit(behitParam);
            }

            if (attackActorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                behitActorObj.Health.ShowHPBar(5);
            }

            //受到其它玩家攻击时，提示开启善恶模式
            if (behitParam.hp > 0 && behitActorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                if (attackActorObj.mActorType == ActorType.AT_REMOTE_PLAYER)
                {
                    if (PlayerData.Instance.CurPKMode == PKMode.PK_MODE_PEACE && (PlayerData.LastNoteSwitchPKTime <= 0 || (Time.realtimeSinceStartup - PlayerData.LastNoteSwitchPKTime) >= 60))
                    {
                        bool autosetpk = false;
                        if (autosetpk)
                        {
                            UITips.ShowTips("受到攻击，切换善恶模式");
                            PlayerData.Instance.SendSetPKRuleRequest(PKMode.PK_MODE_EVIL);
                        }
                        else
                        {
                            MainPanelMgr.Instance.ShowDialog("TipSwitchPKNote");
                            PlayerData.LastNoteSwitchPKTime = Time.realtimeSinceStartup;
                        }
                    }

                    //没有目标则选中攻击者
                    ActorObj mainplayer = CoreEntry.gActorMgr.MainPlayer;
                    if (mainplayer.m_SelectTargetObject == null)
                    {
                        mainplayer.SelectTarget(attackActorObj);
                    }

                    EventParameter ep = EventParameter.Get();
                    ep.objParameter = attackActorObj;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PLAYER_PK_HURT, ep);
                }

                //玩家收到了攻击
                AutoAIMgr automgr = CoreEntry.gAutoAIMgr;
                if (automgr.Config.AutoStrikeBack && TaskMgr.RunTaskType == 0 && !automgr.AutoFight && !behitActorObj.AutoPathFind)
                {
                    float gap = Time.realtimeSinceStartup - automgr.LastOPTime;
                    //LogMgr.Log("Last op gap {0}", gap);
                    if (gap > automgr.Config.StrikeBackGapWithIdle)
                    {
                        automgr.AutoFight = true;
                    }                 
                }
            }
        }

        //技能命中，获取被击的信息，发给被击对象做表现
        public void OnBuffDamage(DamageParam damageParam, float phyAttackPercent, float magicAttackPercent, float addedValue, sbyte skillAttrib)
        {
            //攻击者的信息                    
            ActorObj attackActorObj = damageParam.attackActor;
            ActorObj behitActorObj = damageParam.behitActor;

            if (behitActorObj.IsDeath())
            {
                return;
            }
            
            //LuaTable attackSkillDesc = attackActorObj.GetCurSkillDesc(damageParam.skillID);

            BehitParam behitParam = new BehitParam();

            behitParam.displayType = DamageDisplayType.DDT_NORMAL;

            float critRate = 0;
            if (critRate < 0)
            {
                critRate = 0;
            }
            critRate = Math.Min(critRate, 1.0f);
            
            if (UnityEngine.Random.value < critRate)  //暴击
            {
                behitParam.displayType = DamageDisplayType.DDT_DOUBLE;
            }

            int finalDmg = 0;
            //string strPhydmg = "";

            behitParam.hp = finalDmg;


#if !PUBLISH_RELEASE
            if (m_bMiaoGuai)
            {
                if (attackActorObj.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    behitParam.hp += 1000000;
                }
            }
#endif

            //被击效果修正
            behitParam.damgageInfo = damageParam;
            behitParam.displayType = DamageDisplayType.DDT_BUFF;

            //给技能释放者发送回复信息



            // 发送被击消息给被击者
            behitActorObj.OnSkillBeHit(behitParam);
        }


        //是否技能伤害成功
        public bool IsSkillDamageRange(int skillID, Transform srcTransform,
            Transform dstTransform, float dstRadius)
        {
            bool isSkillSuccess = false;

            LuaTable skillDesc = ConfigManager.Instance.Skill.GetEffectConfig(skillID);

            if (skillDesc == null)
                return false;


            //目标单体
            if (skillDesc.Get<int>("range") == (int)SkillRangeType.SKILL_TARGET || skillDesc.Get<int>("range") == (int)SkillRangeType.SKILL_SELF_CIRCLE)
            {
                Vector3 srcPos = srcTransform.position;

                //直接判断是否小于最小距离
                if (m_baseTool.IsPointInCircleXZ(srcPos, dstTransform.position,
                     skillDesc.Get<int>("distance"), dstRadius))
                {
                    isSkillSuccess = true;
                }

            }

            //目标圆形范围
            if (skillDesc.Get<int>("range") == (int)SkillRangeType.SKILL_TARGET_CIRCLE)
            {
                Vector3 srcPos = srcTransform.position;

                //直接判断 
                if (m_baseTool.IsPointInCircleXZ(srcPos, dstTransform.position,
                     skillDesc.Get<int>("distance"), dstRadius))
                {
                    isSkillSuccess = true;
                }

            }


            //自身原点锥形
            if (skillDesc.Get<int>("range") == (int)SkillRangeType.SKILL_SELF_FUN)
            {
                Vector3 srcPos = srcTransform.position;

                //直接判断 
                if (m_baseTool.PointInFunXZ(srcPos, srcTransform.rotation.eulerAngles,
                        skillDesc.Get<int>("angle"), skillDesc.Get<int>("distance"),
                        dstTransform.position, dstRadius))
                {
                    isSkillSuccess = true;
                }

            }

            //自身边中心矩形
            if (skillDesc.Get<int>("range") == (int)SkillRangeType.SKILL_SELF_RECT)
            {
                Vector3 srcPos = srcTransform.position;

                //矩形攻击
                if (m_baseTool.IsPointInRectangleXZ(srcPos, srcTransform.rotation.eulerAngles,
                    skillDesc.Get<int>("angle"), skillDesc.Get<int>("distance"), dstTransform.position))
                {
                    isSkillSuccess = true;
                }

            }


            return isSkillSuccess;
        }


        //五行属性分为：冰火雷风
        //相克关系：
        //冰克火
        //火克雷
        //雷克风
        //风克冰
        //如果相克的话，在属性伤害最外层会额外增加一定比值的伤害。（目前暂定10%），相克只计算伤害，不计算其他效果
        public static float getElementAdd(BaseAttr atkAttr, BaseAttr behitAttr)
        {
            float re = 1.0f;// getElementAdd(atkAttr.CreatureDesc.element, behitAttr.CreatureDesc.element);
            //LogMgr.UnityLog("相克值:" + re);

            return re;
        }
        public static float getElementAdd(int atk, int behit)
        {
            return 1;
        }


        ////////////////////////////////////////////////////

        public LuaTable GetSkillActon(int nSkillID)
        {
            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(nSkillID);
            if (skillDesc != null)
            {
                int nSkillActionID = Convert.ToInt32(skillDesc.Get<string>("skill_action"));

                return ConfigManager.Instance.Skill.GetSkillActionConfig(nSkillActionID);

            }
            return null;
        }


        public LuaTable GetSkillEffect(int nSkillID, int nIndex)
        {

            LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(nSkillID);
            if (skillDesc != null)
            {
                if (nIndex == 1)
                {
                    if (skillDesc.Get<int>("e_type_1") == 1 || skillDesc.Get<int>("e_type_1") == 3)
                        return ConfigManager.Instance.Skill.GetEffectConfig(skillDesc.Get<int>("effect_1"));


                }

                if (nIndex == 2)
                {
                    if (skillDesc.Get<int>("e_type_1") == 1 || skillDesc.Get<int>("e_type_1") == 3)
                        return ConfigManager.Instance.Skill.GetEffectConfig(skillDesc.Get<int>("effect_2"));
                }

                if (nIndex == 3)
                {
                    if (skillDesc.Get<int>("e_type_1") == 1 || skillDesc.Get<int>("e_type_1") == 3)
                        return ConfigManager.Instance.Skill.GetEffectConfig(skillDesc.Get<int>("effect_3"));
                }

            }
            return null;
        }
    }   //end skillmgr
}

