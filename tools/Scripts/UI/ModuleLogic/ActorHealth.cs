/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : a
* @date     : 2014-xx-xx
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
    /// <summary>
    /// 用于角色生命相关表现。
    /// </summary>
[Hotfix]
	public class ActorHealth
    {
        /// <summary>
        /// 角色对象。
        /// </summary>
        protected ActorObj mActor;

        /// <summary>
        /// 隐藏血条计数。
        /// </summary>
        protected float mHPBarHideCount = 0;

        /// <summary>
        /// 获取角色对象。
        /// </summary>
        public ActorObj Actor
        {
            get { return mActor; }
            set { mActor = value; }
        }

        /// <summary>
        /// 获取角色类型。
        /// </summary>
        public ActorType ActorType
        {
            get { return mActor.mActorType; }
        }

        /// <summary>
        /// 获取血量进度。
        /// </summary>
        public float HPProgress
        {
            get { return CurHp * 1.0f / MaxHp; }
        }

        /// <summary>
        /// 获取血量上限。
        /// </summary>
        public long MaxHp
        {
            get { return mActor.mBaseAttr.MaxHP; }

        }

        /// <summary>
        /// 获取当前血量。
        /// </summary>
        public long CurHp
        {
            get { return mActor.mBaseAttr.CurHP; }

        }

        public ActorHealth(ActorObj actor)
        {
            mActor = actor;
        }

        public void Update(float dt)
        {
            if (mHPBarHideCount > 0)
            {
                mHPBarHideCount -= dt;
                if (mHPBarHideCount <= 0)
                {
                    mHPBarHideCount = 0;

                    //如果时被选中的对象则不隐藏血条
                    PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
                    if (player == null || player.m_SelectTargetObject != Actor)
                    {
                        OnRemoveHPBar();
                    }
                }
            }
        }

        /// <summary>
        /// 显示血条。
        /// </summary>
        /// <param name="duration">显示的持续时长。</param>
        public void ShowHPBar(float duration)
        {
            mHPBarHideCount = Mathf.Max(mHPBarHideCount, duration);
            if (mHPBarHideCount > 0)
            {
                OnCreateHPBar();
            }
            //LogMgr.Log("ShowHPBar id:{0}", Actor.ServerID);
        }

        /// <summary>
        /// 创建血条。
        /// </summary>
        public void OnCreateHPBar()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_CREATE, ep);
        }

        /// <summary>
        /// 移除血条。
        /// </summary>
        /// <param name="force">是否强制隐藏。</param>
        public void OnRemoveHPBar(bool force=false)
        {
            if (force)
            {
                mHPBarHideCount = 0;
            }
            if (mHPBarHideCount > 0)
            {
                return;
            }

            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_DESTORY, ep);
        }

        /// <summary>
        /// 重置血量。
        /// </summary>
        public void OnResetHP()
        {
            OnHPChange();
        }

        /// <summary>
        /// 角色死亡。
        /// </summary>
        public void OnDead()
        {
            OnRemoveHPBar(true);
        }

        /// <summary>
        /// 血量发生改变。
        /// </summary>
        public void OnHPChange()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_UPDATE, ep);
        }

        /// <summary>
        /// 等级发生变化。
        /// </summary>
        public void OnHPLevelChange()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_CREATE, ep);
        }

        public void OnPetLevelChange()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_PET_LEVEL_UPDATE, ep);
        }

        public void OnLordChange()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_CREATE, ep);
        }

        /// <summary>
        /// 血条绑定发生改变。
        /// </summary>
        public void OnNodeChange(GameObject go, string name)
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            ep.goParameter = go;
            ep.stringParameter = name;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_CHANGENODE, ep);
        }

        /// <summary>
        /// 更新PK状态。
        /// </summary>
        /// <param name="oldpk">原来的PK状态。</param>
        /// <param name="newpk">新的PK状态。</param>
        public void OnPKModeStatus()
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_HPBAR_PKSTATUS, ep);
        }

        /// <summary>
        /// 英雄称号改变
        /// </summary>
        public void OnHeroTitleChange(string szIcon)
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            ep.stringParameter = szIcon;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_ChangeHeroTitle, ep);
        }

        /// <summary>
        /// 阵营变化
        /// </summary>
        public void OnChangeFaction(int iCamp)
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = this;
            ep.intParameter = iCamp;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_ChangeFacion, ep);
        }
    

    }

};//End SG

