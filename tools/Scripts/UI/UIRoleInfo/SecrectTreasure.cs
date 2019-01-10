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

[LuaCallCSharp]
[Hotfix]
public class SecrectTreasure : MonoBehaviour
{

    public string modelParentPath;
    public static GameObject CameraObj = null;
    public GameObject modelParent = null;
    private GameObject mRoleModel = null;
    LuaTable mModelCfg = null;
    Animation mAnimator;

    AnimationClip[] mAnimationClips;
    public string mStandActionName = ""; 
    //int animIndex = 0; 
    GameObject m_actionEffect; 
    //float m_lastActionOverTime = 0;

    int preCreatureDisplayId = 0;
    public static List<GameObject> modelParentList = new List<GameObject>();
    public static List<GameObject> RoleModelList = new List<GameObject>();

    void Awake()
    {
        //uGUI.UIEventListener.Get(this.gameObject).onDrag = OnDragEvent; 
    }

    public static void LoadParent(string modelpath)
    {
        if (CameraObj != null)
            return;
        GameObject obj = CoreEntry.gResLoader.Load(modelpath) as GameObject;

        if (obj != null)
        {
            CameraObj = (GameObject)Instantiate(obj);
            CameraObj.transform.parent = MainPanelMgr.Instance.UIRoot3d.transform;
            CameraObj.transform.localPosition = Vector3.zero;
            CameraObj.transform.localScale = Vector3.one;
            CameraObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            CameraObj.SetActive(true);
            for (int i = 1; i < 5; i++ )
            {
                modelParentList.Add(CameraObj.transform.DeepFindChild("modelParent" + i).gameObject);
                RoleModelList.Add(null);
            }
            //modelParent = CameraObj.transform.DeepFindChild("modelParent").gameObject;
        }  
    }
	void Start () {
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
                float dt = delta.x / Math.Abs(delta.x) *  6;
                mRoleModel.transform.Rotate(new Vector3(0f, -dt, 0f)); 
            }

        }
    }


    public void OnEnable()
    {
        //if (CameraObj != null)
        //   CameraObj.SetActive(true);
    }
    public void OnDisable()
    {
        //if (CameraObj != null)
        //    CameraObj.SetActive(false);
    }
    public void OnDestroy()
    {
        if (CameraObj != null)
        {
            GameObject.DestroyImmediate(CameraObj);
            CameraObj = null;
            modelParentList.Clear();
            RoleModelList.Clear();
        }
    }

    public void Update()
    {
        if(this == null)
        {
            return;
        }
        if (bFinishReturn && mAnimator != null && !string.IsNullOrEmpty(mStandActionName))
        {
            if(!mAnimator.isPlaying)
            {
                mAnimator.Play(mStandActionName);
                bFinishReturn = false;
            }
        }
    }

    public void RemoveModel(int index)
    {
        mRoleModel = RoleModelList[index];
        if (mRoleModel != null)
        {
            GameObject.DestroyImmediate(mRoleModel);
        }
        RoleModelList[index] = null;
        mRoleModel = null;
    }
    /// <summary>
    /// 模型id, 模型缩放比例(缩放值在配置表里读取)
    /// </summary>
    /// <param name="CreatureDisplayId"></param>
    /// <param name="scale"></param>
    public void SetModel(int CreatureDisplayId,int index)
     {
         if (index >= modelParentList.Count && index < 0) return;
         float scale = 1;
         if (modelParentList.Count == 0)
         {
             LoadParent(modelParentPath);
         }
         mRoleModel = RoleModelList[index];
         modelParent = modelParentList[index];
         if (preCreatureDisplayId == CreatureDisplayId && mRoleModel != null)
             return;

         mModelCfg = ConfigManager.Instance.Common.GetModelConfig(CreatureDisplayId);
         if (mModelCfg == null)
         {
             LogMgr.UnityLog("CreatureDisplayId : " + CreatureDisplayId);
             return;
         }
         preCreatureDisplayId = CreatureDisplayId;

         string mModelPath = mModelCfg.Get<string>("skl");
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
         mRoleModel.transform.parent = modelParent.transform;
         mRoleModel.transform.localPosition = Vector3.zero;
         mRoleModel.transform.localScale = Vector3.one;
         mRoleModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
         modelParent.transform.localScale = new Vector3(scale,scale,scale);
         RoleModelList[index] = mRoleModel;
        if(string.IsNullOrEmpty(mStandActionName))
        {
            mStandActionName = mModelCfg.Get<string>("san_idle");    
        }
         mAnimator = mRoleModel.gameObject.GetComponent<Animation>();
         if(mAnimator != null && !string.IsNullOrEmpty(mStandActionName))
        {
            mAnimator.Play(mStandActionName);        
        }
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
        if(!string.IsNullOrEmpty(mStandActionName)  &&actionName != mStandActionName)
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
}

