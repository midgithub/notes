using XLua;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Hotfix]
public class MapObstacleGizmos : MonoBehaviour
{
    public byte[] MapPoints { get; set; }
    public int MapSizeX { get; set; }
    public int MapSizeZ { get; set; }

    public int X { get; set; }
    public int Z { get; set; }
    public float Size { get; set; }

    void OnDrawGizmos()
    {
        float y = gameObject.transform.position.y;

        // 画范围
        if(X > 0 && Z > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(X / 2f, y, Z / 2f), new Vector3(X, 1f, Z));
        }

        // 画行走面
        if(MapPoints != null)
        {
            int x = (int)(MapSizeX / Size);
            int z = (int)(MapSizeZ / Size);
            Gizmos.color = Color.green;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    if (MapPoints[j * x + i] == (byte)MapNode.PointType.pass)
                    {
                        Vector3 pos = new Vector3(i * Size, y, j * Size);
                        Vector3 p1 = new Vector3(pos.x - Size / 2f, pos.y, pos.z - Size);
                        Vector3 p2 = new Vector3(pos.x + Size / 2f, pos.y, pos.z + Size);
                        Gizmos.DrawLine(p1, p2);
                        Vector3 p3 = new Vector3(pos.x - Size / 2f, pos.y, pos.z + Size);
                        Vector3 p4 = new Vector3(pos.x + Size / 2f, pos.y, pos.z - Size);
                        Gizmos.DrawLine(p3, p4);
                    }
                }
            }
        }
    }

}

