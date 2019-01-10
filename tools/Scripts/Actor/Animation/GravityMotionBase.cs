/**
* @file     : GravityMotionBase.cs
* @brief    : 
* @details  : 重力运动系统
* @author   : 
* @date     : 2014-10-31
*/

using UnityEngine;
using System.Collections;
using XLua;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif
namespace SG
{


[Hotfix]
    public class GravityMotionBase : MonoBehaviour
    {
        private Transform m_transform = null;

        //初始速度，方向
        private struct Velocity
        {
            public float value;     //值      
            public float angle;     //跟水平轴的夹角

            public float vSubY;        //y轴速度分量
            public float vSubZ;        //z轴速度分量
        };

        private Velocity m_originVelocity;
        private float m_startMotionTime = 0;
        private Vector3 m_startPos = Vector3.zero;
        //private bool m_isInTopPoint = false;

        //重力加速度
        private static float G_ACCE = 8;

        //阻力加速度
        private float F_ACCE = 0;

        //是否启用
        private bool m_isUseGravity = false;

        private ActorObj m_actor = null;
        private BehitState m_BehitState = null;

        //被击信息
        private BehitParam m_behitParame = null;

        private GameDBMgr m_gameDataBase = null;

        private int nCount = 0;


        //上升，下落，平移，下抛
        private enum GravityMotionType
        {
            GMT_NONE = 0,
            GMT_UP,
            GMT_DOWN,
            GMT_PUSH,
        };

        private GravityMotionType m_gravityMotionType = GravityMotionType.GMT_NONE;

        //攻击者朝向        
        private Vector3 m_attackObjForward = Vector3.zero;

        //开始浮空位置    
        private Vector3 m_startToSkyPos = Vector3.zero;

        //private int m_wallLayerMask = 0;
        private int m_groundLayerMask = 0;
        float m_curRadius = 0;

        public bool isUseGravityState
        {
            get { return m_isUseGravity; }
        }

        void Awake()
        {
            m_transform = this.transform;

            m_originVelocity = new Velocity();

            //m_wallLayerMask = 1 << LayerMask.NameToLayer("wall");
            m_groundLayerMask = 1 << LayerMask.NameToLayer("ground");
        }

        public void Init()
        {
            m_gameDataBase = CoreEntry.gGameDBMgr;

            m_actor = this.gameObject.GetComponent<ActorObj>();
            m_BehitState = m_actor.behitState;

            m_isUseGravity = false;
            m_gravityMotionType = GravityMotionType.GMT_NONE;

            m_attackObjForward = Vector3.zero;
            m_startToSkyPos = Vector3.zero;

            F_ACCE = 0;

            nCount = 0;

            m_curRadius = m_actor.GetColliderRadius();

            CancelInvoke("AutoCancelStatic");
            CancelInvoke("MoveDistanceEnd");
        }

