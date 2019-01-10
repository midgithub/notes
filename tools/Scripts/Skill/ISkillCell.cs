/**
* @file     : ISkillCell.cs
* @brief    : 
* @details  : 技能元素基类.接受消息，处理交互
* @author   : 
* @date     : 2014-11-28 9:31
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
public struct stSkillCellInitParam
{
    public int cellIndex;     
    public char stage;       
}   
    
public enum SkillCellEventType
{
    SE_NONE = 0,
    SE_DESTROY_ACTION_EFX,      //删除动作特效 
    SE_STOP_CARRYOFF_TARGET,      //停止带目标位移
    SE_MAX
}

[Hotfix]
public class ISkillCell : MonoBehaviour {

    public bool m_bIsAoe = true;

    //todo:addevent        
    public virtual void Init(ISkillCellData cellData,SkillBase skillBase) {}
    public virtual void Init(ISkillCellData cellData,SkillBase skillBase, ActorObj actorBase) { } 

    public void SetAoeState(bool bIsAoe)
    {
        m_bIsAoe = bIsAoe;
    }


    //接受事件
    public virtual void OnEvent(SkillCellEventType eventType, params ValueArg[] valueArgs) { }


    public virtual void ShowSkillScope() { }
    public virtual void Preload(ISkillCellData cellData, SkillBase skillBase) { }
                                       
}

};  //end SG

