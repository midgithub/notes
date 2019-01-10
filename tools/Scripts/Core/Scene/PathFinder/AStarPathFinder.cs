using XLua;
﻿using UnityEngine;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class AStarPathFinder : IPathFinder
    {
        public bool IsBlocked(Vector3 position)
        {
            return SceneDataMgr.Instance.IsBlocked(position);
        }

        public List<Vector3> FindPathByAStar(Vector3 srcPosition, Vector3 destPosition)
        {
            Node srcNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(srcPosition);
            Node destNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(destPosition);

            List<Vector3> path = new List<Vector3>();
            if (!SceneDataMgr.Instance.SceneGrid.HasBarrier(srcNode, destNode))//无阻挡无需寻路，直接到达
            {
                float x = destNode.position.x;
                float z = destNode.position.y;
                float y = GetTerrainHeight(x, z);

                path.Add(new Vector3(x, y, z));

                return path;
            }

            //A*寻路
            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(srcNode);

            int curIndex = 0;
            Node curNode = null;
            bool bFind = false;
            int cnt = 0;
            while (openSet.Count > 0)
            {
                cnt++;
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

                if (curNode == destNode)
                {
                    bFind = true;

                    break;
                }

                List<Node> surroundNodes = SceneDataMgr.Instance.SceneGrid.GetSurroundNodes(curNode);
                for (int i = 0; i < surroundNodes.Count; i++)
                {
                    Node node = surroundNodes[i];
                    if (closeSet.Contains(node))
                        continue;
                    if (!node.canWalk)
                    {
                        continue;
                    }

                    float gCost = curNode.gCost + SceneDataMgr.Instance.SceneGrid.GetDistance(curNode, node);
                    bool noInOpen = !openSet.Contains(node);
                    if (noInOpen || gCost < node.gCost)
                    {
                        node.gCost = gCost;
                        node.hCost = SceneDataMgr.Instance.SceneGrid.GetDistance(node, destNode);
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
            List<Node> nodes = GeneratePath(srcNode, curNode);
            for (int i = 0; i < nodes.Count; i++)
            {
                float x = nodes[i].position.x;
                float z = nodes[i].position.y;
                float y = GetTerrainHeight(x, z);

                path.Add(new Vector3(x, y, z));
            }
            
            return path;
        }

        public Vector3 LineSegmentDetection(Vector3 segmentStart, Vector3 segmentEnd, ref bool isSlide)
        {
            return SceneDataMgr.Instance.LineSegementDection(segmentStart, segmentEnd, true, ref isSlide);
        }
        private Ray ray = new Ray();
        public float GetTerrainHeight(Vector2 xzPosition)
        {            
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

        #region 寻路点
        public List<Node> GeneratePath(Node srcNode, Node destNode)
        {
            List<Node> pathNodes = new List<Node>();
            Node tmp = destNode;

            int cnt = 0;
            while (tmp != srcNode)
            {
                pathNodes.Add(tmp);

                tmp = tmp.parent;

                if (tmp == null)
                {
                    break;
                }

                if (pathNodes.Contains(tmp))
                {
                    break;
                }

                cnt++;
                if (cnt > 100000)
                {
                    break;
                }
            }

            pathNodes.Reverse();

            List<Node> actualPath = new List<Node>();
            actualPath.Add(srcNode);
            for (int i = 0; i < pathNodes.Count; i++)
            {
                if (!pathNodes[i].canWalk)
                {
                    break;
                }
                else
                {
                    actualPath.Add(pathNodes[i]);
                }
            }

            return FloydSmooth(actualPath);
        }
        #endregion
        #region 路线平滑
        private List<Node> FloydSmooth(List<Node> srcPath)
        {
            //直线处理
            int len = srcPath.Count;
            if (len > 2)
            {
                FloydVector vector = new FloydVector(0, 0);
                FloydVector tmpVct = new FloydVector(0, 0);

                CalFloydVector(vector, srcPath[len - 1], srcPath[len - 2]);

                for (int i = len - 3; i >= 0; i--)
                {
                    CalFloydVector(tmpVct, srcPath[i + 1], srcPath[i]);

                    if (vector.x == tmpVct.x && vector.y == tmpVct.y)
                    {
                        srcPath.RemoveAt(i + 1);
                    }
                    else
                    {
                        vector.x = tmpVct.x;
                        vector.y = tmpVct.y;
                    }
                }
            }

            //消除拐点
            len = srcPath.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (!SceneDataMgr.Instance.SceneGrid.HasBarrier(srcPath[i], srcPath[j]))
                    {
                        for (int k = i - 1; k > j; k--)
                        {
                            srcPath.RemoveAt(k);
                        }

                        i = j;
                        len = srcPath.Count;

                        break;
                    }
                }
            }

            return srcPath;
        }

        private void CalFloydVector(FloydVector vector, Node node1, Node node2)
        {
            vector.x = node1.x - node2.x;
            vector.y = node1.y - node2.y;
        }

[Hotfix]
        class FloydVector
        {
            public int x;
            public int y;

            public FloydVector(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        #endregion

        #region Jump Point Search
        public List<Vector3> FindPath(Vector3 srcPosition, Vector3 destPosition)
        {
            Node srcNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(srcPosition);
            Node destNode = SceneDataMgr.Instance.SceneGrid.GetNodeByPosition(destPosition);

            if (!destNode.canWalk)
            {
                return null;
            }

            List<Vector3> path = new List<Vector3>();

            if (!SceneDataMgr.Instance.SceneGrid.HasBarrier(srcNode, destNode))//无阻挡无需寻路，直接到达
            {
                float x = destNode.position.x;
                float z = destNode.position.y;
                float y = GetTerrainHeight(x, z);

                path.Add(new Vector3(x, y, z));

                return path;
            }

            //JPS寻路
            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();

            openSet.Add(srcNode);

            int curIndex = 0;
            Node curNode = null;
            bool bFind = false;
            int cnt = 0;
            while (openSet.Count > 0)
            {
                cnt++;
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

                if (curNode == destNode)
                {
                    bFind = true;

                    break;
                }

                List<Node> jumpNodes = SceneDataMgr.Instance.SceneGrid.JPSSearch(curNode, srcNode, destNode);
                for (int i = 0; i < jumpNodes.Count; i++)
                {
                    Node node = jumpNodes[i];
                    node.gCost = curNode.gCost + SceneDataMgr.Instance.SceneGrid.GetDistance(curNode, node);
                    node.hCost = SceneDataMgr.Instance.SceneGrid.GetDistance(node, destNode);
                    
                    openSet.Add(node);
                }
            }

            if (!bFind)
            {
                return path;
            }
            List<Node> nodes = GeneratePath(srcNode, curNode);
            for (int i = 0; i < nodes.Count; i++)
            {
                float x = nodes[i].position.x;
                float z = nodes[i].position.y;
                float y = GetTerrainHeight(x, z);

                path.Add(new Vector3(x, y, z));
            }

            return path;
        }
        #endregion
    }
}

