using XLua;
﻿using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO; 
 

namespace SG
{

    //游戏逻辑管理器    
[Hotfix]
    public class TimeMgr : MonoBehaviour
    {
        //游戏开始入口
        void Start()
        {

        }


        //全局时间缩放
        float m_globalScale = 1.0f;
        public float GlobalScale
        {
            get { return m_globalScale; }
            set { m_globalScale = value; }
        }

        //全局缩放

        float m_timeScale= 1;
        public float TimeScale
        {
            get { return Time.timeScale; }
            set { 
                m_timeScale = value;
                Time.timeScale = m_timeScale * m_globalScale; 
            }
        }

    }
 
};//end SG