        // Update is called once per frame
        void Update()
        {
            if (m_actor == null)
            {
                return;
            }

            if (m_actor.IsDeath())
            {
                m_isUseGravity = false;
                return;
            }

            if (m_actor.IgnoredGravityMotion)
            {
                m_isUseGravity = false;
                return;
            }

            if (!m_isUseGravity)
            {
                nCount = 0;
                return;
            }

            float diffTime = Time.time - m_startMotionTime;

            //Y轴当前速度
            float vY = m_originVelocity.vSubY - (G_ACCE - F_ACCE) * diffTime;

            //Y轴位移
            float dY = (m_originVelocity.vSubY - 0.5f * (G_ACCE - F_ACCE) * diffTime) * diffTime;;
            if (m_gravityMotionType == GravityMotionType.GMT_DOWN)
            {
                dY = dY * 0.8f;
            }

            //Z轴位移
            float dZ = m_originVelocity.vSubZ * diffTime;

            Vector3 movePos = m_attackObjForward.normalized * dZ;

            //LogMgr.UnityLog("movePos="+movePos.ToString("f4")+", dir="+m_attackObjForward.normalized.ToString("f4"));
            Vector3 curPos = m_transform.position;
            Vector3 aimPos = m_startPos + new Vector3(movePos.x, dY, movePos.z);

            //m_transform.position = m_startPos + new Vector3(movePos.x, dY, movePos.z);

            bool isOverHeight = false;
            //float heigth = m_transform.position.y - m_startToSkyPos.y;
            float heigth = aimPos.y - m_startToSkyPos.y;
            if (heigth > 5)
            {
                //m_transform.position -= new Vector3(0, heigth-5, 0);
                aimPos -= new Vector3(0, heigth - 2, 0);
                isOverHeight = true;
            }

            //是否碰到墙壁       
            bool isToWall = false;
            if (!(Mathf.Abs(m_originVelocity.vSubZ) <= 0.0001))
            {
                Vector3 dir = m_attackObjForward;

                RaycastHit beHit;

                if (!BaseTool.instance.CanMoveToPos(curPos, aimPos, m_curRadius)
                    || Physics.Raycast(m_transform.position, dir, out beHit, m_curRadius + 0.5f, m_groundLayerMask))
                {
                    //碰到阻挡墙，直接下落
                    aimPos = curPos;
                    isToWall = true;
                }
            }

            //Vector3 groundPos = BaseTool.instance.GetGroundPoint(m_transform.position);
            //if (aimPos.y < groundPos.y)
            //{
            //    aimPos.y = groundPos.y + 0.01f;
            //}
            BaseTool.SetPosition(m_transform, aimPos);
            
            //到达最高点, 碰到墙壁
            if (isOverHeight || isToWall || (m_gravityMotionType == GravityMotionType.GMT_UP && vY <= 0.0001))
            {
                F_ACCE = 0;
                if (isOverHeight || isToWall)
                {
                    SetOriginV(1, 270);
                }
                else
                {
                    //有水平速度，保持不变
                    if (!(Mathf.Abs(m_originVelocity.vSubZ) <= 0.0001))
                    {
                        m_gravityMotionType = GravityMotionType.GMT_DOWN;
                        return;
                    }

                    m_actor.StopAll();
                    SetOriginV(0, 270);
                    m_actor.PlayAction("hit011", false);
                }
            }
        }

        void LateUpdate()
        {
            if (m_actor == null)
            {
                return;
            }

            if (m_actor.IsDeath())
            {
                m_isUseGravity = false;
                return;
            }

            if (!m_isUseGravity)
            {
                return;
            }

            if (IsStandInGround())
            {
                m_isUseGravity = false;

                //达到地面
                DoOnGround();
            }
        }

