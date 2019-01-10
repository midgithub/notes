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

namespace SG
{
[Hotfix]
    public class HitFlyMotionBase : MonoBehaviour
    {
        private Transform mTransform;
        private float mDuration;
        private Vector3 mStartPos;
        private Vector3 mTargetPos;
        private bool mIsStopMove = true;
        private float mStartTime;
        private Vector3 mDeltaPos;

        void Awake()
        {
            mTransform = transform;

        }

        public void StartMove(float time, float distance, ActorObj attacker)
        {
            mDuration = time;
            mStartPos = mTransform.position;
            Vector3 dir;
            if (null != attacker)
            {
                dir = (attacker.transform.position - mTransform.position).normalized;
            }
            else
            {
                dir = mTransform.TransformDirection(Vector3.forward);
            }
            Vector3 maxPos = mStartPos - dir * distance;
            mTargetPos = CoreEntry.gBaseTool.GetLineReachablePos(mStartPos, maxPos);
            mDeltaPos = (mTargetPos - mStartPos) / mDuration;
            mStartTime = Time.time;
            mIsStopMove = false;
        }

        void Update()
        {
            if(mIsStopMove)
            {
                return;
            }

            float diffTime = Time.time - mStartTime;
            if (diffTime >= mDuration)
            {
                mTransform.position = mTargetPos;
                mIsStopMove = true;

                return;
            }

            Vector3 pos = mStartPos + mDeltaPos * diffTime;

            mTransform.position = pos;
        }


    }
}

