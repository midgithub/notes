using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{
    // 在目标身上绑定一个特效
[Hotfix]
    public class BoneBuffEffect : BuffEffect
    {
        //GameObject boneEffect = null;
        //Transform refNode = null;
      
        /// <summary>
        /// 是否随着owner一起缩放
        /// </summary>
      

        public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
        {
            base.Init(desc, _owner, _giver);
           // effectPath = "Effect/skill/buff/" + desc.buff_param3;

            //  scaleEffectWithOwner = (desc.fParam[3] == 0);
        }

        public override void OnEnter(Buff _ownerBuff)
        {
            //if (owner == null || owner.IsDeath())
            //{
            //    return;
            //}

            base.OnEnter(_ownerBuff);

            ////boneEffect = GameObject.Instantiate(Resources.Load(effectPath)) as GameObject;
            //boneEffect = (GameObject)Object.Instantiate(CoreEntry.gResLoader.LoadResource(effectPath));//CoreEntry.gGameObjPoolMgr.InstantiateEffect(effectPath);

            //if (boneEffect == null)
            //    return;


            //if (sBindPos != "")
            //{
            //    // 取得所有子Transform列表
            //    Transform[] transforms = owner.gameObject.GetComponentsInChildren<Transform>();
            //    for (int i = 0; i < transforms.Length; ++i)
            //    {
            //        if (transforms[i].name == sBindPos)
            //        {
            //            refNode = transforms[i];
            //            boneEffect.transform.parent = transforms[i];
            //            boneEffect.transform.localPosition = Vector3.zero;
            //            //boneEffect.transform.localRotation = Quaternion.identity;
            //            break;
            //        }
            //    }
            //}

            //if (refNode == null) // 无绑定点
            //{
            //    // 没有绑定点的特效不继承旋转
            //    refNode = owner.transform;

            //    boneEffect.transform.localPosition = new Vector3(refNode.transform.position.x, refNode.transform.position.y + 0.1f, refNode.transform.position.z);
            //    //boneEffect.transform.localRotation = Quaternion.identity; 
            //    if (scaleEffectWithOwner)
            //    {
            //        boneEffect.transform.localScale = refNode.transform.localScale;
            //    }
            //}

            //OnStealth(owner.m_bStealthState);
        }

        // Update is called once per frame
        public override void Update(float fElapsed)
        {
            //if (sBindPos == "" && refNode != null && boneEffect != null)// 没有绑定点的特效不继承旋转
            //{
            //    boneEffect.transform.localPosition = new Vector3(refNode.transform.position.x, refNode.transform.position.y + 0.1f, refNode.transform.position.z);
            //    if (scaleEffectWithOwner)
            //    {
            //        boneEffect.transform.localScale = refNode.transform.localScale;
            //    }
            //}
        }

        public override void OnExit()
        {
            base.OnExit();
            //if (boneEffect != null)
            //{
            //    GameObject.Destroy(boneEffect);
            //    //CoreEntry.gGameObjPoolMgr.Destroy(boneEffect);
            //    boneEffect = null;
            //}
        }

        public override void OnDie()
        {
            base.OnDie();

        }

        public override void OnStealth(bool bStealth)
        {
            //base.OnStealth(bStealth);
            //if (boneEffect)
            //    boneEffect.SetActive(!bStealth);
        }


        public override void Stacking()
        {
            base.Stacking();

            //叠加效果替换buffeffect
            if (effectDesc != null && effectDesc.Get<int>("buff_type") > 0)
            {
                ownerBuff.RemoveEffect(this);
                ownerBuff.AddEffect((int)effectDesc.Get<int>("buff_type"));
            }
        }
    }

}

