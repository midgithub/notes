using UnityEngine;
using System.Collections;
using XLua;
using System;
using System.Collections.Generic;
using SG.AutoAI;

namespace SG
{
[LuaCallCSharp]
[Hotfix]
    public class TaskMgr
    {
        public enum TaskEnum
        {
            normal = 1,
            update = 2, 
            add = 3,
            accept = 4,
            giveup = 5,
            finish = 6,
            del = 7
        }

        private static TaskMgr instance = null;
        public static TaskMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new TaskMgr();

                return instance;
            }
        }

        public static int bMainUITasking = 0;           //是否是uimain任务发起的寻路 ,OtherTaskType  枚举

        public static bool bRunAndTasking = false;    //自动开始下个任务寻路(uitask内部调用)
                                                      //  true -->点击任务，自动寻路开始下一个任务)

        public static bool bRunAndTaskingPart = false;  //自动开始下个任务寻路  ，默认false
                                                        //  true 表示 部分任务不支持自动任务(NPC对话，进入任务副本类型)

        public static int RunTaskType = 0;   //大于0的情况下表示正在任务进行中， 正在跑的任务类型  1主线， 2跑环   3 讨伐  5帮派任务
        public int lastRunTaskType = 0;   //暂停自动任务时，保存暂停前的自动任务类型

        public static int LastClickTaskID = -1;   //上一次点击的任务ID
        public static bool UseNewAutoAI = true;   //是否使用新的挂机流程
        static bool isWildTAsk_ = false;
        //是否是野外挂机
        public static bool IsWildTask
        {
            get
            {
                return isWildTAsk_;
            }
            set
            {
                //LuaMgr.Instance.PrintStack();
                //LogMgr.LogError("IsWildTask set : " + value);
                isWildTAsk_ = value;
            }
        }
        /// <summary>
        /// 是否是待机状态或受击状态  (到达目的地，判断状态，执行下一步)
        /// </summary>
        public static int IsStandState
        {
            get
            {
                return (int)CoreEntry.gActorMgr.MainPlayer.curActorState;
            }
        }
        /// <summary>
        /// 是否没有全屏界面处于打开状态
        /// </summary>
        public static bool IsNoneOpen
        {
            get
            {
                //LogMgr.LogError("MainPanelMgr.Instance.CurPanelName  " + MainPanelMgr.Instance.CurPanelName);
                return MainPanelMgr.Instance.CurPanelName == "UIMain";
            }
        }

        /// <summary>
        /// 弹领取框的相关任务状态 --是否可弹框领取   任务完成时为true ,弹框后，改变值为false
        /// </summary>
        public Dictionary<OtherTaskType, bool> TaskRewardList = new Dictionary<OtherTaskType, bool>();

        /// <summary>
        /// 是否有奖励可领取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool bTaskRewardOpen(int type)
        {
            if (TaskRewardList.ContainsKey((OtherTaskType)type))
            {
                return TaskRewardList[(OtherTaskType)type];
            }
            return true;
        }

        /// <summary>
        /// 当前类型奖励框弹出后，bool 设fasle
        /// </summary>
        /// <param name="type"></param>
        public void SetTaskRewardFalse(int type)
        {
            if(TaskRewardList.ContainsKey((OtherTaskType)type))
            {
                TaskRewardList[(OtherTaskType)type] = false;
            }
        }

        /// <summary>
        /// 返回当前正在进行的任务，， 返回0 表示 没有自动任务， 或者 当前类型任务未接取。 
        /// </summary>
        /// <returns></returns>
        public int GetCurAutoTaskState()
        {
            if (RunTaskType > 0)
            {
                int mid = GetCurTypeTaskId(RunTaskType);
                if (questList.ContainsKey(mid))
                {
                    return questList[mid].state;
                }
            }
            return 0;
        }

        /// <summary>
        /// 返回击杀怪物id, 0表示任务未在进行中 或 当前任务类型不是杀怪任务
        /// </summary>
        /// <returns></returns>
        public int GetCurTargetMonsterId()
        {
            int monsterId = 0;  
            int curTaskId = 0;
            string questGoals = "";
            MsgData_sQuestInfo curTaskMsg;
            if(RunTaskType >0 && RunTaskType < 6) 
            {
                if(RunTaskType != 4)
                {
                    curTaskId = GetCurTypeTaskId(RunTaskType);
                    if(curTaskId == 0)     //当前任务类型全部打完的情况下， 数据清空,没有目标怪
                    {
                        return monsterId;
                    }
                    curTaskMsg = GetCurTaskMsg(curTaskId);
                    switch (RunTaskType)
                    {
                        case 1:  //主线
                            if(curTaskMsg!=null)
                            {
                                if (curTaskMsg.state == 1)  //任务进行中
                                {
                                    LuaTable cg1 = ConfigManager.Instance.Task.GetMainConfig(curTaskId);
                                    int kind = cg1.Get<int>("kind");
                                    if (kind == 2 || kind == 3 || kind == 10 || kind == 13)
                                    {
                                        questGoals = cg1.Get<string>("questGoals");
                                    }
                                }
                            }
                            break;
                        case 2:  //经验任务
                            LuaTable cg2 = ConfigManager.Instance.Task.GetDailyConfig(curTaskId);
                            questGoals = cg2.Get<string>("questGoals");
                            break;
                        case 3:  //讨伐
                            LuaTable cg3 = ConfigManager.Instance.Task.GetAgainstConfig(curTaskId);
                            questGoals = cg3.Get<string>("questGoals");
                            break;                      
                        case 5:  //帮派
                            LuaTable cg5 = ConfigManager.Instance.Task.GetGuildConfig(curTaskId);
                            questGoals = cg5.Get<string>("questGoals");
                            break;
                        default:
                            break;
                    }
                    if(!string.IsNullOrEmpty(questGoals))
                    {
                        string[] target1 = questGoals.Split(',');
                //        LogMgr.LogError("target1  " + target1[0]);
                        if (target1.Length > 0)
                        {
                            monsterId = Convert.ToInt32(target1[0]);
                        }
                    }
                }
                else
                {
                    if(otherQuestList.ContainsKey(RunTaskType))
                    {
                        OtherTaskData tt = otherQuestList[RunTaskType];
                        curTaskId = tt.id;
                        if (tt.state == 1)  //通缉 进行中
                        {
                            LuaTable l = ConfigManager.Instance.Task.GetTongjiConfig(curTaskId);
                            if (l != null)
                            {
                                monsterId = l.Get<int>("monster_id");
                            }
                        }
                    }
               
                }
            }

          //  LogMgr.LogError("bRunAndTasking " + bRunAndTasking);
          //  LogMgr.LogError("RunTaskType " + RunTaskType);
          //  LogMgr.LogError("Target monsterId "+monsterId);
            return monsterId;
        }

        /// <summary>
        ///  返回当前任务目标的 坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurTargetPos()
        {
            int monsterId = GetCurTargetMonsterId();
            if(monsterId == 0)
            {
                return Vector3.zero;
            }else
            {
            //    LogMgr.LogError("x  "+goPos.x+"   z "+goPos.z);
                return goPos;
            }
        }

        public bool GetCurTargetState()
        {
            int mid = 0;
            if (RunTaskType > 0 && RunTaskType < 6 && RunTaskType != 4)
            {
                if (RunTaskType != 4)
                {
                    mid = GetCurTypeTaskId(RunTaskType);
                }
                switch (RunTaskType)
                {
                    case 1:  //主线
                         MsgData_sQuestInfo mmsg = GetCurTaskMsg(mid);
                        if (mmsg.state == 2)  //任务完成
                        {
                            return true;
                        }else
                        {
                            LuaTable cg = ConfigManager.Instance.Task.GetMainConfig(mid);
                            if (cg.Get<int>("kind") == 1)  //自动完成类型
                            {
                                return true;
                            }
                        }
                        break;
              
                    default:
                        break;
                }
            }
         
            return false;
        }


        public static MsgData_sQuestInfo curMsgData;   //当前正在进行的任务

        public Vector3 goPos = Vector3.zero;    //当前寻路的目标点

        public int goMapId = 0;   //当前寻路的地图ID

        /// <summary>
        /// 保存当前任务数据，开始自动任务
        /// </summary>
        public void SetAutoTaskType(MsgData_sQuestInfo msg,int type)
        {
            bRunAndTasking = true;
            curMsgData = msg;
            RunTaskType = type;
            if(RunTaskType > 0)
            {
                lastRunTaskType = RunTaskType;
            }
        }
        /// <summary>
        /// 关闭任务挂机开关,清除挂机类型 (手动停止任务挂机)
        /// </summary>
        public void ResetAutoTaskType()
        {
            bRunAndTasking = false;
            RunTaskType = 0;
            lastRunTaskType = 0;
        }

        /// <summary>
        /// 获取上次自动挂机的任务类型(暂停任务后，重新自动任务时调用)
        /// </summary>
        /// <returns></returns>
        public int GetLastRunTaskType()
        {
            return lastRunTaskType;
        }

        /// <summary>
        /// 非UIMain主任务下拉界面点开的任务寻路   enumType  = OtherTaskType.
        /// </summary>
        /// <param name="newPos"></param>
        /// <param name="cur"></param>
        public void ChangeToTaskPos(Vector3 newPos, MsgData_sQuestInfo cur, int enumType = (int)OtherTaskType.none)
        {   
            goPos = newPos;
            bRunAndTasking = true;
            bMainUITasking = enumType;
            RunTaskType = enumType;
            curMsgData = cur;
        }

        /// <summary>
        /// 非UIMain主任务下拉界面点开的非任务类型寻路(通缉任务)   enumType  = OtherTaskType.
        /// </summary>
        /// <param name="newPos"></param>
        /// <param name="enumType"></param>
        public void ChangeToUnTaskPos(Vector3 newPos,int _goMapId, int enumType = (int)OtherTaskType.none)
        {
            goPos = newPos;
            goMapId = _goMapId;
            bRunAndTasking = true;
            //bMainUITasking = enumType;
            RunTaskType = enumType;
        }
        public void ResetTaskPos()
        {    //拿值一次后，调用此函数，变量初始化
            goPos = Vector3.zero;
            goMapId = 0;
            bRunAndTasking = false;
            RunTaskType = 0;
            bMainUITasking = (int)OtherTaskType.none;
            curMsgData = new MsgData_sQuestInfo();
        }

        public static void PlayCollectAnimation()
        {
      //      CoreEntry.gActorMgr.MainPlayer.ChangeState(ACTOR_STATE.AS_COLLECT);
        }

        public Vector3 GetTaskToPos()
        {
            return goPos;
        }

        public int GetTaskToMapID()
        {
            return goMapId;
        }

        ///// <summary>
        ///// 主线任务配置表
        ///// </summary>
        //CSVSheet questExcel;
        ///// <summary>
        ///// 经验任务配置表
        ///// </summary>
        //CSVSheet dailyExcel;
        ///// <summary>
        ///// 讨伐任务配置表
        ///// </summary>
        //CSVSheet againstExcel;
        ///// <summary>
        ///// 帮派任务配置表
        ///// </summary>
        //CSVSheet guildExcel;
        ///// <summary>
        ///// 通缉配置
        ///// </summary>
        //public static Dictionary<int, LuaTable> tongjiExcel = null;

        /// <summary>
        /// 任务列表信息
        /// </summary>
        public Dictionary<int, MsgData_sQuestInfo> questList = new Dictionary<int, MsgData_sQuestInfo>();

        /// <summary>
        /// 非任务列表信息 -- 需要揉到 任务下拉滑动界面 当做任务处理的 id
        /// key --类型， value -对应的各个配置id
        /// </summary>
        public Dictionary<int, OtherTaskData> otherQuestList = new Dictionary<int, OtherTaskData>();

        /// <summary>
        /// 日常表整合到任务栏
        /// </summary>
        public Dictionary<int, DailyActivyTaskData> dailyActivyList = new Dictionary<int, DailyActivyTaskData>();

        /// <summary>
        /// 当前正在进行中的主线任务id
        /// </summary>
        public int curTaskMainId = 0;

        /// <summary>
        /// 各任务类型的当前任务ID
        /// </summary>
        public Dictionary<OtherTaskType, int> curTypeTaskIds = new Dictionary<OtherTaskType, int>();

        /// <summary>
        /// 任务列表对应的NPC 表
        /// </summary>
        public Dictionary<int, Dictionary<int, int>> npcList = new Dictionary<int, Dictionary<int, int>>();

        /// <summary>
        /// 进行中的需要计数的任务 ，  key - 目标id ,    value - 目标id 对应的 关联的任务id集合,走怪物死亡触发此表
        /// </summary>
        private Dictionary<long, List<int>> taskingList = new Dictionary<long, List<int>>();

        /// <summary>
        /// 进行中的需要计数的采集表，   key - -采集id    value -  采集物品对应的 关联的任务id集合 走采集信息触发此表
        /// </summary>
        private Dictionary<long, List<int>> collectIds = new Dictionary<long, List<int>>();
        public TaskMgr()
        {
            //tongjiExcel = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_fengyao");
            Send_CS_QueryQuest();

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, GE_CLEANUP_USER_DATA);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_JOYSTICK_DOWN, GE_JOYSTICK_DOWN);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_QueryQuestResult, OnQueryQuestResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_QuestAdd, OnQuestAdd);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_QuestUpdate, OnQuestUpdate);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CS_AcceptQuest, OnAcceptQuestResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GiveupQuestResult, OnGiveupQuestResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_FinishQuestResult, OnFinishQuestResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_QuestDel, OnQuestDel);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_STRUCT_DEFResult, OnStructResult);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DailyQuestStar, OnDailyQuestStar);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DailyQuestFinish, OnDailyQuestFinish);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DailyQuestResult, OnDailyQuestResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DQDraw, OnDQDraw);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AgainstQuestStar, OnAgainstQuestStar);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AgainstQuestFinish, OnAgainstQuestFinish);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AgainstQuestResult, OnAgainstQuestResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AgainstQDraw, OnAgainstQDraw);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GetAgainstQSkipReward, OnGetAgainstQSkipReward);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AgainstQuestSkipResult, OnAgainstQuestSkipResult);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_AgainstQDrawNotice, OnAgainstQDrawNotice);
            // CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_GuildQuestSweep, OnGuildQuestSweep);  //返回帮派任务一键完成奖励信息 msgId:8942

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_QuestRewardResult, OnQuestRewardResult);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_OtherQuestIdChange, OnOtherQuestIdChange); //添加其他功能的id到任务表 或删除其他任务表id
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CC_DailyActivy, OnDailyActivyTaskChange); //添加日常活动表的功能到 任务表中
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_WaterDungeonInfo, OnDailyActivyTaskChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_ResSimpleSecrectDuplInfo, OnDailyActivyTaskChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_RESLEVEL_SWEEP, OnDailyActivyTaskChange);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE, OnGE_BEGIN_LOADSCENE);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FRIST_UI_LOADED, OnGE_FRIST_UI_LOADED);
        }

        public static bool bLoadSceneFinish = true;

        /// <summary>
        /// 加载场景
        /// </summary>
        void OnGE_BEGIN_LOADSCENE(GameEvent ge, EventParameter parameter)
        {
            bLoadSceneFinish = false;
        }
        /// <summary>
        /// 场景加载完毕
        /// </summary>
        void OnGE_FRIST_UI_LOADED(GameEvent ge, EventParameter parameter)
        {
            bLoadSceneFinish = true;
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void GE_CLEANUP_USER_DATA(GameEvent ge, EventParameter parameter)
        {
            questList.Clear();
            otherQuestList.Clear();
            curTypeTaskIds.Clear();
            taskingList.Clear();
            npcList.Clear();
            collectIds.Clear();
            dailyActivyList.Clear();
            ResetAutoTaskType();
        }

        /// <summary>
        /// 点击遥感后，关闭任务自动寻路开关
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void GE_JOYSTICK_DOWN(GameEvent ge, EventParameter parameter)
        {
            ResetAutoTaskType();
            CoreEntry.gActorMgr.MainPlayer.StopMove(true);
        }

        /// <summary>
        /// 获取当前任务的副本id .. 任务类型kind = 14 时，打任务副本
        /// fbID = 0,不是任务副本类型
        /// </summary>
        /// <returns></returns>
        public int GetCurTaskFbId()
        {
            int fbId = 0;
            LuaTable cg = ConfigManager.Instance.Task.GetMainConfig(curTaskMainId);
            if(cg.Get<int>("kind") == 14)
            {
                fbId = Convert.ToInt32(cg.Get<string>("uestGoals"));
            }
            return fbId;
        }

        /// <summary>
        /// 获取指定任务类型 正在进行中的任务id（不包括通缉）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetCurTypeTaskId(int type)
        {
            int id = 0;
            if (curTypeTaskIds.ContainsKey((OtherTaskType)type))
            {
                id = curTypeTaskIds[(OtherTaskType)type];
                if(GetCurTaskMsg(id) == null)
                {
                    id = 0;   
                }
            }
            return id;
        }
        /// <summary>
        /// 获取指定任务id 的msg数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MsgData_sQuestInfo GetCurTaskMsg(int id)
        {
            if (questList.ContainsKey(id))
            {
                return questList[id];
            }
            return null;
        }

        /// <summary>
        /// 保存当前任务表内的 所有采集id
        /// </summary>
        /// <param name="collectId"></param>
        /// <param name="taskId"></param>
        public void LuaToCSharpWithCollectId(long collectId, int taskId)
        {
            if (collectIds.ContainsKey(collectId))
            {
                if (!collectIds[collectId].Contains(taskId))
                {
                    collectIds[collectId].Add(taskId);
                }
            }
            else
            {
                collectIds[collectId] = new List<int>();
                collectIds[collectId].Add(taskId);
            }
        }

        public static int GetLayer(int flag)
        {
            return flag >> 16;
        }

        #region 任务栏添加或删除其他模块的id  

        /// <summary>
        ///   parameter.intParameter --1 添加， 2 删除，   parameter.objParameter - OtherTaskData类
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnOtherQuestIdChange(GameEvent ge, EventParameter parameter)
        {          
            OtherTaskData other = parameter.objParameter as OtherTaskData;
            OtherTaskState state = (OtherTaskState)parameter.intParameter;
            if (state == OtherTaskState.add)
            {
                otherQuestList[(int)other.type] = other;
            }else
            {
                if (otherQuestList.ContainsKey((int)other.type))
                {
                    otherQuestList.Remove((int)other.type);
                }
            }
            EventParameter par1 = EventParameter.Get();
            par1.intParameter = (int)other.type; //其他任务类型
            par1.intParameter1 = (int)state;   //添加or删除
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_OtherQuestUpdate, par1);
        }

        #endregion

        public void OnDailyActivyTaskChange(GameEvent ge, EventParameter parameter)
        {
            dailyActivyList.Clear();
            foreach (var item in DailyMgr.Instance.dailyTaskList)
            {
                LuaTable l;
                int MissionShortId = 0;
                if (DailyMgr.activyExcelConfig.TryGetValue(item.Key, out l))
                {
                    MissionShortId = l.Get<int>("MissionShortId");
                }
                    DailyActivyTaskData tt = new DailyActivyTaskData(OtherTaskType.DailyActivyTask, item.Key, item.Value, MissionShortId);
                dailyActivyList[item.Key] = tt;
            }
            EventParameter par1 = EventParameter.Get();
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DailyActivyTaskUpdate, par1);
        }

        /// <summary>
        /// （任务栏其他模块） 魔宠幻境 重置次数
        /// </summary>
        public void OnDungeonActivyChange(int lessNum)
        {
            if(lessNum > 0)
            {
                if (dailyActivyList.ContainsKey(93))
                {
                    DailyActivyTaskData tt = dailyActivyList[93];
                    tt.info.count = lessNum;
                    dailyActivyList[93] = tt;
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DailyActivyTaskUpdate, EventParameter.Get());
                }
            }else
            {
                if (dailyActivyList.ContainsKey(93))
                {
                    dailyActivyList.Remove(93);
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DailyActivyTaskUpdate, EventParameter.Get());
                }
            }
           
        }


        #region 请求服务器
        /// <summary>
        /// 请求任务列表
        /// </summary>
        public void Send_CS_QueryQuest()
        {
            MsgData_cQueryQuest rsp = new MsgData_cQueryQuest();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_QueryQuest, rsp);
        }

        /// <summary>
        /// 请求接受任务
        /// </summary>
        public void Send_CS_AcceptQuest(int id)
        {
            MsgData_cAcceptQuest rsp = new MsgData_cAcceptQuest();
            rsp.questId = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_AcceptQuest, rsp);
        }
        /// <summary>
        /// 请求放弃任务
        /// </summary>
        public void Send_CS_GiveUpQuest(int id)
        {
            MsgData_cGiveUpQuest rsp = new MsgData_cGiveUpQuest();
            rsp.questId = id;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GiveupQuest, rsp);
        }

        /// <summary>
        /// 请求完成任务  id -> 任务id ,    multipIndex -> 1.免费领1倍，2.银两领双倍，3.元宝领三倍
        /// </summary>
        public void Send_CS_FinishQuest(int id,int multipIndex = 1, int taskType = 1)
        {
            MsgData_cFinishQuest rsp = new MsgData_cFinishQuest();
            rsp.questId = id;
           // if(TaskRewardList.ContainsKey((OtherTaskType)taskType) && multipIndex <= 1)
            if (TaskRewardList.ContainsKey((OtherTaskType)taskType))
            {
              TaskRewardList[(OtherTaskType)taskType] = false;  
            }
            rsp.multipIndex = multipIndex;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_FinishQuest, rsp);
 //           MoveToPos();
        }

        /// <summary>
        /// npcId - 传NPC的 配置表id
        /// 判断NPC任务状态
        /// 判断任务表中是否存在这个NPC，  0 -不是或 任务已完成已交过，  1-任务进行中,未完成，    2 -任务已完成,未交
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public int CheckNpcWithTaskState(int npcId)
        {
            int state = 0;
            //int questId = 0;
            if (npcList.ContainsKey(npcId))
            {
                foreach (var item in npcList[npcId])
                {                
                    if (state < item.Value)
                    {
                        state = item.Value;
                        //questId = item.Key;
                        if (state == 2)   //已完成
                        {
                            return state;
                        }
                    }
                }
            }
            return state;
        }

        /// <summary>
        /// npcId - 传NPC的 配置表id ，和任务state > 0
        /// 判断任务表中是否存在这个NPC
        /// 返回 任务表quest的 配置id ,按任务状态返回，
        /// <param name="npcId"></param>
        /// <returns></returns>
        public int CheckNpcWithTaskId(int npcId,int state)
        {
            int questId = 0;
            if (npcList.ContainsKey(npcId))
            {
                foreach (var item in npcList[npcId])
                {
                    if (state == item.Value)
                    {
                        questId = item.Key;
                        return questId;
                    }
                } 
            }
            return questId;
        }

        /// <summary>
        /// 查找当前任务 对应的 采集物品id
        /// </summary>
        /// <param name="taskId"></param>
        public long CheckCollectMsg(int taskId)
        {
            foreach (var item in collectIds)
            {
                if (item.Value.Contains(taskId))
                {
                    // Send_CS_CollectData(item.Key);
                    bool bLoad = CheckCollectInScene(item.Key);
                    if (bLoad == false)
                    {
                        return 0;
                    }
                    return item.Key;
                }
            }
            return 0;
        }

        /// <summary>
        /// 检查附近是否存在当前采集obj
        /// </summary>
        /// <param name="collectId"></param>
        /// <returns></returns>
        public bool CheckCollectInScene(long collectId)
        {
            //MsgData_cStructDef rsp = new MsgData_cStructDef();
            CollectionObj obj = CoreEntry.gEntityMgr.GetCollectionByConfigID((int)collectId) as CollectionObj;
            //ActorObj obj = CoreEntry.gActorMgr.GetActorByResID((int)collectId);
            if (obj == null)
            {
              //LogMgr.LogError("附近采集资源未加载完");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送触碰采集msg
        /// </summary>
        /// <param name="collectId"></param>
        public void Send_CS_CollectData(long collectId)
        {
            MsgData_cStructDef rsp = new MsgData_cStructDef();
            CollectionObj obj = CoreEntry.gEntityMgr.GetCollectionByConfigID((int)collectId) as CollectionObj;
            //ActorObj obj = CoreEntry.gActorMgr.GetActorByResID((int)collectId);
            if (obj == null)
            {
                return;
            }
            rsp.cID = obj.ServerID;
            if (rsp.cID > 0)
            {
               LogMgr.Log("发送采集请求rsp.cID   " + rsp.cID);
                CoreEntry.netMgr.send((Int16)NetMsgDef.C_STRUCT_DEF, rsp);
            }
        }

        /// <summary>
        /// 玩家自身的坐标
        /// </summary>
        /// <returns></returns>
        public Vector3 MyPlayPos()
        {
            /*
            Vector3 myPos = new Vector3();
            ActorObj ag = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
            myPos = ag.transform.position;
            */
            if(CoreEntry.gActorMgr.MainPlayer == null)
            {
                return Vector3.zero;
            }
            return CoreEntry.gActorMgr.MainPlayer.transform.position;
        }

        //移动到某点    mapId ..场景id...
        public void MoveToPos(Vector3 end, int mapId = 0)
        {
			//Debug.LogError ("====移动到某点===="+ mapId + "=="+end );

            if(MoveDispatcher.bFly)   //传送过程中，停止寻路
            {
           //     LogMgr.LogError("传送过程中，停止寻路");
                CoreEntry.gActorMgr.MainPlayer.StopMove(true);
                return;
            }
            if(end == Vector3.zero)
            {
                Debug.Log("坐标为空");
                return;
            }
            CoreEntry.gAutoAIMgr.AutoFight = false;   //自动打怪停止
            CoreEntry.gActorMgr.MainPlayer.StopMove(true);
            ActorObj ag = CoreEntry.gActorMgr.GetActorByServerID(MainRole.Instance.serverID);
            Vector3 to = GetNearPoint(ag.transform.position, end);
            if(mapId == 0)
            {
                mapId = MapMgr.Instance.EnterMapId;
            }
            MulScenesPathFinder.Instance.StartPathFinder(mapId, to);
            ag.ReqRideHorse();
        }

        /// <summary>
        /// 取该目标点附近的非阻挡点，from -->起点，pos 终点
        /// </summary>
        /// <param name="from"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector3 GetNearPoint(Vector3 from,Vector3 pos,float r = 1f)
        {
            int direction_x = pos.x - from.x > 0 ? 1 : -1;
            int direction_z = pos.z - from.z > 0 ? 1 : -1;
            return new Vector3(pos.x - direction_x * r, pos.y, pos.z - direction_z * r);
        }

        #endregion

        #region 服务器返回

        /// <summary>
        /// 返回任务列表
        /// </summary>
        /// <param name="msg"></param>
        public void OnQueryQuestResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sQueryQuestResult resp = parameter.msgParameter as  MsgData_sQueryQuestResult;
            if(resp.questListCount > 0)
            {
                foreach (var item in resp.questList)
                {
                    questList[item.id] = item;
                    LuaTable cg = ConfigManager.Instance.Task.GetMainConfig(item.id);
                    if (cg != null)  //主线任务
                    {
                        curTaskMainId = cg.Get<int>("id");
                        curTypeTaskIds[OtherTaskType.Quest] = item.id;
                        SendCrownMainTaskMsg(cg.Get<int>("id"));
                        continue;
                    }
                    LuaTable cg1 = ConfigManager.Instance.Task.GetDailyConfig(item.id);
                    if (cg1 != null)  //经验任务
                    {
                       curTypeTaskIds[OtherTaskType.Daily] = item.id;
                       continue;
                    }

                    LuaTable cg2 = ConfigManager.Instance.Task.GetAgainstConfig(item.id);
                    if (cg2 != null)  //讨伐任务
                    {
                        curTypeTaskIds[OtherTaskType.Against] = item.id;
                        continue;
                    }

                    LuaTable cg3 = ConfigManager.Instance.Task.GetGuildConfig(item.id);
                    if (cg3 != null)  //帮派任务
                    {
                        curTypeTaskIds[OtherTaskType.Guild] = item.id;
                        continue;
                    }

                    LuaTable cg4 = ConfigManager.Instance.Task.GetBranchConfig(item.id);
                    if (cg4 != null)  //支线任务
                    {
                        curTypeTaskIds[OtherTaskType.Branch] = item.id;
                        continue;
                    }
                }
            }
            taskingList.Clear();
            collectIds.Clear();
            npcList.Clear();
            foreach (var item in questList)     //刷新进行中任务计数
            {
                if(item.Value.state == 1 && item.Value.goalsList.Length > 0)
                {
                    for (int i = 0; i < item.Value.goalsList.Length; i++)
                    {
                        long cur = item.Value.goalsList[i].goalId;
                        if(taskingList.ContainsKey(cur))
                        {
                            if(!taskingList[cur].Contains(item.Value.id))
                            {
                                taskingList[cur].Add(item.Value.id);
                            }
                        }else
                        {
                            taskingList[cur] = new List<int>();
                            taskingList[cur].Add(item.Value.id);
                        }
                    }
                }
            }
            foreach (var item in questList)    //刷新任务对应的NPC表
            {            
                LuaTable qt = ConfigManager.Instance.Task.GetMainConfig(item.Key);
                if (qt == null)
                {
                    continue;
                }
                int fnpc = qt.Get<int>("finishNpc");
                if (fnpc > 0 )  //&& item.Value.state == 2
                {
                    if(npcList.ContainsKey(fnpc))
                    {
                        if(!npcList[fnpc].ContainsKey(item.Value.id))
                        {
                            npcList[fnpc][item.Value.id] = item.Value.state;  //刷新NPC对应的任务状态
                        }
                    }else
                    {
                        npcList[fnpc] = new Dictionary<int, int>();
                        npcList[fnpc][item.Value.id] = item.Value.state;
                    }
                }
                if(item.Value.state == 1)   //进行中
                {

                }
                else if (item.Value.state == 2)   //完成
                {

                }
            }
            SendMsg(TaskEnum.normal, resp);
        }

        /// <summary>
        /// 返回增加一个任务信息
        /// </summary>
        /// <param name="msg"></param>
        public void OnQuestAdd(GameEvent ge, EventParameter parameter)
        {
            MsgData_sQueryAdd resp = parameter.msgParameter as MsgData_sQueryAdd;
            MsgData_sQuestInfo tt = new MsgData_sQuestInfo();
            tt.id = (int)resp.id;
            //     LogMgr.LogError("添加任务 tt.id " + tt.id +" 任务状态state  "+tt.state);
            tt.state = resp.state;
            tt.flag = resp.flag;
            if (tt.state == 2)
            {
                if (!AutoAIFindTarget.IsInDungeon())
                {
                    CoreEntry.gAutoAIMgr.AutoFight = false;   //完成后，自动打怪停止
                }
            }
            tt.goalsList = resp.goalsList.ToArray();
            questList[tt.id] = tt;
            LuaTable qt = ConfigManager.Instance.Task.GetMainConfig(tt.id);
            if (qt != null)
            {
                curTaskMainId = qt.Get<int>("id");
                curTypeTaskIds[OtherTaskType.Quest] = tt.id;
                SendCrownMainTaskMsg(tt.id);
                int fnpc = qt.Get<int>("finishNpc");
                if (fnpc > 0) // && tt.state == 2                            //刷新任务对应的NPC表   
                {
                    if (npcList.ContainsKey(fnpc))
                    {
                        npcList[fnpc][tt.id] = tt.state;
                    }
                    else
                    {
                        npcList[fnpc] = new Dictionary<int, int>();
                        npcList[fnpc][tt.id] = tt.state;
                    }
                }
            }
            else
            {
                LuaTable cg1 = ConfigManager.Instance.Task.GetDailyConfig(tt.id);
                if (cg1 != null)  //经验任务
                {
                    curTypeTaskIds[OtherTaskType.Daily] = tt.id;
                }
                else
                {
                    LuaTable cg2 = ConfigManager.Instance.Task.GetAgainstConfig(tt.id);
                    if (cg2 != null)  //讨伐任务
                    {
                        curTypeTaskIds[OtherTaskType.Against] = tt.id;
                    }
                    else
                    {
                        LuaTable cg3 = ConfigManager.Instance.Task.GetGuildConfig(tt.id);
                        if (cg3 != null)  //帮派任务
                        {
                            curTypeTaskIds[OtherTaskType.Guild] = tt.id;
                        }
                        else
                        {
                            LuaTable cg4 = ConfigManager.Instance.Task.GetBranchConfig(tt.id);
                            if (cg4 != null)
                            {
                                curTypeTaskIds[OtherTaskType.Branch] = tt.id;
                            }
                            SendMsg(TaskEnum.add, tt, tt.id);
                        }
                        
                    }
                }
            }
            SendMsg(TaskEnum.add, tt, tt.id);
        }

        [CSharpCallLua]
        public delegate void ClickTaskContinue();
        /// <summary>
        /// 返回任务更新 
        /// </summary>
        /// <param name="msg"></param>
        public void OnQuestUpdate(GameEvent ge, EventParameter parameter)
        {
            MsgData_sQueryUpdate resp = parameter.msgParameter as MsgData_sQueryUpdate;
            MsgData_sQuestInfo tt = new MsgData_sQuestInfo();
            tt.id = (int)resp.id;
            tt.state = resp.state;
            tt.flag = resp.flag;
            //            tt.goalListCount = resp.goalListCount;
            // LogMgr.LogError("更新任务状态 tt.id " + tt.id + " 任务状态state  " + tt.state);
            bool bChange = false;
            if(questList.ContainsKey(tt.id))
            {
               if( questList[tt.id].state == 0 && tt.state > 0)  
                {
                    bChange = true;                 
                }
            }
            tt.goalsList = resp.goalsList.ToArray();
            questList[tt.id] = tt;
            for (int i = 0; i < tt.goalsList.Length; i++)    //刷新进行中任务计数
            {
                int cur = tt.goalsList[i].goalId;
                if (taskingList.ContainsKey(cur))
                {
                    if (!taskingList[cur].Contains(tt.id))
                    {
                        taskingList[cur].Add(tt.id);
                    }
                }
                else
                {
                    taskingList[cur] = new List<int>();
                    taskingList[cur].Add(tt.id);
                }
            }
            OtherTaskType _taskType = OtherTaskType.none;
            LuaTable qt = ConfigManager.Instance.Task.GetMainConfig(tt.id);
            if(qt != null)
            {
                _taskType = OtherTaskType.Quest;
                int fnpc = qt.Get<int>("finishNpc");
                if (fnpc > 0) // && tt.state == 2                            //刷新任务对应的NPC表   
                {
                    if (tt.state == 2)
                    {
                        if (!AutoAIFindTarget.IsInDungeon())
                        {
                            CoreEntry.gAutoAIMgr.AutoFight = false;   //完成后，自动打怪停止
                        }                        
                    }
                    if (npcList.ContainsKey(fnpc))
                    {
                        npcList[fnpc][tt.id] = tt.state;
                    }
                    else
                    {
                        npcList[fnpc] = new Dictionary<int, int>();
                        npcList[fnpc][tt.id] = tt.state;
                     //   Debug.Log("NPC表计数2添加任务: " + resp.id + " NPC: " + fnpc);
                    }

                }
            }
            int id = (int)tt.id;
            LuaTable cg1 = ConfigManager.Instance.Task.GetDailyConfig(id);
            if (cg1 != null)  //经验任务
            {
                TaskRewardList[OtherTaskType.Daily] = true;
                _taskType = OtherTaskType.Daily;
            }
            else
            {
                LuaTable cg2 = ConfigManager.Instance.Task.GetAgainstConfig(id);
                if (cg2 != null)  //讨伐任务
                {
                    TaskRewardList[OtherTaskType.Against] = true;
                    _taskType = OtherTaskType.Against;
                }
                else
                {
                    LuaTable cg3 = ConfigManager.Instance.Task.GetGuildConfig(id);
                    if (cg3 != null)  //帮派任务
                    {
                        TaskRewardList[OtherTaskType.Guild] = true;
                        _taskType = OtherTaskType.Guild;
                    }
                    else
                    {
                        LuaTable cg4 = ConfigManager.Instance.Task.GetBranchConfig(id);
                        if (cg4 != null)
                        {
                            TaskRewardList[OtherTaskType.Branch] = true;
                            _taskType = OtherTaskType.Branch;
                        }
                    }
                }
            }
            SendMsg(TaskEnum.update, tt, tt.id);
            if (bChange && RunTaskType ==(int)_taskType && RunTaskType >0)  //上次发的任务状态为未接取时，当更新状态为进行中时，再执行一次寻路操作
            {
                LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
                ClickTaskContinue fun = G.GetInPath<ClickTaskContinue>("Common.AutoTaskContinue");
                if (fun != null)
                {
                    fun();
                }
            }
        }
        /// <summary>
        /// 返回接受任务处理结果 
        /// </summary>
        /// <param name="msg"></param>
        public void OnAcceptQuestResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sAcceptQuestResult resp = parameter.msgParameter as MsgData_sAcceptQuestResult;           
            SendMsg(TaskEnum.accept, resp);
        }
        /// <summary>
        /// 返回放弃任务处理结果 
        /// </summary>
        /// <param name="msg"></param>
        public void OnGiveupQuestResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sGiveupQuestResult resp = parameter.msgParameter as MsgData_sGiveupQuestResult;
            if (resp.result == 0)
            {
                if (questList.ContainsKey((int)resp.id))
                {
                    questList.Remove((int)resp.id);
                }
                SendMsg(TaskEnum.giveup, resp, (int)resp.id);
            }
        }

        public bool bFinishFbQuest = false;

        /// <summary>
        /// 返回完成任务处理结果 
        /// </summary>
        /// <param name="msg"></param>
        public void OnFinishQuestResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sFinishQuestResult resp = parameter.msgParameter as MsgData_sFinishQuestResult;
            if (resp.result == 0)
            {
               // LogMgr.LogError("完成任务 tt.id " + resp.id);
                bRunAndTasking = false;
                if (questList.ContainsKey((int)resp.id))
                {
                    questList.Remove((int)resp.id);
                }
                int id = (int)resp.id;
                LuaTable cg = ConfigManager.Instance.Task.GetMainConfig(id);
                if (cg != null)
                {
                    if(cg.Get<int>("kind") == 14)   //副本类型
                    {
                        bFinishFbQuest = true;
                    }
                }
                else
                {
                    //存储可领取奖励的 任务类型
                    LuaTable cg1 = ConfigManager.Instance.Task.GetDailyConfig(id);
                    if (cg1 != null)  //经验任务
                    {
                        TaskRewardList[OtherTaskType.Daily] = false;
                    }
                    else
                    {
                        LuaTable cg2 = ConfigManager.Instance.Task.GetAgainstConfig(id);
                        if (cg2 != null)  //讨伐任务
                        {
                            TaskRewardList[OtherTaskType.Against] = false;
                        }
                        else
                        {
                            LuaTable cg3 = ConfigManager.Instance.Task.GetGuildConfig(id);
                            if (cg3 != null)  //帮派任务
                            {
                                TaskRewardList[OtherTaskType.Guild] = false;
                            }
                            else
                            {
                                LuaTable cg4 = ConfigManager.Instance.Task.GetBranchConfig(id);
                                if (cg4 != null)  //支线任务
                                {
                                    TaskRewardList[OtherTaskType.Branch] = false;
                                }
                            }
                        }
                    }
                }
                SendMsg(TaskEnum.finish, resp, (int)resp.id);
            }
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        public void OnQuestDel(GameEvent ge, EventParameter parameter)
        {
            MsgData_sQuestDele resp = parameter.msgParameter as MsgData_sQuestDele;
           // Debug.Log("删除任务: " +resp.id);
            if (questList.ContainsKey((int)resp.id))
            {
                questList.Remove((int)resp.id);
            }
            SendMsg(TaskEnum.del, resp, (int)resp.id);
            //完成任务，放弃任务 ，之后都会走到 删除任务

            List<int> deleNpcList = new List<int>();
            foreach (var item in npcList)
            {
                if(item.Value.ContainsKey((int)resp.id))
                {
                    if(!deleNpcList.Contains(item.Key))
                    {
                        deleNpcList.Add(item.Key);
                    }
                    item.Value.Remove((int)resp.id);                 
                }          
            }
            foreach (var item in deleNpcList)
            {
                if(npcList[item].Count == 0)
                {
                    npcList.Remove(item);
                 //   Debug.Log("删除任务NPC: " +item);
                }
            }
        }


        public void OnStructResult(GameEvent ge, EventParameter parameter)
        {
            MsgData_sStructDefResult resp = parameter.msgParameter as MsgData_sStructDefResult;
          //  LogMgr.LogError("采集反馈resp.result =  " + resp.result);
            if(resp.result == 0)
            {
                LogMgr.Log("采集完成...");
            }
            else
            {
                PortalObj portal = CoreEntry.gEntityMgr.GetPortalByServerID(resp.cID) as PortalObj;
                if (portal != null)
                {
                    if (2 == resp.result)
                    {
                        string msg = string.Format("角色等级不足{0}级，无法传送!", portal.PortalCfg.Get<int>("limitLv"));
                        UITips.ShowTips(msg);
                        LogMgr.LogWarning(msg + "(服务器8070协议下发结果)");
                        return;
                    }
                    else if (3 == resp.result)
                    {
                        string msg = string.Format("角色境界等级不足{0}级，无法传送!", portal.PortalCfg.Get<int>("dianfengLv"));
                        UITips.ShowTips(msg);
                        LogMgr.LogWarning(msg + "(服务器8070协议下发结果)");
                        return;
                    }
                }
            }
        }


        void SendMsg(TaskEnum enumType,MsgData msg, int id = 0)
        {
            EventParameter par = EventParameter.Get(msg);
            par.intParameter = (int)enumType;
            par.intParameter1 = id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_TaskUpdate, par);            
        }

        void SendCrownMainTaskMsg(int id)
        {
            EventParameter par = EventParameter.Get();
            par.intParameter = id;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_CROWN_MAINTASKMSG, par);
        }

        #endregion

        //int curAttackId = 0;

        /// <summary>
        /// 客户端之间推送，推送怪物死亡信息，记录死亡数量
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnEmbaDeadSum(GameEvent ge, EventParameter parameter)
        {
            MsgData_sStateChanged data = parameter.msgParameter as MsgData_sStateChanged;
            Debug.Log("怪物死亡推送。。。。" + parameter.intParameter);
            if (taskingList.ContainsKey(parameter.intParameter))
            {
                foreach (var item in taskingList[parameter.intParameter])
                {
                    if (questList.ContainsKey(item))
                    {
                        if (questList[item].goalsList.Length > 0)
                        {
                            for (int i = 0; i < questList[item].goalsList.Length; i++)
                            {
                                if (questList[item].goalsList[i].goalId == parameter.intParameter)
                                {
                                    questList[item].goalsList[i].curCount += 1;
                                    Debug.Log("任务id:  " + item);
                                    Debug.Log("死亡怪物唯一id:  " + data.RoleID);
                                    Debug.Log("死亡怪物id:  " + parameter.intParameter);
                                    Debug.Log("死亡数量:  " + questList[item].goalsList[i].curCount);
                                }
                            }
                        }
                        SendMsg(TaskEnum.update, questList[item], item);
                    }
                    else
                    {
                        LogMgr.UnityWarning("死亡怪物id  " + item);
                    }
                }
               
            }

        }

        /// <summary>
        /// 打开副本，进入副本
        /// </summary>
        /// <param name="npcId"></param>
        /// <param name="serverId"></param>
        public void OpenDungeon()
        {
            MainPanelMgr.Instance.ShowPanel("DungeonPanel", true, delegate ()
            {
                //这里可以加挑战按钮的引导动画
//                int layer = DungeonMgr.Instance.rideInfo.finishLayer + 1;
//                DungeonMgr.Instance.Send_CS_ChallHunLingXianYu(layer);
//                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_OPENTONPCMSG, par);
            });

        }


        /// <summary>
        /// 抛出lua 代码 测试用
        /// </summary>
        /// <param name="obj"></param>
        public void CheckOutWithLua(object obj)
        {
            Debug.Log(obj);
//            LogMgr.LogError("debug断点查看用");
        }


        public int GetFinishLinkId(string finishStr)
        {
            int index = finishStr.IndexOf('#');
            if (index == -1)
                return -1;
            string str = finishStr.Substring(index);
            str = str.Replace("#", "");
            //char[] chars = str.ToCharArray();
            string cur = GetFinishLinkStr(str);

            return Convert.ToInt32(cur);
        }

        public string GetUnFinishLink(string unfinishStr, string questGoals)
        {
            string[] list = questGoals.Split(',');
            string cur = GetFinishLinkStr(unfinishStr, list[0]);
            if (list.Length > 1)
                cur += " x" + list[1];
            return cur;
        }



        /// <summary>
        /// 已完成追踪文字   ----->    quest  finishLink 字段
        /// </summary>
        /// <param name="finishStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetFinishLinkStr(string finishStr, string name = "")
        {
            char[] chars = finishStr.ToCharArray();
            //            List<string> strs = new List<string>();
            string cur = "";
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '#')
                {
                    return cur;
                }
                if (chars[i] != '%' && chars[i] != 's' & chars[i] != '#')
                {
                    if (string.IsNullOrEmpty(cur))
                    {
                        cur = chars[i].ToString();
                    }
                    else
                    {
                        cur = cur + chars[i];
                    }
                }
                else if (i < chars.Length - 1 && chars[i] == '%' && chars[i + 1] == 's')
                {
                    cur = cur + name;
                }
            }

         //   Debug.Log("result:  " + cur);
            return cur;
        }

        public List<string> GetFinishLinkStrs(string str)
        {
            int index = 0;
            char[] chars = str.ToCharArray();
            List<string> strs = new List<string>();
            string cur = "";
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != '%' && chars[i] != 's' & chars[i] != '#')
                {
                    if (string.IsNullOrEmpty(cur))
                    {
                        cur = chars[i].ToString();
                    }
                    else
                    {
                        cur = cur + chars[i];
                    }
                    // 探查<color =#0000ff>声音来源</color><color =#0000ff>{0}</color>等等,113456

                    // 探查<color =#{0}>声音来源</color><color =#{1}>{2}</color>等等,ff3553,ff2222,113456
                }
                else
                {
                    if (i < chars.Length - 1 && chars[i] == '%' && chars[i + 1] == 's')
                    {
                        cur = string.Format("{0}", index);
                        index++;
                    }
                    if (!string.IsNullOrEmpty(cur))
                    {
                        strs.Add(cur);
                    }
                    cur = "";
                }
            }
            return strs;
        }
        public void Send_CS_DailyQuestStar(int taskId)
        {
            Debug.Log("日环任务升到5星");
            MsgData_cDailyQuestStar rsp = new MsgData_cDailyQuestStar();
            rsp.id = taskId;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DailyQuestStar, rsp);
        }

        /// <summary>
        /// 跑环领取全部额外奖励
        /// </summary>
        public void Send_CS_DailyGetReward()
        {
 //          LogMgr.LogError("领取全部奖励");
        }
        /// <summary>
        /// 请求完成跑环任务结果
        /// </summary>
        /// <param name="type"></param>
        public void Send_CS_DailyDraw()
        {
           // Debug.Log("完成任务 - - 请求任务抽奖");
            MsgData_cDailyDraw rsp = new MsgData_cDailyDraw();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DQDraw, rsp);
        }

        public void Send_CS_DailyDrawConfirm()
        {
          //  Debug.Log("完成任务 - - 请求任务抽奖");
            MsgData_cDailyDrawConfirm rsp = new MsgData_cDailyDrawConfirm();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DQDrawConfirm, rsp);
        }


        /// <summary>
        /// 一键完成跑环任务 -扫荡全部
        /// </summary>
        /// <param name="type"></param>
        public void Send_CS_DailySweepFinish(int type = 1)
        {
          //  LogMgr.LogError("扫荡全部 - -  一键完成");
            MsgData_cDailyQuestFinish rsp = new MsgData_cDailyQuestFinish();
            rsp.type = type;
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_DailyQuestFinish, rsp);
        }


        /// <summary>
        /// 讨伐领取全部额外奖励
        /// </summary>
        public void Send_CS_AgainstGetReward()
        {
         //   Debug.Log("领取全部奖励");

        }
        /// <summary>
        /// 请求完成讨伐任务结果
        /// </summary>
        /// <param name="type"></param>
        public void Send_CS_AgainstQuestResult()
        {
        //    Debug.Log("完成任务 - - 请求任务结果");
            MsgData_cAgainstQuestResult rsp = new MsgData_cAgainstQuestResult();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_AgainstQuestResult, rsp);
        }



        /// <summary>
        /// 一键完成讨伐任务 -扫荡全部
        /// </summary>
        /// <param name="type"></param>
        public void Send_CS_AgainstSweepFinish(int type = 1)
        {
         //   LogMgr.LogError("扫荡全部 - -  一键完成");
            MsgData_cAgainstQuestFinish rsp = new MsgData_cAgainstQuestFinish();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_AgainstQuestFinish, rsp);
        }


        /// <summary>
        /// 一键完成帮派任务 -扫荡全部
        /// </summary>
        /// <param name="type"></param>
        public void Send_CS_GuildQuestSweepFinish(int type = 1)
        {
            //   LogMgr.LogError("扫荡全部 - -  一键完成");
            MsgData_cUnionQuestFinish rsp = new MsgData_cUnionQuestFinish();
            CoreEntry.netMgr.send((Int16)NetMsgDef.C_GuildQuestSweep, rsp);
        }

        /// <summary>
        /// 返回帮派任务一键扫荡结果
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnGuildQuestFinish(GameEvent ge, EventParameter parameter)
        {
            //    Debug.Log("返回日环任务一键完成奖励信息");
            MsgData_sDailyQuestFinish resp = parameter.msgParameter as MsgData_sDailyQuestFinish;
            if (resp.result == 0)
            {
            //    LogMgr.LogError("奖励物品个数: " + resp.rewardList.Length);
            }
        }

        /// <summary>
        /// 日环任务升5星结果
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnDailyQuestStar(GameEvent ge, EventParameter parameter)
        {
   //         Debug.Log("日环任务升5星结果");
        }

        /// <summary>
        /// 返回日环任务一键完成奖励信息
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnDailyQuestFinish(GameEvent ge, EventParameter parameter)
        {
        //    Debug.Log("返回日环任务一键完成奖励信息");
            MsgData_sDailyQuestFinish resp = parameter.msgParameter as MsgData_sDailyQuestFinish;
            if(resp.result == 0)
            {
              //  LogMgr.LogError("奖励物品个数: " + resp.rewardList.Length);
            }
        }

        /// <summary>
        ///  返回日环任务结果
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnDailyQuestResult(GameEvent ge, EventParameter parameter)
        {
     //      Debug.Log("返回日环任务结果");
            //MsgData_sDailyQuestResult resp = parameter.msgParameter as MsgData_sDailyQuestResult;
    //        LogMgr.LogError("questList.count " + resp.questList.Length);
    //        LogMgr.LogError("rewardList.count " + resp.rewardList.Length);
        }

        /// <summary>
        /// 返回日环任务抽奖
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnDQDraw(GameEvent ge, EventParameter parameter)
        {
         //   Debug.Log("返回日环任务抽奖");
            //MsgData_sDailyDraw resp = parameter.msgParameter as MsgData_sDailyDraw;
        //    Debug.Log(resp.rewardIndex + "      "  + resp.doubleIndex);
        }

        /// <summary>
        /// 讨伐任务升5星结果
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnAgainstQuestStar(GameEvent ge, EventParameter parameter)
        {
            Debug.Log("讨伐任务升5星结果");
        }

        /// <summary>
        ///  返回讨伐任务一键完成奖励信息
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnAgainstQuestFinish(GameEvent ge, EventParameter parameter)
        {
            //MsgData_sAgainstQuestFinish resp = parameter.msgParameter as MsgData_sAgainstQuestFinish;
           // Debug.Log("返回讨伐任务一键完成奖励信息 resp.result: " +resp.result);
        }

        /// <summary>
        /// 返回讨伐任务结果
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnAgainstQuestResult(GameEvent ge, EventParameter parameter)
        {
         //   Debug.Log("返回讨伐任务结果");
        }

        /// <summary>
        ///  返回讨伐任务抽奖
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnAgainstQDraw(GameEvent ge, EventParameter parameter)
        {
            Debug.Log("返回讨伐任务抽奖");
        }

        /// <summary>
        /// 服务器通知：讨伐跳环领奖
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnGetAgainstQSkipReward(GameEvent ge, EventParameter parameter)
        {
            Debug.Log("服务器通知：讨伐跳环领奖");
        }

        /// <summary>
        /// 服务器返回跳环结果
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnAgainstQuestSkipResult(GameEvent ge, EventParameter parameter)
        {
            Debug.Log("服务器返回跳环结果");
        }

        /// <summary>
        ///  服务器通知是否抽奖，每环结束都要发
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnAgainstQDrawNotice(GameEvent ge, EventParameter parameter)
        {
            Debug.Log(" 服务器通知是否抽奖，每环结束都要发");
        }

        public Dictionary<int, MsgData_sQuestReward> sweepReward = new Dictionary<int, MsgData_sQuestReward>();
        /// <summary>
        /// 服务器返回 扫荡奖励列表  (经验任务，讨伐任务，帮派任务奖励)
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnQuestRewardResult(GameEvent ge, EventParameter parameter)
        {
            sweepReward.Clear();
            MsgData_sQuestRewardResult resp = parameter.msgParameter as MsgData_sQuestRewardResult;
            foreach (var item in resp.itemList)
            {
                sweepReward[(Int32)item.itemId] = item;
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_QuestRewardResult, parameter);
        }
    }

    [LuaCallCSharp]
