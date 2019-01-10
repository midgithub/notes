using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class LongLoop : MonoBehaviour {


    public float LoopTime = 3f;
    public GameObject Obj;
	// Use this for initialization
	void Start () {
	
	}
    void OnEnable()
    {
        preTime = Time.time;
        if (Obj != null)
            Obj.SetActive(true);
    }
    float preTime = 0;
	// Update is called once per frame
	void Update () {
        if (preTime == 0)
        {
            preTime = Time.time;
            if (Obj != null)
                Obj.SetActive(true);
        }
        if (Time.time - preTime > LoopTime)
        {
            preTime = 0;
            if (Obj != null)
                Obj.SetActive(false);
        }

	}
}

