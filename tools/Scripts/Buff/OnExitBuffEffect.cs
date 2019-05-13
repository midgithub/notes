using UnityEngine;
using System.Collections;

using XLua;

namespace SG
{
    

// 操作限制类Buff 释放技能
[Hotfix]
public class OnExitBuffEffect : BuffEffect
{
    GameObject boneEffect = null;
    Transform refNode = null;

    public override void Init(LuaTable  desc, ActorObj _owner, ActorObj _giver)
    {
        base.Init(desc, _owner, _giver);
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
                sBindPos = "E_root";
            }

            if (p4 == 4)
            {
                sBindPos = "";
            }
        }

    public override void OnExit()
    {
        base.OnExit();

            //boneEffect = GameObject.Instantiate(CoreEntry.gResLoader.LoadResource(effectPath)) as GameObject;
        if (string.IsNullOrEmpty(effectPath))
        {
            return;
        }
        boneEffect = CoreEntry.gGameObjPoolMgr.InstantiateEffect(effectPath);

        if (boneEffect == null)
            return;

        EfxAttachActionPool efx = boneEffect.GetComponent<EfxAttachActionPool>();
        if (efx == null)
            efx = boneEffect.AddComponent<EfxAttachActionPool>();
        //if (effectDesc.fParam[1]!=0)
        //    efx.Init(null, effectDesc.fParam[1]);
        //else
            efx.Init(null, 2f); // 默认两秒

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
            boneEffect.transform.localScale = refNode.transform.localScale;
        }

        // 延时销毁特效
        //GameObject.Destroy(boneEffect, effectDesc.fParam[0]);

        OnStealth(owner.m_bStealthState);
    }

    public override void OnStealth(bool bStealth)
    {
        base.OnStealth(bStealth);
        if (boneEffect != null)
        {
            if (owner.mActorType != ActorType.AT_LOCAL_PLAYER)
            {
                boneEffect.SetActive(!bStealth);
            }
        }
    }
}

}

