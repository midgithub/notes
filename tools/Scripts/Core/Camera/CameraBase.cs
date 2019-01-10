using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
     
//Camer
[Hotfix]
public abstract  class CameraBase : MonoBehaviour
{
    /*
     * 是否振动
     */
    public bool m_cameraShake = false;

    /*
        * 是否拉近拉远 
        */
    public bool m_cameraZoom = false;

    /*
     * 纯虚函数
     */
    public abstract Transform UpdateMainCameraTransform();

    /// <summary>
    /// 是否关闭摄像机震动
    /// </summary>
    public virtual bool IsDisableCameraShake()
    {
        return false;
    }
}

}; //end SG

