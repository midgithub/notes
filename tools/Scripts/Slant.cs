/**
* @file     : Slant.cs
* @brief    : 
* @details  : 显示倾斜效果。
* @author   : XuXiang
* @date     : 2017-06-15 11:08
*/

using XLua;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UI/Effects/Slant")]
[Hotfix]
public class Slant : BaseMeshEffect
{
    /// <summary>
    /// 倾斜程度。
    /// </summary>
    public float Ratio = 1;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
        {
            return;
        }

        List<UIVertex> vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);
        int count = vertexList.Count;
        for (int i=0; i< count; ++i)
        {
            UIVertex v = vertexList[i];
            Vector3 p = v.position;
            p.x += p.y * Ratio;     //X按Y高度进行偏移
            v.position = p;
            vertexList[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }
}

