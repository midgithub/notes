/**
* @file     : DizzyState.cs
* @brief    : 
* @details  : 限制 移动、技能
* @author   : 
* @date     : 2014-12-25
*/

using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
enum limitType
{
    Dizzy = 1,   // 眩晕
    Frozen = 2,  // 冰冻
    Move = 4,    // 移动
    Attack = 5,  // 普攻
    Sleep = 6,  // 睡觉
    Skill = 7,  // 技能
    Floating = 8, // 浮空
    Hitdown = 9,  // 倒地

    };

[Hotfix]
public class LimitParam : TimerStateParam
{
     public bool  bStopActor;
     public int   buffid;

    public bool TestCanDo(StateParameter param)
    {
        if (ACTOR_STATE.AS_RUN == param.state && (iState == (int)limitType.Move))
            return false;

        if (ACTOR_STATE.AS_ATTACK == param.state)
        {
            LuaTable desc = ConfigManager.Instance.Skill.GetSkillConfig(param.skillID);
            if (desc != null)
            {
                if ( desc.Get<int>("showtype") == 2) // 普攻
                {
                    if ((iState == (int)limitType.Attack))
                    {
                        return false;
                    }
                 
                }
                else // 技能
                {
                    if((iState == (int)limitType.Skill))
                    {
                        return false;
                    }                
                }
            }
        }
        return true;
    }
};

[Hotfix]
public class LimitState : TimerState
{
    

    public override void OnEnter(ActorObj actorBase)
    {
        m_state = ACTOR_STATE.AS_LIMIT;

        m_actor = actorBase;
        m_actor.StopAll(); // 冰冻状态需要停止动作

        //暂停动作
        if (m_actor.limitParam.bStopActor)
        {
            m_actor.StopAll(); // 冰冻状态需要停止动作
            m_isNonControl = true;
        }
        if(m_actor.limitParam.iState == (int)limitType.Sleep || m_actor.limitParam.iState == (int)limitType.Dizzy)
        {
            //播放动作
            string limitAction = "hit014";//m_actor.actorCreatureDisplayDesc.DizzyAction;
            if (limitAction.Length <= 0)
            {
                //没有，就使用默认的动作
                limitAction = m_actor.ModelConfig.Get<string>("san_stun");
            }
            m_actor.PlayAction(limitAction);                
            m_isNonControl = true;
        }
        else
        if (m_actor.limitParam.iState == (int)limitType.Floating)
        {
            InitFloatingPos();

            if (m_actor.IsHadAction("hit011"))     //m_actor.actorCreatureDisplayDesc.FloatingAction))
                m_actor.PlayAction("hit011"); //m_actor.actorCreatureDisplayDesc.FloatingAction);
            else
                m_actor.PlayAction("stand");

            m_isNonControl = true;
            floatingTime   = Time.time;
            floatingState  = 0;
            floatingkeepTime = m_actor.limitParam.keepTime * 0.001f - 0.5f;
        }
         if (m_actor.limitParam.iState == (int)limitType.Hitdown)
          {
                string hitdown = m_actor.ModelConfig.Get<string>("san_fly");
                if (m_actor.IsHadAction(hitdown))    
                    m_actor.PlayAction(hitdown);  
                else
                    m_actor.PlayAction("stand");

                m_isNonControl = true;
                floatingTime = Time.time;
                floatingState = 0;
                floatingkeepTime = m_actor.limitParam.keepTime * 0.001f - 0.5f;
            }
        else
        {
            m_actor.PlayAction("stand");
        }
        //状态硬直，不可转换其他状态

        base.OnEnter(actorBase);
        //CancelInvoke("LimitExitState");
        //Invoke("LimitExitState", m_actor.limitParam.keepTime);
    }

    void InitFloatingPos()
    {
        floatingPos[0] = new Vector3(m_actor.transform.position.x, m_actor.transform.position.y, m_actor.transform.position.z);
        floatingPos[1] = new Vector3(m_actor.transform.position.x, m_actor.transform.position.y + floatingDis, m_actor.transform.position.z);
        floatingPos[2] = new Vector3(m_actor.transform.position.x, m_actor.transform.position.y, m_actor.transform.position.z);

        hasInitFloatingPos = true;
    }

