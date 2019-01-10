/**
* @file     : WayPointPathFinder.cs
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
    public class WayPointPathFinder : IPathFinder
    {
        public bool IsBlocked(Vector3 position)
        {
            return SceneDataMgr.Instance.IsBlocked(position);
        }

        public System.Collections.Generic.List<Vector3> FindPath(Vector3 srcPosition, Vector3 destPosition)
        {
            Node srcNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(srcPosition);
            Node destNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(destPosition);

            if (!destNode.canWalk)
            {
                return null;
            }

            List<Vector3> path = new List<Vector3>();
            Vector3 destPos = new Vector3(destNode.position.x, GetTerrainHeight(destNode.position.x, destNode.position.y), destNode.position.y);

            if (!SceneDataMgr.Instance.SceneGrid.HasBarrier(srcNode, destNode))//无阻挡无需寻路，直接到达
            {
                path.Add(destPos);

                return path;
            }

            WayPoint scrPoint = SceneDataMgr.Instance.SceneWayPoint.GetNearPoint(srcPosition);
            WayPoint destPoint = SceneDataMgr.Instance.SceneWayPoint.GetNearPoint(destPosition);
            if (null == scrPoint || null == destPoint)
            {
                return null;
            }

            List<WayPoint> openSet = new List<WayPoint>();
            HashSet<WayPoint> closeSet = new HashSet<WayPoint>();

            openSet.Add(scrPoint);

            int curIndex = 0;
            WayPoint curNode = null;
            bool bFind = false;
            while (openSet.Count > 0)
            {
                curNode = openSet[0];
                curIndex = 0;

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost <= curNode.fCost && openSet[i].hCost < curNode.hCost)
                    {
                        curNode = openSet[i];
                        curIndex = i;
                    }
                }

                openSet.RemoveAt(curIndex);
                closeSet.Add(curNode);

                if (curNode == destPoint)
                {
                    bFind = true;

                    break;
                }

                List<WayPoint> surroundNodes = SceneDataMgr.Instance.SceneWayPoint.GetNeighbours(curNode);
                for (int i = 0; i < surroundNodes.Count; i++)
                {
                    WayPoint node = surroundNodes[i];
                    if (closeSet.Contains(node))
                        continue;

                    float gCost = curNode.gCost + SceneDataMgr.Instance.SceneWayPoint.GetDistance(curNode, node);
                    bool noInOpen = !openSet.Contains(node);
                    if (noInOpen || gCost < node.gCost)
                    {
                        node.gCost = gCost;
                        node.hCost = SceneDataMgr.Instance.SceneWayPoint.GetDistance(node, destPoint);
                        node.parent = curNode;

                        if (noInOpen)
                        {
                            openSet.Add(node);
                        }
                    }
                }
            }

            if (!bFind)
            {
                return path;
            }

            List<WayPoint> nodes = GeneratePath(scrPoint, destPoint);

            if (nodes.Count > 1)
            {
                WayPoint tmpPoint = nodes[1];
                Node tmpNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(tmpPoint.pos);
                if (!SceneDataMgr.Instance.SceneGrid.HasBarrier(srcNode, tmpNode))
                {
                    nodes.RemoveAt(0);
                }
            }
            if (nodes.Count > 1)
            {
                WayPoint tmpPoint = nodes[nodes.Count - 2];
                Node tmpNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(tmpPoint.pos);
                if (!SceneDataMgr.Instance.SceneGrid.HasBarrier(tmpNode, destNode))
                {
                    nodes.RemoveAt(nodes.Count - 1);
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                float x = nodes[i].pos.x;
                float z = nodes[i].pos.y;
                float y = GetTerrainHeight(x, z);

                path.Add(new Vector3(x, y, z));
            }
            path.Add(destPos);

            return path;
        }

        public Vector3 LineSegmentDetection(Vector3 segmentStart, Vector3 segmentEnd, ref bool bSlider)
        {
            return SceneDataMgr.Instance.LineSegementDection(segmentStart, segmentEnd, true, ref bSlider);
        }
        private Ray ray = new Ray();
        public float GetTerrainHeight(Vector2 xzPosition)
        {
            //Ray ray = new Ray();
            ray.origin = new Vector3(xzPosition.x, 10000.0f, xzPosition.y);
            ray.direction = new Vector3(0, -1, 0);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("ground")))
            {
                return hitInfo.point.y;
            }

            return 0;
        }

        public float GetTerrainHeight(float x, float y)
        {
            return GetTerrainHeight(new Vector2(x, y));
        }

        public List<WayPoint> GeneratePath(WayPoint srcNode, WayPoint destNode)
        {
            List<WayPoint> pathNodes = new List<WayPoint>();
            WayPoint tmp = destNode;

            while (tmp != srcNode)
            {
                pathNodes.Add(tmp);

                tmp = tmp.parent;
            }
            pathNodes.Add(srcNode);

            pathNodes.Reverse();

            return pathNodes;
        }
    }
}

