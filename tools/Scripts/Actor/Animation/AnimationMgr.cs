using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    //玩家组件
[Hotfix]
    public class AnimationMgr 
    {
        private Animation m_Animation = null;

        //
        private ActorObj m_AnimationActor;

        public ActorObj AnimationActor
        {
            get { return m_AnimationActor; }
            protected set { m_AnimationActor = value; }
        }


        public string m_curActionName = null;


        /// <summary>
        /// 是否是上下步结构
        /// </summary>
        protected bool isUpperAndBasePart = false;

        public bool IsUpperAndBasePart
        {
            get { return isUpperAndBasePart; }
            protected set { isUpperAndBasePart = value; }
        }

        protected Transform upperPart = null;

        public Transform UpperPart
        {
            get { return upperPart; }
            protected set { upperPart = value; }
        }

        protected Transform basePart = null;

        public Transform BasePart
        {
            get { return basePart; }
            protected set { basePart = value; }
        }

        public LineRenderer LineRender = null;



        public void Init(ActorObj actor)
        {
            m_AnimationActor = actor;

            if (m_AnimationActor != null)
            {
                m_Animation = m_AnimationActor.GetComponent<Animation>();
            }

        }


        //下面是一些组件接口

        //1,播放动画
        public virtual void PlayAction(string strAction, bool isCrossFade = true, ActionVoiceDesc voiceDesc = null)
        {

            if (!IsHadAction(strAction))
            {
                return;
            }

            m_curActionName = strAction;

            if (isCrossFade)
            {
                if (IsUpperAndBasePart)
                {
                    UpperPart.GetComponent<Animation>().CrossFade(strAction, 0.1f);
                    if (BasePart != null)
                    {
                        if (IsHadAction(strAction, BasePart.GetComponent<Animation>()))
                        {
                            BasePart.GetComponent<Animation>().CrossFade(strAction, 0.1f);
                        }
                    }
                }
                else
                {
                    m_Animation.CrossFade(strAction, 0.1f);
                }
            }
            else
            {
                if (IsUpperAndBasePart)
                {
                    UpperPart.GetComponent<Animation>().Play(strAction);
                    if (BasePart != null)
                    {
                        if (IsHadAction(strAction, BasePart.GetComponent<Animation>()))
                        {
                            BasePart.GetComponent<Animation>().Play(strAction);
                        }
                    }
                }
                else
                {
                    m_Animation.Play(strAction);
                }
            }

          
        }

        public void StopAction(string strAction)
        {
            m_Animation.Stop(strAction);
        }

        public  void StopAll()
        {
            m_Animation.Stop();
        }

        public  bool IsPlayingAction(string strAction)
        {
            return m_Animation.IsPlaying(strAction);
        }

        public  float GetActionLength(string strAction)
        {
            if (strAction == null || strAction.Length == 0)
            {
                return 0.0f;
            }
            if (m_Animation[strAction] == null)
            {
                LogMgr.UnityLog("GetActionLength:" + strAction);
                return 0.0f;
            }

            return m_Animation[strAction].length;
        }

        public  void SetActionSpeed(string strAction, float speed)
        {
            if (strAction == null || strAction.Length <= 0)
            {
                return;
            }

            if (m_Animation[strAction] == null)
            {
                LogMgr.UnityLog("SetActionSpeed:" + strAction);
                return;
            }

            m_Animation[strAction].speed = speed;
        }

        public  void SetSkillActionSpeed(string strAction, float speed, LuaTable skillDesc)
        {
            float scale = 1f;
            if (m_AnimationActor != null && skillDesc != null && skillDesc.Get<int>("showtype") == 2 && m_AnimationActor.mActorType == ActorType.AT_BOSS)
            {
                scale = m_AnimationActor.NormalAttackSpeedScale;
            }


            SetActionSpeed(strAction, speed * scale);
        }

        public  void SetActionTime(string strAction, float timeValue)
        {
            if (strAction == null || strAction.Length <= 0)
            {
                return;
            }

            if (m_Animation[strAction] == null)
            {
                LogMgr.UnityLog("SetActionTime:" + strAction);
                return;
            }

            m_Animation[strAction].time = timeValue;


        }

        //获取动作剩余时间
        public float GetLeftActionTime(string strAction)
        {
            return m_Animation[strAction].length - m_Animation[strAction].time;
        }

        //获取当前动作播放的时间
        public float GetCurActionTime(string strAction)
        {
            if (strAction.Length == 0)
            {
                return 0;
            }

            return m_Animation[strAction].time;
        }

        //是否存在action
        public  bool IsHadAction(string strAction)
        {
            AnimationState state = null;
            if (IsUpperAndBasePart == false)
            {
                if (null != m_Animation)
                {
                    state = m_Animation[strAction];
                }
            }
            else
            {
                Animation an = UpperPart.GetComponent<Animation>();
                if (null != an)
                {
                    state = an[strAction];
                }
            }

            return state != null;
        }

        public  bool IsHadAction(string strAction, Animation anim)
        {
            //foreach (AnimationState state in anim)
            //{
            //    if (state.clip.name.Equals(strAction))
            //    {
            //        return true;
            //    }
            //}


            AnimationState state = anim[strAction];
            return state != null;
        }

        //获取当前播放的动作
        public string GetCurPlayAction()
        {
            return m_curActionName;

            //string curAction = "atstand001";

            ////按层级最高，且权重最大
            //foreach (AnimationState state in m_Animation)
            //{
            //    if (state.enabled)
            //    {
            //        LogMgr.UnityLog("state.layer=" + state.layer + ", state.name=" + state.name + ", weight=" + state.weight);

            //        if (state.layer > m_Animation[curAction].layer)
            //        {
            //            curAction = state.name;
            //        }
            //        else if (state.layer == m_Animation[curAction].layer)
            //        {
            //            if (state.weight >= m_Animation[curAction].weight)
            //            {
            //                curAction = state.name;
            //            }
            //        }
            //    }
            //}

            //LogMgr.UnityLog("curAction=" + curAction);
            //return curAction;
        }



    }
};  //end SG

