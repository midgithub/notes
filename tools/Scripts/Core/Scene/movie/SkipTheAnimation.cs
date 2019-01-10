using XLua;
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[Hotfix]
public class SkipTheAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    public bool IsSkiped = false;
    GameObject obj = null;
    public bool IsShowText = true;

    public void ShowTextIfHas(bool v)
    {
        if (obj != null)
        {
            obj.SetActive(v);
        }

        IsShowText = v;
    }

    void Skip()
    {
        IsSkiped = true;
        //跳过此段动画
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
           
            animator.Update(info.length);
           
        }
    }

    bool IsOverridedOffset = false;
    Vector3 PosOffset;
    public void SetPos(bool isOverrideOffset, Vector3 posOffset)
    {
        IsOverridedOffset = isOverrideOffset;
        PosOffset = posOffset;
    }

    int CurCount = 0;
    //int MaxCount = 2;
	// Update is called once per frame
	void Update () {

        if (IsShowText)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                CurCount++;
                if (CurCount == 1)
                {
                    if (obj == null)
                    {
                        Object oj = SG.CoreEntry.gResLoader.LoadResource("Animation/movie/skipText");
                        if(oj==null) return;

                        obj = Instantiate(oj) as GameObject;

                        Button but = obj.GetComponentInChildren<Button>();
                        if (but != null)
                        {
                            but.onClick.AddListener(Skip);
                        }

                        obj.transform.parent = gameObject.transform;

                        if (IsOverridedOffset == false)
                        {
                            obj.transform.localPosition = -gameObject.transform.localPosition;
                        }
                        else
                        {
                            obj.transform.localPosition = PosOffset;
                        }

                        obj.SetActive(IsShowText);

                    }
                }
                //if (CurCount >= MaxCount)
                //{
                //    Skip();
                //    CurCount = 0;
                //}
            }
        }
	}
}

