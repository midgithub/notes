/**
* @file     : OtherPlayerModelShow.cs
* @brief    : 用于显示其它玩家的模型
* @details  : 
* @author   : XuXiang
* @date     : 2017-09-19 19:58
*/

using UnityEngine;
using XLua;

namespace SG
{
    [LuaCallCSharp]

[Hotfix]
    public class OtherPlayerModelShow : MonoBehaviour
    {

        /// <summary>
        /// 显示对象预制件路径。
        /// </summary>
        public static string PrefabPath = "UI/Prefabs/RoleInfo/OtherPlayerModelShow";

        /// <summary>
        /// 当前模型显示对象
        /// </summary>
        private static OtherPlayerModelShow CurModeShow;

        /// <summary>
        /// 默认旋转。
        /// </summary>
        private static Quaternion DefaultRotate;

        /// <summary>
        /// 显示玩家模型。
        /// </summary>
        public static void ShowOtherPlayerModel()
        {
            if (CurModeShow == null)
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(PrefabPath, typeof(GameObject));
                if (prefab == null)
                {
                    return;
                }

                GameObject obj = Instantiate(prefab) as GameObject;
                CurModeShow = obj.GetComponent<OtherPlayerModelShow>();
                DontDestroyOnLoad(CurModeShow);
                CurModeShow.transform.localPosition = new Vector3(0, -1000, 0);
                CurModeShow.transform.localScale = Vector3.one;
                //CurModeShow.transform.forward = Vector3.forward;
                DefaultRotate = CurModeShow.ModelParent.rotation;
            }

            if (!CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(true);
                CurModeShow.ModelParent.rotation = DefaultRotate;
            }
            CurModeShow.ShowModel();
        }

        /// <summary>
        /// 隐藏坐骑模型。
        /// </summary>
        public static void HideModel()
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
        /// 模型挂接点。
        /// </summary>
        public Transform ModelParent;

        /// <summary>
        /// 玩家对象。
        /// </summary>
        private GameObject mPlayerObject;

        /// <summary>
        /// 显示模型。
        /// </summary>
        public void ShowModel()
        {
            if (mPlayerObject != null)
            {
                Destroy(mPlayerObject);
                mPlayerObject = null;
            }

            //角色模型
            int dress = OtherPlayerData.Instance.GetDressModelID();
            if (dress == 0)
            {
                return;
            }

            mPlayerObject = SceneLoader.LoadModelObject(dress);
            if (mPlayerObject == null)
            {
                LogMgr.WarningLog("加载衣服id:{0}失败", dress);
                return;
            }

            mPlayerObject.transform.SetParent(ModelParent);
            mPlayerObject.transform.localPosition = Vector3.zero;
            mPlayerObject.transform.localScale = Vector3.one;
            mPlayerObject.transform.forward = ModelParent.forward;
            Transform mRightHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_R_Hand");
            Transform mLeftHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_L_Hand");

            //物品
            int weapon = OtherPlayerData.Instance.GetWeaponModelID();
            if (weapon != 0)
            {
                GameObject obj = SceneLoader.LoadModelObject(weapon);
                if (obj != null)
                {
                    if (OtherPlayerData.Instance.Prof == 4)
                    {
                        GameObject mRightWeapon = obj.transform.Find("DM_R_wuqi01").gameObject;
                        mRightWeapon.SetActive(true);
                        mRightWeapon.transform.SetParent(mRightHand);
                        mRightWeapon.transform.localPosition = Vector3.zero;
                        mRightWeapon.transform.localScale = Vector3.one;
                        mRightWeapon.transform.localRotation = Quaternion.identity;

                        GameObject mLeftWeapon = obj.transform.Find("DM_L_wuqi01").gameObject;
                        mLeftWeapon.SetActive(true);
                        mLeftWeapon.transform.SetParent(mLeftHand);
                        mLeftWeapon.transform.localPosition = Vector3.zero;
                        mLeftWeapon.transform.localScale = Vector3.one;
                        mLeftWeapon.transform.localRotation = Quaternion.identity;
                        Destroy(obj);       //移除空对象
                    }
                    else
                    {
                        GameObject mRightWeapon = obj;
                        mRightWeapon.SetActive(true);
                        mRightWeapon.transform.SetParent(mRightHand);
                        mRightWeapon.transform.localPosition = Vector3.zero;
                        mRightWeapon.transform.localScale = Vector3.one;
                        mRightWeapon.transform.localRotation = Quaternion.identity;
                    }
                }
                else
                {
                    LogMgr.WarningLog("加载武器id:{0}失败", weapon);
                }
            }

            //翅膀
            int wing = OtherPlayerData.Instance.GetWingModelID();
            if (wing != 0)
            {
                Transform mBackHolder = ActorObj.RecursiveFindChild(mPlayerObject.transform, "E_back");
                GameObject obj = SceneLoader.LoadModelObject(wing);
                if (obj != null)
                {
                    GameObject mWing = obj;
                    mWing.transform.parent = mBackHolder;
                    mWing.transform.localPosition = Vector3.zero;
                    mWing.transform.localRotation = Quaternion.identity;
                    mWing.SetActive(true);
                }
                else
                {
                    LogMgr.WarningLog("加载翅膀id:{0}失败", wing);
                }
            }
            
            mPlayerObject.GetComponent<Animation>().Play("stand");
        }
    }
}

