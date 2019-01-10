using UnityEngine;
using System.Collections;
using XLua;


namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class NpcMgr
    {
        private static NpcMgr instance = null;
        public static NpcMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new NpcMgr();

                return instance;
            }
        }

        /// <summary>
        /// 打开NPC
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="objName"></param>
        /// <param name="args"></param>
        public void OpenNpcTk(int npcId)
        {
            CoreEntry.gActorMgr.MainPlayer.StopMove(true);
            PanelBase npcBase = MainPanelMgr.Instance.GetPanel("NPCCommonTkPanel");
            if(npcBase != null)
            {
                if(npcBase.IsShown)  
                {
                    Debug.LogError("isShow");
                    return;
                }
            }

            MainPanelMgr.Instance.ShowPanel("NPCCommonTkPanel", true, delegate ()
            {
                ActorObj obj = CoreEntry.gActorMgr.GetActorByConfigID(npcId);
                EventParameter par = EventParameter.Get();
                par.intParameter = npcId;
                par.objParameter = obj;
                //   par.objParameter1 = args[1];
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_OPENTONPCMSG, par);
            });

        }

        /// <summary>
        /// 判断NPC距离是否在点击范围之内，tt NPC位置 ，
        /// r - 区域半径
        /// </summary>
        /// <param name="tt"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool bInArea(Vector3 tt, float r = 0)
        {
            Vector3 my = TaskMgr.Instance.MyPlayPos();
            float rLength = (r * r) + (r * r);
            float length = ((tt.x - my.x) * (tt.x - my.x)) + ((tt.z - my.z) * (tt.z - my.z));
            if (rLength >= length)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 玩家状态
        /// </summary>
        public static int PlayerState
        {
            get
            {
                return (int)CoreEntry.gActorMgr.MainPlayer.curActorState;
            }
        }

    }
}

