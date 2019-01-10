/**
* @file     : RoleDrag.cs
* @brief    : b
* @details  : 角色属性面板旋转角色
* @author   : lzp
* @date     : 2017/02/26
*/


using UnityEngine;
using System;
using SG;
using XLua;
using UnityEngine.EventSystems;

[LuaCallCSharp]
[Hotfix]
public class RoleDrag : MonoBehaviour {

    public string modelParentPath;
    public GameObject CameraObj = null;
    public GameObject modelParent = null;
    private GameObject mRoleModel = null;
    LuaTable mModelCfg = null;
    Animation mAnimator;

    AnimationClip[] mAnimationClips;
    string mStandActionName; 
    int actionNameIndex = 0;
    string[] actionNames = { "skill003", "skill001", "skill002" }; 
    //int animIndex = 0; 
    //GameObject m_actionEffect; 
    float m_lastActionOverTime = 0;

    int preCreatureDisplayId = 0;

    void Awake()
    {
        uGUI.UIEventListener.Get(this.gameObject).onDrag = OnDragEvent;
        uGUI.UIEventListener.Get(this.gameObject).onPointerClick = OnClickEvent;
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
            modelParent = CameraObj.transform.FindChild("modelParent").gameObject;
        }  
    }
	void Start () {
	}
	 
    public void OnDragEvent(GameObject go, PointerEventData data)
    {
        OnDrag(data.delta);
    }
    public void OnClickEvent(GameObject go)
    {
        OnClick();
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
    #region -- 需注释掉 -- 
    
     void OnClick()
     { 
         if (Time.time < m_lastActionOverTime)
         {
             return; 
         }
 
        if (mRoleModel == null || mAnimator == null)
        {
            return;
        }

        LogMgr.UnityLog("mAnimator.clip.name:" + mAnimator.clip.name);

        if (actionNameIndex >= actionNames.Length)
        {
            actionNameIndex = 0; 
        }
        string aname = actionNames[actionNameIndex++];

        if (mAnimator.IsPlaying("standby"))
        {
            if (actionNameIndex - 1 == 0)
            {
                //CoreEntry.gAudioMgr.PlayUISound(mDisplayDesc.idleVoice);

            }
            else if (actionNameIndex - 1 ==1)
            {
                //CoreEntry.gAudioMgr.PlayUISound(mDisplayDesc.skill1Voice);
            }
            else if (actionNameIndex - 1 == 2)
            {
                //CoreEntry.gAudioMgr.PlayUISound(mDisplayDesc.skill2Voice);
            }
        }
        AnimationClip clip  =  mAnimator.GetClip(aname);
        if (clip != null)
        {
            m_lastActionOverTime = clip.length + Time.time;


            mAnimator.CrossFade(aname, 0.2f);
            mAnimator.CrossFadeQueued("standby");
        }
        else
        {
            LogMgr.LogError("没有找到 clip : " + aname);
        }
         
    }
     

    #endregion

    public void SetModel(int CreatureDisplayId)
     {
         if (modelParent == null)
         {
             LoadParent();
         }
         if (preCreatureDisplayId == CreatureDisplayId)
             return;

         mModelCfg = ConfigManager.Instance.Common.GetModelConfig(CreatureDisplayId);
         if (mModelCfg == null)
         {
             LogMgr.UnityLog("CreatureDisplayId : " + CreatureDisplayId);
             return;
         }
         preCreatureDisplayId = CreatureDisplayId;

         string mModelPath = mModelCfg.Get<string>("skl") + "_DL";
         GameObject rolePrefab = CoreEntry.gResLoader.Load(mModelPath) as GameObject;
        if (rolePrefab == null)
        {
            mModelPath = mModelCfg.Get<string>("skl");
            rolePrefab = CoreEntry.gResLoader.Load(mModelPath) as GameObject;
        }
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


         mStandActionName = "standby";
         mAnimator = mRoleModel.gameObject.GetComponent<Animation>();

         actionNameIndex = 0;
         mAnimationClips = new AnimationClip[mAnimator.GetClipCount() + 1];
         int i = 0;
         foreach (AnimationState clip in mAnimator)
         {
             mAnimationClips[i++] = clip.clip;
         }

         if (findAction("standby"))
         {
             mStandActionName = "standby";
         }
         mAnimator.Play(mStandActionName);
     }
    public bool findAction(string actionName)
    {
        foreach (AnimationClip clip in mAnimationClips)
        {
            if (clip == null)
            {
                continue;
            }
            if (clip.name.IndexOf(actionName) != -1)
            {
                return true;
            }
        }
        return false;
    }


}

