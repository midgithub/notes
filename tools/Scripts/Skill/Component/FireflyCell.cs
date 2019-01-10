using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

    //技能元素：子弹发射器
[Hotfix]
    public class FireflyCell : ISkillCell
    {
        private Transform m_transform;
        private SkillBase m_skillBase;
        //private GameDBMgr m_gameDBMgr;                
        //public SkillClassDisplayDesc m_skillClass;    
        private FireFlyAttackDesc m_fireFlyAttackDesc = null;

        //void Awake()
        //{               
        //    m_gameDBMgr = CoreEntry.gGameDBMgr;
        //}

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            //m_dataIndex = param.damageCellIndex;                            
            m_fireFlyAttackDesc = (FireFlyAttackDesc)cellData;
        }

        public override void Preload(ISkillCellData cellData, SkillBase skillBase)
        {
            Init(cellData, skillBase);
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
        }

        // Use this for initialization
        void Start()
        {
            CancelInvoke("Start");
            //m_skillBase = this.transform.root.gameObject.GetComponent<SkillBase>();
            if (m_skillBase == null || m_skillBase.m_actor == null)
                return;

            if (m_skillBase.m_onlyShowSkillScope)
                return;

            m_transform = m_skillBase.m_actor.transform;

            //Configs.skillConfig skillDesc = m_skillBase.m_actor.GetCurSkillDesc(m_skillBase.m_skillID);
            //m_skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(skillDesc.iID);

            //吟唱后，发射子弹
            float curTime = m_skillBase.GetCurActionTime();

            if (m_fireFlyAttackDesc.fireTime <= curTime)
            {
                Fire();
            }
            else
            {
                Invoke("Fire", m_fireFlyAttackDesc.fireTime - curTime);
            }
        }


        void Fire()
        {
            CancelInvoke("Fire");

            //屏震    不是主玩家 ， 就不震屏幕
            if (m_skillBase != null && m_skillBase.m_actor != null && m_skillBase.m_actor.gameObject != null)
            {
                if (m_skillBase.m_actor.IsMainPlayer() || m_skillBase.m_actor.mActorType == ActorType.AT_BOSS)
                {
                    if (m_fireFlyAttackDesc.cameraShakeDesc != null)
                    {
                        //GameObject cellObj = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(m_fireFlyAttackDesc.cameraShakeDesc.prefabPath));//CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(m_fireFlyAttackDesc.cameraShakeDesc.prefabPath);
                        GameObject cellObj = CoreEntry.gGameObjPoolMgr.InstantiateSkillCell(m_fireFlyAttackDesc.cameraShakeDesc.prefabPath);
                        cellObj.transform.parent = transform;

                        ISkillCell skillCell = cellObj.GetComponent<ISkillCell>();
                        skillCell.Init(m_fireFlyAttackDesc.cameraShakeDesc, m_skillBase);

                        m_skillBase.AddSkillCell(cellObj);
                    }
                }
            }

            //发射点位置
            Transform flyTransform = null;
            if (m_skillBase != null)
            {
                flyTransform = m_skillBase.FindChildTransform(m_fireFlyAttackDesc.firePos);
            }
            if (flyTransform == null)
            {
                LogMgr.UnityError("FireflyCell没有找到绑定点 \"" + m_fireFlyAttackDesc.firePos + "\"");
                return;
            }
            if (m_skillBase == null) return;
            // float diffAngle = 360f / m_fireFlyAttackDesc.bulletNum;
            float diffAngle = m_fireFlyAttackDesc.fAngle;
            for (int i = 0; i < m_fireFlyAttackDesc.bulletNum; ++i)
            {
                //加载子弹        
                GameObject objBullet =
                    (GameObject)Instantiate(CoreEntry.gResLoader.LoadResource(m_fireFlyAttackDesc.bulletPrefab));
                if (objBullet == null)
                {
                    return;
                }

                objBullet.transform.position = flyTransform.position;

                Vector3 targetpos = new Vector3(0, 0, 0);

                // 下面这句有时候会造成弓箭兵 射出的箭向后飞，目标错乱
                //ActorObj actorObj = m_skillBase.m_actor.GetAttackObj();
                //if (actorObj == null)
                //    actorObj = m_skillBase.m_hateActor;

                ActorObj actorObj = m_skillBase.GetSelTarget();

                if (actorObj != null && m_fireFlyAttackDesc.bulletNum == 1)
                {
                    //单个目标，指向目标方向
                    //Vector3 aimPos = new Vector3(actorObj.thisGameObject.transform.position.x,
                    //    objBullet.transform.position.y, actorObj.thisGameObject.transform.position.z);

                    //aimPos = Vector3.Normalize(aimPos);

                    //objBullet.transform.LookAt(aimPos);

                    //Vector3 aimPos = new Vector3(actorObj.thisGameObject.transform.position.x,
                    //    objBullet.transform.position.y, actorObj.thisGameObject.transform.position.z);

                    Vector3 aimPos = new Vector3();
                    aimPos = actorObj.thisGameObject.transform.position;
                    aimPos.y = actorObj.GetBehitEfxPosition().y;

                    Vector3 lookRot = aimPos - flyTransform.position;

                    // 穿透的子弹不需要考虑高度
                    if (m_fireFlyAttackDesc.bThroughFlag)
                        lookRot.y = 0;  // 设置为0时 高度不同时就打不到了

                    if (lookRot == Vector3.zero)
                    {
                        return;
                    }
                    lookRot.Normalize();
                    objBullet.transform.rotation = Quaternion.LookRotation(lookRot);

                    targetpos = aimPos;
                }
                else
                {
                    if (i == 0)
                    {
                        //朝向
                        Quaternion rot =
                            Quaternion.Euler(m_transform.rotation.eulerAngles.x,
                                m_transform.rotation.eulerAngles.y + diffAngle * i, m_transform.rotation.eulerAngles.z);

                        objBullet.transform.rotation = rot;

                    }


                    if (i % 2 == 0)
                    {
                        //朝向
                        Quaternion rot =
                            Quaternion.Euler(m_transform.rotation.eulerAngles.x,
                                m_transform.rotation.eulerAngles.y + diffAngle * (i + 1) * 0.5f, m_transform.rotation.eulerAngles.z);

                        objBullet.transform.rotation = rot;

                    }
                    else
                    {
                        //朝向
                        Quaternion rot =
                            Quaternion.Euler(m_transform.rotation.eulerAngles.x,
                                m_transform.rotation.eulerAngles.y - diffAngle * (i + 1) * 0.5f, m_transform.rotation.eulerAngles.z);

                        objBullet.transform.rotation = rot;

                    }

                }

                // 弹弹乐
                TanTanLe tantan = objBullet.GetComponent<TanTanLe>();
                if (tantan != null)
                {
                    BulletParam param = new BulletParam();
                    param.flySpeed = m_fireFlyAttackDesc.flySpeed;
                    param.flyTime = m_fireFlyAttackDesc.flyTime;
                    param.skillID = m_skillBase.m_skillID;
                    param.castObj = m_skillBase.m_actor.thisGameObject;
                    param.damageActorTypeList = m_fireFlyAttackDesc.aimActorTypeList;

                    param.bThroughFlag = m_fireFlyAttackDesc.bThroughFlag;
                    param.projectile = m_fireFlyAttackDesc.projectile;
                    param.TanTanLeCount = m_fireFlyAttackDesc.TanTanLeCount;
                    param.TanTanLeDis = m_fireFlyAttackDesc.TanTanLeDis;
                    param.dizzyTime = m_fireFlyAttackDesc.dizzyTime;

                    param.targetpos = targetpos;
                    if (m_skillBase != null && m_skillBase.m_actor != null)
                    {
                        ActorObj actorobj = m_skillBase.m_hitActor;
                        if (actorobj != null)
                            param.aimTransform = actorobj.transform;
                        else
                        {
                            actorObj = m_skillBase.m_actor.GetAttackObj();
                            if (actorObj == null)
                                actorObj = m_skillBase.m_hitActor;
                            if (actorobj != null)
                                param.aimTransform = actorobj.transform;
                            {
                                ActorObj gameobj = m_skillBase.GetSelTarget();
                                if (gameobj != null)
                                    param.aimTransform = gameobj.transform;
                            }
                        }
                    }
                    tantan.Init(param);

                }
                else // 子弹
                {
                    Bullet bullet = objBullet.GetComponent<Bullet>();
                    BulletParam param = new BulletParam();

                    param.m_skillBase = m_skillBase;

                    param.flySpeed = m_fireFlyAttackDesc.flySpeed;
                    param.flyTime = m_fireFlyAttackDesc.flyTime;
                    param.skillID = m_skillBase.m_skillID;
                    param.castObj = m_skillBase.m_actor.thisGameObject;
                    param.damageActorTypeList = m_fireFlyAttackDesc.aimActorTypeList;

                    param.bThroughFlag = m_fireFlyAttackDesc.bThroughFlag;
                    param.projectile = m_fireFlyAttackDesc.projectile;
                    param.targetpos = targetpos;
                    param.bTakeAwayTarget = m_fireFlyAttackDesc.bTakeAwayTarget;
                    param.disappearWhenTouchWall = m_fireFlyAttackDesc.disappearWhenTouchWall;
                    param.bCanNotMoveWhenTakenAway = m_fireFlyAttackDesc.bCanNotMoveWhenTakenAway;
                    param.bTraceTarget = m_fireFlyAttackDesc.bTraceTarget;
                    param.bAttachToSkill = m_fireFlyAttackDesc.bAttachToSkill;
                    param.bAttackMoreThanOnce = m_fireFlyAttackDesc.bAttackMoreThanOnce;
                    param.bEndTheSkillWhenDisappear = m_fireFlyAttackDesc.bEndTheSkillWhenDisappear;
                    bullet.Init(param);
                }
            }
        }

        public override void ShowSkillScope()
        {
            //伤害计算
            Vector3 pos = m_skillBase.m_actor.transform.position;
            if (null == m_skillBase.m_actor.m_WarningefxObj)
            {
                if (m_skillBase.m_actor.m_WarningefxObj != null)
                {
                    Destroy(m_skillBase.m_actor.m_WarningefxObj);
                    m_skillBase.m_actor.m_WarningefxObj = null;
                }
                //float fbig = 0.20f;   //贴花比实际伤害范围大百分之fbig
                //else if (DamageTypeID.DTID_RECT == m_oneDamageInfo.type)
                {
                    m_skillBase.m_actor.m_WarningefxObj = Instantiate(
                        CoreEntry.gResLoader.LoadResource("Effect/skill/remain/fx_yujing_changfanglan")) as GameObject;
                    
                    if(m_skillBase.m_actor.m_WarningefxObj == null)return;
                    
                    m_skillBase.m_actor.m_WarningefxObj.transform.localScale = new Vector3(m_fireFlyAttackDesc.displayWidth, 0f, m_fireFlyAttackDesc.displayLength);
                    m_skillBase.m_actor.m_WarningefxObj.transform.forward = m_skillBase.m_actor.transform.forward;
                    m_skillBase.m_actor.m_WarningefxObj.transform.position = new Vector3(pos.x, pos.y + 0.35f, pos.z);

                }
            }
        }

    }

};  //end SG

