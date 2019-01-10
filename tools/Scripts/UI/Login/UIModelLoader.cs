/**
* @file     : UIModelLoader.cs
* @brief    : 
* @details  : 
* @author   : lzp
* @date     : 2017/6/26
*/
using UnityEngine;
using System.Collections.Generic;
using XLua;
using System;
using SG;
using UnityEngine.EventSystems;


[LuaCallCSharp]
[Hotfix]
public class UIModelLoader : MonoBehaviour
{ 
    GameObject modelParent;
    GameObject mRoleModel;
    public String Path = "UIRoot3D/RoleUI/cs_juese_pingtai001/modelParent";

    public GameObject mModelDragUI;
    //Configs.CreatureDisplayDesc mDisplayDesc;
    LuaTable mModelCfg;

    Animation mAnimator;
    AnimationClip[] mAnimationClips;
    string mStandActionName;
    int actionNameIndex = 0;
    string[] actionNames;
    //int animIndex = 0;
    GameObject m_actionEffect;
    float m_lastActionOverTime = 0;
    float m_enterFinishTime = 0;

    public string standy = "enter001_standby";

    private AudioSource mAudioSource;

    private GameObject shadowPro;

    void Awake()
    {
        modelParent = GameObject.Find(Path).gameObject;

        uGUI.UIEventListener.Get(mModelDragUI).onDrag = OnDragEvent;
        uGUI.UIEventListener.Get(mModelDragUI).onPointerClick = OnClickEvent;

    }

    public void OnDragEvent(GameObject go, PointerEventData data)
    {
        OnDrag(data.delta);
    }
    public void OnClickEvent(GameObject go)
    {
        //OnClick();
    }


    void OnDrag(Vector2 delta)
    {
        if (Time.time < m_enterFinishTime)
        {
            return;
        }

        if (mRoleModel != null)
        {
            if (Math.Abs(delta.x) > 0)
            {
                float dt = delta.x / Math.Abs(delta.x) * 4;
                mRoleModel.transform.Rotate(new Vector3(0f, -dt, 0f));
     
            }

        }
    }


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

