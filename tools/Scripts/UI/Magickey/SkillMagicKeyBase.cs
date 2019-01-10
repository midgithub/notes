using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XLua;

namespace SG
{
[Hotfix]
    public class SkillMagicKeyBase : MonoBehaviour
    {
        public ActorObj m_actor = null;
        public GameObject efxObj = null;
        public LuaTable m_skillDesc = null;
        //动作特效
        EfxAttachActionPool m_actionEfx = null;
        //带有挂点的特效
        List<GameObject> m_attachEfxObjectlist = new List<GameObject>();
        private int m_skillID;

         public void Init(ActorObj attackObj,int skillID)
        {
            m_skillID = skillID;
            m_actor = attackObj;
            m_attachEfxObjectlist.Clear();
            m_skillDesc = m_actor.GetCurSkillDesc(m_skillID);
            LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillID);
            if(skill_action!=null)
                Invoke("HideSkillScope", skill_action.Get<float>("skillEfxLength"));
        }

        public static void CreateMagicKey(ActorObj attackObj, int skillID)
         {
             GameObject efxObj = null;
             LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(skillID);
             if (skill_action != null)
             {
                 object obj = CoreEntry.gResLoader.LoadResource(skill_action.Get<string>("skilleffect"));
                 if (obj == null)
                 {
                    LogMgr.LogError("找不到attackEfxPrefab：" + skill_action.Get<string>("skilleffect"));
                 }

                 efxObj = Instantiate((GameObject)obj) as GameObject;//CoreEntry.gGameObjPoolMgr.InstantiateEffect(param.actionEfx);
                if (efxObj != null)
                {
                    efxObj.transform.parent = attackObj.transform;

                    efxObj.transform.localPosition = Vector3.zero;
                    //efxObj.transform.localRotation = Vector3.zero;
                    efxObj.transform.localScale = Vector3.one;
                }
            }
             if (efxObj != null)
             {
                SkillMagicKeyBase mk =  efxObj.GetComponent<SkillMagicKeyBase>();
                if (mk == null)
                   mk = efxObj.AddComponent<SkillMagicKeyBase>();
                mk.Init(attackObj, skillID);
             }
             
             LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
             if(null != skillCfg && skillCfg.Get<int>("showtype") == (int)SkillShowType.ST_MAGICKEY)
             {
                 EventParameter param = EventParameter.Get();
                 param.intParameter = skillCfg.Get<int>("showtype");
                 param.stringParameter = skillCfg.Get<string>("name");
                 CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FLY_SKILL, param);
             }
         }
         
        //播放动画，特效，声音                
        void PlayActionEfxSound()
        { 
            return;
            /*
            Configs.skill_actionConfig skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillID);

            efxObj = this.gameObject;

            float maxEfxTime = 0;
            bool isFollowMove = true;

            if (skill_action.skillEfxLength > 0)
            {
                maxEfxTime = skill_action.skillEfxLength;
            }
            EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
            if (efx == null)
                efx = efxObj.AddComponent<EfxAttachActionPool>();

            if (efx != null)
            {
                efx.Init(m_actor.transform, maxEfxTime, isFollowMove);

                m_actionEfx = efx;

                if (m_actionEfx != null)
                {
                    m_actionEfx.transform.parent = m_actor.transform;
                    m_actionEfx.transform.localPosition = Vector3.zero;

                }

                //设置有挂点的特效            
                Transform[] childTransform = efxObj.GetComponentsInChildren<Transform>();
                //foreach (Transform childTrans in childTransform)
                for (int i = 0; i < childTransform.Length; ++i)
                {
                    Transform childTrans = childTransform[i];
                    EfxSetAttachPoint setAttach = childTrans.gameObject.GetComponent<EfxSetAttachPoint>();
                    if (setAttach == null || setAttach.m_attachPointEnum == AttachPoint.E_None)
                    {
                        continue;
                    }

                    setAttach.Init(false);

                    Transform parent = m_actor.GetChildTransform(setAttach.m_attachPointEnum.ToString());
                    if (parent != null)
                    {
                        childTrans.parent = parent;
                        childTrans.localPosition = Vector3.zero;
                        childTrans.localRotation = Quaternion.identity;
                        childTrans.localScale = Vector3.one;

                        m_attachEfxObjectlist.Add(childTrans.gameObject);
                    }
                }
            }
            */
            //yield return 1;
        }

        void DestroyEfx()
        {
            if (m_actionEfx != null)
            {
                m_actionEfx.DetachObject(); 
            }

            for (int i = 0; i < m_attachEfxObjectlist.Count; ++i)
            {
                Destroy(m_attachEfxObjectlist[i].gameObject);
                //CoreEntry.gGameObjPoolMgr.Destroy(m_attachEfxObjectlist[i].gameObject);
            }
            m_attachEfxObjectlist.Clear();
        }

        public void HideSkillScope()
        { 
            //DestroyEfx();
            Destroy(this.gameObject);
            //CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);
        }

    }

};  //end SG

