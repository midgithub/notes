using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
#if !UNITY_5_3_8
using UnityEngine.Profiling;
#endif

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class ProfilerData
    {
        //总开关
        //为false，则不会显示ui
        public static bool EnableProfilerSwitch = true;

        //统计加载的lua和csv内存
        public const bool EnableFileProfile = false;

        //是否正在统计信息
        public static bool IsProfiling = false;

        public static bool Flag
        {
            get
            {
                return EnableProfilerSwitch || EnableFileProfile;
            }
        }

        public const float K_ = 1024f;
        public const float M_ = K_ * K_;

        static readonly Dictionary<string, float> luaSrcDict = new Dictionary<string, float>();
        static readonly HashSet<string> luaNameSet = new HashSet<string>();
        static readonly Dictionary<string, float> csvDict = new Dictionary<string, float>();
        static readonly HashSet<string> csvNameSet = new HashSet<string>();

        public static void AddLuaFile(string name, TextAsset ta)
        {
            if (ta == null)
            {
                return;
            }
            luaNameSet.Add(ta.name.ToLower());

            if (!Flag)
            {
                return;
            }
            else
            {
                //if (!Profiler.enabled)
                //{
                //    Profiler.enabled = true;
                //}
            }
            
            int size = Profiler.GetRuntimeMemorySize(ta);
            luaSrcDict[name] = size / M_;
        }

        public static void AddCSVFile(string name, TextAsset ta)
        {
            if (ta == null)
            {
                return;
            }
            csvNameSet.Add(ta.name.ToLower());

            if (!Flag)
            {
                return;
            }
            else
            {
                //if (!Profiler.enabled)
                //{
                //    Profiler.enabled = true;
                //}
            }

            int size = Profiler.GetRuntimeMemorySize(ta);
            csvDict[name] = size / M_;
        }

        public static bool IsCSV(string name)
        {
            return csvNameSet.Contains(name.ToLower());
        }

        public static bool IsLua(string name)
        {
            return luaNameSet.Contains(name.ToLower());
        }

[Hotfix]
        public class Result
        {
            public int count;
            public float MB;
        }

        public static Result GetMem_GameObject()
        {
            Result r = new Result();
            GameObject[] textures = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            r.count = textures.Length;

            int size = 0;
            foreach (GameObject t in textures)
            {
                if (!string.IsNullOrEmpty(t.name))
                {
                    size += Profiler.GetRuntimeMemorySize(t);
                }
            }
            r.MB = (float)size / M_;
            return r;
        }

        //mesh, texture, animation clip
        //textasset
        public static Result GetMemByType<T>(System.Func<T, bool> check)
        {
            Result r = new Result();

            T[] objs = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
            int size = 0;
            foreach (var i in objs)
            {
                bool flag = true;
                if (check != null)
                {
                    flag = check(i);
                }

                if (flag)
                {
                    r.count++;
                    size += Profiler.GetRuntimeMemorySize(i as Object);
                }
            }
            r.MB = (float)size / M_;

            return r;
        }

        //全部内存
        public static float GetTotalMem()
        {
            var total = (float)Profiler.GetTotalReservedMemory();
            total /= M_;
            return total;
        }

        public static float GetUnusedMem()
        {
            var total = (float)Profiler.GetTotalUnusedReservedMemory();
            total /= M_;
            return total;
        }

        public static float GetMonoHeapMem()
        {
            var total = (float)Profiler.GetMonoHeapSize();
            total /= M_;
            return total;
        }

        public static float GetMonoUsedMem()
        {
            var total = (float)Profiler.GetMonoUsedSize();
            total /= M_;
            return total;
        }

        //Lua运行时内存
        public static float GetLuaRuntimeMem()
        {
            //接口返回的是K，所以只需要除以K_
            return (float)LuaMgr.Instance.GetLuaEnv().Memroy / K_;
        }

        //Lua脚本加载作为资源占用的内存
        public static Result GetLuaSrcResMem()
        {
            Result r = new Result();
            r.count = luaSrcDict.Count;
            foreach (var i in luaSrcDict)
            {
                r.MB += i.Value;
            }
            return r;
        }

        //csv资源内存
        public static Result GetCSVResMem()
        {
            Result r = new Result();
            r.count = csvDict.Count;
            foreach (var i in csvDict)
            {
                r.MB += i.Value;
            }
            return r;
        }
    }
}

