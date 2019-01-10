/**
* @file     : ActionCell.cs
* @brief    : 
* @details  : 播放动作，声音，特效元素
* @author   :  
* @date     : 2014-12-30
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

[Hotfix]
    public class ActionCell : ISkillCell
    {
        private ActionCellDesc m_actionCellDesc = null;
        SkillBase m_skillBase = null;
        ActorObj m_actor = null;
        public GameObject m_WarningefxObj = null;

        //当前动作特效脚本
        EfxAttachActionPool m_actionEfx = null;

        //带有挂点的特效
        List<GameObject> m_attachEfxObjectlist = null;

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            m_actionCellDesc = (ActionCellDesc)cellData;
        }

        //接受事件
        public override void OnEvent(SkillCellEventType eventType, params ValueArg[] valueArgs)
        {
            switch (eventType)
            {
                case SkillCellEventType.SE_DESTROY_ACTION_EFX:
                    {
                        if (m_actionEfx != null)
                        {
                            m_actionEfx.DestoryObject();
                            m_actionEfx = null;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        // PoolManager
        void OnEnable()
        {
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        void OnDisable()
        {
            CancelInvoke("Start");
            if (m_WarningefxObj != null)
            {
                m_WarningefxObj.SetActive(false);
            }
        }


        void Start()
        {
            CancelInvoke("Start");

            if (m_skillBase == null || m_skillBase.m_actor == null)
                return;

            //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();
            m_actor = m_skillBase.m_actor;


            if (m_actionCellDesc.delayTime > 0)
            {
                Invoke("DoAction", m_actionCellDesc.delayTime);
            }
            else
            {
                DoAction();
            }

            //这个肯定会用预警
            if (0 != m_actionCellDesc.efxWarning.Length)
            {
                //if (m_WarningefxObj != null)
                //{
                //    Destroy(m_WarningefxObj);
                //    m_WarningefxObj = null;
                //}
                if (m_WarningefxObj == null)
                {
                    m_WarningefxObj = Instantiate(
                        CoreEntry.gResLoader.LoadResource(m_actionCellDesc.efxWarning)) as GameObject;
                }
                if(m_WarningefxObj == null)return;
                m_WarningefxObj.SetActive(true);

                //m_efxObj.transform.position = m_skillBase.m_actor.m_transform.position;
                Vector3 pos = m_skillBase.m_actor.transform.position;
                if (m_actionCellDesc.shouldAttachToOwner == false)
                {
                    pos = m_skillBase.transform.position;
                }
                pos = CoreEntry.gBaseTool.GetGroundPoint(pos);
                pos.y += 0.3f;


                ParticleScaler ScaleComponet = m_WarningefxObj.GetComponent<ParticleScaler>();
                if (ScaleComponet == null)
                    ScaleComponet = m_WarningefxObj.AddComponent<ParticleScaler>();
                // ScaleComponet.particleScale = m_actor.actorCreatureDisplayDesc.sacle ;


                float fbig = 0.10f;   //贴花比实际伤害范围大百分之fbig
                if (DamageTypeID.DTID_FUN == m_actionCellDesc.type)
                {
                    //m_WarningefxObj.transform.localScale = new Vector3(1.0f, 0f, 1.0f) * m_oneDamageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);
                    // ScaleComponet.prevScale = m_oneDamageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);
                    ScaleComponet.particleScale = m_actionCellDesc.damageNode.funDamage.radius * (1 + fbig);//m_oneDamageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);
                                                                                                            //圆形按半径算。所以乘以2
                    if (m_actionCellDesc.damageNode.funDamage.angle == 360)
                    {
                        ScaleComponet.particleScale *= 2.0f;
                    }
                    if (m_actionCellDesc.damageNode.funDamage.angle <= 360)
                    {

                        m_WarningefxObj.transform.position = new Vector3(pos.x, pos.y + 0.1f, pos.z);

                        m_WarningefxObj.transform.forward = m_skillBase.m_actor.transform.forward;

                    }

                    if (m_actionCellDesc.damageNode.rectDamage.offDistance != 0)
                    {
                        m_WarningefxObj.transform.localPosition += m_skillBase.m_actor.transform.forward * m_actionCellDesc.damageNode.rectDamage.offDistance;

                    }

                    //if (m_actionCellDesc.damageNode.rectDamage.offDistanceX != 0)
                    //{
                    //    m_WarningefxObj.transform.localPosition += m_skillBase.m_actor.transform.right * m_actionCellDesc.damageNode.rectDamage.offDistanceX;

                    //}
                }
                else if (DamageTypeID.DTID_RECT == m_actionCellDesc.type)
                {
                    m_WarningefxObj.transform.localScale = new Vector3((1 + fbig) * m_actionCellDesc.damageNode.rectDamage.width, 0f, (1 + fbig) * m_actionCellDesc.damageNode.rectDamage.length);  //new Vector3(1.0f, 0f, 1.0f) * m_actionCellDesc.damageNode.rectDamage.length;  // m_oneDamageInfo.damageNode.funDamage.radius * 2 * (1 + fbig);

                    m_WarningefxObj.transform.forward = m_skillBase.m_actor.transform.forward;

                    m_WarningefxObj.transform.position = pos;

                    if (m_actionCellDesc.damageNode.rectDamage.offDistance != 0)
                    {
                        m_WarningefxObj.transform.localPosition += m_skillBase.m_actor.transform.forward * m_actionCellDesc.damageNode.rectDamage.offDistance;

                    }

                    //if (m_actionCellDesc.damageNode.rectDamage.offDistanceX != 0)
                    //{
                    //    m_WarningefxObj.transform.localPosition += m_skillBase.m_actor.transform.right * m_actionCellDesc.damageNode.rectDamage.offDistanceX;

                    //}
                    //m_WarningefxObj.transform.localPosition -= new Vector3(0, 0,  m_oneDamageInfo.damageNode.rectDamage.length * 0.5f);
                    //m_WarningefxObj.transform.position += m_oneDamageInfo.damageNode.rectDamage.length / 2 * m_skillBase.m_actor.transform.forward;
                }


                //  Invoke("HideDecal", m_actionCellDesc.);
            }

        }

        void DoAction()
        {
            //播放动作，特效    
            if (m_actionCellDesc.name.Length > 0)
            {
                m_actor.PlayAction(m_actionCellDesc.name, false);
                m_actor.SetActionTime(m_actionCellDesc.name, m_actionCellDesc.setStartTime);
                m_actor.SetActionSpeed(m_actionCellDesc.name, m_actionCellDesc.speed);
            }

            //播放声音
            string sound1 = "";
            string sound2 = "";
            bool ret1 = AudioCore.GenerateAudio(m_actionCellDesc.sound1, ref sound1);
            bool ret2 = AudioCore.GenerateAudio(m_actionCellDesc.sound2, ref sound2);

            if (ret1 && sound1.Length > 0)
            {
                m_actor.StopSound();
                m_actor.PlaySound(sound1);
            }

            if (ret2 && sound2.Length > 0)
            {
                m_actor.StopSound2();
                m_actor.PlaySound2(sound2);
            }

            if (m_actionCellDesc.efx.Length > 0)
            {
                //GameObject efxObj = Instantiate(
                //    CoreEntry.gResLoader.LoadResource(m_actionCellDesc.efx)) as GameObject;
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(m_actionCellDesc.efx);

                float maxEfxTime = 0;
                NcCurveAnimation[] efxAnimations = efxObj.GetComponentsInChildren<NcCurveAnimation>();
                for (int i = 0; i < efxAnimations.Length; ++i)
                {
                    efxAnimations[i].m_fDelayTime /= m_actionCellDesc.speed;
                    efxAnimations[i].m_fDurationTime /= m_actionCellDesc.speed;

                    float efxTime = efxAnimations[i].m_fDelayTime + efxAnimations[i].m_fDurationTime;
                    if (efxTime > maxEfxTime)
                    {
                        maxEfxTime = efxTime;
                    }
                }

                LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillBase.m_skillID);
                if (skill_action != null && skill_action.Get<float>("skillEfxLength") > 0)
                {
                    maxEfxTime = skill_action.Get<float>("skillEfxLength");
                }

                //LogMgr.UnityLog("maxEfxTime=" + maxEfxTime + ", " + m_strActionName);

                //特效存在时间
                if (maxEfxTime <= 0.001)
                {
                    maxEfxTime = 5;
                }

                EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
                if (efx == null)
                {
                    efx = efxObj.AddComponent<EfxAttachActionPool>();
                }

                m_actionEfx = efx;

                if (m_actionCellDesc.shouldAttachToOwner)
                {
                    efx.Init(m_skillBase.m_actor.transform, maxEfxTime);

                    //设置有挂点的特效            
                    Transform[] childTransform = efxObj.GetComponentsInChildren<Transform>();
                    foreach (Transform childTrans in childTransform)
                    {
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

                            if (m_attachEfxObjectlist == null)
                            {
                                m_attachEfxObjectlist = new List<GameObject>();
                            }
                            m_attachEfxObjectlist.Add(childTrans.gameObject);
                        }
                    }

                    //影子
                    ghostMesh[] ghostMesh = efx.GetComponentsInChildren<ghostMesh>();

                    SkinnedMeshRenderer MianSkinMesh = m_actor.m_skinnedMeshRenderer[0];
                    for (int i = 0; i < m_actor.m_skinnedMeshRenderer.Length; ++i)
                    {
                        if (m_actor.m_skinnedMeshRenderer[i].name.Contains("weapon"))
                        {
                            continue;
                        }
                        MianSkinMesh = m_actor.m_skinnedMeshRenderer[i];
                    }

                    for (int i = 0; i < ghostMesh.Length; ++i)
                    {
                        ghostMesh[i].characterMesh[0] = MianSkinMesh;

                    }
                }
                else
                {
                    efx.Init(m_skillBase.transform, maxEfxTime, false);
                    efx.transform.parent = m_skillBase.transform;
                    BaseTool.ResetTransform(efx.transform);

                }
            }
        }

        //删除特效
        void OnDestroy()
        {
            if (m_actionEfx != null)
            {
                //退出后，自己处理
                m_actionEfx.DetachObject();
            }

            if (m_WarningefxObj != null)
            {
                Destroy(m_WarningefxObj);
                m_WarningefxObj = null;
            }

            if (m_attachEfxObjectlist != null)
            {
                for (int i = 0; i < m_attachEfxObjectlist.Count; ++i)
                {
                    Destroy(m_attachEfxObjectlist[i].gameObject);
                }
                m_attachEfxObjectlist.Clear();
            }
        }

        public override void Preload(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            m_actionCellDesc = (ActionCellDesc)cellData;

            if (m_actionCellDesc.efx.Length > 0)
            {
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(m_actionCellDesc.efx);
                CoreEntry.gGameObjPoolMgr.Destroy(efxObj);
            }

            //if (0 != m_actionCellDesc.efxWarning.Length)
            //{
            //    m_WarningefxObj = Instantiate(
            //      CoreEntry.gResLoader.LoadResource(m_actionCellDesc.efxWarning)) as GameObject;
            //}

        }

    }
}

