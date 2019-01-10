using XLua;
﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SG
{
    public enum MonsterType    // 布怪类型
    {
        none = 0,
        monster = 1,   //怪物
        npc = 2,       //NPC
        collection = 3,   //采集点
        portal = 4,   //传送门
        specail = 5,    //??
        birth = 6,     //出生点
        area = 7,      //安全区域
        point = 8,   //寻路点
    }




    //怪物编辑 -加载怪物属性。。
[Hotfix]
    public class MonsterInfo
    {
        public int mapId; //地图id
        public int id;  //怪物id
        public int modelId; //模型表对应的id
        public int editorType;  //配置表中编辑器类型
        public MonsterType enumType;  //刷怪类型
        public string name;  //怪物名字
        public int count;  //怪物数量
        public int wave_index;  //怪物分组
        public int index;  //怪物刷怪点索引号    ----- 编辑器根据索引号 来删除地图中怪点， 此ID 只做客户端用
        public List<int> pointRalations = new List<int>();  //路点关联集合，只有编辑路点(MonsterType.point)类型的时候，才会赋值这个集合
        public int pointLastAddIndex = 0;     //路点上次添加关联的索引值
        public int pointLastRemoveIndex = 0;     //路点上次删除关联的索引值
        public Vector3 ve3;  //怪物坐标
        public float eulerAnglesY; //怪物朝向
        public float r;   //怪物区域半径
        public int interval_time;   //刷怪间隔
        public int refresh_index;   //第几波开始刷怪
        public bool bShowArea = false;   //是否显示可攻击范围，前端编辑时使用。

        public GameObject itemObj;  //脚本绑定的物体 -- 只在前端编辑器用
        public MonsterInfo()
        {

        }
        public MonsterInfo(int _id, string _name)
        {
            this.id = _id;
            this.name = _name;
        }
        public MonsterInfo(int _id, string _name, int _modelId)
        {
            this.id = _id;
            this.name = _name;
            this.modelId = _modelId;
        }        
    }

[Hotfix]
    public class MonsterLoad : MonoBehaviour
    {

#if UNITY_EDITOR
        //public static Dictionary<MonsterType, string> TypeNames = new Dictionary<MonsterType, string>()
        //{
        //{MonsterType.monster,"monster"},
        //{MonsterType.npc,"npc"},
        //{MonsterType.collection,"collect"},
        //{MonsterType.portal,"door"},
        //};

        public static Dictionary<int, string> CacheModelSkn = new Dictionary<int, string>();

        public static MonsterLoad instance = null;

        public static Vector3 mouthDownPos = new Vector3(0, 0, 0);   //场景编辑中 鼠标点击下去的坐标位置。

        public int mapId = 0;

        static string _parentPrefab = "MapObj/MapPointRoot";  //父节点预制体

        static string _itemPrefab = "MapObj/ModelRoot";   //怪点预制体

        //各父节点
        Dictionary<MonsterType, GameObject> parentObj = new Dictionary<MonsterType, GameObject>();
        /// <summary>
        /// 各父节点名称
        /// </summary>
        Dictionary<MonsterType, string> parentStr = new Dictionary<MonsterType, string>();
        ///// <summary>
        ///// 各种预制体路径
        ///// </summary>
        //Dictionary<MonsterType, string> itemStrPath = new Dictionary<MonsterType, string>();

        public MonsterLoad()
        {
            instance = this;
            foreach (MonsterType item in Enum.GetValues(typeof(MonsterType)))
            {
                parentStr[item] = item.ToString();
//                itemStrPath[item] = _itemPrefab;
            }

        }

        /// <summary>
        /// 所有布阵 d 集合。。。
        /// </summary>
        Dictionary<MonsterType, Dictionary<int, GameObject>> items = new Dictionary<MonsterType, Dictionary<int, GameObject>>();

        public void GetCurMapData(int _mapId)
        {
            LogMgr.Log("_mapId: " + _mapId);
            mapId = _mapId;
            //			curMapDesc = GetRecordMapDesc(mapId);

        }
        public void SceneLoadPoint(Dictionary<MonsterType, Dictionary<int, MonsterInfo>> all)
        {
            foreach (var item in all)
            {
                if (!parentObj.ContainsKey(item.Key) || parentObj[item.Key] == null)
                {
                    GameObject p = (GameObject)UnityEngine.Object.Instantiate(CoreEntry.gResLoader.Load(_parentPrefab));
                    p.name = parentStr[item.Key];
                    p.transform.position = Vector3.zero;
                    parentObj[item.Key] = p;
                }
                if (!items.ContainsKey(item.Key))
                {
                    items[item.Key] = new Dictionary<int, GameObject>();
                }
                if (item.Value.Count > 0)
                {
                    foreach (var _item in item.Value)
                    {
                        MonsterItem tt = new MonsterItem();
                        if (items[item.Key].ContainsKey(_item.Value.index))
                        {
                            tt = items[item.Key][_item.Value.index].GetComponent<MonsterItem>();
                            tt.RefreshData(_item.Value);
                        }
                        else
                        {
                            GameObject obj = (GameObject)UnityEngine.Object.Instantiate(CoreEntry.gResLoader.Load(_itemPrefab));
                            obj.name = _item.Value.name.ToString();
                            tt = obj.GetComponent<MonsterItem>();
                            obj.transform.parent = parentObj[item.Key].transform;
                            tt.RefreshData(_item.Value);
                            items[item.Key][_item.Value.index] = obj;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定类型成员坐标
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetMemberPositon(MonsterType type, int index)
        {
            if(items.ContainsKey(type) && items[type].ContainsKey(index))
            {
                MonsterItem tt = items[type][index].GetComponent<MonsterItem>();
                return tt.pointText.transform.position;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 场景中加载怪物模型（单个添加）
        /// </summary>
        /// <returns>The load point.</returns>
        /// <param name="info">Info.</param>
        /// <param name="index">Index.</param>
        /// <param name="callback">Callback.</param>
        public void SceneLoadPoint(MonsterType type, MonsterInfo info, int index)
        {
            if (mapId == 0)
            {
                LogMgr.LogError("请先选择地图！");
                return;
            }
            if (!parentObj.ContainsKey(type) || parentObj[type] == null)
            {
                GameObject p = (GameObject)UnityEngine.Object.Instantiate(CoreEntry.gResLoader.Load(_parentPrefab));
                p.name = parentStr[type];
                p.transform.position = Vector3.zero;
                parentObj[type] = p;
            }

            MonsterItem tt = new MonsterItem();
            if (!items.ContainsKey(type))
            {
                items[type] = new Dictionary<int, GameObject>();
            }
            if (items[type].ContainsKey(info.index))
            {
                tt = items[type][info.index].GetComponent<MonsterItem>();
                tt.SetV(info);
            }
            else
            {
                GameObject obj = (GameObject)Instantiate(CoreEntry.gResLoader.Load(_itemPrefab));
                obj.name = info.name.ToString();
                tt = obj.GetComponent<MonsterItem>();
                obj.transform.parent = parentObj[type].transform;
                tt.RefreshData(info);
                items[type][info.index] = obj;
            }

            //         if (!items.ContainsKey(type))
            //         {
            //             items[type] = new List<GameObject>();
            //         }
            //if (items[type].Count >= index + 1) {
            //	tt =  items[type][index].GetComponent<MonsterItem>();
            //	tt.SetV(info[index]);

            //} else {
            //	GameObject obj = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(itemStrPath[type]));
            //	obj.name = index.ToString();
            //	tt = obj.GetComponent<MonsterItem>();
            //	obj.transform.parent = parentObj[type].transform;
            //	tt.RefreshData(info[index]);
            //	items[type].Add(obj);
            //}
            //         tt.gameObject.name = item.Value[i].name;

            //         return info [index];
        }

        /// <summary>
        /// 场景中删除模型。。。。
        /// </summary>
        /// <returns>The load point.</returns>
        /// <param name="info">Info.</param>
        /// <param name="index">Index.</param>
        /// <param name="callback">Callback.</param>
        public void SceneDelePoint(int index, MonsterType tp)
        {

            if (!items.ContainsKey(tp))
            {
                LogMgr.LogError("类型错误");
                return;
            }
            if (items[tp].ContainsKey(index))
            {
                LogMgr.Log("已删除  " + index);
                GameObject t = items[tp][index];
                DestroyImmediate(t);
                items[tp].Remove(index);
            }
            else
            {
                LogMgr.LogError("找不到  " + index);
            }
        }

        //生成并保存XML文件
        public void SaveMonsterXml(int mapW, int mapH, int mapSize)
        {
            if (parentObj == null)
            {
                LogMgr.LogError("场景中未载入怪物，请先添加或加载怪物");
                return;
            }
            string filePath = Application.dataPath + @"/ResData/Data/MapData/" + @"" + mapId + ".xml";
            if (!File.Exists(filePath))
            {
                XmlData xmldata1 = new XmlData();
                xmldata1.CreateXml(filePath);
                //				LogMgr.LogError("文件不存在=" + filePath);
                return;
            }

            XmlData xmldata = new XmlData();

            filePath = @"Data/MapData/" + mapId;
            //open的是resource资源
            xmldata.OpenXmlInEditor(filePath);

            //删除所有的节点
            xmldata.RemoveAllChild();
            //            int serverIndex = 0;
            //List<GameObject> _objs = new List<GameObject>();
            XmlElement firstElm = xmldata.GetEmptyElement("First");
            firstElm.SetAttribute("mapW", mapW.ToString());
            firstElm.SetAttribute("mapH", mapH.ToString());
            //            firstElm.SetAttribute("tileSize", mapSize.ToString());
            firstElm.SetAttribute("mapId", mapId.ToString());

            foreach (KeyValuePair<MonsterType, Dictionary<int, GameObject>> tt in items)
            {
                XmlElement typeElm = xmldata.GetEmptyElement(tt.Key.ToString());
                foreach (var item in tt.Value)
                {
                    MonsterItem monster = item.Value.GetComponent<MonsterItem>();
                    XmlElement newElm = xmldata.GetEmptyElement("table");
                    newElm.SetAttribute("id", tt.Key == MonsterType.birth ? 1.ToString() : monster.info.id.ToString());   //出生点 无id
                    newElm.SetAttribute("index", monster.info.index.ToString());
                    newElm.SetAttribute("type", ((int)monster.info.enumType).ToString());
                    newElm.SetAttribute("wave_index", monster.info.wave_index.ToString());
                    newElm.SetAttribute("intervalTime", monster.info.interval_time.ToString());
                    newElm.SetAttribute("x", item.Value.transform.position.x.ToString());
                    newElm.SetAttribute("y", item.Value.transform.position.y.ToString());
                    newElm.SetAttribute("z", item.Value.transform.position.z.ToString());
                    double dir = (Math.PI * (item.Value.transform.eulerAngles.y)) / 180f;   //传弧度
                    newElm.SetAttribute("dir", dir.ToString());
                    newElm.SetAttribute("modelId", monster.info.modelId.ToString());
                    if(tt.Key == MonsterType.point)
                    {
                        if(monster.info.pointRalations.Count > 0)
                        {
                            List<string> stringPointList = monster.info.pointRalations.ConvertAll<string>(x => x.ToString());
                            string points = string.Join(",", (string[])stringPointList.ToArray());
                            newElm.SetAttribute("pointRalations", points);
                        }
                    }
                    typeElm.AppendChild(newElm);
                    //LogMgr.LogError("坐标: (" + (int)item.Value.transform.position.x + "," + (int)item.Value.transform.position.z + ") 的角度:  " + item.Value.transform.eulerAngles.y +
                    //    "  弧度为: " + dir);
                }
                firstElm.AppendChild(typeElm);
            }
            //XmlElement typeElm2 = xmldata.GetEmptyElement(MonsterType.specail.ToString());
            //firstElm.AppendChild(typeElm2);
            xmldata.InsertElement(firstElm);
            xmldata.SaveXml();
        }
        /// <summary>
        /// 索引值， 类型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        public void SelectionTypeObj(int index, MonsterType type)
        {
            if (items.ContainsKey(type) && items[type].ContainsKey(index))
            {
                GameObject t = items[type][index];
                t.name = t.GetComponent<MonsterItem>().info.name;
                //                LogMgr.LogError("t.name  " + t.name);
                EditorGUIUtility.PingObject(t);
                Selection.activeGameObject = t;
                //                t.GetComponent<MonsterItem>().ChangeModelShow();
            }
            else
            {
                LogMgr.LogError("search  false");
            }
        }

        ////获取当前地图信息
        //mapConfig GetRecordMapDesc(int id)
        //{

        //    CSVSheet mapCsv = CSVConfigManager.Instance.LoadOrFindSheet("mapConfig", typeof(mapConfig));

        //    mapConfig record = mapCsv.GetRecord<mapConfig>(id);

        //    if (record == null)
        //    {

        //        SG.LogMgr.UnityLog(" ERROR! 配置文件:mapConfig 中没有找到ID:" + id);

        //    }

        //    return record;

        //}
#endif
    }

[Hotfix]
    public class MonsterExcelData
    {
        public int id;      //模板ID
        public string name;  //名称
        public int type;      //怪物enum类型 MonsterType  1.怪物    2.NPC    3.采集  4.传送门 5.特殊  6.出生点
        public int editorType;   //配置表中编辑器类型
        public string modelPath;   //模型路径
        public int modelId;  //模型id

        public MonsterExcelData(int _id,int _editorType, string _name, int _type, int _modelId, string _path = "")
        {
            this.id = _id;
            this.editorType = _editorType;
            this.name = _name;
            this.type = _type;
            this.modelId = _modelId;
            this.modelPath = _path;
        }
        public MonsterExcelData(string _name, int _type)
        {
            //            this.id = _id;
            this.name = _name;
            this.type = _type;
        }

        public static string GetMonterTypeStrName(MonsterType tt)
        {
            string name = "";
            switch (tt)
            {
                case MonsterType.monster:
                    name = "怪物";
                    break;
                case MonsterType.npc:
                    name = "NPC";
                    break;
                case MonsterType.collection:
                    name = "采集";
                    break;
                case MonsterType.portal:
                    name = "传送门";
                    break;
                case MonsterType.specail:
                    name = "特殊";
                    break;
                case MonsterType.birth:
                    name = "出生点";
                    break;
                case MonsterType.area:
                    name = "安全区";
                    break;
                case MonsterType.point:
                    name = "路点";
                    break;
                default:
                    break;
            }
            return name;
        }




    }
}

