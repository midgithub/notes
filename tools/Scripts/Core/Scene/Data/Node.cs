/**
* @file     : Node.cs
* @brief    : 地图点数据
* @details  : 地图点数据
* @author   : CW
* @date     : 2017-06-20
*/
using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class Node
    {
        public bool canWalk;

        public Vector2 position;

        public int x;

        public int y;

        public float gCost;

        public float hCost;

        public float fCost
        {
            get { return gCost + hCost; }
        }

        public Node parent;

        public Node(bool canWalk, Vector2 position, int x, int y)
        {
            this.canWalk = canWalk;
            this.position = position;
            this.x = x;
            this.y = y;
        }

        public Node(bool canWalk, float posX, float posY, int x, int y) : this(canWalk, new Vector2(posX, posY), x, y)
        {
        }
    }
}

