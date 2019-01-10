using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SG;
[Hotfix]
public class UIClickSound : MonoBehaviour
{
    public int soundID = 900000;
    public void Awake()
    {
        Button button = this.GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(OnSoundClick);
        } 
    }
     
    public void OnSoundClick()
    {

        if (!CoreEntry.cfg_bEaxToggle)
        {
            return;
        }

        if (soundID == 0)
        {
            return;
        }

        string path = AudioMgr.GetAudioPath(soundID);
        if (path == null)
        {
            LogMgr.UnityError("无效音效ID:" + soundID);
            return;
        }

        AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(path, typeof(AudioClip));
        if (clip == null)
        {
            LogMgr.UnityError("音效配置错误 路径:" + path + "  id" + soundID);
            return;
        }

        NGUITools.PlaySound(clip); 
    }
}

