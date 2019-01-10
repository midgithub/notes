/**
* @file     : cameralookatPos.cs
* @brief    : look at a postion
* @details  : 
* @author   : blisswang
* @date     : 2015-02-03
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class CameraLookatPos : MonoBehaviour
    {
	    // Use this for initialization
	    void Start ()
	    {
	
	    }

        public void SetCameraLookatPos(Vector3 vCamera, Vector3 vTarget)
        {            
            this.transform.position = vCamera;
            this.transform.LookAt(vTarget);
        }
	
	    // Update is called once per frame
	    //void Update () 
	    //{	
	    //}
    }

};//End SG

