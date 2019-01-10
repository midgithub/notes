using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class ImbededAnimation 
    {
        /// <summary>
        /// 内部类，回调接口
        /// </summary>
[Hotfix]
        class ImbededAnimationFinishedProcessor : MonoBehaviour
        {
            private object refObj = null;

            /// <summary>
            /// 回调物体引用
            /// </summary>
            public object RefObj
            {
                get { return refObj; }
                set { refObj = value; }
            }

            public void ProcessWhenAnimationFinished()
            {
                ImbededAnimation obj = RefObj as ImbededAnimation;
                if (obj != null)
                {
                    obj.CallFinishHandler();
                }
            }

            public void ProcessEventOfAnimation(int key)
            {
                 ImbededAnimation obj = RefObj as ImbededAnimation;
                 if (obj != null)
                 {
                     obj.CallEventHandler(key);
                 }
            }
        };

        /// <summary>
        /// 结束事件
        /// </summary>
        AnimationEvent finishedEvent = null;
        public float Length
        {
            get
            {
                if (finishedEvent != null)
                {
                    return finishedEvent.time;
                }

                return -1f;
            }
        }

        public delegate void ProcessWhenAnimationFinished(ImbededAnimation imbededAnimation, AnimationEventFromScript animationEventFromScript);

        ProcessWhenAnimationFinished finishedHandler = null;

        /// <summary>
        /// 调结束回调
        /// </summary>
        public void CallFinishHandler()
        {
            if (finishedHandler != null)
            {
                finishedHandler(this,eventFromScript);
            }
        }

        #region EventOfAnimation
        public delegate void ProcessEventOfAnimation(AnimationEvent evt,ImbededAnimation imbededAnimation, AnimationEventFromScript animationEventFromScript);

        public struct AnimationEventInfo
        {
            public AnimationEvent Evt{get;set;}
            public ProcessEventOfAnimation Handler{get;set;}
        };
       
        Dictionary<int, AnimationEventInfo> EventInfoList = new Dictionary<int, AnimationEventInfo>();
        int EventCounter = 0;

        public AnimationEventInfo GetEventInfo(int key)
        {
            AnimationEventInfo info = new AnimationEventInfo();

            if (EventInfoList.ContainsKey(key))
            {
                info = EventInfoList[key];
            }

            return info;
        }

        /// <summary>
        /// 调用非结束回调
        /// </summary>
        /// <param name="key"></param>
        public void CallEventHandler(int key)
        {
              AnimationEventInfo info = GetEventInfo(key);
              if (info.Evt != null && info.Handler != null)
              {
                  info.Handler(info.Evt, this, eventFromScript);
              }
        }
        #endregion

        /// <summary>
        /// prefab实体
        /// </summary>
        GameObject gameObj = null;

        public GameObject GameObj
        {
            get { return gameObj; }
        }

        /// <summary>
        /// 脚本事件安装器
        /// </summary>
        AnimationEventFromScript eventFromScript = null;

        public AnimationEventFromScript EventFromScript
        {
            get { return eventFromScript; }
        }

      
      
        public ImbededAnimation(string prefabPath, ProcessWhenAnimationFinished handler)
        {
            Load(prefabPath, handler);
        }

        public ImbededAnimation()
        {

        }

        /// <summary>
        /// 清空animation资源
        /// </summary>
        public void Unload()
        {
            if (gameObj != null)
            {
                Object.Destroy(gameObj);
            }
        }

        /// <summary>
        /// 加载animation资源
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public AnimationEventFromScript Load(string prefabPath, ProcessWhenAnimationFinished handler)
        {
            //设置名称
           // name = prefabPath;
            //设置回调
            finishedHandler = handler;

            Object orgObj = CoreEntry.gResLoader.Load(prefabPath);
            if (orgObj != null)
            {
                gameObj = GameObject.Instantiate(orgObj) as GameObject;
                eventFromScript = gameObj.AddComponent<AnimationEventFromScript>();
                finishedEvent = eventFromScript.AddAnimationFinishedEvent<ImbededAnimationFinishedProcessor>("ProcessWhenAnimationFinished");

                ImbededAnimationFinishedProcessor tmp = eventFromScript.GetComponent<ImbededAnimationFinishedProcessor>();
                if (tmp != null)
                {
                    tmp.RefObj = this;
                }


                orgObj = null;
                //Resources.UnloadUnusedAssets();
            }
            else
            {
                LogMgr.ErrorLog("ImbededAnimation: Can not find animation prefab: {0}.", prefabPath);
            }
            return eventFromScript;
        }

        /// <summary>
        /// 增加一个非结束对话
        /// </summary>
        /// <param name="time"></param>
        /// <param name="handler"></param>
        public void AddEvent(float time,ProcessEventOfAnimation handler)
        {
            if (eventFromScript != null)
            {
                AnimationEventInfo info = new AnimationEventInfo();
                AnimationEvent evt = eventFromScript.AddAnimationEvent<ImbededAnimationFinishedProcessor>("ProcessEventOfAnimation", time);
                if (evt != null)
                {
                    info.Evt = evt;
                    info.Handler = handler;

                    int key = EventCounter++;
                    EventInfoList.Add(key, info);

                    evt.intParameter = key;
                }
            }
            
        }
    }
}

