using UnityEngine;
using XLua;

namespace SG
{
    enum BuffEffectTriggerType
    {
        Circuit,
        Dying,
        Dead,
        CastingSkill,
        CastedSkill,

    }

[Hotfix]
    public class ActorEventArgs
    {

    }

[Hotfix]
    public class BuffEffect
    {
        public Buff ownerBuff;
        public bool bFinished = false;
        public float buffdelayTime=0;

        protected ActorObj owner;
        protected ActorObj giver;
        protected LuaTable effectDesc = null;

        protected string sBindPos = "";
        protected string effectPath;

        protected int m_skillLevel = 0;

        GameObject boneEffect = null;
        Transform refNode = null;

        bool scaleEffectWithOwner = false;

        public virtual void Init(LuaTable desc, ActorObj _owner, ActorObj _giver)
        {
            owner = _owner;
            giver = _giver;
            effectDesc = desc;

            effectPath = "Effect/skill/buff/" + desc.Get<string>("buff_param3");
            if (string.IsNullOrEmpty(desc.Get<string>("buff_param3")))
            {
                effectPath = string.Empty;
            }

            int p4 = desc.Get<int>("buff_param4");
            if (p4 == 0)
                sBindPos = "";

            if (p4 == 1)
            {
                sBindPos = "E_top";
            }

            if (p4 == 2)
            {
                sBindPos = "E_Spine";
            }

            if (p4 == 3)
            {
                sBindPos = "E_Root";
            }

            if(p4 == 4)
            {
                sBindPos = "";
            }
        }

        protected void InitBase(ActorObj _owner, ActorObj _giver)
        {

        }
        public virtual void OnEnter(Buff _ownerBuff)
        {
            ownerBuff = _ownerBuff;

            if (owner == null || owner.IsDeath())
            {
                return;
            }

            //Object obj = CoreEntry.gResLoader.LoadResource(effectPath);

            //if (obj == null)
            //    return;

            //boneEffect = (GameObject)Object.Instantiate(obj);

            if (string.IsNullOrEmpty(effectPath))
            {
                return;
            }

            boneEffect = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effectPath);

            if (boneEffect == null)
                return;


            if (sBindPos != "")
            {
                // 取得所有子Transform列表
                Transform[] transforms = owner.gameObject.GetComponentsInChildren<Transform>();
                for (int i = 0; i < transforms.Length; ++i)
                {
                    if (transforms[i].name == sBindPos)
                    {
                        refNode = transforms[i];
                        boneEffect.transform.parent = transforms[i];
                        boneEffect.transform.localPosition = Vector3.zero;
                        boneEffect.transform.localRotation = Quaternion.identity;
                        break;
                    }
                }
            }
            else
            {
                refNode = owner.transform;
                boneEffect.transform.parent = owner.transform;
                boneEffect.transform.localPosition = Vector3.zero;
                boneEffect.transform.localRotation = Quaternion.identity;
            }

            if (refNode == null) // 无绑定点
            {
                // 没有绑定点的特效不继承旋转
                refNode = owner.transform;

                boneEffect.transform.localPosition = new Vector3(refNode.transform.position.x, refNode.transform.position.y + 0.1f, refNode.transform.position.z);
                boneEffect.transform.localRotation = Quaternion.identity; 
                if (scaleEffectWithOwner)
                {
                    boneEffect.transform.localScale = refNode.transform.localScale;
                }
            }

            OnStealth(owner.m_bStealthState);
        }

        // Update is called once per frame
        public virtual void Update(float fElapsed)
        {
            if (sBindPos == "" && refNode != null && boneEffect != null)// 没有绑定点的特效不继承旋转
            {
                boneEffect.transform.localPosition = new Vector3(refNode.transform.position.x, refNode.transform.position.y + 0.1f, refNode.transform.position.z);
                if (scaleEffectWithOwner)
                {
                    boneEffect.transform.localScale = refNode.transform.localScale;
                }
            }
        }

        public virtual void OnExit()
        {
            if (boneEffect != null)
            {
                //GameObject.Destroy(boneEffect);
                CoreEntry.gGameObjPoolMgr.Destroy(boneEffect);
                boneEffect = null;
            }
        }

        public virtual void OnDie()
        {
            SetFinished();
        }

        public virtual void OnStealth(bool bStealth)
        {
           
            if (boneEffect)
                boneEffect.SetActive(!bStealth);

        }

        public void SetFinished()
        {
            bFinished = true;
        }

        public bool IsFinished()
        {
            return bFinished;
        }

        public virtual void OnTimeOut()
        {
            SetFinished();
        }

        public virtual void ReplaceBuff()
        {

        }

        public virtual void RefreshAttrs(BaseAttr attr)
        {

        }

        public virtual void Stacking()
        {
            
            //叠加效果替换buffeffect
            if (effectDesc != null && effectDesc.Get<int>("buff_type") > 0)
            {
                ownerBuff.RemoveEffect(this);
                ownerBuff.AddEffect((int)effectDesc.Get<int>("buff_type"));
            }

        }

        public static BuffEffect CreateBuffEffect(LuaTable desc, Buff buff )
        {
            if (desc == null)
                return null;


            BuffEffect newEff = null;
            switch (desc.Get<int>("action_type"))
            {
                case (int)BuffActionType.SCALE:    //缩放
                    newEff = new ScaleBuffEffect();
                
                    break;
                case (int)BuffActionType.EFFECT: //特效
                    newEff = new BoneBuffEffect();
                    break;
 
                case (int)BuffActionType.REBORN: // 重生类
                    newEff = new RebornEffect();
                    break;
                case (int)BuffActionType.LIMIT: // 限制类
                    newEff = new LimitControlEffect();
                    break;
                case (int)BuffActionType.RECOVER: // 无敌
                    newEff = new RecoverBuffEffect();
                    break;
 
                case (int)BuffActionType.STEALTH: // 潜行
                    newEff = new StealthBuffEffect();
                    break;
                case (int)BuffActionType.SHEEP: // 变形
                    newEff = new SheepBuffEffect();
                    break;
 
                case (int)BuffActionType.MAT:  //材质
                    newEff = new MaterialBuffEffect();
                    break;

                case (int)BuffActionType.FEAR:   //恐惧
                    newEff = new FearBuffEffect();
                    break;

                case (int)BuffActionType.CHARM:  //魅惑
                    newEff = new CharmBuffEffect();
                    break;


                case (int)BuffActionType.DISARM:  //缴械
                    newEff = new DisarmBuffEffect();
                    break;

                case (int)BuffActionType.ATT:  //修改属性 
                    newEff = new ChangeAttrEffect();
                    break;

                case (int)BuffActionType.ICE:   //冰冻 
                    newEff = new DisarmBuffEffect();
                    break;

                case (int)BuffActionType.FLAG:   //旗子 
                    newEff = new DisarmBuffEffect();
                    break;


                default:
                    break;
            }

            if (newEff != null)
            {
                newEff.Init(desc, buff.mAttack, buff.mTarget);
                return newEff;
            }
            return null;
        }
    }
}

