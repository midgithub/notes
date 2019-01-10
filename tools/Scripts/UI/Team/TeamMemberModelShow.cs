/**
* @file     : TeamMemberModelShow.cs
* @brief    : 队伍成员模型展示
* @details  : 
* @author   : XuXiang
* @date     : 2017-11-23 11:13
*/

using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class TeamMemberModelShow : MonoBehaviour
    {
        /// <summary>
        /// 模型挂接点。
        /// </summary>
        public Transform ModelParent;

        /// <summary>
        /// 玩家对象。
        /// </summary>
        private GameObject mPlayerObject;

        /// <summary>
        /// 当前穿着模型。
        /// </summary>
        private int m_CurDress = 0;

        /// <summary>
        /// 玩家右手，用于绑定武器。
        /// </summary>
        private Transform mRightHand;

        /// <summary>
        /// 左手挂接点
        /// </summary>
        private Transform mLeftHand;

        /// <summary>
        /// 左手武器。
        /// </summary>
        private GameObject mLeftWeapon;

        /// <summary>
        /// 右手武器。
        /// </summary>
        private GameObject mRightWeapon;

        /// <summary>
        /// 当前武器编号。
        /// </summary>
        private int mCurWeaponID;

        /// <summary>
        /// 当前翅膀对象。
        /// </summary>
        private GameObject mWing;

        /// <summary>
        /// 角色背部挂点。
        /// </summary>
        private Transform mBackHolder;

        /// <summary>
        /// 当前翅膀对应的模型id
        /// </summary>
        private int mCurWing = 0;

        /// <summary>
        /// 初始化信息。
        /// </summary>
        /// <param name="role">角色编号。</param>
        public void Init(MsgData_sTeamRole role)
        {
            int dress = SceneLoader.GetClothesModelID(role.FashionsDress, role.Dress, role.Prof,0);
            int weapon = SceneLoader.GetWeaponModelID(role.FashionsArms, 0, role.Arms, role.Prof);
            int wing = SceneLoader.GetWingModelID(role.FashionsHead, role.Wing, role.Prof);
            ShowDress(dress);
            ShowWeapon(weapon, role.Prof);
            ShowWing(wing);
            if (mPlayerObject != null)
            {
                mPlayerObject.GetComponent<Animation>().Play("stand_UI");
            }            
        }

        public void ShowDress(int dress)
        {
            if (m_CurDress == dress)
            {
                return;
            }

            //清除武器和翅膀，武器是挂在人身上，不用手动Destroy
            mCurWeaponID = 0;
            mLeftWeapon = null;
            mRightWeapon = null;
            mCurWing = 0;
            mWing = null;

            //清除模型
            Destroy(mPlayerObject);
            mPlayerObject = null;
            mRightHand = null;
            mLeftHand = null;
            mBackHolder = null;

            //加载新模型
            m_CurDress = dress;
            if (m_CurDress == 0)
            {
                return;
            }

            mPlayerObject = SceneLoader.LoadModelObject(m_CurDress);
            if (mPlayerObject == null)
            {
                LogMgr.WarningLog("加载衣服id:{0}失败", m_CurDress);
                return;
            }

            mPlayerObject.transform.SetParent(ModelParent);
            mPlayerObject.transform.localPosition = Vector3.zero;
            mPlayerObject.transform.localScale = Vector3.one;
            mPlayerObject.transform.forward = ModelParent.forward;
            mRightHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_R_Hand");
            mLeftHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_L_Hand");
            mBackHolder = ActorObj.RecursiveFindChild(transform, "E_back");
        }

        /// <summary>
        /// 显示武器。
        /// </summary>
        /// <param name="weapon">武器模型编号。</param>
        public void ShowWeapon(int weapon, int prof)
        {
            if (mCurWeaponID == weapon || mPlayerObject == null)
            {
                return;
            }

            //移除原来的武器            
            Destroy(mLeftWeapon);
            mLeftWeapon = null;
            Destroy(mRightWeapon);
            mRightWeapon = null;

            //加载新武器
            mCurWeaponID = weapon;
            if (mCurWeaponID == 0)
            {
                return;
            }
                        
            GameObject obj = SceneLoader.LoadModelObject(mCurWeaponID);
            if (obj == null)
            {
                LogMgr.WarningLog("加载武器id:{0}失败", mCurWeaponID);
                return;
            }

            //刺客职业特殊处理
            if (prof == 4)
            {
                mRightWeapon = obj.transform.Find("DM_R_wuqi01").gameObject;
                mRightWeapon.SetActive(true);
                mRightWeapon.transform.SetParent(mRightHand);
                mRightWeapon.transform.localPosition = Vector3.zero;
                mRightWeapon.transform.localScale = Vector3.one;
                mRightWeapon.transform.localRotation = Quaternion.identity;

                mLeftWeapon = obj.transform.Find("DM_L_wuqi01").gameObject;
                mLeftWeapon.SetActive(true);
                mLeftWeapon.transform.SetParent(mLeftHand);
                mLeftWeapon.transform.localPosition = Vector3.zero;
                mLeftWeapon.transform.localScale = Vector3.one;
                mLeftWeapon.transform.localRotation = Quaternion.identity;
                Destroy(obj);       //移除空对象
            }
            else
            {
                mRightWeapon = obj;
                mRightWeapon.SetActive(true);
                mRightWeapon.transform.SetParent(mRightHand);
                mRightWeapon.transform.localPosition = Vector3.zero;
                mRightWeapon.transform.localScale = Vector3.one;
                mRightWeapon.transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// 先翅膀。
        /// </summary>
        /// <param name="wing">翅膀模型编号。</param>
        public void ShowWing(int wing)
        {
            if (mCurWing == wing || mPlayerObject == null)
            {
                return;
            }

            //移除原来的翅膀
            Destroy(mWing);
            mWing = null;

            //更新翅膀
            mCurWing = wing;
            if (mCurWing == 0)
            {
                return;
            }

            GameObject obj = SceneLoader.LoadModelObject(mCurWing);
            if (obj == null)
            {
                LogMgr.WarningLog("加载翅膀id:{0}失败", mCurWing);
                return;
            }
                
            //挂接新翅膀
            mWing = obj;
            mWing.transform.parent = mBackHolder;
            mWing.transform.localPosition = Vector3.zero;
            mWing.transform.localRotation = Quaternion.identity;
            mWing.SetActive(true);
        }
    }
}

