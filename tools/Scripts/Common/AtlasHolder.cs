using XLua;
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    /// <summary>
    /// 图集持有者，用于保存一个图集所具有的Sprite。
    /// </summary>
[Hotfix]
    public class AtlasHolder : MonoBehaviour
    {
        /// <summary>
        /// 获取Spite集合。
        /// </summary>
        /// <returns>名称到Sprite的字典。</returns>
        public Dictionary<string, Sprite> GetSprites()
        {
            Dictionary<string, Sprite> ret = new Dictionary<string, Sprite>();
            if (Sprites == null)
            {
                return ret;
            }

            for (int i = 0; i < Sprites.Count; ++i)
            {
                Sprite sp = Sprites[i];
                if (sp != null)
                {
                    string spname = sp.name;
                    int inex = spname.LastIndexOf('.');
                    ret.Add(inex == -1 ? spname : spname.Substring(0, inex), sp);
                }                
            }

            return ret;
        }

        public List<Sprite> Sprites;
    }
}

