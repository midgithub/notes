using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
[Hotfix]
    public class EffectSetting : MonoBehaviour
    {
        public GameObject[] LowObjs;
        public GameObject[] MidObjs;

        public readonly static List<EffectSetting> list = new List<EffectSetting>();

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

        void Set()
        {
            if (LowObjs == null)
            {
                return;
            }

            int picSetting = SettingManager.Instance.PicSetting;

            foreach (var i in LowObjs)
            {
                if (i != null)
                {
                    i.SetActive(picSetting>1);
                    TrailRenderer tr = i.GetComponent<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.enabled = picSetting>1;
                    }
                }
            }

            foreach (var i in MidObjs)
            {
                if(i != null)
                {
                    i.SetActive(picSetting > 2);
                    TrailRenderer tr = i.GetComponent<TrailRenderer>();
                    if (tr != null)
                    {
                        tr.enabled = picSetting > 2;
                    }

                }
            }

        }

        private void Awake()
        {
            list.Add(this);
        }

        private void OnEnable()
        {
            Set();
        }

        private void OnDestroy()
        {
            list.Remove(this);
        }
    }
}

