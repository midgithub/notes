/**
* @file     : WayPoints.cs
* @brief    : 
* @details  : 
* @author   : CW
* @date     : 2017-12-12
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class WayPoint
    {
        public Vector2 pos;
        public int id;
        public List<int> neighbours;
        public float gCost;
        public float hCost;
        public float fCost
        {
            get { return gCost + hCost; }
        }

        public WayPoint parent;

        public WayPoint(int id, Vector3 pos, List<int> neighbours)
        {
            this.id = id;
            this.pos = new Vector2(pos.x, pos.z);
            this.neighbours = neighbours;
        }
    }

[Hotfix]
    public class WayPoints
    {
        public List<WayPoint> wayPointsData;

        public void LoadWayPoints(int mapID)
        {
            wayPointsData = new List<WayPoint>();

            List<SceneEntitySet> entities = CoreEntry.gGameDBMgr.GetEnityConfigInfo(mapID);
            if (null == entities)
            {
                return;
            }

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].type == EntityConfigType.ECT_WAYPOINT)
                {
                    for (int j = 0; j < entities[i].entityList.Count; j++)
                    {
                        SceneEntityConfig data = entities[i].entityList[j];
                        WayPoint point = new WayPoint(data.index, data.position, data.neighbours);

                        wayPointsData.Add(point);
                    }
                }
            }
        }

        public WayPoint GetNearPoint(Vector2 pos)
        {
            if (wayPointsData.Count == 0)
            {
                return null;
            }

            WayPoint point = wayPointsData[0];
            float distance = Vector2.Distance(pos, point.pos);
            for (int i = 1; i < wayPointsData.Count; i++)
            {
                float f = Vector2.Distance(wayPointsData[i].pos, pos);
                if (f < distance)
                {
                    point = wayPointsData[i];
                    distance = f;
                }
            }

            return point;
        }

        public WayPoint GetNearPoint(Vector3 pos)
        {
            return GetNearPoint(new Vector2(pos.x, pos.z));
        }

        public List<WayPoint> GetNeighbours(WayPoint p)
        {
            List<WayPoint> points = new List<WayPoint>();
            for (int i = 0; i < wayPointsData.Count; i++)
            {
                for (int j = 0; j < p.neighbours.Count; j++)
                {
                    if (wayPointsData[i].id == p.neighbours[j])
                    {
                        points.Add(wayPointsData[i]);
                    }
                }
            }

            return points;
        }

        public float GetDistance(WayPoint a, WayPoint b)
        {
            return Vector2.Distance(a.pos, b.pos);
        }
    }
}

