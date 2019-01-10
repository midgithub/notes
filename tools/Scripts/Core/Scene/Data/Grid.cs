/**
* @file     : Grid.cs
* @brief    : 地图网格数据
* @details  : 地图网格数据
* @author   : CW
* @date     : 2017-06-20
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class Grid
    {
        public int width;

        public int height;

        public float nodeSize = 0.0f;
        private float reciprocalSize;

        public Node[,] gridData;

        public Grid(int w, int h, float size)
        {
            width = w;
            height = h;
            nodeSize = size;
            reciprocalSize = 1.0f / nodeSize;

            gridData = new Node[width, height];
        }

        /// <summary>
        /// 根据世界坐标获取最近点
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Node GetNodeByPosition(Vector3 position)
        {
            return GetNodeByPosition(new Vector2(position.x, position.z));
        }

        /// <summary>
        /// 根据二维坐标获取最近点
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Node GetNodeByPosition(Vector2 position)
        {
            int x = (int)(position.x * reciprocalSize);
            int y = (int)(position.y * reciprocalSize);

            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);

            Node node = gridData[x, y];

            return node;
        }

        /// <summary>
        /// 获取当前点周围八个方向的点
        /// </summary>
        /// <param name="centerNode"></param>
        /// <returns></returns>
        public List<Node> GetSurroundNodes(Node centerNode)
        {
            List<Node> nodes = new List<Node>();
            int x, y;
            for (int i = -1; i < 2; i++)
            {
                x = centerNode.x + i;
                if (x < 0 || x >= width)
                    continue;

                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    y = centerNode.y + j;

                    if (y < 0 || y >= height)
                        continue;

                    nodes.Add(gridData[x, y]);
                }
            }

            return nodes;
        }

        /// <summary>
        /// 获取两个点的距离
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public float GetDistance(Node a, Node b)
        {
            return Vector2.Distance(a.position, b.position);
        }

        /// <summary>
        /// 曼哈顿估值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int ManhattanCost(Node a, Node b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
        
        /// <summary>
        /// 判断两点之间是否有阻挡物
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool HasBarrier(Node a, Node b)
        {
            if (a == b)
                return false;

            int distX = Mathf.Abs(a.x - b.x);
            int distY = Mathf.Abs(a.y - b.y);

            LineFormula formula;
            float start, end;

            List<Node> nodes = null;

            if (distX > distY)
            {
                formula = new LineFormula(a, b, 0);

                start = Mathf.Min(a.position.x, b.position.x);
                end = Mathf.Max(a.position.x, b.position.x);

                for (float i = start + nodeSize; i <= end; i += nodeSize)
                {
                    float yPos = formula.CalResult(i);
                    nodes = GetNodesNearPoint(0, formula.GetAFlag(), i, yPos);

                    for (int j = 0; j < nodes.Count; j++)
                    {
                        if (!nodes[j].canWalk)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                formula = new LineFormula(a, b, 1);

                start = Mathf.Min(a.position.y, b.position.y);
                end = Mathf.Max(a.position.y, b.position.y);

                for (float i = start + nodeSize; i <= end; i += nodeSize)
                {
                    float xPos = formula.CalResult(i);
                    nodes = GetNodesNearPoint(1, formula.GetAFlag(), xPos, i);

                    for (int j = 0; j < nodes.Count; j++)
                    {
                        if (!nodes[j].canWalk)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 根据二维左边获取附近的格子点
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        private List<Node> GetNodesNearPoint(int flag, int aflag, float posX, float posY)
        {
            List<Node> nodes = new List<Node>();

            if (null == gridData)
                return nodes;

            int xInt = (int)(posX * 10);
            int yInt = (int)(posY * 10);
            int sInt = (int)(nodeSize * 10);

            bool xIsInt = (xInt % sInt == 0);
            bool yIsInit = (yInt % sInt == 0);

            if (xIsInt && yIsInit)
            {
                int x = (int)(posX * reciprocalSize);
                int y = (int)(posY * reciprocalSize);

                if (flag == 0)
                {
                    if (aflag == -1)
                    {
                        y = y - 1;
                    }
                }
                else if (flag == 1)
                {
                    if (aflag == -1)
                    {
                        x = x - 1;
                    }
                }

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    nodes.Add(gridData[x, y]);
                }
            }
            else
            {
                int x = (int)(posX * reciprocalSize);
                int y = (int)(posY * reciprocalSize);
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    nodes.Add(gridData[x, y]);
                }
            }

            return nodes;
        }

        #region Jump Points Search
        /// <summary>
        /// JPS沿邻居方向搜索所有的跳跃点
        /// </summary>
        /// <param name="curNode"></param>
        /// <param name="startNode"></param>
        /// <param name="destNode"></param>
        /// <returns>跳跃点</returns>
        public List<Node> JPSSearch(Node curNode, Node startNode, Node destNode)
        {
            List<Node> jumpNodes = new List<Node>();
            List<Node> neighbors = JPSNeighbors(curNode, startNode);
            for (int i = 0; i < neighbors.Count; i++)
            {
                int dirX = neighbors[i].x - curNode.x;
                int dirY = neighbors[i].y - curNode.y;

                Node jumpNode = JPSJump(curNode, dirX, dirY, startNode, destNode);
                if (null != jumpNode)
                {
                    jumpNodes.Add(jumpNode);
                }
            }

            return jumpNodes;
        }

        /// <summary>
        /// JPS寻找直接邻居节点和被迫邻居节点
        /// </summary>
        /// <param name="current"></param>
        /// <param name="startNode"></param>
        /// <returns>邻居点</returns>
        public List<Node> JPSNeighbors(Node current, Node startNode)
        {
            if (current == startNode)
            {
                return GetSurroundNodes(current);
            }

            List<Node> neighbors = new List<Node>();
            Node parent = current.parent;

            int dirX = current.x - parent.x;
            int dirY = current.y - parent.y;

            if (0 == dirX)
            {
                int x = current.x;
                int y = current.y + dirY;
                if (y >= 0 && y < height)
                {
                    neighbors.Add(gridData[x, y]);
                }

                x = current.x + 1;
                y = current.y;
                if (x < width)
                {
                    Node node = gridData[x, y];
                    if(!node.canWalk)
                    {
                        y = current.y + dirY;
                        if(y >= 0 && y < height)
                        {
                            neighbors.Add(gridData[x, y]);
                        }
                    }
                }

                x = current.x - 1;
                y = current.y;
                if (x >= 0)
                {
                    Node node = gridData[x, y];
                    if(!node.canWalk)
                    {
                        y = current.y + dirY;
                        if (y >= 0 && y < height)
                        {
                            neighbors.Add(gridData[x, y]);
                        }
                    }
                }
            }
            else if (0 == dirY)
            {
                int x = current.x + dirX;
                int y = current.y;
                if (x >= 0 && x < width)
                {
                    neighbors.Add(gridData[x, y]);
                }

                x = current.x;
                y = current.y + 1;
                if (y < height)
                {
                    Node node = gridData[x, y];
                    if(!node.canWalk)
                    {
                        x = current.x + dirX;
                        if (x >= 0 && x < width)
                        {
                            neighbors.Add(gridData[x, y]);
                        }
                    }
                }

                x = current.x;
                y = current.y - 1;
                if (y >= 0)
                {
                    Node node = gridData[x, y];
                    if(!node.canWalk)
                    {
                        x = current.x + dirX;
                        if (x >= 0 && x < width)
                        {
                            neighbors.Add(gridData[x, y]);
                        }
                    }
                }
            }
            else
            {
                int x = current.x + dirX;
                int y = current.y + dirY;
                bool bX = (x >= 0 && x < width);
                bool bY = (y >= 0 && y < height);

                if (bX)
                {
                    neighbors.Add(gridData[x, current.y]);
                }
                if(bY)
                {
                    neighbors.Add(gridData[current.x, y]);
                }
                if (bX && bY)
                {
                    neighbors.Add(gridData[x, y]);
                }

                x = current.x;
                y = parent.y;
                Node node = gridData[x, y];
                if (!node.canWalk)
                {
                    x = current.x + dirX;
                    if (bX)
                    {
                        neighbors.Add(gridData[x, y]);
                    }
                }

                x = parent.x;
                y = current.y;
                node = gridData[x, y];
                if (!node.canWalk)
                {
                    y = current.y + dirY;
                    if (bY)
                    {
                        neighbors.Add(gridData[x, y]);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// JPS查找跳跃点
        /// </summary>
        /// <param name="curNode"></param>
        /// <param name="dirX"></param>
        /// <param name="dirY"></param>
        /// <param name="startNode"></param>
        /// <param name="destNode"></param>
        /// <returns>跳跃点</returns>
        public Node JPSJump(Node curNode, int dirX, int dirY, Node startNode, Node destNode)
        {
            int x = curNode.x + dirX;
            int y = curNode.y + dirY;

            Node nextNode = gridData[x, y];
            nextNode.parent = curNode;
            if (!nextNode.canWalk)
            {
                return null;
            }

            if (nextNode.x == destNode.x && nextNode.y == destNode.y)
            {
                return nextNode;
            }

            if (dirX != 0 && dirY != 0)
            {
                if (HasDiagonalForcedNeigb(nextNode, dirX, dirY))
                {
                    return nextNode;
                }

                if (JPSJump(nextNode, dirX, 0, startNode, destNode) != null ||
                    JPSJump(nextNode, 0, dirY, startNode, destNode) != null)
                    return nextNode;
            }
            else
            {
                if (dirX != 0)
                {
                    if (HasHorizontalForcedNeigb(nextNode, dirX))
                    {
                        return nextNode;
                    }
                }

                if (dirY != 0)
                {
                    if (HasVerticalForcedNeigb(nextNode, dirY))
                    {
                        return nextNode;
                    }
                }
            }

            return JPSJump(nextNode, dirX, dirY, startNode, destNode);
        }

        /// <summary>
        /// JPS对角线查找被迫邻居节点
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dirX"></param>
        /// <param name="dirY"></param>
        /// <returns></returns>
        public bool HasDiagonalForcedNeigb(Node current, int dirX, int dirY)
        {
            int x = current.x;
            int y = current.y - dirY;
            Node node = gridData[x, y];
            if (!node.canWalk)
            {
                x = current.x + dirX;
                if (x >= 0 && x < width)
                {
                    Node checkNode = gridData[x, y];
                    if (checkNode.canWalk)
                    {
                        return true;
                    }
                }
            }

            x = current.x - dirX;
            y = current.y;
            node = gridData[x, y];
            if (!node.canWalk)
            {
                y = current.x + dirY;
                if (y >= 0 && y < height)
                {
                    Node checkNode = gridData[x, y];
                    if (checkNode.canWalk)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// JPS水平查找被迫邻居节点
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dirX"></param>
        /// <returns></returns>
        public bool HasHorizontalForcedNeigb(Node current, int dirX)
        {
            int x = current.x;
            int y = current.y + 1;
            if (y < height)
            {
                Node node = gridData[x, y];
                if (!node.canWalk)
                {
                    x = current.x + dirX;
                    if (x >= 0 && x < width)
                    {
                        Node checkNode = gridData[x, y];
                        if (checkNode.canWalk)
                        {
                            return true;
                        }
                    }
                }
            }

            x = current.x;
            y = current.y - 1;
            if (y >= 0)
            {
                Node node = gridData[x, y];
                if (!node.canWalk)
                {
                    x = current.x + dirX;
                    if (x >= 0 && x < width)
                    {
                        Node checkNode = gridData[x, y];
                        if (checkNode.canWalk)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// JPS垂直查找被迫邻居节点
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dirY"></param>
        /// <returns></returns>
        public bool HasVerticalForcedNeigb(Node current, int dirY)
        {
            int x = current.x + 1;
            int y = current.y;
            if (x < width)
            {
                Node node = gridData[x, y];
                if (!node.canWalk)
                {
                    y = current.y + dirY;
                    if (y >= 0 && y < width)
                    {
                        Node checkNode = gridData[x, y];
                        if (checkNode.canWalk)
                        {
                            return true;
                        }
                    }
                }
            }

            x = current.x - 1;
            y = current.y;
            if (x >= 0)
            {
                Node node = gridData[x, y];
                if (!node.canWalk)
                {
                    y = current.y + dirY;
                    if (y >= 0 && y < width)
                    {
                        Node checkNode = gridData[x, y];
                        if (checkNode.canWalk)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// 直线计算公式
    /// </summary>
[Hotfix]
    class LineFormula
    {
        private float a;
        private float b;
        //private int type;

        public LineFormula(Node start, Node end, int type)
        {
            //this.type = type;

            if (start.position.x == end.position.x)
            {
                if (type == 1)
                {
                    a = 0;
                    b = start.position.x;
                }
                else
                {
                    LogMgr.LogError("LineFormula error");
                }
            }
            else if (start.position.y == end.position.y)
            {
                if (type == 0)
                {
                    a = 0;
                    b = start.position.y;
                }
                else
                {
                    LogMgr.LogError("LineFormula error");
                }
            }
            else
            {
                if (type == 0)
                {
                    a = (end.position.y - start.position.y) / (end.position.x - start.position.x);
                    b = start.position.y - a * start.position.x;
                }
                else if (type == 1)
                {
                    a = (end.position.x - start.position.x) / (end.position.y - start.position.y);
                    b = start.position.x - a * start.position.y;
                }

            }
        }

        public float CalResult(float input)
        {
            return a * input + b;
        }

        public int GetAFlag()
        {
            if(a > 1e-5f)
            {
                return 1;
            }
            else if(a < -1e-5f)
            {
                return -1;
            }

            return 0;
        }
    }
}

