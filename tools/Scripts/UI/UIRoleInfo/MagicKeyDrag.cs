/**
* @file     : MagicKeyDrag.cs
* @brief    : b
* @details  :  
* @author   :  
* @date     :  
*/


using UnityEngine;
using System;
using SG;
using XLua;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

[LuaCallCSharp]
[Hotfix]
public class MagicKeyDrag : MonoBehaviour
{

    public string modelParentPath;
    public GameObject CameraObj = null;
    public Camera mCarmera = null;
    public GameObject modelParent = null;
    private GameObject mRoleModel = null;
    LuaTable mModelCfg = null;
    Animation mAnimator;

    AnimationClip[] mAnimationClips;
    public string mStandActionName = "";
    //int animIndex = 0;
    GameObject m_actionEffect;
    //float m_lastActionOverTime = 0;
    public GameObject mObjPfx; //特效对象

    public int preCreatureDisplayId = 0;

    private static Dictionary<int, GameObject> CacheModel = new Dictionary<int, GameObject>();

    /// <summary>
    /// 添加模型缓存。
    /// </summary>
    /// <param name="mid">模型ID。</param>
    public static void AddCache(int mid)
    {
        if (CacheModel.ContainsKey(mid))
        {
            return;
        }

        LuaTable cfg = ConfigManager.Instance.Common.GetModelConfig(mid);
        if (cfg == null)
        {
            LogMgr.LogError("The model config is null. id:{0}", mid);
            return;
        }

        string mModelPath = cfg.Get<string>("skl");
        GameObject prefab = CoreEntry.gResLoader.Load(mModelPath) as GameObject;
        if (prefab == null)
        {
            LogMgr.LogError("模型路径错误: {0}", mModelPath);
            return;
        }

        GameObject obj = (GameObject)Instantiate(prefab);
        DontDestroyOnLoad(obj);
        obj.SetActive(false);
        CacheModel.Add(mid, obj);
    }

    /// <summary>
    /// 移除缓存。
    /// </summary>
    /// <param name="mid"></param>
    public static void RemoveCache(int mid)
    {
        GameObject obj;
        if (CacheModel.TryGetValue(mid, out obj))
        {
            CacheModel.Remove(mid);
            GameObject.Destroy(obj);
            obj = null;
        }
    }

    void Awake()
    {
        uGUI.UIEventListener.Get(this.gameObject).onDrag = OnDragEvent;
    }

    public void LoadParent()
    {
        GameObject obj = CoreEntry.gResLoader.Load(modelParentPath) as GameObject;

        if (obj != null)
        {
            CameraObj = (GameObject)Instantiate(obj);
            CameraObj.transform.parent = MainPanelMgr.Instance.UIRoot3d.transform;
            CameraObj.transform.localPosition = Vector3.zero;
            CameraObj.transform.localScale = Vector3.one;
            CameraObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            CameraObj.SetActive(true);
            mCarmera = CameraObj.GetComponentInChildren<Camera>();
            modelParent = CameraObj.transform.DeepFindChild("modelParent").gameObject;
        }
        else
        {
            LogMgr.LogError(string.Format("查找资源失败 {0}" ,modelParentPath));
        }
    }
    void Start()
    {
    }

    public void PlayAni(string name)
    {
        mAnimator.Play(name);
        mAnimator.PlayQueued(mStandActionName);
    }

    public void OnDragEvent(GameObject go, PointerEventData data)
    {
        OnDrag(data.delta);
    }

    void OnDrag(Vector2 delta)
    {
        if (mRoleModel != null)
        {
            if (Math.Abs(delta.x) > 0)
            {
                float dt = delta.x / Math.Abs(delta.x) * 6;
                mRoleModel.transform.Rotate(new Vector3(0f, -dt, 0f));
            }

        }
    }

    public void OnEnable()
    {
        if (CameraObj != null)
            CameraObj.SetActive(true);
    }
    public void OnDisable()
    {
        if (CameraObj != null)
            CameraObj.SetActive(false);
    }
    public void OnDestroy()
    {
        if (CameraObj != null)
        {
            GameObject.DestroyImmediate(CameraObj);
            CameraObj = null;
        }
    }

    public void Update()
    {
        if (this == null)
        {
            return;
        }
        if (bFinishReturn && mAnimator != null && !string.IsNullOrEmpty(mStandActionName))
        {
            if (!mAnimator.isPlaying)
            {
                if (mAnimator.GetClip(mStandActionName) != null)
                {
                    mAnimator.Play(mStandActionName);
                    bFinishReturn = false;
                }
            }
        }
    }

