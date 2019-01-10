/**
* @file     : SceneDataMgr.cs
* @brief    : 场景地图数据管理
* @details  : 阻挡，行走等地图数据管理
* @author   : CW
* @date     : 2017-06-12
*/
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using XLua;

namespace SG
{
[Hotfix]
    public class SceneDataMgr
    {
        private static SceneDataMgr instance = null;
        public static SceneDataMgr Instance
        {
            get
            {
                if (null == instance)
                    instance = new SceneDataMgr();

                return instance;
            }
        }

        private float nodeSize = 0;
        private float centerOffset = 0;
        private float reciprocalSize = 0.0f;
        public float NodeSize
        {
            get { return nodeSize; }
        }
        private Grid sceneGrid;
        public Grid SceneGrid
        {
            get { return sceneGrid; }
        }

        private WayPoints sceneWayPoint;
        public WayPoints SceneWayPoint
        {
            get { return sceneWayPoint; }
        }

        private bool bUserReviseDir = false;
        private Vector2 reviseDir = Vector2.zero;

        /// <summary>
        /// 加载地图数据
        /// </summary>
        /// <param name="mapID"></param>
        public void LoadSceneData(int mapID)
        {
            mapID = MapMgr.Instance.EnterMapId;

            LuaTable mapCfg = ConfigManager.Instance.Map.GetMapConfig(mapID);
            if (null == mapCfg)
            {
                sceneGrid = null;
                sceneWayPoint = null;

                return;
            }

            string filePath = @"Data/MapData/" + mapCfg.Get<string>("path");
            TextAsset fileAsset = CoreEntry.gResLoader.LoadTextAsset(filePath, LoadModule.AssetType.Txt);

            if (null == fileAsset)
            {
                sceneGrid = null;
                sceneWayPoint = null;

                return;
            }

            byte[] data = fileAsset.bytes;
            int offset = 0;

            uint fourcc = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);

            int version = BitConverter.ToInt32(data, offset);
            offset += sizeof(Int32);

            int mapId = BitConverter.ToInt32(data, offset);
            offset += sizeof(Int32);

            int width = BitConverter.ToInt16(data, offset);
            offset += sizeof(Int16);

            int height = BitConverter.ToInt16(data, offset);
            offset += sizeof(Int16);

            LogMgr.Log("LoadSceneData fourcc:{0} version:{1} mapId:{2} width:{3} height:{4}", fourcc, version, mapId, width, height);

            nodeSize = BitConverter.ToSingle(data, offset);
            offset += sizeof(Single);
            centerOffset = nodeSize * 0.5f;
            reciprocalSize = 1.0f / nodeSize;

            int w = (int)(width * reciprocalSize);
            int h = (int)(height * reciprocalSize);

