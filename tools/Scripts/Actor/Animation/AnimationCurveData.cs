using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//一个perfabs保存一份，有位移的动作有对应的数据
namespace SG
{

[Hotfix]
public class AnimationCurveData : MonoBehaviour {        
    public List<CurveData> m_listCurveData;

    public void Clear()
    {                
        m_listCurveData.Clear();                    
    }

    public void AddCurveData(CurveData data)
    {        
        for (int i = 0; i < m_listCurveData.Count; ++i)
        {
            if (m_listCurveData[i].strAnimationName.Equals(data.strAnimationName))
            {
                //已经存在了
                return;                
            }
        }

        m_listCurveData.Add(data);        
    }        

    public CurveData GetCurveData(string strAnimationName)
    {
        for (int i = 0; i < m_listCurveData.Count; ++i)
        {
            if (m_listCurveData[i].strAnimationName.Equals(strAnimationName))
            {
                return m_listCurveData[i];
            }
        }

        return null;            
    }               
}


[System.Serializable]
[Hotfix]
public class CurveData
{
    public string strAnimationName;         //动作名
    public OneCurveData[] curve;            //对应的曲线

    //卡帧，前面的帧不缩放，后面的压缩
    public int keyIndex = 0;    
}

[System.Serializable]
[Hotfix]
public class OneCurveData
{
    public string propertyName;     //属性名：m_LocalPosition.x
    public AnimationCurve curve;    //动画曲线
}

};  //end SG

