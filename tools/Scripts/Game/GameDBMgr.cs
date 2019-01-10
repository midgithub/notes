using XLua;
﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Runtime.InteropServices;

/*
    游戏静态数据存放
*/
namespace SG
{

    //actortype
    [System.Serializable]

    //被击类型
    public enum BehitType
    {
        BT_NONE = 0,
        BT_NORMAL,              //普通受击，可带位移(hit001)，程序位移
        BT_HITBACK,             //大退受击(hit002)(必须有位移，美术位移)
        BT_HITDOWN,             //击倒受击(hit003)(可以有位移，美术位移)    
        BT_HITSKY,              //悬空受击(hit005_start, hit005_end)(必须没有位移)            
        BT_MAX,
    };


    ////技能伤害信息
    //[System.Serializable]
    //public class SkillDesc
    //{
    //    //基本信息    
    //    public int id;                  //技能ID
    //    public string attackAction;     //技能动作    
    //    public string attackActionEfx;  //技能动作特效        
    //    public float cd;                //cd时间          
    //    public bool canMove;            //释放技能中能否移动     
    //    public int attackHp;            //技能伤害    
    //    public float keepTime;          //技能持续时间(默认是动作时间)    
    //    public float distance;          //攻击距离        

    //    //被击信息
    //    public float moveTime;          //小退时间
    //    public float moveScale;         //小退位移系数      

    //    public float backScale;         //击退位移系数          
    //    public float flyScale;          //击飞位移系数      
    //    public float skyTime;           //悬空时间   

    //    public int weight;              //权重

    //    //死亡后退力
    //    public float backForceScale;    //后退力比值                
    //    public float upForceScale;      //向上力比值                

    //    //技能霸体
    //    public bool hadEndure;
    //    public float endureStart;       //霸体开始时间
    //    public float endureKeep;        //霸体持续时间

    //    //伤害列表
    //    public List<OneDamageInfo> damageList;
    //};

    //技能元素数据接口
    [System.Serializable]
[Hotfix]
    public class ISkillCellData
    {
        public string prefabPath;       //prefab路径
        public int stage = 1;              //数据是在技能哪个阶段，0：吟唱，1：释放。默认释放阶段

    }

    //位移元素
    [System.Serializable]
[Hotfix]
    public class MovePosAttackDesc : ISkillCellData
    {
        public bool isCarryOffTarget = false;   //是否带走目标        
        public bool isStopForTarget = false;    //是否碰到目标停止  

        public List<ActorType> resetAimActorTypeList = null; //穿透目标对象

        public string efxPrefab = "";        //拖尾特效    

        public float moveDistance = 0.0f;

    };

    //子弹元素
    [System.Serializable]
[Hotfix]
    public class FireFlyAttackDesc : ISkillCellData
    {
        public float flySpeed;          //飞行的速度   
        public float flyTime;           //飞行的时间   

        public string firePos;          //发射点
        public float fireTime;          //发射时间    

        public float neerDistance;      //近点距离

        public List<ActorType> aimActorTypeList;  //目标类型   

        public string bulletPrefab;         //子弹资源    
        public int bulletNum;               //子弹数量

        public float fAngle;                //子弹间的夹角

        public bool bThroughFlag;           //是否穿透

        public bool bOnGound;               //是否在地面

        public float projectile;            //是否抛物线子弹

        public int TanTanLeCount;         //弹弹乐次数

        public float TanTanLeDis;           //弹弹乐距离

        public float dizzyTime;             //弹弹乐眩晕时间

        public bool bTakeAwayTarget;  //是否带走目标
        public bool disappearWhenTouchWall = true; //碰到墙是否消失
        public bool bCanNotMoveWhenTakenAway; //带走的时候能不能移动

        public bool bTraceTarget; //跟踪目标

        public bool bAttachToSkill;

        public bool bAttackMoreThanOnce = false;

        public bool bEndTheSkillWhenDisappear = false;

        public float displayWidth = 1; //显示伤害宽度
        public float displayLength = 8;//显示伤害长度

        public CameraShakeCellDesc cameraShakeDesc = null;      //相机震动元素    

    };

    //碰撞伤害元素
    [System.Serializable]
[Hotfix]
    public class CollideDamageDesc : ISkillCellData
    {
        public float endCarryOffTargetTime;           //结束带走时间
        public List<int> aimActorTypeList;  //伤害对象

        //public string prefabPath;
    }

    //顿帧元素,绑定到伤害元素一起
    [System.Serializable]
[Hotfix]
    public class FrameStopCellDesc : ISkillCellData
    {
        public float frameStopTime;         //顿帧持续时间

        //public string prefabPath;
    }

    //相机震动元素
    [System.Serializable]
[Hotfix]
    public class CameraShakeCellDesc : ISkillCellData
    {
        //public string shakePrefabPath;  //相机震动
        public float playTime;          //播放时间
        public bool activeWhenHit = false;
        public bool bBlur = false;
        public float fBlurTime = 0.0f;
    }

    //相机拉近拉远元素
    [System.Serializable]
[Hotfix]
    public class CameraZoomCellDesc : ISkillCellData
    {
        public float playTime = 0f;
        public float distance = 0f;
        public float blurStart = 0f;
        public float blurDuration = 0f;
    }

    //每条曲线数据
    [System.Serializable]
[Hotfix]
    public class OneCurveDamageInfo
    {
        public float offsetAngle;
        public float length;

        public float hitTime;
        public float damageWidth;
        public float damageLength;
    }

    //曲线伤害元素
    [System.Serializable]
[Hotfix]
    public class CurveDamageCellDesc : ISkillCellData
    {
        public List<OneCurveDamageInfo> curveDamageInfoList;        //曲线信息
        public List<ActorType> aimActorTypeList;       //目标对象   
                                                       //public string                       prefabPath;             //对应的prefab  
    }

    //无敌元素
    [System.Serializable]
[Hotfix]
    public class ImmuneCellDesc : ISkillCellData
    {
        public float startTime;
        public float keepTime;

        //public string prefabPath;             //对应的prefab  
    }

    //打断吟唱元素
[Hotfix]
    public class BreakPrepareDesc : ISkillCellData
    {
        public int breakDamageHP;              //被打断需要的伤害    
        public string behitAction = "";            //打断后播放的被击动作

        //public string UIPrefabPath;           //UI元素  
        public bool isUsedUI = false;
    }

    //boss死从天降元素
[Hotfix]
    public class BossDeathFromSkyDesc : ISkillCellData
    {
        public float riseTime;          //拉高位移时间
        public float prepareDownTime;   //下来预警时间
        public float downTime;          //落地时间                

        public string efxPrefab;        //地面特效  
    }

    public class SummonCellDesc : ISkillCellData  // add by yuxj
    {
        public int npcPosType = 0; // 召唤NPC的位置类型
        public int npcID;  // ID
        public int npcCount = 1; // 召唤数量
        public float npcDis = 3f;
        public int killBefore = 0; // 重复召唤时，删除原来召唤的
        public int buffID = 0;
        public float lifeTime = float.MaxValue;
        public bool deadthToKill = false;
        public int summonType = 0; // 0.属性从表中读取，1.属性从召唤者继承*表里设置的属性百分比，2.属性从召唤者继承*表里设置的属性百分比 模型外观变成召唤者
        public bool followMe = false;

    };

    /// <summary>
    /// 隐身元素
    /// </summary>
[Hotfix]
    public class StealthCellDesc : ISkillCellData
    {
        public float startTime = 0.0f;//开始时间
        public float durationTime = 0.0f;//持续时间
    };

    [System.Serializable]
[Hotfix]
    public class RotationDesc
    {
        public float BeginRadius = 1f;
        public float EndRadius = 2f;
        public float Duration = 2f;
        public float Speed = 1f;
    }

    [System.Serializable]
[Hotfix]
    public class PointDesc
    {
        public Vector3 Point;
        public bool BackToActor = false;
        public bool EndSkill = false;
        public bool SkipWhenTouch = false;

    }
    //位置元素
    [System.Serializable]
[Hotfix]
    public class PositionCellDesc : ISkillCellData
    {
        public bool AttachToActor = false;
        public bool SyncRotation = false;
        public bool SyncOwnerPosition = false;
        public bool LookAtActor = true;
        public bool Static = true;
        public float Speed = 0f;
        public float DelayMovingTime = 0f;
        /// <summary>
        /// 离attach物体的偏移值
        /// </summary>
        public Vector3 AttachOffset = Vector3.zero;
        public int BehaviorSkillId = -1;
        public List<PointDesc> PointList = new List<PointDesc>();
        public List<RotationDesc> RotationList = new List<RotationDesc>();
    };

    //buff工厂元素
    [System.Serializable]
[Hotfix]
    public class BuffFactoryCellDesc : ISkillCellData
    {
        public float Radius = 10f;
        public float Delay = 0;
    };

    //力工厂元素
    [System.Serializable]
[Hotfix]
    public class ForceFactoryCellDesc : ISkillCellData
    {
        public float Radius = 10f;
        public float Speed = 10f;
        public float Delay = 0;
        public bool IsAoe = true;
        public float LastTime = 1f;
    };

    [System.Serializable]
[Hotfix]
    public class SubSkillDesc
    {
        public int SkillId = 0;
        public float DelayTime = 0;
    };


    //复合技能
    [System.Serializable]
[Hotfix]
    public class CompositedSkillCellDesc : ISkillCellData
    {
        public List<SubSkillDesc> SkillList = new List<SubSkillDesc>();
    };

    [System.Serializable]
    public enum DamageTypeID
    {
        DTID_FUN = 1,       //扇形，圆形伤害
        DTID_RECT,          //矩形伤害
    };

    //扇形，圆形伤害
    [System.Serializable]
    public struct FunDamageNode
    {
        public float angle;
        public float radius;
        public float offDistance;           //扇形原点偏移角色的距离   
        public float offDistanceX;
    };

    [System.Serializable]
    public struct RectDamageNode
    {
        public float width;
        public float length;
        public float offDistance;           //矩形计算点偏移角色的距离
        public float offDistanceX;
    };



    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    [System.Serializable]
    public struct DamageNodeUnion
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public FunDamageNode funDamage;

        [System.Runtime.InteropServices.FieldOffset(0)]
        public RectDamageNode rectDamage;
    }

[Hotfix]
    public class DizzyParamDesc
    {
        public float keepTime;
    };

    //伤害信息
    [System.Serializable]
