using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;

namespace SG.AutoAI
{
    /// <summary>
    /// 查找目标。
    /// </summary>
[Hotfix]
    public class AutoAIFindTarget : Action
    {
        /// <summary>
        /// 查找目标间隔。
        /// </summary>
        public static float FindGap = 1;

        /// <summary>
        /// 副本类型集合。
        /// </summary>
        public static HashSet<int> DungeonType = null;

        /// <summary>
        /// 判断是否在副本中。
        /// </summary>
        /// <returns>是否为副本。</returns>
        public static bool IsInDungeon()
        {
            if (DungeonType == null)
            {
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                List<int> types = G.GetInPath<List<int>>("ConfigData.MapConfig.DungeonType");
                DungeonType = new HashSet<int>();
                for (int i=0;i< types.Count; ++i)
                {
                    DungeonType.Add(types[i]);
                }
            }

            int mapid = MapMgr.Instance.EnterMapId;
            LuaTable cfg = ConfigManager.Instance.Map.GetMapConfig(mapid);
            int type = cfg.Get<int>("type");
            return DungeonType.Contains(type);
        }

        /// <summary>
        /// 野外是否查询怪物位置。
        /// </summary>
        public static bool WildQuery = false;

        /// <summary>
        /// 目标所在位置。
        /// </summary>
        private Vector3 m_TargetPosition;

        /// <summary>
        /// 是否请求位置中。
        /// </summary>
        private bool m_IsRequestPositon;

        /// <summary>
        /// 使用等待计数。
        /// </summary>
        private float m_QueryWaitCount = 0;

        /// <summary>
        /// 等待的查询列表。
        /// </summary>
        private List<Vector3> m_QueryList = new List<Vector3>();

        /// <summary>
        /// 当前查询索引。
        /// </summary>
        private int m_QueryIndex = 0;

        /// <summary>
        /// 是否有目标。
        /// </summary>
        private bool m_IsHaveTarget;

        /// <summary>
        /// 查找间隔计数。
        /// </summary>
        private float m_FindGapCount = 0;

        /// <summary>
        /// 获取目标位置。
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTargetPosition()
        {
            return m_TargetPosition;
        }

        public override void Reset()
        {
            LogMgr.LogAI("AutoAIFindTarget.Reset");
            //m_TargetPosition = Vector3.zero;
            m_IsRequestPositon = false;
            m_IsHaveTarget = false;
            m_FindGapCount = 0;
            m_QueryList.Clear();
            m_QueryIndex = 0;
        }

        protected override void OnStart()
        {
            LogMgr.LogAI("AutoAIFindTarget.OnStart");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_QueryMonsterByPosition, OnQueryPositoin);
            TryFindTarget();
        }

