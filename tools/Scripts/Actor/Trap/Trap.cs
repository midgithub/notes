using XLua;
﻿using UnityEngine;
using System.Collections;
using System;
namespace SG
{
[Hotfix]
    public class Trap : Entity
    {
        //private Animation m_Animation;

        void OnEnable()
        {

        }
        // Use this for initialization
        void Start()
        {

        }

        private void PlayAction(string actionName)
        {
            //if (m_Animation != null)
            //{
            //    if(m_Animation.GetClip(actionName))
            //    {
            //        m_Animation.CrossFade(actionName, 0.5f);
            //    }
            //    else
            //    {
            //        LogMgr.UnityLog("陷阱没有找到动作名:" + actionName); 
            //    }
            //}
        }

        //生物id
        public override void Init(int resID, int configID, long entityID , string strEnterAction = "", bool isNpc = false)
        {
            m_shadowType = 0;
            base.Init(resID, configID, entityID, strEnterAction, isNpc);
        }

        public void OnTrigger()
        {
            //PlayAction("attack001");
        }

        void showDecal()
        {

        }
        void hideDecel()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

     }


}

