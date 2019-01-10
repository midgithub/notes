using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class ResetOnEnable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public Animation m_anim; 

    public GameObject m_resetObj; 

    void OnEnable()
    {
        m_resetObj.SetActive(false);
        m_anim.Stop();     
    }

    void OnDisable()
    {
        m_resetObj.SetActive(false);
        m_anim.Stop(); 

    }






 
}

