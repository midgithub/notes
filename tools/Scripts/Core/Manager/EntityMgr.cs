/**
* @file     : Entitymgr.cs
* @brief    : Entity对象管理
* @details  : 场景中除Player，NPC和Monster外的实体管理
* @author   : CW
* @date     : 2017-07-05
*/
using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class EntityMgr : MonoBehaviour 
    {
        private List<Entity> mCollectionList = new List<Entity>();
        private List<Entity> mPortalList = new List<Entity>();
        private List<Entity> mTrapList = new List<Entity>();
        private List<Entity> mCampList = new List<Entity>();

        /// <summary>
        /// 清除实体对象
        /// </summary>
        public void ClearObjs()
        {
            mCollectionList.Clear();
            mPortalList.Clear();
            mTrapList.Clear();
            mCampList.Clear();
        }

        /// <summary>
        /// 获取所有实体对象
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetAllEntities()
        {
            List<Entity> entities = new List<Entity>();
            entities.AddRange(mCollectionList);
            entities.AddRange(mPortalList);
            entities.AddRange(mTrapList);
            entities.AddRange(mCampList);

            return entities;
        }

        #region 采集点
        /// <summary>
        /// 添加一个采集点
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>是否添加成功</returns>
        public bool AddCollection(CollectionObj entity)
        {
            if (!mCollectionList.Contains(entity))
            {
                mCollectionList.Add(entity);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取所有的采集点
        /// </summary>
        public List<Entity> GetAllCollections()
        {
            return mCollectionList;
        }

        /// <summary>
        /// 根据ServerID获取采集点数据
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public Entity GetCollectionByServerID(long serverID)
        {
            for (int i = 0; i < mCollectionList.Count; i++)
            {
                if (serverID == mCollectionList[i].ServerID)
                {
                    return mCollectionList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据配置id获取采集点
        /// </summary>
        /// <param name="configID"></param>
        /// <returns></returns>
        public Entity GetCollectionByConfigID(int configID)
        {
            for (int i = 0; i < mCollectionList.Count; i++)
            {
                if (configID == mCollectionList[i].ConfigID)
                {
                    return mCollectionList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据ServerID删除采集点
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否删除成功</returns>
        public bool RemoveCollectionByServerID(long serverID)
        {
            for (int i = 0; i < mCollectionList.Count; i++)
            {
                if (serverID == mCollectionList[i].ServerID)
                {
                    mCollectionList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 传送门
        /// <summary>
        /// 添加一个传送门
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>是否添加成功</returns>
        public bool AddPortal(Entity entity)
        {
            if (!mPortalList.Contains(entity))
            {
                mPortalList.Add(entity);

                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 获取所有的传送门
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetAllPortals()
        {
            return mPortalList;
        }

        /// <summary>
        /// 根据ServerID获取传送门数据
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public Entity GetPortalByServerID(long serverID)
        {
            for (int i = 0; i < mPortalList.Count; i++)
            {
                if (serverID == mPortalList[i].ServerID)
                {
                    return mPortalList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据配置id获取传送门
        /// </summary>
        /// <param name="configID"></param>
        /// <returns></returns>
        public Entity GetPortalByConfigID(int configID)
        {
            for (int i = 0; i < mPortalList.Count; i++)
            {
                if (configID == mPortalList[i].ConfigID)
                {
                    return mPortalList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据ServerID删除传送门
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否删除成功</returns>
        public bool RemovePortalByServerID(long serverID)
        {
            for (int i = 0; i < mPortalList.Count; i++)
            {
                if (serverID == mPortalList[i].ServerID)
                {
                    mPortalList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 陷阱
        /// <summary>
        /// 添加一个陷阱
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>是否添加成功</returns>
        public bool AddTrap(Entity entity)
        {
            if (!mTrapList.Contains(entity))
            {
                mTrapList.Add(entity);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取所有的陷阱
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetAllTraps()
        {
            return mTrapList;
        }

        /// <summary>
        /// 根据ServerID获取陷阱数据
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public Entity GetTrapByServerID(long serverID)
        {
            for (int i = 0; i < mTrapList.Count; i++)
            {
                if (serverID == mTrapList[i].ServerID)
                {
                    return mTrapList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据ServerID删除陷阱
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns>是否删除成功</returns>
        public bool RemoveTrapByServerID(long serverID)
        {
            for (int i = 0; i < mTrapList.Count; i++)
            {
                if (serverID == mTrapList[i].ServerID)
                {
                    mTrapList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }
        #endregion

        #region 阵营
        public bool AddCamp(Entity entity)
        {
            if (!mCampList.Contains(entity))
            {
                mCampList.Add(entity);

                return true;
            }

            return false;
        }

        public List<Entity> GetAllCamps()
        {
            return mCampList;
        }

        public Entity GetCampByServerID(long serverID)
        {
            for (int i = 0; i < mCampList.Count; i++)
            {
                if (serverID == mCampList[i].ServerID)
                {
                    return mCampList[i];
                }
            }

            return null;
        }

        public bool RemoveCampByServerID(long serverID)
        {
            for (int i = 0; i < mCampList.Count; i++)
            {
                if (serverID == mCampList[i].ServerID)
                {
                    mCampList.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}

