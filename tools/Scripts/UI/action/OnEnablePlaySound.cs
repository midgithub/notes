using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class OnEnablePlaySound : MonoBehaviour 
{
    private AudioSource m_Audiosources;
    void OnEnable()
    {
        if (!SG.CoreEntry.cfg_bEaxToggle)
            return;

        m_Audiosources = GetComponent<AudioSource>();
        if (m_Audiosources != null)
            m_Audiosources.Play();
    }
}

