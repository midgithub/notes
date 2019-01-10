using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace SG
{
    /// <summary>
    /// 贴图图集管理。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class AtlasSpriteManager
    {
        /// <summary>
        /// 单例对象。
        /// </summary>
        public static AtlasSpriteManager _instance = null;

        /// <summary>
        /// 获取单例。
        /// </summary>
        public static AtlasSpriteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AtlasSpriteManager();
                    _instance.m_Default = _instance.GetSprite("ItemIco", "Default");
                }
                return _instance;
            }
        }

        /// <summary>
        /// UI变灰材质。
        /// </summary>
        private Material m_UIGrey;

        public static string PATH_UI_GREY = "Shaders/UI/UIGrey";

        /// <summary>
        /// 获取UI变灰材质。
        /// </summary>
        public Material UIGrey
        {
            get
            {
                if (m_UIGrey == null)
                {
                    //                     m_UIGrey = (Material)CoreEntry.gResLoader.Load(PATH_UI_GREY, typeof(Material));
                    //                     if (m_UIGrey == null)
                    //                     {
                    //                         LogMgr.LogWarning("Can not found the material. path:{0}", PATH_UI_GREY);
                    //                     }
                    m_UIGrey = new Material(CoreEntry.gResLoader.LoadShader("UI/Grey"));
                }
                return m_UIGrey;
            }
        }

        private Material m_RTMaterial;
        public static string PATH_RT_MATERIAL = "Material/RTMaterial";
        public Material RTMaterial
        {
            get
            {
                if (null == m_RTMaterial)
                {
                    m_RTMaterial = CoreEntry.gResLoader.Load(PATH_RT_MATERIAL, typeof(Material)) as Material;
                    if (null == m_RTMaterial)
                    {
                        LogMgr.UnityError("no material at path :" + PATH_RT_MATERIAL);
                    }
                }

                return m_RTMaterial;
            }
        }

        /// <summary>
        /// 获取贴图。
        /// </summary>
        /// <param name="prefab">贴图路径。(图集路径:贴图名称)</param>
        /// <returns>贴图对象</returns>
        public Sprite GetSprite(string path)
        {
            string[] names = path.Split(':');
            if (names.Length < 2)
            {
                LogMgr.WarningLog("Can not found sprite in path({0}).", path);
                return m_Default;
            }
            return GetSprite(names[0], names[1]);
        }

        /// <summary>
        /// 获取贴图。
        /// </summary>
        /// <param name="prefab">图集预制件路径。</param>
        /// <param name="name">贴图名称。</param>
        /// <returns>贴图对象</returns>
        public Sprite GetSprite(string prefab, string name)
        {
            Dictionary<string, Sprite> atlas;
            if (!m_AtlasSprites.TryGetValue(prefab, out atlas))
            {
                atlas = LoadAtlas(prefab);
                if (atlas != null)
                {
                    m_AtlasSprites.Add(prefab, atlas);
                }
                else
                {
                    return null;
                }                
            }

            Sprite sp;
            if (!atlas.TryGetValue(name, out sp))
            {
                LogMgr.WarningLog("Can not found sprite({0}) in atlas({1}).", name, prefab);
                sp = m_Default;
            }
            return sp;
        }

        /// <summary>
        /// 移除某个图集。
        /// </summary>
        /// <param name="prefab">图集名称。</param>
        public void RemoveCache(string prefab)
        {
            Dictionary<string, Sprite> cache;
            if (m_AtlasSprites.TryGetValue(prefab, out cache))
            {
                cache.Clear();
                m_AtlasSprites.Remove(prefab);
            }
        }

        /// <summary>
        /// 清除缓存。
        /// </summary>
        public void ClearCache()
        {
            var e = m_AtlasSprites.GetEnumerator();
            while (e.MoveNext())
            {
                var kvp = e.Current;
                kvp.Value.Clear();
            }
            e.Dispose();
            m_AtlasSprites.Clear();
        }

        /// <summary>
        /// 加载图集。
        /// </summary>
        /// <param name="prefab">图集预制件路径。</param>
        /// <returns>图集的贴图集合。</returns>
        private Dictionary<string, Sprite> LoadAtlas(string prefab)
        {
            string path = "UI/Atlas/" + prefab;
            GameObject obj = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
            if (obj == null)
            {
                //Log.Warning("Can not found the prefab. path:{0}", prefab);
                return null;
            }

            AtlasHolder holder = obj.GetComponent<AtlasHolder>();
            if (holder == null)
            {
                LogMgr.WarningLog("The prefab has not component of 'AtlasHolder'. path:{0}", prefab);
                return null;
            }

            return holder.GetSprites();
        }

        /// <summary>
        /// 图集贴图缓存。
        /// </summary>
        private Dictionary<string, Dictionary<string, Sprite>> m_AtlasSprites = new Dictionary<string, Dictionary<string, Sprite>>();

        /// <summary>
        /// 默认图标。
        /// </summary>
        private Sprite m_Default;
    }
}