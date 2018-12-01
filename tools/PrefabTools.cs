using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class PrefabTools
{
    private static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    //[MenuItem("Assets/模型工具/替换模型预设引用的@fbx 动画文件")]
    //private static void ReplacePrefabAnimationClip()
    //{
    //    FindPrefabs();
    //    StartReplace();
    //}

    [MenuItem("Assets/模型工具/删除 @.FBX 文件")]
    private static void DelUnusedFbx()
    {
        FindPrefabs();
        StartDel();
    }

    [MenuItem("Assets/模型工具/检测预设动画组件")]
    private static void CheckPrefabAnim()
    {
        FindPrefabs();
        CheckePrefabAnim();
    }

    private static void FindPrefabs()
    {
        string[] guids = null;
        List<string> path = new List<string>();
        prefabs.Clear();

        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        if (objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetType() == typeof(GameObject))
                {
                    string assetPath = AssetDatabase.GetAssetPath(objs[i]);
                    GameObject prefab = GetPrefabAtPath(assetPath);
                    if (prefab != null)
                    {
                        //Debug.Log(assetPath + " prefab Name: " + prefab.name);
                        prefabs.Add(assetPath, prefab);
                    }
                }
                else
                {
                    path.Add(AssetDatabase.GetAssetPath(objs[i]));
                }
            }

            if (path.Count > 0)
            {
                guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(GameObject).ToString().Replace("UnityEngine.", "")), path.ToArray());
            }
            else
            {
                guids = new string[] { };
            }
        }

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject prefab = GetPrefabAtPath(assetPath);
            if (prefab != null)
            {
                //Debug.Log(assetPath + " prefab Name: " + prefab.name);
                prefabs.Add(assetPath, prefab);
            }
        }
    }

    static void StartReplace()
    {
        int n = 0;
        foreach (var item in prefabs)
        {
            UpdateProgress(++n, prefabs.Count, item.Key);
            if (item.Key.EndsWith(".prefab"))
            {
                Animation animationComponent = item.Value.GetComponent<Animation>();
                if(null == animationComponent)
                {
                    Debug.Log(item.Key + " Animation 组件丢失----");
                    continue;
                }
                AnimationClip[] clips = AnimationUtility.GetAnimationClips(animationComponent.gameObject);
                for (int i = 0; i < clips.Length; i++)
                {
                    string name = clips[i].name;
                    Debug.Log(name);
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("replace animtion clip over---");
    }

    static void StartDel()
    {
        int n = 0;
        foreach (var item in prefabs.Keys)
        {
            UpdateProgress(++n, prefabs.Count, item);
            if (item.Contains("@") && item.EndsWith(".FBX"))
            {
                Debug.Log("删除 @fbx 文件：" + item);
                AssetDatabase.DeleteAsset(item);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("删除 @FBX 文件完毕r---");
    }

    static void CheckePrefabAnim()
    {
        int n = 0;
        foreach (var item in prefabs)
        {
            //Debug.Log("检测文件： " + item.Key);
            UpdateProgress(++n, prefabs.Count, item.Key);
            if (item.Key.EndsWith(".prefab"))
            {
                Animation animationComponent = item.Value.GetComponent<Animation>();
                if (null == animationComponent)
                {
                    Debug.LogWarning(item.Key + " Animation 组件丢失----");
                    continue;
                }
                AnimationClip[] clips = AnimationUtility.GetAnimationClips(animationComponent.gameObject);
                for (int i = 0; i < clips.Length; i++)
                {
                    if(null == clips[i])
                    {
                        Debug.LogError(item.Key + " 动画组件引用动画为空--");
                        break;
                    } 
                }
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log("检测预设动画组件完毕---");
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    static GameObject GetPrefabAtPath(string path)
    {
        GameObject pre = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        return pre;
    }
}
