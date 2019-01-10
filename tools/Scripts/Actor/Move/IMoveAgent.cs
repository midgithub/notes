using XLua;
﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{   
    public enum MoveStatus
    {
        Running = 0,
        Pause,
        Stopped ,
        PrepareStop ,
    }
[Hotfix]
    public class IMoveAgent : MonoBehaviour
    {
        protected ActorObj actorObj;

        public static float SyncPeriod = 0.2f; //todo ,sync frequency ,from global setting

        protected IPathFinder pathFinder;
        protected MoveStatus status = MoveStatus.Stopped;

        //protected long SyncCount = 0;
        protected float lastLoginTime = Time.time;

        public MoveStatus Status
        {
            get { return status; }
            set
            {
                status = value; OnStatusChanged();
            }
        }
        public virtual void FaceTo(GameObject target)
        {
            if(actorObj.GetCurState() == ACTOR_STATE.AS_RUN || actorObj.GetCurState() == ACTOR_STATE.AS_RUN_TO_ATTACK)
                FaceTo(target.transform.position);
        }
        public virtual void FaceTo(Vector3 position)
        {
            actorObj.FaceTo(position);
        }       
        public bool IsCanArrived(Vector3 position)
        {
            return !IsBlocked(position);
        }
        public bool IsBlocked(Vector3 position)
        {
            if (pathFinder != null)
            {
                return pathFinder.IsBlocked(position);
            }
            return false;
        }  
        public virtual void SetServerPosition(Vector2 position)
        {
            Vector3 clientPosition = ServerPositionConverter.ConvertToClientPosition(position);
            clientPosition.y = pathFinder.GetTerrainHeight(clientPosition.x, clientPosition.z);
            // transform.position = clientPosition;

            BaseTool.SetPosition(transform, clientPosition);

            status = MoveStatus.Stopped;
            
            //Debug.LogWarning(string.Format("name : {0} ,pos({1} ,{2})", gameObject.name, clientPosition.x, clientPosition.z));
        }
        public void SetServerRotation(float rotation)
        {
            transform.rotation = ServerPositionConverter.ConvertToClientRotation(rotation, transform.rotation);
        }
        public void SetServerPositionAndRotation(Vector2 position, float rotation)
        {
            SetServerPosition(position);
            SetServerRotation(rotation);
        }
        public Vector2 GetServerPosition()
        {
            return ServerPositionConverter.ConvertToServerPosition(transform.position);
        }
        public float GetServerRotation()
        {
            return ServerPositionConverter.ConvertToServerRotation(transform.rotation);
        }
        public void GetServerPositionAndRotation(ref Vector2 position ,ref float rotation)
        {
            position = ServerPositionConverter.ConvertToServerPosition(transform.position);
            rotation = ServerPositionConverter.ConvertToServerRotation(transform.rotation);
        }     
        public void Update()
        {
            /*
            if (Time.time - lastLoginTime >= 10.0f && SyncCount > 0)
            {
                Debug.Log("Sync Count is " + SyncCount / (Time.time - lastLoginTime));
                lastLoginTime = Time.time;
                SyncCount = 0;
            }*/

            if (Status != MoveStatus.Running && Status != MoveStatus.PrepareStop)
            {
                return;
            }

            UpdateImpl(Time.deltaTime);            
        }

        public virtual void SendDirection2Server(Quaternion rotation)
        {
        }

        protected float XZDistance(Vector3 v0 ,Vector3 v1)
        {
            return XZProjectorSelf(v1 - v0).magnitude;
        }
        protected float XZDistanceSqr(Vector3 v0 ,Vector3 v1)
        {
            return XZProjectorSelf(v1 - v0).sqrMagnitude;
        }
        protected Vector3 XZProjector(Vector3 v0)
        {
            return new Vector3(v0.x, 0, v0.z);
        }
        protected Vector3 XZProjectorSelf(Vector3 v0)
        {
            v0.y = 0;
            return v0;
        }

        public virtual void DoInit(ActorObj actor) 
        { 
            actorObj = actor;
            if(pathFinder == null)
                pathFinder = new WayPointPathFinder();
            if (null != actorObj)
            {
                actorObj.AutoPathFind = false;
            }
        }
        protected virtual void OnStatusChanged()
        {
            //StateParameter param;
            switch (status)
            {
                case MoveStatus.Stopped:
                    actorObj.RequestChangeState(ACTOR_STATE.AS_STAND);
                    actorObj.AutoPathFind = false;
                    break;
                case MoveStatus.Running:
                case MoveStatus.PrepareStop:
                    actorObj.RequestChangeState(ACTOR_STATE.AS_RUN);
                    break;
                default:
                    break;
            }
        }      
        protected virtual void UpdateImpl(float deltaTime) {}
        public virtual float GetSpeed()
        {
            if(actorObj != null)
            {
                return actorObj.GetSpeed();
            }
            return 4.0f;
        }
        public virtual void Stop() 
        {
            Status = MoveStatus.Stopped;
        }
        public virtual bool MovePosition(Vector3 destination) { return false; }
        public virtual void MoveDirection(Vector3 dir) { }

        /// <summary>
        /// 获取剩余路点
        /// </summary>
        /// <returns></returns>
        public virtual List<Vector3> GetRestPath() { return null; }
        /// <summary>
        /// 获取当前寻路的完整路点
        /// </summary>
        /// <returns></returns>
        public virtual List<Vector3> GetPath() { return null; }
    }
}

