/**
* @file     : a
* @brief    : b
* @details  : 这个类专门针对UI  
* @author   : a
* @date     : 2014-xx-xx
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{


[Hotfix]
	public class SkillCastMgr : IModule {
        ActorObj m_playerObj = null;
        Dictionary<int, int> m_ShareCDCfgCache = new Dictionary<int, int>();
		//----------- 每个管理器必须写的方法 ----------
		public override bool LoadSrv(IModuleServer IModuleSrv)
		{
			ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
			moduleSrv.GSkillCastMgr = this;
			
			return true;
		}
		
		public override void InitializeSrv()
		{
			m_EventMgr = CoreEntry.gEventMgr;
            m_EventMgr.AddListener(GameEvent.GE_SKILL_MAGICCOLLDOWN, OnMagicCollDown);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, OnCleanupUserData);

            //if (bPublicCDEnable) {
            //	m_EventMgr.AddListener(GameEvent.RESPOND_CAST_SKILL_SUCCESS, OnSkillPublicCoolDown);			
            //}     
        }
        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        public  void OnCleanupUserData(GameEvent ge, EventParameter parameter)
        {
            m_skillCDCoolDict.Clear();
            m_SkillShareCDMap.Clear();
            m_ShareCDCfgCache.Clear();
        }
    public static IModule Newer(GameObject go)
		{
			IModule module = go.AddComponent<SkillCastMgr>();
			return module;
		}
		//-------------------------------------------
	
		public bool InCDCoolState(int skillId)
		{
            if (CoreEntry.gActorMgr.MainPlayer == null)
                return true;
 
            return IsInCoolDownTime(skillId) || IsInShareCDTime(skillId); 
		}

        //是否CD时间中
        public bool IsInCoolDownTime(int skillID)
        {
            if (!m_skillCDCoolDict.ContainsKey(skillID))
            {
                return false;
            }

            SkillCDData cdData = m_skillCDCoolDict[skillID];

            float lastTime = cdData.fBeginTime;
            float curTime = Time.time;

            
            if ((curTime - lastTime) * 1000 <= cdData.iCDCoolTime)
            {
                return true;
            } 

            return false;
        }

		public bool GetSkillCDData(int entityID, int skillId, out SkillCDData cdData)
		{
			if (m_skillCDCoolDict.TryGetValue(skillId, out cdData))
			{
				return true;
			}

			return false;
		}


        //当前距CD时间结束还有多久
        public float QuerySkillCDOverTime(int skillID)
        {
            if (!m_skillCDCoolDict.ContainsKey(skillID))
            {
                return 0f;
            }

            SkillCDData cdData = m_skillCDCoolDict[skillID];
            float curTime = Time.time;
            float lastTime = cdData.fBeginTime;

            float remainTime = cdData.iCDCoolTime  - (curTime - lastTime) * 1000;
            if (remainTime < 0)
            {
                remainTime = 0f;
            }

            return remainTime;
        }

        public float GetSkillCDBegin(int skillId)
        {
            SkillCDData cdData;
            if (!m_skillCDCoolDict.TryGetValue(skillId, out cdData))
            {
                return 0f;
            }

            return cdData.fBeginTime;
        }

        public int GetSkillCDTime(int skillId)
        {
            SkillCDData cdData;
            if (!m_skillCDCoolDict.TryGetValue(skillId, out cdData))
            {
                return 0;
            }

            return cdData.iCDCoolTime;
        }


        //public void SetSkillCDData(int skillId, SkillCDData cdData)
        //{
        //    m_skillCDCoolDict[skillId] = cdData;
           
        //}



        //public void SetCDCountDown(int skillId, int cdCountDown)
        //{
        //    SkillCDData cdData;
        //    if (m_skillCDCoolDict.TryGetValue(skillId, out cdData))
        //    {
        //        cdData.iCDCountDown = cdCountDown;
        //        m_skillCDCoolDict[skillId] = cdData;
        //    }
        //}

        //public void SetCDCoolState(int skillId, bool cdCoolState)
        //{
        //    SkillCDData cdData;
        //    if (m_skillCDCoolDict.TryGetValue(skillId, out cdData))
        //    {
        //        cdData.bInCDCoolState = cdCoolState;
        //        m_skillCDCoolDict[skillId] = cdData;
        //    }
        //}
        int gameStartCount = 0; 
		public void CastSkill(int skillId)
		{
			if (InCDCoolState(skillId) == true)
			{
				return;
			}
            LogMgr.UnityWarning("CastSkill:" + skillId);
            EventParameter parameter = EventParameter.Get();
			parameter.intParameter = skillId;
            parameter.goParameter = CoreEntry.gActorMgr.MainPlayer.gameObject;
            m_EventMgr.TriggerEvent(GameEvent.GE_NOTIFY_CAST_SKILL, parameter);

            if (CoreEntry.GameStart == false)
            {
                if (gameStartCount > 2)
                {
                    CoreEntry.GameStart = true;
                    gameStartCount = 0;
                }
                else 
                { 
                    gameStartCount++; 
                } 
            }
		}

        public void ShowSkillScope(int skillId)
		{
			if (InCDCoolState(skillId) == true)
			{
				return;
			}

			EventParameter parameter = EventParameter.Get();
			parameter.intParameter = skillId;
            parameter.goParameter = CoreEntry.gActorMgr.MainPlayer.gameObject;

            m_EventMgr.TriggerEvent(GameEvent.GE_NOTIFY_SHOW_SKILL_SCOPE, parameter); 	     
		}

        public void HideSkillScope(int skillId)
		{
			if (InCDCoolState(skillId) == true)
			{
				return;
			}

			EventParameter parameter = EventParameter.Get();
			parameter.intParameter = skillId;
            parameter.goParameter = CoreEntry.gActorMgr.MainPlayer.gameObject;

            m_EventMgr.TriggerEvent(GameEvent.GE_NOTIFY_HIDE_SKILL_SCOPE, parameter); 	     
		}
         

        public void OnMagicCollDown(GameEvent ge, EventParameter parameter)
        {  
            MsgData_sCooldown data = parameter.msgParameter as MsgData_sCooldown;

            int skillId = data.SkillID;
            long nPlayerID = data.CasterID;

            //GameObject gameObj = parameter.goParameter;

            if (m_playerObj == null)
            {
                m_playerObj = CoreEntry.gActorMgr.MainPlayer;
            }

            SkillCDData cdData;
            if (m_skillCDCoolDict.TryGetValue(skillId, out cdData) == false)
            { 
                cdData.iCDCoolTime = data.CD;//(int)m_playerObj.GetSkillCDTime(skillId);
                cdData.iCDCountDown = 0;
                cdData.bCDEnable = false;
                cdData.bInCDCoolState = false;
                if (cdData.iCDCoolTime > 0)
                {
                    cdData.bInCDCoolState = true;
                    cdData.fBeginTime = Time.time;
                }

                m_skillCDCoolDict.Add(skillId, cdData);
            }
            else
            {
                cdData.iCDCoolTime = data.CD;//(int)m_playerObj.GetSkillCDTime(skillId);
                if (cdData.iCDCoolTime > 0)
                {
                    cdData.bInCDCoolState = true;
                    cdData.fBeginTime = Time.time;
                }
                m_skillCDCoolDict[skillId] = cdData;
            }
            if (cdData.bInCDCoolState == true && m_playerObj.ServerID == nPlayerID)
            {
                m_playerObj.SetCoolDownTime(skillId);
                SetShareCoolDownTime(skillId, data.GroupID, data.GroupCD);
                m_playerObj.SetShareCoolDownTime(skillId,data.GroupID,data.GroupCD);
                EventParameter param = EventParameter.Get();
                param.intParameter1 = skillId;
                param.intParameter2 = m_playerObj.EntityID;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EU_ONSKILLCOOLDOWN, param);


            }
            //普通攻击连击
            if (cdData.bInCDCoolState == true && m_playerObj.ServerID == nPlayerID)
            {
                EventParameter param = EventParameter.Get();
                param.intParameter = skillId;
                param.intParameter1 = m_playerObj.curCastSkillID;
                param.intParameter2 = m_playerObj.EntityID;

                param.goParameter = m_playerObj.gameObject;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_RESPOND_CAST_SKILL_SUCCESS, param); 
            }
        }
        public void OnSkillCDList(GameEvent ge, EventParameter param)
        {
            MsgData_sSkillCDList data = param.msgParameter as MsgData_sSkillCDList;
            for (int i = 0; i < data.items.Count; i++)
            {
                int skillId = data.items[i].skillID;
                int cdTime = data.items[i].cdTime;
                SkillCDData cdData;
                if (m_skillCDCoolDict.TryGetValue(skillId, out cdData) == false)
                {
                    cdData.iCDCoolTime = cdTime;
                    cdData.iCDCountDown = 0;
                    cdData.bCDEnable = false;
                    cdData.bInCDCoolState = false;
                    if (cdData.iCDCoolTime > 0)
                    {
                        cdData.bInCDCoolState = true;
                        cdData.fBeginTime = Time.time;
                    }

                    m_skillCDCoolDict.Add(skillId, cdData);
                }
                else
                {
                    cdData.iCDCoolTime = cdTime;
                    if (cdData.iCDCoolTime > 0)
                    {
                        cdData.bInCDCoolState = true;
                        cdData.fBeginTime = Time.time;
                    }
                    m_skillCDCoolDict[skillId] = cdData;
                }

//                 LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(skillId);
//                 if (skillCfg != null)
//                 {
//                     int groundid = skillCfg.Get<int>("group_id");
//                     int group_cd = skillCfg.Get<int>("group_cd");
//                     SetShareCoolDownTime(skillId, groundid, group_cd);
//                 }

            }
        }

        public void Disable()
        {
            CancelInvoke("UpdateCD");
        }

        public void Enable()
        {
            InvokeRepeating("UpdateCD", 0f, 1.0f);
        }


        public void UpdateCD()
        { 
            Dictionary<int, SkillCDData>.Enumerator iter = m_skillCDCoolDict.GetEnumerator();
            while (iter.MoveNext())
            {
                //int skillId = iter.Current.Key;
                SkillCDData cdData = iter.Current.Value;

                float fDurTime = Time.time - cdData.fBeginTime;
                if (fDurTime*1000 > (cdData.iCDCoolTime + 1000f))
                {
                    cdData.bCDEnable = false;
                    cdData.bInCDCoolState = false;  
                }
            }
            UpdateShareCDOverTime();   

        } 
        public void UpdateSkillCD()
        {
            m_playerObj = CoreEntry.gActorMgr.MainPlayer;
            if (m_playerObj == null)
                return;
            int nPlayerID = m_playerObj.EntityID;

            Dictionary<int, SkillCDData>.Enumerator iter = m_skillCDCoolDict.GetEnumerator();
            while (iter.MoveNext())
            {
                int skillId = iter.Current.Key;
                SkillCDData cdData = iter.Current.Value;
                if (cdData.bInCDCoolState == true)
                {
                    EventParameter param = EventParameter.Get();
                    param.intParameter1 = skillId;
                    param.intParameter2 = nPlayerID;
                    //m_playerObj.SetShareCoolDownTime(skillId);
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EU_ONSKILLCOOLDOWN, param);
                }
            }
        }
        //公共技能CD组
        public bool IsInShareCDTime(int nSkillID)
        {
            //m_SkillShareCDMap

            int Group_cd_id = GetGroup_cd_idBySkillID(nSkillID);
            
            if (Group_cd_id == 0)
            {
                return false;
            }


            if (!m_SkillShareCDMap.ContainsKey(Group_cd_id))
            {
                return false;
            }


            ShareCDData data = m_SkillShareCDMap[Group_cd_id];
            float lastTime = data.fSkillTime;
            float curTime = Time.time;


            if ((curTime - lastTime) * 1000 <= data.cdTime)
            {
                return true;
            }

            return false;

        }
        public float QueryShareCDOverTime(int skillID)
        {
            int group_cd_id = GetGroup_cd_idBySkillID(skillID);


            if (!m_SkillShareCDMap.ContainsKey(group_cd_id))
            {
                return 0f;
            }
            ShareCDData data = m_SkillShareCDMap[group_cd_id];
            float lastTime = data.fSkillTime;
            float curTime = Time.time;


            float remainTime = data.cdTime - (curTime - lastTime) * 1000;
            if (remainTime < 0)
            {
                remainTime = 0f;
            }

            return remainTime;
        }
        //更新CD组，如果CD组的剩余时间为0，则删除当前CD组
        public void UpdateShareCDOverTime()
        {
            if (m_SkillShareCDMap.Count == 0)
            {
                return;
            }

            Dictionary<int, ShareCDData>.Enumerator iter = m_SkillShareCDMap.GetEnumerator();
            List<int> delList = null;
            while (iter.MoveNext())
            {
                ShareCDData data = iter.Current.Value;
                float lastTime = data.fSkillTime;
                int cdTime = data.cdTime;
                float curTime = Time.time;

                float remainTime = cdTime - (curTime - lastTime) * 1000;
                if (remainTime <= 0)
                {
                    if (delList == null)
                        delList = new List<int>();
                    delList.Add(iter.Current.Key);
                }
            }

            if (delList != null)
            {
                for (int i = 0; i < delList.Count; i++)
                {
                    m_SkillShareCDMap.Remove(delList[i]);
                }
            }
        }
        public void SetShareCoolDownTime(int skillID, int GroupId, int GroupCD)
        {
            float curTime = Time.time;

            int group_cd_id = GetGroup_cd_idBySkillID(skillID);
            //查询技能组
            //LuaTable skillDesc = GetCurSkillDesc(skillID);

            ShareCDData data = new ShareCDData();
            data.fSkillTime = curTime;
            data.nSkillID = skillID;
            data.cdTime = GroupCD;

            if (group_cd_id != GroupId)
            {
                Debug.Log(string.Format("服务器技能cd组与客户端cd组不一致skillid:{0} 服务器：groupId:{1}  客户端groupid:{2}", skillID, GroupId, group_cd_id));
            }

            if (m_SkillShareCDMap.ContainsKey(group_cd_id))
            {
                m_SkillShareCDMap[group_cd_id] = data;
                return;
            }

            m_SkillShareCDMap.Add(group_cd_id, data);
        }
        //获取当前技能信息    
        public LuaTable GetCurSkillDesc(int skillID)
        { 
            if (skillID <= 0)
            {
                return null;
            }
            return ConfigManager.Instance.Skill.GetSkillConfig(skillID);
 
        }
 
        public float GetShareSkillCDBegin(int skillId)
        {
            float value = 0;

            int group_cd_id = GetGroup_cd_idBySkillID(skillId);
              
            ShareCDData data;
            if (!m_SkillShareCDMap.TryGetValue(group_cd_id, out data))
            {
                return 0f;
            }
            value = data.fSkillTime;

            return value;
        }

        public int GetGroup_cd_idBySkillID(int skillId)
        {
            int group_cd_id = 0;
            if (!m_ShareCDCfgCache.TryGetValue(skillId, out group_cd_id))
            {
                LuaTable skillDesc = GetCurSkillDesc(skillId);
                if (skillDesc == null)
                    return 0;
                group_cd_id = skillDesc.Get<int>("group_cd_id");
                m_ShareCDCfgCache[skillId] = group_cd_id;
            }

            return group_cd_id;

        }
        public int GetShareSkillCDTime(int skillID)
        {
            int group_cd_id = GetGroup_cd_idBySkillID(skillID);
            ShareCDData data;

            if (!m_SkillShareCDMap.TryGetValue(group_cd_id, out data))
            {
                return 0;
            }
            return data.cdTime;

        }

        public int GetShareSkillCDTime(LuaTable skillDesc)
        {
            int value = 0;
            ShareCDData data;
            if (!m_SkillShareCDMap.TryGetValue(skillDesc.Get<int>("group_cd_id"), out data))
            {
                return value;
            }
            value = data.cdTime;
            return value;
        }
        //清理所有技能CD   此函数废弃  , 改用 ActorObj . ClearAllSkillCD
        public void ClearSkillCD()
        {
            //for (int i = 0; i < CoreEntry.gTeamMgr.playerList.Count; i++)
            //{
            //    ActorObj playerObj = CoreEntry.gTeamMgr.playerList[i].GetComponent<ActorObj>();

            //    int skillId = 0;
            //    int nPlayerID = playerObj.EntityID;
            //    for (int j = 0; j < playerObj.mBaseAttr.BigSkillIdArray.Length; j++)
            //    {
            //        skillId = playerObj.mBaseAttr.BigSkillIdArray[j];
            //        SkillCDData cdData;

            //        if (m_skillCDCoolDict.TryGetValue(skillId, out cdData) == true)
            //        {
            //            if (cdData.bInCDCoolState == true)
            //            {
            //                cdData.bInCDCoolState = false;

            //                EventToUI.SetArg(UIEventArg.Arg1, skillId);
            //                EventToUI.SetArg(UIEventArg.Arg2, nPlayerID);
            //                EventToUI.SendEvent("EU_ONCANCELSKILLCOOL");

            //            }
            //        }
            //    }
            //}

            //m_skillCDCoolDict.Clear();
        }




		public bool SetBtnSkillID(int nEntityID,int btnIndex, int skillID)
		{
			if (btnIndex > 0)// && btnIndex <= 6)
			{  
                EventParameter param = EventParameter.Get();
                param.intParameter = btnIndex;
                param.intParameter1 = skillID;
                param.intParameter2 = nEntityID;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EU_ONSETSKILLID, param); 
				return true;
			}

			return false;
		}

		public void PressButton(int buttonIndex)
		{
            //重置                        
            SetPressButtonHandled(false); 

            EventParameter param = EventParameter.Get();
            param.intParameter = buttonIndex;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EU_ONPRESS_SKILLBUTTON, param); 

		}

        public void PressingButton(int buttonIndex)
        {
            //重置                        
            //SetPressButtonHandled(false);
            EventParameter param = EventParameter.Get();
            param.intParameter = buttonIndex;
            param.objParameter = true;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EU_ONPROCESSING_SKILLBUTTON, param); 

        }

        public void PressedButton(int buttonIndex)
        {
            //重置                        
            //SetPressButtonHandled(false); 
            EventParameter param = EventParameter.Get();
            param.intParameter = buttonIndex;
            param.objParameter = false;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EU_ONPROCESSING_SKILLBUTTON, param);
        }

        /*
         * 解决SkillCast Arg1参数值被修改bug
         */
        bool m_PressButtonHandle = false;
        public void SetPressButtonHandled(bool handled)
        {
            m_PressButtonHandle = handled;
        }
        public bool IsPressButtonHandled()
        {
            return m_PressButtonHandle;
        }

		public struct SkillCDData{
			public int iCDCoolTime;			// CD冷却时间
			public int iCDCountDown;		// CD倒计时
			public bool bCDEnable;			// 是否启用CD
			public bool bInCDCoolState;		// 当前是否在CD中
            public float fBeginTime;
		}
        struct ShareCDData
        {
            public int nSkillID;
            public float fSkillTime;
            public int cdTime; //cd时长
        };

		private Dictionary<int, SkillCDData> m_skillCDCoolDict = new Dictionary<int, SkillCDData>();
		private SkillCDData m_publicCDCool;
        private Dictionary<int, ShareCDData> m_SkillShareCDMap = new Dictionary<int, ShareCDData>();


		//事件管理器
		private EventMgr m_EventMgr;
	}

};//End SG

