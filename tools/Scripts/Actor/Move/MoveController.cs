using XLua;
﻿using System.Collections.Generic;
using UnityEngine;
using System;

namespace SG
{    
[Hotfix]
    public class MoveController : IMoveAgent
    {
        protected Vector3 moveDirection;
        protected Queue<Vector3> pointList = new Queue<Vector3>();
        protected Vector3 currentDestination;

        protected float lastSendMoveTime = 0.0f;
        protected float lastSendDirTime = 0.0f;
        protected float toralanceDirectionError = (float)Math.Abs(Math.Cos(10 * Math.PI / 180));

        protected bool isSlide = false;
        public override void SetServerPosition(Vector2 position)
        {
        }
        public override void MoveDirection(Vector3 direction)
        {
            actorObj.AutoPathFind = false;
            if (direction.sqrMagnitude < 1e-3f)
            {
                return;
            }
            pointList.Clear();
            status = MoveStatus.Running;

            isSlide = false;

            //todo ,get move speed from actor
            float speed = GetSpeed();
                        
            if(Vector3.Dot(moveDirection,direction) < toralanceDirectionError)            
            {
                moveDirection = direction;
                currentDestination = pathFinder.LineSegmentDetection(transform.position, transform.position + moveDirection * speed * SyncPeriod * 2, ref isSlide);                                

                FaceTo(currentDestination);

                SyncMoveToServer(transform.position, currentDestination);
                //SendMoveTo(transform.position, currentDestination);
            }
            else
            {                
                float remainLength = XZDistance(currentDestination, transform.position);
                if (remainLength < speed * SyncPeriod * 0.5f)
                {
                    Vector3 newDestination = pathFinder.LineSegmentDetection(transform.position, transform.position + moveDirection * speed * SyncPeriod * 2, ref isSlide);
                    if ((currentDestination - newDestination).sqrMagnitude > 1e-3f)
                    {
                        SendMoveTo(transform.position, newDestination);
                        currentDestination = newDestination;
                    }                    
                }
            }
        }
        public override bool MovePosition(Vector3 destination)
        {
           // Debug.LogError("===================自动寻路=============" + destination);
            isSlide = false;
            actorObj.AutoPathFind = false;

            List<Vector3> points = pathFinder.FindPath(transform.position, destination);

            if (points == null || points.Count == 0)
            {
                LogMgr.DebugLog(string.Format("目标点{0}不可达，寻路失败", destination));
                return false;
            }

            pointList.Clear();
            for(int i = 0; i < points.Count; i++)
            {
                Vector3 p = points[i];
                pointList.Enqueue(p);
            }

            currentDestination = pointList.Dequeue();
            moveDirection = (currentDestination - transform.position).normalized;
            FaceTo(currentDestination);
            SendMoveTo(transform.position, currentDestination);

            Status = MoveStatus.Running;
            actorObj.AutoPathFind = true;

            return true;
        }
        protected override void UpdateImpl(float deltaTime)
        {            
            if (XZDistanceSqr(currentDestination, transform.position) < 1e-3f )
            {
                if (pointList.Count <1)
                {                    
                    Stop();
                    return;
                }

                currentDestination = pointList.Dequeue();
                FaceTo(currentDestination);
                SendMoveTo(transform.position, currentDestination);
            }

            //todo ,get move speed from actor
            float speed = GetSpeed();
            //float speed = 6.0f;
            if (isSlide)
            {
                speed *= 0.2f;
            }

            float dist;
            float moveDist = deltaTime * speed;
            float remainDist = moveDist;
            Vector3 prevPositioin = transform.position;

            while (true)
            {
                dist = XZDistance(currentDestination, prevPositioin);
                if (remainDist <= dist)
                {
                    break;
                }
                if (pointList.Count < 1)
                {
                    currentDestination.y = pathFinder.GetTerrainHeight(currentDestination.x, currentDestination.z); 
                    BaseTool.SetPosition(transform, currentDestination);

                    Stop();
                    return ;
                }
                remainDist -= dist;
                prevPositioin = currentDestination;
                currentDestination = pointList.Dequeue();
                FaceTo(currentDestination);
                SendMoveTo(transform.position, currentDestination);
            }

            Vector3 pos = prevPositioin + (currentDestination - prevPositioin).normalized * remainDist;
            float height = pathFinder.GetTerrainHeight(pos.x, pos.z);
            if (0 != height)
            {
                pos.y = height;
            }

            //SendMoveTo(transform.position, currentDestination);
            //transform.position = pos;

            BaseTool.SetPosition(transform, pos);

        }

        public override void Stop()
        {
 	        base.Stop();

            pointList.Clear();

            Vector2 serverPos = ServerPositionConverter.ConvertToServerPosition(transform.position);
            //NetLogicGame.Instance.SendReqMoveStop((int)serverPos.x, (int)serverPos.y, (int)(transform.eulerAngles.y * ServerPositionConverter.ServerScale));
            //float rad = (float)(transform.eulerAngles.y * Math.PI / 180.0f);
            //rad -= (float)Math.PI * 0.5f;
            //Vector3 dir = new Vector3((float)Math.Sin(rad), 0, (float)Math.Cos(rad));
            //Debug.Log(string.Format("stop direction ({0} ,{1} ,{2})",dir.x ,dir.y ,dir.z));
            NetLogicGame.Instance.SendReqMoveStop((int)serverPos.x, (int)serverPos.y, (int)(ServerPositionConverter.ConvertToServerRotation(transform.rotation)));
        }

        public override void FaceTo(GameObject target)
        {
 	        FaceTo(target.transform.position);
        }

        public override void FaceTo(Vector3 poisition)
        {
 	        base.FaceTo(poisition);

            /*
            if (Time.time - lastSendDirTime > SyncPeriod)
            {
                NetLogicGame.Instance.SendReqChangeDir(transform.eulerAngles.y);

                lastSendDirTime = Time.time;
            }*/
        }

        public override void SendDirection2Server(Quaternion rotation)
        {
            //NetLogicGame.Instance.SendReqChangeDir((int)(ServerPositionConverter.ConvertToServerRotation(rotation)));
            NetLogicGame.Instance.SendReqChangeDir(rotation.eulerAngles.y * Math.PI / 180.0f);
        }

        private void SendMoveTo(Vector3 srcPos, Vector3 destPos)
        {
            //if (Time.time - lastSendMoveTime >= SyncPeriod)
            {
                Vector3 srcServerPos = ServerPositionConverter.ConvertToServerPosition(srcPos);
                Vector3 destServerPos = ServerPositionConverter.ConvertToServerPosition(destPos);

                NetLogicGame.Instance.SendReqMoveTo((int)srcServerPos.x, (int)srcServerPos.y, (int)destServerPos.x, (int)destServerPos.y);

                //SyncCount++;
                lastSendMoveTime = Time.time;
            }
        }
        private void SyncMoveToServer(Vector3 srcPos, Vector3 destPos)
        {
            if (Time.time - lastSendMoveTime >= SyncPeriod)
            {
                SendMoveTo(srcPos, destPos);
                lastSendMoveTime = Time.time;
            }
        }

        public override float GetSpeed()
        {
            return PlayerData.Instance.BaseAttr.Speed;
        }

        public override List<Vector3> GetRestPath()
        {
            List<Vector3> restPath = new List<Vector3>();
            if (status == MoveStatus.Running)
            {
                restPath.Add(currentDestination);
            }
            foreach (Vector3 pos in pointList)
            {
                restPath.Add(pos);
            }

            return restPath;
        }
    }
}

