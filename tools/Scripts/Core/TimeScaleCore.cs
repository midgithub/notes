/**
* @file     : TimeScaleCore.cs
* @brief    : 
* @details  : 系统变量Time.timeScale操作，需要保持原子操作
* @author   : 
* @date     : 2014-12-16
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class TimeScaleCore {
    private static bool m_isBeSet = false;

    public static bool SetValue(float value)
    {     
        if (m_isBeSet)
        {
            return false;
        }
        
        CoreEntry.gTimeMgr.TimeScale = value;
        m_isBeSet = true;
         
        return true;    
    } 
    
    public static void ResetValue()
    {
        CoreEntry.gTimeMgr.TimeScale = 1;
        m_isBeSet = false;
    }              
}

};  //end SG

