using XLua;
﻿using UnityEngine;
using System.Collections;


[RequireComponent(typeof(UIAnchor))]
[Hotfix]
public class UIAnchorUpdate : MonoBehaviour
{

    int SetCount = 10;
    IEnumerator Start()
    {
        while (SetCount > 0)
        {
            GetComponent<UIAnchor>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            SetCount--;
        }
    }


}

