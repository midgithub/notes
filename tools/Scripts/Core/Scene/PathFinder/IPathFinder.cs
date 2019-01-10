using XLua;
﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    public interface IPathFinder
    {
        /// <summary>
        /// 检测目标位置是否阻挡
        /// </summary>
        /// <param name="position">目标位置</param>
        /// <returns>阻挡返回true 否则返回false</returns>
        bool IsBlocked(Vector3 position);
        /// <summary>
        /// 使用起点用终点进行寻路，返回路径点列表，如果目标位置是障碍，则在起始位置和目标位置进行连线，选择离目标点最近可达的直线上的位置
        /// </summary>
        /// <param name="srcPosition">起始位置</param>
        /// <param name="destPosition">目标位置</param>
        /// <returns>返回路径点列表，可以为空</returns>
        List<Vector3> FindPath(Vector3 srcPosition, Vector3 destPosition);
        /// <summary>
        /// 判断线段覆盖的地图部分是否可行走，返回线段上最远可到达的点坐标
        /// </summary>
        /// <param name="segmentStart">线段起始位置</param>
        /// <param name="segmentEnd">线段终止位置</param>
        /// <param name="bSlider">是否滑行</param>
        /// <returns>返回闭区间[segmentStart,segmentEnd]之间最远可达的任一点</returns>
        Vector3 LineSegmentDetection(Vector3 segmentStart, Vector3 segmentEnd, ref bool bSlider);
        /// <summary>
        /// 获取地表可行走高度
        /// </summary>
        /// <param name="xzPosition">x,z平面位置</param>
        /// <returns>如果坐标在地形范围内，返回地表可行走高度，否则返回0</returns>
        float GetTerrainHeight(Vector2 xzPosition);
        /// <summary>
        /// 获取地表可行走高度
        /// </summary>
        /// <param name="x">x位置</param>
        /// <param name="z">z位置</param>
        /// <returns>如果坐标在地形范围内，返回地表可行走高度，否则返回0</returns>
        float GetTerrainHeight(float x, float y);
    }
}

