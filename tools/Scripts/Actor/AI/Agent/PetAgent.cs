/**
* @file     : PlayerAgent.cs
* @brief    : 
* @details  : 玩家AI代理器，负责AI的管理
* @author   : 
* @date     : 2014-9-23 9:55
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

    [behaviac.TypeMetaInfo("PetAgent", "")]
[Hotfix]
    public class PetAgent : ActorAgent
    {
        //元数据
        [behaviac.MemberMetaInfo]
        
        //行为树
        public string m_behaviorTree = "pet";

        PlayerObj mPlayerObj = null;

        //行为树是否加载成功                    
        private bool m_btLoadResult = false;

        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
            InitPlayer();
            base.InitData();

        }

        public PlayerObj PlayerObj
        {
            get
            {
                if (mPlayerObj == null)
                {
                    mPlayerObj = CoreEntry.gActorMgr.MainPlayer;
                }
                return mPlayerObj;
            }

        }


        void OnEnable()
        {
            InitPlayer();
        }

        public void InitPlayer()
        {
            //初始化
            base.Init();

            //for debug
            this.SetIdFlag(1);

            //加载行为树         
            if (m_behaviorTree.Length > 0)
            {

                m_btLoadResult = btload(m_behaviorTree, true);
                if (m_btLoadResult)
                {
                    btsetcurrent(m_behaviorTree);
                }
                else
                {
                    LogMgr.LogError("load bt tree failed! " + m_behaviorTree);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!CoreEntry.GameStart)
                return;

            //执行行为树
            if (m_btLoadResult)
            {
                behaviac.EBTStatus ret = btexec();

                if (ret != behaviac.EBTStatus.BT_RUNNING)
                {

                    //结束了
                    btunload(m_behaviorTree);
                    m_btLoadResult = false;
                }
            }
        }
    }


};  //end SG

