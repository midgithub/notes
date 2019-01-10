/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;
using System;

[Hotfix]
public class MainThreadDispatcher : MonoBehaviour 
{
    private List<Action> mActionList = new List<Action>();
    private System.Object mLockObj = new System.Object();
    public void AddAction(Action action)
    {
        lock (mLockObj)
        {
            mActionList.Add(action);
        }
    }
    List<Action> list = new List<Action>();
    void Update()
    {
       // List<Action> list = new List<Action>();
        list.Clear();
        lock (mLockObj)
        {
            list.AddRange(mActionList);
            mActionList.Clear();
        }
        
        for (int i = 0; i < list.Count; i++)
        {
            list[i]();
        }
    }
}

