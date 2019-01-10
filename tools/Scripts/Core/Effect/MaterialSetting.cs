using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class MaterialSetting : MonoBehaviour
    {
        public readonly static List<MaterialSetting> list = new List<MaterialSetting>();

        public Material[] mats;
        readonly List<Material> originList = new List<Material>();
        Renderer theRnd;

        public static void OnChange()
        {
            foreach (var i in list)
            {
                if (i != null)
                {
                    i.Set();
                }
            }
            list.RemoveAll((a) =>
            {
                return a == null;
            });
        }

        public void Awake()
        {
            if(list.Find(s=>s== this) == null)
            {
                list.Add(this);
            }
            theRnd = GetComponent<Renderer>();
            if (theRnd == null)
            {
                return;
            }
            originList.Clear();
            originList.AddRange(theRnd.materials);
 
        } 

        private void OnEnable()
        {
            Set();
        }

        private void OnDestroy()
        {
            list.Remove(this);
        }

        public void Set()
        {
            if (theRnd == null || mats == null)
            {
                return;
            }

            if (GameGraphicSetting.IsLowQuality)
            {
//                 Material[] ms = new Material[mats.Length];
//                 System.Array.Copy(mats, ms, mats.Length);
                theRnd.materials = mats;
            }
            else
            {
                theRnd.materials = originList.ToArray();
            }
        }
    }
}

