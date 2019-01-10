using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[Hotfix]
public class LevelUpGuildTouch : MonoBehaviour{

    bool bClose = false;

	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (bClose)
        {
            return;
        }
        bClose = IsTouching();
        if (bClose)
        {
            CloseTk();
        }
    }

    void CloseTk()
    {
        if(this != null)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 检测屏幕点是否在tag = levelupGuild上面
    /// </summary>
    /// <param name="pos">屏幕位置</param>
    /// <returns></returns>
    private bool IsPositionOverUI(Vector2 pos)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(pos.x, pos.y);
        EventSystem.current.RaycastAll(eventData, results);
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.CompareTag("levelupGuild"))
            {
                return true;
            }
        }

        return false;
    }
    private bool IsTouching()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButton(0))
            {
                if(!IsPositionOverUI(Input.mousePosition))
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            return false;
        }

        int count = Input.touchCount;
        for (int i = 0; i < count; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (TouchPhase.Began == touch.phase)
            {
                if (!IsPositionOverUI(Input.mousePosition))
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            if (TouchPhase.Canceled != touch.phase && TouchPhase.Ended != touch.phase)
            {
                return true;
            }
        }
        return false;
    }
}

