using UnityEngine;
using System.Collections;
using SG.AutoAI;
using XLua;

namespace SG
{
    /// <summary>
    /// 自动AI管理。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class AutoAIMgr : MonoBehaviour
    {
        /// <summary>
        /// 自动战斗配置。
        /// </summary>
        private AutoAIConfig m_Config = null;

        /// <summary>
        /// 获取自动战斗配置。
        /// </summary>
        public AutoAIConfig Config
        {
            get
            {
                if (m_Config == null)
                {
                    m_Config = AutoAIConfig.LoadConfig();
                }
                return m_Config;
            }
        }

        /// <summary>
        /// 重新加载配置。
        /// </summary>
        public void ReloadConfig()
        {
            m_Config = AutoAIConfig.LoadConfig();
        }

        /// <summary>
        /// 自动战斗行为。
        /// </summary>
        private AutoAI.Action m_FightBehavior = null;

        /// <summary>
        /// 是否启用自动战斗。
        /// </summary>
        private bool m_AutoFight = false;

        /// <summary>
        /// 获取或设置是否使用自动战斗。
        /// </summary>
        public bool AutoFight
        {
            get { return m_AutoFight; }
            set
            {
                if (m_AutoFight == value)
                {
                    return;
                }
                m_AutoFight = value;                
                if (m_AutoFight)
                {
                    m_SuspendDelayCount = 0;
                    m_FightBehavior.Reset();
                }
                m_FightBehavior.Run = m_AutoFight;

                //改变通知
                EventParameter ep = EventParameter.Get();
                ep.intParameter = m_AutoFight ? 1 : 0;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_AUTO_FIGHT, ep);
            }
        }

        public void MoorPhSetAuto(bool value)
        {
            if (m_AutoFight == value)
            {
                return;
            }
            m_AutoFight = value;
            if (m_AutoFight)
            {
                m_SuspendDelayCount = 0;
                m_FightBehavior.Reset();
            }
            m_FightBehavior.Run = m_AutoFight;

            //改变通知
            EventParameter ep = EventParameter.Get();
            ep.intParameter = m_AutoFight ? 1 : 0;
            ep.intParameter1 = 100;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_AUTO_FIGHT, ep);
        }

        /// <summary>
        /// 是否暂时挂起。
        /// </summary>
        private bool m_IsSuspend = false;

        /// <summary>
        /// 挂起延时计数。
        /// </summary>
        private float m_SuspendDelayCount = 0;

        /// <summary>
        /// 挂起延时，结束刮起后一定时间才恢复。
        /// </summary>
        public static float SuspendDelay = 1;

        /// <summary>
        /// 获取或设置自动AI是否暂时挂起。
        /// </summary>
        public bool IsSuspend
        {
            get { return m_IsSuspend; }
            set
            {
                m_IsSuspend = value;
                if (m_IsSuspend)
                {
                    bool auto = AutoFight;
                    AutoFight = false;              //得停一下当前自动逻辑
                    AutoFight = auto;
                }
                else
                {
                    m_SuspendDelayCount = SuspendDelay;
                }
            }
        }

        public void Awake()
        {
            m_FightBehavior = CreateBehaviorTree();
        }

        // Use this for initialization
        void Start()
        {

        }

        /// <summary>
        /// 获取上次操作时间。
        /// </summary>
        public float LastOPTime
        {
            get { return m_LastOPTime; }
        }

        /// <summary>
        /// 上次操作时间。
        /// </summary>
        private float m_LastOPTime = 0;

        // Update is called once per frame
        void Update()
        {
            if (IsTouching())
            {
                m_LastOPTime = Time.realtimeSinceStartup;
            }

            if (m_FightBehavior != null && m_FightBehavior.Run && !m_IsSuspend)
            {
                if (m_SuspendDelayCount > 0)
                {
                    m_SuspendDelayCount -= Time.deltaTime;
                }
                else
                {
                    m_FightBehavior.Update();
                }                
            }            
        }

        /// <summary>
        /// 是否触摸屏幕
        /// </summary>
        /// <returns></returns>
        public static bool IsTouching()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            return true;
        }
#elif UNITY_ANDROID || UNITY_IOS
        int count = Input.touchCount;
        if (count > 0)
        {
                return true;
        }
#endif
         return false;
        }

        /// <summary>
        /// 创建自动战斗的行为树。
        /// </summary>
        /// <returns>自动战斗的行为树。</returns>
        private Action CreateBehaviorTree()
        {
            ParallelAction ret = new AutoAI.ParallelAction();

            //保持血量
            SequenceAction keeplife = new SequenceAction();
            keeplife.AddAction(new AutoAICheckLife());
            keeplife.AddAction(new AutoAICheckPotion());
            keeplife.AddAction(new AutoAIUsePotion());
            ret.AddAction(new RepeatAction(keeplife, 0, 5));

            //战斗
            SequenceAction fight = new SequenceAction();
            AutoAIFindTarget find = new AutoAIFindTarget();
            fight.AddAction(find);
            fight.AddAction(new MoveAction(Vector3.zero, 0, find.GetTargetPosition));
            fight.AddAction(new AutoAIAttackTarget());
            ret.AddAction(new RepeatAction(fight));

            //复活
            return ret;
        }
    }
}

