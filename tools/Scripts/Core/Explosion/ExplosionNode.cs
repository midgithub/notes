/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections;
using SG;
using System;

namespace SG
{
[Hotfix]
    public class ExplosionNode : MonoBehaviour 
    {
        public GameObject mEffectGo;
        private int mIndex;
        private Action<int, float> mEnterCallback;
        private Action<int> mExitCallback;
        private float mDuration = 0.0f;

        public void Init(int index, Action<int, float> enterCallback, Action<int> exitCallback)
        {
            mIndex = index;
            mEnterCallback = enterCallback;
            mExitCallback = exitCallback;

            if (null != mEffectGo)
            {
                mDuration = mEffectGo.GetComponent<ParticleSystem>().duration;
                mEffectGo.SetActive(false);
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (IsPositiveCollider(collider))
            {
                if (null != mEffectGo)
                {
                    mEffectGo.SetActive(true);
                }

                if (null != mEnterCallback)
                {
                    mEnterCallback(mIndex, mDuration);
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (IsPositiveCollider(collider))
            {
                if (null != mEffectGo)
                {
                    mEffectGo.SetActive(false);
                }

                if (null != mExitCallback)
                {
                    mExitCallback(mIndex);
                }
            }
        }

        private bool IsPositiveCollider(Collider collider)
        {
            ActorObj actor = collider.GetComponent<ActorObj>();
            if (null == actor)
            {
                return false;
            }

            return actor == CoreEntry.gActorMgr.MainPlayer;
        }
    }
}

