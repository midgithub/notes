using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

    //处理curve类
[Hotfix]
    public class CurveMoveParam
    {
        public bool isCarryOffTarget = false;   //是否带走目标        
        public bool isStopForTarget = false;    //是否碰到目标停止  

        public List<ActorType> aimActorTypeList = null; //目标对象    
    }

[Hotfix]
    public class AnimationCurveBase
    {
        //private Animation m_ani = null;    
        private Transform m_transform = null;
        private Vector3 m_DeltaPos = new Vector3(); //累加在曲线位移时间内其他行为造成的主角位移动距离
        private Vector3 m_LastPos = new Vector3();  //上一帧主角所在位置
        public GameObject m_actorObject = null;
        private AnimationCurveData m_animationCurveData = null;
        private ActorObj m_actor;

        private float m_beginTime = 0;

        AnimationCurve m_curveX = new AnimationCurve();
        AnimationCurve m_curveY = new AnimationCurve();
        AnimationCurve m_curveZ = new AnimationCurve();

        //private string m_curPlayAction = "";        

        //private CurveMoveParam m_curveMoveParam = null;
        private bool m_needMove = false;

        //生物半径
        private float m_curRadius = 0;

        public Vector3 m_MoveDestPos = Vector3.zero;

        public bool m_bHitBack = false;


        int m_uuid = -1;

        //int m_num = 1;

        //初始化
        public void Init(GameObject obj)
        {
            m_actorObject = obj;
            //m_ani = m_actorObject.GetComponent<Animation>();
            m_actor = (m_actorObject.GetComponent<ActorObj>());

            m_transform = m_actorObject.transform;
            m_animationCurveData = m_actorObject.GetComponent<AnimationCurveData>();
            if (m_animationCurveData == null)
            {
                //LogMgr.LogError(obj.name + ":缺少动画曲线组件AnimationCurveData，可能将导致异常。美术请检查该模型！！！");
            }

            if (m_actor != null && m_actor != null)
            {
                m_curRadius = m_actor.GetColliderRadius();
            }
            else
            {
                LogMgr.LogError("Please Check m_actor:" + m_actor + " : " + m_actor);
            }

            //如果是PVP状态
            if (CoreEntry.gGameMgr.IsPvpState())
            {
                m_curRadius += 1.8f;

            }


        }

        //位移开始
        public bool StartMove(string clipName, Vector3 dstPos, CurveMoveParam param, int uuid)
        {
            //m_curveMoveParam = param;
            m_needMove = false;

            m_curRadius = m_actor.GetColliderRadius();

            //如果是PVP状态
            if (CoreEntry.gGameMgr.IsPvpState())
            {
                m_curRadius += 1.8f;

            }

            m_MoveDestPos = dstPos;

            if (MoveToPos(clipName, dstPos))
            {
                m_needMove = true;
                m_uuid = uuid;
                return true;
            }

            return false;
        }

        public void UpdateFrame()
        {

            if (!m_needMove)
            {
                return;
            }

            float fDistance = Vector3.Distance(m_MoveDestPos, m_LastPos);
            if (fDistance < 0.01f)
            {
                SetMoveState(false);
                return;
            }


            //if (!m_actor.IsPlayingAction(m_curPlayAction))
            //{
            //    //m_needMove = false;
            //    SetMoveState(false);
            //    return;
            //}

            //插值开始
            float diffTime = Time.time - m_beginTime;

            if (m_LastPos != Vector3.zero)
            {
                m_DeltaPos += m_transform.position - m_LastPos;
            }

            //计算当前帧的位置
            Vector3 curPos = m_transform.position;
            Vector3 nextPos = GetPos(diffTime) + m_DeltaPos;

            Vector3 moveDir = nextPos - curPos;
            moveDir.Normalize();

            //目标移动位置
            Vector3 moveDstPos = nextPos;

            Vector3 groundPos = BaseTool.instance.GetGroundPoint(moveDstPos);
            if (groundPos == Vector3.zero)
            {
                if (m_actor.mActorType == ActorType.AT_MONSTER || m_actor.mActorType == ActorType.AT_BOSS || m_actor.mActorType == ActorType.AT_MECHANICS)
                {
                    BaseTool.SetPosition(m_transform, CoreEntry.gActorMgr.MainPlayer.transform.position);
                }
                else
                {
                    BaseTool.SetPosition(m_transform, m_LastPos);
                }

                return;
            }
            else
            {
                BaseTool.SetPosition(m_transform, groundPos);
            }

            m_LastPos = m_transform.position;

        }

        public void SetExit(int uuid)
        {
            //m_beginTime = 0;        
            //m_needMove = false;
            if (m_uuid != uuid)
            {
                return;
            }

            SetMoveState(false);
        }

        public void SetExit()
        {
            m_beginTime = 0;
        }

        void SetMoveState(bool bSet)
        {
            m_needMove = bSet;

        }

        public Vector3 GetPos(float curTime)
        {
            return new Vector3(m_curveX.Evaluate(curTime), m_curveY.Evaluate(curTime), m_curveZ.Evaluate(curTime));
        }

        public Vector3 GetCurveMoveEndPosition(string clipName)
        {
            if (m_animationCurveData == null)
            {
                return Vector3.zero;
            }

            //给定动画是否有位移曲线
            CurveData curveData = m_animationCurveData.GetCurveData(clipName);
            if (curveData == null)
            {
                LogMgr.UnityLog("can't find " + clipName + " curve data");
                return Vector3.zero;
            }

            Vector3 endPos = Vector3.zero;

            //这里必须帧数一样
            for (int i = 0; i < curveData.curve.Length; ++i)
            {
                if (curveData.curve[i].propertyName.Contains("m_LocalPosition.x"))
                {
                    AnimationCurve tempX = curveData.curve[i].curve;

                    endPos.x = tempX.keys[tempX.length - 1].value;
                }
                else if (curveData.curve[i].propertyName.Contains("m_LocalPosition.y"))
                {
                    AnimationCurve tempY = curveData.curve[i].curve;
                    endPos.y = tempY.keys[tempY.length - 1].value;
                }
                else if (curveData.curve[i].propertyName.Contains("m_LocalPosition.z"))
                {
                    AnimationCurve tempZ = curveData.curve[i].curve;
                    endPos.z = tempZ.keys[tempZ.length - 1].value;
                }
            }

            endPos = m_transform.rotation * endPos + m_transform.position;

            return endPos;
        }

        //给定终点移动
        public bool MoveToPos(string strClipName, Vector3 dstPos, bool bHitBack = false)
        {
            m_needMove = true;

            //重新获取下radius,防止init函数获取失败
            m_curRadius = m_actor.GetColliderRadius();

            //给定动画是否有位移曲线
            if (null == m_animationCurveData)
            {
                return false;
            }
            CurveData curveData = m_animationCurveData.GetCurveData(strClipName);
            if (curveData == null)
            {
                LogMgr.DebugLog("can't find {0} curve data", strClipName);
                return false;
            }

            //是否是击退
            m_bHitBack = bHitBack;

            m_DeltaPos = Vector3.zero;
            m_LastPos = m_transform.position;

            List<float> listValueX = new List<float>();
            List<float> listValueY = new List<float>();
            List<float> listValueZ = new List<float>();
            List<float> listTime = new List<float>();

            List<Keyframe> listKeyFrameX = new List<Keyframe>();
            List<Keyframe> listKeyFrameY = new List<Keyframe>();
            List<Keyframe> listKeyFrameZ = new List<Keyframe>();

            //这里必须帧数一样
            for (int i = 0; i < curveData.curve.Length; ++i)
            {
                if (curveData.curve[i].propertyName.Contains("m_LocalPosition.x"))
                {
                    AnimationCurve tempX = curveData.curve[i].curve;

                    //LogMgr.UnityLog("curveX length=" + tempX.length);
                    for (int j = 0; j < tempX.length; ++j)
                    {
                        listTime.Add(tempX.keys[j].time);
                        listValueX.Add(tempX.keys[j].value);
                        listKeyFrameX.Add(tempX.keys[j]);
                    }
                }
                else if (curveData.curve[i].propertyName.Contains("m_LocalPosition.y"))
                {
                    AnimationCurve tempY = curveData.curve[i].curve;

                    //LogMgr.UnityLog("curveY length=" + tempY.length);
                    for (int j = 0; j < tempY.length; ++j)
                    {
                        listValueY.Add(tempY.keys[j].value);
                        listKeyFrameY.Add(tempY.keys[j]);
                    }
                }
                else if (curveData.curve[i].propertyName.Contains("m_LocalPosition.z"))
                {
                    AnimationCurve tempZ = curveData.curve[i].curve;

                    //LogMgr.UnityLog("curveZ length=" + tempZ.length);
                    for (int j = 0; j < tempZ.length; ++j)
                    {
                        listValueZ.Add(tempZ.keys[j].value);
                        listKeyFrameZ.Add(tempZ.keys[j]);
                    }
                }
            }

            //帧数不一致，不处理
            if (listValueX.Count != listValueY.Count || listValueX.Count != listValueZ.Count
                || listValueY.Count != listValueZ.Count)
            {
                LogMgr.LogError(strClipName + " have diffent key count");
                return false;
            }

            bool needCutKey = false;

            //缩放系数        
            float distanceA = Vector3.Distance(dstPos, m_transform.position);

            Vector3 pos1 = new Vector3(listValueX[0], listValueY[0], listValueZ[0]);
            Vector3 pos2 = new Vector3(listValueX[listValueX.Count - 1],
                listValueY[listValueX.Count - 1], listValueZ[listValueX.Count - 1]);
            float distanceB = Vector3.Distance(pos1, pos2);

            //是否有卡帧系数
            if (curveData.keyIndex > 0 & curveData.keyIndex < listValueX.Count - 1)
            {
                //卡帧前面的位移
                Vector3 pos3 = new Vector3(listValueX[curveData.keyIndex], listValueY[0], listValueZ[curveData.keyIndex]);
                float distanceC = Vector3.Distance(pos1, pos3);

                //LogMgr.UnityLog("distanceC=" + distanceC);

                if (distanceA > distanceC)
                {
                    distanceA -= distanceC;
                    distanceB -= distanceC;
                    needCutKey = true;
                }
            }

            float fscale = distanceA / distanceB;
            if (Mathf.Approximately(distanceB, 0) || fscale <= 0)
            {
                fscale = 1f;
            }

            //LogMgr.UnityLog("distanceA=" + distanceA + ", distanceB=" + distanceB + ", fscale=" + fscale);

            //使用点的旋转            
            List<Vector3> listCurvePos = new List<Vector3>();
            for (int i = 0; i < listValueX.Count; ++i)
            {
                Vector3 pos = new Vector3(listValueX[i], listValueY[i], listValueZ[i]);

                //缩放
                if (needCutKey && curveData.keyIndex > 0 && curveData.keyIndex < listValueX.Count - 1)
                {
                    //后面的缩放
                    if (i > curveData.keyIndex)
                    {
                        pos.Scale(new Vector3(fscale, 1, fscale));
                        pos += new Vector3(listValueX[curveData.keyIndex], pos.y, listValueZ[curveData.keyIndex]);
                    }
                }
                else
                {
                    pos.Scale(new Vector3(fscale, 1f, fscale));
                }

                //自身旋转
                pos = m_transform.rotation * pos;

                listCurvePos.Add(pos);
            }

            //终点旋转
            Vector3 curveDstPos = listCurvePos[listCurvePos.Count - 1] + m_transform.position;

            Vector3 fromDir = curveDstPos - m_transform.position;
            Vector3 toDir = dstPos - m_transform.position;

            Quaternion rot = Quaternion.FromToRotation(fromDir, toDir);

            //旋转到指定点方向
            for (int i = 0; i < listCurvePos.Count; ++i)
            {
                listCurvePos[i] = rot * listCurvePos[i];
            }

            //找到对应的值                        
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            //动画曲线
            for (int i = 0; i < listCurvePos.Count; ++i)
            {
                Vector3 pos = listCurvePos[i] + m_transform.position;

                //LogMgr.UnityLog("pos=" + pos.ToString("f4") + ", time=" + listTime[i]);

                Keyframe keyX = new Keyframe(listTime[i], pos.x, listKeyFrameX[i].inTangent, listKeyFrameX[i].outTangent);
                keyX.tangentMode = listKeyFrameX[i].tangentMode;

                Keyframe keyY = new Keyframe(listTime[i], pos.y, listKeyFrameY[i].inTangent, listKeyFrameY[i].outTangent);
                keyY.tangentMode = listKeyFrameY[i].tangentMode;

                Keyframe keyZ = new Keyframe(listTime[i], pos.z, listKeyFrameZ[i].inTangent, listKeyFrameZ[i].outTangent);
                keyZ.tangentMode = listKeyFrameZ[i].tangentMode;

                curveX.AddKey(keyX);
                curveY.AddKey(keyY);
                curveZ.AddKey(keyZ);
            }

            // 平滑处理
            if (curveX.length > 3)
                for (int i = 1; i < curveX.length - 1; i++)
                {
                    // 小的颤动平滑掉
                    if (Mathf.Abs((curveX.keys[i].value - curveX.keys[i - 1].value) + (curveX.keys[i].value - curveX.keys[i + 1].value)) > 0.3f)
                        continue;
                    if (Mathf.Abs((curveY.keys[i].value - curveY.keys[i - 1].value) + (curveY.keys[i].value - curveY.keys[i + 1].value)) > 0.3f)
                        continue;
                    if (Mathf.Abs((curveZ.keys[i].value - curveZ.keys[i - 1].value) + (curveZ.keys[i].value - curveZ.keys[i + 1].value)) > 0.3f)
                        continue;

                    curveX.SmoothTangents(i, 0.5f);
                    curveY.SmoothTangents(i, 0.5f);
                    curveZ.SmoothTangents(i, 0.5f);
                }

            m_curveX = curveX;
            m_curveY = curveY;
            m_curveZ = curveZ;

            m_beginTime = Time.time;

            return true;
        }
    }
}

