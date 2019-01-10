using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using XLua;

namespace SG
{

    enum BuffCategory
    {
        EBBT_NEUTRAL = 0,     //
        EBBT_POSITIVE = 1,    //
        EBBT_NEGATIVE = 2,    //
    }

    enum BuffPosEnum
    {
        BUFF_E_TOP = 1,   //头顶
        BUFF_E_SPINE = 2, //胸部
        BUFF_E_ROOT = 3 , //root点
    }


    [LuaCallCSharp]
[Hotfix]
    public class BuffMgr : MonoBehaviour
    {

        public Dictionary<long , ActorObj> buffList = new Dictionary<long, ActorObj>();


        public void Init( )
        {
            buffList.Clear();
        }

        public void AddBuff(BuffData buffData,ActorObj TargetObj)
        {
            if(TargetObj != null)
            {
                //区分目标
                LuaTable desc = ConfigManager.Instance.Skill.GetBuffConfig(buffData.buffType);
                if(desc != null)
                {
              
                    TargetObj.Addbuff(buffData, TargetObj);

                    //  buffList.Add(buffData.BufferInstanceID, TargetObj);

                    buffList[buffData.BufferInstanceID] = TargetObj;
                    if(TargetObj.ServerID == CoreEntry.gActorMgr.MainPlayer.ServerID)
                    {
                        EventParameter msg = new EventParameter();
                        msg.longParameter = buffData.buffType;
                       // Debug.LogError("添加BUFF  " + buffData.BufferInstanceID + "   " + buffData.buffType);
                        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_ADDBUFF, msg);
                    }
                }
            }
        }


        public void RemoveBuff(long buffId, ActorObj TargetObj)
        {      
            if (TargetObj != null)
            {
                buffList.Remove(buffId);
                TargetObj.RemoveBuff(buffId);
            }

        }


        public void UpdateBuff(BuffData buff )
        {
            ActorObj actor;
            buffList.TryGetValue(buff.BufferInstanceID, out actor);

            if(actor != null)
            {
                actor.UpdateBuff(buff);

            }         

        }

 
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