        //浮空受击
        public void DoBehit()
        {
            //浮空状态不能释放技能
            m_BehitState.isNonControl = true;

            //离地面高度
            float height = GetHeightToGround();
            if (height <= 0.1f)
            {
                return;
            }

            if (nCount > 2)
            {
                m_actor.StopAll();
                string clipName = "hit006";
                m_actor.PlayAction(clipName);

                m_actor.SetActionSpeed(clipName, 2f);

                m_actor.UseCurveData1(clipName, 2.5f);

                m_isUseGravity = false;
                //  ExitBehitState();

                if (!m_actor.IsHadAction(clipName))
                {
                    Vector3 vCurPos = m_actor.transform.position;
                    RaycastHit curHit;
                    //强拉到地面
                    if (Physics.Raycast(vCurPos, -Vector3.up, out curHit, 10, m_groundLayerMask))
                    {
                        vCurPos.y = curHit.point.y;
                    }

                    BaseTool.SetPosition(m_transform, vCurPos);
                }


                float actionLen = m_actor.GetActionLength(clipName);
                Invoke("ExitBehitState", actionLen);
                nCount = 0;
                return;

            }

            nCount++;

            m_behitParame = m_actor.damageBebitParam;

            int skillID = m_behitParame.damgageInfo.skillID;

            //ActorObj m_hitActorBase = m_behitParame.damgageInfo.attackActor;

            //只处理带位移的普通技能                
            //LuaTable skillDesc = m_hitActorBase.GetCurSkillDesc(skillID);
            char bodyType = (char)m_actor.BodyType;

            int weight = 1;

            //技能力度纠正        
            if (m_behitParame.damgageInfo.weight > 0)
            {
                weight = m_behitParame.damgageInfo.weight;
                LogMgr.UnityLog("Gravity dobehit skillid=" + skillID + ", reset weight=" + weight);
            }

            //获取技能受击反馈
            SkillBehitDisplayDesc behitDisplay = m_gameDataBase.GetSkillBehitDisplayDesc(weight, bodyType);
            if (behitDisplay == null)
            {
                return;
            }
            
            //没有硬直,定格，没有位移      
            if (!behitDisplay.isNonControl)
            {
                return;
            }

            //带位移        
            float moveDistance = 0;

            //动作                
            if (behitDisplay.behitType == BehitType.BT_NORMAL)
            {
                //普通受击
               // moveDistance = skillDesc.hitMoveDistance;
                moveDistance = 0.2f;
              
            }
            else if (behitDisplay.behitType == BehitType.BT_HITBACK)
            {
                //美术位移            
                if (!m_behitParame.damgageInfo.isNotUseCurveMove)
                {
                    string clipName = behitDisplay.actionList[0];
                    moveDistance = m_actor.GetAnimationCurveLength(clipName);

                    //LogMgr.UnityLog("moveDistance=" + moveDistance + ", clipName=" + clipName);    
                }
            }
            else if (behitDisplay.behitType == BehitType.BT_HITDOWN)
            {
                m_actor.StopAll();
                string clipName = behitDisplay.actionList[0];
                m_actor.PlayAction(clipName);
                m_actor.UseCurveData1(clipName, 2.5f);

                m_isUseGravity = false;
              //  ExitBehitState();

                if (!m_actor.IsHadAction(clipName))
                {
                    Vector3 vCurPos = m_actor.transform.position;
                    RaycastHit curHit;
                    //强拉到地面
                    if (Physics.Raycast(vCurPos, -Vector3.up, out curHit, 10, m_groundLayerMask))
                    {
                        vCurPos.y = curHit.point.y;
                    }

                    BaseTool.SetPosition(m_transform, vCurPos);
                }


                float actionLen = m_actor.GetActionLength(clipName);
                Invoke("ExitBehitState", actionLen);

                return;
            }


            else if (behitDisplay.behitType == BehitType.BT_HITSKY)
            {
                //浮空追击
                CancelInvoke("MoveDistanceEnd");
                CancelInvoke("AutoCancelStatic");

                F_ACCE = 0;
              //  SetOriginV(skillDesc.hitSkyOriginV, skillDesc.hitSkyAngle);

                m_actor.StopAll();
                m_actor.PlayAction("hit013", false);
                return;
            }

            //LogMgr.UnityLog("moveDistance=" + moveDistance); 


            if (height < 0.5f)
            {
                return;
            }


            //没有位移
            if (moveDistance <= 0.001 )
            {
                //F_ACCE = G_ACCE;
                SetOriginV(1, 90);
                m_actor.StopAll();
                m_actor.PlayAction("hit013", false);              
            }
            else
            {
                //当前的高度
                SetOriginV(3, 75);
                m_actor.StopAll();
                m_actor.PlayAction("hit013", false);                 
            }
        }

        void MoveDistanceEnd()
        {
            CancelInvoke("MoveDistanceEnd");

            //定格
            F_ACCE = G_ACCE;
            SetOriginV(0, 0);
        }

        void AutoCancelStatic()
        {
            CancelInvoke("AutoCancelStatic");
            F_ACCE = 0;

            SetOriginV(0, 270);

            if (m_isUseGravity)
            {
                m_actor.StopAll();
                m_actor.PlayAction("hit011", false);
            }
        }

