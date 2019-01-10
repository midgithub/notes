using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using XLua;

namespace SG
{

    public enum BuffActionType
    {
        SCALE = 1,          //缩放
        EFFECT = 2,                //特效表现
        REBORN = 3,             //重生类

        LIMIT = 4,          //限制类  麻痹  定身 眩晕 中毒
        RECOVER = 5,            //无敌
        STEALTH = 6,            //隐身
        SHEEP = 7,              //变形
        MAT = 8,               //材质

        FEAR = 9,              //恐惧
        CHARM = 10,             //魅惑
        DISARM = 11, 			//缴械

        ATT = 12,              //修改属性   
        ICE = 13,              //冰冻 
        FLAG = 14,              //旗子


    }

    public enum SourceEnum
    {
        SELF,           //自己

        TEAM,           //队友

        FRIEND,             //友方

        ENEMY,          //敌方

        ALL,            //全体

        ELSE,           //其他

    }


    [LuaCallCSharp]
[Hotfix]
    public class BuffData
    {
        public long TargetID;   // 目标ID
        public int buffType;
        public long BufferInstanceID = 0; // Buffer实例ID
        public int Life = 0; // Buffer持续时间
        public int Count = 0; // Buffer叠加层数
        public int[] Param = new int[3]; // Buffer参数

    };



    [LuaCallCSharp]
[Hotfix]
    public class Buff
    {
        //  public float buffdelayTime = 0; // Buff延时生效时间 

        public int mIstacking;
        public ActorObj mAttack; // buff释放者
        public ActorObj mTarget; // buff承受者

        public int mBuffType = 0;
        public long mBuffId = 0; // Buffer实例ID
        public int mLife = 0; // Buffer持续时间  毫秒
        public int BuffLife = 0; // Buffer持续时间  毫秒
        public int mCount = 0; // Buffer叠加层数
        public int[] mParam = new int[3]; // Buffer参数

        bool bFinished;


        /// <summary>
        ///临时参数
        /// </summary>
        public int TempParameter = 0;

        List<BuffEffect> effects = new List<BuffEffect>();
        public LuaTable desc { get; set; }

        //LogicObject deliver;
        public Buff(BuffData buffData, ActorObj attack, ActorObj targetObj)
        {
            mAttack = attack;
            mTarget = targetObj;
            mBuffId = buffData.BufferInstanceID;
            bFinished = false;

            mBuffType = buffData.buffType;

            desc = ConfigManager.Instance.Skill.GetBuffConfig(mBuffType);
            if (desc != null)
            {
                mLife = buffData.Life;
                BuffLife = buffData.Life;   //使用服务器时间
            }

        }

        public T GetBuffEffect<T>() where T : BuffEffect
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i] is T)
                {
                    return (T)(object)effects[i];
                }
            }
            return default(T);

        }

        static public void OnBuffUpdateAll(ActorObj acotr)
        {
            int nCount = 0;
            while (true)
            {
                if (acotr.buffList.Count <= nCount)
                    break;

                Buff buff = acotr.buffList[nCount];
                buff.Update();

                if (buff.IsFinished())
                {
                    buff.OnExit();
                    //LogMgr.LogError("Buff.cs OnBuffUpdateAll " + buff.mBuffType);
                    acotr.buffList.Remove(buff);
                    //LogMgr.LogError("Buff.cs actor.buffList.count " + acotr.buffList.Count);
                }
                else
                    nCount++;
            }   
           
        }


        public void Update()
        {

            if (mLife > float.Epsilon)
            {
                BuffLife -= (int)(mAttack.GetBehaviorDeltaTime() * 1000);

                if (BuffLife <= float.Epsilon)
                {
                    for (int i = 0; i < effects.Count; i++)
                    {
                        BuffEffect eff = effects[i];
                        eff.OnTimeOut();
                    }
                    SetFinished();
                }
            }

            if (IsFinished() != false)
            {
                for (int i = 0; i < effects.Count; i++)
                {
                    BuffEffect eff = effects[i];
                    if (eff.buffdelayTime <= 0)
                    {
                        eff.Update(mAttack.GetBehaviorDeltaTime() * 1000);
                    }

                }
            }

        }

        public void OnEnter()
        {
            if (desc != null)
            {
                //特殊效果
                //   for (int i = 0; i < desc.buff_param3.Length; i++)
                {
                    if (desc.Get<int>("buff_type") != 0)
                    {
                        BuffEffect eff = BuffEffect.CreateBuffEffect(desc, this);
                        if (eff != null)
                        {
                            // eff.buffdelayTime = eff.effectDesc.buffdelayTime;
                            effects.Add(eff);
                        }
                    }
                }
            }

            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                if (eff.buffdelayTime <= 0)
                    eff.OnEnter(this);
            }

            //LogMgr.UnityLog("Buff OnEnter");
        }

        public void OnExit()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                eff.OnExit();
            }
            //LogMgr.UnityLog("Buff OnExit");
        }

        public void SetFinished()
        {
            //LogMgr.UnityError("Buff.cs SetFinished " + mBuffType);
            bFinished = true;
        }

        public void FinishEffect()
        {
            SetFinished();
            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                eff.SetFinished();
            }

        }


        public bool IsFinished()
        {
            if (bFinished == false)
                return false;

            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                if (eff.IsFinished() == false)
                    return false;
            }
            return bFinished;
        }

        // 角色死亡时
        public void OnDie()
        {
            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                eff.OnDie();
            }

            SetFinished();

        }

        // 角色死亡时
        public void OnStealth(bool bStealth)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                eff.OnStealth(bStealth);
            }
        }

        public void PauseBuff()
        {


        }

        public void ResumeBuff()
        {


        }

        // Buff重新被加上，在这里重设时间等
        public void ReplaceBuff()
        {
            BuffLife = mLife;
            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                eff.ReplaceBuff();
            }
        }


        static public void Preload(int iBuffId)
        {

        }

        //有的buff会需要设置Actor的State，但是State有时会被其他优先状态覆盖，在重设的时候会调用此处
        public bool ResetBuffState()
        {
            return false;
        }

        // 叠加Buff类
        public void Stacking()
        {
            //直接叠加时间
        }


        public void RemoveEffect(BuffEffect effect)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                BuffEffect eff = effects[i];
                if (eff == effect)
                {
                    effect.OnExit();
                    effect.SetFinished();
                    effects.RemoveAt(i);
                    return;
                }
            }
        }


        public void AddEffect(int buffEffectId)
        {
            BuffEffect eff = BuffEffect.CreateBuffEffect(desc, this);
            if (eff != null)
            {
                effects.Add(eff);
                eff.OnEnter(this);

            }
        }
    }
}