[Hotfix]
    public class OtherTaskData
    {
        public int type;   //其他任务类型
        public int id;    //配置表id
        public int state;   //0 未领取或可领取   1.进行中   2已完成
        public OtherTaskData(OtherTaskType _type,int _id,int _state)
        {
            type = (int)_type;
            id = _id;
            state = _state;
        }
    }

[Hotfix]
    public class DailyActivyTaskData
    {
        public int type;   //日常任务类型 = 6
        public int id;    //dailyActivy配置表id
        public MsgData_sDailyActivyItem info;
        public int index; //排序索引

        public DailyActivyTaskData(OtherTaskType _type,int _id, MsgData_sDailyActivyItem _info,int _index)
        {
            type = (int)_type;
            id = _id;
            info = _info;
            index = _index;
        }
    }

    /// <summary>
    /// 任务类型枚举
    /// </summary>
    [LuaCallCSharp]
    public enum OtherTaskType
    {
        none = 0,
        Quest =1,    //主线
        Daily =2,   //经验
        Against = 3, //讨伐
        TongJi = 4,  //通缉任务
        Guild = 5, //帮派任务
        DailyActivyTask = 6,  //日常活动配置表类型
        Branch = 7,         //支线任务
    }

    [LuaCallCSharp]
    public enum OtherTaskState
    {
       add = 1,
       remove = 2,
    }

}

