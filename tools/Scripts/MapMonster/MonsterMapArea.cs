using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class MonsterMapArea : MonoBehaviour
{

    Dictionary<int, Vector3> points = new Dictionary<int, UnityEngine.Vector3>();  //圆 ， 存中心点

    Dictionary<int, List<Vector3>> points2 = new Dictionary<int, List<Vector3>>();   //多边形，存多个角
    //float r;

    /// <summary>
    /// 画区域 -- 圆
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pointOne"></param>
    /// <param name="pointTwo"></param>
    public void SetArea(int index, Vector3 pointOne, Vector3 pointTwo)
    {
        float dx = pointTwo.x - pointOne.x;
        float dy = pointTwo.y - pointOne.y;
        float dz = pointTwo.z - pointOne.z;

        //r = Mathf.Sqrt(dx * dx + (dz * dz)) / 2;
        Vector3 center = new Vector3((dx / 2) + pointOne.x, pointOne.y + (dy / 2) + 1f, (dz / 2) + pointOne.z);
        points[index] = center;
    }

    public void SetArea(int index, Vector3 pot)
    {
        if (!points2.ContainsKey(index))
        {
            points2[index] = new List<Vector3>();
        }
        if (!points2[index].Contains(pot))
        {
            points2[index].Add(pot);
        }
    }

    public void CleadLastPoint(int index)
    {
        if (points2.ContainsKey(index))
        {
            if (points2[index].Count > 0)
            {
                points2[index].RemoveAt(points2[index].Count - 1);
            }
        }
    }

    public void ClearList(int index)  //清除某个图形
    {
        if (points2.ContainsKey(index))
        {
            points2[index].Clear();
        }
    }
    void OnDrawGizmos()
    {
        //画多边形
        if (points2.Count == 0)
            return;
        if (points2[1].Count == 0)
            return;
        Gizmos.color = Color.red;
        Vector3 curPoint = Vector3.zero;
        foreach (var item in points2[1])
        {
            if (curPoint == Vector3.zero)
            {
                curPoint = item;
            }
            Gizmos.DrawLine(curPoint, item);
            curPoint = item;
        }
        Vector3 startV = points2[1][0];
        Vector3 endV = points2[1][points2[1].Count - 1];
        Gizmos.DrawLine(startV, endV);
    }
}

