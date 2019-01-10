/**
* @file     : ArenaMgr
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-08-31
*/
using UnityEngine;
using System;
using System.Collections.Generic;

using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class ArenaRecord
    {
        public int ID;
        public long Time;
        public string Name;
    }

    [LuaCallCSharp]
    [Hotfix]
    public class ArenaMgr
    {
        private static ArenaMgr instance = null;
        public static ArenaMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new ArenaMgr();

                return instance;
            }
        }

        /// <summary>
        /// ս��˫������
        /// </summary>
        private MsgData_sResArenaMemVo[] mArenaFighters = null;
        /// <summary>
        /// ս��˫������
        /// </summary>
        public MsgData_sResArenaMemVo[] ArenaFighters
        {
            get { return mArenaFighters; }
        }
        public MsgData_sResEnterArena ArenaEnemyIds = null;

        /// <summary>
        /// �����ҵ�����
        /// </summary>
        private MsgData_sResMeArenaInfo mMeArenaInfo = null;
        /// <summary>
        /// �����ҵ�����
        /// </summary>
        public MsgData_sResMeArenaInfo MeArenaInfo
        {
            get { return mMeArenaInfo; }
        }

        private DateTime mMeInfoTime;
        public DateTime MeInfoTime
        {
            get { return mMeInfoTime; }
        }

        /// <summary>
        /// ��ս�б�
        /// </summary>
        private List<MsgData_sResArenaRoleVo> mChallengeList = null;
        /// <summary>
        /// ��ս�б�
        /// </summary>
        public List<MsgData_sResArenaRoleVo> ChallengeList
        {
            get { return mChallengeList; }
        }

        /// <summary>
        /// ��ս����
        /// </summary>
        private MsgData_sResArenaChallenge mChallengeResult = null;
        /// <summary>
        /// ��ս����
        /// </summary>
        public MsgData_sResArenaChallenge ChallengeResult
        {
            get { return mChallengeResult; }
        }

        /// <summary>
        /// ս����¼
        /// </summary>
        private List<ArenaRecord> mRecords = null;
        /// <summary>
        /// ս����¼
        /// </summary>
        public List<ArenaRecord> Records
        {
            get { return mRecords; }
        }

        private bool isArenaFight = false;
        public bool IsArenaFight
        {
            get { return isArenaFight; }
            set { isArenaFight = value; }
        }

        public bool IsArenaScene
        {
            get
            {
                int mapid = MapMgr.Instance.EnterMapId;
                var info = ConfigManager.Instance.Map.GetMapConfig(mapid);
                if (info != null && info.GetInPath<int>("type") == 42)
                {
                    return true;
                }
                return false;
            }
        }

        private bool isArena = false;
        public bool IsArena
        {
            get { return isArena; }
            set { isArena = value; }
        }


        private int mFightResult = -1;
        public int FightResult
        {
            get { return mFightResult; }
        }

        private int mLocalIndex = 0;
        private Vector2[] initPos;
        private int[] fighterHP;
        public int[] CurHP
        {
            get { return fighterHP; }
        }

        private bool isFightOver;

        private Dictionary<long, Dictionary<int, float>> mSkillTimeDict = null;

        private bool isRankClick = false;
        public bool IsRankClick
        {
            set { isRankClick = value; }
            get { return isRankClick; }
        }
        private bool isRankChange = false;
        public bool IsRankChange
        {
            get { return isRankChange; }
        }

        private ArenaMgr()
        {
            mArenaFighters = new MsgData_sResArenaMemVo[2];
            mChallengeList = new List<MsgData_sResArenaRoleVo>();
            mRecords = new List<ArenaRecord>();
            initPos = new Vector2[2];
            fighterHP = new int[2];
            mSkillTimeDict = new Dictionary<long, Dictionary<int, float>>();
            isFightOver = true;
            isRankChange = false;
            isRankClick = false;
        }

        public void ResigtNetMsg()
        {
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENAENTER, OnEnterArena);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENAINFO, OnArenaInfo);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENALIST, OnArenaList);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENACHALLENGE, OnChallengeArena);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENAREWARD, OnArenaReward);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENARECORD, OnArenaRecord);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENABUYTIMES, OnBuyTimes);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENABUYCD, OnBuyCD);
            CoreEntry.netMgr.bindMsgHandler(NetMsgDef.S_ARENARANKCHANGE, OnRankChange);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_AFTER_LOADSCENE, OnAfterLoadScene);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FRIST_UI_LOADED, OnFightUILoaded);
            //CoreEntry.gEventMgr.AddListener(GameEvent.GE_OBJ_ENTER_SCENE, OnEnterArenaData);
            mFightResult = -1;
        }

        

        #region ������Ϣ
        private static MsgData_sSceneObjectEnterHead CacheEnterHead = new MsgData_sSceneObjectEnterHead();
        private void OnEnterArenaData(GameEvent ge, EventParameter parameter)
        {
            NetReadBuffer buffer = parameter.objParameter as NetReadBuffer;
            CacheEnterHead.unpack(buffer);
            buffer.pos = 0;     //重新给具体类型的消息读取

            if (CacheEnterHead.ObjType == (sbyte)EnEntType.EntType_VirtualPlayer)//EntType_VirtualPlayer
            {
                MsgData_sResArenaMemVo sstaticStruct = new MsgData_sResArenaMemVo();
                sstaticStruct.unpack(buffer);

                Debug.LogError(sstaticStruct.RoleID+ "   " + sstaticStruct.Name);
                //CacheSceneObj(EnEntType.EntType_VirtualPlayer, sstaticStruct.Guid, sstaticStruct);
            }
        }

        public ActorObj GetVirtualPlayer()
        {
            if (ArenaMgr.Instance.ArenaEnemyIds == null)
            {
                return null;
            }
             
            return CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaMgr.Instance.ArenaEnemyIds.guidVirtualPlayer); 
        }
        private void OnEnterArena(MsgData msg)
        {
            //Debug.LogError("=============OnEnterArenaOnEnterArenaOnEnterArena===================");
            MsgData_sResEnterArena data = msg as MsgData_sResEnterArena;
            ArenaEnemyIds = data;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ARENA_HP_REFRESH, null);

            //MainPanelMgr.Instance.ShowDialog("UIArenaFight");
            //MainPanelMgr.gotoPanel("UIArenaFight");
            //mFightResult = data.Result;
            //mArenaFighters[0] = data.List[0];
            //mArenaFighters[1] = data.List[1];
            //fighterHP[0] = (int)data.List[0].HP;
            //fighterHP[1] = (int)data.List[1].HP;
            //if (data.List[0].RoleID == MainRole.Instance.serverID)
            //{
            //    mLocalIndex = 0;
            //}
            //else
            //{
            //    mLocalIndex = 1;
            //}
            //mSkillTimeDict.Clear();
            //isFightOver = true;
            //Debug.Log("==============OnEnterArenaOnEnterArenaOnEnterArena=======================");
            //CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_ENTER, EventParameter.Get(data.Result));

            //string posParam = ConfigManager.Instance.Consts.GetValue<string>(399, "param");
            //if (!string.IsNullOrEmpty(posParam))
            //{
            //    string[] param = posParam.Split('#');
            //    Debug.LogError(posParam);
            //    if (param.Length != 2)
            //    {
            //        LogMgr.UnityError("const error:jjcmap " + posParam);

            //        return;
            //    }

            //    float posX, posY;
            //    string[] param2 = param[0].Split(',');
            //    float.TryParse(param2[0], out posX);
            //    float.TryParse(param2[1], out posY);
            //    Vector2 pos = new Vector2(posX, posY);
            //    MapMgr.Instance.PlayerInitPos = pos;
            //    initPos[0] = pos;

            //    param2 = param[1].Split(',');
            //    float.TryParse(param2[0], out posX);
            //    float.TryParse(param2[1], out posY);
            //    pos = new Vector2(posX, posY);
            //    initPos[1] = pos;

            //    MainPanelMgr.gotoPanel("UIArenaFight");

            //    //MapMgr.Instance.DoEnterScene(constCfg.val1);
            //}
            isArenaFight = false;
            isArena = true;
        }

        private void OnArenaInfo(MsgData msg)
        {
            MsgData_sResMeArenaInfo data = msg as MsgData_sResMeArenaInfo;
            mMeArenaInfo = data;
            mMeInfoTime = DateTime.Now;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_INFO, null);
        }

        private void OnArenaList(MsgData msg)
        {
            MsgData_sResArenaList data = msg as MsgData_sResArenaList;
            mChallengeList.Clear();
            for (int i = 0; i < data.List.Count; i++)
            {
                mChallengeList.Add(data.List[i]);
            }

            mChallengeList.Sort((a, b) =>
            {
                if (a.Rank < b.Rank)
                    return -1;
                return 1;
            });

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_LIST, EventParameter.Get(data.Type));
        }

        private void OnChallengeArena(MsgData msg)
        {
            //Debug.LogError("===============OnChallengeArenaOnChallengeArena================");
            MsgData_sResArenaChallenge data = msg as MsgData_sResArenaChallenge;
            mChallengeResult = data;
            mFightResult = data.Result;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_CHALLENGE_RESULT, EventParameter.Get(data));
            if (mFightResult == 0)
            {
                MainPanelMgr.Instance.ShowPanel("UIArenaSuccess");
            }
            else
            {
                MainPanelMgr.Instance.ShowPanel("UIArenaFail");
            }
            ArenaEnemyIds = null;
            //MainPanelMgr.gotoPanel("UIArenaFight");
        }

        private void OnArenaReward(MsgData msg)
        {
            MsgData_sResArenaReward data = msg as MsgData_sResArenaReward;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_REWARD_RESULT, EventParameter.Get(data.Result));
        }

        private void OnArenaRecord(MsgData msg)
        {
            MsgData_sResArenaRecord data = msg as MsgData_sResArenaRecord;
            mRecords.Clear();
            for (int i = 0; i < data.List.Count; i++)
            {
                ArenaRecord r = new ArenaRecord();
                r.ID = data.List[i].ID;
                r.Time = data.List[i].Time;
                r.Name = data.List[i].Param.BytesToString();
                r.Name = r.Name.Replace("\0", "");
                mRecords.Add(r);
            }

            mRecords.Sort((a, b) =>
            {
                if (a.Time > b.Time)
                    return -1;

                return 1;
            });

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_RECORD, null);
        }

        private void OnBuyTimes(MsgData msg)
        {
            MsgData_sResBuyArenaTimes data = msg as MsgData_sResBuyArenaTimes;
            if (data.Result == 0)
            {
                mMeArenaInfo.MaxChallengeTimes = data.Times;
            }

            EventParameter parameter = EventParameter.Get();
            parameter.intParameter = data.Result;
            parameter.intParameter1 = data.Times;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_BUY_TIMES, parameter);
        }

        private void OnBuyCD(MsgData msg)
        {
            MsgData_sResBuyArenaCD data = msg as MsgData_sResBuyArenaCD;
            if (data.Result == 0)
            {
                mMeArenaInfo.CD = data.CD;
            }

            EventParameter parameter = EventParameter.Get();
            parameter.intParameter = data.Result;
            parameter.intParameter1 = data.CD;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_BUY_CD, parameter);
        }

        private void OnRankChange(MsgData msg)
        {
            MsgData_sResArenaRankChange data = msg as MsgData_sResArenaRankChange;
            if (data.Flag == 0)
            {
                isRankClick = false;
                isRankChange = true;

                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ARENA_RANK_CHANGE, null);
            }
            else
            {
                isRankChange = false;
            }
        }
        #endregion

        #region �����ӿ�
        public void SendReqExitArena()
        {
            MsgData_cReqExitArena data = new MsgData_cReqExitArena();

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENAEXIT, data);

            isArenaFight = false;
            isArena = false;
        }

        public void SendReqArenaInfo()
        {
            MsgData_cReqMeArenaInfo data = new MsgData_cReqMeArenaInfo();

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENAINFO, data);
        }

        public void SendReqArenaList(int type)
        {
            MsgData_cReqArenaList data = new MsgData_cReqArenaList();
            data.Type = type;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENALIST, data);
        }

        public void SendReqChallenge(int rank)
        {
            MsgData_cReqArenaChallenge data = new MsgData_cReqArenaChallenge();
            data.Rank = rank;

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENACHALLENGE, data);
            MainPanelMgr.gotoPanel("UIArenaFight");
        }

        public void SendReqReward()
        {
            MsgData_cReqArenaReward data = new MsgData_cReqArenaReward();

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENAREWARD, data);
        }

        public void SendReqRecord()
        {
            MsgData_cReqArenaRecord data = new MsgData_cReqArenaRecord();

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENARECORD, data);
        }

        public void SendReqBuyTimes()
        {
            MsgData_cReqBuyArenaTimes data = new MsgData_cReqBuyArenaTimes();

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENABUYTIMES, data);
        }

        public void SendReqBuyCD()
        {
            MsgData_cReqBuyArenaCD data = new MsgData_cReqBuyArenaCD();

            CoreEntry.netMgr.send((Int16)NetMsgDef.C_ARENABUYCD, data);
        }
        #endregion

        #region ������ģ��ս��
        private void DoEnterScene()
        {

        }

        private void OnAfterLoadScene(GameEvent ge, EventParameter parameter)
        {
            if (isArenaFight)
            {
                LoadFigters();
            }
        }

        public void OnFightUILoaded(GameEvent ge, EventParameter parameter)
        {
            //UI�������ɣ���ʾ����
            if (isArenaFight)
            {
                //MainPanelMgr.Instance.HideDialog("JoystickPanel");
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_AREAN_SHOW_FIGHT, null);
            }
        }

        private void LoadFigters()
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == mLocalIndex)
                {
                    continue;
                }

                MsgData_sResArenaMemVo player = ArenaFighters[i];
                int modelID = SceneLoader.GetClothesModelID(player.FashionDress, player.Dress, player.Job,0);

                GameObject obj = CoreEntry.gSceneLoader.CreateGameObject(modelID);
                if (obj == null) return;
                ActorObj actorObject = null;
                actorObject = obj.GetComponent<ActorObj>();
                if (null != actorObject)
                {
                    UnityEngine.Object.DestroyImmediate(actorObject);
                }

                PlayerAgent agent = obj.GetComponent<PlayerAgent>();
                if (null != agent)
                {
                    agent.enabled = false;
                }

                Vector3 position = Vector3.zero;
                position.x = initPos[1 - mLocalIndex].x;
                position.z = initPos[1 - mLocalIndex].y;
                position.y = CommonTools.GetTerrainHeight(new Vector2(position.x, position.y));

                obj.transform.position = position;
                obj.transform.rotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;

                OtherPlayer otherPlayer = obj.AddComponent<OtherPlayer>();
                otherPlayer.Init(modelID, modelID, player.RoleID, "", true);
                otherPlayer.ServerID = player.RoleID;
                CoreEntry.gActorMgr.AddActorObj(otherPlayer);

                int weaponID = SceneLoader.GetWeaponModelID(player.FashionWeapon, player.ShenBing, player.Weapon, player.Job);
                LoadWeapon(obj.transform, weaponID, player.Job == 4);
            }
        }

        private void LoadWeapon(Transform mainBody,int id, bool bDoubleWeapon)
        {
            Transform rTran = ActorObj.RecursiveFindChild(mainBody, "DM_R_Hand");
            if (null == rTran)
            {
                LogMgr.UnityError("no DM_R_Hand point!!!");

                return;
            }

            GameObject obj = SceneLoader.LoadModelObject(id);
            if (null == obj)
            {
                LogMgr.UnityError("no weapon:id " + id);

                return;
            }

            if (bDoubleWeapon)
            {
                Transform lTran = ActorObj.RecursiveFindChild(mainBody, "DM_L_Hand");
                if (null == lTran)
                {
                    LogMgr.UnityError("no DM_L_Hand point!!!");

                    return;
                }

                Transform lWeapon = obj.transform.Find("DM_L_wuqi01");
                Transform rWeapon = obj.transform.Find("DM_R_wuqi01");

                lWeapon.SetParent(lTran);
                lWeapon.localPosition = Vector3.zero;
                lWeapon.localScale = Vector3.one;
                lWeapon.localRotation = Quaternion.identity;

                rWeapon.SetParent(rTran);
                rWeapon.localPosition = Vector3.zero;
                rWeapon.localScale = Vector3.one;
                rWeapon.localRotation = Quaternion.identity;

                UnityEngine.Object.Destroy(obj); 
            }
            else
            {
                obj.transform.SetParent(rTran);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.identity;
            }
        }

        public void OnSkillDamage(DamageParam damageParam)
        {
            long attkerID = damageParam.attackActor.ServerID;
            int behitIndex = 0;

            MsgData_sResArenaMemVo attker = null;
            MsgData_sResArenaMemVo beAtker = null;
            if (attkerID == mArenaFighters[0].RoleID)
            {
                attker = mArenaFighters[0];
                beAtker = mArenaFighters[1];
                behitIndex = 1;
            }
            else
            {
                attker = mArenaFighters[1];
                beAtker = mArenaFighters[0];
                behitIndex = 0;
            }

            double dmg_value = .0f;
		    double extra_attr_dmg = .0f;
		    double attack_dmg = attker.Atk;

		    //1.�����Ƿ�����,Э��û�������ֶΣ���ȥ��

		    int src_level = (int)attker.Level;
		    int dst_level = (int)beAtker.Level;
		    //2.��������,����������
            // ����������;
            string dmgParam = ConfigManager.Instance.Consts.GetValue<string>(3, "param");
            string[] param1 = dmgParam.Split(',');
            float nVal1 = .0f;
            float nVal2 = .0f;
            if (param1.Length == 2)
            {
                if (!float.TryParse(param1[0], out nVal1))
                {
                    nVal1 = .0f;
                }
                if (!float.TryParse(param1[1], out nVal2))
                {
                    nVal2 = .0f;
                }
            }

            LuaTable lvupCfg = ConfigManager.Instance.Actor.GetLevelUpConfig(dst_level);
		    double aviod_param1 = lvupCfg.Get<float>("lv_pvesubdamage");

            dmgParam = ConfigManager.Instance.Consts.GetValue<string>(61, "param");
            param1 = dmgParam.Split('#');
		    double aviod_param2 = 0.8f;
		    double aviod_param3 = 0.0f;
            string[] param2 = null;
            if (param1.Length > 1)
            {
                param2 = param1[1].Split(',');
            }
            if (null != param2 && param2.Length == 2)
            {
                if (!double.TryParse(param2[0], out aviod_param2))
                {
                    aviod_param2 = 0.8f;
                }
                if (!double.TryParse(param2[1], out aviod_param3))
                {
                    aviod_param3 = 0.0f;
                }
            }

		    double damage_f1 = 0.0f;
		    double damage_f2 = 0.0f;
		    double damage_f3 = 0.0f;
		    double dst_def = Math.Max(1, beAtker.Def - attker.SubDef);		

			double dst_avoid_rate = (dst_def / (dst_def + dst_level * aviod_param1)) * aviod_param2 + aviod_param3;
            damage_f1 = (attack_dmg * UnityEngine.Random.Range(nVal1, nVal2) * (1.0f) + extra_attr_dmg) * (1 - dst_avoid_rate);

			// �����Ƿ񱩻�;
            param2 = null;
            if (param1.Length > 2)
            {
                param2 = param1[2].Split(',');
            }
            double critical_param1 = 0.6f;
            double critical_param2 = 0.0f;
            double critical_param3 = 3.0f;
            if (null != param2 && param2.Length == 3)
            {
                if (!double.TryParse(param2[0], out critical_param1))
                {
                    critical_param1 = 0.6f;
                }
                if (!double.TryParse(param2[1], out critical_param2))
                {
                    critical_param2 = 0.0f;
                }
                if (!double.TryParse(param2[2], out critical_param3))
                {
                    critical_param3 = 3.0f;
                }
            }


            double cridam_param1 = 0.5f;
            double cridam_param2 = 0.2f;
            param2 = null;
            if (param1.Length > 3)
            {
                param2 = param1[3].Split(',');
            }
            if (null != param2 && param2.Length == 2)
            {
                if (!double.TryParse(param2[0], out cridam_param1))
                {
                    cridam_param1 = 0.5f;
                }
                if (!double.TryParse(param2[1], out cridam_param2))
                {
                    cridam_param2 = 0.2f;
                }
            }

			double src_critical = attker.Cri;
			double src_critical_dmg = attker.CriValue;
			double puncture = attker.AbsAtk;
			double dst_tenacity = beAtker.DefCri;
			double dst_critical_def = beAtker.SubCri;
			double critical_rate = 0.0f;

			critical_rate = (src_critical / (src_critical + dst_tenacity * critical_param3)) * critical_param1 + critical_param2;

			double critical_dmg = Math.Max((src_critical_dmg - dst_critical_def + cridam_param1), cridam_param2); // ����;
            bool isCritical = false;
            if (UnityEngine.Random.Range(0.0f, 1.0f) < critical_rate)
			{
                isCritical = true;
				damage_f2 = (damage_f1 + puncture) * (1 + critical_dmg);
			}
			else
			{
                isCritical = false;
				damage_f2 = (damage_f1 + puncture);
			}

			// �����Ƿ񷢶�����,Э��û�������ֶΣ���ȥ��

			// �����˺�;
			double mindmg_param = 0.5f; //��С�˺�ϵ��;
			damage_f3 = Math.Max(src_level * mindmg_param, damage_f2);

			dmg_value = damage_f3;

		    //3.�˺������ͼ���;
		    dmg_value = dmg_value * (1 + (attker.DmgAdd * 0.01f));
		    dmg_value = dmg_value * (1 - (beAtker.DmgSub * 0.01f));

		    //4.�˺�ֵ�����ָ���;
		    if(dmg_value <= 1)
			    dmg_value = 100.0;

            ActorObj behitActorObj = damageParam.behitActor;
            BehitParam behitParam = new BehitParam();
            behitParam.displayType = DamageDisplayType.DDT_NORMAL;
            behitParam.hp = (int)dmg_value;
            behitParam.damgageInfo = damageParam;
            behitActorObj.OnSkillBeHit(behitParam);

            DoCalHP(behitIndex, dmg_value, isCritical);
        }

        private void DoCalHP(int index, double damage, bool isCritical)
        {
            if (isFightOver)
            {
                return;
            }

            int realDamage = (int)damage;
            int curHp = fighterHP[index];

            float wVal = ConfigManager.Instance.Consts.GetValue<float>(3, "fval");
            if (wVal < 1.0f)
            {
                wVal = 1.0f;
            }
            if (mFightResult == 0 && index != mLocalIndex)
            {
                realDamage = (int)(damage * wVal);
            }
            else if(mFightResult != 0 && index == mLocalIndex)
            {
                realDamage = (int)(damage * wVal);
            }

            if(curHp < realDamage)//��ǰѪ�������˺�ֵ
            {
                if (mFightResult == 0)//�ɹ�
                {
                    if (index == mLocalIndex)//��ǰ�˺�����
                    {
                        return;
                    }
                    else
                    {
                        realDamage = curHp;
                    }
                }
                else//ʧ��
                {
                    if (index == mLocalIndex)
                    {
                        realDamage = curHp;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            fighterHP[index] -= realDamage;

            EventParameter p = EventParameter.Get();
            p.goParameter = CoreEntry.gActorMgr.GetPlayerActorByServerID(mArenaFighters[index].RoleID).gameObject;
            if (isCritical)
            {
                p.intParameter = (int)FlyTextType.Crit;
            }
            else
            {
                p.intParameter = index == mLocalIndex? (int)FlyTextType.PlayerNormal : (int)FlyTextType.MonsterNormal;
            }
            p.intParameter1 = realDamage;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FLYTEXT, p);

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ARENA_HP_REFRESH, null);

            if (fighterHP[index] <= 0)
            {
                DoFightOver();
            }
        }

        private void DoFightOver()
        {
            ActorObj winner = null;
            ActorObj loser = null;
            if (mFightResult == 0)
            {
                winner = CoreEntry.gActorMgr.MainPlayer;
                loser = CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaFighters[1 - mLocalIndex].RoleID);
            }
            else
            {
                winner = CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaFighters[1 - mLocalIndex].RoleID);
                loser = CoreEntry.gActorMgr.MainPlayer;
            }

            if (null != winner)
            {
                PlayerAgent agent = winner.GetComponent<PlayerAgent>();
                if (null != agent)
                {
                    agent.enabled = false;
                }
            }
            if (null != loser)
            {
                PlayerAgent agent = loser.GetComponent<PlayerAgent>();
                if (null != agent)
                {
                    agent.enabled = false;
                }
                loser.OnDead(0, winner, null, null);
            }
            
            if (mFightResult == 0)
            {
                MainPanelMgr.Instance.ShowPanel("UIArenaSuccess");
            }
            else
            {
                MainPanelMgr.Instance.ShowPanel("UIArenaFail");
            }

            isFightOver = true;
        }
        public void DoSkipFight()
        {
            if (mFightResult == 0)
            {
                fighterHP[1 - mLocalIndex] = 0;
            }
            else
            {
                fighterHP[mLocalIndex] = 0;
            }

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ARENA_HP_REFRESH, null);

            DoFightOver();
        }

        public void DoFight()
        {
            ////if (null != CoreEntry.gActorMgr.MainPlayer)
            ////{
            ////    PlayerObj player = CoreEntry.gActorMgr.MainPlayer.GetComponent<PlayerObj>();
            ////    if (player != null)
            ////    {
            ////        player.PlayerAgent.enabled = true;
            ////        player.m_bAutoMove = true;
            ////    }
            ////}

            ////ActorObj actor = CoreEntry.gActorMgr.GetPlayerActorByServerID(ArenaFighters[1 - mLocalIndex].RoleID);
            ////if (null != actor)
            ////{
            ////    PlayerAgent agent = actor.GetComponent<PlayerAgent>();
            ////    if (null != agent)
            ////    {
            ////        agent.enabled = true;
            ////    }
            ////}

            isFightOver = false;
        }

        public long GetAttackID(long id)
        {
            if (ArenaFighters[0].RoleID == id)
            {
                return ArenaFighters[1].RoleID;
            }
            else
            {
                return ArenaFighters[0].RoleID;
            }
        }

        /// <summary>
        /// ѡ���Ƿ�����
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public int ChooseSkillID(ActorObj actor)
        {
            if (actor == null)
            {
                return -1;
            }
            MsgData_sResArenaMemVo player = null;
            if (mArenaFighters[0].RoleID == actor.ServerID)
            {
                player = mArenaFighters[0];
            }
            else
            {
                player = mArenaFighters[1];
            }

            int skillID = -1;
            int priority = 0;
            for (int i = 0; i < player.SkillIDs.Length; i++)
            {
                int id = player.SkillIDs[i];
                if (id == 0)
                {
                    continue;
                }
                if (IsInCDTime(actor.ServerID, id))
                {
                    continue;
                }

                //ѡ�����ȼ����ߵ�
                LuaTable skillDesc = ConfigManager.Instance.Skill.GetSkillConfig(id);
                if (skillDesc != null)
                {
                    if (skillDesc.Get<int>("priority") > priority)
                    {
                        priority = skillDesc.Get<int>("priority");
                        skillID = id;
                    }
                }
            }

            if (skillID != -1)
            {
                SetSkillCastTime(actor.ServerID, skillID, Time.time);
            }

            return skillID;
        }

        /// <summary>
        /// �жϼ����Ƿ���CD��
        /// </summary>
        /// <param name="casterID"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        private bool IsInCDTime(long casterID, int skillID)
        {
            Dictionary<int, float> skillCD = null;
            if(!mSkillTimeDict.TryGetValue(casterID,out skillCD))
            {
                return false;
            }

            float lastTime = 0.0f;
            if(!skillCD.TryGetValue(skillID, out lastTime))
            {
                return false;
            }

            float curTime = Time.time;
            LuaTable skillCfg = ConfigManager.Instance.Skill.GetSkillConfig(skillID);
            if (null != skillCfg)
            {
                if((curTime - lastTime) * 1000 <= skillCfg.Get<int>("cd") * 1.0f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ���ü����ͷ�ʱ��
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="skillID"></param>
        /// <param name="time"></param>
        private void SetSkillCastTime(long serverID, int skillID, float time)
        {
            Dictionary<int, float> skillTime = null;
            if (!mSkillTimeDict.TryGetValue(serverID, out skillTime))
            {
                skillTime = new Dictionary<int, float>();
            }

            skillTime[skillID] = time;

            mSkillTimeDict[serverID] = skillTime;
        }
        #endregion
    }
}

