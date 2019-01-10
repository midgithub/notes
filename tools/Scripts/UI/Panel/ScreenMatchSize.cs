using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenMatchSize : MonoBehaviour {

    public Canvas mCans;
    public RectTransform mTargetObj;
	// Use this for initialization
	void Awake () 
    {
        Resize();
	}
    public static Vector2 GetUIScreenSize(Canvas cas)
    {

        CanvasScaler fs = cas.GetComponent<CanvasScaler>();

        Vector2 screenSize = fs.referenceResolution;



        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        Vector2 vResloution = fs.referenceResolution;


        if (fs.matchWidthOrHeight == 0)
        {
            float Scale = screenWidth / vResloution.x;
            screenSize.y = screenHeight / Scale;
        }
        else
        {
            float Scale = screenHeight / vResloution.y;
            screenSize.x = screenWidth / Scale;
        }

        return screenSize;

    }

    public void Resize()
    {
        if (mCans == null)
            return;
        Vector2 uiSize = GetUIScreenSize(mCans);
        Vector2 targetSize = mTargetObj.sizeDelta;

        Vector2 targetReizeSize = uiSize;

        CanvasScaler fs = mCans.GetComponent<CanvasScaler>();

        if (fs.matchWidthOrHeight == 0)
        {
            float Scale = targetSize.x/ uiSize.x;
            targetReizeSize.y = uiSize.y / Scale;
        }
        else
        {
            float Scale = targetSize.y / uiSize.y;
            targetReizeSize.x = uiSize.x / Scale;
        }

        mTargetObj.sizeDelta = targetReizeSize; ;

    }
     

}

