using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SG
{
    // 修改指定数值, 最大生命，最大法力，攻击力，防御力 等等
[Hotfix]
    public class AddHpAttrEffect : ChangeAttrEffect
    {
      //  public override void ChangeAttribteFortran()
       // {
            ////暂时不使用"治疗效果"这个武将属性，因为没有足够的数组长度
            //ChangeAttrType typeid1 = GetChangeAttrType(effectDesc.stringParam1);
            //ChangeAttrType typeid2 = GetChangeAttrType(sBindPos);

            //if (typeid1 == ChangeAttrType.Unknown)
            //    LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, effectDesc.stringParam1));
            //if (typeid2 == ChangeAttrType.Unknown)
            //    LogMgr.UnityLog(string.Format("buffConfig  id = {0} 找不到对应的Key={1}", effectDesc.id, sBindPos));

        

            //float curValue1 = GetAttribteValue(owner, typeid1);//血量
            //float curValue2 = GetAttribteValue(giver, typeid2);//攻击
            //float newValue = 0;


            //float hitValue1 = GetAttribteValue(owner, typeid1);
            //float hitValue2 = GetAttribteValue(owner, typeid2);
            ////                      攻击力                     技能治疗加成         技能基础值
            //newValue = curValue1 + curValue2 * (float)effectDesc.fParam[1] + (float)effectDesc.fParam[0];
            //// 公式
            ////newValue = curValue1 + (curValue2 * (((float)effectDesc.fParam[0]) * 0.01f) + (float)effectDesc.fParam[1]);

            //SetAttribteValue(owner, typeid1, newValue);


     //   }






    }
}