    public override bool CanChangeState(StateParameter stateParm)
    {
        bool rel = !m_isNonControl;

        //是否是特殊状态下改变的
        bool specialFlag = false;
        if (stateParm.state == ACTOR_STATE.AS_DEATH)
        {
            specialFlag = true;
        }

        if (stateParm.state == ACTOR_STATE.AS_BEHIT) // 受击可以打断睡眠
        {
            if (m_actor.limitParam.iState == (int)limitType.Sleep)
            {
                specialFlag = true;
            }
        }

        if (specialFlag && stateParm.state != ACTOR_STATE.AS_LIMIT) // 受击可以打断睡眠
        {
            m_actor.limitParam.iState = 0;
        }

        //如果设置了特殊标志位，也要返回成功
        if (specialFlag)
        {
            rel = true;
        }

        return rel;
    }

    public override void OnExit(StateParameter stateParm)
    {
        CancelInvoke("LimitExitState");
        m_isNonControl = false;
        if (m_actor.limitParam.iState == (int)limitType.Sleep)
        {
            m_actor.RemoveBuff(m_actor.limitParam.buffid); // 睡眠需要删除 睡眠Buff
        }

        if (floatingTime != 0)
        {
           // m_actor.transform.position = floatingPos[0];
            m_actor.PlayAction("stand");
            floatingTime = 0;
        }

        hasInitFloatingPos = false;
        //m_actor.limitParam.iState = 0;
    }

    protected override void PreExiting()
    {
        m_isNonControl = false;
        base.PreExiting();
    }

    //void LimitExitState()
    //{
    //    CancelInvoke("LimitExitState");
    //    m_isNonControl = false;
 
    //    StateParameter param = new StateParameter();
    //    param.state = ACTOR_STATE.AS_STAND;
    //    m_actor.RequestChangeState(param);

    //}

    public int floatingState = 0; // 0上升状态，1浮空状态，2下坠状态
    public Vector3[] floatingPos = new Vector3[3];
    public float floatingTime = 0;
    public float floatingkeepTime = 0;
    float floatingNum = 0;
    float floatingDis = 2.2f;
    bool hasInitFloatingPos = false;

    public override void FrameUpdate()
    {
        // 处理悬空的位移效果
        if (m_actor.limitParam.iState == (int)limitType.Floating)
        {
            if(floatingState==0) // 上升
            {
                //如果是其他状态，比如是冰冻状态进入LimitState的话，在状态中，进入悬空状态，floatpos没有被初始化
                //这里需要初始化一下
                if (hasInitFloatingPos == false)
                {
                    InitFloatingPos();
                }

                Vector3 pos = Vector3.Lerp(m_actor.transform.position, floatingPos[1], (Time.time - floatingTime) * 4);
                if (pos == floatingPos[1])
                {
                    floatingState = 1;
                }
                m_actor.SetPosition(pos);

            }
            else
            if(floatingState==1) // 悬浮
            {
                // 随机浮动一个值
                int intTime = (int)(Time.time*1000);
                if (intTime / 230 > floatingNum)
                {
                    floatingNum = intTime / 230;
                    if(floatingNum%2==0)
                        floatingPos[2] = m_actor.transform.position;
                    else
                    {
                        floatingPos[2].x = m_actor.transform.position.x + Random.Range(-0.05f, 0.05f);
                        floatingPos[2].y = m_actor.transform.position.y + Random.Range(-1.5f, 1.5f);
                        floatingPos[2].z = m_actor.transform.position.z + Random.Range(-0.05f, 0.05f);
                    }
                }

                Vector3 pos = Vector3.Lerp(m_actor.transform.position, floatingPos[2], Time.deltaTime/2f );
                m_actor.SetPosition(pos);

                if (Time.time - floatingTime > floatingkeepTime - 0.3f)
                {
                    floatingState = 2;
                    floatingPos[2] = m_actor.transform.position;
                }
            }
            else
            if (floatingState == 2) // 下落
            {
                Vector3 pos = Vector3.Lerp(floatingPos[2], floatingPos[0], (Time.time - (floatingTime + floatingkeepTime - 0.3f)) / 0.3f);
                m_actor.SetPosition (pos);
                if (pos == floatingPos[0])
                {
                    floatingState = 3;
                    if (m_actor.IsHadAction("hit012"))
                    {
                        m_actor.SetActionSpeed("hit012", 2f);
                        m_actor.PlayAction("hit012");
                    }
                }
            }
        }
    }

}

};  //end SG

