using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class DungeonMono : MonoBehaviour {

    public static DungeonMono instance;
    [HideInInspector]
    public int delTime = 3; //3秒延时刷怪
    [HideInInspector]
    public bool bStart = false;
    public List<MsgData_sSceneObjectEnterMonster> list = new List<MsgData_sSceneObjectEnterMonster>();

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        delTime = 3;
        if (DungeonMgr.bLoadFirstUI)
        {
            InvokeRepeating("LoadMonsterLate", 0, 1);  //延时3秒刷怪
            DungeonMgr.bLoadFirstUI = false;
        }

        InvokeRepeating("RefreshTimeDataUpdate", 0, 1);  //1秒执行一次
    }

    void LoadMonsterLate()
    {    
        if(delTime >= 0)    //开场倒计时
        {
            EventParameter par = EventParameter.Get();
            par.intParameter = delTime;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_DungeonStartTimeDown, par);   //lua 输出 UI逻辑
            delTime -= 1;
        }else
        {
            CancelInvoke("LoadMonsterLate");
            /*

            bStart = true;        //倒计时结束，场景刷怪
            list = DungeonMgr.Instance.monsterList;
            foreach (var item in list)
            {
                LoadModel(item);
            }
            list.Clear();
            */
        }
    }
    void RefreshTimeDataUpdate()
    {
        EventParameter par = EventParameter.Get();
        CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_HunLingXianYuTimeSecondUpdate, par);
    }

    public void LoadModel(MsgData_sSceneObjectEnterMonster monster)
    {
        CoreEntry.gSceneLoader.LoadMonster(monster);
    }
    /// <summary>
    /// 战斗中 打开全屏UI ，这个UI会被销毁，而倒计时没走完。。 此时还没有切开始自动战斗
    /// </summary>
    private void OnDisable()
    {
      //  Debug.LogError("delTime " + delTime);
        if (delTime > 0)
        {
            CoreEntry.gAutoAIMgr.AutoFight = true;
        }
    }

    void OnDestroy()
    {
        CancelInvoke("LoadMonsterLate");
        CancelInvoke("RefreshTimeData");
    }

}

