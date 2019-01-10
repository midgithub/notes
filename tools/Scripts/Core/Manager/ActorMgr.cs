/**
* @file     : ActorMgr.cs
* @brief    : Actor对象管理类
* @details  : Actor对象管理类
* @author   : CW
* @date     : 2017-06-27
*/

using XLua;
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
    public class ActorMgr : MonoBehaviour
    {
        private List<ActorObj> mPlayerList = new List<ActorObj>();
        private List<ActorObj> mNPCList = new List<ActorObj>();
        private List<ActorObj> mMonsterList = new List<ActorObj>();
        private List<ActorObj> mPetList = new List<ActorObj>();

        private PlayerObj m_MainPlayer = null;


        public PlayerObj MainPlayer
        {

            get { return m_MainPlayer; }
            set
            {
                m_MainPlayer = value;
            }
        }




        /// <summary>
        /// 添加一个Actor
        /// </summary>
        /// <param name="actor"></param>
        public void AddActorObj(ActorObj actor)
        {
            if (actor is PlayerObj || actor is OtherPlayer)
            {
                if (!mPlayerList.Contains(actor))
                {
                    mPlayerList.Add(actor);
                }
            }
            else if (actor is MonsterObj)
            {
                if (!mMonsterList.Contains(actor))
                {
                    mMonsterList.Add(actor);
                }
            }
            else if (actor is NpcObj)
            {
                if (!mNPCList.Contains(actor))
                {
                    mNPCList.Add(actor);
                }
            }
            else if (actor is PetObj)
            {
                if (!mPetList.Contains(actor))
                {
                    mPetList.Add(actor);
                }
            }
        }

        /// <summary>
        /// 根据serverid获取actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ActorObj GetActorByServerID(long serverID)
        {
            ActorObj actor = null;
            actor = GetPlayerActorByServerID(serverID);
            if (null == actor)
            {
                actor = GetMonsterActorByServerID(serverID);
            }
            if (null == actor)
            {
                actor = GetNPCActorByServerID(serverID);
            }
            if (null == actor)
            {
                actor = GetPetActorByServerID(serverID);
            }

            return actor;
        }

        /// <summary>
        /// 根据serverid获取player actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ActorObj GetPlayerActorByServerID(long serverID)
        {
            for (int i = 0; i < mPlayerList.Count; i++)
            {
                if (serverID == mPlayerList[i].ServerID)
                    return mPlayerList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据serverid获取 monster actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ActorObj GetMonsterActorByServerID(long serverID)
        {
            for (int i = 0; i < mMonsterList.Count; i++)
            {
                if (serverID == mMonsterList[i].ServerID)
                    return mMonsterList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据serverid获取npc actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ActorObj GetNPCActorByServerID(long serverID)
        {
            for (int i = 0; i < mNPCList.Count; i++)
            {
                if (serverID == mNPCList[i].ServerID)
                    return mNPCList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据serverid获取pet actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ActorObj GetPetActorByServerID(long serverID)
        {
            for (int i = 0; i < mPetList.Count; i++)
            {
                if (serverID == mPetList[i].ServerID)
                    return mPetList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据entityID获取actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ActorObj GetActorByEntityID(int entityID)
        {
            ActorObj actor = null;
            actor = GetPlayerActorByEntityID(entityID);
            if (null == actor)
            {
                actor = GetMonsterActorByEntityID(entityID);
            }
            if (null == actor)
            {
                actor = GetNPCActorByEntityID(entityID);
            }
            if (null == actor)
            {
                actor = GetPetActorByEntityID(entityID);
            }

            return actor;
        }

        /// <summary>
        /// 根据entityID获取player actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public ActorObj GetPlayerActorByEntityID(int entityID)
        {
            for (int i = 0; i < mPlayerList.Count; i++)
            {
                if (entityID == mPlayerList[i].EntityID)
                    return mPlayerList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据entityID获取 monster actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public ActorObj GetMonsterActorByEntityID(int entityID)
        {
            for (int i = 0; i < mMonsterList.Count; i++)
            {
                if (entityID == mMonsterList[i].EntityID)
                    return mMonsterList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据entityID获取npc actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public ActorObj GetNPCActorByEntityID(int entityID)
        {
            for (int i = 0; i < mNPCList.Count; i++)
            {
                if (entityID == mNPCList[i].EntityID)
                    return mNPCList[i];
            }

            return null;
        }

        /// <summary>
        /// 根据entityID获取pet actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public ActorObj GetPetActorByEntityID(int entityID)
        {
            for (int i = 0; i < mPetList.Count; i++)
            {
                if (entityID == mPetList[i].EntityID)
                    return mPetList[i];
            }

            return null;
        }

        /// <summary>
        /// 通关配置ID获取角色。
        /// </summary>
        /// <param name="cfgid">配置ID。</param>
        /// <returns>角色对象，只能获取NPC、Monster和Pet。</returns>
        public ActorObj GetActorByConfigID(int cfgid)
        {
            if (cfgid == 0)
            {
                return MainPlayer;
            }

            for (int i = 0; i < mMonsterList.Count; i++)
            {
                if (cfgid == mMonsterList[i].ConfigID)
                    return mMonsterList[i];
            }
            for (int i = 0; i < mNPCList.Count; i++)
            {
                if (cfgid == mNPCList[i].ConfigID)
                    return mNPCList[i];
            }
            for (int i = 0; i < mPetList.Count; i++)
            {
                if (cfgid == mPetList[i].ConfigID)
                    return mPetList[i];
            }

            return null;
        }

        /// <summary>
        /// 获取当前的BOSS对象。
        /// </summary>
        /// <returns>BOSS对象。</returns>
        public ActorObj GetBoss()
        {
            for (int i = 0; i < mMonsterList.Count; i++)
            {
                MonsterObj monster = mMonsterList[i] as MonsterObj;
                if (monster.IsBoss)
                {
                    return monster;
                }
            }
            return null;
        }

        ///// <summary>
        ///// 根据resID获取Actor
        ///// </summary>
        ///// <param name="resID"></param>
        ///// <returns></returns>
        //[System.Obsolete("使用GetActorByServerID或者GetActorByEntityID", false)]
        //public ActorObj GetActorByResID(int resID)
        //{
        //    for (int i = 0; i < mPlayerList.Count; i++)
        //    {
        //        if (resID == mPlayerList[i].resid)
        //            return mPlayerList[i];
        //    }
        //    for (int i = 0; i < mMonsterList.Count; i++)
        //    {
        //        if (resID == mMonsterList[i].resid)
        //            return mMonsterList[i];
        //    } 
        //    for (int i = 0; i < mNPCList.Count; i++)
        //    {
        //        if (resID == mNPCList[i].resid)
        //            return mNPCList[i];
        //    }
        //    for (int i = 0; i < mPetList.Count; i++)
        //    {
        //        if (resID == mPetList[i].resid)
        //            return mPetList[i];
        //    }

        //    return null;
        //}

        /// <summary>
        /// 获取所有的Actors
        /// </summary>
        /// <returns></returns>
        public List<ActorObj> GetAllActors()
        {
            List<ActorObj> actors = new List<ActorObj>();
            actors.AddRange(mPlayerList);
            actors.AddRange(mMonsterList);
            actors.AddRange(mNPCList);
            actors.AddRange(mPetList);

            return actors;
        }

        /// <summary>
        /// 获取所有的 MonsterActor
        /// </summary>
        /// <returns></returns>
        public List<ActorObj> GetAllMonsterActors()
        {
            return mMonsterList;
        }

        /// <summary>
        /// 获取所有的NPC Actor
        /// </summary>
        /// <returns></returns>
        public List<ActorObj> GetAllNPCActors()
        {
            return mNPCList;
        }

        /// <summary>
        /// 获取所有的玩家Actor
        /// </summary>
        /// <returns></returns>
        public List<ActorObj> GetAllPlayerActors()
        {
            return mPlayerList;
        }

        /// <summary>
        /// 获取所有的pet Actor
        /// </summary>
        /// <returns></returns>
        public List<ActorObj> GetAllPetActors()
        {
            return mPetList;
        }

        /// <summary>
        /// 移除Actor
        /// </summary>
        /// <param name="actor"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveActor(ActorObj actor)
        {
            return RemoveActorByServerID(actor.ServerID);
        }

        /// <summary>
        /// 根据serverID移除一个Actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveActorByServerID(long serverID)
        {
            bool result = false;
            result = RemovePlayerActorByServerID(serverID);
            if (!result)
            {
                result = RemoveMonsterActorByServerID(serverID);
            }
            if (!result)
            {
                result = RemoveNPCActorByServerID(serverID);
            }
            if (!result)
            {
                result = RemovePetActorByServerID(serverID);
            }

            return result;
        }

        /// <summary>
        /// 根据serverID移除一个player Actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemovePlayerActorByServerID(long serverID)
        {
            for (int i = 0; i < mPlayerList.Count; i++)
            {
                if (serverID == mPlayerList[i].ServerID)
                {
                    mPlayerList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据serverID移除一个monster Actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveMonsterActorByServerID(long serverID)
        {
            for (int i = 0; i < mMonsterList.Count; i++)
            {
                if (serverID == mMonsterList[i].ServerID)
                {
                    mMonsterList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据serverID移除一个npc Actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveNPCActorByServerID(long serverID)
        {
            for (int i = 0; i < mNPCList.Count; i++)
            {
                if (serverID == mNPCList[i].ServerID)
                {
                    mNPCList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据serverID移除一个pet Actor
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemovePetActorByServerID(long serverID)
        {
            for (int i = 0; i < mPetList.Count; i++)
            {
                if (serverID == mPetList[i].ServerID)
                {
                    mPetList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据entityID移除一个Actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveActorByEntityID(int entityID)
        {
            bool result = false;
            result = RemovePlayerActorByEntityID(entityID);
            if (!result)
            {
                result = RemoveMonsterActorByEntityID(entityID);
            }
            if (!result)
            {
                result = RemoveNPCActorByEntityID(entityID);
            }
            if (!result)
            {
                result = RemovePetActorByEntityID(entityID);
            }

            return result;
        }

        /// <summary>
        /// 根据entityID移除一个player Actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemovePlayerActorByEntityID(int entityID)
        {
            for (int i = 0; i < mPlayerList.Count; i++)
            {
                if (entityID == mPlayerList[i].EntityID)
                {
                    mPlayerList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据entityID移除一个monster Actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveMonsterActorByEntityID(int entityID)
        {
            for (int i = 0; i < mMonsterList.Count; i++)
            {
                if (entityID == mMonsterList[i].EntityID)
                {
                    mMonsterList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据entityID移除一个npc Actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemoveNPCActorByEntityID(int entityID)
        {
            for (int i = 0; i < mNPCList.Count; i++)
            {
                if (entityID == mNPCList[i].EntityID)
                {
                    mNPCList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据entityID移除一个pet Actor
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns>是否移除成功</returns>
        public bool RemovePetActorByEntityID(int entityID)
        {
            for (int i = 0; i < mPetList.Count; i++)
            {
                if (entityID == mPetList[i].EntityID)
                {
                    mPetList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 清除Actor数据
        /// </summary>
        public void ClearActor()
        { 
            mPlayerList.Clear();
            mNPCList.Clear();
            mMonsterList.Clear();
            mPetList.Clear();
        }

        /// <summary>
        /// 切线清除角色。
        /// </summary>
        public void OnSwitchLineClear()
        {
            Debug.LogWarning("OnSwitchLineClearActor");

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_OBJ_CLEAR, null);
        }
    }
}

