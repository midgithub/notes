using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[Hotfix]
public class CommonProgressSlider : MonoBehaviour {


    public Image Image;

    bool bMove = false;
    float startValue = 0;  //起始百分比
    float endValue = 0;  //结束百分比
    float Speed = 0;    // 百分比/秒
    float interal = 2f;  //动画时间


    /// <summary>
    /// 起始百分比，结束百分比
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void SetValue(float start,float end)
    {
      //  Debug.LogError("    start  "+start  +   "     end  " +end);
        startValue = start;
        endValue = end;
        Speed = Mathf.Abs((endValue - startValue) / interal);
        bMove = true;
    }

    private void OnDisable()
    {
        bMove = false;
    }
    // Update is called once per frame
    void Update () {
	    if(bMove)
        {
            if(startValue < endValue)
            {
                float mv = Speed * Time.deltaTime;
                startValue += mv;
                Image.fillAmount = startValue;
            }
            else
            {
                Image.fillAmount = endValue;
                bMove = false;
            }
        }
	}
}

