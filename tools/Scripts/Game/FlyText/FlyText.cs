/**
* @file     : FlyText.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

namespace SG
{
[Hotfix]
    public class FlyText : MonoBehaviour
    {
        public FlyTextCurve AniCurve;

        public Text ShowText;
        public float AniStart;
        public FlyTextType FlyType;
        public Vector2 StartPosition;
        public Vector3 BindPosition;
        public bool Inverse;

        private Vector3 mOffset;        //随机偏移
        private GameObject mOwner;

        private Transform mBindTran;

        public void Init(GameObject owner, string text, FlyTextType type, int isGood)
        {
            FlyType = type;

            ShowText.text = text;
            mOwner = owner;

            Inverse = false;
            InitBindPosition();
            GetRandomPosition();
        }

        private void InitBindPosition()
        {
            mBindTran = null;

            if (null == mOwner)
            {
                return;
            }
            ActorObj actor = mOwner.GetComponent<ActorObj>();
            if (null == actor)
            {
                return;
            }
            //Configs.modelConfig modelCfg = CSVConfigManager.GetmodelConfig(actor.resid);
            LuaTable modelCfg = ConfigManager.Instance.Common.GetModelConfig(actor.resid);
            if (null == modelCfg)
            {
                return;
            }
            
            mBindTran = mOwner.transform.FindChild(modelCfg.Get<string>("beHitPoint"));
            if(null == mBindTran)
            {
                mBindTran = mOwner.transform.FindChild("E_top");
            }

            BindPosition = Vector3.zero;
            if (null != mBindTran)
            {
                BindPosition = mBindTran.position;
            }
            UpdateBindPosition();

            if (actor != CoreEntry.gActorMgr.MainPlayer)
            {
                ActorObj mainObj = CoreEntry.gActorMgr.MainPlayer;
                Camera main = CoreEntry.gCameraMgr.MainCamera;
                if (null != main && null != mainObj)
                {
                    Vector3 ownerPos = main.WorldToScreenPoint(mOwner.transform.position);
                    Vector3 mainPos = main.WorldToScreenPoint(mainObj.transform.position);

                    Inverse = ownerPos.x < mainPos.x;
                }
            }
        }

        public void UpdateBindPosition()
        {
            Vector3 pos = Vector3.zero;

            Camera uicamera = MainPanelMgr.Instance.uiCamera;
            if (null != uicamera && null != CoreEntry.gCameraMgr.MainCamera)
            {
                pos = CoreEntry.gCameraMgr.MainCamera.WorldToScreenPoint(BindPosition);
                pos = uicamera.ScreenToWorldPoint(pos);
            }

            transform.position = pos;
        }

        private void GetRandomPosition()
        {
            int x = Random.Range(0, 10) - 10;
            int y = Random.Range(0, 20) - 10;

            StartPosition = new Vector2(x * 1.0f, y * 1.0f);
        }

        public void GetAnimationPos(float time, ref float x, ref float y)
        {
            x = AniCurve.PosXCurve.Evaluate(time) * 100;
            y = AniCurve.PosYCurve.Evaluate(time) * 100;
        }

        public float GetAnimationScale(float time)
        {
            float scale = 1.0f;
            if (null != AniCurve.ScaleCurve)
            {
                scale = AniCurve.ScaleCurve.Evaluate(time);
            }

            return scale;
        }

        public float GetAnimationAlpha(float time)
        {
            float alpha = 1.0f;
            if (null != AniCurve.AlphaCurve)
            {
                alpha = AniCurve.AlphaCurve.Evaluate(time);
            }

            return alpha;
        }

        public float GetAnimationTime()
        {
            return AniCurve.GetAnimationTime();
        }
    }
}

