/**
* @file     : Gradient.cs
* @brief    : 
* @details  : uGUI文本从上往下的颜色渐变
* @author   : XuXiang copy from langresser(http://blog.csdn.net/langresser_king/article/details/50158199)
* @date     : 2017-06-06
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using XLua;
namespace UI.Effects
{
    [AddComponentMenu("UI/Effects/Gradient")]
    [LuaCallCSharp]
[Hotfix]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private Color32
            topColor = Color.white;
        [SerializeField]
        private Color32
            bottomColor = Color.black;


        public void SetTopColor(Color v)
        {
            topColor = v;
            this.enabled = false;
            this.enabled = true;
        }
        public void SetBottomColor(Color v)
        {
            bottomColor = v;
            this.enabled = false;
            this.enabled = true;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            ApplyGradient(vertexList, 0, vertexList.Count);
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }

        private void ApplyGradient(List<UIVertex> vertexList, int start, int end)
        {
            if (vertexList.Count <= 0)
            {
                return;
            }
            start = Math.Max(0, Math.Min(start, vertexList.Count - 1));
            end = Math.Max(0, Math.Min(end, vertexList.Count));

            //算出最高和最低的Y值
            float bottomY = vertexList[0].position.y;
            float topY = vertexList[0].position.y;
            for (int i = start; i < end; ++i)
            {
                float y = vertexList[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }

            //根据每个顶点的Y值在最高和最低的范围内对颜色进行差值
            float uiElementHeight = topY - bottomY;
            for (int i = start; i < end; ++i)
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                vertexList[i] = uiVertex;
            }
        }
    }
}