            sceneGrid = new Grid(w, h, nodeSize);
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    byte _type = data[offset];
                    offset++;
                    MapNode.PointType nodeType = (MapNode.PointType)_type;
                    sceneGrid.gridData[i, j] = new Node(nodeType == MapNode.PointType.pass, nodeSize * i, nodeSize * j, i, j);
                }
            }

            sceneWayPoint = new WayPoints();
            sceneWayPoint.LoadWayPoints(mapID);
        }

        /// <summary>
        /// 判断一点是否是阻挡点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsBlocked(Vector3 pos)
        {
            if (null == sceneGrid)
                return false;

            int w = sceneGrid.width;
            int h = sceneGrid.height;

            int x = (int)(pos.x * reciprocalSize);
            int z = (int)(pos.z * reciprocalSize);

            if (x >= w || x < 0)
            {
                return true;
            }

            if (z >= h || z < 0)
            {
                return true;
            }

            Node node = sceneGrid.gridData[x, z];
            return !node.canWalk;
        }

        /// <summary>
        /// 取该目标点附近的非阻挡点。
        /// </summary>
        /// <param name="from">场景编号。</param>
        /// <param name="from">起点。</param>
        /// <param name="to">终点。</param>
        /// <param name="dst">可移动的目标点。</param>
        /// <param name="r">附近半径。</param>
        /// <returns></returns>
        public bool GetNearPoint(int sendID, Vector3 from, Vector3 to, out Vector3 dst, int r = 1)
        {
            if (sendID != MapMgr.Instance.EnterMapId)
            {
                LogMgr.WarningLog("场景数据目前使用当前场景，应该改成sendID对应的数据");
                dst = to;
                return true;
            }

            //目标点可达
            if (!IsBlocked(to))
            {
                dst = to;
                return true;
            }

            //开始一圈一圈地查找最近点
            //int direction_x = to.x - from.x > 0 ? 1 : -1;
            //int direction_z = to.z - from.z > 0 ? 1 : -1;
            //for (int i = -1; i <= 1; i++)
            //{
            //    for (int j = -1; j <= 1; j++)
            //    {
            //        int x = direction_x * i;
            //        int z = direction_z * j;
            //        Vector3 newPoint = new Vector3(to.x + x, to.y, to.z + z);
            //        bool bWalk = IsBlocked(newPoint);
            //        if (!bWalk)
            //        {
            //            //return newPoint;
            //        }
            //    }
            //}

            //return to;
            dst = Vector3.zero;
            return false;
        }

        public Vector3 LineSegementDection(Vector3 startPos, Vector3 endPos, bool useSlide, ref bool isSlide)
        {
            if (null == sceneGrid)
                return startPos;

            int w = sceneGrid.width;
            int h = sceneGrid.height;

            Vector2 dir = new Vector2(endPos.x - startPos.x, endPos.z - startPos.z);
            dir = dir / 10;

            Vector2 start = new Vector2(startPos.x, startPos.z);
            Vector2 tmp = start;

            int startX = (int)(start.x * reciprocalSize);
            int startY = (int)(start.y * reciprocalSize);
            int endX = startX;
            int endY = startY;

            int i = 0;
            for (i = 1; i < 10; i++)
            {
                tmp = start + dir * i;
                int x = (int)(tmp.x * reciprocalSize);
                int y = (int)(tmp.y * reciprocalSize);

                if (x >= w || x < 0 || y >= h || y < 0)
                {
                    break;
                }

                Node node = sceneGrid.gridData[x, y];
                if (node.canWalk)
                {
                    endX = x;
                    endY = y;
                }
                else
                {
                    break;
                }
            }

            if (useSlide)
            {
                if (startX == endX && startY == endY)
                {
                    isSlide = true;
                    return GetSliderPosition(startPos, endPos);
                }
            }

            isSlide = false;
            bUserReviseDir = false;

            if (endX >= w || endX < 0 || endY >= h || endY < 0)
            {
                return startPos;
            }

            Node endNode = sceneGrid.gridData[endX, endY];
            return new Vector3(endNode.position.x + centerOffset, startPos.y, endNode.position.y + centerOffset);
        }

        private Vector3 GetSliderPosition(Vector3 startPos, Vector3 endPos)
        {
            int x = (int)(startPos.x * reciprocalSize);
            int y = (int)(startPos.z * reciprocalSize);

            if (x < sceneGrid.width && x >= 0 && y < sceneGrid.height && y >= 0)
            {
                Node cur = sceneGrid.gridData[x, y];

                Vector2 dir = reviseDir;
                if (!bUserReviseDir)
                {
                    dir = new Vector2(endPos.x - startPos.x, endPos.z - startPos.z);
                }

                List<Node> list = GetNodesByDir(cur, dir);
                for (int i = 0; i < list.Count; i++)
                {
                    Node nd = list[i];
                    if (nd.canWalk)
                    {
                        Vector3 destPos = new Vector3(nd.position.x + centerOffset, startPos.y, nd.position.y + centerOffset);
                        reviseDir = new Vector2(destPos.x - startPos.x, destPos.z - startPos.z);
                        bUserReviseDir = true;

                        return destPos;
                    }
                }
            }

            return startPos;
        }

        private List<Node> GetNodesByDir(Node node, Vector2 dir)
        {
            List<Node> list = new List<Node>();
            List<Node> tmpList = GetNodesBeforeSort(node);
            int[] orderIndex = new int[5];
            if (dir.x < 0.00001f && dir.x > -0.00001f)
            {
                if (dir.y > 0.00001f)
                {
                    orderIndex = new int[5]{2, 1, 3, 0, 4};
                }
                else if(dir.y < -0.00001f)
                {
                    orderIndex = new int[5] {6, 5, 7, 4, 0};
                }
            }
            else if (dir.y < 0.00001f && dir.y > -0.00001f)
            {
                if (dir.x > 0.00001f)
                {
                    orderIndex = new int[5] {0, 1, 7, 2, 6};
                }
                else if (dir.y < -0.00001f)
                {
                    orderIndex = new int[5] {4, 5, 3, 6, 2};
                }
            }
            else
            {
                if (dir.x > 0.00001f && dir.y > 0.00001f)
                {
                    if (dir.x > dir.y)
                    {
                        orderIndex = new int[5] {0, 1, 2, 7, 3};
                    }
                    else if (dir.x == dir.y)
                    {
                        orderIndex = new int[5] {1, 0, 2, 7, 3};
                    }
                    else
                    {
                        orderIndex = new int[5] {2, 1, 0, 3, 7};
                    }
                }
                else if (dir.x > 0.00001f && dir.y < -0.00001f)
                {
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        orderIndex = new int[5] { 0, 7, 6, 1, 5};
                    }
                    else if (Mathf.Abs(dir.x) == Mathf.Abs(dir.y))
                    {
                        orderIndex = new int[5] { 7, 0, 6, 1, 5};
                    }
                    else
                    {
                        orderIndex = new int[5] { 6, 7, 0, 5, 1};
                    }
                }
                else if (dir.x < -0.00001f && dir.y > 0.00001f)
                {
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        orderIndex = new int[5] { 4, 3, 2, 5, 1};
                    }
                    else if (Mathf.Abs(dir.x) == Mathf.Abs(dir.y))
                    {
                        orderIndex = new int[5] { 3, 4, 2, 5, 1};
                    }
                    else
                    {
                        orderIndex = new int[5] { 2, 3, 4, 1, 5};
                    }
                }
                else if (dir.x < -0.00001f && dir.y < -0.00001f)
                {
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        orderIndex = new int[5] { 4, 5, 6, 3, 7};
                    }
                    else if (Mathf.Abs(dir.x) == Mathf.Abs(dir.y))
                    {
                        orderIndex = new int[5] { 5, 4, 6, 3, 7};
                    }
                    else
                    {
                        orderIndex = new int[5] { 6, 5, 4, 7, 3};
                    }
                }
            }
            
            for (int i = 0; i < 5; i++)
            {
                Node nd = tmpList[orderIndex[i]];
                if (null != nd)
                {
                    list.Add(nd);
                }
            }

            return list;
        }

        private List<Node> GetNodesBeforeSort(Node node)
        {
            List<Node> nodes = new List<Node>();
            int x, y;
            Node nd = null;

            x = node.x + 1;
            y = node.y;
            if (x < sceneGrid.width)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x + 1;
            y = node.y + 1;
            if (x < sceneGrid.width && y < sceneGrid.height)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x;
            y = node.y + 1;
            if (y < sceneGrid.height)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x - 1;
            y = node.y + 1;
            if (x >= 0 && y < sceneGrid.height)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x - 1;
            y = node.y;
            if (x >= 0)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x - 1;
            y = node.y - 1;
            if (x >= 0 && y >= 0)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x;
            y = node.y - 1;
            if (y >= 0)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            x = node.x + 1;
            y = node.y - 1;
            if (x < sceneGrid.width && y >= 0)
            {
                nd = sceneGrid.gridData[x, y];
            }
            else
            {
                nd = null;
            }
            nodes.Add(nd);

            return nodes;
        }
    }
}
