using UnityEngine;
using System.Collections;
using XLua;
using System;
using System.Collections.Generic;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class ShenBingMgr
    {

        private static ShenBingMgr instance = null;
        public static ShenBingMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new ShenBingMgr();

                return instance;
            }
        }

        /// <summary>
        /// 神兵配置
        /// </summary>
        public static Dictionary<int, LuaTable> CacheShenBingConfig = null;

        public ShenBingMgr()
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            CacheShenBingConfig = G.Get<Dictionary<int, LuaTable>>("t_shenbing");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MagicWeaponInfo, OnMagicWeaponInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MagicWeaponLevelUp, OnMagicWeaponLevelUp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MagicWeaponProficiency, OnMagicWeaponProficiency);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MagicWeaponChangeModel, OnMagicWeaponChangeModel);

        }
        public int GetShenBingModelId(int magicWeaponId)
        {
            LuaTable l;
            if (CacheShenBingConfig.TryGetValue(magicWeaponId, out l))
            {
                int model = l.Get<int>("model");
                return model;
            }
            return 0;
        }

        /// <summary>
        /// 请求神兵进阶 3250
        /// </summary>
        /// <param name="buyType"></param>
        public void Send_CS_MagicWeaponLevelUp(int buyType = 1)   //buyType  0表示 自动购买， 
        {
            MsgData_cMagicWeaponLevelUp rsp = new MsgData_cMagicWeaponLevelUp();
            rsp.autobuy = buyType;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MagicWeaponLevelUp, rsp);
        }


        /// <summary>
        /// 请求神兵换模型  3456
        /// </summary>
        /// <param name="buyType"></param>
        public void Send_CS_MagicWeaponChangeModel(int id)   //神兵配置表id
        {
            MsgData_cMagicWeaponChangeModel rsp = new MsgData_cMagicWeaponChangeModel();
            rsp.level = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_MagicWeaponChangeModel, rsp);
        }

        /// <summary>
        /// 神兵信息
        /// </summary>
        public MsgData_sMagicWeaponInfo info;

        /// <summary>
        /// 神兵战斗力
        /// </summary>
        public long Power;

        /// <summary>
        ///  服务器通知:神兵信息 msgId:8248
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnMagicWeaponInfo(GameEvent ge, EventParameter parameter)
        {
            info = parameter.msgParameter as MsgData_sMagicWeaponInfo;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_MagicWeaponInfo, EventParameter.Get(info));
        }

        /// <summary>
        /// 服务器通知:神兵熟练度 msgId:8249
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnMagicWeaponProficiency(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sMagicWeaponProficiency resp = parameter.msgParameter as MsgData_sMagicWeaponProficiency;
        }

        /// <summary>
        /// 服务器通知:神兵进阶 msgId:8250
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnMagicWeaponLevelUp(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sMagicWeaponLevelUp resp = parameter.msgParameter as MsgData_sMagicWeaponLevelUp;
        }

        /// <summary>
        /// 服务器通知:神兵换模型结果 msgId:8456
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnMagicWeaponChangeModel(GameEvent ge, EventParameter parameter)
        {
            MsgData_sMagicWeaponChangeModel resp = parameter.msgParameter as MsgData_sMagicWeaponChangeModel;
            if(resp.level >0)
            {
                PlayerData.Instance.ShenBingId = resp.level;
            }else
            {
               // Debug.LogError("幻化失败 result = " + resp.level);
            }
        }











    }
}

