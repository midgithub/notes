/**
* @file     : DispatcherMgr.cs
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-06-22
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class MoveDispatcher
    {
        private static MoveDispatcher instance = null;
        public static MoveDispatcher Instance
        {
            get
            {
                if (null == instance)
                    instance = new MoveDispatcher();

                return instance;
            }
        }

        /// <summary>
        /// �Ƿ��������ͣ�  bFly = true �������ͣ�  bFly = false  ����Э�鷵�أ����ͽ���
        /// </summary>
        public static bool bFly = false;

        private MoveDispatcher()
        {
            mMoveHandlerDict = new Dictionary<long, IMoveHandler>();
        }

        private Dictionary<long, IMoveHandler> mMoveHandlerDict;

        public void Init()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OTHER_MOVE_TO, OnDispatchMoveTo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OTHER_CHANGE_DIR, OnDispatchChangeDir);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_OTHER_MOVE_STOP, OnDispatchMoveStop);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_MONSTER_MOVE_TO, OnDispatchMonsterMoveTo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CHANGE_POS, OnDispatchMonsterChangePos);
        }

        public void AddMoveHandler(long id, IMoveHandler handler)
        {
            if (mMoveHandlerDict.ContainsKey(id))
                return;

            mMoveHandlerDict.Add(id, handler);
        }

        public void RemoveHandler(long id)
        {
            if (!mMoveHandlerDict.ContainsKey(id))
                return;

            mMoveHandlerDict.Remove(id);
        }

        private void OnDispatchMoveTo(GameEvent ge, EventParameter parameter)
        {
            MsgData_sOtherMoveTo msg = parameter.msgParameter as MsgData_sOtherMoveTo;
            if (null == msg)
                return;

            IMoveHandler handler;
            if (mMoveHandlerDict.TryGetValue(msg.RoleID, out handler))
            {
                Vector2 from = ServerPositionConverter.ConvertToClientPositionVector2(new Vector2(msg.SrcX, msg.SrcY));
                Vector2 to = ServerPositionConverter.ConvertToClientPositionVector2(new Vector2(msg.DestX, msg.DestY));
                handler.OnMoveTo(from, to);
            }
        }

        private void OnDispatchChangeDir(GameEvent ge, EventParameter parameter)
        {
            MsgData_sOtherChangeDir msg = parameter.msgParameter as MsgData_sOtherChangeDir;
            if (null == msg)
                return;

            IMoveHandler handler;
            if (mMoveHandlerDict.TryGetValue(msg.GUID, out handler))
            {
                handler.OnChangeDir((float)msg.Dir);
            }
        }

        private void OnDispatchMoveStop(GameEvent ge, EventParameter parameter)
        {
            MsgData_sOtherMoveStop msg = parameter.msgParameter as MsgData_sOtherMoveStop;
            if (null == msg)
                return;

            IMoveHandler handler;
            if (mMoveHandlerDict.TryGetValue(msg.RoleID, out handler))
            {
                Vector2 pos = ServerPositionConverter.ConvertToClientPositionVector2(new Vector2(msg.StopX, msg.StopY));

                handler.OnMoveStop(pos, msg.Dir * 0.001f);
            }

            CoreEntry.gSceneObjMgr.UpdateObjMove(msg.RoleID, msg.StopX, msg.StopY, msg.Dir);
        }

        private void OnDispatchMonsterMoveTo(GameEvent ge, EventParameter parameter)
        {
            MsgData_sMonsterMoveTo msg = parameter.msgParameter as MsgData_sMonsterMoveTo;
            if (null == msg)
                return;

            IMoveHandler handler;
            if (mMoveHandlerDict.TryGetValue(msg.RoleID, out handler))
            {
                Vector2 to = ServerPositionConverter.ConvertToClientPositionVector2(new Vector2(msg.DestX, msg.DestY));
                handler.OnMonsterMoveTo(to);
            }
        }

        private void OnDispatchMonsterChangePos(GameEvent ge, EventParameter parameter)
        {
            MsgData_sChangePos msg = parameter.msgParameter as MsgData_sChangePos;
            if (null == msg)
                return;
            MoveDispatcher.bFly = false;
            ActorObj obj = CoreEntry.gActorMgr.GetActorByServerID(msg.RoleID);
            if (obj != null)
            {
                obj.StopMove(false);
                obj.SetPosition((float)msg.PosX, (float)msg.PosY);
            }
        }
    }

    public interface IMoveHandler
    {
        void OnMoveTo(Vector2 from, Vector2 to);
        void OnChangeDir(float dir);
        void OnMoveStop(Vector2 stopPos, float stopDir);
        void OnMonsterMoveTo(Vector2 to);
    }

}

