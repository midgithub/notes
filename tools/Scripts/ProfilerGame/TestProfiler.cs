using XLua;
﻿using UnityEngine;
using System.Collections.Generic;
#if !UNITY_5_3_8
using UnityEngine.Profiling;
#endif

namespace SG
{
[Hotfix]
    public class TestProfiler : MonoBehaviour
    {
        public bool IsPrint = false;

        private void Start()
        {
            if (ProfilerData.Flag)
            {
                Profiler.enabled = true;
            }
        }

        HashSet<string> nameSet = new HashSet<string>();

        float lastTime = 0;
        private void Update()
        {
            if (IsPrint)
            {
                IsPrint = false;

                Print1();
            }
        }

        void Print1()
        {
            {
                var r = ProfilerData.GetLuaSrcResMem();
                LogMgr.LogError("lua文件数 : " + r.count + ",   内存 ： " + r.MB + " MB");
                LogMgr.LogError("lua总内存 : " + ProfilerData.GetLuaRuntimeMem() + " MB");
            }

            {
                var r = ProfilerData.GetMemByType<TextAsset>((a) =>
                {
                    return ProfilerData.IsLua(a.name);
                });
                LogMgr.LogError("### 实际 lua文件数 : " + r.count + ",   内存 ： " + r.MB + " MB");
            }

            {
                HashSet<string> set = new HashSet<string>();
                var r = ProfilerData.GetMemByType<TextAsset>((a) =>
                {
                    var ln = a.name.ToLower();
                    if (set.Contains(ln))
                    {
                        return false;
                    }
                    set.Add(ln);
                    return ln.Contains(".lua");
                });
                LogMgr.LogError("### 实际 lua文件数 2 : " + r.count + ",   内存 ： " + r.MB + " MB");
            }

            {
                var r = ProfilerData.GetCSVResMem();
                LogMgr.LogError("csv文件数 : " + r.count + ",   内存 ： " + r.MB + " MB");
            }

            {
                nameSet.Clear();
                var r = ProfilerData.GetMemByType<TextAsset>((a) =>
                {
                    if (nameSet.Contains(a.name.ToLower()))
                    {
                        return false;
                    }
                    nameSet.Add(a.name.ToLower());
                    return ProfilerData.IsCSV(a.name);
                });
                LogMgr.LogError("### 实际 csv文件数 : " + r.count + ",   内存 ： " + r.MB + " MB");
            }

            {
                var r = ProfilerData.GetMemByType<Texture>((a) =>
                {
                    return !string.IsNullOrEmpty(a.name);
                });
                LogMgr.LogError("纹理数 : " + r.count + ",   纹理内存 ： " + r.MB + " MB");
            }
        }

        void TickPrint()
        {
            var d = Time.time - lastTime;
            if (d >= 1f)
            {
                lastTime = Time.time;

            }
        }
    }
}