    public void RemoveModel()
    {
        if (mRoleModel != null)
        {
            GameObject.DestroyImmediate(mRoleModel);
        }
        mObjPfx = null;
        mRoleModel = null;
        preCreatureDisplayId = 0;
        if (CameraObj != null && CameraObj.activeSelf)
            CameraObj.gameObject.SetActive(false);
    }
    /// <summary>
    /// 模型id, 模型缩放比例(缩放值在配置表里读取)
    /// </summary>
    /// <param name="CreatureDisplayId"></param>
    /// <param name="scale"></param>
    public void SetModel(int CreatureDisplayId, float scale = 1)
    {
        SetModelEx(CreatureDisplayId, scale, Vector3.zero, Vector3.zero);
    }
    public void SetModel(int CreatureDisplayId,string _path, float scale = 1)
    {
        SetModelEx(CreatureDisplayId, scale, Vector3.zero, Vector3.zero,_path);
    }
    /// <summary>
    /// 模型id, 模型缩放比例(缩放值在配置表里读取)
    /// </summary>
    /// <param name="CreatureDisplayId"></param>
    /// <param name="scale"></param>
    public void SetModelEx(int CreatureDisplayId, float scale, Vector3 offset, Vector3 dir,string _path = "")
    {
        if (CameraObj != null && !CameraObj.activeSelf)
            CameraObj.gameObject.SetActive(true);

        if (modelParent == null)
        {
            LoadParent();
        }

        mModelCfg = ConfigManager.Instance.Common.GetModelConfig(CreatureDisplayId);
        if (mModelCfg == null)
        {
            LogMgr.UnityLog("CreatureDisplayId : " + CreatureDisplayId);
            return;
        }


        if (preCreatureDisplayId == CreatureDisplayId && mRoleModel != null)
        {
            mStandActionName = mModelCfg.Get<string>("san_idle");
            mAnimator = mRoleModel.gameObject.GetComponent<Animation>();
            if (mAnimator != null && !string.IsNullOrEmpty(mStandActionName))
            {
                if (mAnimator.GetClip(mStandActionName) != null)
                {
                    mAnimator.Play(mStandActionName);
                }
            }

            return;
        }
        preCreatureDisplayId = CreatureDisplayId;

        GameObject obj;
        if (CacheModel.TryGetValue(CreatureDisplayId, out obj))
        {
            CacheModel.Remove(CreatureDisplayId);

            if (mRoleModel != null)
            {
                GameObject.DestroyImmediate(mRoleModel);
            }

            obj.SetActive(true);
            mRoleModel = obj;
        }
        else
        {
            string mModelPath = _path;
            if (mModelPath == "") mModelPath = mModelCfg.Get<string>("skl");
            GameObject rolePrefab = CoreEntry.gResLoader.Load(mModelPath) as GameObject;
            if (rolePrefab == null)
            {
                LogMgr.UnityLog("模型路径错误: " + mModelPath);
                return;
            }


            if (mRoleModel != null)
            {
                GameObject.DestroyImmediate(mRoleModel);
            }

            mRoleModel = (GameObject)Instantiate(rolePrefab);
        }
        
        mRoleModel.transform.parent = modelParent.transform;
        mRoleModel.transform.localPosition = offset;
        mRoleModel.transform.localScale = Vector3.one;
        mRoleModel.transform.localRotation = Quaternion.Euler(dir);
        mRoleModel.SetActive(true);
        modelParent.transform.localScale = new Vector3(scale, scale, scale);

        //if (string.IsNullOrEmpty(mStandActionName))
        {
            mStandActionName = mModelCfg.Get<string>("san_idle");
        }
        mAnimator = mRoleModel.gameObject.GetComponent<Animation>();
        if (mAnimator != null && !string.IsNullOrEmpty(mStandActionName))
        {
            if (mAnimator.GetClip(mStandActionName) != null)
            {
                LogMgr.UnityLog("CreatureDisplayId " + CreatureDisplayId + " mStandActionName " + mStandActionName);
                mAnimator.Play(mStandActionName);
            }
        }
        RawImage raw = this.GetComponent<RawImage>();
        if(raw)
        {
            raw.texture = mCarmera.targetTexture;
            raw.material = AtlasSpriteManager.Instance.RTMaterial;
        }
    }

