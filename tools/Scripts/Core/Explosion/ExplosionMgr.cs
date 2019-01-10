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
    public class ExplosionMgr : MonoBehaviour
    {
        public AnimationCurve mAniCurve;
        public float Delay = 0.3f;
        private ExplosionNode[] mNodes;
        private CameraBase mCameraBase;
        private float mStartAniTime = 0f;
        private int mEnterIndex;
        private int mExitIndex;
        private float mDuration;

        void Awake()
        {
            int childCount = transform.childCount;
            mNodes = new ExplosionNode[childCount];
            for (int i = 0; i < childCount; i++)
            {
                Transform tran = transform.GetChild(i);
                ExplosionNode node = null;
                if (null != tran)
                {
                    node = tran.GetComponent<ExplosionNode>();
                    if (null == node)
                    {
                        node = tran.gameObject.AddComponent<ExplosionNode>();
                    }

                    if (null != node)
                    {
                        node.Init(i, EnterNode, ExitNode);
                    }
                }
                mNodes[i] = node;
            }

            mEnterIndex = -1;
            mExitIndex = -1;
        }

        void OnDisable()
        {
            CancelInvoke("EndShake");
            CancelInvoke("DoShake");
        }

        void Update()
        {
            if (null != mCameraBase && mCameraBase.m_cameraShake)
            {
                Transform tran = mCameraBase.UpdateMainCameraTransform();
                float posY = mAniCurve.Evaluate(Time.time - mStartAniTime);
                tran.position = tran.position + new Vector3(0, posY, 0);      
            }
        }

        private void EnterNode(int index, float time)
        {
            mEnterIndex = index;

            mDuration = time;
            CancelInvoke("DoShake");
            Invoke("DoShake", Delay);
        }

        private void ExitNode(int index)
        {
            mExitIndex = index;

            if (mExitIndex == mEnterIndex)
            {
                CancelInvoke("DoShake");
            }
        }

        private void DoShake()
        {
            if (null == mCameraBase && null != CoreEntry.gCameraMgr.MainCamera)
            {
                mCameraBase = CoreEntry.gCameraMgr.MainCamera.GetComponent<CameraBase>();
            }

            if (null != mCameraBase)
            {
                if (!mCameraBase.IsDisableCameraShake() && !mCameraBase.m_cameraShake)
                {
                    mCameraBase.m_cameraShake = true;
                    mStartAniTime = Time.time;

                    float t = mAniCurve[mAniCurve.length - 1].time;
                    Invoke("EndShake", t);
                }
            }

            Invoke("DoShake", mDuration);
        }

        void EndShake()
        {
            CancelInvoke("EndShake");
            mCameraBase.m_cameraShake = false;
        }
    }
}

