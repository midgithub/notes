using UnityEngine;
using System.Collections;
using XLua;

//状态接口，每个状态都必须从这里继承

namespace SG
{    
    [LuaCallCSharp]
    public enum ACTOR_STATE
    {
        AS_NONE,
        AS_ENTER,           //出场
        AS_STAND,           //待机
        AS_RUN,             //跑步
        AS_DEATH,           //死亡                        
        AS_ATTACK,          //攻击
        AS_BEHIT,           //被击  
        AS_DIZZY,           //眩晕     
        AS_LIMIT,           //限制操作
        AS_SHEEP,           //变形
        AS_RUN_TO_ATTACK,   //移动到攻击距离
        AS_FEAR,            //恐惧
        AS_AIMING, //瞄准
        AS_STONE, //石化
        AS_COLLECT,//采集
        AS_STORY_STAND,//剧情，待机
        AS_STORY_PLAY_ANIM,//剧情，播放动画
        AS_STORY_PLAY_ANIM_LOOP,//剧情，播放动画，循环
        AS_FIT,//准备合体

    };

    [LuaCallCSharp]
    //需要切换的状态参数
[Hotfix]
    public class StateParameter
    {
        public static string ANI_NAME_RUN = "run";
        public static string ANI_NAME_STAND = "stand";
        public static string ANI_NAME_MOUNT_RUN = "mount_run";
        public static string ANI_NAME_MOUNT_STAND = "mount_stand";
        public static string ANI_NAME_FLY_RUN = "fly_run";
        public static string ANI_NAME_FLY_STAND = "fly_stand";
        public static string ANI_NAME_MOUNT_BEHIT = "mount_hit001";
        public static string ANI_NAME_FLY_BEHIT = "fly_hit001";
        public static string ANI_NAME_DIE = "die001";

        public ACTOR_STATE state;
        public bool IsHitBack = false;
        
        //攻击，被击，技能ID
        public int skillID;
        public ActorObj AttackActor;    //技能释放actor
        public ActorObj HitActor;       //被攻击者

        //组合技能切换
        public bool isComposeSkill;
        public ComposeSkillDesc composeSkillDesc;

        //剧情，播放动画
        public string animInStory = string.Empty;  
        
        public StateParameter()
        {
            isComposeSkill = false; 
            skillID = 0;                       
        }                          
    }

    [LuaCallCSharp]
[Hotfix]
    public class IActorState : MonoBehaviour {    
    //状态硬直
    protected bool m_isNonControl = false;
    
    //硬直保护状态(保护被连续的被击)
    protected bool m_isNonControlProtect = false;    

    //霸体状态(保护当前技能的释放不被打断)
    protected bool m_isEndure = false;   

    public ACTOR_STATE m_state;

    protected ActorObj m_actor = null;

    protected bool m_isDeathEnd = true;

    //当前的参数
    public StateParameter CurParam { get; set; }

    //进入
    public virtual void OnEnter(ActorObj actor) { }
    public virtual void FrameUpdate() { }
    public virtual void OnExit(StateParameter stateParm) { }

    //位移请求
    public virtual bool CanMove() { return false; }

    public virtual bool CanShowSelf() { return true; }

    //能否切换状态
    public virtual bool CanChangeState(StateParameter stateParm) 
    {                
        return true; 
    }                        
    
    public void Reset()
    {
        m_isNonControl = false;                    
        m_isEndure = false;
    }
    
    public bool IsNonControl
    {
        get { return m_isNonControl;}
    }

    public bool IsEndure()
    {
        return m_isEndure;
    }

    public bool IsDeathEnd()
    {
        return m_isDeathEnd;
    }
    }

}