    //设置相机下 模型挂点的Transform坐标(读取模型表 坐标配置)
    public void SetCfgPosition()
    {
        if(mModelCfg== null || modelParent == null)
        {
            return;
        }
        float posX = mModelCfg.Get<float>("posX");
        float posY = mModelCfg.Get<float>("posY");
        float posZ = mModelCfg.Get<float>("posZ");
        float rotateX = mModelCfg.Get<float>("rotateX");
        float rotateY = mModelCfg.Get<float>("rotateY");
        float rotateZ = mModelCfg.Get<float>("rotateZ");

        modelParent.transform.localPosition = new Vector3(posX,posY,posZ);
        modelParent.transform.localEulerAngles = new Vector3(rotateX, rotateY, rotateZ);
        modelParent.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置相机下 模型挂点的Transform坐标(手动设置)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="rotateX"></param>
    /// <param name="rotateY"></param>
    /// <param name="rotateZ"></param>
    public void SetFixedCfgPosition(float x,float y,float z,float rotateX,float rotateY,float rotateZ)
    {
        if (mModelCfg == null || modelParent == null)
        {
            return;
        }
        modelParent.transform.localPosition = new Vector3(x, y, z);
        modelParent.transform.localEulerAngles = new Vector3(rotateX, rotateY, rotateZ);
        modelParent.transform.localScale = Vector3.one;
    }

    public void SetMagicKeyModelY(float y)
    {
        if (mRoleModel)
        {
            mRoleModel.transform.localPosition = new Vector3(0, y, 0);
        }
    }

    bool bFinishReturn = false;

    /// <summary>
    /// 展示指定动作
    /// </summary>
    /// <param name="actionName"></param>
    public void ShowSkillActionAnimation(string actionName)
    {
        if (mAnimator != null && !string.IsNullOrEmpty(actionName))
        {
            mAnimator.Play(actionName);
        }
        if (!string.IsNullOrEmpty(mStandActionName) && actionName != mStandActionName)
        {
            bFinishReturn = true;    //播放完
        }
    }

    public void SetWeapon(int fashionWeapon, int shenbing, int weapon, int job)
    {
        ShowWeapon(SceneLoader.GetWeaponModelID(fashionWeapon, shenbing, weapon, job), job == 4);
    }
    public void ShowWeapon(int id, bool bDoubleWeapon)
    {
        if (null == mRoleModel)
        {
            return;
        }

        Transform rTran = ActorObj.RecursiveFindChild(mRoleModel.transform, "DM_R_Hand");
        if (null == rTran)
        {
            LogMgr.UnityError("no DM_R_Hand point!!!");

            return;
        }

        GameObject obj = SceneLoader.LoadModelObject(id);
        if (null == obj)
        {
            LogMgr.UnityError("no weapon:id " + id);

            return;
        }

        if (bDoubleWeapon)
        {
            Transform lTran = ActorObj.RecursiveFindChild(mRoleModel.transform, "DM_L_Hand");
            if (null == lTran)
            {
                LogMgr.UnityError("no DM_L_Hand point!!!");

                return;
            }

            Transform lWeapon = obj.transform.Find("DM_L_wuqi01");
            Transform rWeapon = obj.transform.Find("DM_R_wuqi01");

            lWeapon.SetParent(lTran);
            lWeapon.localPosition = Vector3.zero;
            lWeapon.localScale = Vector3.one;
            lWeapon.localRotation = Quaternion.identity;

            rWeapon.SetParent(rTran);
            rWeapon.localPosition = Vector3.zero;
            rWeapon.localScale = Vector3.one;
            rWeapon.localRotation = Quaternion.identity;

            UnityEngine.Object.Destroy(obj);
        }
        else
        {
            obj.transform.SetParent(rTran);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;
        }
    }

    public void SetWing(int id)
    {
        if (null == mRoleModel)
        {
            return;
        }

        Transform bTran = ActorObj.RecursiveFindChild(mRoleModel.transform, "E_back");
        if (bTran == null)
        {
            return;
        }

        id = ConfigManager.Instance.BagItem.GetWingModelID(id);
        if (id != 0)
        {
            GameObject obj = SceneLoader.LoadModelObject(id);
            if (obj == null)
            {
                return;
            }

            obj.transform.SetParent(bTran);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏模型中某个挂点
    /// </summary>
    /// <param name="objName"></param>
    public void HideModelTran(string objName)
    {
        if (null == mRoleModel)
        {
            return;
        }
        if (string.IsNullOrEmpty(objName))
        {
            return;
        }
        Transform child = mRoleModel.transform.Find(objName);
        if (child != null)
        {
            child.gameObject.SetActive(false);
        }
    }
    public void DestroyPfx()
    {
        if (mObjPfx != null)
            GameObject.DestroyImmediate(mObjPfx);
        mObjPfx = null; 
    }
    /// 添加特效
    public GameObject AddPfx(string strPfxName)
    {
        if(null == mRoleModel || string.IsNullOrEmpty(strPfxName)) return null;
        if (mObjPfx != null && mObjPfx.name == strPfxName) return null;

        GameObject rolePrefab = CoreEntry.gResLoader.Load(strPfxName) as GameObject;
        if (rolePrefab != null)
        {
            mObjPfx = GameObject.Instantiate(rolePrefab) as GameObject;
            mObjPfx.transform.SetParent(mRoleModel.transform);
            mObjPfx.transform.localPosition = Vector3.zero;
            mObjPfx.transform.localRotation = Quaternion.identity;
            mObjPfx.transform.localScale = Vector3.one;
            mObjPfx.name = strPfxName;
        }

        return mObjPfx;
    }


    //控制nc脚本
    public void SetNCAni(bool bAni)
    {
        if(modelParent != null)
        {
            NcCurveAnimation nc = modelParent.GetComponent<NcCurveAnimation>();
            if(nc != null)
            {
                nc.enabled = bAni;
            }
        }
    }

}

