using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class ChangeShaderName
{
    private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

    [MenuItem("Assets/更改Shader名")]
    public static void ChangeName()
    {
        string[] guids = null;
        List<string> path = new List<string>();
        shaders.Clear();

        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        if (objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetType() == typeof(Shader))
                {
                    string assetPath = AssetDatabase.GetAssetPath(objs[i]);
                    Shader shader = GetShaderAtPath(assetPath);
                    if (shader != null)
                    {
                        Debug.Log(assetPath + " shaderName: " + shader.name);
                        shaders.Add(assetPath, shader);
                    }
                }
                else
                {
                    path.Add(AssetDatabase.GetAssetPath(objs[i]));
                } 
            }

            if (path.Count > 0)
            {
                guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Shader).ToString().Replace("UnityEngine.", "")), path.ToArray());
            }
            else
            {
                guids = new string[] { };
            }  
        }

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            Shader shader = GetShaderAtPath(assetPath);
            
            if (shader != null)
            {
                Debug.Log(assetPath + " shaderName: " + shader.name);
                shaders.Add(assetPath, shader);
            }  
        }

        StartChange();
    }

    static void StartChange()
    {
        int n = 0;
        foreach (var item in shaders)
        {
            UpdateProgress(++n, shaders.Count, item.Key);
            AssetDatabase.RenameAsset(item.Key, item.Value.name.Replace("/", "-").Replace(" ", ""));
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("find shaders over---");
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    static Shader GetShaderAtPath(string path)
    {
        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(path);

        //string mat = path.Replace(Path.GetFileName(path),"") + "Cube.prefab";
        //var ds = AssetDatabase.GetDependencies(mat);

        //AssetDatabase.RenameAsset(path, shader.name.Replace("/","-").Replace(" ",""));
        return shader;
    }
}
