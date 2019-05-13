/**
* @file     :  
* @brief    : 
* @details  : 
* @author   :  
* @date     :  
*/

using UnityEngine;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class PlayerModelShowInfo : MonoBehaviour
    {
        /// <summary>
        /// 显示对象预制件路径。
        /// </summary>
        public static string PrefabPath = "UI/Prefabs/RoleInfo/FirstRes/PlayerModelShowInfo";

        /// <summary>
        /// 当前模型显示对象
        /// </summary>
        public static PlayerModelShowInfo CurModeShow;

        /// <summary>
        /// 默认旋转。
        /// </summary>
        private static Quaternion DefaultRotate;

        /// <summary>
        /// 技能特效。
        /// </summary>
        private static GameObject SkillEffect;

        private static GameObject _objZhenFa;

        public Camera mCarmera = null;

        /// <summary>
        /// 显示玩家模型。
        /// </summary>
        /// <param name="type">显示类型。0正面 1技能预览</param>
        public static void ShowPlayerModel(int type = 0)
        {
            if (CurModeShow == null)
            {
                GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(PrefabPath, typeof(GameObject));
                if (prefab == null)
                {
                    return;
                }

                GameObject obj = Instantiate(prefab) as GameObject;
                CurModeShow = obj.GetComponent<PlayerModelShowInfo>();
                CurModeShow.transform.localPosition = new Vector3(0, -1000, 0);
                CurModeShow.transform.localScale = Vector3.one;
                DefaultRotate = CurModeShow.ModelParent.rotation;
                CurModeShow.mCarmera = obj.GetComponentInChildren<Camera>();

            }

            if (!CurModeShow.gameObject.activeSelf)
            {
                CurModeShow.gameObject.SetActive(true);
                CurModeShow.ModelParent.rotation = DefaultRotate;
            }
            CurModeShow.ShowModel();
        }

        /// <summary>
        /// 设置摄像机位置。
        /// </summary>
        public static void SetCameraPosition(float x, float y, float z)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }
            CurModeShow.CameraT.localPosition = new Vector3(x, y, z);            
        }

        /// <summary>
        /// 设置摄像机旋转。
        /// </summary>
        public static void SetCameraRotation(float x, float y, float z)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }
            CurModeShow.CameraT.localRotation = Quaternion.Euler(x, y, z);
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

            if (SkillEffect != null)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(SkillEffect);
            }
        }

        /// <summary>
        /// 播放动画。
        /// </summary>
        /// <param name="name">动画名称。</param>
        public static void PlayAnimation(string name)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }

            //播放动作                        
            Animation ani = CurModeShow.mPlayerObject.GetComponent<Animation>();
            ani.Play(name);
            ani.PlayQueued("stand");
        }

        /// <summary>
        /// 显示阵法
        /// </summary>
        public static void HangZhenFa(int id)
        {
            if (CurModeShow == null) return;
            if (_objZhenFa != null) _objZhenFa.SetActive(false);

            LuaTable row = RawLuaConfig.Instance.GetRowData("t_zhenfa", id);
            if(row != null)
            {
                LuaTable cfg = RawLuaConfig.Instance.GetRowData("t_model", row.Get<int>("ui_show_sen"));
                if (cfg == null) return;

                _objZhenFa = GameObject.Instantiate(CoreEntry.gResLoader.Load(cfg.Get<string>("skl"))) as GameObject;
                if(_objZhenFa != null)
                {
                    _objZhenFa.SetActive(true);
                    _objZhenFa.transform.parent = CurModeShow.mPlayerObject.transform;
                    _objZhenFa.transform.localPosition = new Vector3(0, 0, 0);
                    _objZhenFa.transform.localRotation = Quaternion.identity;
                }
            }
        }

        /// <summary>
        /// 展示技能。
        /// </summary>
        /// <param name="id">技能编号。</param>
        public static void ShowSkill(int id)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }

            LuaTable cfg = ConfigManager.Instance.Skill.GetSkillConfig(id);
            if (cfg != null)
            {
                string actionstr = cfg.Get<string>("skill_action");
                int action;
                if (int.TryParse(actionstr, out action))
                {
                    LuaTable actioncfg = ConfigManager.Instance.Skill.GetSkillActionConfig(action);
                    if (actioncfg != null)
                    {
                        //播放动作                        
                        Animation ani = CurModeShow.mPlayerObject.GetComponent<Animation>();
                        ani.Play(actioncfg.Get<string>("animation"));
                        ani.PlayQueued("stand");
                        AddSkillEffect(actioncfg);
                    }
                }
            }
            else
            {
                Debug.LogAssertionFormat("未知技能 id:{0}", id);
            }
        }

        /// <summary>
        /// 添加技能特效。
        /// </summary>
        /// <param name="actioncfg">技能效果配置。</param>
        public static void AddSkillEffect(LuaTable actioncfg)
        {
            //技能特效
            if (SkillEffect != null)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(SkillEffect);
            }

            string actionEfx = actioncfg.Get<string>("skilleffect");
            if (!string.IsNullOrEmpty(actionEfx))
            {
                SkillEffect = CoreEntry.gGameObjPoolMgr.InstantiateEffect(actionEfx);
                if (SkillEffect != null)
                {
                    EfxAttachActionPool efx = SkillEffect.GetComponent<EfxAttachActionPool>();
                    if (efx == null)
                    {
                        efx = SkillEffect.AddComponent<EfxAttachActionPool>();
                    }

                    Transform player = CurModeShow.mPlayerObject.transform;
                    Transform eft = SkillEffect.transform;
                    efx.Init(player, actioncfg.Get<float>("skillEfxLength"), false);
                    if (actioncfg.Get<bool>("isBind"))
                    {
                        Transform bindTran;
                        string hangpoint = actioncfg.Get<string>("hangPoint");
                        if (!string.IsNullOrEmpty(hangpoint))
                        {
                            bindTran = player.transform.DeepFindChild(hangpoint);
                            if (null == bindTran)
                            {
                                bindTran = player;
                            }
                        }
                        else
                        {
                            bindTran = player;
                        }

                        eft.SetParent(bindTran);
                        eft.localPosition = Vector3.zero;
                        eft.localScale = Vector3.one;
                        eft.localRotation = Quaternion.identity;
                    }
                    else
                    {
                        eft.SetParent(player.parent);
                        eft.position = player.position;
                        eft.localScale = Vector3.one;
                        eft.rotation = player.rotation;
                    }
                }
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
        /// 试穿时装。
        /// </summary>
        /// <param name="id">时装编号。</param>
        public static void TryFashion(int id)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }

            int part = id % 10;
            int modelid = ConfigManager.Instance.BagItem.GetFashionModelID(id, PlayerData.Instance.Job,0);
            if (part == 1)
            {
                CurModeShow.ChangeWeapon(modelid);
            }
            else if (part == 2)
            {
                CurModeShow.ChangeClothes(modelid);
            }
            else if (part == 3)
            {
                CurModeShow.ChangeWing(modelid);
            }
        }

        /// <summary>
        /// 试穿神装接口
        /// </summary>
        /// <param name="part">部位</param>
        /// <param name="modelid">对应id</param>
        public static void TryGod(int part,int modelid)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }
            if (modelid == 0)
                return;
            if (part == 1)
            {
                CurModeShow.ChangeWeapon(modelid);
            }
            else if (part == 2)
            {
                CurModeShow.ChangeClothes(modelid);
            }
            else if (part == 3)
            {
                CurModeShow.ChangeWing(modelid);
            }
        }

        /// <summary>
        /// 取消试穿时装。
        /// </summary>
        /// <param name="part">时装部位。</param>
        public static void CancelTryFashion(int part)
        {
            if (CurModeShow == null || !CurModeShow.gameObject.activeSelf)
            {
                return;
            }
            
            if (part == 1)
            {
                CurModeShow.ChangeWeapon(PlayerData.Instance.GetWeaponModelID());
            }
            else if (part == 2)
            {
                CurModeShow.ChangeClothes(PlayerData.Instance.GetDressModelID());
            }
            else if (part == 3)
            {
                CurModeShow.ChangeWing(PlayerData.Instance.GetWingModelID());
            }
        }

        /// <summary>
        /// 摄像机。
        /// </summary>
        public Transform CameraT;

        /// <summary>
        /// 模型挂接点。
        /// </summary>
        public Transform ModelParent;

        /// <summary>
        /// 玩家对象。
        /// </summary>
        private GameObject mPlayerObject;

        /// <summary>
        /// 当前衣服外观。
        /// </summary>
        private int mCurClothesID;

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
        /// 显示模型。
        /// </summary>
        public void ShowModel()
        {
            //创建初始模型
            if (mPlayerObject == null)
            {
                //创建角色
                mCurClothesID = PlayerData.Instance.GetDressModelID();
                mPlayerObject = SceneLoader.LoadModelObject(mCurClothesID);
                if (mPlayerObject == null)
                {
                    LogMgr.WarningLog("加载衣服id:{0}失败", mCurClothesID);
                    return;
                }
                mPlayerObject.transform.SetParent(ModelParent);
                mPlayerObject.transform.localPosition = Vector3.zero;
                mPlayerObject.transform.localScale = Vector3.one;
                mPlayerObject.transform.forward = ModelParent.forward;                
                mRightHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_R_Hand");
                mLeftHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_L_Hand");
                mCurClothesID = 0;
            }

            //创建初始武器
            if (mRightWeapon == null)
            {
                //创建武器
                ChangeWeapon(PlayerData.Instance.GetWeaponModelID());
            }
            mCurClothesID = 0;
            mCurWing = PlayerData.Instance.GetWingModelID();
            mCurWeaponID = PlayerData.Instance.GetWeaponModelID();
            ChangeClothes(PlayerData.Instance.GetDressModelID());
            mPlayerObject.GetComponent<Animation>().Play("stand");
            CameraT.localPosition = new Vector3(0, 0, 0f);
            CameraT.localRotation = Quaternion.Euler(0, 0, 0);
        }

        /// <summary>
        /// 改变武器。
        /// </summary>
        public void ChangeWeapon(int id)
        {
            //已经有同名武器了
            if (mCurWeaponID == id)
            {
                return;
            }

            GameObject obj = SceneLoader.LoadModelObject(id);
            if (obj == null)
            {
                LogMgr.WarningLog("加载武器id:{0}失败", id);
                return;
            }

            if (PlayerData.Instance.Job == 4)
            {
                //刺客时双手武器特殊处理
                if (mRightWeapon != null)
                {
                    Destroy(mRightWeapon);
                    mRightWeapon = null;
                }
                mRightWeapon = obj.transform.Find("DM_R_wuqi01").gameObject;
                mRightWeapon.SetActive(true);
                mRightWeapon.transform.SetParent(mRightHand);
                mRightWeapon.transform.localPosition = Vector3.zero;
                mRightWeapon.transform.localScale = Vector3.one;
                mRightWeapon.transform.localRotation = Quaternion.identity;

                if (mLeftWeapon != null)
                {
                    Destroy(mLeftWeapon);
                    mLeftWeapon = null;
                }
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
                if (mRightWeapon != null)
                {
                    Destroy(mRightWeapon);
                    mRightWeapon = null;
                }

                mRightWeapon = obj;
                mRightWeapon.SetActive(true);
                mRightWeapon.transform.SetParent(mRightHand);
                mRightWeapon.transform.localPosition = Vector3.zero;
                mRightWeapon.transform.localScale = Vector3.one;
                mRightWeapon.transform.localRotation = Quaternion.identity;
            }
            mCurWeaponID = id;
        }

        /// <summary>
        /// 检查衣服是否变化。
        /// </summary>
        private void ChangeClothes(int id)
        {
            if (mCurClothesID == id)
            {
                return;
            }
            if(CoreEntry.gActorMgr.MainPlayer != null)
            {
                if (CoreEntry.gMorphMgr.IsInMorph(CoreEntry.gActorMgr.MainPlayer.ServerID))
                {
                    return;
                }
            }
            GameObject prefab = SceneLoader.LoadModelObject(id);
            if (prefab == null)
            {
                LogMgr.WarningLog("加载衣服id:{0}失败", id);
                return;
            }
            prefab.SetActive(true);
            GameObject tmpObj = mPlayerObject;
            if (tmpObj != null)
            {
                GameObject.DestroyImmediate(tmpObj);
            }
            mPlayerObject = prefab;
            mPlayerObject.transform.SetParent(ModelParent);
            mPlayerObject.transform.localPosition = Vector3.zero;
            mPlayerObject.transform.localScale = Vector3.one;
            mPlayerObject.transform.forward = ModelParent.forward;
            mRightHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_R_Hand");
            mLeftHand = ActorObj.RecursiveFindChild(mPlayerObject.transform, "DM_L_Hand");

            mCurClothesID = id;
 
            int WeaponID = mCurWeaponID;
            int Wing = mCurWing;
            mCurWeaponID = 0;
            mCurWing = 0;
            ChangeWeapon(WeaponID);
            ChangeWing(Wing); 
        }
        
        /// <summary>
        /// 改变翅膀。
        /// </summary>
        /// <param name="id">翅膀模型编号。</param>
        private void ChangeWing(int id)
        {

            if (mCurWing == id)
            {
                return;
            }

            if (mBackHolder == null)
            {
                mBackHolder = ActorObj.RecursiveFindChild(transform, "E_back");
            }

            //更新翅膀
            if (id != 0)
            {
                GameObject obj = SceneLoader.LoadModelObject(id);
                if (obj == null)
                {
                    LogMgr.WarningLog("加载翅膀id:{0}失败", id);
                    return;
                }

                //移除原来翅膀
                if (mWing != null)
                {
                    Destroy(mWing);
                    mWing = null;
                }

                //挂接新翅膀
                mWing = obj;
                mWing.transform.parent = mBackHolder;
                mWing.transform.localPosition = Vector3.zero;
                mWing.transform.localRotation = Quaternion.identity;
                mWing.SetActive(true);
            }
            else
            {
                Destroy(mWing);
                mWing = null;
            }
            mCurWing = id;
        }

        protected void OnEnable()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_INFO, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_ADD, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_DEL, OnBagChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_UPDATE, OnBagChange);
        }

        protected void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BAG_INFO, OnBagChange);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BAG_ITEM_ADD, OnBagChange);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BAG_ITEM_DEL, OnBagChange);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BAG_ITEM_UPDATE, OnBagChange);
        }

        private void OnBagChange(GameEvent ge, EventParameter parameter)
        {
            //装备背包变更
            if (parameter.intParameter == BagType.ITEM_BAG_TYPE_EQUIP)
            {
                ChangeClothes(PlayerData.Instance.GetDressModelID());
                ChangeWeapon(PlayerData.Instance.GetWeaponModelID());
            }
            if(parameter.intParameter == BagType.ITEM_BAG_TYPE_ROLEITEM)
            {
                ChangeWing(PlayerData.Instance.GetWingModelID());
            }
        }
    }
}

