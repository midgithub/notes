using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Hotfix]
public class PointMapArea : MonoBehaviour {

    Dictionary<int, Vector3> points = new Dictionary<int, Vector3>();

    /// <summary>
    /// 添加关联
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pot"></param>
    public void SetArea(int index, Vector3 pot)
    {
        points[index] = pot;
    }
    /*
    /// <summary>
    /// 清除关联
    /// </summary>
    /// <param name="index"></param>
    public void ClearArea(int index)
    {
        if (points.ContainsKey(index))
        {
            points.Remove(index);
        }
    }
    */
    public void ClearAll()
    {
        points.Clear();
    }

    void OnDrawGizmos()
    {
        if (points.Count == 0)
        {
            return;
        }
        Gizmos.color = Color.red;
      //  Vector3 curPoint = Vector3.zero;
        foreach (var item in points)
        {
            Gizmos.DrawLine(item.Value, this.transform.position);
            //   curPoint = item;
        }
        //  Vector3 startV = points2[1][0];
        //  Vector3 endV = points2[1][points2[1].Count - 1];
        //  Gizmos.DrawLine(startV, endV);

    }
}

