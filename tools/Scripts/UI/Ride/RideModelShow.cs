/**
* @file     : RideModelShow.cs
* @brief    : 用于显示坐骑模型
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-13 17:08
*/

using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class RideModelShow : MonoBehaviour
    {
        /// <summary>
        /// 显示对象预制件路径。
        /// </summary>
        public static string PrefabPath = "UI/Prefabs/Ride/FirstRes/RideModelShow";

        /// <summary>
        /// 当前模型显示对象
        /// </summary>
        public static RideModelShow CurModeShow;
        public Camera mCarmera = null;


        /// <summary>
        /// 显示坐骑。
        /// </summary>
        /// <param name="skin">坐骑编号。</param>
        public static void ShowRideModel(int skin)
        {
            if (CurModeShow == null)
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(PrefabPath, typeof(GameObject));
                if (prefab == null)
                {
                    return;
                }

                GameObject obj = Instantiate(prefab) as GameObject;
                CurModeShow = obj.GetComponent<RideModelShow>();
                DontDestroyOnLoad(CurModeShow);
                CurModeShow.transform.localPosition = new Vector3(0, -1000, 0);
                CurModeShow.transform.localScale = Vector3.one;
                CurModeShow.transform.forward = Vector3.forward;
                CurModeShow.mCarmera = obj.GetComponentInChildren<Camera>();

            }

            if (!CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(true);
            }
            CurModeShow.ShowModel(skin);
        }

        /// <summary>
        /// 隐藏坐骑模型。
        /// </summary>
        public static void HideRideModel()
        {
            if (CurModeShow == null)
            {
                return;
            }

            if (CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 旋转模型。
        /// </summary>
        /// <param name="r"></param>
        public static void Rotate(Vector3 r)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }
            CurModeShow.ModelParent.Rotate(r);
        }
        /// <summary>
        /// 当前坐骑ID。
        /// </summary>
        private int mCurID;

        /// <summary>
        /// 坐骑对象。
        /// </summary>
        private GameObject mRide;

        /// <summary>
        /// 模型挂接点。
        /// </summary>
        public Transform ModelParent;

        public bool bFinishReturn = false;
        public string mStandActionName = "stand";       //待机
        public string mIdleActionName = "idle";        //思考
        public string mEnterActionName = "enter001";   //出场动画

        // Use this for initialization
        void Start()
        {

        }
        public void Update()
        {
            if (this == null)
            {
                return;
            }
            if (bFinishReturn && !string.IsNullOrEmpty(mStandActionName))
            {
                Animation mAnimator = mRide.GetComponent<Animation>();
                if (!mAnimator.isPlaying)
                {
                    mAnimator.Play(mStandActionName);
                    bFinishReturn = false;
                }
            }
        }

        /// <summary>
        /// 显示模型。
        /// </summary>
        /// <param name="skin">坐骑编号。</param>
        public void ShowModel(int skin)
        {
            Animation mAnimator = null;
            if (mCurID == skin)
            {
                mAnimator = mRide.GetComponent<Animation>();
                if (null != mAnimator)
                {
                    //mAnimator.Play(mIdleActionName);
                    bFinishReturn = true;
                }
                return;
            }

            string path = ConfigManager.Instance.Ride.GetRideSkin(skin,true);
            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
            if (prefab == null)
            {
                LogMgr.ErrorLog("加载坐骑失败 skin:{0} path:{1}", skin, path);
                return;
            }

            mCurID = skin;
            if (mRide != null)
            {
                Destroy(mRide);
            }
            mRide = Instantiate(prefab) as GameObject;
            mRide.transform.SetParent(ModelParent);
            mRide.transform.localPosition = Vector3.zero;
            mRide.transform.localScale = Vector3.one;
            mRide.transform.forward = ModelParent.forward;
            mAnimator = mRide.GetComponent<Animation>();
            SetCfgPosition();
          //  if (null != mAnimator)
          //   {
          //      Debug.LogError("ID不相同");
          //      mAnimator.Play(mEnterActionName);
          //       bFinishReturn = true;
          //   }
        }

        /// <summary>
        /// 设置相机下 模型挂点的Transform坐标(读取模型表 坐标配置)
        /// </summary>
        /// <param name="modelId"></param>
        public void SetCfgPosition()
        {
            if (CurModeShow == null)
            {
                return;
            }
            int _modelId = ConfigManager.Instance.Ride.GetModelID(mCurID);
            LuaTable  mModelCfg = ConfigManager.Instance.Common.GetModelConfig(_modelId);
            float posX = mModelCfg.Get<float>("posX");
            float posY = mModelCfg.Get<float>("posY");
            float posZ = mModelCfg.Get<float>("posZ");
            float rotateX = mModelCfg.Get<float>("rotateX");
            float rotateY = mModelCfg.Get<float>("rotateY");
            float rotateZ = mModelCfg.Get<float>("rotateZ");

            ModelParent.transform.localPosition = new Vector3(posX, posY, posZ);
            ModelParent.transform.localEulerAngles = new Vector3(rotateX, rotateY, rotateZ);
            ModelParent.transform.localScale = Vector3.one;
        }


        /// <summary>
        /// 播放enter动画
        /// </summary>
        public static void ShowEnterAnimation()
        {
            if (CurModeShow == null)
            {
                return;
            }
            GameObject obj = CurModeShow.GetHorseModel();
            if (obj == null)
            {
                return;
            }
            Animation mAnimator = obj.GetComponent<Animation>();
            mAnimator.Play(CurModeShow.mEnterActionName);
            CurModeShow.bFinishReturn = true;
        }
        /// <summary>
        /// 播放idle动画
        /// </summary>
        public static void ShowHorseActionAnimation()
        {
            if (CurModeShow == null)
            {
                return;
            }
            GameObject obj = CurModeShow.GetHorseModel();
            if (obj == null)
            {
                return;
            }
            Animation mAnimator = obj.GetComponent<Animation>();
            if (!mAnimator.IsPlaying(CurModeShow.mEnterActionName) && null != mAnimator.GetClip(CurModeShow.mIdleActionName))
            {
                mAnimator.Play(CurModeShow.mIdleActionName);
                CurModeShow.bFinishReturn = true;
            }
        }
        public GameObject GetHorseModel()
        {
            if (mRide != null)
            {
                return mRide;
            }
            return null;
        }

    }
}

