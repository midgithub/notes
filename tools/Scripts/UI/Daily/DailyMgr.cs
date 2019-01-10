using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;


namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class DailyMgr
    {

        private static DailyMgr instance = null;
        public static DailyMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new DailyMgr();

                return instance;
            }
        }

        /// <summary>
        /// 活动状态列表 --前端读配置表
        /// </summary>
        public Dictionary<int, MsgData_sDailyActivyItem> list = new Dictionary<int, MsgData_sDailyActivyItem>();

        public Dictionary<int, MsgData_sDailyActivyItem> dailyTaskList = new Dictionary<int, MsgData_sDailyActivyItem>();

        /// <summary>
        /// 日常活动配置
        /// </summary>
        public static Dictionary<int, LuaTable> activyExcelConfig = null;

        public static Dictionary<int, LuaTable> levelExcelConfig = null;

        public DailyMgr()
        {
            activyExcelConfig = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_dailyActivy");
            levelExcelConfig = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_lvup");
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_SC_DailyActivy, OnDailyActivy);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CLEANUP_USER_DATA, GE_CLEANUP_USER_DATA);
        }
        public MsgData_sDailyActivyItem GetActivyData(int cfgId)
        {
            MsgData_sDailyActivyItem tt = null;
            if(list.ContainsKey(cfgId))
            {
                tt = list[cfgId];
            }
            return tt;
        }

        /// <summary>
        ///  切换账号清除数据
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void GE_CLEANUP_USER_DATA(GameEvent ge, EventParameter parameter)
        {
            list.Clear();
            dailyTaskList.Clear();
        }
        public LuaTable GetDailyConfig(int id)
        {
            LuaTable t;
            activyExcelConfig.TryGetValue(id, out t);
            return t;
        }

        public LuaTable GetExpConfig(int level)
        {
            LuaTable t;
            levelExcelConfig.TryGetValue(level, out t);
            return t;
        }

        public void OnDailyActivy(GameEvent ge, EventParameter parameter)
        {
            MsgData_sDailyActivyList resp = parameter.msgParameter as MsgData_sDailyActivyList;
            for (int i = 0; i < resp.count; i++)
            {
                MsgData_sDailyActivyItem tt = resp.list[i];
                if(tt.id > 0)
                {
                    list[tt.id] = tt;
                    LuaTable l;
                    if (activyExcelConfig.TryGetValue(tt.id,out l))
                    {
                        int ShowInMission = l.Get<int>("ShowInMission");
                        if(ShowInMission > 0)
                        {
                            //int openMaxNum = l.Get<int>("openMaxNum");
                            //int lessNum = GetLessDailyNum(tt.id);
                            //if (lessNum > 0)   
                           // {
                               dailyTaskList[tt.id] = tt;
                           // }else
                          //  {
                          //    if(dailyTaskList.ContainsKey(tt.id))
                          //      {
                           //         dailyTaskList.Remove(tt.id);   //主界面只显示没有打完的活动
                          //      }
                           // }
                        }
                    }
                }
            }
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DailyActivy, EventParameter.Get(resp));
        }

        public int GetLessDailyNum(int id)
        {
            LuaTable cfg = GetDailyConfig(id);

            int maxNum = cfg.Get<int>("openMaxNum");  // 最大次数上限
            int count = 0;
            int lessCount = 0;
            switch (id)
            {
                case 14:   //通缉
                    count = TongJiMgr.Instance.info.finishCount;
                    lessCount = maxNum + TongJiMgr.Instance.info.buyCount - count;
                    break;
                case 15:   //世界BOSS                   
                    lessCount = 1;
                    break;
                case 17:  //竞技场
                    if(ArenaMgr.Instance.MeArenaInfo != null)
                    {
                        count = ArenaMgr.Instance.MeArenaInfo.ChallengeTimes;
                    }
                    lessCount = maxNum - count;
                    break;
                case 39: //经验副本
                    count = ExpFuBenDataMgr.Instance.Info.time;
                    lessCount = maxNum + ExpFuBenDataMgr.Instance.Info.buyCount - count;
                    break;
                case 44:  //等级副本
                    lessCount = LevelFuBenDataMgr.Instance.enterNum;
                    break;
                case 57:  //寻宝

                    break;
                case 72:  //boss挑战（个人）

                    break;
                case 88:  //试炼秘境
                    lessCount = SecretDuplDataMgr.Instance.counts;
                    break;
                case 93:  //魔宠幻境
                    lessCount = DungeonMgr.Instance.fabaoInfo.resetNum;
                    break;
                case 107:  //宝塔秘境

                    break;
                case 108:   //经验任务
                    int questId2 = TaskMgr.Instance.GetCurTypeTaskId(2);
                    MsgData_sQuestInfo questData2 = TaskMgr.Instance.GetCurTaskMsg(questId2);
                    if (questData2 != null)
                    {
                        count = TaskMgr.GetLayer(questData2.flag);
                        lessCount = maxNum - count+1;
                    }else
                    {
                        lessCount = 0;
                    }

                    break;
                    
                case 119:  //讨伐任务
                    int questId3 = TaskMgr.Instance.GetCurTypeTaskId(3);
                    MsgData_sQuestInfo questData3 = TaskMgr.Instance.GetCurTaskMsg(questId3);
                    if (questData3 != null)
                    {
                        count = TaskMgr.GetLayer(questData3.flag);
                        lessCount = maxNum - count+1;
                    }else
                    {
                        lessCount = 0;
                    }
                    break;
                case 123:   //魔域地牢

                    break;
                case 131:  //资源副本
                    maxNum = PlayerData.Instance.ResLevelData.GetCfgAllNum();
                    count = PlayerData.Instance.ResLevelData.GetAllNum();
                    lessCount = maxNum - count;
                    break;
                case 154:  //跨服组队秘境

                    break;
                case 215:  //组队秘境
                    lessCount = SecretDuplDataMgr.Instance.tili;
                    break;
                case 234:   //首领BOSS
                    lessCount = 0;
                    break;
                case 235:    //恶灵猎行
                    lessCount = 0;
                    break;
                case 236:   //双倍经验
                    lessCount = 0;
                    break;
                case 237:   //每日答题
                    lessCount = 0;
                    break;
                default:
                    break;
            }
           // Debug.LogError("id   " +id +"   lesscount  " +lessCount);
            return lessCount;
        }


        public string GetLevelUpValue(float exp)
        {
            float lvl = 0;
            int myLevel = PlayerData.Instance.BaseAttr.Level;
#pragma warning disable 0219
            long myExp = PlayerData.Instance.BaseAttr.Exp;
            LuaTable cfg = GetExpConfig(myLevel);
            long _exp = cfg.Get<long>("exp");  // 等级经验配置
            //long lessExp = _exp - myExp;
            if(exp>0)
            {
                lvl = exp / _exp;
            }
            return lvl.ToString("0.00");
        }
    }

}

