/*******************************************************
* Copyright (c) 2017, All rights reserved.
* 
* FileName: MapMeshMono.cs
* 地图阻挡编辑

* Author:  
* Date: 2017-6-3  
*******************************************************/
using XLua;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[Hotfix]
public class MapMeshMono : MonoBehaviour
{
#if UNITY_EDITOR


    public Rect mRectArea;
    public Texture tex;

    private int hang = 100;
    private int lie = 100;
    private float size = 0.5f;

    public int sizeDefault = 1;  //格子初始值
    public float sizeScale = 0.5f;  //格子缩放比例，
    public int meshHeight = 15;

    public int brushSize = 1; //笔刷大小
    public MapNode.PointType currentType = MapNode.PointType.none;   // 笔刷刷的类型
    //uint fourcc = (uint)0xAB  | ((uint)0x0C<<8) | ((uint)0xcd<<16) | ((uint)0xff<<24)

    public uint fourcc = (uint)0x6D | ((uint)0x65 << 8) | ((uint)0x73 << 16) | ((uint)0x68 << 24);


    public int version = 1; //版本号
    public int mapId;
    public string mapPathName;     //文本名字
    public MapNode[,] mMapPoints;  //地图上的所有格子
    //private bool isLeftMouseDrag = false;

    //设宽
    public int Widht
    {
        get
        {
            return lie;
        }
        set
        {
            lie = value;
            ResetMapPointArea();
        }
    }

    //设高
    public int Height
    {
        get
        {
            return hang;
        }
        set
        {
            hang = value;
            ResetMapPointArea();
        }
    }


    //设置格子大小  默认1x1 
    public float Size
    {
        get
        {
            return size;
        }
        set
        {
            size = value;
            ResetMapPointArea(false);
        }
    }

    Vector3 mouseFrom;
    Vector3 mouseTo;
    RaycastHit _hitInfo;

    bool bDraw = false;

    public void SetDrawState(bool bdraw)
    {
        bDraw = bdraw;
    }

