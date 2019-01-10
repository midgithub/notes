/**
* @file     : FlyAttrManager.cs
* @brief    : 属性改变飘字
* @details  : 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    [LuaCallCSharp]
[Hotfix]
    public class FlyAttrManager : IModule
    {
        private static FlyAttrManager flyAttr;
        //----------- 每个管理器必须写的方法 ----------
        public override bool LoadSrv(IModuleServer IModuleSrv)
        {
            ModuleServer moduleSrv = (ModuleServer)IModuleSrv;
            moduleSrv.GFlyAttrMgr = this;

            return true;
        }

        public override void InitializeSrv()
        {
            flyAttr = this;
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_FLY_ATTR, OnGameEventAttrChange);
        }

        public static IModule Newer(GameObject go)
        {
            IModule module = go.AddComponent<FlyAttrManager>();
            return module;
        }

        ////属性改变飘字管理单实例。
        //private static FlyAttrManager _instance = null;

        ///// <summary>
        ///// 获取血条管理。
        ///// </summary>
        //public static FlyAttrManager Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new FlyAttrManager();
        //        }
        //        return _instance;
        //    }
        //}

        public static BasicAttrEnum[] ShowAttr = new BasicAttrEnum[]
        { BasicAttrEnum.Power, BasicAttrEnum.Attack, BasicAttrEnum.Defence, BasicAttrEnum.MaxHP, BasicAttrEnum.DefenceIgnore, BasicAttrEnum.Hit,
            BasicAttrEnum.Dodge, BasicAttrEnum.Crit, BasicAttrEnum.Toughness, BasicAttrEnum.Speed, BasicAttrEnum.Impale, BasicAttrEnum.ParryProb};

        /// <summary>
        /// 获取要显示的属性数组。
        /// </summary>
        /// <param name="attr">属性值。</param>
        /// <returns>属性数组。</returns>
        public static double[] GetShowAttr(double[] attrs)
        {
            double[] ret = new double[ShowAttr.Length];
            for (int i=0; i< ShowAttr.Length; ++i)
            {
                ret[i] = attrs[(int)ShowAttr[i]];
            }
            return ret;
        }

        public static string[] Prefabs = new string[4] { "UI/Prefabs/FlyAttr/FirstRes/ItemFlyPowerAdd" ,
            "UI/Prefabs/FlyAttr/ItemFlyPowerSub" , "UI/Prefabs/FlyAttr/FirstRes/ItemFlyAttrAdd", "UI/Prefabs/FlyAttr/FirstRes/ItemFlyAttrSub" };

        public struct FlyAttrInfo
        {
            public FlyAttrInfo(BasicAttrEnum type, int ov, int nv)
            {
                AttrType = type;
                OldValue = ov;
                NewValue = nv;
            }

            public BasicAttrEnum AttrType { get; set; }
            public int OldValue { get; set; }
            public int NewValue { get; set; }
        }

        /// <summary>
        /// 属性显示间隔。
        /// </summary>
        public static float ShowAttrGap = 0.3f;

        /// <summary>
        /// 飘字缓存。
        /// </summary>
        private Dictionary<int, Stack<ItemFlyAttr>> m_CacheFlyAttr = new Dictionary<int, Stack<ItemFlyAttr>>();

        /// <summary>
        /// 当前显示的飘字对象。
        /// </summary>
        private List<ItemFlyAttr> CurFlyAttr = new List<ItemFlyAttr>();

        /// <summary>
        /// 战斗力飘字对象。
        /// </summary>
        private ItemFlyAttr CurPowerFlyAttr = null;

        /// <summary>
        /// 需要显示的属性队列。
        /// </summary>
        private List<FlyAttrInfo> NeedShow = new List<FlyAttrInfo>();

        /// <summary>
        /// 上次显示属性的时间。
        /// </summary>
        private float m_LastShowTime = 0;

        /// <summary>
        /// 属性飘字挂接点。
        /// </summary>
        private Transform m_FlyAttrRoot;

        /// <summary>
        /// 获取属性飘字挂接点。
        /// </summary>
        public Transform FlyAttrRoot
        {
            get
            {
                if (m_FlyAttrRoot == null)
                {
                    string path = "UI/Prefabs/FlyAttr/FirstRes/FlyAttrRoot";
                    GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
                    if(prefab != null)
                    {
                        GameObject obj = GameObject.Instantiate(prefab) as GameObject;
                        obj.SetActive(true);
                        obj.GetComponent<Canvas>().worldCamera = MainPanelMgr.Instance.uiCamera;
                        GameObject.DontDestroyOnLoad(obj);
                         m_FlyAttrRoot = obj.transform;
                    }

                }

                return m_FlyAttrRoot;
            }
        }

        void Update()
        {
            if (NeedShow.Count > 0 && Time.realtimeSinceStartup >= m_LastShowTime + ShowAttrGap)
            {
                FlyAttrInfo info = NeedShow[0];
                NeedShow.RemoveAt(0);
                ShowAttrChange(info.AttrType, info.OldValue, info.NewValue, 0);
                m_LastShowTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 属性变化。
        /// </summary>
        public void OnGameEventAttrChange(GameEvent ge, EventParameter parameter)
        {
            double[] oldattrs = (double[])parameter.objParameter;
            double[] newattrs = (double[])parameter.objParameter1;
            //float delay = 0;
            for (int i = 0; i < ShowAttr.Length; ++i)
            {
                int change = (int)(newattrs[i] - oldattrs[i]);
                if (change != 0)
                {
                    if (ShowAttr[i] != BasicAttrEnum.Power)
                    {
                        NeedShow.Add(new FlyAttrInfo(ShowAttr[i], (int)oldattrs[i], (int)newattrs[i]));
                    }
                    else
                    {
                        //战斗力直接显示
                        ShowAttrChange(ShowAttr[i], (int)oldattrs[i], (int)newattrs[i], 0);
                    }
                }
            }
        }

        public ItemFlyAttr GetFlyAttr(int type)
        {
            Stack<ItemFlyAttr> cache;
            if (!m_CacheFlyAttr.TryGetValue(type, out cache))
            {
                cache = new Stack<ItemFlyAttr>();
                m_CacheFlyAttr.Add(type, cache);
            }

            if (cache.Count > 0)
            {
                ItemFlyAttr item = cache.Pop();
                item.gameObject.SetActive(true);
                return item;
            }

            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(Prefabs[type], typeof(GameObject));
            
            
            if(prefab ==null) return null;
            
            GameObject obj = GameObject.Instantiate(prefab) as GameObject;
            RectTransform rt = obj.GetComponent<RectTransform>();
            ItemFlyAttr ret = obj.GetComponent<ItemFlyAttr>();
            obj.SetActive(true);
            rt.SetParent(FlyAttrRoot);
            rt.anchoredPosition3D = Vector3.zero;
            rt.localScale = Vector3.one;
            ret.ShowType = type;
            return ret;
        }

        public void RemoveFlyAttr(ItemFlyAttr item)
        {
            Stack<ItemFlyAttr> cache;
            if (!m_CacheFlyAttr.TryGetValue(item.ShowType, out cache))
            {
                cache = new Stack<ItemFlyAttr>();
                m_CacheFlyAttr.Add(item.ShowType, cache);
            }

            item.gameObject.SetActive(false);
            CurFlyAttr.Remove(item);
            cache.Push(item);

            //战斗力去除
            if (item.ShowType == 0 || item.ShowType == 1)
            {
                CurPowerFlyAttr = null;
            }
        }

        /// <summary>
        /// 显示属性改变。
        /// </summary>
        /// <param name="type">属性类型。</param>
        /// <param name="oldvalue">改变前的值。</param>
        /// <param name="newvalue">改变值。</param>
        /// <param name="delay">显示延迟。</param>
        public void ShowAttrChange(BasicAttrEnum type, int oldvalue, int newvalue, float delay)
        {
            int change = newvalue - oldvalue;
            if (type == BasicAttrEnum.Power)
            {
                //同一时间只显示一个战斗变化
                if (CurPowerFlyAttr != null)
                {
                    RemoveFlyAttr(CurPowerFlyAttr);
                }
                CurPowerFlyAttr = GetFlyAttr(change > 0 ? 0 : 1);
                if(newvalue > oldvalue)
                {
                    //CoreEntry.gAudioMgr.PlayUISound(900011);
                }
                CurPowerFlyAttr.Init(type, oldvalue, newvalue, 0, -150, delay);
            }
            else
            {
                ItemFlyAttr item = GetFlyAttr(change > 0 ? 2 : 3);
                item.Init(type, oldvalue, newvalue, 200, 50, delay);
                CurFlyAttr.Add(item);
            }
        }

        public static void CloseAllFlyAttr()
        {
            if(flyAttr != null)
            {
                flyAttr.m_CacheFlyAttr.Clear();
                flyAttr.NeedShow.Clear();

                while(flyAttr.CurFlyAttr.Count > 0)
                {
                    flyAttr.RemoveFlyAttr(flyAttr.CurFlyAttr[0]);
                }
            }
            
        }

        public void ShowTextChange(string text,int oldvalue, int newvalue, float delay)
        {
            int change = newvalue - oldvalue;
            {
                ItemFlyAttr item = GetFlyAttr(change > 0 ? 2 : 3);
                item.Init(BasicAttrEnum.Exp, oldvalue, newvalue, 200, 50, delay);
                item.NameText.text = text;
                CurFlyAttr.Add(item);
            }
        }
    }
}

