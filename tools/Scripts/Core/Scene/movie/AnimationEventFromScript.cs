using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class AnimationEventFromScript : Common_Base{

        Animator animator = null;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public AnimationEvent AddAnimationEvent<T>(string functionName, float time) where T : MonoBehaviour
        {
            AnimationEvent evt = null;
            if (animator != null)
            {
                 GetARequiredComponent<T>(gameObject);

                 evt = new AnimationEvent();

                 //更新动画，以便可以读取正确的info
                 animator.Update(0);
                 AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                 if (info.length > 0)
                 {
                     AddAnEvent(evt, functionName, time);
                 }
                 else
                 {
                     StartCoroutine(AddAnEventRoutine(evt, functionName, time));
                 }
            }

            return evt;
        }

        /// <summary>
        /// 在动画最后一帧插入Event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public AnimationEvent AddAnimationFinishedEvent<T>(string functionName) where T : MonoBehaviour
        {
            AnimationEvent evt = null;
            if (animator != null)
            {
                GetARequiredComponent<T>(gameObject);
                evt = new AnimationEvent();

                //更新动画，以便可以读取正确的info
                animator.Update(0);
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                if (info.length > 0)
                {
                    //在动画最后动态添加事件，触发对话
                    AddAnEvent(evt,functionName, info.length);
                }
                else
                {
                    StartCoroutine(AddFinishEventForAnimation(evt, functionName));
                }
            }

            return evt;
        }

        protected IEnumerator AddFinishEventForAnimation(AnimationEvent evt, string functionName)
        {
            yield return new WaitForEndOfFrame();
            if (animator != null)
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                //在动画最后动态添加事件，触发对话
                AddAnEvent(evt, functionName, info.length);
            }
        }

        protected IEnumerator AddAnEventRoutine(AnimationEvent evt, string functionName, float time)
        {
            yield return new WaitForEndOfFrame();
             if (animator != null)
             {
                 AddAnEvent(evt, functionName, time);
             }
        }

        protected AnimationClip mainClip = null;

        protected void AddAnEvent(AnimationEvent evt,string functionName, float time)
        {
            if (animator != null)
            {
                if (evt != null)
                {
                    evt.time = time;
                    evt.functionName = functionName;
                    evt.messageOptions = SendMessageOptions.DontRequireReceiver;
                    if (mainClip == null)
                    {
                        //第二次就读不到，可能是unity4.6的bug
                        AnimatorClipInfo[] anims = animator.GetCurrentAnimatorClipInfo(0);
                        if (anims.Length > 0)
                        {
                            mainClip = anims[0].clip;
                        }
                    }

                    if (mainClip != null)
                    {
                        mainClip.AddEvent(evt);
                    }
                }
            }
         
        }

     
    }
}

