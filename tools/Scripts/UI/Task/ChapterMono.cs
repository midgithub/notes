using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class ChapterMono : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowOpenAnimation()
    {
        this.gameObject.GetComponent<Animator>().Play("zhangjiekaiqi");

    }   

    public void ShowCloseAnimation()
    {
        this.gameObject.GetComponent<Animator>().Play("zhangjiekaiqi_1");
    }

    public void CloseClick()
    {
        if(MainPanelMgr.Instance.ReturnPrePanel())
        {
            MainPanelMgr.Instance.ShowPanel("UIMain");
        }
    }

}

