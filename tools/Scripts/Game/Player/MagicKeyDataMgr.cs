/**
* @file     : MagicKeyDataMgr.cs
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


namespace SG
{

    [CSharpCallLua]
    public delegate string GetMagicKeyDesc(int id, int star);

    // S->C 法宝信息
    [LuaCallCSharp]
    [Hotfix]
    public class cMagickeyinfo
    {
        public long guid; // 物品实例id
        public int magickeyID; // 法宝配置ID
        public int wuxing; // 悟性值
        public int level; // 等级
        public double totalExp; // 总经验值
        public int starCount; // 法宝星级
        public int starProgress; // 法宝星级进度
        public int passiveskill1; // 被动技能1
        public int passiveskill2; // 被动技能2
        public int passiveskill3; // 被动技能3
        public int passiveskill4; // 被动技能4
        public int passiveskill5; // 被动技能5
        public int passiveskill6; // 被动技能6
        public int passiveskill7; // 被动技能7
        public int passiveskill8; // 被动技能8
        public int passiveskill9; // 被动技能9
        public int passiveskill10; // 被动技能10
        public int passiveskill11; // 被动技能11
        public int passiveskill12; // 被动技能12
        public int awakeCount; // 觉醒次数 0、未觉醒 1、觉醒一次 2、觉醒二次
        public int feisheng; // 飞升等级
        public int feishengext; // 当前飞升经验
        public int activeSkillID;//主动技能id
        public int FightPower; // 战斗力
        public int[] AttrType = new int[3]; // 属性类型
        public int[] AttrValue = new int[3]; // 属性值
    }

    //仙灵列表
    [LuaCallCSharp]
    [Hotfix]
    public class cGodInfosList
	{
		public long guid; // 物品实例id
		public int wuxing; // 悟性值
		public int id; // 配置ID
		public long magicGuid; // 法宝实例ID
    }

    //法福祝神福值,合成进度球
    [LuaCallCSharp]
    [Hotfix]
    public class WashMaker
    {
        public int MagicKeyMakeOrder; // 打造的等阶ID
        public int MagicKeyWashValue; // 现有的打造祝福值
        public int MagicKeyMaxWashValue; // 最大的打造祝福值;
    }


    [LuaCallCSharp]
    [Hotfix]
    public class MagicKeyDataMgr
    {


        //private static MagicKeyDataMgr _instance = null;
        public static MagicKeyDataMgr Instance
        {
            get
            {
               return PlayerData.Instance.MagicKeyDataMgr;
            }
        }

        Dictionary<long, cMagickeyinfo> mMagicKeyInfo = new Dictionary<long, cMagickeyinfo>();
        public Dictionary<long, cMagickeyinfo> MagicKeyInfo
        {
            get { return mMagicKeyInfo; }
            set { mMagicKeyInfo = value; }
        }

        Dictionary<long, cGodInfosList> mGodInfosList = new Dictionary<long, cGodInfosList>();
        public Dictionary<long, cGodInfosList> GodInfosList
        {
            get { return mGodInfosList; }
            set { mGodInfosList = value; }
        } 

        private List<WashMaker> mWashMaker= new List<WashMaker>();
        public List<WashMaker> WashMaker
        {
            get { return mWashMaker; }
            set { mWashMaker = value; }
        }
        public int Power = 0;

        private Dictionary<int, LuaTable> m_CacheMagicKeyConfig = null;
#region msg
        public void ResisterMsg()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_UPDATEMAGICKEYINFO, OnUpdateMagicKeyInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_RETURNMAGICKEYGODINFOS, OnReturnMagicKeyGodInfos);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MAGICKEYINSETSKILL, OnMagicKeyInsetSkill);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_MagicKeyWashInfo, OnMagicKeyWashInfo);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AutoUseMagicKeySkill, OnAutoMseMagicKeySkill);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_UseMagicKeySkill, OnUseMagicKeySkill);

            m_CacheMagicKeyConfig = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_magicKeyDataNew"); 
         }
    //更新法宝信息 
    public void OnUpdateMagicKeyInfo(GameEvent ge, EventParameter param)
        {
            MsgData_sUpdateMagicKeyInfo data = param.msgParameter as MsgData_sUpdateMagicKeyInfo;
            for (int i = 0; i < data.items.Count; i++)
            {
                UpdateMagickeyInfo(data.items[i]);
            }
        }
        //返回法宝仙灵列表
        public void OnReturnMagicKeyGodInfos(GameEvent ge, EventParameter param)
        {
            MsgData_sReturnMagicKeyGodInfos data = param.msgParameter as MsgData_sReturnMagicKeyGodInfos;
            for (int i = 0; i < data.items.Count; i++)
            {
                var v = data.items[i];
                AddGodInfoList(v);
            }
        } 
        public void OnMagicKeyInsetSkill(GameEvent ge, EventParameter param)
        {
            //MsgData_sMagicKeyInsetSkill data = param.msgParameter as MsgData_sMagicKeyInsetSkill;

        }
        public void OnMagicKeyWashInfo(GameEvent ge, EventParameter param)
        {
            MsgData_sMagicKeyWashInfo data = param.msgParameter as MsgData_sMagicKeyWashInfo;
            for (int i = 0; i < data.items.Count; i++)
            {
                UpdateMagicKeyWashInfo(data.items[i]);
            }
        }


        public void OnAutoMseMagicKeySkill(GameEvent ge,EventParameter param)
        { 
            int skillID = GetUseMagicKeySkillID();
            if (skillID > 0)
            {
                ModuleServer.MS.GSkillCastMgr.CastSkill(skillID);
            }
        }
        public void OnUseMagicKeySkill(GameEvent ge, EventParameter param)
        { 
            SkillMagicKeyBase.CreateMagicKey((ActorObj)param.objParameter, param.intParameter);
        }

         
        public int GetUseMagicKeySkillID()
        {
            int skillID = 0;
            int nPriority = -1; 
            Dictionary<long, cMagickeyinfo>.Enumerator iter = mMagicKeyInfo.GetEnumerator();
            while (iter.MoveNext())
            {
                ItemInfo item = PlayerData.Instance.BagData.GetItemInfoByUID(iter.Current.Value.guid);
                if (item != null)
                {
                    if (item.Bag != BagType.TIEM_BAG_TYPE_MAGICKEY)
                        continue;
                }
                else
                {
                    continue;
                }
                int tmpSkillID = iter.Current.Value.activeSkillID;
                 
                if (!ModuleServer.MS.GSkillCastMgr.InCDCoolState(tmpSkillID))
                { 
                    LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(tmpSkillID);
                    if (skillDesc != null)
                    {
                        if (skillDesc.Get<int>("priority") > nPriority)
                        {
                            nPriority = skillDesc.Get<int>("priority");
                            skillID = tmpSkillID;
                        }
                    } 
                }
            } 
            return skillID;
        }

#endregion

        public void UpdateMagickeyInfo(MsgData_sMagickeyinfo item)
        {
            cMagickeyinfo v = new cMagickeyinfo();
            v.guid = item.guid; // 物品实例id
            v.magickeyID = item.magickeyID; // 法宝配置ID
            v.wuxing = item.wuxing; // 悟性值
            v.level = item.level; // 等级
            v.totalExp = item.totalExp; // 总经验值
            v.starCount = item.starCount; // 法宝星级
            v.starProgress = item.starProgress; // 法宝星级进度
            v.passiveskill1 = item.passiveskill1; // 被动技能1
            v.passiveskill2 = item.passiveskill2; // 被动技能2
            v.passiveskill3 = item.passiveskill3; // 被动技能3
            v.passiveskill4 = item.passiveskill4; // 被动技能4
            v.passiveskill5 = item.passiveskill5; // 被动技能5
            v.passiveskill6 = item.passiveskill6; // 被动技能6
            v.passiveskill7 = item.passiveskill7; // 被动技能7
            v.passiveskill8 = item.passiveskill8; // 被动技能8
            v.passiveskill9 = item.passiveskill8; // 被动技能9
            v.passiveskill10 = item.passiveskill10; // 被动技能10
            v.passiveskill11 = item.passiveskill11; // 被动技能11
            v.passiveskill12 = item.passiveskill12; // 被动技能12
            v.awakeCount = item.awakeCount; // 觉醒次数 0、未觉醒 1、觉醒一次 2、觉醒二次
            v.feisheng = item.feisheng; // 飞升等级
            v.feishengext = item.feishengext; // 当前飞升经验
            v.activeSkillID = GetMagickeySkillID(v.magickeyID);
            v.FightPower = item.FightPower;
            Array.Copy(item.AttrType, v.AttrType, item.AttrType.Length);
            Array.Copy(item.AttrValue, v.AttrValue, item.AttrType.Length); 
            if (mMagicKeyInfo.ContainsKey(v.guid))
            {
                mMagicKeyInfo[v.guid] = v;
            }
            else
            {
                mMagicKeyInfo.Add(v.guid, v);
            } 
        }

        public cMagickeyinfo GetMagicKeyInfo(long guid)
        {
            cMagickeyinfo v = null;
            if (mMagicKeyInfo.ContainsKey(guid))
            {
                v = mMagicKeyInfo[guid];
            }
            return v;
        }


        public void AddGodInfoList(MsgData_sGodInfosList item)
        {
            cGodInfosList v = new cGodInfosList();
            v.guid = item.guid; // 物品实例id
		    v.wuxing = item.wuxing; // 悟性值
		    v.id = item.id; // 配置ID
		    v.magicGuid = item.magicGuid; // 法宝实例ID
            if (mGodInfosList.ContainsKey(v.guid))
            {
                mGodInfosList[v.guid] = v;
            }
            else
            {
                mGodInfosList.Add(v.guid, v);
            } 
        }

        public cGodInfosList GetGodInfosList(long guid)
        {
            cGodInfosList v = null;
            if (mGodInfosList.ContainsKey(guid))
            {
                v = mGodInfosList[guid];
            }
            return v;
        }

        //获取主法宝id
        public int GetMasterMagicKey()
        {
            int magickeyid = 0;
            BagInfo baginfo = PlayerData.Instance.BagData.GetBagInfo(BagType.TIEM_BAG_TYPE_MAGICKEY);
            if(baginfo!=null)
            {
                var mkinfo = baginfo.GetItemInfo(0);
                if (mkinfo != null)
                {
                    var magickKeyInfo = PlayerData.Instance.MagicKeyDataMgr.GetMagicKeyInfo(mkinfo.UID);
                    if (magickKeyInfo != null)
                    {
                        magickeyid = magickKeyInfo.magickeyID;
                    }
                }
            }

            return magickeyid;
        }
        //获取主法宝星级
        public int GetMasterMagicKeyStar()
        {
            int star = 0;
            BagInfo baginfo = PlayerData.Instance.BagData.GetBagInfo(BagType.TIEM_BAG_TYPE_MAGICKEY);
            if (baginfo != null)
            {
                var mkinfo = baginfo.GetItemInfo(0);
                if (mkinfo != null)
                {
                    var magickKeyInfo = PlayerData.Instance.MagicKeyDataMgr.GetMagicKeyInfo(mkinfo.UID);
                    if (magickKeyInfo != null)
                    {
                        star = magickKeyInfo.starCount;
                    }
                }
            }

            return star;
        }
        public  int GetMagickeyModel(int magickeyid)
        {
            int modelid = 0;
            LuaTable tbl = null;
            if(m_CacheMagicKeyConfig.TryGetValue(magickeyid,out tbl))
            {
                modelid = tbl.Get<int>("model");
            }

            return modelid;
        }

        public int GetMagickeyModelByStar(int magickeyid,int star)
        {
            int modelid = 0;
            LuaTable tbl = null;
            if (m_CacheMagicKeyConfig.TryGetValue(magickeyid, out tbl))
            {
                int step = (star - 1) / 10 + 1;
                modelid = tbl.Get<int>("model"+step);
            }

            return modelid;
        }
        public double GetMagickeyModelScale(int magickeyid)
        {
            double scale = 1.0; 

            LuaTable tbl = null;
            if (m_CacheMagicKeyConfig.TryGetValue(magickeyid, out tbl))
            {
                scale = tbl.Get<double>("scale");
            }
              
            return scale;
        }

        public int GetMagickeySkillID(int magickeyid)
        {
            int kSkillID = 0;

            LuaTable tbl = null;
            if (m_CacheMagicKeyConfig.TryGetValue(magickeyid, out tbl))
            {
                kSkillID = tbl.Get<int>("baseSkillID");
            }
            return kSkillID;
        }

        public string GetMagickeyName(int magickeyid)
        {
            string name = "";

            LuaTable tbl = null;
            if (m_CacheMagicKeyConfig.TryGetValue(magickeyid, out tbl))
            {
                name = tbl.Get<string>("nameIcon");
            }
            return name;
        }
        public string GetMagickeyName(int magickeyid,int star)
        {

            GetMagicKeyDesc fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<GetMagicKeyDesc>("ConfigData.MagicKeyConfig.GetMagicKeyDesc");

            string name = "";

            if(fun != null)
            {
                name = fun(magickeyid, star);
            }

            return name;
        }


        public cGodInfosList GetGodInfoListById(long id)
        {
            cGodInfosList v = null;
            Dictionary<long, cGodInfosList>.Enumerator iter = mGodInfosList.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.id == id)
                {
                    v = iter.Current.Value;
                    return v;
                }
            }
            return v;
        } 

        public int GetGodInfoListNumById(int id)
        {
            int num = 0;
            //cGodInfosList v = null;
            Dictionary<long, cGodInfosList>.Enumerator iter = mGodInfosList.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.id == id)
                {
                    num++;
                }
            }
            return num;
        }

        public cGodInfosList GetGodInfoListByMagicKey(long magicKeyGuid)
        {
            cGodInfosList v = null;
            Dictionary<long, cGodInfosList>.Enumerator iter = mGodInfosList.GetEnumerator();
            while(iter.MoveNext())
            {
                if (iter.Current.Value.magicGuid == magicKeyGuid)
                {
                     v = iter.Current.Value;
                     return v;
                }
            }
            return v;
        }


        //合成进度球
        public void UpdateMagicKeyWashInfo(MsgData_sWashMakeresult v)
        {
            WashMaker currWash = mWashMaker.Find(s => s.MagicKeyMakeOrder == v.MagicKeyMakeOrder);
            if (currWash == null)
            {
                WashMaker wsh = new WashMaker();
                wsh.MagicKeyMakeOrder = v.MagicKeyMakeOrder;
                wsh.MagicKeyMaxWashValue = v.MagicKeyMaxWashValue;
                wsh.MagicKeyWashValue = v.MagicKeyWashValue;
                mWashMaker.Add(wsh);
            }
            else
            {
                currWash.MagicKeyMaxWashValue = v.MagicKeyMaxWashValue;
                currWash.MagicKeyWashValue = v.MagicKeyWashValue;
            }
        }
        public WashMaker GetWashMaker()
        {
            if (mWashMaker.Count > 0)
                return mWashMaker[0];
            return null;
        }

    }
}