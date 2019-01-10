using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
[Hotfix]
public class TriggerMovie : MonoBehaviour
{
    public GameObject m_showObject;

    /// <summary>
    /// 是否是通过时间触发
    /// </summary>
    public bool IsUseTiggerTimer = false;
    /// <summary>
    /// 时间触发时间间隔
    /// </summary>
    public float TiggerTimer = 0.05f;

    void Awake()
    {
        m_showObject.SetActive(false );

        if (IsUseTiggerTimer)
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FuBen_loading_end, GE_FuBen_loading_end);
        }
        
    }

    public void GE_FuBen_loading_end(GameEvent ge, EventParameter paramter)
    {
        if (IsUseTiggerTimer)
        {
            Invoke("TimeOut", TiggerTimer);
            if (CoreEntry.gEventMgr != null)
            {
                CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_FuBen_loading_end, GE_FuBen_loading_end);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsUseTiggerTimer == true)
        {
            return;
        }

        //if (CoreEntry.gTeamMgr == null)
        //    return;
        if (CoreEntry.gActorMgr.MainPlayer == null)
            return;
        if (other.gameObject == CoreEntry.gActorMgr.MainPlayer)
        {
            OnPlayerEnter(other.gameObject); 
        }

    }

    void TimeOut()
    {
        OnPlayerEnter(CoreEntry.gActorMgr.MainPlayer.gameObject);
    }

    //int m_triggerNextWaveTimes = 0; 

    void OnPlayerEnter(GameObject mainPlayer)
    {
        //StartCoroutine(OnPlayerEnterDelay()); 
    }

    //IEnumerator OnPlayerEnterDelay()
    //{
    //    //等待场景加载完才播boss 动画
    //    //while (CoreEntry.bLoadSceneComplete != true)
    //    //{
    //    //    LogMgr.unityLog("等待场景加载完才播boss 动画 ");
    //    //    yield return new WaitForSeconds(0.1f);
    //    //}


    //    //int id = MapMgr.Instance.EnterMapId;
    //    //add by lzp 20161208 攻城战1v1指引 
    //    if (MainRole.Instance.mapid == 578)
    //    {
    //    }
    //    //if (LogMgr.isTriggerMovie ==false)
    //    //{
    //    //    TriggerRoot root = NGUITools.FindInParents<TriggerRoot>(gameObject);

    //    //    if (root != null && root.isTriggerNextWave)
    //    //    {
    //    //        if (m_triggerNextWaveTimes <= 0)
    //    //        {
    //    //            CoreEntry.gGameMgr.TriggerNextWave();
    //    //            m_triggerNextWaveTimes = 1;
    //    //        }
    //    //    }
    //    //    //Invoke("SetEnableFalse", 0.001f);
    //    //    gameObject.SetActive(false);

    //    //    yield break; 
    //    //    //return; 
    //    //}

    //    m_showObject.SetActive(true);
    //    gameObject.SetActive(false); 
    //}

}