        protected override void OnStop()
        {
            LogMgr.LogAI("AutoAIFindTarget.OnStop");
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_QueryMonsterByPosition, OnQueryPositoin);
        }

        /// <summary>
        /// 查询怪物返回。
        /// </summary>
        void OnQueryPositoin(GameEvent ge, EventParameter parameter)
        {
            MsgData_sQueryMonsterByPosition data = parameter.msgParameter as MsgData_sQueryMonsterByPosition;
            m_IsRequestPositon = false;
            m_QueryWaitCount = 0;
            m_IsHaveTarget = data.num > 0;
            m_TargetPosition.x = (int)((float)data.x / 1000f);
            m_TargetPosition.z = (int)((float)data.y / 1000f);            
            LogMgr.LogAI("AutoAIFindTarget.Update. Query monster number:{0}", data.num);

            //是否还要继续查询
            if (!m_IsHaveTarget)
            {
                TryFindTarget();
            }
        }

        public override ActionState Update()
        {
            if (m_IsRequestPositon)
            {
                m_QueryWaitCount -= Time.deltaTime;
                if (m_QueryWaitCount <= 0)
                {
                    m_QueryWaitCount = 0;
                    m_IsRequestPositon = false;
                    LogMgr.LogAI("AutoAIFindTarget.Update. Query wait time out!");
                    TryFindTarget();
                }
                return ActionState.Running;
            }

            if (m_FindGapCount > 0)
            {
                m_FindGapCount -= Time.deltaTime;
                if (m_FindGapCount <= 0)
                {
                    m_FindGapCount = 0;
                    TryFindTarget();
                }
            }

            return m_IsHaveTarget ? ActionState.Succeed : ActionState.Running;      //查找目标永远不会失败
        }

        /// <summary>
        /// 尝试查找目标。
        /// </summary>
        private void TryFindTarget()
        {
            FindTarget();
            m_FindGapCount = m_IsHaveTarget ? 0 : FindGap;
        }

        /// <summary>
        /// 查找目标。
        /// </summary>
        private void FindTarget()
        {
            PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
            if (player == null)
            {
                return;
            }

            //有任务在身时，判断任务位置
            Vector3 selfpos = player.transform.position;
            int mid = 0;
            LogMgr.LogAI("AutoAIFindTarget.FindTarget");
            if (TaskMgr.RunTaskType != 0 && !IsInDungeon())
            {
                mid = TaskMgr.Instance.GetCurTargetMonsterId();
                if (mid != 0)
                {
                    const float RESET_DISTANCE = 5;             //重新回到任务点距离
                    Vector3 pos = TaskMgr.Instance.goPos;
                    float x = selfpos.x - pos.x;
                    float z = selfpos.z - pos.z;
                    if (x*x + z*z >= RESET_DISTANCE * RESET_DISTANCE)
                    {
                        LogMgr.LogAI("AutoAIFindTarget.FindTarget 离开任务目标位置太远，重新回去");
                        m_TargetPosition = pos;
                        m_IsHaveTarget = true;
                        return;
                    }
                }
            }
            
            int mapid = MapMgr.Instance.EnterMapId;
            LuaTable cfg = ConfigManager.Instance.Map.GetMapConfig(mapid);
            ActorObj actor = player.FindTarget(cfg.Get<int>("autoFightRange"), mapid);
            if (actor != null)
            {
                //如果选择的不是当前怪物则忽略
                if (mid !=0 && actor is MonsterObj)
                {
                    MonsterObj monster = actor as MonsterObj;
                    if (monster.ConfigID != mid)
                    {
                        return;
                    }                    
                }

                m_TargetPosition = actor.transform.position;
                m_IsHaveTarget = true;
                return;
            }

            if (MapMgr.Instance.CurMapType == MapMgr.MapType.Map_CrossServer)
                return ;


            //向服务器查询位置            
            if (WildQuery || IsInDungeon())
            {
                //加载查询列表
                if (m_QueryList.Count <= 0)
                {
                    m_QueryList = GetSortedEntList(player.transform.position);
                    m_QueryIndex = 0;
                }

                //开始查询
                if (m_QueryList.Count > 0)
                {
                    Vector3 pos = m_QueryList[m_QueryIndex];
                    m_QueryIndex = (m_QueryIndex + 1) % m_QueryList.Count;
                    m_IsRequestPositon = true;
                    m_QueryWaitCount = 5;
                    NetLogicGame.Instance.SendQueryMonsterByPosition((int)(pos.x * 1000), (int)(pos.z * 1000));
                    LogMgr.LogAI("AutoAIFindTarget.FindTarget RequestPositon:{0}", pos);
                }
                else
                {
                    LogMgr.LogAI("AutoAIFindTarget.FindTarget The QueryList is empty!");
                }                
            }
            else
            {
                LogMgr.LogAI("AutoAIFindTarget.FindTarget Can not find target.");
            }
        }

        public static List<Vector3> GetSortedEntList(Vector3 pos)
        {
            List<Vector3> positions = new List<Vector3>();
            List<SceneEntitySet> infos = CoreEntry.gGameDBMgr.GetEnityConfigInfo(MapMgr.Instance.EnterMapId);
            if (infos == null)
            {
                return positions;
            }

            const int MAX_POSITION = 50;
            for (int i=0; i< infos.Count && positions.Count < MAX_POSITION; ++i)
            {
                SceneEntitySet info = infos[i];
                if (info.type == EntityConfigType.ECT_MONSTER)
                {
                    for (int j = 0; j < info.entityList.Count && positions.Count < MAX_POSITION; ++j)
                    {
                        SceneEntityConfig cfg = info.entityList[j];
                        positions.Add(cfg.position);
                    }
                }
            }

            //按离指定位置距离进行排序
            positions.Sort((a, b) => { return Vector3.SqrMagnitude(pos - a).CompareTo(Vector3.SqrMagnitude(pos - b)); });
            return positions;
        }
    }

    /// <summary>
    /// 攻击目标。
    /// </summary>
