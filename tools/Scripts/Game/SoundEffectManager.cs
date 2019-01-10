using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
   
[Hotfix]
    public class SoundEffectManager
    {
        Dictionary<string, AudioSource> Record = new Dictionary<string, AudioSource>();

        private bool onlyPlayOneSample = false;

        public bool OnlyPlayOneSample
        {
            get { return onlyPlayOneSample; }
            set { onlyPlayOneSample = value; }
        }

        public void PlaySoundEffect(ActorObj actor, AudioSource audioSrc, string strClip)
        {
            if (actor != null)
            {
                //不播放自己角色中，非主角的声音
                //if (actor.mActorType != ActorType.AT_LOCAL_PLAYER)
                //{
                //    return;
                //}

                if (actor.mActorType == ActorType.AT_LOCAL_PLAYER)
                {
                    if (audioSrc != null)
                    {
                        audioSrc.volume = CoreEntry.gGameMgr.SoundVolume;
                    }
                }
                              
                PlaySoundEffect(audioSrc, strClip);
            }
            
        }

        public void PlaySoundEffect(AudioSource audioSrc, string strClip)
        {
            if (audioSrc != null)
            {
                if (onlyPlayOneSample)
                {
                    if (Record.ContainsKey(strClip))
                    {
                        AudioSource check = Record[strClip];
                        if (check != null)
                        {
                            //同名在播放的声音。不允许再播放
                            if (check.isPlaying)
                            {
                                return;
                            }
                        }
                    }

                    Record[strClip] = audioSrc;
                }

                AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(strClip, typeof(AudioClip));
                if (clip != null)
                {
                    audioSrc.clip = clip;
                    audioSrc.Play();
                }
                else
                {
                    //LogMgr.UnityLog(string.Format("load clip error! clip= {0}",strClip));
                }
            }
        }
    }
}

