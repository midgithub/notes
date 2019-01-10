using UnityEngine;
using System.Collections;
using System;
using XLua;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class ShieldMgr
    {

        private static ShieldMgr instance = null;
        public static ShieldMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new ShieldMgr();

                return instance;
            }
        }

        /// <summary>
        /// 圣盾配置
        /// </summary>
        public static Dictionary<int, LuaTable> CacheShieldConfig = null;

        public ShieldMgr()
        {          
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            CacheShieldConfig = G.Get<Dictionary<int, LuaTable>>("t_pifeng");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_PiFengUpdateInfo, OnPiFengInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_PiFengLevelUp, OnPiFengLevelUp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_PiFengChangeModel, OnPiFengChangeModel);

        }
        public int GetShieldModelId(int shieldId)
        {
            LuaTable l;
            if (CacheShieldConfig.TryGetValue(shieldId, out l))
            {
                int model = l.Get<int>("model");
                return model;
            }
            return 0;
        }

        /// <summary>
        /// 请求圣盾进阶 3724
        /// </summary>
        /// <param name="buyType"></param>
        public void Send_CS_PiFengLevelUp(int buyType = 0,int upType = 1)   //buyType  1表示 自动购买，  upType  1激活, 2升级
        {
            //测试用
            //if (upType == 2)
            //{
            //    buyType = 0;
            //}

            MsgData_cPiFengLevelUp rsp = new MsgData_cPiFengLevelUp();
            rsp.autobuy = buyType;
            rsp.uptype = upType;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_PiFengLevelUp, rsp);
        }


        /// <summary>
        /// 请求圣盾换模型  3725
        /// </summary>
        /// <param name="buyType"></param>
        public void Send_CS_PiFengChangeModel(int id)   //圣盾配置表id
        {
            MsgData_cPiFengChangeModel rsp = new MsgData_cPiFengChangeModel();
            rsp.level = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_PiFengChangeModel, rsp);
        }

        /// <summary>
        /// 圣盾信息
        /// </summary>
        public MsgData_sPiFengInfo info;

        /// <summary>
        /// 玩家当前已拥有的圣盾最高阶id
        /// </summary>
        public int curId;

        /// <summary>
        /// 圣盾战斗力
        /// </summary>
        public long Power;

        /// <summary>
        ///  服务器通知:圣盾信息 msgId:8723
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnPiFengInfo(GameEvent ge, EventParameter parameter)
        {
            info = parameter.msgParameter as MsgData_sPiFengInfo;
            curId = info.level;

            EventParameter ep = EventParameter.Get();
            ep.msgParameter = parameter.msgParameter;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_PiFengInfo, ep);
  //          Debug.LogError("receive OnPiFengInfo level ---> " + curId);
        }

        /// <summary>
        /// 服务器通知:圣盾进阶 msgId:8724
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnPiFengLevelUp(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sPiFengLevelUp resp = parameter.msgParameter as MsgData_sPiFengLevelUp;
        }

        /// <summary>
        /// 服务器通知:圣盾换模型结果 msgId:8725
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnPiFengChangeModel(GameEvent ge, EventParameter parameter)
        {
            MsgData_sPiFengChangeModel resp = parameter.msgParameter as MsgData_sPiFengChangeModel;
            if (resp.result == 0)
            {
                LogMgr.LogError("更换成功 " + resp.result);
            }
            else
            {
                LogMgr.LogError("更换失败 " + resp.result);
            }
        }
        /// <summary>
        /// 重置圣盾信息
        /// </summary>
        public void ReSet()
        {
            this.curId = 0;
            this.info = null;
            this.Power = 0;
        }
    }

}

