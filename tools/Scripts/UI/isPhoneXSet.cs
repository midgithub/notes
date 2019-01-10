using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class isPhoneXSet : MonoBehaviour
{

    public bool isPhoneX = false;
    public int leftright = 1;
    public float retatime = 0.2f;
    IEnumerator Start()
    {

        float org = (float)Screen.width / (float)Screen.height;
        if (org > 2) isPhoneX = true;
        yield return new WaitForSeconds(retatime);
        if (!isPhoneX) yield break;
        // Debug.LogError(org + "==Screen.height==" + Screen.height + "==Screen.width==" + Screen.width);
        RectTransform rc = transform.GetComponent<RectTransform>();
        if (rc.localPosition.x > 0) leftright = -1;
        float rx = rc.anchoredPosition.x;
        rc.anchoredPosition = new Vector2(rx + leftright * 48, rc.anchoredPosition.y);
        // Debug.LogError(rc.localPosition.x);
    }

}

