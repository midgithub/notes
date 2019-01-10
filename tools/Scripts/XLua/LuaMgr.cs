//#define MANAGER_SELF_OBJ
using UnityEngine;
using System.Collections;
using System;
using XLua;
using SG;
using System.Collections.Generic;
[LuaCallCSharp]
[Hotfix]
public class LuaMgr : MonoBehaviour
{

    private static LuaMgr m_instance;
    public static LuaMgr Instance
    {
        get
        {
#if  MANAGER_SELF_OBJ            
            if (m_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "LuaMgr";
                m_instance = obj.AddComponent<LuaMgr>();
                GameObject.DontDestroyOnLoad(obj);
            }
#else
        m_instance = CoreEntry.gLuaMgr;
#endif


            return m_instance;
        }
    }

    static LuaEnv luaEnv;

    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    static bool canDispose = false;                 //是否可以销毁了
    static SortedDictionary<int, LuaBehaviour> luaObjects = new SortedDictionary<int, LuaBehaviour>();

    public static void AddLua(LuaBehaviour lua)
    {
        luaObjects.Add(lua.Index, lua);
    }

    public static void RemoveLua(LuaBehaviour lua)
    {
        luaObjects.Remove(lua.Index);
        if (canDispose && luaObjects.Count <= 0)
        {
            canDispose = false;
            DisposeLua();
        }
    }

    static void DisposeLua()
    {
        NetClientThread.ClearLua();
        if (luaEnv != null)
        {
            luaEnv.DoString("require 'Lua/Dispose'");
            luaEnv.Dispose();
            luaEnv = null;
        }
    }

    public void Awake()
    {
        if (luaEnv == null)
        {
            luaEnv = new LuaEnv();
            luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
            luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
            luaEnv.AddBuildin("protobuf.c", XLua.LuaDLL.Lua.LoadProtobufC);			
        }
        luaEnv.AddLoader(CustomLoader);
        DoString("require 'Lua/main'");
    }

    public static byte[] CustomLoader(ref string filename)
    {
        TextAsset luaCfg;
		if (filename.Split('/')[0].Equals("Lua"))
		{
			luaCfg = CoreEntry.gResLoader.LoadTextAsset(filename + ".lua", LoadModule.AssetType.Txt);
		}
		else
		{
			luaCfg = Resources.Load(filename + ".lua", typeof(TextAsset)) as TextAsset;
		}

        if (luaCfg != null)
        {
            ProfilerData.AddLuaFile(filename, luaCfg);
        }

        if (luaCfg != null)
        {
            return luaCfg.bytes;
        }
        return null;
    }

    public object[] DoString(string filename)
    {
        return luaEnv.DoString(filename); 
    }


    public LuaEnv GetLuaEnv()
    {
        return luaEnv;
    }
    

    //////////////////////////////////////////////////////////////////////////
    void  Start()
    {
    }

    void Update()
    {
        if (luaEnv != null && Time.time - lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            luaEnv.GcStep(5 * 1024);
            //luaEnv.FullGc();
            lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaObjects.Count > 0)
        {
            canDispose = true;
        }
        else
        {
            DisposeLua();
        }
    }
}