[Hotfix]
    public class AutoAIAttackTarget : Action
    {
        /// <summary>
        /// 控制的角色。
        /// </summary>
        private PlayerObj m_Actor = null;

        /// <summary>
        /// 攻击的目标。
        /// </summary>
        private ActorObj m_Target = null;
        
        /// <summary>
        /// 当前使用的技能编号。
        /// </summary>
        private int m_CurSkillID = 0;

        /// <summary>
        /// 使用技能硬直计数。
        /// </summary>
        private float m_StiffCount = 0;

        public override void Reset()
        {
            LogMgr.LogAI("AutoAIAttackTarget.Reset");
            m_Actor = null;
            m_Target = null;
            m_CurSkillID = 0;
            m_StiffCount = 0;
        }

        protected override void OnStart()
        {
            LogMgr.LogAI("AutoAIAttackTarget.OnStart");
            m_Actor = CoreEntry.gActorMgr.MainPlayer;
            if (m_Actor != null)
            {
                m_Target = m_Actor.CheckSkillTarget();
            }
        }

        protected override void OnStop()
        {
            LogMgr.LogAI("AutoAIAttackTarget.OnStop");
        }

        public override ActionState Update()
        {
            //硬直结束再说
            if (m_StiffCount > 0)
            {
                m_StiffCount -= Time.deltaTime;
                if (m_StiffCount <= 0)
                {
                    m_StiffCount = 0;
                }
                return ActionState.Running;
            }

            m_Actor = CoreEntry.gActorMgr.MainPlayer;
            if (m_Actor == null || m_Target == null)
            {
                return ActionState.Stop;
            }
            if (m_Actor.IsDeath())
            {
                return ActionState.Failed;
            }
            if (m_Target.IsDeath())
            {
                return ActionState.Succeed;
            }
            
            TryAttack();
            return ActionState.Running;
        }

        /// <summary>
        /// 尝试攻击。
        /// </summary>
        private void TryAttack()
        {
            if (!CanAttack(m_Actor))
            {
                return;
            }

            //判断之前的技能是否释放完毕
            if (m_CurSkillID != 0)
            {
                if (IsCastSkillOver(m_Actor, m_CurSkillID))
                {
                    m_CurSkillID = 0;
                }                
                return;
            }

            m_CurSkillID = ChooseSkill();
            if (m_CurSkillID != 0)
            {
                if (CastSkill(m_Actor, m_CurSkillID))
                {
                    LuaTable t = ConfigManager.Instance.Skill.GetSkillConfig(m_CurSkillID);
                    m_StiffCount = t.Get<float>("stiff_time")/1000.0f;          //等待一个硬直时间
                }                
            }
        }

        /// <summary>
        /// 判断角色是否可进行攻击。
        /// </summary>
        /// <param name="actor">角色对象。</param>
        /// <returns>是否可进行攻击。</returns>
        public static bool CanAttack(ActorObj actor)
        {
            if (!actor.CanChangeAttack() || actor.curActorState == ACTOR_STATE.AS_RUN)
            {
                return false;
            }

            //当前是否硬直状态
            if (actor.IsNonControl || actor.IsActorEndure())
            {
                return false;
            }
            return true;
        }

        public const int MorphShowType = 93;                //变身之后的技能
        public const int SpecialSkillType = 73;             //特殊技能类型

        /// <summary>
        /// 选择一个技能。
        /// </summary>
        /// <param name="actor">角色对象。</param>
        /// <returns>选择的技能编号。</returns> 
        public int preSkill = 0; //保存上次释放的技能id
        public int ChooseSkill()
        {
            int skillid = 0;
            int usepriority = 0;
            int level = PlayerData.Instance.BaseAttr.Level;
            bool ismor = CoreEntry.gMorphMgr.IsMorphing;        //是否在变身中

            int skillidReal = 0;//普攻技能可以重复释放
            foreach (KeyValuePair<int, int> e in m_Actor.m_skillBindsDict)
            {
                //跳过冷却中的技能
                if (m_Actor.IsInCoolDownTime(e.Value))
                {
                    continue;
                }

                //等级限制
                LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(e.Value);
                if (skillDesc == null || level < skillDesc.Get<int>("needLvl"))
                {
                    continue;
                }

                //变身状态下要使用变身对应的技能
                int showtype = skillDesc.Get<int>("showtype");
                if ((ismor && showtype != MorphShowType) || (!ismor && showtype == MorphShowType))
                {
                    continue;
                }

                //配置是否使用特殊技能
                if (showtype == SpecialSkillType && !CoreEntry.gAutoAIMgr.Config.UseSpecialSkill)
                {
                    continue;
                }

                //选择优先级最高的技能
                int priority = skillDesc.Get<int>("priority");
                if(priority == -1 )
                {
                    continue;
                }
                if (usepriority == 0 || usepriority < priority)
                {
                    skillidReal = e.Value;
                    usepriority = priority;
                    //当前技能被卡住强制释放下个技能
                    if ((preSkill > 0 && preSkill != e.Value) || preSkill <= 0)
                    {
                        skillid = e.Value;

                    } 
                }
            }
            if (skillid == 0)
            {
                skillid = skillidReal;
            } 
            preSkill = skillid;
            return skillid;
        }

        /// <summary>
        /// 是否技能。
        /// </summary>
        /// <param name="actor">要是否技能的角色对象。</param>
        /// <param name="skillid">技能编号。</param>
        /// <returns>是否能成功释放。</returns>
        public static bool CastSkill(ActorObj actor, int skillid)
        {
            if (!actor.CanCastSkill(skillid))
            {
                return false;
            }

            actor.StopMove(true);
            EventParameter param = EventParameter.Get();
            param.goParameter = actor.gameObject;
            param.intParameter = skillid;            
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_NOTIFY_CAST_SKILL, param);

            return true;
        }

        /// <summary>
        /// 判断技能是否使用完毕，有连招的会继续使用连招。
        /// </summary>
        /// <param name="actor">角色对象。</param>
        /// <param name="skillid">技能编号。</param>
        /// <returns>是否使用完毕。</returns>
        public static bool IsCastSkillOver(ActorObj actor, int skillid)
        {
            if (actor.curActorState != ACTOR_STATE.AS_ATTACK)
            {
                return true;
            }

            //如果在攻击状态没有动作，则视为技能施放完毕
            if (actor.GetComponent<Animation>().isPlaying == false && actor.m_bIsTower == false)
            {
                return true;
            }

            //判断连招，是否组合技能              
            ComposeSkillDesc desc = CoreEntry.gGameDBMgr.GetComposeSkillDesc(actor.curCastSkillID, skillid);
            if (desc != null)
            {
                StateParameter param = new StateParameter();
                param.state = ACTOR_STATE.AS_ATTACK;
                param.skillID = skillid;
                param.isComposeSkill = true;
                param.composeSkillDesc = desc;

                //请求连招切换，能连招则认为此技能结束了
                bool bRet = actor.ComposeSkillCanCastSkill(param.composeSkillDesc.changeTime);
                return bRet;
            }

            return false;
        }
    }
}

