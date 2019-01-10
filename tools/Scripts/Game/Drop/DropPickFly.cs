/**
* @file     : DropPickFly
* @brief    : 拾取特效
* @details  : 拾取特效
* @author   : CW
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class DropPickFly : MonoBehaviour
    {
        private Vector3 mStartPos;
        private GameObjectPool mPickObjectPool = null;

        //private float mYOffset = 3.0f;
        //private float mYMax;
        //private float mYSpeed1;
        //private GameObject mToObj;
        //private float startTime;

        public void Init(Vector3 from, GameObject go, GameObjectPool pickPool = null)
        {
            mStartPos = from;
            //mToObj = go;
            mPickObjectPool = pickPool;

            //Vector3 goPos = mToObj.transform.position;
            //mYMax = goPos.y + mYOffset;
            //mYSpeed1 = (mYMax - mStartPos.y) * 1.25f;
            //startTime = Time.realtimeSinceStartup;
        }

        private Ray mRay;
        private Vector3 mEndPos;
        private float mDistance;
        private float mTimeParam;
        public void Init(Vector3 from, Vector3 to, GameObjectPool pickPool = null)
        {
            mStartPos = from;
            mPickObjectPool = pickPool;

            transform.position = mStartPos;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            if (null != CoreEntry.gCameraMgr.MainCamera)
            {
                mDistance = Vector3.Distance(mStartPos, CoreEntry.gCameraMgr.MainCamera.transform.position);
                Vector3 screenPos = MainPanelMgr.Instance.uiCamera.WorldToScreenPoint(to);
                mRay = CoreEntry.gCameraMgr.MainCamera.ScreenPointToRay(screenPos);
                mTimeParam = 3.0f;
            }
        }

        void Update()
        {
            if (null != CoreEntry.gCameraMgr.MainCamera)
            {
                mEndPos = CoreEntry.gCameraMgr.MainCamera.transform.position + (mRay.direction).normalized * mDistance;
            }
            Vector3 dir = mEndPos - transform.position;
            float dis = dir.magnitude;

            if (dis > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, mEndPos, Time.deltaTime * mTimeParam);
                mTimeParam += 0.05f;
            }
            else
            {
                if (null != mPickObjectPool)
                {
                    mPickObjectPool.RecyclePrefabInstance(gameObject);
                }
            }
        }

        /*
        // Update is called once per frame
        void Update()
        {
            float t = Time.realtimeSinceStartup - startTime;
            Vector3 pos;
            if (t < .8f)
            {
                pos = new Vector3(mStartPos.x, mStartPos.y + t * mYSpeed1, mStartPos.z);
                transform.position = pos;
            }
            else if (t < 2.0f)
            {
                t -= .8f;
                Vector3 xz = new Vector3((mToObj.transform.position.x - transform.position.x) / 1.2f, (mToObj.transform.position.y - transform.position.y) / 1.2f, (mToObj.transform.position.z - transform.position.z) / 1.2f);
                pos = new Vector3(xz.x * t + transform.position.x, transform.position.y + xz.y * t, xz.z * t + transform.position.z);

                transform.position = pos;
                float dist = Vector3.Distance(mToObj.transform.position, pos);
                if (dist < 0.1f)
                {
                    if (null != mPickObjectPool)
                    {
                        mPickObjectPool.RecyclePrefabInstance(gameObject);
                    }
                }
            }
            else
            {
                if (null != mPickObjectPool)
                {
                    mPickObjectPool.RecyclePrefabInstance(gameObject);
                }
            }
        }
         * */
    }
}

