using XLua;
﻿using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[Hotfix]
public class OffsetChange : MonoBehaviour 
{
	public float LoopCount=1;
    public float AStartTime = 0;
    public float AXSpeed = 0;
    public float AYSpeed = 1;
    public float BStartTime = 3;
    public float BXSpeed = 0;
    public float BYSpeed = 1;

    float xoffset = -1;
    float yoffset = -1;
	float prevXoffset=-10;
    float prevYoffset=-10;
	bool bFirstEnter=true;
	float timer=0;
	Material UseMaterial;
	// Use this for initialization

    void Reset()
    {
        {
            UseMaterial = GetComponent<Renderer>().material;
        }


        if (AXSpeed > 0) xoffset = -1;
        else if (AXSpeed < 0) xoffset = 1;
        else xoffset = 0;
        if (AYSpeed > 0) yoffset = -1;
        else if (AYSpeed < 0) yoffset = 1;
        else yoffset = 0;

        Vector2 CurrentOffeet = Vector2.zero;

        bFirstEnter = true;
        timer = 0.0f;
        prevXoffset = -10;
        prevYoffset = -10;

        UseMaterial.SetTextureOffset("_MainTex", CurrentOffeet);
    }

	void Start () 
	{

	}

    void OnDisable()
    {
        //Reset();
    }

    void OnEnable()
    {
        Reset();
    }
	
	// Update is called once per frame
	void Update () 
	{
        float deltaTime = 0f;

        {
            deltaTime = Time.deltaTime;
        }

		timer+=deltaTime;
		if(LoopCount <= 0)
		{
			xoffset+=deltaTime*AXSpeed;
			yoffset+=deltaTime*AYSpeed;
			if(xoffset>1) xoffset=-1;
			if(yoffset>1) yoffset=-1;
			if(xoffset<-1) xoffset=1;
			if(yoffset<-1) yoffset=1;
		}
		else if(LoopCount==1)
		{
			if(timer >= AStartTime)
			{
				xoffset += deltaTime * AXSpeed;
 		        yoffset += deltaTime* AYSpeed;
				if(xoffset>1) xoffset=1;
				if(xoffset<-1) xoffset=-1;
				if(yoffset>1) yoffset=1;
				if(yoffset<-1) yoffset=-1;
			}
		}
		else
		{
			if(timer<AStartTime)
			{
				
			}
			else if (timer < BStartTime)
            {
                xoffset += deltaTime * AXSpeed;
                yoffset += deltaTime * AYSpeed;
                if(xoffset>1) xoffset=1;
				if(xoffset<-1) xoffset=-1;
				if(yoffset>1) yoffset=1;
				if(yoffset<-1) yoffset=-1;
            }
            else
            {
				if(bFirstEnter)
				{
					if(BXSpeed>0) xoffset=-1;
		            else if(BXSpeed<0) xoffset=1;
		            else xoffset=0;
                    if(BYSpeed>0) yoffset=-1;
		            else if(BYSpeed<0) yoffset=1;
		            else yoffset=0;
					bFirstEnter=false;
				}
                xoffset += deltaTime * BXSpeed;
                yoffset += deltaTime * BYSpeed;
                if(xoffset>1) xoffset=1;
				if(xoffset<-1) xoffset=-1;
				if(yoffset>1) yoffset=1;
				if(yoffset<-1) yoffset=-1;
            }
		}
        if (prevXoffset != xoffset || prevYoffset != yoffset)
        {
            Vector2 CurrentOffeet = Vector2.zero;
            CurrentOffeet.Set(xoffset, yoffset);

            UseMaterial.SetTextureOffset("_MainTex", CurrentOffeet);
        }
        prevXoffset = xoffset;
        prevYoffset = yoffset;
	}
}

