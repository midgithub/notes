/**
* @file     : EquipDataMgr.cs
* @brief    : 
* @details  :  
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SG
{
    [LuaCallCSharp]
    [Hotfix]
    //装备位置信息
    public class EquipPosInfo
    {
        public int level; // 星级
        public int exp; // 经验
        public int starLevel;//星级
        public int vip1;    // 钻石VIP打造 ，1已打造，0未打造
        public int vip2;    // 王者VIP打造 ，1已打造，0未打造
        public int vip3;	// 至尊VIP打造 ，1已打造，0未打造
        public int madness;   //0未狂化， 1已狂化
        public void UpdateEquipInfo(MsgData_sEquipData data)
        {
            level = data.level;
            exp = data.exp;
            starLevel = data.starLevel;
            vip1 = data.vip1;
            vip2 = data.vip2;
            vip3 = data.vip3;
            madness = data.madness;
        }

        /// <summary>
        /// 将数据写入缓冲区。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Pack(NetWriteBuffer buffer)
        {
            buffer.WriteInt32(level);
            buffer.WriteInt32(exp);
            buffer.WriteInt32(starLevel);
            buffer.WriteInt32(vip1);
            buffer.WriteInt32(vip2);
            buffer.WriteInt32(vip3);
            buffer.WriteInt32(madness);
        }

        /// <summary>
        /// 从缓冲区中读取数据。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Uppack(NetReadBuffer buffer)
        {
            level = buffer.ReadInt32();
            exp = buffer.ReadInt32();
            starLevel = buffer.ReadInt32();
            vip1 = buffer.ReadInt32();
            vip2 = buffer.ReadInt32();
            vip3 = buffer.ReadInt32();
            madness = buffer.ReadInt32();
        }
    }
    [LuaCallCSharp]
    [Hotfix]
    public class EquipAttInfo
    {
        public long uid; // 装备cid;
        public int strenLvl; // 强化等级;
        public int strenVal; // 强化值
        public int attrAddLvl; // 追加属性等级;
        public int groupId; // 套装id;
        public int groupId2; // 套装id2;
        public int groupId2Bind; //  套装id2绑定状态0 未绑定，1 已绑定
        public int group2Level; // 套装等级
        public int superNum; // 卓越数量
        public int isyuangu; // 是否远古
        public int godLevel; // 神化等级
        public int blessLevel; // 神佑等级
        public int grades;     //品阶
        public List<EquipSuperVO> Superitems = new List<EquipSuperVO>(); // 卓越属性列表
        public List<EquipNewSuperVO> NewSuperitems = new List<EquipNewSuperVO>(); // 新卓越属性列表

        public EquipSuperVO GetEquipSuperVO(long uid)
        {
            EquipSuperVO v = Superitems.Find(s => s.uid == uid);
            return v;
        }

        public void AddEquipSuperVOlua(long uid, int id, int val)
        {
            EquipSuperVO v = new EquipSuperVO();
            v.uid = uid;
            v.id = id;
            v.val = val;
            Superitems.Add(v);

        }
        public void AddEquipSuperVO(MsgData_sSuperVO data)
        {
            if (data == null)
                return;
            EquipSuperVO v = new EquipSuperVO();
            v.uid = data.uid;
            v.id = data.id;
            v.val = data.val;
            Superitems.Add(v);
        }
        public void UpdateEquipSuperVO(MsgData_sSuperVO data)
        {
            if (data == null)
                return;
            EquipSuperVO v = GetEquipSuperVO(data.uid);
            if (v == null)
                AddEquipSuperVO(data);
            else
            {
                v.id = data.id;
                v.val = data.val;
            }
        }

        public EquipNewSuperVO GetEquipNewSuperVO(int id)
        {
            EquipNewSuperVO v = NewSuperitems.Find(s => s.id == id);
            return v;
        }

        //true 有附加属性，false 无附加属性
        public bool HasEquipNewSuperVO()
        {
            bool res = false;
            for (int i = 0; i < NewSuperitems.Count; i++)
            {
                if (NewSuperitems[i].id != 0)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// 获取属性数量。
        /// </summary>
        /// <returns></returns>
        public int GetNewSuperNum()
        {
            int n = 0;
            for (int i = 0; i < NewSuperitems.Count; i++)
            {
                if (NewSuperitems[i].id != 0 & NewSuperitems[i].wash > 0)
                {
                    ++n;
                }
            }
            return n;
        }

        public void AddEquipNewSuperVOlua(int id, int wash)
        {
            EquipNewSuperVO v = new EquipNewSuperVO();
            v.id = id;
            v.wash = wash;
            NewSuperitems.Add(v);
        }
        public void AddEquipNewSuperVO(MsgData_sNewSuperVO data)
        {
            if (data == null)
                return;
            EquipNewSuperVO v = new EquipNewSuperVO();
            v.id = data.id;
            v.wash = data.wash;
            NewSuperitems.Add(v);
        }

        public void UpdateEquipNewSuperVO(MsgData_sNewSuperVO data)
        {
            EquipNewSuperVO v = GetEquipNewSuperVO(data.id);
            if (v == null)
                AddEquipNewSuperVO(data);
            else
            {
                v.wash = data.wash;
            }
        }


        public void UpdateEquipItemInfo(MsgData_sItemEquipVO data)
        {
            if (data == null) return;
            this.uid = data.uid;
            this.strenLvl = data.strenLvl;
            this.strenVal = data.strenVal;
            this.groupId = data.groupId;
            this.groupId2 = data.groupId2;
            this.groupId2Bind = data.groupId2Bind;
            this.group2Level = data.group2Level;
            this.godLevel = data.godLevel;
            this.blessLevel = data.blessLevel;

        }

        public void UpdateExtra(MsgData_sEquipExtraVO data)
        {
            if (data == null) return;
            this.uid = data.uid;
            this.attrAddLvl = data.level;
        }

        /// <summary>
        /// 他人身上装备信息
        /// </summary>
        /// <param name="data"></param>
        public void UpdateOtherHumanEquipVO(MsgData_sOtherHumanItemEquipVO data)
        {
            if (data == null) return;
            // this.uid = data.uid;
            this.strenLvl = data.StrenLevel;
            this.strenVal = data.StrenVal;
            this.groupId = data.GroupId;
            this.groupId2 = data.GroupId2;
            this.groupId2Bind = data.Bind;
            this.group2Level = data.GroupLevel;
            this.godLevel = data.GodLevel;
            this.blessLevel = data.BlessLevel;
            this.grades = data.grades;
        }

        /// <summary>
        /// 将数据写入缓冲区。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Pack(NetWriteBuffer buffer)
        {
            buffer.WriteInt64(uid);
            buffer.WriteInt32(strenLvl);
            buffer.WriteInt32(strenVal);
            buffer.WriteInt32(attrAddLvl);

            buffer.WriteInt32(groupId);
            buffer.WriteInt32(groupId2);
            buffer.WriteInt32(groupId2Bind);
            buffer.WriteInt32(group2Level);
            buffer.WriteInt32(superNum);
            buffer.WriteInt32(isyuangu);
            buffer.WriteInt32(godLevel);
            buffer.WriteInt32(blessLevel);

            buffer.WriteInt32(Superitems.Count);
            for (int i=0; i< Superitems.Count; ++i)
            {
                Superitems[i].Pack(buffer);
            }

            buffer.WriteInt32(NewSuperitems.Count);
            for (int i = 0; i < NewSuperitems.Count; ++i)
            {
                NewSuperitems[i].Pack(buffer);
            }
        }

        /// <summary>
        /// 从缓冲区中读取数据。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Uppack(NetReadBuffer buffer)
        {
            uid = buffer.ReadInt64();
            strenLvl = buffer.ReadInt32();
            strenVal = buffer.ReadInt32();
            attrAddLvl = buffer.ReadInt32();

            groupId = buffer.ReadInt32();
            groupId2 = buffer.ReadInt32();
            groupId2Bind = buffer.ReadInt32();
            group2Level = buffer.ReadInt32();
            superNum = buffer.ReadInt32();
            isyuangu = buffer.ReadInt32();
            godLevel = buffer.ReadInt32();
            blessLevel = buffer.ReadInt32();

            int n1 = buffer.ReadInt32();
            Superitems.Clear();
            for (int i=0; i<n1; ++i)
            {
                EquipSuperVO vo = new EquipSuperVO();
                vo.Uppack(buffer);
                Superitems.Add(vo);
            }

            int n2 = buffer.ReadInt32();
            NewSuperitems.Clear();
            for (int i = 0; i < n2; ++i)
            {
                EquipNewSuperVO vo = new EquipNewSuperVO();
                vo.Uppack(buffer);
                NewSuperitems.Add(vo);
            }
        }
    }


    // S->C 卓越属性列表
    [LuaCallCSharp]
    [Hotfix]
    public class EquipSuperVO
    {
        public long uid; // 属性uid
        public int id; // 卓越id
        public int val; // 值1

        /// <summary>
        /// 将数据写入缓冲区。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Pack(NetWriteBuffer buffer)
        {
            buffer.WriteInt64(uid);
            buffer.WriteInt32(id);
            buffer.WriteInt32(val);
        }

        /// <summary>
        /// 从缓冲区中读取数据。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Uppack(NetReadBuffer buffer)
        {
            uid = buffer.ReadInt64();
            id = buffer.ReadInt32();
            val = buffer.ReadInt32();
        }
    }

    // S->C 新卓越属性列表;
    [LuaCallCSharp]
    [Hotfix]
    public class EquipNewSuperVO
    {
        public int id; // 新卓越id;
        public int wash; // // 洗练值;

        /// <summary>
        /// 将数据写入缓冲区。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Pack(NetWriteBuffer buffer)
        {
            buffer.WriteInt32(id);
            buffer.WriteInt32(wash);
        }

        /// <summary>
        /// 从缓冲区中读取数据。
        /// </summary>
        /// <param name="buffer">数据缓冲区。</param>
        public void Uppack(NetReadBuffer buffer)
        {
            id = buffer.ReadInt32();
            wash = buffer.ReadInt32();
        }
    }


    [LuaCallCSharp]
    [Hotfix]
    public class EquipDataMgr
    {
        //private static EquipDataMgr _instance = null;
        public static EquipDataMgr Instance
        {
            get
            {
                return PlayerData.Instance.EquipMgr;
            }
        }

        private Dictionary<int, LuaTable> m_EquipConfig = null;//装备表
        private Dictionary<int, LuaTable> m_EquipStarupConfig = null;//升星表
        //装备位置信息
        public Dictionary<int, EquipPosInfo> mEquipPosInfoData = new Dictionary<int, EquipPosInfo>();
        //装备属性信息
        public Dictionary<long, EquipAttInfo> mEquipAttinfo = new Dictionary<long, EquipAttInfo>();
        //装备选中最小星级
        public int selectstar = 0;

        public void ResisterMsg()
        {
            //装备位置信息
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_POSINFO, OnEquipPosInfo);
            //返回添加装备附加信息
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_EQUIPADD, OnEquipAdd);
            //返回装备卓越信息
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_EQUIPSUPER, OnEquipSuper);
            //返回装备追加属性信息
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_EQUIPEXTRA, OnEquipExtra);
            //返回装备新卓越信息
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_EQUIPNEWSUPER, OnEquipNewSuper);
            // 返回装备附加信息 msgId:8131;
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_EQUIPINFO, OnEquipInfo);

            //删除物品 
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BAG_ITEM_DEL, OnItemDel);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EquipGroup, OnEquipGroup);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EquipGroupTwo, OnEquipGroupTwo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EquipSmelt, OnEquipSmelt);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EquipGroupPeel, OnEquipGroupPeel);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_EquipGroupLvlUp, OnEquipGroupLvlUp);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EQUIP_STARSELECT, onEquipStarSelect);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EquipPeiYang, OnEquipPeiYang);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_EquipPeiYangSet, OnEquipPeiYangSet);
            m_EquipConfig = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_equip");
            m_EquipStarupConfig = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_equipstarup");

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnClearData);
           
        }
        public void OnClearData(GameEvent ge, EventParameter paramter)
        {
            mEquipPosInfoData.Clear();
            mEquipAttinfo.Clear();
        }

        public void onEquipStarSelect(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipPosStarSelect dataNew = paramter.msgParameter as MsgData_sEquipPosStarSelect;
            if (dataNew.result == 0)
            {
                selectstar = dataNew.star;
            }
        }
        //更新装备背包数据
        public void OnEquipPosInfo(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipPosInfo data = paramter.msgParameter as MsgData_sEquipPosInfo;
            selectstar = data.selectstar;

            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_sEquipData sed = data.items[i];
                EquipPosInfo itemInfo = GetEquipPosInfo(sed.pos);
                if (itemInfo != null)
                {
                    itemInfo.UpdateEquipInfo(sed);
                }
            }
        }
        //根据装备位置获取位置信息
        public EquipPosInfo GetEquipPosInfo(int pos)
        {
            if (!mEquipPosInfoData.ContainsKey(pos))
            {
                EquipPosInfo posInfo = new EquipPosInfo();
                posInfo.level = posInfo.exp = posInfo.starLevel =  0;
                mEquipPosInfoData[pos] = posInfo;
                return posInfo;
            }
            else
            {
                return mEquipPosInfoData[pos];
            }
        }

        //获取装备位最小星级
        public int GetMinEqupStar()
        {
            int starLevel = -1;
            for (int i = 0; i < 10; i++)
            {
                EquipPosInfo pos = GetEquipPosInfo(i);
                if(starLevel == -1)
                {
                    starLevel = pos.starLevel;
                }
                else
                {
                    starLevel = Math.Min(pos.starLevel, starLevel);
                } 
            } 
            return starLevel;
        }


        //////////////////////装备属性信息///////////////////////////////////////

        //获取装备扩展属性
        public EquipAttInfo EquipAttrInfo(long uid)
        {
            EquipAttInfo info = null;
            if (mEquipAttinfo.ContainsKey(uid))
            {
                info = mEquipAttinfo[uid];
            }
            return info;
        }



        //根据装备uid获取属性信息,没有就创建
        public EquipAttInfo GetEquipAttrInfo(long uid)
        {
            EquipAttInfo info = null;
            if (!mEquipAttinfo.ContainsKey(uid))
            {
                info = new EquipAttInfo();
                mEquipAttinfo.Add(uid, info);
            }
            else
            {
                info = mEquipAttinfo[uid];
            }
            return info;
        }

        public void DelEquipAttrInfo(long uid)
        {
            if (mEquipAttinfo.ContainsKey(uid))
            {
                mEquipAttinfo.Remove(uid);
            }
        }

        // //返回添加装备附加信息
        public void OnEquipAdd(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipAdd data = paramter.msgParameter as MsgData_sEquipAdd;
            EquipAttInfo info = GetEquipAttrInfo(data.uid);
            if (info == null) return;
            info.uid = data.uid;
            info.strenLvl = data.strenLvl;
            info.strenVal = data.strenVal;
            info.attrAddLvl = data.attrAddLvl;
            info.groupId = data.groupId;
            info.groupId2 = data.groupId2;
            info.groupId2Bind = data.groupId2Bind;
            info.group2Level = data.group2Level;
            info.superNum = data.superNum;
            info.isyuangu = data.isyuangu;
            info.godLevel = data.godLevel;
            info.blessLevel = data.blessLevel;
            info.grades = data.grades;
            for (int i = 0; i < data.Superitems.Length; i++)
            {
                info.AddEquipSuperVO(data.Superitems[i]);
            }

            for (int i = 0; i < data.NewSuperitems.Length; i++)
            {
                info.AddEquipNewSuperVO(data.NewSuperitems[i]);
            }

        }

        //返回装备卓越信息
        public void OnEquipSuper(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipSuper data = paramter.msgParameter as MsgData_sEquipSuper;
            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_sEquipSuperListVO item = data.items[i];
                EquipAttInfo info = GetEquipAttrInfo(item.uid);
                if(info!=null)
                {
                    info.superNum = item.superNum;
                    for (int j = 0; j < item.items.Length; j++)
                    {
                        info.UpdateEquipSuperVO(item.items[j]);
                    }
                }

            }

        }
        //返回装备追加属性信息
        public void OnEquipExtra(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipExtra data = paramter.msgParameter as MsgData_sEquipExtra;
            for (int i = 0; i < data.items.Count; i++)
            {
                EquipAttInfo info = GetEquipAttrInfo(data.items[i].uid);
                if(info!=null)
                info.UpdateExtra(data.items[i]);
            }
        }
        //返回装备新卓越信息
        public void OnEquipNewSuper(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipNewSuper data = paramter.msgParameter as MsgData_sEquipNewSuper;
            for (int i = 0; i < data.items.Count; i++)
            {
                MsgData_sEquipNewSuperListVO item = data.items[i];
                EquipAttInfo info = GetEquipAttrInfo(item.uid);
                if(info!=null)
                {
                    for (int j = 0; j < item.items.Length; j++)
                    {
                        info.UpdateEquipNewSuperVO(item.items[j]);
                    }
                }
            }
        }
        //返回装备附加信息
        public void OnEquipInfo(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipInfo data = paramter.msgParameter as MsgData_sEquipInfo;
            for (int i = 0; i < data.items.Count; i++)
            {
                EquipAttInfo info = GetEquipAttrInfo(data.items[i].uid);
                if(info!=null)
                info.UpdateEquipItemInfo(data.items[i]);
            }
        }

        public void OnItemDel(GameEvent ge, EventParameter paramter)
        {
            int type = paramter.intParameter2;
            if (type == 1)
            {
                long uid = (long)paramter.objParameter;
                DelEquipAttrInfo(uid);
            }
        }

        #region  装备套装相关

        /// <summary>
        /// 获取当前套装类型的 已穿戴套装件数
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public int GetEquipCurGroupNum(int groupId)
        {
            int sum = 0;
            BagInfo bagInfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_EQUIP);
            if(bagInfo==null) return sum;
            //List<ItemInfo> equipList = new List<ItemInfo>();
            foreach (var item in bagInfo.ItemInfos)
            {
                if (mEquipAttinfo[item.Value.UID].groupId == groupId)
                {
                    sum += 1;
                }
            }
            return sum;
        }

        public void Send_CC_MyEquipSelectClick(object obj)
        {
            EventParameter par = EventParameter.Get();
            par.objParameter = obj;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_MyEquipSelectClick, par);
        }

        /// <summary>
        /// 客户端请求:设置装备套装 msgId:3464(装备铸炼功能)
        /// </summary>
        public void Send_CS_EquipGroup(long equipId, long itemUid)
        {
            MsgData_cEquipGroup rsp = new MsgData_cEquipGroup();
            rsp.equipUid = equipId;
            //rsp.groupId = groupId;
            rsp.itemUid = itemUid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipGroup, rsp);
        }

        /// <summary>
        /// 客户端请求:设置装备套装 msgId:3564
        /// </summary>
        public void Send_CS_EquipGroupTwo(long equipId, int groupId, int isBind = 0)
        {
            MsgData_cEquipGroupTwo rsp = new MsgData_cEquipGroupTwo();
            rsp.equipUid = equipId;
            rsp.groupId = groupId;
            rsp.isBind = isBind;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipGroupTwo, rsp);
        }


        /// <summary>
        /// 客户端请求:装备熔炼 msgId:3514
        /// </summary>
        public void Send_CS_EquipSmelt(int flag, int needId, int needNum = 1)
        {
            MsgData_cEquipSmelt rsp = new MsgData_cEquipSmelt();
            rsp.flags = flag;
            MsgData_cEquipSmeltList v = new MsgData_cEquipSmeltList();
            v.uid = needId;
            rsp.list = new List<MsgData_cEquipSmeltList>();
            rsp.list.Add(v);
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipSmelt, rsp);
        }

        /// <summary>
        /// 客户端请求：套装升级 msgId:3685
        /// </summary>
        public void Send_CS_EquipGroupLvlUp(long equipUid, int type = 1)
        {
            MsgData_cEquipGroupLvlUp rsp = new MsgData_cEquipGroupLvlUp();
            rsp.equipUid = equipUid;
            rsp.isBind = 0;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipGroupLvlUp, rsp);
        }

        /// <summary>
        /// 客户端请求：剥离装备套装 msgId:3558
        /// </summary>
        public void Send_CS_EquipGroupPeel(long equipUid)
        {
            MsgData_cEquipGroupPeel rsp = new MsgData_cEquipGroupPeel();
            rsp.equipUid = equipUid;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_EquipGroupPeel, rsp);
        }

        public void OnEquipGroup(GameEvent ge, EventParameter paramter)
        {
            //  Debug.LogError("receive equipgroup");
            MsgData_sEquipGroup resp = paramter.msgParameter as MsgData_sEquipGroup;
            if (resp.result == 0)
            {
                LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(resp.itemId);
                if (cfg == null) return;
                int groupId = cfg.Get<int>("use_param_1");
                /*
                MsgData_sItemEquipVO vo = new MsgData_sItemEquipVO();
                vo.uid = resp.equipUid;
                vo.groupId = groupId;
                EquipAttInfo info = GetEquipAttrInfo(resp.equipUid);
                info.groupId = vo.groupId;
                info.uid = vo.uid;
                mEquipAttinfo[vo.uid] = info;
                */
                Send_CS_EquipGroupTwo(resp.equipUid, groupId);
            }
            else
            {
                //    Debug.LogError("resp.result == " + resp.result);
            }
            //  CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_EquipGroupUpdate, paramter);
        }

        public void OnEquipGroupTwo(GameEvent ge, EventParameter paramter)
        {
            //Debug.LogError("receive equipgroupTwo");
            MsgData_sEquipGroupTwo resp = paramter.msgParameter as MsgData_sEquipGroupTwo;
            if (resp.result == 0)
            {
                MsgData_sItemEquipVO vo = new MsgData_sItemEquipVO();
                vo.uid = resp.equipUid;
                vo.groupId = resp.groupId;
                EquipAttInfo info = GetEquipAttrInfo(resp.equipUid);
                if(info!=null)
                {
                    info.groupId = vo.groupId;
                    info.uid = vo.uid;
                    mEquipAttinfo[vo.uid] = info;
                }

                //  Debug.LogError("equipUid  " + resp.equipUid);
            }
            else
            {
                //     Debug.LogError("resp.result == " + resp.result);
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_EquipGroupUpdate, paramter);
        }

        public void OnEquipSmelt(GameEvent ge, EventParameter paramter)
        {
            // Debug.LogError("receive equipSmelt");
            MsgData_sEquipSmelt resp = paramter.msgParameter as MsgData_sEquipSmelt;
            if (resp.result == 0)
            {
                //     Debug.LogError("equipUid  " + resp.equipUid);
            }
            else
            {
                //        Debug.LogError("resp.result == " + resp.result);
            }
        }


        public void OnEquipGroupLvlUp(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipGroupLvlUp resp = paramter.msgParameter as MsgData_sEquipGroupLvlUp;
            if (resp.result == 0)
            {
                if (mEquipAttinfo.ContainsKey(resp.equipUid))
                {
                    mEquipAttinfo[resp.equipUid].group2Level = resp.equipLevel;
                }
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_EquipGroupUpdate, paramter);
        }

        public List<EquipNewSuperVO> PeiYangitems = new List<EquipNewSuperVO>(); // 新卓越属性列表
        public void OnEquipPeiYang(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipPeiYang data = paramter.msgParameter as MsgData_sEquipPeiYang;
            PeiYangitems.Clear();
            if (data.result == 0)
            {
                EquipAttInfo attInfo = EquipAttrInfo(data.id);
                if (attInfo == null) return;
                for (int i = 0; i < attInfo.NewSuperitems.Count; i++)
                {
                    EquipNewSuperVO item = new EquipNewSuperVO();
                    item.id = attInfo.NewSuperitems[i].id;
                    item.wash = 0;
                    MsgData_sNewSuperVO super = data.items.Find(s => s.id == attInfo.NewSuperitems[i].id);
                    if (super != null)
                    {
                        item.wash = super.wash;
                    }
                    PeiYangitems.Add(item);
                }
            }
        }
        public void OnEquipPeiYangSet(GameEvent ge, EventParameter paramter)
        {

        }

        public void OnEquipGroupPeel(GameEvent ge, EventParameter paramter)
        {
            MsgData_sEquipGroupPeel resp = paramter.msgParameter as MsgData_sEquipGroupPeel;
            if (resp.result == 0)
            {
                if (mEquipAttinfo.ContainsKey(resp.equipUid))
                {
                    mEquipAttinfo[resp.equipUid].groupId = 0;
                    mEquipAttinfo[resp.equipUid].group2Level = 0;
                }
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_EquipGroupUpdate, paramter);
        }
        #endregion


        /// <summary>
        /// 玩家装备附加组中 取 当前装备的附加属性
        /// </summary>
        /// <param name="equipUid"></param>
        /// <param name="attrInfoList"></param>
        /// <returns></returns>
        public EquipAttInfo GetCurEquipAttInfo(long equipUid, Dictionary<long, EquipAttInfo> attrInfoList)
        {
            if (attrInfoList.ContainsKey(equipUid))
            {
                return attrInfoList[equipUid];
            }
            return null;
        }

        /// <summary>
        /// 获取已穿戴的套装装备是否有当前位置的装备
        /// 返回 >0 的值， 当前位置有穿戴
        /// </summary>
        /// <param name="groupList"></param>
        /// <returns></returns>
        public bool IsEquipGroupListPos(int Pos, Dictionary<long, int> groupPosList)
        {
            bool isEquip = false;
            foreach (var item in groupPosList)
            {
                if (item.Value == Pos)
                {
                    isEquip = true;
                    break;
                }
            }
            return isEquip;
        }

        /// <summary>
        ///  true 表示穿戴在身上，  
        /// </summary>
        /// <param name="iteminfo"></param>
        /// <param name="equipList"></param>
        /// <returns></returns>
        public bool IsWearEquip(long equipUid, List<long> equipList)
        {
            if (equipList.Contains(equipUid))
            {
                return true;
            }
            return false;
        }

        public ItemInfo GetCurPosWearEquip(int pos)
        {
            BagInfo bagInfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_EQUIP);
            if(bagInfo!=null)
            {
                foreach (var item in bagInfo.ItemInfos)
                {
                    if (item.Value.Pos == pos)
                    {
                        return item.Value;
                    }
                }
            }

            return null;
        }

        private EquipTipInfo CreatEquipTipInfo()
        {
            return new EquipTipInfo();
        }


        /// <summary>
        /// 获取当前装备的宝石信息
        /// </summary>
        /// <param name="info"></param>
        public Dictionary<long, ItemInfo> GetCurItemInfoStoneData(ItemInfo info)
        {
            Dictionary<long, ItemInfo> stoneIdList = new Dictionary<long, ItemInfo>();
            if (info.Bag != BagType.ITEM_BAG_TYPE_EQUIP)
            {
                return stoneIdList;  //非身上穿戴
            }
            stoneIdList = GetCurItemInfoStoneDataByPos(info.Pos);
            return stoneIdList;
        }

        /// <summary>
        /// 根据背包位置获取镶嵌列表
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Dictionary<long, ItemInfo> GetCurItemInfoStoneDataByPos(int pos)
        {
            Dictionary<long, ItemInfo> stoneIdList = new Dictionary<long, ItemInfo>();
            int from = pos * 4;
            for (int i = 0; i < 7; i++)
            {
                int index = from + i;
                if(i>= 4 )
                {
                    index = 40 + i - 4 + pos * 3;
                }
                ItemInfo tt = BagData.Instance.GetItemInfo(BagType.ITEM_BAG_TYPE_GEM, index);
                if (tt != null)
                {
                    stoneIdList[tt.UID] = tt;
                }
            }
            return stoneIdList;
        }


        /// <summary>
        /// 获取玩家装备附加组中 当前套装类型的 已激活的套装最高等级
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetEquipCurGroupLevel(int groupId, Dictionary<long, ItemInfo> equipGroupList, Dictionary<long, EquipAttInfo> attrList)
        {

            Dictionary<int, int> levels = new Dictionary<int, int>();   //每个区间段套装等级计数
            foreach (var item in equipGroupList)
            {
                int level = attrList[item.Value.UID].group2Level;
                if (levels.ContainsKey(level))
                {
                    levels[level]++;
                }
                else
                {
                    levels[level] = 1;
                }
            }
            List<KeyValuePair<int, int>> lstorder = levels.OrderBy(c => c.Key).ToList();  // 升序排序
            Dictionary<int, int> list = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> item in lstorder)
            {
                list.Add(item.Key, item.Value);
            }
            return list;
        }

        /// <summary>
        /// 获取装备穿戴位置
        /// </summary>
        /// <param name="id"></param>
        /// <returns>-1表示表格中没有找到此装备位置,其它值表示当前装备位置</returns>
        public int GetEquipItemPos(int id)
        {
            if (m_EquipConfig == null)
                return -1;

            LuaTable t;
            if (m_EquipConfig.TryGetValue(id, out t))
            {
                return t.Get<int>("pos");
            }

            return -1;
        }
        /// <summary>
        /// 根据装备获取装备升星流光段数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public int GetEquipStarByEquipLevel(int equipid, int equipstarid)
        {
            if (m_EquipStarupConfig == null)
                return 0;
            if (m_EquipConfig == null)
                return 0;

            LuaTable t;
            if (m_EquipStarupConfig.TryGetValue(equipstarid, out t))
            {
                LuaTable tequiplua = null;
                if (m_EquipConfig.TryGetValue(equipid, out tequiplua))
                {
                    int level = tequiplua.Get<int>("level");
                    if (level > 0)
                    {
                        return t.Get<int>(string.Format("effect_level{0}", level));
                    }
                }
            }

            return 0;
        }
        /// <summary>
        /// 根据升星表获取流光模型id
        /// </summary>
        /// <param name="job"></param>
        /// <param name="equipstarid"></param>
        /// <returns></returns>
        public int GetEquipStarModelID(int job,int equipstarid)
        {
            if (m_EquipStarupConfig == null)
                return 0; 
            LuaTable t;
            if (m_EquipStarupConfig.TryGetValue(equipstarid, out t))
            { 
               return t.Get<int>(string.Format("model{0}", job)); 
            }

            return 0;
        }


        /// <summary>
        /// 获取时间装备升星流光段数
        /// </summary>
        /// <param name="equipstarid"></param>
        /// <returns></returns>
        public int GetFashionStar(int equipstarid)
        {
            if (m_EquipStarupConfig == null)
                return 0;
            LuaTable t;
            if (m_EquipStarupConfig.TryGetValue(equipstarid, out t))
            {
                return t.Get<int>("fashions_effectlv");
            }
            return 0;
        }

        /// <summary>
        /// 获取玩家自己的装备tip数据
        /// </summary>
        /// <param name="iteminfo"></param>
        /// <returns></returns>
        public EquipTipInfo GetMyTipInfo(ItemInfo iteminfo)
        {
            BagInfo bagInfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_EQUIP);
            List<ItemInfo> equipList = new List<ItemInfo>();
            if(bagInfo!=null)
            {
                foreach (var item in bagInfo.ItemInfos)
                {
                    equipList.Add(item.Value);
                }
            }

            EquipPosInfo posInfo = new EquipPosInfo();
            if (iteminfo.Bag == BagType.ITEM_BAG_TYPE_EQUIP)  //是否已穿戴
            {
                if (mEquipPosInfoData.ContainsKey(iteminfo.Pos))
                {
                    posInfo = mEquipPosInfoData[iteminfo.Pos];
                }
            }
            else
            {
                int pos = GetEquipItemPos(iteminfo.ID);
                if (pos != -1)
                {
                    if (mEquipPosInfoData.ContainsKey(pos))
                    {
                        posInfo = mEquipPosInfoData[pos];
                    }
                }
            }
            return GetTipInfo(iteminfo, equipList, mEquipAttinfo, posInfo);
        }

        //装备传承tips，需显示itemInfo，但附加属性显示的是itemSuper里的值
        public EquipTipInfo GetMyTipInfoBless(ItemInfo iteminfo, ItemInfo itemSuper)
        {
            BagInfo bagInfo = PlayerData.Instance.BagData.GetBagInfo(BagType.ITEM_BAG_TYPE_EQUIP);
            if(bagInfo!=null)
            {
                List<ItemInfo> equipList = new List<ItemInfo>();
                foreach (var item in bagInfo.ItemInfos)
                {
                    equipList.Add(item.Value);
                }
                EquipPosInfo posInfo = new EquipPosInfo();
                if (mEquipPosInfoData.ContainsKey(iteminfo.Pos))
                {
                    posInfo = mEquipPosInfoData[iteminfo.Pos];
                }
                EquipTipInfo tipsInfo = GetTipInfo(iteminfo, equipList, mEquipAttinfo, posInfo);
                tipsInfo.attrInfo = GetCurEquipAttInfo(itemSuper.UID, mEquipAttinfo);
                return tipsInfo;
            }
            return null;

        }



        /// <summary>
        /// 获取自己或者公共的详细装备tip数据
        /// 
        /// iteminfo --当前装备属性
        /// equipList --已穿戴在身上的装备列表
        /// EquipAttinfo --玩家拥有的所有装备的额外属性(已穿戴和未穿戴的)
        /// posInfo  - 当前装备的 装备位
        /// </summary>
        /// <returns></returns>
        public EquipTipInfo GetTipInfo(ItemInfo iteminfo, List<ItemInfo> equipList, Dictionary<long, EquipAttInfo> EquipAttinfo, EquipPosInfo posInfo)
        {
            EquipTipInfo info = CreatEquipTipInfo();
            Dictionary<long, ItemInfo> list = new Dictionary<long, ItemInfo>();
            List<long> equipPosList = new List<long>();
            foreach (var item in equipList)
            {
                list[item.UID] = item;
                if(!equipPosList.Contains(item.UID))
                {
                    equipPosList.Add(item.UID);
                }
            }
            Dictionary<long, ItemInfo> groupList = new Dictionary<long, ItemInfo>();
            if (EquipAttinfo.ContainsKey(iteminfo.UID))
            {
                info.groupId = EquipAttinfo[iteminfo.UID].groupId;
            }
            if (info.groupId > 0)
            {
                foreach (var item in EquipAttinfo)
                {
                    if (item.Value.groupId == info.groupId && list.ContainsKey(item.Value.uid))
                    {
                        groupList[item.Value.uid] = list[item.Value.uid];  //取当前套装组 的已穿戴装备
                    }
                }
                info.groupLevels = GetEquipCurGroupLevel(info.groupId, groupList, EquipAttinfo); //取当前套装已穿戴装备的所有等级，及对应等级的套装数量
            }
            // Debug.LogError("当前套装组穿戴数量" + groupList.Count);
            if (info.groupLevels.Count > 0)
            {
                LuaTable cfg = ConfigManager.Instance.BagItem.GetEquipGroupConfig(info.groupId);
                if(cfg!=null)
                {
                    string attrlevels = cfg.Get<string>("attrList");
                    if (!string.IsNullOrEmpty(attrlevels))
                    {
                        string[] strList = attrlevels.Split(',');
                        int isActive = 0;
                        int unMinLevel = -1;
                        int curLevel = 0;
                        int curLevelCount = 0;
                        int sum = 0;
                        foreach (var item in info.groupLevels)
                        {
                            if (unMinLevel < 0)
                            {
                                unMinLevel = item.Key;
                            }
                            else if (unMinLevel > item.Key)
                            {
                                unMinLevel = item.Key;
                            }
                            if (isActive > 0)
                            {
                                break;
                            }
                            int lessNum = groupList.Count - sum;
                            for (int i = strList.Length - 1; i >= 0; i--)
                            {
                                int count = Convert.ToInt32(strList[i]);
                                if (lessNum >= count)
                                {
                                    curLevel = item.Key;
                                    curLevelCount = lessNum;
                                    isActive++;
                                    break;
                                }
                            }
                            sum += item.Value;
                        }
                        if (isActive > 0)   //取已激活的 等级和数量，  按数量高低取值，优先取 数量最多的那个等级
                        {
                            info.isActive = isActive;
                            info.activeGroupLevel = curLevel;
                            info.activeGroupCount = curLevelCount;
                        }
                        else    //未激活，取最高套装等级的 那一列
                        {
                            info.isActive = 0;
                            info.activeGroupLevel = unMinLevel;
                            info.activeGroupCount = groupList.Count;
                        }
                    }
                }
 
            }
            Dictionary<long, int> groupPosList = new Dictionary<long, int>();
            foreach (var item in groupList)
            {
                groupPosList[item.Key] = item.Value.Pos;
            }
            info.isWear = IsWearEquip(iteminfo.UID, equipPosList) == true ? 1 : 0;
            info.itemInfo = iteminfo;
            info.attrInfo = GetCurEquipAttInfo(iteminfo.UID, EquipAttinfo);
            // info.equipList = list;
            // info.equipGroupList = groupList;
            info.equipPosList = equipPosList;
            info.equipGroupPosList = groupPosList;
            info.equipGroupListCount = groupList.Count;
            //info.attrInfoList = EquipAttinfo;
            info.posInfo = posInfo;
            //装备宝石在普通背包也需要对比
            int pos = GetEquipItemPos(info.itemInfo.ID);
            if(pos > -1)
            {
                info.stoneList = GetCurItemInfoStoneDataByPos(pos);
            }
            else
            {
                info.stoneList = GetCurItemInfoStoneDataByPos(info.itemInfo.Pos);
            }
            info.stoneListCount = info.stoneList.Count;
            return info;
        }


        /// <summary>
        /// 获取他人身上穿戴的装备TIP数据
        /// </summary>
        /// <param name="tt"></param>
        public EquipTipInfo GetOtherTipInfo(MsgData_sOtherHumanBSInfoRet tt, MsgData_sOtherHumanItemEquipVO cur, int pos)
        {
 
            EquipTipInfo info = CreatEquipTipInfo();
            if (cur == null)   //当前装备位是空。。 没有穿戴
            {
                return info;
            }
            ItemInfo iteminfo = new ItemInfo();
            iteminfo.Init(cur, pos);
            info.itemInfo = iteminfo;
            EquipAttInfo attrInfo = new EquipAttInfo();
            attrInfo.UpdateOtherHumanEquipVO(cur);
            info.isWear = 1;   //大于0表示身上的穿戴
            info.itemInfo = iteminfo;
            info.attrInfo = attrInfo;
            info.groupId = cur.GroupId;
            Dictionary<long, ItemInfo> groupList = new Dictionary<long, ItemInfo>();
            if (info.groupId > 0 &&tt != null && tt.ItemEquipCount > 0)
            {
                Dictionary<int, int> levels = new Dictionary<int, int>();   //每个区间段套装等级计数
                //int groupCount = 0;
                foreach (var item in tt.ItemEquipList)
                {
                    if (item.GroupId == info.groupId)
                    {
                        LuaTable cfg = ConfigManager.Instance.BagItem.GetItemConfig(item.TID);
                        if(cfg!=null)
                        {
                            int _pos = cfg.Get<int>("pos");
                            ItemInfo equipInfo = new ItemInfo();
                            equipInfo.Init(item, _pos);
                            groupList[item.TID] = equipInfo;  //取当前套装组 的已穿戴装备
                                                              //groupCount++;
                            int level = item.GroupLevel;
                            if (levels.ContainsKey(level))
                            {
                                levels[level]++;
                            }
                            else
                            {
                                levels[level] = 1;
                            }
                        }
                     
                    }
                }
                List<KeyValuePair<int, int>> lstorder = levels.OrderBy(c => c.Key).ToList();  // 升序排序
                Dictionary<int, int> list = new Dictionary<int, int>();
                foreach (KeyValuePair<int, int> item in lstorder)
                {
                    list.Add(item.Key, item.Value);
                }
                Dictionary<long, int> groupPosList = new Dictionary<long, int>();
                foreach (var item in groupList)
                {
                    groupPosList[item.Key] = item.Value.Pos;
                }
                info.groupLevels = list; //取当前套装已穿戴装备的所有等级，及对应等级的套装数量
                info.equipGroupListCount = groupList.Count;
                // info.equipGroupList = groupList;
                info.equipGroupPosList = groupPosList;
            }
            if (info.groupLevels.Count > 0)
            {
                LuaTable cfg = ConfigManager.Instance.BagItem.GetEquipGroupConfig(info.groupId);
                if(cfg!=null)
                {
                    string attrlevels = cfg.Get<string>("attrList");
                    if (!string.IsNullOrEmpty(attrlevels))
                    {
                        string[] strList = attrlevels.Split(',');
                        int isActive = 0;
                        int unMinLevel = -1;
                        int curLevel = 0;
                        int curLevelCount = 0;
                        int sum = 0;
                        foreach (var item in info.groupLevels)
                        {
                            if (unMinLevel < 0)
                            {
                                unMinLevel = item.Key;
                            }
                            else if (unMinLevel > item.Key)
                            {
                                unMinLevel = item.Key;
                            }
                            if (isActive > 0)
                            {
                                break;
                            }
                            int lessNum = info.equipGroupListCount - sum;
                            for (int i = strList.Length - 1; i >= 0; i--)
                            {
                                int count = Convert.ToInt32(strList[i]);
                                if (lessNum >= count)
                                {
                                    curLevel = item.Key;
                                    curLevelCount = lessNum;
                                    isActive++;
                                    break;
                                }
                            }
                            sum += item.Value;
                        }
                        if (isActive > 0)   //取已激活的 等级和数量，  按数量高低取值，优先取 数量最多的那个等级
                        {
                            info.isActive = isActive;
                            info.activeGroupLevel = curLevel;
                            info.activeGroupCount = curLevelCount;
                        }
                        else    //未激活，取最高套装等级的 那一列
                        {
                            info.isActive = 0;
                            info.activeGroupLevel = unMinLevel;
                            info.activeGroupCount = info.equipGroupListCount;
                        }
                    }
                }

            }
            //info.posInfo = posInfo;
            //info.stoneList = GetCurItemInfoStoneData(info.itemInfo);
            //info.stoneListCount = info.stoneList.Count;
            return info;
        }


    }

    [LuaCallCSharp]
    [Hotfix]
    public class EquipTipInfo
    {
        public ItemInfo itemInfo; //(lua用到)当前选中的装备

        public EquipAttInfo attrInfo;  //(lua用到)当前选中装备的附加属性,没有则为 null 

        public int isWear;   //(lua用到)大于0 - 装备穿戴在身上， 小于等于0 ，装备不在身上

        ////已弃用
        //public Dictionary<long, ItemInfo> equipList; //已穿戴的装备
        ////已弃用
        //public Dictionary<long, EquipAttInfo> attrInfoList;  //所有装备的附加属性 

        //新增 已穿戴装备的UID
        public List<long> equipPosList; //已穿戴的装备 

        public int groupId;  //(lua用到)套装组id ,  groupId = 0时，下面的list 都为空   (lua用到)

        public int isActive;       //(lua用到)大于0 - 激活，小于等于0,未激活 ,未激活时,activeGroupLevel 取的未激活的套装最低装等,  activeGroupCount最低装等数量

        public int activeGroupLevel; //(lua用到)激活的套装等级（顺序- 从激活套装组要求的数量来决定当前等级， 比如3，4，5，6件套。最低的一件装备是1级, 
                                     //                       如果6件都超过1级了，4件超过20级了,3件超过50级
                                     //                    取6件套的 里面最低等级 对应的套装属性（如果有件是1级，取1级套装属性），
                                     //                     不取4件套的20级套装属性。。 策划要求。。。特此备注说明    

        public int activeGroupCount;  //(lua用到)激活该套装等级对应的套装数量       

        //新加套装组穿戴位置
        public Dictionary<long, int> equipGroupPosList;//(lua用到)已穿戴当前套装组的装备 ,  key -查看自己的装备，key 是uid.  .  查看别人装备 key存的是配置表id, 
                                                       //查看别人装备的msg数据里，服务器没有下发uid,所以 只能存tid 处理  , value - 已穿戴的当前套装组的装备位置

        ////已弃用
        //public Dictionary<long, ItemInfo> equipGroupList; //(lua用到)已穿戴当前套装组的装备 ,  key -查看自己的装备，key 是uid.  .  查看别人装备 key存的是配置表id, 
        //                                                  //查看别人装备的msg数据里，服务器没有下发uid,所以 只能存tid 处理

        public int equipGroupListCount;    //(lua用到)已穿戴当前套装组的装备数量

        public Dictionary<int, int> groupLevels = new Dictionary<int, int>();   //当前套装已穿戴装备的所有等级，及对应等级的套装数量(降序排序,第一个元素是最高级)

        public EquipPosInfo posInfo;  //(lua用到)装备位 

        public Dictionary<long, ItemInfo> stoneList = new Dictionary<long, ItemInfo>();  //(lua用到)当前装备的宝石信息

        public int stoneListCount;   //(lua用到)当前装备的宝石个数
                                     //public EquipTipType type;  


        /// <summary>
        /// 将对象数据序列化成字符串。
        /// </summary>
        /// <returns>字符串内容。</returns>
        public string Serialize()
        {
            CacheWriter.Reset();
            itemInfo.Pack(CacheWriter);
            CacheWriter.WriteInt32(attrInfo == null ? 0 : 1);       //标记是否有attrInfo
            if (attrInfo != null)
            {
                attrInfo.Pack(CacheWriter);
            }

            CacheWriter.WriteInt32(isWear);
            CacheWriter.WriteInt32(groupId);
            CacheWriter.WriteInt32(isActive);
            CacheWriter.WriteInt32(activeGroupLevel);
            CacheWriter.WriteInt32(activeGroupCount);

            CacheWriter.WriteInt32(equipPosList == null ? 0 : 1);       //标记是否有equipPosList
            if (equipPosList != null)
            {
                CacheWriter.WriteInt32(equipPosList.Count);
                for (int i=0; i< equipPosList.Count; ++i)
                {
                    CacheWriter.WriteInt64(equipPosList[i]);
                }
            }

            CacheWriter.WriteInt32(equipGroupPosList == null ? 0 : 1);       //标记是否有equipGroupPosList
            if (equipGroupPosList != null)
            {
                CacheWriter.WriteInt32(equipGroupPosList.Count);
                var e = equipGroupPosList.GetEnumerator();
                while (e.MoveNext())
                {
                    var kvp = e.Current;
                    CacheWriter.WriteInt64(kvp.Key);
                    CacheWriter.WriteInt32(kvp.Value);
                }
                e.Dispose();
            }

            //CacheWriter.WriteInt32(equipList == null ? 0 : 1);       //标记是否有equipList
            //if (equipList != null)
            //{
            //    CacheWriter.WriteInt32(equipList.Count);
            //    var e = equipList.GetEnumerator();
            //    while (e.MoveNext())
            //    {
            //        var kvp = e.Current;
            //        kvp.Value.Pack(CacheWriter);
            //    }
            //    e.Dispose();
            //}

            //CacheWriter.WriteInt32(attrInfoList == null ? 0 : 1);       //标记是否有attrInfoList
            //if (attrInfoList != null)
            //{
            //    CacheWriter.WriteInt32(attrInfoList.Count);
            //    var e = attrInfoList.GetEnumerator();
            //    while (e.MoveNext())
            //    {
            //        var kvp = e.Current;
            //        CacheWriter.WriteInt64(kvp.Key);
            //        kvp.Value.Pack(CacheWriter);
            //    }
            //    e.Dispose();
            //}

            //CacheWriter.WriteInt32(equipGroupList == null ? 0 : 1);       //标记是否有attrInfoList
            //if (equipGroupList != null)
            //{
            //    CacheWriter.WriteInt32(equipGroupList.Count);
            //    var e = equipGroupList.GetEnumerator();
            //    while (e.MoveNext())
            //    {
            //        var kvp = e.Current;
            //        kvp.Value.Pack(CacheWriter);
            //    }
            //    e.Dispose();
            //}

            CacheWriter.WriteInt32(equipGroupListCount);
            {
                CacheWriter.WriteInt32(groupLevels.Count);
                var e = groupLevels.GetEnumerator();
                while (e.MoveNext())
                {
                    var kvp = e.Current;
                    CacheWriter.WriteInt32(kvp.Key);
                    CacheWriter.WriteInt32(kvp.Value);
                }
                e.Dispose();
            }

            CacheWriter.WriteInt32(posInfo == null ? 0 : 1);       //标记是否有posInfo
            if (posInfo != null)
            {
                posInfo.Pack(CacheWriter);
            }

            CacheWriter.WriteInt32(stoneListCount);
            {
                CacheWriter.WriteInt32(stoneList.Count);
                var e = stoneList.GetEnumerator();
                while (e.MoveNext())
                {
                    var kvp = e.Current;
                    kvp.Value.Pack(CacheWriter);
                }
                e.Dispose();
            }

            //base64编码成字符串
            int len;
            byte[] cdata = PacketUtil.compress_lz4(CacheWriter.Data, 0, CacheWriter.Length, out len);
            string data = Convert.ToBase64String(cdata, 0, len);

            return data;
        }

        /// <summary>
        /// 将字符串数据反序列号成对象。
        /// </summary>
        /// <param name="data">字符串数据。</param>
        /// <returns>是否成功。</returns>
        public bool Deserialize(string data)
        {
            bool ok = true;
            try
            {
                byte[] bytes = Convert.FromBase64String(data);
                int len = PacketUtil.uncompress_lz4(bytes, 0, bytes.Length, CacheDst, CacheDst.Length);
                CacheReader.Init(CacheDst, 0, len);

                itemInfo = new ItemInfo();
                itemInfo.Uppack(CacheReader);
                attrInfo = CacheReader.ReadInt32() == 0 ? null : new EquipAttInfo();
                if (attrInfo != null)
                {
                    attrInfo.Uppack(CacheReader);
                }

                isWear = CacheReader.ReadInt32();
                groupId = CacheReader.ReadInt32();
                isActive = CacheReader.ReadInt32();
                activeGroupLevel = CacheReader.ReadInt32();
                activeGroupCount = CacheReader.ReadInt32();

                //CacheWriter.WriteInt32(equipPosList == null ? 0 : 1);       //标记是否有equipPosList
                //if (equipPosList != null)
                //{
                //    CacheWriter.WriteInt32(equipPosList.Count);
                //    for (int i = 0; i < equipPosList.Count; ++i)
                //    {
                //        CacheWriter.WriteInt64(equipPosList[i]);
                //    }
                //}

                //CacheWriter.WriteInt32(equipGroupPosList == null ? 0 : 1);       //标记是否有equipGroupPosList
                //if (equipGroupPosList != null)
                //{
                //    CacheWriter.WriteInt32(equipGroupPosList.Count);
                //    var e = equipGroupPosList.GetEnumerator();
                //    while (e.MoveNext())
                //    {
                //        var kvp = e.Current;
                //        CacheWriter.WriteInt64(kvp.Key);
                //        CacheWriter.WriteInt32(kvp.Value);
                //    }
                //    e.Dispose();
                //}

                equipPosList = CacheReader.ReadInt32() == 0 ? null : new List<long>();
                if (equipPosList != null)
                {
                    int n = CacheReader.ReadInt32();
                    for (int i = 0; i < n; ++i)
                    {
                        long k = CacheReader.ReadInt64();
                        equipPosList.Add(k);
                    }
                }

                equipGroupPosList = CacheReader.ReadInt32() == 0 ? null : new Dictionary<long, int>();
                if (equipGroupPosList != null)
                {
                    int n = CacheReader.ReadInt32();
                    for (int i = 0; i < n; ++i)
                    {
                        long k = CacheReader.ReadInt64();
                        int pos = CacheReader.ReadInt32();
                        equipGroupPosList.Add(k, pos);
                    }
                }

                //equipList = CacheReader.ReadInt32() == 0 ? null : new Dictionary<long, ItemInfo>();
                //if (equipList != null)
                //{
                //    int n = CacheReader.ReadInt32();
                //    for (int i=0; i<n; ++i)
                //    {
                //        ItemInfo info = new ItemInfo();
                //        info.Uppack(CacheReader);
                //        equipList.Add(info.UID, info);
                //    }
                //}

                //attrInfoList = CacheReader.ReadInt32() == 0 ? null : new Dictionary<long, EquipAttInfo>();
                //if (attrInfoList != null)
                //{
                //    int n = CacheReader.ReadInt32();
                //    for (int i = 0; i < n; ++i)
                //    {
                //        EquipAttInfo info = new EquipAttInfo();
                //        long key = CacheReader.ReadInt64();
                //        info.Uppack(CacheReader);
                //        attrInfoList.Add(key, info);
                //    }
                //}

                //equipGroupList = CacheReader.ReadInt32() == 0 ? null : new Dictionary<long, ItemInfo>();
                //if (equipGroupList != null)
                //{
                //    int n = CacheReader.ReadInt32();
                //    for (int i = 0; i < n; ++i)
                //    {
                //        ItemInfo info = new ItemInfo();
                //        info.Uppack(CacheReader);
                //        equipGroupList.Add(info.UID, info);
                //    }
                //}

                equipGroupListCount = CacheReader.ReadInt32();
                {
                    groupLevels = new Dictionary<int, int>();
                    int n = CacheReader.ReadInt32();
                    for (int i = 0; i < n; ++i)
                    {
                        int group = CacheReader.ReadInt32();
                        int level = CacheReader.ReadInt32();
                        groupLevels.Add(group, level);
                    }
                }

                posInfo = CacheReader.ReadInt32() == 0 ? null : new EquipPosInfo();
                if (posInfo != null)
                {
                    posInfo.Uppack(CacheReader);
                }

                stoneListCount = CacheReader.ReadInt32();
                {
                    stoneList = new Dictionary<long, ItemInfo>();
                    int n = CacheReader.ReadInt32();
                    for (int i = 0; i < n; ++i)
                    {
                        ItemInfo info = new ItemInfo();
                        info.Uppack(CacheReader);
                        stoneList.Add(info.UID, info);
                    }
                }
            }
            catch
            {
                ok = false;
            }
            return ok;
        }

        public static NetReadBuffer CacheReader = new NetReadBuffer();
        public static NetWriteBuffer CacheWriter = new NetWriteBuffer();
        public static byte[] CacheDst = new byte[1024 * 2];
    }

    public enum EquipTipType
    {
        simple = 1,  //只显示基础信息，物品掉落，奖励查询
        deatail = 2,  //显示详细,查看别人装备，非背包页查看自己装备
        bag = 3,   //背包查看，显示穿戴按钮
    }
}