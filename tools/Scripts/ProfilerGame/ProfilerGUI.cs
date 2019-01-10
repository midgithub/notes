#define PROFILER_GUI_ENABLE

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
    public class ProfilerGUI : MonoBehaviour
    {
        public static void ShowMemo(bool flag)
        {
#if PROFILER_GUI_ENABLE
            if (!ProfilerData.EnableProfilerSwitch)
            {
                return;
            }

            ProfilerData.IsProfiling = flag;
            Profiler.enabled = flag;
#endif
        }

#if PROFILER_GUI_ENABLE
        //static ProfilerGUI instance = null;

        float totalMem = 0f;
        float unusedMem = 0f;
        float monoHeapMem = 0f;
        float monoUsedMem = 0f;
        ProfilerData.Result texResult = new ProfilerData.Result();
        ProfilerData.Result meshResult = new ProfilerData.Result();
        ProfilerData.Result animClipResult = new ProfilerData.Result();
        ProfilerData.Result luaResult = new ProfilerData.Result();
        //ProfilerData.Result csvResult = new ProfilerData.Result();
        float luaMem = 0f;
        string drawCallCountText = string.Empty;

        HashSet<string> luaNameSet = new HashSet<string>();
        //HashSet<string> csvNameSet = new HashSet<string>();

        //void Awake()
        //{
        //    instance = this;
        //}

        GUIStyle myStyle = null;

        static string ToMBStr(float v)
        {
            return v.ToString("f3") + " MB";
        }

        float startX = Screen.width / 4;
        float width = 200;
        float height = 200;

        float startY = 130;
        float ySpace = 30;
        int yIndex = 0;
        float NextY()
        {
            int i = yIndex;
            yIndex++;
            return startY + i * ySpace;
        }
        void OnGUI()
        {
            if (!ProfilerData.IsProfiling)
            {
                return;
            }

            yIndex = 0;

            if (myStyle == null)
            {
                myStyle = new GUIStyle();
                myStyle.normal.background = null;    //这是设置背景填充的
                myStyle.normal.textColor = new Color(0, 0, 1);   //设置字体颜色的
                myStyle.fontSize = 20;
            }
            
            GUI.Label(new Rect(startX, NextY(), width, height), string.Format("总申请内存:{0}  未使用内存:{1}", ToMBStr(totalMem), ToMBStr(unusedMem)), myStyle);
            GUI.Label(new Rect(startX, NextY(), width, height), string.Format("MonoUsed:{0}  MonoHeap:{1}", ToMBStr(monoUsedMem), ToMBStr(monoHeapMem)), myStyle);
            ShowLabel("纹理", texResult);
            ShowLabel("mesh", meshResult);
            ShowLabel("animation clip", animClipResult);
            ShowLabel("lua文件残留", luaResult);
            //ShowLabel("csv文件残留", csvResult);
            GUI.Label(new Rect(startX, NextY(), width, height), "Lua内存:" + ToMBStr(luaMem), myStyle);
#if UNITY_EDITOR
            GUI.Label(new Rect(startX, NextY(), width, height), "DrawCall:" + drawCallCountText, myStyle);
#endif
        }

        void ShowLabel(string fieldName, ProfilerData.Result result)
        {
            GUI.Label(new Rect(startX, NextY(), width, height), fieldName + "数:" + result.count + "  " + fieldName + "内存:" + ToMBStr(result.MB), myStyle);
        }

        float lastTime = 0f;
        private void Update()
        {
            if (!ProfilerData.IsProfiling)
            {
                return;
            }

            var diff = Time.time - lastTime;
            if (diff <= 1f)
            {
                return;
            }
            lastTime = Time.time;

#if UNITY_EDITOR
            //drawcall信息，只有在编辑器内有效
            {
                drawCallCountText = string.Empty;
                var text = UnityEditorInternal.ProfilerDriver.GetOverviewText(UnityEditorInternal.ProfilerArea.Rendering, UnityEditorInternal.ProfilerDriver.lastFrameIndex);
                const string tag = "Draw Calls: ";
                int index = text.IndexOf(tag);
                if (index >= 0)
                {
                    index += tag.Length;
                    int index2 = -1;
                    for (int i = index; i < text.Length; i++)
                    {
                        char ch = text[i];
                        if (!char.IsDigit(ch))
                        {
                            index2 = i;
                            break;
                        }
                    }
                    if (index2 > index)
                    {
                        drawCallCountText = text.Substring(index, index2 - index);
                    }
                }
            }
            
#endif

            totalMem = ProfilerData.GetTotalMem();
            unusedMem = ProfilerData.GetUnusedMem();
            monoHeapMem = ProfilerData.GetMonoHeapMem();
            monoUsedMem = ProfilerData.GetMonoUsedMem();
            texResult = ProfilerData.GetMemByType<Texture>((a) =>
            {
                return !string.IsNullOrEmpty(a.name);
            });
            meshResult = ProfilerData.GetMemByType<Mesh>((a) =>
            {
                return !string.IsNullOrEmpty(a.name);
            });
            animClipResult = ProfilerData.GetMemByType<AnimationClip>((a) =>
            {
                return !string.IsNullOrEmpty(a.name);
            });

            luaNameSet.Clear();
            luaResult = ProfilerData.GetMemByType<TextAsset>((a) =>
            {
                var ln = a.name.ToLower();
                if (luaNameSet.Contains(ln))
                {
                    return false;
                }
                luaNameSet.Add(ln);
                return ln.Contains(".lua");
            });

            //csvNameSet.Clear();
            //csvResult = ProfilerData.GetMemByType<TextAsset>((a) =>
            //{
            //    var ln = a.name.ToLower();
            //    if (csvNameSet.Contains(ln))
            //    {
            //        return false;
            //    }
            //    csvNameSet.Add(ln);
            //    return ProfilerData.IsCSV(ln);
            //});

            luaMem = ProfilerData.GetLuaRuntimeMem();
        }
#endif
    }
}

