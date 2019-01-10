using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdjustCutoutSide{
    Left,
    Right
}

public class AdjustCutout : MonoBehaviour {

    public AdjustCutoutSide anchorSide = AdjustCutoutSide.Left;
    int anchorValue;
    UIWidget uiWidget;
    ScreenOrientation screenOrientationKind = ScreenOrientation.Unknown;
    private void Awake()
    {
        screenOrientationKind = Screen.orientation;
        uiWidget = GetComponent<UIWidget>();
        if (null == uiWidget)
        {
            Debug.LogError("not found UIWidget:" + name);
            return;
        }
        if (AdjustCutoutSide.Left == anchorSide)
        {
            anchorValue = uiWidget.leftAnchor.absolute;
        }
        else
        {
            anchorValue = uiWidget.rightAnchor.absolute;
        }
        if (CheckSwitchScreen.instance.liuHai)
        {
            InitView();
        }
    }
 //   // Use this for initialization
 //   void Start () {
		
	//}

    void InitView()
    {
        Debug.Log("ScreenAdapation--:" + screenOrientationKind);
        if (uiWidget == null)
        {
            return;
        }
        if (AdjustCutoutSide.Left == anchorSide)
        {
            if (screenOrientationKind == ScreenOrientation.LandscapeLeft || screenOrientationKind == ScreenOrientation.Landscape)
            {
                // home 在右边
                uiWidget.leftAnchor.absolute = anchorValue + CheckSwitchScreen.instance.cutoutHeight;
            }
            else if (screenOrientationKind == ScreenOrientation.LandscapeRight)
            {
                uiWidget.leftAnchor.absolute = anchorValue;
            }
            else
            {
                Debug.Log("XXXXXXXXXXXX:" + CheckSwitchScreen.instance.cutoutHeight);
                // home 在右边
                uiWidget.leftAnchor.absolute = anchorValue + CheckSwitchScreen.instance.cutoutHeight;
            }
        }
        else {
            if (screenOrientationKind == ScreenOrientation.LandscapeLeft || screenOrientationKind == ScreenOrientation.Landscape)
            {
                // home 在右边
                uiWidget.rightAnchor.absolute = anchorValue;
            }
            else if (screenOrientationKind == ScreenOrientation.LandscapeRight)
            {
                uiWidget.rightAnchor.absolute = anchorValue - CheckSwitchScreen.instance.cutoutHeight;
            }
            else
            {
                Debug.Log("XXXXXXXXXXXX:" + CheckSwitchScreen.instance.cutoutHeight);
                // home 在右边
                uiWidget.rightAnchor.absolute = anchorValue - CheckSwitchScreen.instance.cutoutHeight;
            }
        }
        uiWidget.UpdateAnchors();
    }

    void Update()
    {
        if (CheckSwitchScreen.instance.liuHai && screenOrientationKind != Screen.orientation)
        {
            
            screenOrientationKind = Screen.orientation;
            InitView();
        }
    }
}