    void OnDrawGizmos()
    {
        if (mMapPoints == null)
        {
            ResetMapPointArea();
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < (int)(hang / size); ++i)    //画所有格子
        {
            for (int j = 0; j < (int)(lie / size); ++j)
            {
                MapNode p = mMapPoints[i, j];

                //                Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY + size), new Vector3(p.mX + size, meshHeight, p.mY + size));  //上
                Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY), new Vector3(p.mX + size, meshHeight, p.mY));   //下
                Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY), new Vector3(p.mX, meshHeight, p.mY + size));//左
                                                                                                                 //                Gizmos.DrawLine(new Vector3(p.mX + size, meshHeight, p.mY), new Vector3(p.mX + size, meshHeight, p.mY + size));  //右
                Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY), new Vector3(p.mX + size, meshHeight, p.mY + size));   //顺斜线
                Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY + size), new Vector3(p.mX + size, meshHeight, p.mY));   //逆斜线
            }
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < (int)(hang / size); ++i)    //画阻挡区域
        {
            for (int j = 0; j < (int)(lie / size); ++j)
            {
                MapNode p = mMapPoints[i, j];
                if (p.Type == MapNode.PointType.barrier)
                {
                    //                    Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY + size), new Vector3(p.mX + size, meshHeight, p.mY + size));  //上
                    Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY), new Vector3(p.mX + size, meshHeight, p.mY));   //下
                    Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY), new Vector3(p.mX, meshHeight, p.mY + size));//左
                                                                                                                     //                    Gizmos.DrawLine(new Vector3(p.mX + size, meshHeight, p.mY), new Vector3(p.mX + size, meshHeight, p.mY + size));  //右
                    Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY), new Vector3(p.mX + size, meshHeight, p.mY + size));   //顺斜线
                    Gizmos.DrawLine(new Vector3(p.mX, meshHeight, p.mY + size), new Vector3(p.mX + size, meshHeight, p.mY));   //逆斜线
                }
            }
        }
        if (bDraw)
        {
            ChangeState();
            bDraw = false;
        }
        if (id_x >= 0 && id_y >= 0 && id_x < (int)(hang / size) && id_y < (int)(lie / size) && currentType != MapNode.PointType.none)
        {
            Gizmos.color = currentType == MapNode.PointType.barrier ? Color.magenta : Color.white;
            Gizmos.DrawCube(new Vector3(mMapPoints[id_x, id_y].mX + ((float)brushSize * size / 2), meshHeight, mMapPoints[id_x, id_y].mY + ((float)brushSize * size / 2)), new Vector3(brushSize * size, 1, brushSize * size));
        }
    }
    //当前scene中点击的点坐标
    int id_x;
    int id_y;
    private void ChangeState()
    {
        if (Event.current.button == 0)  //鼠标左键
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out _hitInfo, 10000, -1))
            {
                Vector3 pos = _hitInfo.point;
                Vector3 v3 = pos;
                //中心点00 判断
                //id_x = (int)(Math.Abs((-hang / 2 - v3.z) * mMapPoints.GetLength(0) / hang));
                //id_y = (int)(Math.Abs((-lie / 2 - v3.x) * mMapPoints.GetLength(1) / lie));             
                //左下角00 判断
                id_x = (int)(Math.Abs((v3.z) * mMapPoints.GetLength(0) / hang));
                id_y = (int)(Math.Abs((v3.x) * mMapPoints.GetLength(1) / lie));
                for (int i = 0; i < brushSize; i++)
                {
                    for (int j = 0; j < brushSize; j++)
                    {
                        int Idx = i + id_x;
                        int Idy = j + id_y;
                        if (Idx >= 0 && Idy >= 0 && Idx < hang / size && Idy < lie / size && currentType != MapNode.PointType.none)
                        {
                            mMapPoints[Idx, Idy].Type = currentType;
                        }
                    }
                }
            }
        }

    }

    GameObject brushObj;
    //static string brushObjPath = @"Animation/actorContour_ex";
    //刷新数组信息
    void ResetMapPointArea(bool bChangeMeshArea = false)
    {

        if (mMapPoints != null && mMapPoints.GetLength(0) == (int)(hang / size) && mMapPoints.GetLength(1) == (int)(lie / size))  //inspector频繁赋值,设置过滤条件，已赋值了不再赋值
        {
        }
        else
        {
            if (bChangeMeshArea)  //修改格子大小，修改后重新计算阻挡区域
            {
                MapNode[,] newPoints = new MapNode[(int)(hang / size), (int)(lie / size)];
                //            mMapPoints = new MapNode[(int)(hang / size), (int)(lie / size)];
                float _scale = 0;
                if (mMapPoints != null)
                {
                    _scale = (float)newPoints.GetLength(0) / mMapPoints.GetLength(0);
                }
                for (int i = 0; i < (int)(hang / size); ++i)
                {
                    for (int j = 0; j < (int)(lie / size); ++j)
                    {
                        MapNode p = new MapNode(size * j, size * i);    //左下角为00点的数组
                        newPoints[i, j] = p;
                        if (mMapPoints != null && _scale > 0)
                        {
                            int m = (int)(i / _scale);
                            int n = (int)(j / _scale);
                            newPoints[i, j].Type = mMapPoints[m, n].Type;
                        }
                    }
                }
                mMapPoints = newPoints;
            }
            else     //修改长宽 ，清空数据
            {
                mMapPoints = new MapNode[(int)(hang / size), (int)(lie / size)];
                for (int i = 0; i < (int)(hang / size); ++i)
                {
                    for (int j = 0; j < (int)(lie / size); ++j)
                    {
                        MapNode p = new MapNode(size * j, size * i);    //左下角为00点的数组
                        mMapPoints[i, j] = p;
                    }
                }
            }

        }
    }

    void SetBrushProjectorSize(float size, Vector3 v)
    {
        GetBrushProjector(v);
        p.orthographicSize = size;
        Material mat = p.material;
        if (AssetDatabase.GetAssetPath(mat) != "Assets/Effect/Textures/xulie/Materials/ani0074.mat")
        {
            Material m = AssetDatabase.LoadMainAssetAtPath("Assets/Effect/Textures/xulie/Materials/ani0074.mat") as Material;
            mat = m;
        }
    }
    GameObject tempgo;
    Projector p;
    void GetBrushProjector(Vector3 v)
    {
        if (tempgo == null)
        {
            tempgo = GameObject.Find("groundprojector");
            tempgo.SetActive(true);
            p = tempgo.GetComponent<Projector>();
            p.orthographic = true;
        }
        tempgo.transform.LookAt(v - Vector3.up);

    }

#endif
}

[Hotfix]
public class MapNode : IComparable<MapNode>
{
    public float mX;
    public float mY;

    //  A*  start
    public int sCost;  //与起点的长度
    public int eCost;  //与终点的长度

    public int allCost  //实际总长度
    {
        get
        {
            return sCost + eCost;
        }
    }
    //    end

    public PointType Type = PointType.pass;

    public MapNode(float x, float y)
    {
        mX = x;
        mY = y;
    }

    public enum PointType
    {
        pass = 0,      //空白处
        barrier = 1,   //阻挡处
        none = 2,      //不处理
    }

    public int CompareTo(MapNode other)
    {
        throw new NotImplementedException();
    }
}

