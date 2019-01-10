using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class ActorAnimation : MonoBehaviour {

    /// <summary>
    /// 动作的序号
    /// </summary>
    public float Action = 0;
    int lastAction = -1;

    public List<AnimationState> list = new List<AnimationState>();
    [HideInInspector]
    public List<float> timeList = new List<float>();
    Animation anim = null;

    /// <summary>
    /// 是否停桢
    /// </summary>
    public bool IsStop = false;
    bool lastStop = false;

    /// <summary>
    /// 当播完一次性动作后，关闭该组件
    /// </summary>
    public bool DisableSelfWhenOnceActionFinished = false;
  
    /// <summary>
    /// 从当前播放的动作上，获取动作序号
    /// </summary>
    public void UpdateToCurrentAnImation()
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (anim.IsPlaying(list[i].name))
            {
                Action = i;
                break;
            }
        }
    }

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim != null)
        {
            foreach (AnimationState aniState in anim)
            {
                list.Add(aniState);
                timeList.Add(0);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (anim != null)
        {
            if (IsStop != lastStop)
            {
                //定桢逻辑
                if (IsStop)
                {
                    //暂停动作
                    CancelInvoke("PlayDefaultAnimation");
                    for (int i = 0; i < list.Count; ++i)
                    {
                        timeList[i] = list[i].time;
                       
                    }
                    anim.Stop();
                }
                else
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        list[i].time = timeList[i];
                    }

                    //暂停后继续播放
                    int index = (int)Action;
             
                    ProcessOnceAction(index, list[index].time);
                    anim.Play(list[index].name);
                }

                lastStop = IsStop;
            }

            //正常播放动作
            if (IsStop == false)
            {
                int index = (int)Action;
                if (lastAction != index)
                {
                    lastAction = index;

                    ProcessOnceAction(index, 0);

                    anim.Play(list[index].name);
                }
            }
           
        }
	}

    void ProcessOnceAction(int index, float invokeOffsetTime)
    {
        AnimationClip clip = list[index].clip;
        CancelInvoke("PlayDefaultAnimation");
        //如果不是循环播放动作，在最后接默认动作
        if (clip.wrapMode != WrapMode.Loop || clip.wrapMode != WrapMode.PingPong)
        {
            Invoke("PlayDefaultAnimation", clip.length - invokeOffsetTime);
        }
    }

    void PlayDefaultAnimation()
    {
        CancelInvoke("PlayDefaultAnimation");
        if (DisableSelfWhenOnceActionFinished)
        {
            enabled = false;
        }
        else
        {
            anim.Play();
            //anim.CrossFade(anim.clip.name);
        }
    }
}

