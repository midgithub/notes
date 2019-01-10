using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG.AutoAI
{
    /// <summary>
    /// AI行为状态。
    /// </summary>
    public enum ActionState
    {
        /// <summary>
        /// 停止状态。
        /// </summary>
        Stop,

        /// <summary>
        /// 运行中。
        /// </summary>
        Running,
        
        /// <summary>
        /// 已成功。
        /// </summary>
        Succeed,

        /// <summary>
        /// 已失败。
        /// </summary>
        Failed,
    }

    /// <summary>
    /// 自动AI动作，自动战斗行为由Action组合而成。动作使用时，先调用Reset，再将Run设为true。
    /// </summary>
[Hotfix]
    public abstract class Action
    {
        /// <summary>
        /// 是否运行中。
        /// </summary>
        protected bool m_Run = false;

        /// <summary>
        /// 获取是否运行动作。
        /// </summary>
        public virtual bool Run
        {
            get { return m_Run; }
            set
            {
                if (m_Run == value)
                {
                    return;
                }
                m_Run = value;
                if (m_Run)
                {
                    OnStart();
                }
                else
                {
                    OnStop();
                }
            }
        }

        /// <summary>
        /// 重置动作。
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// 开始。
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// 结束。
        /// </summary>
        protected virtual void OnStop() { }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>动作状态。</returns>
        public virtual ActionState Update() { return m_Run ? ActionState.Running : ActionState.Stop; }
    }

    /// <summary>
    /// 组合节点。
    /// </summary>
[Hotfix]
    public class CompositeAction : Action
    {
        /// <summary>
        /// 子节点列表。
        /// </summary>
        protected List<Action> m_Children = new List<Action>();

        /// <summary>
        /// 获取子节点列表。
        /// </summary>
        public List<Action> Children
        {
            get { return m_Children; }
        }
        
        /// <summary>
        /// 重置动作。
        /// </summary>
        public override void Reset()
        {
            for (int i = 0; i < m_Children.Count; ++i)
            {
                m_Children[i].Reset();
            }
        }

        /// <summary>
        /// 添加动作。
        /// </summary>
        /// <param name="act">动作。</param>
        public void AddAction(Action act)
        {
            m_Children.Add(act);
        }

        /// <summary>
        /// 移除动作。
        /// </summary>
        /// <param name="act">动作。</param>
        public void RemoveAction(Action act)
        {
            m_Children.Remove(act);
        }
    }

    /// <summary>
    /// 并行节点。所有节点都在运行。
    /// </summary>
[Hotfix]
    public class ParallelAction : CompositeAction
    {
        /// <summary>
        /// 并行的返回模式。
        /// </summary>
        public enum ParallelMode
        {
            /// <summary>
            /// 全部成功才返回成功。
            /// </summary>
            And,

            /// <summary>
            /// 一个成功则返回成功。
            /// </summary>
            Or,
        }

        /// <summary>
        /// 并行策略模式。
        /// </summary>
        private ParallelMode m_Mode = ParallelMode.Or;

        /// <summary>
        /// 获取或设置并行策略模式。
        /// </summary>
        public ParallelMode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        /// <summary>
        /// 获取或设置是否启用动作。
        /// </summary>
        public override bool Run
        {
            set
            {
                base.Run = value;
                for (int i = 0; i < m_Children.Count; ++i)
                {
                    m_Children[i].Run = value;
                }
            }
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>动作状态。</returns>
        public override ActionState Update()
        {
            bool isrunnig = false;      //是否有运行中的动作
            int sn = 0;                 //成功的动作数量
            int fn = 0;                 //失败的动作数量
            for (int i = 0; i < m_Children.Count; ++i)
            {
                Action act = m_Children[i];
                if (act.Run)
                {
                    ActionState state = act.Update();
                    if (state == ActionState.Running)
                    {
                        isrunnig = true;
                    }
                    else if (state == ActionState.Succeed)
                    {
                        sn = sn + 1;
                    }
                    else if (state == ActionState.Failed)
                    {
                        fn = fn + 1;
                    }
                }                
            }

            //只要有一个是运行状态则返回运行状态。
            if (isrunnig)
            {
                return ActionState.Running;
            }

            //没有成功或失败的
            if (sn + fn <= 0)
            {
                return ActionState.Stop;
            }

            //一个成功则返回成功
            if (m_Mode == ParallelMode.Or)
            {
                return sn > 0 ? ActionState.Succeed : ActionState.Failed;
            }

            //一个失败则返回失败
            if (m_Mode == ParallelMode.And)
            {
                return fn > 0 ? ActionState.Failed : ActionState.Succeed;
            }

            return ActionState.Stop;        //理论上不会走到这一句
        }
    }
    
    /// <summary>
    /// 顺序节点，逐个Action执行。
    /// </summary>
[Hotfix]
    public class SequenceAction : CompositeAction
    {
        /// <summary>
        /// 当前的动作索引。
        /// </summary>
        private int m_Index = 0;

        /// <summary>
        /// 重置动作。
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            m_Index = 0;
        }

        /// <summary>
        /// 获取或设置是否启用动作。
        /// </summary>
        public override bool Run
        {
            set
            {
                base.Run = value;
                if (m_Children.Count > 0)
                {
                    m_Children[0].Run = value;
                    for (int i = 1; i < m_Children.Count; ++i)
                    {
                        m_Children[i].Run = false;
                    }
                }           
            }
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>动作状态。</returns>
        public override ActionState Update()
        {
            //都运行完了。
            if (m_Index >= m_Children.Count)
            {
                return ActionState.Stop;
            }

            Action act = m_Children[m_Index];
            ActionState state = act.Update();
            if (state == ActionState.Running)
            {
                return ActionState.Running;
            }
            if (state == ActionState.Failed)
            {
                return ActionState.Failed;
            }
            if (state == ActionState.Succeed)
            {
                //当前成功了则执行下一个，如果是最后一个了则返回成功
                act.Run = false;
                ++m_Index;
                if (m_Index < m_Children.Count)
                {
                    m_Children[m_Index].Run = true;     //逐个启用
                }
                return m_Index >= m_Children.Count ? ActionState.Succeed : ActionState.Running;
            }
            return ActionState.Stop;
        }
    }

    /// <summary>
    /// 重复动作。
    /// </summary>
[Hotfix]
    public class RepeatAction : Action
    {
        /// <summary>
        /// 重复次数，小于等于0表示一直重复。
        /// </summary>
        private int m_Number = 0;

        //获取或设置重复次数。
        public int Number
        {
            get { return m_Number; }
            set { m_Number = value; }
        }        

        /// <summary>
        /// 当前重复计数。
        /// </summary>
        private int m_Count = 0;

        /// <summary>
        /// 获取当前重复的次数。
        /// </summary>
        public int Count
        {
            get { return m_Count; }
        }

        /// <summary>
        /// 需要重复执行的动作。
        /// </summary>
        private Action m_Target;

        /// <summary>
        /// 获取或设置需要重复的动作。
        /// </summary>
        public Action Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

        /// <summary>
        /// 间隔时间。
        /// </summary>
        private float m_GapTime = 0;

        /// <summary>
        /// 获取或设置重复间隔。
        /// </summary>
        public float GapTime
        {
            get { return m_GapTime; }
            set { m_GapTime = Mathf.Max(0, value); }
        }

        /// <summary>
        /// 间隔时间计数。
        /// </summary>
        private float m_GapCount = 0;

        /// <summary>
        /// 获取间隔计数。
        /// </summary>
        public float GapCount
        {
            get { return m_GapCount; }
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="act">要重复的动作。</param>
        /// <param name="number">要重复的次数。</param>
        public RepeatAction(Action act = null, int number = 0, float gap = 0)
        {
            m_Target = act;
            m_Number = number;
            m_GapTime = Mathf.Max(0, gap);
        }

        /// <summary>
        /// 获取或设置是否启用动作。
        /// </summary>
        public override bool Run
        {
            set
            {
                base.Run = value;
                if (m_Target != null)
                {
                    m_Target.Run = value;
                }
            }
        }

        /// <summary>
        /// 重置动作。
        /// </summary>
        public override void Reset()
        {
            m_Count = 0;
            if (m_Target != null)
            {
                m_Target.Reset();
            }
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>动作状态。</returns>
        public override ActionState Update()
        {
            if (m_Target == null)
            {
                return ActionState.Stop;
            }

            //重复间隔计数
            if (m_GapCount > 0)
            {
                m_GapCount -= Time.deltaTime;
                if (m_GapCount <= 0)
                {
                    m_GapCount = 0;
                    m_Target.Reset();
                    m_Target.Run = true;
                }
                return ActionState.Running;
            }


            if (!m_Target.Run) return ActionState.Stop;

            ActionState state = m_Target.Update();
            if (state != ActionState.Running)
            {
                m_Target.Run = false;
                ++m_Count;                
                if (m_Number <= 0 || m_Count < m_Number)
                {
                    if (m_GapTime > 0)
                    {
                        m_GapCount = m_GapTime;
                    }
                    else
                    {
                        m_Target.Reset();
                        m_Target.Run = true;
                    }                    
                }                
            }
            
            return (m_Number <= 0 || m_Count < m_Number) ? ActionState.Running : ActionState.Stop;
        }
    }

    /// <summary>
    /// 等待动作。
    /// </summary>
[Hotfix]
    public class WaitAction : Action
    {
        /// <summary>
        /// 等待时间。
        /// </summary>
        private float m_Duration;

        /// <summary>
        /// 获取或设置等待时间。
        /// </summary>
        public float Duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        /// <summary>
        /// 等待时间计数。
        /// </summary>
        private float m_Count;

        /// <summary>
        /// 获取已经等待的时间。
        /// </summary>
        public float Count
        {
            get { return m_Count; }
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="duration">等待的时间秒。</param>
        public WaitAction(float duration = 1)
        {
            m_Duration = duration;
        }

        /// <summary>
        /// 重置动作。
        /// </summary>
        public override void Reset()
        {
            m_Count = 0;
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>动作状态。</returns>
        public override ActionState Update()
        {
            m_Count += Time.deltaTime;
            return m_Count >= m_Duration ? ActionState.Succeed : ActionState.Running;
        }
    }

    /// <summary>
    /// 移动动作。
    /// </summary>
[Hotfix]
    public class MoveAction : Action
    {
        public delegate Vector3 GetMoveTargetDelegate();

        /// <summary>
        /// 移动目标。
        /// </summary>
        private Vector3 m_Target = Vector3.zero;

        /// <summary>
        /// 获取目标回调。若不为null，则OnStart时会调一次。
        /// </summary>
        private GetMoveTargetDelegate m_GetTargetCall = null;

        /// <summary>
        /// 获取移动目标。
        /// </summary>
        public Vector3 Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

        /// <summary>
        /// 误差范围。
        /// </summary>
        private float m_ErrorRange = 3;

        /// <summary>
        /// 角色编号。
        /// </summary>
        private int m_ActorID = 0;

        /// <summary>
        /// 要控制移动的角色。
        /// </summary>
        private ActorObj m_Actor;

        /// <summary>
        /// 是否已经到达目标。
        /// </summary>
        private bool m_Ok = false;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="actid">要控制的对象编号。</param>
        /// <param name="call"></param>
        public MoveAction(Vector3 target, int actid = 0, GetMoveTargetDelegate call = null)
        {
            m_ActorID = actid;
            m_Target = target;
            m_GetTargetCall = call;
        }

        /// <summary>
        /// 重置动作。
        /// </summary>
        public override void Reset()
        {
            m_Ok = false;
        }

        protected override void OnStart()
        {
            if (m_GetTargetCall != null)
            {
                m_Target = m_GetTargetCall();
            }
            m_Actor = CoreEntry.gActorMgr.GetActorByConfigID(m_ActorID);
            if (m_Actor != null)
            {
                if (IsMoveToEnd())
                {
                    m_Actor.StopMove(false);
                }
                else
                {
                    if (!m_Actor.MoveToPos(m_Target))
                    {
                        m_Actor.StopMove(false);
                    }
                }
            }
        }

        protected override void OnStop()
        {
            if (m_Actor != null)
            {
                m_Actor.StopMove(false);
            }
        }

        /// <summary>
        /// 更新。
        /// </summary>
        /// <returns>动作状态。</returns>
        public override ActionState Update()
        {
            m_Actor = CoreEntry.gActorMgr.GetActorByConfigID(m_ActorID);
            if (m_Ok || m_Actor == null)
            {
                return ActionState.Stop;
            }

            m_Ok = IsMoveToEnd();
            if (m_Ok)
            {
                return ActionState.Succeed;
            }

            if (!m_Actor.AutoPathFind)
            {
                return ActionState.Failed;
            }
            
            return ActionState.Running;
        }

        public bool IsMoveToEnd()
        {
            if (ArenaMgr.Instance.IsArena)
            {
                ActorObj virtualPlayer = ArenaMgr.Instance.GetVirtualPlayer();
                if (virtualPlayer != null)
                {
                    m_Target = virtualPlayer.transform.position;
                } 
            }
            Vector3 pos = m_Actor.transform.position;
            float x = pos.x - m_Target.x;
            float z = pos.z - m_Target.z;
            bool ok = x * x + z * z <= m_ErrorRange * m_ErrorRange;
            return ok;
        }
    }
}

