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

[Hotfix]
public class ServerList 
{
    private static ServerList instance = null;

    public static ServerList Instance
    {
        get
        {
            if (null == instance)
                instance = new ServerList();

            return instance;
        }
    }
}

