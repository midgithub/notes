using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using XLua;
using SG;

[Hotfix]
public class CommonProgress : MonoBehaviour {

    [SerializeField]
    Image sliderImage;

    private static CommonProgress instance = null;
    public static CommonProgress Instance
    {
        get
        {
            if (null == instance)
                instance = new CommonProgress();

            return instance;
        }
    }


    float delTime = 0;
    DateTime dt;

    public void SetInfo(float time)
    {
        delTime = time /1000;
        dt = DateTime.Now;
    }	
	
	// Update is called once per frame
	void Update () {
	    if(delTime > 0)
        {
            double startTime = (DateTime.Now - dt).TotalSeconds;
            if (startTime <= delTime)
            {
                double value = startTime / delTime;
                sliderImage.fillAmount = (float)value;
            }else
            {        
                delTime = 0;
                EventParameter par = EventParameter.Get();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CC_SliderTimeOver, par);
            }
          
        }
	}
}

