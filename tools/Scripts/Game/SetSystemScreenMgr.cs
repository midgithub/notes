using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using XLua;
using SG;
using System.Text.RegularExpressions;

[Hotfix]
public class SetSystemScreenMgr {

    int systemMemorySize = 0;
    int CpuCount = 0;
    int versionLevel = 1;

    private static SetSystemScreenMgr m_instance = null;
    public static SetSystemScreenMgr Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SetSystemScreenMgr();
            }
            return m_instance;
        }
    }

    //void Awake()
    //{
    //    SetScreenByConfig();
    //}

    //IEnumerator Start()
    //{
    //    DontDestroyOnLoad(gameObject);

    //    while (true)
    //    {
    //        CpuCount = SystemInfo.processorCount;
    //        systemMemorySize = SystemInfo.systemMemorySize;
    //        versionLevel = SystemInfo.graphicsShaderLevel;
    //        yield return new WaitForSeconds(1);
    //    }
    //}

    public void SetScreenByConfig()
    {
        CpuCount = SystemInfo.processorCount;
        systemMemorySize = SystemInfo.systemMemorySize;
        versionLevel = SystemInfo.graphicsShaderLevel;
        Dictionary<int, LuaTable> data = LuaMgr.Instance.GetLuaEnv().Global.Get<Dictionary<int, LuaTable>>("t_SystemScreen");
        if (data == null)
        {
            LogMgr.LogError("请检查 t_SystemScreen ");
            return;
        }
        foreach (KeyValuePair<int, LuaTable> v in data)
        {
            List<int> processorRange = new List<int>();
            List<int> systemMemoryRange = new List<int>();
            List<int> graphicsShaderRange = new List<int>();

            string processorCountstr = v.Value.Get<string>("processorCount");
            string systemMemorySizestr = v.Value.Get<string>("systemMemorySize");
            string graphicsShaderLevelstr = v.Value.Get<string>("graphicsShaderLevel");


            processorCountstr.Split('#').ApplyAllItem(C => processorRange.Add(int.Parse(C)));
            systemMemorySizestr.Split('#').ApplyAllItem(C => systemMemoryRange.Add(int.Parse(C)));
            graphicsShaderLevelstr.Split('#').ApplyAllItem(C => graphicsShaderRange.Add(int.Parse(C)));
            float screenSize = v.Value.Get<float>("ScreenSize");
            if (processorRange.Count != 2 || systemMemoryRange.Count != 2 || graphicsShaderRange.Count != 2)
                LogMgr.LogError("SystemScreenConfigError:" + v.Key.ToString());
            else
            {
                if (CpuCount >= processorRange[0] && CpuCount <= processorRange[1] &&
                    systemMemorySize >= systemMemoryRange[0] && systemMemorySize <= systemMemoryRange[1] &&
                    versionLevel >= graphicsShaderRange[0] && versionLevel <= graphicsShaderRange[1])
                { 
                    Screen.SetResolution((int)(Screen.width * screenSize), (int)(Screen.height * screenSize), true);
                    LogMgr.Log("SetScreenSize:" + screenSize.ToString());
                }

            } 
        } 
    }

    //public float Size = 1f;

    //void OnGUI()
    //{
    //    GUILayout.Box(systemMemorySize.ToString());
    //    GUILayout.Box(versionLevel.ToString());
    //    GUILayout.Box(CpuCount.ToString());
    //    if (GUILayout.Button("SetScreen", GUILayout.Height(50)))
    //    {
    //        float screenSize = Size;
    //        int width = (int)(Screen.width * screenSize);
    //        int height = (int)(Screen.height * screenSize);
    //        Screen.SetResolution(width,height, true); 
    //        LogMgr.Log("SetScreen:"+width.ToString()+";"+height.ToString());
    //    }
    //}

}

