/**
* @file     : PortalObj.cs
* @brief    : 传送门
* @details  : 传送门
* @author   : CW
* @date     : 2017-07-07
*/
using UnityEngine;
using XLua;

namespace SG
{
[Hotfix]
    public class PortalObj : Entity
    {
        protected LuaTable m_PortalCfg;
        public LuaTable PortalCfg
        {
            get { return m_PortalCfg; }
        }

        protected GameObject m_EffectObj;
        public GameObject EffectObj
        {
            get { return m_EffectObj; }
        }

        protected bool m_IgnoreEnter;

        public override void Init(int resID, int ConfigID, long ServerID, string strEnterAction = "", bool isNpc = false)
        {
            m_shadowType = 0;
            base.Init(resID, ConfigID, ServerID, strEnterAction, isNpc);

            m_PortalCfg = ConfigManager.Instance.Map.GetPortalConfig(ConfigID);
 
            m_IgnoreEnter = false;
            m_EffectObj = null;
            if (null != m_PortalCfg)
            {
                //碰撞体
                SphereCollider collider = gameObject.GetComponent<SphereCollider>();
                if (null == collider)
                {
                    collider = gameObject.AddComponent<SphereCollider>();
                }
                collider.isTrigger = true;
                collider.center = Vector3.zero;
                collider.radius = m_PortalCfg.Get<float>("trigger_dist");

                //判断是否初始点在检测范围内
                ActorObj actor = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
                if (null != actor)
                {
                    float distance = Vector3.Distance(actor.transform.position, transform.position);
                    float cdRadius = actor.GetColliderRadius();
                    distance = distance - cdRadius - 0.2f;

                    if (distance <= m_PortalCfg.Get<float>("trigger_dist"))
                    {
                       // m_IgnoreEnter = true;
                    }
                }

                //特效
                GameObject obj = CoreEntry.gGameObjPoolMgr.Instantiate(m_PortalCfg.Get<string>("pfx"));//(GameObject)Instantiate(CoreEntry.gResLoader.LoadResource(m_PortalCfg.pfx));
                if (null != obj)
                {
                    obj.transform.parent = transform;
                    obj.transform.localPosition = new Vector3(0f, m_PortalCfg.Get<int>("effect_high"), 0f);
                    obj.transform.localScale = Vector3.one;
                }

                m_EffectObj = obj;
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            ActorObj actor = collider.GetComponent<ActorObj>();
            if (null == actor)
            {
                return;
            }
            if (actor != CoreEntry.gActorMgr.MainPlayer)
            {
                return;
            }
            
            if (m_IgnoreEnter)
            {
                return;
            }

            if (PlayerData.Instance.BaseAttr.Level < m_PortalCfg.Get<int>("limitLv"))
            {
                string msg = string.Format("人物等级不足{0}级，无法传送!", m_PortalCfg.Get<int>("limitLv"));
                UITips.ShowTips(msg);
                LogMgr.LogWarning(msg + "(客户端主动判断)");
                return;
            }
            if (PlayerData.Instance.BaseAttr.JingJieLevel < m_PortalCfg.Get<int>("dianfengLv"))
            {
                string msg = string.Format("人物境界等级不足{0}级，无法传送!", m_PortalCfg.Get<int>("dianfengLv"));
                UITips.ShowTips(msg);
                LogMgr.LogWarning(msg + "(客户端主动判断)");
                return;
            }
            if(m_PortalCfg.Get<int>("type") == 3)   //活动地图传送门 --世界BOSS,银两BOSS等
            {
                ActivityMgr.Instance.Send_CS_ActivityEnter(m_PortalCfg.Get<int>("targetActivity"));
            }
            else
            {
                //
                CoreEntry.gActorMgr.MainPlayer.StopMove(true);
                NetLogicGame.Instance.SendReqStaticTrigger(serverID);
            }
          
        }

        void OnTriggerExit(Collider collider)
        {
            ActorObj actor = collider.GetComponent<ActorObj>();
            if (null == actor)
            {
                return;
            }
            if (actor != CoreEntry.gActorMgr.MainPlayer)
            {
                return;
            }

            m_IgnoreEnter = false;
        }

        public override void RecycleObj()
        {
            if (null != m_EffectObj)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(m_EffectObj);
            }
            CoreEntry.gObjPoolMgr.RecycleObject(resid, gameObject);
        }
    }
}

