using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FindShaderWithDefaultTexture : EditorWindow
{
    [MenuItem("SenTools/检查/检查带有默认贴图的shader")]
    private static void FindShaderWithTexture()
    {
        GetWindow<FindShaderWithDefaultTexture>().Show();
        Find();
    }

    private static List<UnityEngine.Object> res = new List<UnityEngine.Object>();

    private static void Find()
    {
        res.Clear();
        var allfiles = Directory.GetFiles("Assets/ResData/Shaders", "*.shader", SearchOption.AllDirectories);
        foreach (var file in allfiles)
        {
            foreach (var item in AssetDatabase.GetDependencies(file).Where(s => s.IsTexture()))
            {
                res.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file));
                break;
            }
        }
        EditorUtility.DisplayDialog("", "就绪", "OK");
    }

    private Vector2 scro = Vector2.zero;
    private void OnGUI()
    {
        scro = EditorGUILayout.BeginScrollView(scro);
        EditorGUILayout.BeginVertical();
        foreach (var item in res)
        {
            EditorGUILayout.ObjectField(item, typeof(UnityEngine.Object), true);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    [MenuItem("SenTools/检查/交叉资源")]
    private static void FindRefOtherTexture()
    {
        var allfiles = Directory.GetFiles("Assets/AssetsRaw/Effect/Textures", "*.mat", SearchOption.AllDirectories);
        foreach (var file in allfiles)
        {
            string fileNormal = file.Replace('\\', '/');
            var dps = AssetDatabase.GetDependencies(fileNormal);
            for (int i = 0; i < dps.Length; i++)
            {
                if (dps[i].Equals(fileNormal))
                {
                    continue;
                }
            }
        }
    }
}
