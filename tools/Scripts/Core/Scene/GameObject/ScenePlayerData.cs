using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
    /// <summary>
    /// 场景玩家信息(由于超过最大创建上限而不创建的玩家数据)
    /// </summary>
[Hotfix]
    public class ScenePlayerData
    {
        /// <summary>
        /// 玩家进场数据。
        /// </summary>
        private MsgData_sSceneObjectEnterHuman m_EnterData;

        /// <summary>
        /// 获取或设置玩家进场数据。
        /// </summary>
        public MsgData_sSceneObjectEnterHuman EnterData
        {
            get { return m_EnterData; }
            set { m_EnterData = value; }
        }

        /// <summary>
        /// 获取玩家Guid。
        /// </summary>
        public long Guid
        {
            get
            {
                return m_EnterData == null ? 0 : m_EnterData.Guid;
            }
        }
    }
}

