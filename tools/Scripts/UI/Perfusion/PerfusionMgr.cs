using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;

namespace SG
{
    /// <summary>
    /// 灌注
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class PerfusionMgr
    {
        private static PerfusionMgr instance = null;
        public static PerfusionMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new PerfusionMgr();
                return instance;
            }
        }

        
        /// <summary>
        ///已使用的属性丹的数量
        /// </summary>
        public Dictionary<AttrDanEnumType, int> typeValues = new Dictionary<AttrDanEnumType, int>();

        /// <summary>
        /// 获得已使用的某类型的丹药数量
        /// </summary>
        /// <returns></returns>
        public int GetEnumTypeValue(int enumType)
        {
            if(typeValues.ContainsKey((AttrDanEnumType)enumType))
            {
                return typeValues[(AttrDanEnumType)enumType];
            }
            return 0;
        }

        /*

        /// <summary>
        /// 每种灌注类型对应的属性丹AttrDanEnumType 枚举值
        /// </summary>
        public Dictionary<int, PerfusionAttrDanType> typeEnumAttrs = new Dictionary<int, PerfusionAttrDanType>()
        {
            {8,new PerfusionAttrDanType(AttrDanEnumType.Ride,AttrDanEnumType.RidePer) },  //坐骑
            {118,new PerfusionAttrDanType(AttrDanEnumType.ShenBing,AttrDanEnumType.ShenBingPer) },  //神兵

        };
        /// <summary>
        /// 获得灌注类型的使用丹药协议AttrDanEnumType
        /// </summary>
        /// <returns></returns>
        public PerfusionAttrDanType GetEnumTypeAttrs(int enumType)
        {
            if(typeEnumAttrs.ContainsKey(enumType))
            {
                return typeEnumAttrs[enumType];
            }else
            {
                Debug.LogError("脚本PerfusionMgr ---> typeEnumAttrs 表 未配置当前灌注类型 对应的AttrDanEnumType 协议使用类型");
                return null;
            }
        }
        */

        public PerfusionMgr()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_USE_ATTR_DAN, OnUSE_ATTR_DAN);
        }


        /// <summary>
        /// 发送使用属性丹请求。
        /// </summary>
        public void SendUseAttrDanRequest(int type)
        {
            MsgData_cUseAttrDan data = new MsgData_cUseAttrDan();
            data.Type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_USE_ATTR_DAN, data);
        }

        /// <summary>
        /// 服务端通知：使用属性丹回复
        /// </summary>
        /// <param name="data">网络消息数据。</param>
        private void OnUSE_ATTR_DAN(GameEvent ge, EventParameter parameter)
        {
            MsgData_sUseAttrDan info = parameter.msgParameter as MsgData_sUseAttrDan;
            if (info.Result == 0)
            {
                typeValues[(AttrDanEnumType)info.Type] = info.PillNumber;  //更新使用类型的 已使用数量
               // EventParameter ep = EventParameter.Get();
               // ep.intParameter = info.Type;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_USE_ATTR_DAN, parameter);
            }
        }

    }

    public enum AttrDanEnumType
    {
        Ride = 1,
        LingShou = 2,
        ShenBing = 3,
        LingZhen = 4,
        RideWar = 5,
        PiFeng = 6,
        ShengLing = 7,
        PiFengPer = 8,
        BaoJia = 9,
        BaoJiaPer = 10,
        TianGang = 11,
        TianGangPer = 12,
        ZhanNu = 13,
        ZhanNuPer = 14,
        RidePer = 15,
        ShenBingPer = 16,
        RideWarPer = 17,
        WuXing = 18,
        MingLun = 19,
        MingLunPer = 20,
        HunQi = 21,
        HunQiPer = 22,
        WuXingPer = 23,
        ShengQi = 24,
        ShengQiPer = 25,
        ZhenFa = 26,
        ZhenFaPer = 27,
        Qilinbi = 28,
        QilinbiPer = 29,
        JianYu = 30,
        JianYuPer = 31,
    }
    /*
    [LuaCallCSharp]
[Hotfix]
    public class PerfusionAttrDanType
    {
        /// <summary>
        /// 属性丹
        /// </summary>
        public int attrDan;
        /// <summary>
        /// 资质丹
        /// </summary>
        public int attPerDan;

        public PerfusionAttrDanType(AttrDanEnumType one, AttrDanEnumType two)
        {
            attrDan = (int)one;
            attPerDan = (int)two;
        }
    }
    */

}