        //浮空开始
        public void StartHitToSky(float value, float angle)
        {
            m_isUseGravity = true;

            F_ACCE = 0;
            SetOriginV(value, angle);

            m_actor.StopAll();


            if (m_actor.mActorType == ActorType.AT_LOCAL_PLAYER || m_actor.mActorType == ActorType.AT_PVP_PLAYER || m_actor.mActorType == ActorType.AT_BOSS)
            {
                m_actor.PlayAction("hit011", false);
            }
            else
            {
                m_actor.PlayAction("hit010", false);
            }


            m_startToSkyPos = m_transform.position;
        }

        //设置初速度
        void SetOriginV(float value, float angle)
        {
            //浮空追击，但是退出了重力系统
            if (!m_isUseGravity)
            {
                return;
            }

            angle = angle % 360;

            m_originVelocity.value = value;
            m_originVelocity.angle = angle;

            m_originVelocity.vSubY = m_originVelocity.value * Mathf.Sin(m_originVelocity.angle * Mathf.Deg2Rad);
            m_originVelocity.vSubZ = m_originVelocity.value * Mathf.Cos(m_originVelocity.angle * Mathf.Deg2Rad);

            m_startMotionTime = Time.time;

            m_startPos = m_transform.position;

            //根据角度确定是什么类型运动
            if (angle > 0 && angle < 180)
            {
                m_gravityMotionType = GravityMotionType.GMT_UP;
            }
            else if (angle == 0 || angle == 180)
            {
                m_gravityMotionType = GravityMotionType.GMT_PUSH;
            }
            else if (angle > 180 && angle < 360)
            {
                m_gravityMotionType = GravityMotionType.GMT_DOWN;
            }

            //navmesh disenable
            NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }

            //设置被击硬直   
            m_BehitState.isNonControl = true;

           

            m_behitParame = m_actor.damageBebitParam;

            if (m_behitParame == null)
            {
                return;
                //播放声音
              //  m_BehitState.PlayBehitSound(null, m_behitParame.damgageInfo.attackActor);
            }

            m_attackObjForward = m_behitParame.damgageInfo.attackActor.transform.forward;
        }

        //是否到地面了:速度方向向下，距离地面位移0
        bool IsStandInGround()
        {
            //不上升就需要检查
            if (m_gravityMotionType == GravityMotionType.GMT_UP)
            {
                return false;
            }

            //距离地面距离                
            float height = GetHeightToGround();
            if (height <= 0.011f)//避免太小
            {
                nCount = 0;
                return true;
            }

            return false;
        }

        //获取当前地面高度
        float GetHeightToGround()
        {
            //获取对应地面上的点
            Vector3 groundPos = BaseTool.instance.GetGroundPoint(m_transform.position);

            return m_transform.position.y - groundPos.y;
        }

        //到达地面
        void DoOnGround()
        {
            nCount = 0;

            if (m_actor.IsDeath())
            {
                m_isUseGravity = false;
                return;
            }

            CancelInvoke("AutoCancelStatic");
            CancelInvoke("MoveDistanceEnd");

            if (m_actor.mActorType == ActorType.AT_BOSS
                || m_actor.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                m_BehitState.isNonControlProtect = true;
            }

            m_actor.StopAll();
            m_actor.PlayAction("hit012");

            m_BehitState.m_isHitDownState = true;

            //navmesh disenable
            NavMeshAgent agent = this.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
            }

            float actionLen = m_actor.GetActionLength("hit012");
            Invoke("ExitBehitState", actionLen);
        }

        void ExitBehitState()
        {
            CancelInvoke("ExitBehitState");
            m_BehitState.ExitBehitState();
        }

        //切换到普通受击
        public void BreakExitBehitState()
        {
            CancelInvoke("ExitBehitState");
        }
    }

};  //end SG