[Hotfix]
    public class OneDamageInfo : ISkillCellData
    {
        public DamageTypeID type;
        public List<ActorType> aimActorTypeList;
        public float hitTime;
        //public string prefabPath;

        //伤害间隔，持续时间
        public bool isRepeatedly = false;
        public float damageDiff = 0;
        public int keepTime = 0;

        //纠正技能权重
        public int resetSkillWeight;
        public bool isNotUseCurveMove = false;  //不使用美术位移 

        //眩晕效果            
        public DizzyParamDesc dizzyParamDesc = null;

        public FrameStopCellDesc frameStopDesc = null;          //顿帧元素
        public CameraShakeCellDesc cameraShakeDesc = null;      //相机震动元素    

        public DamageNodeUnion damageNode;

        public bool m_bUseRemain = true;

        //预警效果
        public string efxWarning = "";
    };

    //播放动作，特效，声音元素
[Hotfix]
    public class ActionCellDesc : ISkillCellData
    {
        public DamageTypeID type;
        public float playTime = 0;  //播放时间
        public float delayTime = 0; //延迟时间

        public string name = "";
        public string efx = "";
        public float setStartTime = 0;   //动作开始时间
        public float speed = 1;
        public int sound1 = 0;
        public int sound2 = 0;

        public DamageNodeUnion damageNode;

        //预警效果
        public string efxWarning = "";

        public bool shouldAttachToOwner = true;//是否附加到Owner


    }

    //吟唱数据
[Hotfix]
    public class PrepareStage
    {
        public bool open = false;
        public float keepTime = 0;
        public bool isLookAtTarget = false;
    }

    //[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    //[System.Serializable]
    //public struct AttackTemplateUnion
    //{
    //    //普通攻击模版数据
    //    //[System.Runtime.InteropServices.FieldOffset(0)]
    //    public NormalAttackDesc normalAttackData;

    //    //[System.Runtime.InteropServices.FieldOffset(0)]
    //    public RushAttackDesc rushAttackData;

    //    //[System.Runtime.InteropServices.FieldOffset(0)]
    //    public MovePosAttackDesc movePosAttackDesc;

    //    //[System.Runtime.InteropServices.FieldOffset(0)]
    //    public FocusAttackDesc focusAttackData;

    //    //[System.Runtime.InteropServices.FieldOffset(0)]
    //    public FireFlyAttackDesc fireFlyAttackData;
    //}

    //模版数据表，设定好的数据
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    [System.Serializable]
[Hotfix]
    public class SkillClassDisplayDesc
    {
        public int skillID;             //技能ID
        public string prefabPath;       //模版表现prefab      
        public float neerDistance;      //最近攻击距离
        public int classID;             //模版ID
                                        //  public float distance;          //攻击距离

        //各种技能元素
        //public List<OneDamageInfo> damageList;     //普通伤害计算方式         

        public MovePosAttackDesc movePosAttackDesc = null;     //位移元素
                                                               //public FireFlyAttackDesc fireFlyAttackData = null;     //子弹发射元素
                                                               //public CollideDamageDesc collideDamageDesc = null;      //碰撞伤害元素
                                                               ////public FrameStopCellDesc frameStopDesc = null;          //顿帧元素
                                                               ////public CameraShakeCellDesc cameraShakeDesc = null;      //相机震动元素
                                                               //public CurveDamageCellDesc curveDamageCellDesc = null;  //曲线伤害元素
                                                               //public ImmuneCellDesc immuneCellDesc = null;            //无敌元素
                                                               //public AttackBreakDesc attackBreakDesc = null;          //蓄力技能被打断条件
                                                               //吟唱
        public PrepareStage prepareStage = null;

        //吟唱技能元素数据
        public List<ISkillCellData> prepareStageDataList = null;

        //释放技能元素数据
        public List<ISkillCellData> castStageDataList = null;
    }


    //种怪表
[Hotfix]
    public class SpawnMonster
    {
        public int mapID; //场景ID
        public int resID;      //怪物ID
        public Vector3 pos;    //位置      
        public Vector3 angle;    //旋转角度      
                                 //public Vector3 scale;    //缩放
        public int waveSequence;    //第几波
        public bool bIsPatrol;      //是否是巡逻怪
        public Vector3[] patrolPathList;    //巡逻怪路点

        public string strAssertPath;    //assert位置  
        public bool enable;
        public string strEnterAction;   // 入场动作     
    };

[Hotfix]
    public class SpawnPlayer
    {
        public Vector3 pos;    //位置      
        public Vector3 angle;    //旋转角度      

    };


    public struct BossSkill
    {
        public int iSkillid;
        public float fLastTime;
    }

    //动作音效表
[Hotfix]
    public class OneVoiceInfo
    {
        public float playTime = 0;
        public string assetPath = "";
        public string resourcePath = "";
        public UnityEngine.Object voiceObj = null;
    }

[Hotfix]
    public class ActionVoiceInfo
    {
        public string actionName;
        public List<OneVoiceInfo> voiceInfoList = null;
    }

[Hotfix]
    public class ActionVoiceDesc
    {
        public int resID;
        public List<ActionVoiceInfo> actionVoiceInfoList = null;

        public ActionVoiceDesc DeepCopy()
        {
            ActionVoiceDesc m_actionVoiceDesc = new ActionVoiceDesc();

            m_actionVoiceDesc.resID = resID;
            m_actionVoiceDesc.actionVoiceInfoList = new List<ActionVoiceInfo>();

            for (int i = 0; i < actionVoiceInfoList.Count; ++i)
            {
                ActionVoiceInfo actionInfo = new ActionVoiceInfo();
                actionInfo.actionName = actionVoiceInfoList[i].actionName;
                actionInfo.voiceInfoList = new List<OneVoiceInfo>();

                for (int j = 0; j < actionVoiceInfoList[i].voiceInfoList.Count; ++j)
                {
                    OneVoiceInfo voiceInfo = new OneVoiceInfo();
                    voiceInfo.playTime = actionVoiceInfoList[i].voiceInfoList[j].playTime;
                    voiceInfo.assetPath = actionVoiceInfoList[i].voiceInfoList[j].assetPath;
                    voiceInfo.resourcePath = actionVoiceInfoList[i].voiceInfoList[j].resourcePath;

                    //                 voiceInfo.voiceObj = AssetDatabase.LoadAssetAtPath(
                    //                         actionVoiceInfoList[i].voiceInfoList[j].assetPath, typeof(AudioClip));

                    actionInfo.voiceInfoList.Add(voiceInfo);
                }

                m_actionVoiceDesc.actionVoiceInfoList.Add(actionInfo);
            }

            return m_actionVoiceDesc;
        }
    }

    //生物表
[Hotfix]
    public class CreatureDesc
    {
        public int resID;
        public string strName;
        public int ctype;        //player,怪物,boss,npc
        public int hp;         //生命
        public int speed;      //移动速度             
        public string standAction;      //待机动作
        public string runAction;      //待机动作  
        public string dieAction;      //死亡动作  
        public string walkAction;      //行走动作  
        public int defaultSkillID;      //默认技能ID

        //被击后，基本位移
        public float baseMoveDistance;

        public float baseBackDistance;
        public float baseFlyDistance;

        public List<BossSkill> BossSkillList = new List<BossSkill>();   //boss技能

        public string strPrefabPath;    //prefab位置  
        public string behitefxPrefab;   //被击特效    

        public CreatureDesc()
        {
            baseMoveDistance = 0;
            baseBackDistance = 0;
            baseFlyDistance = 0;
            BossSkillList.Clear();

            behitefxPrefab = "";
        }
    };

    //生物动画层级设定    
[Hotfix]
    public class ActionLevel
    {
        public string strAction;
        public char level;
        public string strSound;
        public string strSound2;
    };

[Hotfix]
    public class ActionLevelDesc
    {
        public int resID;
        public string strPrefabPath;    //prefab位置        

        public int num;
        public ActionLevel[] actionLevel;
    };

    //生物动作相关表
[Hotfix]
    public class CreatureActionDesc
    {
        public int resID;       //生物ID

        public List<string> beHitActions;   //受击动作
        public string hitBackAction;
        public string hitDownAction;
        public string hitSkyStartAction;
        public string hitSkyEndAction;
        public string hitFlyAction;
    };


    //组合技能表
[Hotfix]
    public class ComposeSkillDesc
    {
        public int srcSkillID;
        public int dstSkillID;
        public int composeSkillID;
        // public int endSkillID;

        public int ratio;           //执行的概率
        public int order;           //执行的顺序
        public int defaultorder;    //默认的执行顺序
        public bool isOrder;

        public float changeTime;    //动作切换时间点
        public float startTime;     //动作开始时间点
    };

    //技能切换表
    public enum ChangeSkillType
    {
        CST_DISTANCE = 1,       //攻击距离
    };

[Hotfix]
    public class ChangeSkillDesc
    {
        public int srcSkillID;
        public int dstSkillID;
        public int changeType;
    };

    //场景区域表
[Hotfix]
    public class OneZoneData
    {
        public int zoneID;
        public int doorID;
        public Vector3 startPos;
        public Vector3 endPos;
    }

[Hotfix]
    public class MapZoneDesc
    {
        public int mapID;
        public List<OneZoneData> zoneList;
    }

[Hotfix]
    public class WaveTriggerDesc
    {
        public int mapID;
        public int zoneID;
        public int waveSeq;
        public float turbulenceTime;
        public int trigType;
        public int loopType;
        public int loopMonsterID;
        public float loopMonsterIntervalTime;
        public float loopTimeRange;
        public float arg1;
        public float arg2;
        public float arg3;
        public float arg4;
        public float arg5;
        public float arg6;
    }

    //伤害表现类型
    public enum DamageDisplayType
    {
        DDT_NORMAL = 1,         //正常伤害
        DDT_DOUBLE,             //暴击伤害    
        DDT_IMMUNE,             //免疫
        DDT_MISS,               //miss    
        DDT_SUCK,               //吸血
        DDT_BONCE,              //反伤
        DDT_BUFF,               //Buff伤害
    };

    //被击效果参数


    //伤害参数（收到技能，BUFF等任何伤害传送给被击对象的参数）
[Hotfix]
    public class BehitParam
    {
        //伤害数值
        public int hp;               //伤害    
        public DamageDisplayType displayType;

        //伤害参数（被击效果修正）
        public DamageParam damgageInfo;
        public void clean()
        {
            
            if (damgageInfo != null)
            {
                damgageInfo.clean();
                damgageInfo = null;
            }
            
        }
    };

    public struct OneAttachAnim
    {
        public string prefab;       //挂接prefab
        public string hangPoint;    //挂点            
        public string action;       //对应播放的动作 
    };

    //附加动作表
[Hotfix]
    public class CreatureAttachAnimDesc
    {
        public int resID;       //角色ID
        public string action;   //播放的动作

        public List<OneAttachAnim> attachList;
    };

    //硬直设置表
    public struct NonControlSet
    {
        public int keepTime;    //硬直时间
        public bool canReset;      //时间能否重置
    };

    //技能受击表现表
[Hotfix]
    public class SkillBehitDisplayDesc
    {
        public short skillWeight;     //技能力度值
        public short creatureBody;    //生物体形值
        public bool isNonControl;   //是否硬直            
        public BehitType behitType; //被击类型         

        public NonControlSet nonControlSet;    //硬直设置
        public List<string> actionList;         //受击动作表现
    };

    //浮空技能描述表