        if (mAnimator.IsPlaying(standy))
        {
            if (actionNameIndex - 1 == 0)
            {
                //CoreEntry.gAudioMgr.PlayUISound(mDisplayDesc.idleVoice);

            }
            else if (actionNameIndex - 1 == 1)
            {
                //CoreEntry.gAudioMgr.PlayUISound(mDisplayDesc.skill1Voice);
            }
            else if (actionNameIndex - 1 == 2)
            {
                //CoreEntry.gAudioMgr.PlayUISound(mDisplayDesc.skill2Voice);
            } 
        }
        AnimationClip clip = mAnimator.GetClip(aname);
        if (clip != null)
        {
            m_lastActionOverTime = clip.length + Time.time;


            mAnimator.CrossFade(aname, 0.2f);
            mAnimator.CrossFadeQueued(standy);
        }
        else
        {
            LogMgr.LogError("没有找到 clip : " + aname);
        }

    }
     

    public void SetModel(string showSkills, int mainModelID, int weaponModelId, int wingModelID, bool doubleBlade, int audioID, int enterAudioID)
    {
        CancelInvoke();

        CoreEntry.gAudioMgr.PlayUISound(audioID);

        actionNames = new string[3]{ "skill003", "skill002", "skill001" };
        if (!string.IsNullOrEmpty(showSkills))
        {
            string[] ids = showSkills.Split('#');
            if(null != ids && ids.Length > 0)
            {
                actionNames = new string[ids.Length];
                for(int i = 0;i < ids.Length;i++)
                {
                    int id = 0;
                    int.TryParse(ids[i], out id);
                    LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(id);
                    if(null != skillCfg)
                    {
                        int actionID = 0;
                        int.TryParse(skillCfg.Get<string>("skill_action"), out actionID);
                        LuaTable skillActionCfg = ConfigManager.Instance.Skill.GetSkillActionConfig(actionID);
                        if(null != skillActionCfg)
                        {
                            actionNames[i] = skillActionCfg.Get<string>("animation");
                        }
                    }
                }
            }
        }

        mModelCfg = ConfigManager.Instance.Common.GetModelConfig(mainModelID);
        if (mModelCfg == null)
        {
            LogMgr.UnityLog("CreatureDisplayId : " + mainModelID);
            return;
        }

        string mModelPath = mModelCfg.Get<string>("skllogin");
        GameObject rolePrefab = CoreEntry.gResLoader.Load(mModelPath) as GameObject;
        if (rolePrefab == null)
        {
            LogMgr.UnityLog("模型路径错误: " + mModelPath);
            return;
        }

        PlayerAgent agent = rolePrefab.GetComponent<PlayerAgent>();
        if (null != agent)
        {
            agent.enabled = false;
        }

        if (mRoleModel != null)
        { 
            GameObject.DestroyImmediate(mRoleModel);
        }

        mRoleModel = (GameObject)Instantiate(rolePrefab);
        if (null == modelParent)
        {
            modelParent = GameObject.Find(Path).gameObject;
        }
        mRoleModel.transform.parent = modelParent.transform;
        mRoleModel.transform.localPosition = Vector3.zero;
        mRoleModel.transform.localScale = Vector3.one;
        mRoleModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
        CommonTools.SetMainLayer(mRoleModel, mainModelID);

        LuaTable armModelCfg = ConfigManager.Instance.Common.GetModelConfig(weaponModelId);
        if (null != armModelCfg)
        {
            rolePrefab = CoreEntry.gResLoader.Load(armModelCfg.Get<string>("skl")) as GameObject;
            if (null != rolePrefab)
            {
                GameObject weaponObj = (GameObject)Instantiate(rolePrefab);
                if (null != weaponObj)
                {
                    CommonTools.SetMainLayer(weaponObj, weaponModelId);
                    if (doubleBlade)
                    {
                        Transform hand = mRoleModel.transform.DeepFindChild("DM_R_Hand");
                        Transform weapon = weaponObj.transform.Find("DM_R_wuqi01");
                        if (null != hand && null != weapon)
                        {
                            weapon.SetParent(hand);
                            weapon.localPosition = Vector3.zero;
                            weapon.localRotation = Quaternion.identity;
                        }
                        else
                        {
                            if (null == hand)
                            {
                                LogMgr.UnityError(string.Format("modelID:{0} prefab:{1} has no point:DM_R_Hand", mainModelID, mModelPath));
                            }
                            if(null == weapon)
                            {
                                LogMgr.UnityError(string.Format("modelID:{0} prefab:{1} has no point:DM_R_wuqi01", weaponModelId, rolePrefab));
                            }
                        }

                        hand = mRoleModel.transform.DeepFindChild("DM_L_Hand");
                        weapon = weaponObj.transform.Find("DM_L_wuqi01");
                        if (null != hand && null != weapon)
                        {
                            weapon.SetParent(hand);
                            weapon.localPosition = Vector3.zero;
                            weapon.localRotation = Quaternion.identity;
                        }
                        else
                        {
                            if (null == hand)
                            {
                                LogMgr.UnityError(string.Format("modelID:{0} prefab:{1} has no point:DM_L_Hand", mainModelID, mModelPath));
                            }
                            if(null == weapon)
                            {
                                LogMgr.UnityError(string.Format("modelID:{0} prefab:{1} has no point:DM_L_wuqi01", weaponModelId, rolePrefab));
                            }
                        }

                        Destroy(weaponObj);
                    }
                    else
                    {
                        Transform hand = mRoleModel.transform.DeepFindChild("DM_R_Hand");
                        if(null != hand)
                        {
                            weaponObj.transform.SetParent(hand);
                            weaponObj.transform.localPosition = Vector3.zero;
                            weaponObj.transform.localRotation = Quaternion.identity;
                        }
                        else
                        {
                            LogMgr.UnityError(string.Format("modelID:{0} prefab:{1} has no point:DM_R_Hand", mainModelID, mModelPath));
                        }
                    }
                }
            }
        }

        LuaTable wingCfg = ConfigManager.Instance.Common.GetModelConfig(wingModelID);
        if (null != wingCfg)
        {
            rolePrefab = CoreEntry.gResLoader.Load(wingCfg.Get<string>("skl")) as GameObject;
            if (null != rolePrefab)
            {
                GameObject wingObj = (GameObject)Instantiate(rolePrefab);
                if (null != wingObj)
                {
                    CommonTools.SetMainLayer(wingObj, wingModelID);
                    Transform backTran = mRoleModel.transform.DeepFindChild("E_back");
                    wingObj.transform.parent = backTran;
                    wingObj.transform.localPosition = Vector3.zero;
                    wingObj.transform.localRotation = Quaternion.identity;
                    wingObj.SetActive(true);
                }
            }
        }

        mAnimator = mRoleModel.gameObject.GetComponent<Animation>();
        if (mAnimator.GetClip("enter001"))
        {
            mAnimator.Play("enter001");
            Invoke("Do3DModleStandbyAni", mAnimator["enter001"].clip.length);
            m_lastActionOverTime = mAnimator["enter001"].clip.length + Time.time;
            m_enterFinishTime = mAnimator["enter001"].clip.length + Time.time;
        }

        //shadow
        if (shadowPro == null)
        {
            if(SettingManager.Instance.PicSetting == 3)
            {
                UnityEngine.Object obj = CoreEntry.gResLoader.Load("Effect/common/fx_shadow_pro");
                if (obj == null)
                {
                    LogMgr.LogError("找不到 prefab: " + "Effect/common/fx_shadow_pro");
                    return;
                }
                shadowPro = GameObject.Instantiate(obj) as GameObject;
            }
        }
        if (shadowPro != null)
        {
            shadowPro.transform.localScale = Vector3.one;
            shadowPro.transform.localPosition = Vector3.zero;
            shadowPro.transform.localRotation = Quaternion.identity;

            CameraLock cl = shadowPro.GetComponent<CameraLock>();
            if (cl != null)
            {
                cl.m_TargetObj = mRoleModel;
            }
        }

        //UnityEngine.Object obj = CoreEntry.gResLoader.Load("Effect/common/fx_shadow");
        //if (obj == null)
        //{
        //    return;
        //}
        //GameObject blob = GameObject.Instantiate(obj) as GameObject;
        //blob.transform.parent = mRoleModel.transform;
        //blob.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //blob.transform.localPosition = Vector3.zero;

        mAudioSource = mRoleModel.GetComponent<AudioSource>();
        if (null == mAudioSource)
        {
            mAudioSource = mRoleModel.AddComponent<AudioSource>();
        }
        if (null != mAudioSource)
        {
            if (!CoreEntry.cfg_bEaxToggle)
            {
                return;
            }

            string path = AudioMgr.GetAudioPath(enterAudioID);
            if (path == null)
            {
                LogMgr.UnityError("无效音效ID:" + enterAudioID);
                return;
            }

            AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(path, typeof(AudioClip));
            if (clip == null)
            {
                LogMgr.UnityError("音效配置错误 路径:" + path + "  id" + enterAudioID);
                return;
            }

            mAudioSource.spatialBlend = 0f;
            mAudioSource.minDistance = 1.0f;
            mAudioSource.maxDistance = 5000;
            mAudioSource.rolloffMode = AudioRolloffMode.Linear;
            mAudioSource.clip = clip;
            mAudioSource.Play();

        }
    }

    public void Do3DModleStandbyAni()
    {
        if (mAnimator != null)
            mAnimator.Play(standy);
    }
    void OnDestroy()
    {
        CancelInvoke();
    }
}

