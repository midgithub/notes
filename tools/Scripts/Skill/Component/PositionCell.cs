/**
* @file     : PositionCell.cs
* @brief    : 
* @details  : 位置元素
* @author   : 
* @date     : 2014-12-9
*/

using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{

[Hotfix]
    public class PositionCell : ISkillCell
    {
        SkillBase m_skillBase = null;
        PositionCellDesc cellDesc = null;
        LuaTable behaviorSkillDesc = null;

        /// <summary>
        /// 初始点的位置，前方向，右方向
        /// </summary>
        Vector3 orgPos = Vector3.zero;
        Vector3 orgFront= Vector3.forward;
        Vector3 orgRight = Vector3.right;

        /// <summary>
        /// 直线运动的点
        /// </summary>
        Vector3 curMovingPos = Vector3.zero;

        /// <summary>
        /// 总旋转时间
        /// </summary>
        float rotatingTimer = 0f;
        /// <summary>
        /// 经过关键帧的长度
        /// </summary>
        float accumlatedRotatingDuration = 0f;
        /// <summary>
        /// 控制总旋转的角度
        /// </summary>
        float rotatingAngle = 0f;
        /// <summary>
        ///直线运动的关键点的序号
        /// </summary>
        int currentIndex = -1;
        /// <summary>
        /// 旋转运动的关键点的序号
        /// </summary>
        int currentRotationIndex = -1;

        /// <summary>
        /// 是否开始移动
        /// </summary>
        bool isStartMoving = false;

        Transform targetTransform = null;

        public override void Init(ISkillCellData cellData, SkillBase skillBase)
        {
            m_skillBase = skillBase;
            cellDesc = (PositionCellDesc)cellData;

            skillBase.ShouldUpdatePositionToCastObj = false;

            orgPos = m_skillBase.m_actor.transform.position;
            orgFront = m_skillBase.m_actor.transform.forward;
            orgRight = m_skillBase.m_actor.transform.right;
            targetTransform = null;

            if( cellDesc != null )
            {
                //有对应的行为技能ID
                if (cellDesc.BehaviorSkillId > 0)
                {
                    behaviorSkillDesc = skillBase.m_actor.GetCurSkillDesc(cellDesc.BehaviorSkillId);
                    if (behaviorSkillDesc != null)
                    {
                        ActorObj targetActorBase = skillBase.m_actor.GetTargetFromSelector(behaviorSkillDesc, null);
                        if (targetActorBase != null)
                        {
                            targetTransform = targetActorBase.transform;
                        }
                    }
                }
                else
                {
                    //默认技能行为
                    if (skillBase.m_hitActor != null)
                    {
                        targetTransform = skillBase.m_hitActor.transform;
                    }
                }

                //是否attach到actor上
                if (cellDesc.AttachToActor)
                {
                    if (cellDesc.SyncRotation)
                    {
                        skillBase.transform.parent = targetTransform;
                        BaseTool.ResetTransform(skillBase.transform);
                        skillBase.transform.localPosition += cellDesc.AttachOffset;
                    }
                    else
                    {
                        SyncPosition();
                        BaseTool.SetLocalPosition(m_skillBase.transform, curMovingPos);
                    }
                }
                else
                {
                    //走相应的轨迹
                    if (cellDesc.PointList.Count > 0)
                    {
                        BaseTool.SetPosition(skillBase.transform, GetPos(0));
                        skillBase.transform.forward = skillBase.transform.forward;
                        currentIndex = 0;
                    }
                   
                }

                //同步施法者位置
                if (cellDesc.SyncOwnerPosition)
                {
                    BaseTool.SetPosition(m_skillBase.m_actor.transform, skillBase.transform.position);
                }

                Reset();

                if (cellDesc.DelayMovingTime >= 0)
                {
                    //吟唱后，发射子弹
                    float curTime = m_skillBase.GetCurActionTime();

                    if (cellDesc.DelayMovingTime <= curTime)
                    {
                        StartToMove();
                    }
                    else
                    {
                        Invoke("StartToMove", cellDesc.DelayMovingTime - curTime);
                    }
                }
           }
        }

        void Reset()
        {
            curMovingPos = m_skillBase.transform.localPosition;
            rotatingTimer = 0f;

            //旋转角度永远从后方开始
            Vector3 angleVector = m_skillBase.m_actor.transform.rotation.eulerAngles;
            rotatingAngle = (angleVector.y - 180) * Mathf.Deg2Rad;
            
            isStartMoving = false;
            currentRotationIndex = 0;
            accumlatedRotatingDuration = 0;
        }

        Vector3 GetPos(int index)
        {
            if (cellDesc != null && cellDesc.PointList.Count > index)
            {
                return orgPos + GetSpacePos(cellDesc.PointList[index].Point);
            }

            return orgPos;
        }

        Vector3 GetSpacePos(Vector3 pos)
        {
            return pos.x * orgRight + pos.y * Vector3.up + pos.z * orgFront;
        }

        Vector3 GetTargetPos()
        {
            if (targetTransform != null)
            {
                return targetTransform.position + GetSpacePos(cellDesc.AttachOffset);
            }

            return m_skillBase.transform.position;
        }

        void StartToMove()
        {
            isStartMoving = true;
        }

        void LateUpdate()
        {
            if (isStartMoving)
            {
                if (cellDesc.AttachToActor == false && cellDesc.Speed > 0 && cellDesc.PointList.Count > 1)
                {
                    int nextIndex = currentIndex + 1;


                    if ((cellDesc.PointList.Count > nextIndex) )
                    {
                        Vector3 start = GetPos(currentIndex);
                        
                        if (cellDesc.PointList[nextIndex].SkipWhenTouch)
                        {
                            start = curMovingPos;
                        }
                        
                        Vector3 end = GetPos(nextIndex);

                        //如果回到actor重新获取信息
                        bool backToActor = (cellDesc.PointList[nextIndex].BackToActor && targetTransform != null);
                        if (backToActor)
                        {
                            start = curMovingPos;
                            end = GetTargetPos();
                        }


                        Vector3 dir = end - start;
                        float disSQ = dir.sqrMagnitude;
                        dir.Normalize();
                        m_skillBase.transform.LookAt(end);
                        Vector3 dest = curMovingPos + dir * Time.deltaTime;

                        bool reachedEnd = false;

                        //到当前点的条件
                        if (backToActor)
                        {
                            reachedEnd = (disSQ <= 1);
                        }
                        else
                        {
                            reachedEnd = ((dest - start).sqrMagnitude >= disSQ);
                        }

                        //是否到了当前点
                        if (reachedEnd)
                        {
                            //是否在该点结束技能
                            if (cellDesc.PointList[nextIndex].EndSkill)
                            {
                                m_skillBase.SkillEnd();
                            }

                            //继续找下一个移动节点
                            curMovingPos = end;
                            m_skillBase.transform.forward = dir;
                            currentIndex++;
                        }
                        else
                        {
                            curMovingPos +=dir * cellDesc.Speed * Time.deltaTime;
                        }

                        BaseTool.SetPosition(m_skillBase.transform, curMovingPos);
                    }
                }
                else if (cellDesc.AttachToActor == true && cellDesc.SyncRotation == false)
                {
                    //只同步位移，不同步旋转的情况
                    SyncPosition();
                }

                if (ProcessRotation(curMovingPos) == false)
                {
                    //不旋转直接设置位置
                    BaseTool.SetLocalPosition(m_skillBase.transform,curMovingPos);
                }
            }
        }


        void SyncPosition()
        {
            curMovingPos = GetTargetPos();
            if (cellDesc.LookAtActor)
            {
                m_skillBase.transform.LookAt(curMovingPos);
            }
        }

        /// <summary>
        /// 处理围绕旋转
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        bool ProcessRotation(Vector3 center)
        {
            if (cellDesc.RotationList.Count > 0)
            {
                RotationDesc desc = cellDesc.RotationList[currentRotationIndex];
                rotatingTimer += Time.deltaTime ;
                rotatingAngle += Time.deltaTime * desc.Speed;

                float diffTime = rotatingTimer - accumlatedRotatingDuration;
                float radius = Mathf.Lerp(desc.BeginRadius, desc.EndRadius, diffTime / desc.Duration);
                //Debug.LogWarning(radius + "  " +rotatingTimer / (desc.Duration + accumlatedRotatingDuration) + " " + (desc.Duration + accumlatedRotatingDuration) + " " + Time.time);
                Vector3 finalOffset = Mathf.Cos(rotatingAngle) * radius * Vector3.forward + Mathf.Sin(rotatingAngle) * radius * Vector3.right;
                BaseTool.SetLocalPosition(m_skillBase.transform, center + finalOffset);

                if (diffTime >= desc.Duration)
                {
                    //指向下一个节点
                    if (currentRotationIndex < (cellDesc.RotationList.Count - 1))
                    {
                        accumlatedRotatingDuration = desc.Duration;
                        currentRotationIndex++;
                    }
                }

                return true;
            }

            return false;
        }

        public void OnTriggerEnter(Collider other)
        {
            int nextIndex = currentIndex+1;
            if (nextIndex < cellDesc.PointList.Count)
            {
                ActorObj actorBase = other.transform.root.gameObject.GetComponent<ActorObj>();
                if (actorBase == null)
                {
                    actorBase = other.transform.gameObject.GetComponent<ActorObj>();
                    //bIsMonster = true;
                }

                if (actorBase != null && actorBase.gameObject != m_skillBase.m_actor.gameObject)
                {
                    if (cellDesc.PointList[nextIndex].SkipWhenTouch)
                    {
                        currentIndex++;
                    }
                }
            }
        }
    }

};  //end SG

