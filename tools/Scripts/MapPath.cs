/**
* @file     : MapLine.cs
* @brief    : 绘制地图路线
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-29 09:52
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class MapPath : BaseMeshEffect
    {
        public float PointSize = 20;
        public float MiddleRatio = 0.8f;        //中间点缩放
        public float MaxGap = 30;               //最大间隔
        public List<Vector2> PointList = new List<Vector2>();

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            float hs = PointSize / 2;
            float ms = hs * MiddleRatio;
            float gap = Math.Max(Math.Max(5, ms), MaxGap);
            List <UIVertex> allv = new List<UIVertex>();         //所有顶点
            List<UIVertex> pointv = new List<UIVertex>();       //一个路径点的数据
            vh.GetUIVertexStream(pointv);            
            for (int i=0; i< PointList.Count; ++i)
            {
                Vector2 p = PointList[i];
                AddPointVertex(p, hs, pointv, allv);

                //如果需要绘制中间点
                if (ms > 0.0001f && i < PointList.Count - 1)
                {
                    Vector2 next = PointList[i + 1];
                    float dis = Vector2.Distance(p, next);
                    float n = dis / gap;
                    float yn = n - (int)n;
                    for (int j=0; j<n; ++j)
                    {
                        float t = (j + yn) * 1.0f / (n + 1);
                        Vector2 mp = Vector2.Lerp(p, next, t);
                        AddPointVertex(mp, ms, pointv, allv);
                    }
                    //Vector2 mp = (p + PointList[i + 1]) / 2;
                    //AddPointVertex(mp, ms, pointv, allv);
                }
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(allv);
        }

        private static Vector2[] CacheVertexPosition = new Vector2[6];      //缓存一个点的顶点位置

        /// <summary>
        /// 添加一个点的信息。
        /// </summary>
        /// <param name="p">点的坐标。</param>
        /// <param name="r">显示半径。</param>
        /// <param name="pointv">要显示一个点的UIV列表。</param>
        /// <param name="allv">最终显示的UIV列表。</param>
        public static void AddPointVertex(Vector2 p, float r, List<UIVertex> pointv, List<UIVertex> allv)
        {
            //计算顶点坐标
            CacheVertexPosition[0] = new Vector2(p.x - r, p.y - r);
            CacheVertexPosition[1] = new Vector2(p.x - r, p.y + r);
            CacheVertexPosition[2] = new Vector2(p.x + r, p.y + r);
            CacheVertexPosition[3] = CacheVertexPosition[2];
            CacheVertexPosition[4] = new Vector2(p.x + r, p.y - r);
            CacheVertexPosition[5] = CacheVertexPosition[0];

            //添加到顶点列表
            for (int j = 0; j < CacheVertexPosition.Length; ++j)
            {
                UIVertex uiv = pointv[j];
                uiv.position = CacheVertexPosition[j];
                allv.Add(uiv);
            }
        }

        public void ClearPath()
        {
            PointList.Clear();
            this.graphic.SetVerticesDirty();
        }

        public void SetPath(List<Vector2> path)
        {
            PointList = path;
            this.graphic.SetVerticesDirty();
        }

        public void SetPathForLua(LuaTable tb)
        {
            List<Vector2> path = tb.Cast<List<Vector2>>();
            SetPath(path);
        }
    }
}

