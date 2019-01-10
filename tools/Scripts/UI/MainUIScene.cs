using XLua;
﻿using UnityEngine;
using System.IO;
using System.Collections;
using SG;
using UnityEngine.SceneManagement;

[Hotfix]
public class MainUIScene : MonoBehaviour
{

    static bool bLoginPanel = true;
	// Use this for initialization
	void Start ()
    {
        //登入界面改成prefab加载，MainUI,首次进入LoginPanel add by lzp 2015/6/8
        if (bLoginPanel)
        {
            MainPanelMgr.gotoPanel("LoginPanel");
            //MainPanelMgr.gotoPanel("UILogin"); 
            bLoginPanel = false;
        }
        if (SceneManager.GetActiveScene().name != MapMgr.mainCityMapName)
        {
            // UI Init
            MainPanelMgr.Instance.Init();
        }
        //add by Alex 20150416 向核心入口传递当前场景的背景音乐源
        CoreEntry.g_CurrentSceneMusic = gameObject.GetComponent<AudioSource>();
        //控制MainUI的音乐
        AudioSource a = gameObject.GetComponent<AudioSource>();
        if (a != null)
        {
            a.mute = !CoreEntry.cfg_bMusicToggle;
            if (!a.mute)
            {
                a.Play();
            }
        }

        //StartCoroutine(testAudio()); 

	}


    //音频测试
    IEnumerator testAudio()
    {
        yield return new WaitForSeconds(1.1f);

        //for (int i = 0; i < 100; i++ )
        //{
        //    AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(@"Sound/scene/xlys_amb", typeof(AudioClip));
        //    yield return new WaitForSeconds(0.001f);
        //}
        
        LogMgr.UnityError("------------------  testAudio");
    }
	
  
}

