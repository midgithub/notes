using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SG;
[Hotfix]
public class UISwitchScene  {
    public delegate void AlarmEventHandler(long serverID);//声明关于事件的委托
    static public event AlarmEventHandler Alarm;

    public delegate void TeleportRequestEventHandler(int type, int map, int x, int y);//声明关于事件的委托
    static public event TeleportRequestEventHandler TeleportRequest;

    static public int IS_REVIEW = 0;     //过审开关 0/关，1/开
    static GameObject _tip;
    static public void InitUITip(int type,long serverID, int _type, int map, int x, int y)
    {
        if(_tip == null)
        {
           Object o = CoreEntry.gResLoader.Load("UI/Prefabs/Common/UISwitchScene");
            _tip = GameObject.Instantiate(o) as GameObject;
        }
        _tip.SetActive(true);
        Button btn_ok = _tip.transform.Find("Frame/Ok").GetComponent<Button>();
        btn_ok.onClick.RemoveAllListeners();
        btn_ok.onClick.AddListener(delegate() {
            if(type == 1)
            {
                Alarm(serverID);
            }else
            {
                TeleportRequest(_type,map ,x,y);
            }
            _tip.SetActive(false);
        });
        Button btn_cancel = _tip.transform.Find("Frame/Cancel").GetComponent<Button>();
        btn_cancel.onClick.RemoveAllListeners();
        btn_cancel.onClick.AddListener(delegate () {
            _tip.SetActive(false);
        });

    }
}
