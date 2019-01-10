/**
* @file     : SceneEfx.cs
* @brief    : 
* @details  : 场景特效。没有挂点
* @author   : 
* @date     : 2014-10-23
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
public class SceneEfxPool : MonoBehaviour {
    
    public void Init(Vector3 pos, float destoryTime)
    {
        this.transform.position = pos;
        Invoke("AutoDestory", destoryTime);                
    }

	// Use this for initialization
    //void Start () 
    //{
	
    //}            

	// Update is called once per frame
	//void Update () {
	
	//}

    void AutoDestory()
    {
        CancelInvoke("AutoDestory");
        CoreEntry.gGameObjPoolMgr.Destroy(this.gameObject);
        //Destroy(this.gameObject);
    }
}

};  //end SG