[Hotfix]
    public class SkySkillDesc
    {
        public int skillID;
        public float originV;
        public float angle;
    }

    //地图对象类型
    public enum EntityConfigType
    {
        ECT_MONSTER = 0,    //怪物
        ECT_NPC,            //NPC
        ECT_COLLECTION,     //采集点
        ECT_PORTAL,         //传送门
        ECT_BIRTH,          //出生点
        ECT_WAYPOINT,       //路点
    };
    //地图对象配置表
[Hotfix]
    public class SceneEntitySet
    {
        public EntityConfigType type;
        public List<SceneEntityConfig> entityList;
    };
[Hotfix]
    public class SceneEntityConfig
    {
        public int configID;
        public int modelID;
        public int waveSeq;
        public Vector3 position;
        public int index;
        public List<int> neighbours;
    };

[Hotfix]
    public class GameDBMgr : MonoBehaviour
    {
        private static GameDBMgr m_Instance = null;


        //种怪表
        private Dictionary<int, List<SpawnMonster>> m_spawnMonsterMap = new Dictionary<int, List<SpawnMonster>>();

        private Dictionary<int, List<SceneEntitySet>> m_EnityConfigMap = new Dictionary<int, List<SceneEntitySet>>();

        //技能模版表    
        private Dictionary<int, SkillClassDisplayDesc> m_SkillClassDisplayDescMap;


        //组合技能表
        private List<ComposeSkillDesc> m_composeSkillDescList;

        //技能切换表
        private List<ChangeSkillDesc> m_changeSkillDescList;

#pragma warning disable 0649
        //地图区域表
        private Dictionary<int, MapZoneDesc> m_mapZoneMap;


        //技能受击表现表    
        private Dictionary<int, SkillBehitDisplayDesc> m_skillBehitDisplayMap;



        public static int HashCodeForShort2(short value1, short value2, short factor)
        {
            return value1 * factor + value2;
        }

        public static GameDBMgr instance
        {
            get { return m_Instance; }
        }

        void Awake()
        {

            loadDatas(0);

        }

        public void loadDatas(float timeScal = 1.0f)
        {
            StartCoroutine(loadFiles(timeScal));
        }

        IEnumerator loadFiles(float timeScal = 1.0f)
        {


            LogMgr.UnityLog("------------------------------------加载XML    开始-------------------------------------------------------------------------------timeScal:" + timeScal);

            CoreEntry.gTimeMgr.TimeScale = 1;
            // yield return new WaitForSeconds(1f * timeScal);

            float beginTime = RealTime.time;

            m_Instance = this;

            XmlData xmlData = new XmlData();
            XmlNodeList nodeList;// = xmlData.ReadAll();

            string filePath = "";


            //读取技能模版表
            m_SkillClassDisplayDescMap = new Dictionary<int, SkillClassDisplayDesc>();
            filePath = @"Data/skill/SkillClassDisplayDesc";
            xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                LogMgr.UnityLog("open xml faild! " + filePath);
                yield return true;
            }
            //   yield return new WaitForSeconds(delayTime); 

            nodeList = xmlData.ReadAll();


            foreach (XmlElement node in nodeList)
            {
                SkillClassDisplayDesc data = new SkillClassDisplayDesc();

                int skillID = int.Parse(node.GetAttribute("skillID"));
                string prefabPath = node.GetAttribute("prefabPath");

                data.skillID = skillID;
                data.prefabPath = prefabPath;
                data.neerDistance = 0;
                // data.distance = 0;

                //if (node.HasAttribute("distance"))
                //{
                //    data.distance = float.Parse(node.GetAttribute("distance"));
                //}

                data.prepareStageDataList = new List<ISkillCellData>();
                data.castStageDataList = new List<ISkillCellData>();

                //伤害计算    
                //data.damageList = new List<OneDamageInfo>();
                foreach (XmlElement attack in node.ChildNodes)
                {
                    if (attack.Name != "damage")
                    {
                        continue;
                    }

                    OneDamageInfo damgeOne = new OneDamageInfo();
                    damgeOne.aimActorTypeList = new List<ActorType>();

                    damgeOne.type = (DamageTypeID)int.Parse(attack.GetAttribute("type"));
                    damgeOne.hitTime = float.Parse(attack.GetAttribute("hitTime"));
                    damgeOne.prefabPath = attack.GetAttribute("prefabPath");


                    if (attack.HasAttribute("bUseRemain"))
                    {

                        {
                            int nTemp = int.Parse(attack.GetAttribute("bUseRemain"));
                            damgeOne.m_bUseRemain = nTemp != 0 ? true : false;
                        }
                    }




                    //float curDistance = 0;

                    if (attack.HasAttribute("stage"))
                    {
                        damgeOne.stage = int.Parse(attack.GetAttribute("stage"));
                    }

                    if (damgeOne.stage == 0)
                    {
                        data.prepareStageDataList.Add(damgeOne);
                    }
                    else if (damgeOne.stage == 1)
                    {
                        data.castStageDataList.Add(damgeOne);
                    }

                    //child
                    foreach (XmlElement damageChild in attack.ChildNodes)
                    {
                        if (damageChild.Name == "repeate")
                        {
                            damgeOne.isRepeatedly = damageChild.GetAttribute("value").Equals("true");
                            damgeOne.damageDiff = float.Parse(damageChild.GetAttribute("diff"));
                            damgeOne.keepTime = int.Parse(damageChild.GetAttribute("keep"));
                        }

                        else if (damageChild.Name == "fun" && damgeOne.type == DamageTypeID.DTID_FUN)
                        {
                            damgeOne.damageNode.funDamage = new FunDamageNode();

                            damgeOne.damageNode.funDamage.angle = float.Parse(damageChild.GetAttribute("angle"));
                            damgeOne.damageNode.funDamage.radius = float.Parse(damageChild.GetAttribute("radius"));
                            damgeOne.damageNode.funDamage.offDistance = 0;
                            damgeOne.damageNode.funDamage.offDistanceX = 0;
                            if (damageChild.HasAttribute("offDistance"))
                            {
                                damgeOne.damageNode.funDamage.offDistance = float.Parse(damageChild.GetAttribute("offDistance")); ;
                            }

                            if (damageChild.HasAttribute("offDistanceX"))
                            {
                                damgeOne.damageNode.funDamage.offDistanceX = float.Parse(damageChild.GetAttribute("offDistanceX")); ;
                            }

                            //curDistance = damgeOne.damageNode.funDamage.radius + damgeOne.damageNode.funDamage.offDistance;
                        }
                        else if (damageChild.Name == "rect" && damgeOne.type == DamageTypeID.DTID_RECT)
                        {
                            damgeOne.damageNode.rectDamage = new RectDamageNode();

                            damgeOne.damageNode.rectDamage.length = float.Parse(damageChild.GetAttribute("length"));
                            damgeOne.damageNode.rectDamage.width = float.Parse(damageChild.GetAttribute("width"));
                            damgeOne.damageNode.rectDamage.offDistance = 0;
                            damgeOne.damageNode.rectDamage.offDistanceX = 0;
                            if (damageChild.HasAttribute("offDistance"))
                            {
                                damgeOne.damageNode.rectDamage.offDistance = float.Parse(damageChild.GetAttribute("offDistance")); ;
                            }

                            if (damageChild.HasAttribute("offDistanceX"))
                            {
                                damgeOne.damageNode.rectDamage.offDistanceX = float.Parse(damageChild.GetAttribute("offDistanceX")); ;
                            }

                            //curDistance = damgeOne.damageNode.rectDamage.length + damgeOne.damageNode.rectDamage.offDistance;
                        }
                        else if (damageChild.Name == "skillWeight")
                        {
                            damgeOne.resetSkillWeight = int.Parse(damageChild.GetAttribute("resetValue"));
                        }
                        else if (damageChild.Name == "notUseCurveMove")
                        {
                            if (damageChild.GetAttribute("value") == "true")
                            {
                                damgeOne.isNotUseCurveMove = true;
                            }
                        }
                        else if (damageChild.Name == "frameStop")
                        {
                            //顿帧元素
                            damgeOne.frameStopDesc = new FrameStopCellDesc();
                            damgeOne.frameStopDesc.frameStopTime = float.Parse(damageChild.GetAttribute("time"));

                            damgeOne.frameStopDesc.prefabPath = damageChild.GetAttribute("prefabPath");
                        }
                        else if (damageChild.Name == "cameraShake")
                        {
                            damgeOne.cameraShakeDesc = new CameraShakeCellDesc();
                            //    damgeOne.cameraShakeDesc.playTime = float.Parse(damageChild.GetAttribute("playtime"));

                            damgeOne.cameraShakeDesc.prefabPath = damageChild.GetAttribute("prefabPath");

                            if (damageChild.HasAttribute("activeWhenHit"))
                            {
                                int activeWhenHit = int.Parse(damageChild.GetAttribute("activeWhenHit"));
                                damgeOne.cameraShakeDesc.activeWhenHit = (activeWhenHit == 0 ? false : true);
                            }

                            if (damageChild.HasAttribute("blurtime"))
                            {
                                float blurTime = float.Parse(damageChild.GetAttribute("blurtime"));
                                damgeOne.cameraShakeDesc.fBlurTime = blurTime;
                                if (blurTime > 0.0f)
                                {
                                    damgeOne.cameraShakeDesc.bBlur = true;
                                }
                            }


                        }
                        else if (damageChild.Name == "dizzyState")
                        {
                            damgeOne.dizzyParamDesc = new DizzyParamDesc();
                            damgeOne.dizzyParamDesc.keepTime = float.Parse(damageChild.GetAttribute("keepTime"));
                        }
                        else if (damageChild.Name == "warning")
                        {
                            // damgeOne.efxWarning = damageChild.GetAttribute("efxPrefab");

                            if (damageChild.GetAttribute("value") == "true")
                            {
                                if (damgeOne.type == DamageTypeID.DTID_FUN)
                                {
                                    if (damgeOne.damageNode.funDamage.angle == 360)
                                    {
                                        damgeOne.efxWarning = "Effect/skill/remain/fx_yujing_yuan";
                                    }
                                    else
                                    {
                                        damgeOne.efxWarning = "Effect/skill/remain/fx_yujing_shanxing";
                                    }
                                }

                                if (damgeOne.type == DamageTypeID.DTID_RECT)
                                {
                                    damgeOne.efxWarning = "Effect/skill/remain/fx_yujing_changfang";
                                }

                            }
                        }
                    }

                    //if (curDistance > data.distance)
                    //{
                    //    data.distance = curDistance;
                    //}

                    //data.damageList.Add(damgeOne);
                }

                foreach (XmlElement zoom in node.ChildNodes)
                {
                    if (zoom.Name != "cameraZoom")
                    {
                        continue;
                    }
                    CameraZoomCellDesc desc = new CameraZoomCellDesc();
                    desc.prefabPath = zoom.GetAttribute("prefabPath");
                    desc.playTime = float.Parse(zoom.GetAttribute("playTime"));
                    desc.distance = float.Parse(zoom.GetAttribute("distance"));

                    if (zoom.HasAttribute("blurStart"))
                    {
                        var str = zoom.GetAttribute("blurStart");
                        if (string.IsNullOrEmpty(str.Trim()))
                        {
                            desc.blurStart = -1;
                        }
                        else
                        {
                            desc.blurStart = float.Parse(str);
                        }
                    }
                    else
                    {
                        desc.blurStart = -1f;
                    }

                    if (zoom.HasAttribute("blurDuration"))
                    {
                        var str = zoom.GetAttribute("blurDuration");
                        if (string.IsNullOrEmpty(str.Trim()))
                        {
                            desc.blurDuration = 0;
                        }
                        else
                        {
                            desc.blurDuration = float.Parse(str);
                        }
                    }
                    else
                    {
                        desc.blurDuration = 0;
                    }

                    data.castStageDataList.Add(desc);
                }

                //其他元素            
                foreach (XmlElement attack in node.ChildNodes)
                {
                    //位移元素
                    if (attack.Name == "moveAttack")
                    {
                        MovePosAttackDesc cellData = new MovePosAttackDesc();
                        cellData.prefabPath = attack.GetAttribute("prefabPath");

                        //yy 美术位移
                        cellData.moveDistance = float.Parse(attack.GetAttribute("moveDistance"));

                        //
                        data.movePosAttackDesc = new MovePosAttackDesc();
                        data.movePosAttackDesc.efxPrefab = "";
                        data.movePosAttackDesc.prefabPath = attack.GetAttribute("prefabPath");

                        data.movePosAttackDesc.moveDistance = cellData.moveDistance;

                        foreach (XmlElement moveSet in attack.ChildNodes)
                        {
                            if (moveSet.Name == "resetDistance")
                            {
                                data.movePosAttackDesc.resetAimActorTypeList = new List<ActorType>();


                            }

                        }


                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        foreach (XmlElement moveSet in attack.ChildNodes)
                        {
                            if (moveSet.Name == "efx")
                            {
                                cellData.efxPrefab = moveSet.GetAttribute("prefab");
                            }
                            else if (moveSet.Name == "isCarryOffTarget")
                            {
                                if (moveSet.GetAttribute("value") == "true")
                                {
                                    cellData.isCarryOffTarget = true;
                                }
                            }
                            else if (moveSet.Name == "isStopForTarget")
                            {
                                if (moveSet.GetAttribute("value") == "true")
                                {
                                    cellData.isStopForTarget = true;
                                }
                            }
                        }
                    }
                    else if (attack.Name == "fireFlyAttack")
                    {
                        //子弹元素   
                        FireFlyAttackDesc cellData = new FireFlyAttackDesc();

                        cellData.flySpeed = float.Parse(attack.GetAttribute("flySpeed"));
                        cellData.flyTime = float.Parse(attack.GetAttribute("flyTime"));
                        cellData.firePos = attack.GetAttribute("firePosObj");
                        cellData.fireTime = float.Parse(attack.GetAttribute("fireTime"));
                        cellData.neerDistance = float.Parse(attack.GetAttribute("neerDistance"));
                        cellData.aimActorTypeList = new List<ActorType>();
                        cellData.prefabPath = attack.GetAttribute("prefabPath");

                        int nTemp = int.Parse(attack.GetAttribute("bThroughFlag"));
                        cellData.bThroughFlag = nTemp != 0 ? true : false;

                        if (attack.HasAttribute("bOnGound"))
                        {
                            nTemp = int.Parse(attack.GetAttribute("bOnGound"));
                            cellData.bOnGound = nTemp != 0 ? true : false;
                        }



                        if (attack.HasAttribute("projectile"))
                            cellData.projectile = float.Parse(attack.GetAttribute("projectile"));

                        if (attack.HasAttribute("TanTanLeCount"))
                            cellData.TanTanLeCount = int.Parse(attack.GetAttribute("TanTanLeCount"));

                        if (attack.HasAttribute("TanTanLeDis"))
                            cellData.TanTanLeDis = float.Parse(attack.GetAttribute("TanTanLeDis"));

                        if (attack.HasAttribute("dizzyTime"))
                            cellData.dizzyTime = float.Parse(attack.GetAttribute("dizzyTime"));

                        if (attack.HasAttribute("displayWidth"))
                            cellData.displayWidth = float.Parse(attack.GetAttribute("displayWidth"));

                        if (attack.HasAttribute("displayLength"))
                            cellData.displayLength = float.Parse(attack.GetAttribute("displayLength"));


                        data.neerDistance = cellData.neerDistance;

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        foreach (XmlElement fireChild in attack.ChildNodes)
                        {
                            if (fireChild.Name == "bullet")
                            {
                                cellData.bulletNum = int.Parse(fireChild.GetAttribute("num"));
                                cellData.fAngle = float.Parse(fireChild.GetAttribute("angle"));
                                cellData.bulletPrefab = fireChild.GetAttribute("prefab");

                                if (fireChild.HasAttribute("bTakeAwayTarget"))
                                {
                                    int temp = int.Parse(fireChild.GetAttribute("bTakeAwayTarget"));
                                    cellData.bTakeAwayTarget = temp != 0 ? true : false;
                                }

                                if (fireChild.HasAttribute("disappearWhenTouchWall"))
                                {
                                    int temp = int.Parse(fireChild.GetAttribute("disappearWhenTouchWall"));
                                    cellData.disappearWhenTouchWall = temp != 0 ? true : false;
                                }

                                if (fireChild.HasAttribute("bCanNotMoveWhenTakenAway"))
                                {
                                    int temp = int.Parse(fireChild.GetAttribute("bCanNotMoveWhenTakenAway"));
                                    cellData.bCanNotMoveWhenTakenAway = temp != 0 ? true : false;
                                }

                                if (fireChild.HasAttribute("bEndTheSkillWhenDisappear"))
                                {
                                    int temp = int.Parse(fireChild.GetAttribute("bEndTheSkillWhenDisappear"));
                                    cellData.bEndTheSkillWhenDisappear = temp != 0 ? true : false;
                                }

                                if (fireChild.HasAttribute("bTraceTarget"))
                                {
                                    int temp = int.Parse(fireChild.GetAttribute("bTraceTarget"));
                                    cellData.bTraceTarget = temp != 0 ? true : false;
                                }

                                ParseBoolFromInt(attack, "bAttachToSkill", ref cellData.bAttachToSkill);
                                ParseBoolFromInt(attack, "bAttackMoreThanOnce", ref cellData.bAttackMoreThanOnce);
                            }

                            else if (fireChild.Name == "cameraShake")
                            {
                                cellData.cameraShakeDesc = new CameraShakeCellDesc();
                                cellData.cameraShakeDesc.prefabPath = fireChild.GetAttribute("prefabPath");
                                if (fireChild.HasAttribute("activeWhenHit"))
                                {
                                    int activeWhenHit = int.Parse(fireChild.GetAttribute("activeWhenHit"));
                                    cellData.cameraShakeDesc.activeWhenHit = (activeWhenHit == 0 ? false : true);
                                }
                            }
                        }
                    }
                    else if (attack.Name == "colliderDamage")
                    {
                        //碰撞伤害元素
                        CollideDamageDesc cellData = new CollideDamageDesc();
                        cellData.prefabPath = attack.GetAttribute("prefabPath");

                        cellData.aimActorTypeList = new List<int>();
                        foreach (XmlElement damageSet in attack.ChildNodes)
                        {
                            if (damageSet.Name == "endCarryOffTargetTime")
                            {
                                cellData.endCarryOffTargetTime = float.Parse(damageSet.GetAttribute("value"));
                            }
                        }

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }
                    }
                    else if (attack.Name == "curveDamage")
                    {
                        CurveDamageCellDesc cellData = new CurveDamageCellDesc();
                        cellData.aimActorTypeList = new List<ActorType>();
                        cellData.curveDamageInfoList = new List<OneCurveDamageInfo>();
                        cellData.prefabPath = attack.GetAttribute("prefabPath");

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        foreach (XmlElement curveSet in attack.ChildNodes)
                        {
                            if (curveSet.Name == "curve")
                            {
                                OneCurveDamageInfo oneCurve = new OneCurveDamageInfo();
                                oneCurve.offsetAngle = float.Parse(curveSet.GetAttribute("offsetAngle"));
                                oneCurve.length = float.Parse(curveSet.GetAttribute("length"));

                                foreach (XmlElement damageSet in curveSet.ChildNodes)
                                {
                                    if (damageSet.Name == "damageCell")
                                    {
                                        oneCurve.hitTime = float.Parse(damageSet.GetAttribute("hitTime"));
                                        oneCurve.damageWidth = float.Parse(damageSet.GetAttribute("width"));
                                        oneCurve.damageLength = float.Parse(damageSet.GetAttribute("length"));
                                    }
                                }

                                cellData.curveDamageInfoList.Add(oneCurve);
                            }
                        }

                    }
                    else if (attack.Name == "immune")
                    {
                        ImmuneCellDesc cellData = new ImmuneCellDesc();
                        cellData.prefabPath = attack.GetAttribute("prefabPath");
                        cellData.startTime = float.Parse(attack.GetAttribute("startTime"));
                        cellData.keepTime = float.Parse(attack.GetAttribute("keepTime"));

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }
                    }
                    else if (attack.Name == "attackBreak")
                    {
                        BreakPrepareDesc cellData = new BreakPrepareDesc();
                        cellData.prefabPath = attack.GetAttribute("prefabPath");
                        cellData.breakDamageHP = int.Parse(attack.GetAttribute("breakDamageHP"));

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        foreach (XmlElement breakSet in attack.ChildNodes)
                        {
                            if (breakSet.Name == "behitAction")
                            {
                                cellData.behitAction = breakSet.GetAttribute("action");
                            }
                            else if (breakSet.Name == "UI")
                            {
                                //cellData.UIPrefabPath = breakSet.GetAttribute("prefabPath");                                
                                if (breakSet.GetAttribute("used") == "true")
                                {
                                    cellData.isUsedUI = true;
                                }
                            }
                        }
                    }
                    else if (attack.Name == "prepareStage")
                    {
                        data.prepareStage = new PrepareStage();
                        if (attack.GetAttribute("open").Equals("true"))
                        {
                            data.prepareStage.open = true;
                        }
                        data.prepareStage.keepTime = float.Parse(attack.GetAttribute("keepTime"));

                        foreach (XmlElement prepare in attack.ChildNodes)
                        {
                            if (prepare.Name == "isLookAtTarget")
                            {
                                if (prepare.GetAttribute("value").Equals("true"))
                                {
                                    data.prepareStage.isLookAtTarget = true;
                                }
                            }
                        }
                    }
                    else if (attack.Name == "bossDeathFromSky")
                    {
                        //boss技能死从天降
                        BossDeathFromSkyDesc cellData = new BossDeathFromSkyDesc();

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        cellData.riseTime = float.Parse(attack.GetAttribute("riseTime"));
                        cellData.prepareDownTime = float.Parse(attack.GetAttribute("prepareDownTime"));
                        cellData.downTime = float.Parse(attack.GetAttribute("downTime"));

                        cellData.prefabPath = attack.GetAttribute("prefabPath");
                        if (attack.HasAttribute("efxPrefab"))
                        {
                            cellData.efxPrefab = attack.GetAttribute("efxPrefab");
                        }
                    }
                    else if (attack.Name == "actionCell")
                    {
                        //动作元素
                        ActionCellDesc cellData = new ActionCellDesc();

                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        cellData.prefabPath = attack.GetAttribute("prefabPath");
                        cellData.name = attack.GetAttribute("name");
                        if (attack.HasAttribute("playTime"))
                        {
                            cellData.playTime = float.Parse(attack.GetAttribute("playTime"));
                        }

                        if (attack.HasAttribute("delayTime"))
                        {
                            cellData.delayTime = float.Parse(attack.GetAttribute("delayTime"));
                        }

                        ParseBoolFromInt(attack, "shouldAttachToOwner", ref cellData.shouldAttachToOwner);

                        for (int k = 0; k < attack.ChildNodes.Count; ++k)
                        {
                            XmlElement nodeElm = (XmlElement)attack.ChildNodes[k];

                            if (nodeElm.Name == "efx")
                            {
                                cellData.efx = nodeElm.GetAttribute("value");
                            }
                            else if (nodeElm.Name == "setStartTime")
                            {
                                cellData.setStartTime = float.Parse(nodeElm.GetAttribute("value"));
                            }
                            else if (nodeElm.Name == "speed")
                            {
                                cellData.speed = float.Parse(nodeElm.GetAttribute("value"));
                            }
                            else if (nodeElm.Name == "sound1")
                            {
                                cellData.sound1 = int.Parse(nodeElm.GetAttribute("value"));
                            }
                            else if (nodeElm.Name == "sound2")
                            {
                                cellData.sound2 = int.Parse(nodeElm.GetAttribute("value"));
                            }

                            //yy 读取预警数据
                            else if (nodeElm.Name == "fun")
                            {
                                cellData.damageNode.funDamage = new FunDamageNode();

                                cellData.damageNode.funDamage.angle = float.Parse(nodeElm.GetAttribute("angle"));
                                cellData.damageNode.funDamage.radius = float.Parse(nodeElm.GetAttribute("radius"));
                                cellData.damageNode.funDamage.offDistance = 0;
                                if (nodeElm.HasAttribute("offDistance"))
                                {
                                    cellData.damageNode.funDamage.offDistance = float.Parse(nodeElm.GetAttribute("offDistance")); ;
                                }

                                if (nodeElm.HasAttribute("offDistanceX"))
                                {
                                    cellData.damageNode.funDamage.offDistanceX = float.Parse(nodeElm.GetAttribute("offDistanceX")); ;
                                }

                                //
                                if (cellData.damageNode.funDamage.angle == 360)
                                {
                                    cellData.efxWarning = "Effect/skill/remain/fx_yujing_yuan";
                                }
                                else
                                {
                                    cellData.efxWarning = "Effect/skill/remain/fx_yujing_shanxing";
                                }

                                //float curDistance = cellData.damageNode.funDamage.radius + cellData.damageNode.funDamage.offDistance;

                                cellData.type = DamageTypeID.DTID_FUN;
                            }
                            else if (nodeElm.Name == "rect")
                            {
                                cellData.damageNode.rectDamage = new RectDamageNode();

                                cellData.damageNode.rectDamage.length = float.Parse(nodeElm.GetAttribute("length"));
                                cellData.damageNode.rectDamage.width = float.Parse(nodeElm.GetAttribute("width"));
                                cellData.damageNode.rectDamage.offDistance = 0;
                                cellData.damageNode.rectDamage.offDistanceX = 0;
                                if (nodeElm.HasAttribute("offDistance"))
                                {
                                    cellData.damageNode.rectDamage.offDistance = float.Parse(nodeElm.GetAttribute("offDistance")); ;
                                }

                                if (nodeElm.HasAttribute("offDistanceX"))
                                {
                                    cellData.damageNode.rectDamage.offDistanceX = float.Parse(nodeElm.GetAttribute("offDistanceX")); ;
                                }

                                //float curDistance = cellData.damageNode.rectDamage.length + cellData.damageNode.rectDamage.offDistance;

                                cellData.efxWarning = "Effect/skill/remain/fx_yujing_changfang";

                                cellData.type = DamageTypeID.DTID_RECT;
                            }


                        }
                    }
                    else if (attack.Name == "summonCell") // add by yuxj 召唤技能
                    {
                        SummonCellDesc cellData = new SummonCellDesc();
                        if (attack.HasAttribute("stage"))
                        {
                            cellData.stage = int.Parse(attack.GetAttribute("stage"));
                        }

                        if (cellData.stage == 0)
                        {
                            data.prepareStageDataList.Add(cellData);
                        }
                        else if (cellData.stage == 1)
                        {
                            data.castStageDataList.Add(cellData);
                        }

                        if (attack.HasAttribute("posType"))
                            cellData.npcPosType = int.Parse(attack.GetAttribute("posType"));

                        cellData.prefabPath = attack.GetAttribute("prefabPath");

                        for (int k = 0; k < attack.ChildNodes.Count; ++k)
                        {
                            XmlElement nodeElm = (XmlElement)attack.ChildNodes[k];
                            if (nodeElm.Name == "npc")
                            {
                                if (nodeElm.HasAttribute("npcID"))
                                    cellData.npcID = int.Parse(nodeElm.GetAttribute("npcID"));
                                if (nodeElm.HasAttribute("count"))
                                    cellData.npcCount = int.Parse(nodeElm.GetAttribute("count"));
                                if (nodeElm.HasAttribute("dis"))
                                    cellData.npcDis = float.Parse(nodeElm.GetAttribute("dis"));
                                if (nodeElm.HasAttribute("killBefore"))
                                    cellData.killBefore = int.Parse(nodeElm.GetAttribute("killBefore"));
                                if (nodeElm.HasAttribute("buffID"))
                                    cellData.buffID = int.Parse(nodeElm.GetAttribute("buffID"));

                                if (nodeElm.HasAttribute("lifeTime"))
                                    cellData.lifeTime = float.Parse(nodeElm.GetAttribute("lifeTime"));
                                if (nodeElm.HasAttribute("deadthToKill"))
                                    cellData.deadthToKill = bool.Parse(nodeElm.GetAttribute("deadthToKill"));
                                if (nodeElm.HasAttribute("summonType"))
                                    cellData.summonType = int.Parse(nodeElm.GetAttribute("summonType"));
                                if (nodeElm.HasAttribute("followMe"))
                                    cellData.followMe = bool.Parse(nodeElm.GetAttribute("followMe"));

                            }
                        }
                    }
                    else if (attack.Name == "positionCell")
                    {
                        PositionCellDesc cellData = new PositionCellDesc();

                        DoSkillCellData(cellData, attack, data);

                        ParseBoolFromInt(attack, "attachToActor", ref cellData.AttachToActor);

                        if (attack.HasAttribute("behaviorSkillId"))
                            cellData.BehaviorSkillId = int.Parse(attack.GetAttribute("behaviorSkillId"));

                        if (attack.HasAttribute("speed"))
                            cellData.Speed = float.Parse(attack.GetAttribute("speed"));

                        if (attack.HasAttribute("delayMovingTime"))
                            cellData.DelayMovingTime = float.Parse(attack.GetAttribute("delayMovingTime"));

                        if (attack.HasAttribute("attachOffsetX"))
                            cellData.AttachOffset.x = float.Parse(attack.GetAttribute("attachOffsetX"));

                        if (attack.HasAttribute("attachOffsetY"))
                            cellData.AttachOffset.y = float.Parse(attack.GetAttribute("attachOffsetY"));

                        if (attack.HasAttribute("attachOffsetZ"))
                            cellData.AttachOffset.z = float.Parse(attack.GetAttribute("attachOffsetZ"));

                        ParseBoolFromInt(attack, "syncRotation", ref cellData.SyncRotation);

                        ParseBoolFromInt(attack, "syncOwnerPosition", ref cellData.SyncOwnerPosition);

                        ParseBoolFromInt(attack, "lookAtActor", ref cellData.LookAtActor);

                        for (int k = 0; k < attack.ChildNodes.Count; ++k)
                        {
                            XmlElement nodeElm = (XmlElement)attack.ChildNodes[k];
                            if (nodeElm.Name == "point")
                            {
                                PointDesc pt = new PointDesc();
                                if (nodeElm.HasAttribute("x"))
                                    pt.Point.x = float.Parse(nodeElm.GetAttribute("x"));

                                if (nodeElm.HasAttribute("y"))
                                    pt.Point.y = float.Parse(nodeElm.GetAttribute("y"));

                                if (nodeElm.HasAttribute("z"))
                                    pt.Point.z = float.Parse(nodeElm.GetAttribute("z"));

                                ParseBoolFromInt(nodeElm, "backToActor", ref pt.BackToActor);

                                ParseBoolFromInt(nodeElm, "endSkill", ref pt.EndSkill);

                                ParseBoolFromInt(nodeElm, "skipWhenTouch", ref pt.SkipWhenTouch);

                                cellData.PointList.Add(pt);
                            }
                            else if (nodeElm.Name == "rotation")
                            {
                                RotationDesc rot = new RotationDesc();
                                if (nodeElm.HasAttribute("speed"))
                                    rot.Speed = float.Parse(nodeElm.GetAttribute("speed"));

                                if (nodeElm.HasAttribute("beginRadius"))
                                    rot.BeginRadius = float.Parse(nodeElm.GetAttribute("beginRadius"));

                                if (nodeElm.HasAttribute("endRadius"))
                                    rot.EndRadius = float.Parse(nodeElm.GetAttribute("endRadius"));

                                if (nodeElm.HasAttribute("duration"))
                                    rot.Duration = float.Parse(nodeElm.GetAttribute("duration"));

                                cellData.RotationList.Add(rot);
                            }
                        }
                    }
                    else if (attack.Name == "buffFactoryCell")
                    {
                        BuffFactoryCellDesc cellData = new BuffFactoryCellDesc();

                        DoSkillCellData(cellData, attack, data);

                        if (attack.HasAttribute("radius"))
                            cellData.Radius = int.Parse(attack.GetAttribute("radius"));

                        if (attack.HasAttribute("delay"))
                            cellData.Delay = float.Parse(attack.GetAttribute("delay"));
                    }
                    else if (attack.Name == "forceFactoryCell")
                    {
                        ForceFactoryCellDesc cellData = new ForceFactoryCellDesc();

                        DoSkillCellData(cellData, attack, data);

                        if (attack.HasAttribute("radius"))
                            cellData.Radius = int.Parse(attack.GetAttribute("radius"));

                        if (attack.HasAttribute("speed"))
                            cellData.Speed = int.Parse(attack.GetAttribute("speed"));

                        if (attack.HasAttribute("delay"))
                            cellData.Delay = float.Parse(attack.GetAttribute("delay"));

                        if (attack.HasAttribute("lastTime"))
                            cellData.LastTime = float.Parse(attack.GetAttribute("lastTime"));

                        ParseBoolFromInt(attack, "isAoe", ref cellData.IsAoe);
                    }
                    else if (attack.Name == "compositedSkillCell")
                    {
                        CompositedSkillCellDesc cellData = new CompositedSkillCellDesc();
                        DoSkillCellData(cellData, attack, data);
                        for (int k = 0; k < attack.ChildNodes.Count; ++k)
                        {
                            XmlElement nodeElm = (XmlElement)attack.ChildNodes[k];
                            if (nodeElm.Name == "subSkillDesc")
                            {
                                SubSkillDesc skill = new SubSkillDesc();
                                if (nodeElm.HasAttribute("skillId"))
                                    skill.SkillId = int.Parse(nodeElm.GetAttribute("skillId"));

                                if (nodeElm.HasAttribute("delayTime"))
                                    skill.DelayTime = float.Parse(nodeElm.GetAttribute("delayTime"));

                                cellData.SkillList.Add(skill);
                            }
                        }

                    }
                    else if (attack.Name == "stealthCell")
                    {
                        StealthCellDesc cellData = new StealthCellDesc();
                        if (attack.HasAttribute("disappearTime"))
                        {
                            float.TryParse(attack.GetAttribute("disappearTime"), out cellData.startTime);
                        }
                        if (attack.HasAttribute("duration"))
                        {
                            float.TryParse(attack.GetAttribute("duration"), out cellData.durationTime);
                        }

                        DoSkillCellData(cellData, attack, data);
                    }
                }

                if (m_SkillClassDisplayDescMap.ContainsKey(skillID))
                {
                    LogMgr.UnityError("SkillClassDisplayDescMap ID 重复 skillID:" + skillID);
                }
                else
                {
                    m_SkillClassDisplayDescMap.Add(skillID, data);
                }
            }



            m_composeSkillDescList = new List<ComposeSkillDesc>();
            filePath = @"Data/skill/ComposeSkillDesc";
            xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                LogMgr.UnityLog("open xml faild! " + filePath);
                yield return true;
            }



            nodeList = xmlData.ReadAll();
            foreach (XmlElement node in nodeList)
            {
                ComposeSkillDesc data = new ComposeSkillDesc();

                data.srcSkillID = int.Parse(node.GetAttribute("srcSkillID"));
                data.dstSkillID = int.Parse(node.GetAttribute("dstSkillID"));
                data.composeSkillID = int.Parse(node.GetAttribute("composeSkillID"));
                //  data.endSkillID = int.Parse(node.GetAttribute("endSkillID"));

                data.changeTime = 0;
                data.startTime = 0;
                data.ratio = 0;
                data.order = 0;
                data.isOrder = false;

                XmlNodeList childList = node.ChildNodes;
                foreach (XmlElement action in childList)
                {
                    if (action.Name == "action")
                    {
                        data.changeTime = float.Parse(action.GetAttribute("changeTime"));
                        string startTime = action.GetAttribute("startTime");
                        if (startTime != null && startTime.Length > 0)
                        {
                            data.startTime = float.Parse(action.GetAttribute("startTime"));
                        }

                        if (action.HasAttribute("ratio"))
                        {
                            data.ratio = int.Parse(action.GetAttribute("ratio"));
                        }

                        if (action.HasAttribute("order"))
                        {
                            data.order = int.Parse(action.GetAttribute("order"));
                            data.isOrder = true;
                        }
                        if (action.HasAttribute("defaultOrder"))
                        {
                            data.defaultorder = int.Parse(action.GetAttribute("defaultOrder"));
                        }
                    }
                }

                m_composeSkillDescList.Add(data);
            }

            m_changeSkillDescList = new List<ChangeSkillDesc>();
            filePath = @"Data/skill/ChangeSkillDesc";
            xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                LogMgr.UnityLog("open xml faild! " + filePath);
                yield return true;
            }

            nodeList = xmlData.ReadAll();
            foreach (XmlElement node in nodeList)
            {
                ChangeSkillDesc data = new ChangeSkillDesc();

                data.srcSkillID = int.Parse(node.GetAttribute("srcSkillID"));
                data.dstSkillID = int.Parse(node.GetAttribute("dstSkillID"));
                data.changeType = int.Parse(node.GetAttribute("changeType"));

                m_changeSkillDescList.Add(data);
            }


            //这个后面变成传送门什么的
            /*
            m_mapZoneMap = new Dictionary<int, MapZoneDesc>();
            filePath = @"Data/MapZoneDesc";
            xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                LogMgr.UnityLog("open xml faild! " + filePath);
                yield return true;
            }
            else
            {
                nodeList = xmlData.ReadAll();
                foreach (XmlElement node in nodeList)
                {
                    MapZoneDesc data = new MapZoneDesc();

                    data.mapID = int.Parse(node.GetAttribute("mapid"));
                    data.zoneList = new List<OneZoneData>();

                    //zone elment
                    XmlNodeList zoneList = node.ChildNodes;
                    foreach (XmlElement zoneNode in zoneList)
                    {
                        if (!zoneNode.Name.Equals("zone"))
                        {
                            continue;
                        }

                        OneZoneData zoneData = new OneZoneData();

                        zoneData.zoneID = int.Parse(zoneNode.GetAttribute("id"));
                        zoneData.doorID = zoneData.zoneID;
                        if (zoneNode.HasAttribute("doorId"))
                        {
                            zoneData.doorID = int.Parse(zoneNode.GetAttribute("doorId"));
                            //<0用默认值
                            if (zoneData.doorID < 0)
                            {
                                zoneData.doorID = zoneData.zoneID;
                            }
                        }

                        //zone pos
                        XmlNodeList posList = zoneNode.ChildNodes;
                        foreach (XmlElement posNode in posList)
                        {
                            if (posNode.Name.Equals("start"))
                            {
                                float x = float.Parse(posNode.GetAttribute("x"));
                                float y = float.Parse(posNode.GetAttribute("y"));
                                float z = float.Parse(posNode.GetAttribute("z"));

                                zoneData.startPos = new Vector3(x, y, z);
                            }
                            else if (posNode.Name.Equals("end"))
                            {
                                float x = float.Parse(posNode.GetAttribute("x"));
                                float y = float.Parse(posNode.GetAttribute("y"));
                                float z = float.Parse(posNode.GetAttribute("z"));

                                zoneData.endPos = new Vector3(x, y, z);
                            }
                        }

                        data.zoneList.Add(zoneData);
                    }

                    m_mapZoneMap.Add(data.mapID, data);
                }
            }
            */

            //技能被击反馈
            m_skillBehitDisplayMap = new Dictionary<int, SkillBehitDisplayDesc>();
            filePath = @"Data/skill/SkillBehitDisplayDesc";
            xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                LogMgr.UnityLog("open xml faild! " + filePath);
                yield return true;
            }
            //   yield return new WaitForSeconds(delayTime); 

            nodeList = xmlData.ReadAll();
            foreach (XmlElement node in nodeList)
            {
                SkillBehitDisplayDesc data = new SkillBehitDisplayDesc();

                data.skillWeight = short.Parse(node.GetAttribute("weight"));
                data.creatureBody = short.Parse(node.GetAttribute("body"));
                if (node.GetAttribute("isNonControl").Equals("true"))
                {
                    data.isNonControl = true;
                }

                if (data.isNonControl)
                {
                    //behittype                                
                    foreach (XmlElement attachNode in node.ChildNodes)
                    {
                        if (attachNode.Name.Equals("behit"))
                        {
                            data.behitType = (BehitType)int.Parse(attachNode.GetAttribute("type"));
                            data.actionList = new List<string>();

                            foreach (XmlElement behitNode in attachNode.ChildNodes)
                            {
                                if (behitNode.Name.Equals("nonControlTime"))
                                {
                                    data.nonControlSet = new NonControlSet();
                                    data.nonControlSet.keepTime = int.Parse(behitNode.GetAttribute("value"));
                                    if (behitNode.GetAttribute("reset").Equals("true"))
                                    {
                                        data.nonControlSet.canReset = true;
                                    }
                                }
                                else if (behitNode.Name.Equals("action"))
                                {
                                    data.actionList.Add(behitNode.GetAttribute("name"));
                                }
                            }
                        }
                    }
                }

                m_skillBehitDisplayMap.Add(HashCodeForShort2(data.skillWeight, data.creatureBody, 100), data);
            }

            LogMgr.UnityLog("------------------------------------加载XML结束---------------------------------------:" + (RealTime.time - beginTime));

        }




        //地图寻路点
        Vector3[] mWayPoints;

        //玩家出生点
        SpawnPlayer m_playerSpawnPoint;


        public SpawnPlayer PlayerSpawnPoint
        {
            get { return m_playerSpawnPoint; }
            set
            {

                m_playerSpawnPoint = value;

            }
        }

        /// <summary>
        /// 其他玩家出生点
        /// </summary>
        public List<SpawnPlayer> OtherPlayerSpawnPointList = new List<SpawnPlayer>();

        //敌方玩家出生点
        SpawnPlayer m_enemyPlayerSpawnPoint;
        public SpawnPlayer EnemyPlayerSpawnPoint
        {
            get { return m_enemyPlayerSpawnPoint; }
            set { m_enemyPlayerSpawnPoint = value; }
        }


        //进入新的场景前清除上一个场景的数据
        public void ClearSceneData()
        {
            mWayPoints = null;
            PlayerSpawnPoint = null;
        }

        //通过mapID 加载寻路点  和 玩家出生点
        public void loadWayPoints(int mapID)
        {
            string filePath = @"Data/MapData/" + @"" + mapID + "_Info";

            if (OtherPlayerSpawnPointList != null)
            {
                OtherPlayerSpawnPointList.Clear();
            }

            //读取种怪表
            PlayerSpawnPoint = new SpawnPlayer();

            XmlData xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                //LogMgr.UnityLog("loadWayPoints open xml faild! " + filePath);
                return;
            }


            XmlNodeList nodeList = xmlData.ReadAll();
            if (nodeList == null) return;
            Vector3[] wayPoints = new Vector3[nodeList.Count * 2];

            if (nodeList != null)
            {
                foreach (XmlElement node in nodeList)
                {
                    string typeName = node.GetAttribute("typeName");

                    if (typeName.Equals("WayPoint"))
                    {
                        Vector3 pos = new Vector3(float.Parse(node.GetAttribute("x")),
                            float.Parse(node.GetAttribute("y")),
                            float.Parse(node.GetAttribute("z")));

                        int index = int.Parse(node.GetAttribute("index"));

                        wayPoints[index] = pos;

                    }
                    else if (typeName.Equals("PlayerSpawnPoint"))
                    {

                        Vector3 pos = new Vector3(float.Parse(node.GetAttribute("x")),
                            float.Parse(node.GetAttribute("y")),
                            float.Parse(node.GetAttribute("z")));


                        Vector3 angle = new Vector3(float.Parse(node.GetAttribute("angleX")),
                        float.Parse(node.GetAttribute("angleY")),
                        float.Parse(node.GetAttribute("angleZ")));

                        PlayerSpawnPoint.pos = pos;
                        PlayerSpawnPoint.angle = angle;
                    }
                    else if (typeName.Equals("TargetPlayerSpawnPoint"))
                    {
                        Vector3 pos = new Vector3(float.Parse(node.GetAttribute("x")),
                        float.Parse(node.GetAttribute("y")),
                        float.Parse(node.GetAttribute("z")));


                        Vector3 angle = new Vector3(float.Parse(node.GetAttribute("angleX")),
                        float.Parse(node.GetAttribute("angleY")),
                        float.Parse(node.GetAttribute("angleZ")));

                        EnemyPlayerSpawnPoint = new SpawnPlayer();
                        EnemyPlayerSpawnPoint.pos = pos;
                        EnemyPlayerSpawnPoint.angle = angle;
                    }
                    else if (typeName.Equals("OtherPlayerSpawnPoint"))
                    {

                        Vector3 pos = new Vector3(float.Parse(node.GetAttribute("x")),
                            float.Parse(node.GetAttribute("y")),
                            float.Parse(node.GetAttribute("z")));


                        Vector3 angle = new Vector3(float.Parse(node.GetAttribute("angleX")),
                        float.Parse(node.GetAttribute("angleY")),
                        float.Parse(node.GetAttribute("angleZ")));

                        SpawnPlayer otherPlayerSpawnPoint = new SpawnPlayer();
                        otherPlayerSpawnPoint.pos = pos;
                        otherPlayerSpawnPoint.angle = angle;

                        OtherPlayerSpawnPointList.Add(otherPlayerSpawnPoint);
                    }
                }
            }


            List<Vector3> wayPointList = new List<Vector3>();
            for (int j = 0; j < wayPoints.Length; j++)
            {
                if (!wayPoints[j].Equals(Vector3.zero))
                {
                    wayPointList.Add(wayPoints[j]);
                }
            }

            mWayPoints = wayPointList.ToArray();

        }

        //通过mapID 寻路点数组
        public Vector3[] GetWayPoints()
        {

            return mWayPoints;
        }


        //add by Alex 20150319 获取挑战敌军的位置信息
        public SpawnPlayer[] GetSpawnChallengePlayerPos()
        {
            SpawnPlayer[] re = new SpawnPlayer[3];
            for (int i = 0; i < 3; i++)
            {
                re[i] = new SpawnPlayer();
            }

            if (EnemyPlayerSpawnPoint == null)
            {
                LogMgr.UnityError(" 对战地图没有 找到 敌方 出生点!!!!");
                return re;
            }

            Vector3 z = new Vector3(0, 0, 1f);

            Vector3 left = Quaternion.AngleAxis(EnemyPlayerSpawnPoint.angle.y + 90, Vector3.up) * z;

            //暂时用玩家自己的出生点做偏移作为挑战玩家的出生位置,直到工具做好后 

            re[2].pos = EnemyPlayerSpawnPoint.pos + left * 3;
            re[2].angle = new Vector3(EnemyPlayerSpawnPoint.angle.x, EnemyPlayerSpawnPoint.angle.y, EnemyPlayerSpawnPoint.angle.z);

            re[1].pos = EnemyPlayerSpawnPoint.pos - left * 3;
            re[1].angle = new Vector3(EnemyPlayerSpawnPoint.angle.x, EnemyPlayerSpawnPoint.angle.y, EnemyPlayerSpawnPoint.angle.z);

            re[0] = EnemyPlayerSpawnPoint;
            re[0].pos.z += 2;
            return re;
        }


        //add by Alex 20150319 获取挑战敌军的位置信息
        public SpawnPlayer[] GetSpawnChallengePlayerPosEx()
        {
            SpawnPlayer[] re = new SpawnPlayer[3];
            for (int i = 0; i < 3; i++)
            {
                re[i] = new SpawnPlayer();
            }

            if (EnemyPlayerSpawnPoint == null)
            {
                LogMgr.UnityError(" 对战地图没有 找到 敌方 出生点!!!!");
                return re;
            }

            Vector3 z = new Vector3(0, 0, 1f);

            Vector3 left = Quaternion.AngleAxis(EnemyPlayerSpawnPoint.angle.y + 90, Vector3.up) * z;

            //暂时用玩家自己的出生点做偏移作为挑战玩家的出生位置,直到工具做好后 

            re[2].pos = EnemyPlayerSpawnPoint.pos + left * 4;
            re[2].angle = new Vector3(EnemyPlayerSpawnPoint.angle.x, EnemyPlayerSpawnPoint.angle.y, EnemyPlayerSpawnPoint.angle.z);

            re[0] = EnemyPlayerSpawnPoint;

            re[1].pos = EnemyPlayerSpawnPoint.pos - left * 4;
            re[1].angle = new Vector3(EnemyPlayerSpawnPoint.angle.x, EnemyPlayerSpawnPoint.angle.y, EnemyPlayerSpawnPoint.angle.z);

            return re;
        }



        //通过mapID获取种怪信息
        public SpawnPlayer[] GetSpawnPlayerPos()
        {


            SpawnPlayer[] re = new SpawnPlayer[3];
            for (int i = 0; i < 3; i++)
            {
                re[i] = new SpawnPlayer();
            }
            if (null == PlayerSpawnPoint)
            {
                return re;
            }

            if (PlayerSpawnPoint == null)
            {
                return re;
            }

            Vector3 z = new Vector3(0, 0, 1f);

            Vector3 left = Quaternion.AngleAxis(PlayerSpawnPoint.angle.y + 90, Vector3.up) * z;
            //pvp状态,站开阵型
            if (CoreEntry.gGameMgr.IsPvpState())
            {
                left *= 3;
            }
            else
            {
                if (CoreEntry.IsMobaGamePlay())
                {
                    left *= 3;
                }
                else
                    left *= 2;
            }

            re[2].pos = PlayerSpawnPoint.pos + left;
            re[2].angle = new Vector3(PlayerSpawnPoint.angle.x, PlayerSpawnPoint.angle.y, PlayerSpawnPoint.angle.z);

            re[1].pos = PlayerSpawnPoint.pos - left;
            re[1].angle = new Vector3(PlayerSpawnPoint.angle.x, PlayerSpawnPoint.angle.y, PlayerSpawnPoint.angle.z);

            re[0] = PlayerSpawnPoint;
            if (CoreEntry.gGameMgr.IsPvpState())
            {
                re[0].pos.z -= 2;
            }
            return re;
        }





        //通过mapID获取种怪信息
        public SpawnMonster[] GetSpawnMonsterInfo(int mapID)
        {
            if (m_spawnMonsterMap.ContainsKey(mapID))
            {
                return m_spawnMonsterMap[mapID].ToArray();
            }

            //读取种怪表
            List<SpawnMonster> spawnMonsterList = new List<SpawnMonster>();
            string filePath = @"Data/MapData/" + mapID;
            XmlData xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                //LogMgr.UnityLog("open xml faild! " + filePath);
                return new SpawnMonster[0];
            }

            XmlNodeList nodeList = xmlData.ReadAll();
            foreach (XmlElement node in nodeList)
            {
                SpawnMonster data = new SpawnMonster();

                int id = int.Parse(node.GetAttribute("resid"));

                Vector3 pos = new Vector3(float.Parse(node.GetAttribute("x")),
                    float.Parse(node.GetAttribute("y")),
                    float.Parse(node.GetAttribute("z")));

                Vector3 angle = new Vector3(float.Parse(node.GetAttribute("angleX")),
                    float.Parse(node.GetAttribute("angleY")),
                    float.Parse(node.GetAttribute("angleZ")));

                int waveSeq = int.Parse(node.GetAttribute("waveseq"));
                bool enable = false;
                if (node.GetAttribute("enable").Equals("true"))
                {
                    enable = true;
                }

                data.mapID = mapID;
                data.resID = id;
                data.pos = pos;
                data.angle = angle;
                data.waveSequence = waveSeq;
                data.enable = enable;

                spawnMonsterList.Add(data);
            }
            m_spawnMonsterMap.Add(mapID, spawnMonsterList);

            return spawnMonsterList.ToArray();
        }




        //通过技能ID获取模版表现信息
        public SkillClassDisplayDesc GetSkillClassDisplayDesc(int skillID)
        {
            if (!m_SkillClassDisplayDescMap.ContainsKey(skillID))
            {
                //add by lzp 
                //LogMgr.UnityLog(" SkillClassDisplayDesc.xml 没有技能ID：" + skillID);
                //Debug.LogError(" SkillClassDisplayDesc.xml 没有技能ID：" + skillID);
                return null;
            }
            return m_SkillClassDisplayDescMap[skillID];
        }



        List<ComposeSkillDesc> composeSkillList = new List<ComposeSkillDesc>();
        List<ComposeSkillDesc> composeSkillListSort = new List<ComposeSkillDesc>();
        //获取组合技能
        public ComposeSkillDesc GetComposeSkillDesc(int srcSkillID, int dstSkillID)
        {

            composeSkillList.Clear();

            if (m_composeSkillDescList == null)
                return null;


            // foreach (ComposeSkillDesc skill in m_composeSkillDescList)
            for (int j = 0; j < m_composeSkillDescList.Count; ++j)
            {
                ComposeSkillDesc skill = m_composeSkillDescList[j];
                if (skill.srcSkillID == srcSkillID && skill.dstSkillID == dstSkillID)
                {
                    //return skill;
                    composeSkillList.Add(skill);
                }
            }

            if (composeSkillList.Count == 0)
            {
                return null;
            }


            if (composeSkillList.Count == 1)
            {
                return composeSkillList[0];
            }

            //顺序执行
            if (composeSkillList[0].isOrder)
            {
                int maxOrder = 0;
                ComposeSkillDesc aimDesc = null;
                //foreach (ComposeSkillDesc skill in composeSkillList)
                for (int j = 0; j < composeSkillList.Count; ++j)
                {
                    ComposeSkillDesc skill = composeSkillList[j];
                    //最大的顺序    
                    if (skill.order > maxOrder)
                    {
                        maxOrder = skill.order;
                        aimDesc = skill;
                    }
                }

                return aimDesc;
            }

            //List<ComposeSkillDesc> composeSkillListSort = new List<ComposeSkillDesc>(); 
            composeSkillListSort.Clear();
            //foreach (ComposeSkillDesc skill in composeSkillList)
            for (int j = 0; j < composeSkillList.Count; ++j)
            {
                ComposeSkillDesc skill = composeSkillList[j];
                //概率=100，或小于0的去掉            
                if (skill.ratio >= 100)
                {
                    return skill;
                }

                if (skill.ratio > 0)
                {
                    composeSkillListSort.Add(skill);
                }
            }

            if (composeSkillListSort.Count == 0)
            {
                return null;
            }

            //高低排序
            //composeSkillListSort.Sort(DescSortForComposeSkill);

            int[] index = new int[composeSkillListSort.Count];

            int maxNum = 0;
            int i = 0;
            //foreach (ComposeSkillDesc skill in composeSkillListSort)
            for (int j = 0; i < composeSkillListSort.Count; ++j)
            {
                ComposeSkillDesc skill = composeSkillListSort[j];
                maxNum += skill.ratio;
                index[i++] = maxNum;
            }

            UnityEngine.Random.seed = System.Guid.NewGuid().GetHashCode();
            int value = UnityEngine.Random.Range(0, maxNum);

            for (int k = 0; k < index.Length; ++k)
            {
                if (value < index[k])
                {
                    return composeSkillListSort[k];
                }
            }

            return null;
        }

        //成功释放后，设置顺序
        public void SetComposeSkillOrder(int srcSkillID, int dstSkillID, int composeSkillID)
        {
            int num = 0;
            foreach (ComposeSkillDesc skill in m_composeSkillDescList)
            {
                if (skill.srcSkillID == srcSkillID && skill.dstSkillID == dstSkillID)
                {
                    num++;
                }
            }

            foreach (ComposeSkillDesc skill in m_composeSkillDescList)
            {
                if (skill.srcSkillID != srcSkillID || skill.dstSkillID != dstSkillID)
                {
                    continue;
                }

                if (skill.composeSkillID == composeSkillID)
                {
                    skill.order -= (num - 1);
                }
                else
                {
                    skill.order += 1;
                }
            }

            //foreach (ComposeSkillDesc skill in m_composeSkillDescList)
            //{
            //    //LogMgr.UnityLog(skill.srcSkillID + ", " + skill.dstSkillID + ", " + skill.composeSkillID + ", order=" + skill.order);
            //}
        }

        public void ResetComposeSkillOrder(int dstSkillID)
        {
            foreach (ComposeSkillDesc skill in m_composeSkillDescList)
            {
                if (skill.dstSkillID != dstSkillID)
                {
                    continue;
                }

                if (skill.defaultorder != 0)
                {
                    skill.order = skill.defaultorder;
                }
            }
        }

        //获取切换技能
        public int GetChangeSkillID(int srcSkillID, int type)
        {
            foreach (ChangeSkillDesc skill in m_changeSkillDescList)
            {
                if (skill.srcSkillID == srcSkillID && skill.changeType == type)
                {
                    return skill.dstSkillID;
                }
            }

            return -1;
        }


        //获取指定位置的zone信息
        public OneZoneData GetAimPosZoneData(int mapID, Vector3 pos)
        {
            //Y轴数据不用,X-Z平面        
            if (!m_mapZoneMap.ContainsKey(mapID))
            {
                return null;
            }

            MapZoneDesc mapZoneDesc = m_mapZoneMap[mapID];
            foreach (OneZoneData zoneData in mapZoneDesc.zoneList)
            {
                if (zoneData.startPos.x <= pos.x && zoneData.startPos.z <= pos.z
                    && zoneData.endPos.x >= pos.x && zoneData.endPos.z >= pos.z)
                {
                    return zoneData;
                }
            }

            return null;
        }

        public OneZoneData GetAimPosZoneData(int mapID, int zoneID)
        {
            //Y轴数据不用,X-Z平面        
            if (!m_mapZoneMap.ContainsKey(mapID))
            {
                return null;
            }

            MapZoneDesc mapZoneDesc = m_mapZoneMap[mapID];
            foreach (OneZoneData zoneData in mapZoneDesc.zoneList)
            {
                if (zoneData.zoneID == zoneID)
                {
                    return zoneData;
                }
            }

            return null;
        }

        //通过坐标获取当前所在的zone
        public int GetZoneIDByPos(int iMapID, Vector3 pos)
        {
            MapZoneDesc mapZoneDesc = m_mapZoneMap[iMapID];
            foreach (OneZoneData zoneData in mapZoneDesc.zoneList)
            {
                if (pos.x >= zoneData.startPos.x && pos.x <= zoneData.endPos.x
                    && pos.z >= zoneData.startPos.z && pos.z <= zoneData.endPos.z)
                {
                    return zoneData.zoneID;
                }
            }

            return -1;
        }

        public int GetDoorIDOfZoneByPos(int iMapID, Vector3 pos)
        {
            MapZoneDesc mapZoneDesc = m_mapZoneMap[iMapID];
            foreach (OneZoneData zoneData in mapZoneDesc.zoneList)
            {
                if (pos.x >= zoneData.startPos.x && pos.x <= zoneData.endPos.x
                    && pos.z >= zoneData.startPos.z && pos.z <= zoneData.endPos.z)
                {
                    return zoneData.doorID;
                }
            }

            return -1;
        }


        public SkillBehitDisplayDesc GetSkillBehitDisplayDesc(int weight, int body)
        {
            int key = HashCodeForShort2((short)weight, (short)body, 100);
            SkillBehitDisplayDesc desc;
            if (m_skillBehitDisplayMap.TryGetValue(key, out desc))
            {
                return desc;
            }
            return null;
        }


        //获取默认技能列表
        public int[] GetDefaultLearnSkill(int resID)
        {
            List<int> skillList = new List<int>();

            //获取默认技能
            //Configs.CreatureDesc creatue = CsvMgr.GetRecordCreatureDesc(resID);
            //if (creatue != null)
            //{
            //    skillList.Add(creatue.normalAttID1);
            //    skillList.Add(creatue.normalAttID2);
            //}
            //else
            //{
            //    return skillList.ToArray();
            //}

            //for (int i = 0; i < creatue.SpecialSkillNum; ++i)
            //{
            //    skillList.Add(creatue.SpecialSkillDescSkillID [i]);            
            //}

            ////todo:多段技能       

            //switch (resID)
            //{
            //case 10003:
            //    {
            //       // int[] skills = { 500101, 500201, 500301, 500401, 500501, 500601, 500701, 500801, 500901 };
            //      //  skillList.AddRange(skills);
            //    }
            //    break;

            //default:
            //    break;
            //}

            return skillList.ToArray();
        }


        void DoSkillCellData(ISkillCellData cellData, XmlElement element, SkillClassDisplayDesc data)
        {
            if (cellData != null && element != null)
            {

                if (element.HasAttribute("stage"))
                {
                    cellData.stage = int.Parse(element.GetAttribute("stage"));
                }

                if (cellData.stage == 0)
                {
                    data.prepareStageDataList.Add(cellData);
                }
                else if (cellData.stage == 1)
                {
                    data.castStageDataList.Add(cellData);
                }

                cellData.prefabPath = element.GetAttribute("prefabPath");
            }
        }

        void ParseBoolFromInt(XmlElement element, string name, ref bool val)
        {
            if (element != null && element.HasAttribute(name))
            {
                int nTemp = int.Parse(element.GetAttribute(name));
                val = (nTemp != 0) ? true : false;
            }
        }


        public List<SceneEntitySet> GetEnityConfigInfo(int mapID)
        {
            if (m_EnityConfigMap.ContainsKey(mapID))
            {
                return m_EnityConfigMap[mapID];
            }

            //读取种怪表
            string filePath = @"Data/MapData/" + mapID;
            XmlData xmlData = new XmlData();
            if (!xmlData.OpenXml(filePath))
            {
                LogMgr.LogError("Can not load map data. path:{0}", filePath);
                return null;
            }

            List<SceneEntitySet> entityList = new List<SceneEntitySet>();

            XmlNode firstNode = xmlData.ReadNode("First");
            foreach (XmlElement typeNode in firstNode.ChildNodes)
            {
                SceneEntitySet entities = new SceneEntitySet();
                if ("monster".Equals(typeNode.Name))
                {
                    entities.type = EntityConfigType.ECT_MONSTER;
                }
                else if ("npc".Equals(typeNode.Name))
                {
                    entities.type = EntityConfigType.ECT_NPC;
                }
                else if ("collection".Equals(typeNode.Name))
                {
                    entities.type = EntityConfigType.ECT_COLLECTION;
                }
                else if ("portal".Equals(typeNode.Name))
                {
                    entities.type = EntityConfigType.ECT_PORTAL;
                }
                else if ("birth".Equals(typeNode.Name))
                {
                    entities.type = EntityConfigType.ECT_BIRTH;
                }
                else if ("point".Equals(typeNode.Name))
                {
                    entities.type = EntityConfigType.ECT_WAYPOINT;
                }
                else
                {
                    continue;
                }

                entities.entityList = new List<SceneEntityConfig>();
                foreach (XmlElement node in typeNode.ChildNodes)
                {
                    SceneEntityConfig data = new SceneEntityConfig();

                    data.configID = int.Parse(node.GetAttribute("id"));
                    data.modelID = int.Parse(node.GetAttribute("modelId"));
                    data.waveSeq = int.Parse(node.GetAttribute("wave_index"));
                    data.index = int.Parse(node.GetAttribute("index"));

                    float x = float.Parse(node.GetAttribute("x"));
                    float y = float.Parse(node.GetAttribute("y"));
                    float z = float.Parse(node.GetAttribute("z"));
                    data.position = new Vector3(x, y, z);

                    if (entities.type == EntityConfigType.ECT_WAYPOINT)
                    {
                        data.neighbours = new List<int>();
                        string pr = node.GetAttribute("pointRalations");
                        if(!(pr == null || pr == ""))
                        {
                            string[] nbs = pr.Split(',');
                            for (int ni = 0; ni < nbs.Length; ++ni)
                            {
                                data.neighbours.Add(int.Parse(nbs[ni]));
                            }
                        }
                    }

                    entities.entityList.Add(data);
                }

                entityList.Add(entities);
            }

            m_EnityConfigMap.Add(mapID, entityList);

            return entityList;
        }

    } //end GameDBMgr

}


