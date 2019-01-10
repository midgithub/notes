using XLua;
﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class SetRanderQueue : MonoBehaviour
{

    public int renderQueue = 3000;
    //public bool runOnlyOnce = true;
    public bool notLoop = true;


    public float delayTime = 0f; 

    void Start()
    {
        //Update();
    }

    public void SetQueue( int q )
    {
        renderQueue = q;
        OnEnable();
    }

    void OnEnable()
    {
        StartCoroutine(delayUpdateRanderQueue()); 

    }

    IEnumerator delayUpdateRanderQueue()
    {
        yield return new WaitForSeconds(delayTime);

        if (notLoop)
        {
            ChangeAllChild(renderQueue, transform); 
        }
        else
        {
            while (true )
            {
                ChangeAllChild(renderQueue, transform); 
                yield return new WaitForSeconds(0.1f);
            }
        }

    }

    public static void ChangeAllChild(int rg,Transform t)
    {
        if (t.GetComponent<Renderer>() != null && t.GetComponent<Renderer>().sharedMaterial != null)
        {
            t.GetComponent<Renderer>().sharedMaterial.renderQueue = rg;
        }
     
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i) ;
            ChangeAllChild(rg, child);
        }        
    }

#if UNITY_EDITOR
    // 在运行状态下设置了renderQueue,停止执行材质的renderQueue不会恢复，这里恢复一下
    // 只在编辑器状态下执行
    void OnDestroy()
    {
        ChangeAllChild(-1,transform);
    }
#endif

    //void Update()
    //{
    //    ChangeAllChild(renderQueue, transform);
    //    if ((Application.platform != RuntimePlatform.WindowsEditor || runOnlyOnce) && Application.isPlaying)
    //    {
    //        this.enabled = false;
    //    }
    // }
}

