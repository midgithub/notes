using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    //技能元素：曲线运动
[Hotfix]
    public class CurveMoveCell : ISkillCell
    {
        private MovePosAttackDesc m_movePosAttackDesc = null;
        CurveMoveParam m_param = null;
        SkillBase m_skillBase = null;
        int m_uuid = -1;


        private Transform m_transform;
        //private GameLogicMgr m_gameManager;
        //private GameDBMgr m_gameDBMgr;
        private BaseTool m_baseTool;

        private int m_skillID;
        private bool m_needMove = false;


        //能否修改移动距离
        public bool m_canResetDistance = false;
        public float m_distance = 0;

        Vector3 m_dstPos = Vector3.zero;


        void Awake()
        {
            m_transform = this.transform;

            //m_gameManager = CoreEntry.gGameMgr;
            //m_gameDBMgr = CoreEntry.gGameDBMgr;
            m_baseTool = CoreEntry.gBaseTool;
        }

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            m_movePosAttackDesc = (MovePosAttackDesc)cellData;

            m_param = null;
            m_uuid = -1;
            m_needMove = false;
            m_canResetDistance = false;
            m_distance = 0;
            m_dstPos = Vector3.zero;
        }

        //接受事件
        public override void OnEvent(SkillCellEventType eventType, params ValueArg[] valueArgs)
        {
            switch (eventType)
            {
                case SkillCellEventType.SE_STOP_CARRYOFF_TARGET:
                    {
                        if (m_param != null)
                        {
                            m_param.isCarryOffTarget = false;
                        }
                    }
                    break;
            }
        }

        // PoolManager
        void OnEnable()
        {
            CancelInvoke("Start");
            Invoke("Start", 0.000001f);
        }

        void Start()
        {
            CancelInvoke("Start");

            if (m_skillBase == null || m_skillBase.m_actor == null)
                return;

            m_skillBase.m_actor.ReqirueCanNotBeControlledByInput();

            //主角在电梯上禁止美术位移
            if (CoreEntry.gSceneMgr.IsPlayerOnElevator())
            {
                return;
            }

            if (m_movePosAttackDesc == null)
            {
                return;
            }
            
            LuaTable skillDesc = m_skillBase.m_actor.GetCurSkillDesc(m_skillBase.m_skillID);
            if (skillDesc == null)
            {
                LogMgr.UnityError("non SkillDesc! " + m_skillBase.m_skillID);
                return;
            }

            //SkillClassDisplayDesc skillClass = m_gameDBMgr.GetSkillClassDisplayDesc(skillDesc.Get<int>("skillDisplayID"));

            //计算位移距离
            m_distance = m_movePosAttackDesc.moveDistance;

            //使用目标的位置, 前面0.5米

            if (m_skillBase.m_actor.IsMainPlayer())
            {
                if (m_skillBase.m_actor.m_SelectTargetObject != null)
                {
                    float dist1 = Vector3.Distance(transform.position, m_skillBase.m_actor.m_SelectTargetObject.transform.position) - 0.5f;
                    if (dist1 > m_distance)
                    {
                        dist1 = m_distance;
                    }

                    m_distance = dist1;
                }

            }

            float resetDistance = m_distance;

            //设置位移  
            do
            {
                ActorObj ActorObj = m_skillBase.m_actor.GetAttackObj();
                if (ActorObj == null)
                    ActorObj = m_skillBase.m_hitActor;

                if (ActorObj == null)
                {
                    LogMgr.UnityLog("CurveMove:no target");
                    break;
                }

                GameObject targetObj = ActorObj.thisGameObject;
                if (targetObj == null)
                {
                    LogMgr.UnityLog("CurveMove:no target");
                    break;
                }

                ActorObj targetActorBase = targetObj.GetComponent<ActorObj>();
                if (targetActorBase == null)
                {
                    LogMgr.UnityError("CurveMove:no actorbase");
                    break;
                }


                if (m_movePosAttackDesc.isStopForTarget)
                {
                    float fDistTo = m_skillBase.m_actor.GetColliderRadius() + targetObj.GetComponent<ActorObj>().GetColliderRadius();

                    if (CoreEntry.gGameMgr.IsPvpState())
                    {
                        fDistTo += 1.2f;
                    }


                    //不可以穿透，重置位移大小                          
                    float aimDistance = Vector3.Distance(m_transform.position, targetObj.transform.position) - fDistTo - 0.2f;    //两个物体间的距离

                    //范围内，不需要位移
                    if (aimDistance <= 0)
                    {
                        //Debug.LogWarning("CurveMove: don't need excute!");
                        return;
                    }

                    resetDistance = Mathf.Min(aimDistance, resetDistance);
                }
                else
                {
                    resetDistance = m_distance;
                }
            } while (false);



            Quaternion r0 = Quaternion.Euler(m_transform.eulerAngles);
            Vector3 pos = m_transform.position +
                r0 * Vector3.forward * resetDistance;

            //找到对应地面上的点
            Vector3 aimPos = m_baseTool.GetGroundPoint(pos);
            if (aimPos.Equals(Vector3.zero))
            {
                return;
            }

            Vector3 dstDir = aimPos - m_transform.position;
            dstDir.Normalize();

            //左手方向
            //Vector3 leftDir = Quaternion.Euler(new Vector3(0, 90, 0)) * dstDir;
            //Vector3 rightDir = Quaternion.Euler(new Vector3(0, -90, 0)) * dstDir;
            //Vector3 backDir = Quaternion.Euler(new Vector3(0, 180, 0)) * dstDir;

            //float dstDistance = Vector3.Distance(m_transform.position, aimPos);
            m_needMove = true;

            //向上移动点
            //aimPos += new Vector3(0, 0.2f, 0);
            aimPos = m_baseTool.GetLineReachablePos(m_transform.position, aimPos);
            aimPos = m_baseTool.GetGroundPoint(aimPos);

            if (m_skillBase.m_actor.IsMainPlayer())
            {
                m_dstPos = aimPos;
            }
            else
            {
                m_dstPos = m_skillBase.m_actor.m_CurveMovePos;
            }
            
            //使用曲线
            CurveMoveParam param = new CurveMoveParam();
            param.aimActorTypeList = m_movePosAttackDesc.resetAimActorTypeList;
            param.isCarryOffTarget = m_movePosAttackDesc.isCarryOffTarget;
            param.isStopForTarget = m_movePosAttackDesc.isStopForTarget;

            LuaTable skill_action = CoreEntry.gSkillMgr.GetSkillActon(m_skillBase.m_skillID);
            if(skill_action!=null)
            m_uuid = m_skillBase.m_actor.UseCurveData3(skill_action.Get<string>("animation"), m_dstPos,// m_dstPos,
                param);

            m_param = param;

            //拖尾特效
            if (m_movePosAttackDesc.efxPrefab.Length > 0)
            {
                //GameObject efxObj = Instantiate(CoreEntry.gResLoader.LoadResource(m_movePosAttackDesc.efxPrefab)) as GameObject;
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(m_movePosAttackDesc.efxPrefab);

                EfxAttachActionPool efx = efxObj.GetComponent<EfxAttachActionPool>();
                if (efx == null)
                    efx = efxObj.AddComponent<EfxAttachActionPool>();

                float efxTime = 1.0f;
                if (skill_action != null)
                    m_skillBase.m_actor.GetLeftActionTime(skill_action.Get<string>("animation"));

                efx.Init(m_skillBase.m_actor.transform, efxTime);
            }
        }


        // Update is called once per frame
        void LateUpdate()
        {
            if (m_needMove)
            {
                //同步角色位移
                m_skillBase.Syncm_transform();
            }
        }

        void OnDisable()
        {
            //主角在电梯上禁止美术位移
            if (CoreEntry.gSceneMgr.IsPlayerOnElevator())
            {
                return;
            }

            if (m_skillBase == null || m_skillBase.m_actor == null)
            {
                return;
            }

            m_skillBase.m_actor.ReleaseCanNotBeControlledByInput();

            m_skillBase.m_actor.ExitAnimationCurveMove(m_uuid);

            CancelInvoke("Start");
        }

        void OnDestroy()
        {
            OnDisable();
        }

        public override void Preload(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            m_movePosAttackDesc = (MovePosAttackDesc)cellData;
            if (m_movePosAttackDesc.efxPrefab.Length > 0)
            {
                GameObject efxObj = CoreEntry.gGameObjPoolMgr.InstantiateEffect(m_movePosAttackDesc.efxPrefab);
                CoreEntry.gGameObjPoolMgr.Destroy(efxObj);

            }
        }
    }
}

