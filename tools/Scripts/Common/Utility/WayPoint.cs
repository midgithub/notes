using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;


[ExecuteInEditMode]
[Hotfix]
public class WayPoint : MonoBehaviour
{
 



    public int Index = -1;
    //public int Index
    //{
    //    get { return mIndex; }
    //    set { mIndex = value; }
    //}


	// Use this for initialization


    void Awake()
    {
          Index = -1;
          //string md5In = Md5Util.GetFileHash(""); 

    }


	void Start () {
        //object [] points = GameObject.FindObjectsOfTypeAll(typeof(WayPoint));
        //Debug.Log("points:" + points.Length);



        if (Index == -1)
        {
            GameObject[] points = GameObject.FindGameObjectsWithTag("WayPoint");

            Index = points.Length - 1; 

        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
 
}

