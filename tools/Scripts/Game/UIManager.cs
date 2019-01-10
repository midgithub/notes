using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class UIManager
{
    void Awake()
    {
       Init();
    }


	// Use this for initialization
	void Start () {
	
	}


    void Init()
    {
        GameObject gUI = GameObject.Find("GameUI");
        if (gUI == null)
        {
             gUI = new GameObject("GameUI");

        }

         CoreEntry.InitUI();

    }
	
	
}

};//end SG

