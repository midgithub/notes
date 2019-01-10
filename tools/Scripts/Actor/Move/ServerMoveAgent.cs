
/**
* @file     : ServerMoveAgent.cs
* @brief    : 
* @details  : 网络移动同步实现
* @author   : 
* @date     : 2014-11-28 9:31
*/


using XLua;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{    
[Hotfix]
    public class ServerMoveData
    {
        public uint ID;
        public uint StartTime;
        public Vector2 StartPosition;
        public float Speed = 6.0f;
        public Vector2 Destination;
        public float yaw;
    }    
[Hotfix]
    public class ServerMoveAgent : IMoveAgent, IMoveHandler
    {
        public static float MaxSmoothSpeedTimes = 3.0f;

        protected uint startLogicTime;
        protected Vector3 moveSpeed;
        protected Vector3 destination ;
        protected Vector3 moveDirection;
        protected Vector3 lastPosition;
        protected float yaw;

        public void OnSynchronize(ServerMoveData packet ,MoveStatus state = MoveStatus.Running)
        {
            /*
            //强制设置位置
            transform.position = new Vector3(packet.Destination.x, 0, packet.Destination.y);
            float height = pathFinder.GetTerrainHeight(transform.position.x, transform.position.z);
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
            return; */

            Status = state;
            
            destination = new Vector3(packet.Destination.x, 0, packet.Destination.y);
            moveDirection = (packet.Destination - new Vector2(transform.position.x, transform.position.z)).normalized;
            moveSpeed = moveDirection * GetSpeed() * 1.1f; //临时加速其它玩家和怪物，使其被到达目标位置的可能性增加
            moveSpeed = new Vector3(moveSpeed.x, 0, moveSpeed.y);
            lastPosition = transform.position;
            yaw = packet.yaw;

            if (Status == MoveStatus.Running)
            {
                FaceTo(destination);
            }

            /*
            startLogicTime = packet.StartTime;

            //计算到目标位置的距离和方向,用于判定是否需要加速
            Vector2 xzPosition = transform.position;
            Vector2 dir = packet.Destination - xzPosition;
            float length = dir.magnitude;
            dir.Normalize();

            //计算可用于移动到目标位置的可用时间片
            //todo ,get timeService from logic module                
            LatencyEstimate timeService = new LatencyEstimate();
            float serverMoveTime = (packet.Destination - packet.StartPosition).magnitude / packet.Speed;

            float diffTime = packet.StartTime - timeService.ServerTime;
            float expectMoveTime = diffTime > 0 ? serverMoveTime + diffTime : serverMoveTime - diffTime;            

            Vector3 speed2D;
            //计算需要的移动速度
            float expectSpeed = length / expectMoveTime;     
            //计算速度和做位置校正       
            //误差太大，则强制把目标位置设置到服务器指定位置(强制拉人）
            if (expectSpeed > packet.Speed * MaxSmoothSpeedTimes)
            {
                SetServerPosition(packet.StartPosition);
                speed2D = dir * packet.Speed ;                
            }
            else
            {
                //平滑加/减速追赶服务器目标位置
                speed2D = dir * expectSpeed ;                                
            }
            moveSpeed = new Vector3(speed2D.x ,0 ,speed2D.y);

            //设置目标位置，朝向目标
            destination = new Vector3(packet.Destination.x ,0 , packet.Destination.y);
            FaceTo(destination);
            Status = MoveStatus.Running; */

            //StateParameter parameter = new StateParameter();
            //parameter.state = ACTOR_STATE.AS_RUN;
            //actorObj.RequestChangeState(parameter);
        }
        protected override void UpdateImpl(float deltaTime)
        {
            if (actorObj.IsDeath())
            {
                return;
            }  

            if(actorObj.GetCurState() != ACTOR_STATE.AS_RUN)
            {
                actorObj.RequestChangeState(ACTOR_STATE.AS_RUN);
            }


            if((transform.position- lastPosition).sqrMagnitude >1e-3f)
            {
                lastPosition = transform.position;
                moveDirection = destination - lastPosition;
                //moveDirection = destination - transform.position;
                moveDirection.y = 0;
                moveDirection.Normalize();
                moveSpeed = moveDirection * GetSpeed() * 1.1f; 
            }

            Vector3 movePath = deltaTime * moveSpeed ;
            float sqrDist = XZDistanceSqr(transform.position, destination);
            float moveSqrDist = movePath.sqrMagnitude;            

            if (moveSqrDist >= sqrDist )
            {
                //transform.position = destination;

                BaseTool.SetPosition(transform, destination);

                if (Status == MoveStatus.PrepareStop)
                {
                    OnChangeDir(yaw);
                }
                Stop();
            }
            else
            {
                //transform.position = transform.position + movePath;       

                BaseTool.SetPosition(transform, transform.position + movePath);
            }
            
            float height = pathFinder.GetTerrainHeight(transform.position.x, transform.position.z) ;

            //  transform.position = new Vector3(transform.position.x, height, transform.position.z);

            BaseTool.SetPosition(transform, new Vector3(transform.position.x, height, transform.position.z));
            lastPosition = transform.position;         
        }

        public override void DoInit(ActorObj actor)
        {
            base.DoInit(actor);

            MoveDispatcher.Instance.AddMoveHandler(actorObj.ServerID, this);
        }

        void OnDisable()
        {
            status = MoveStatus.Stopped;

            MoveDispatcher.Instance.RemoveHandler(actorObj.ServerID);
        }

        public void OnMoveTo(Vector2 from, Vector2 to)
        {
            ServerMoveData data = new ServerMoveData();
            data.StartPosition = from;
            data.Destination = to;

            OnSynchronize(data);
        }

        public void OnChangeDir(float dir)
        {
            Vector3 angle = transform.eulerAngles;
            angle.y = dir * 180.0f/(float)Math.PI;

            /*
            if (gameObject.name.IndexOf("zhanshi") != -1)
            {
                Debug.LogWarning(string.Format("name : {0} ,dir({1})", gameObject.name, angle.y));
            }*/

            transform.eulerAngles = angle;
        }

        public void OnMoveStop(Vector2 stopPos, float stopDir)
        {
            ServerMoveData data = new ServerMoveData();
            data.StartPosition = new Vector2(transform.position.x, transform.position.z);
            data.Destination = stopPos;
            data.yaw = stopDir;

            OnSynchronize(data, MoveStatus.PrepareStop);
        }

        public void OnMonsterMoveTo(Vector2 to)
        {
            if (actorObj.IsDeath())
            {
                return;
            }

            ServerMoveData data = new ServerMoveData();
            data.StartPosition = new Vector2(transform.position.x, transform.position.z);
            data.Destination = to;

            OnSynchronize(data);
        }
    }
}

