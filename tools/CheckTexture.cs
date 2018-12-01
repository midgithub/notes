using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class CheckTexture : MonoBehaviour
{
    private static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
    private static Dictionary<string, List<string>> refs = new Dictionary<string, List<string>>();

    [MenuItem("Assets/模型贴图工具(外部引用)")]
    private static void CheckTextureRef()
    {
        FindTextures();
        ReadFile();
        Check();
    }

    [MenuItem("Assets/模型贴图工具(压缩格式)")]
    private static void CheckTextureCompressFormat()
    {
        FindTextures();
        CheckFormat();
    }

    private static void Check()
    {
        int n = 0;
        foreach (var texture in textures.Keys)
        {
            UpdateProgress(++n, textures.Count, texture);
           
            if (refs.ContainsKey(texture))
            {
                bool refOut = false;
                for (int i = 0; i < refs[texture].Count; i++)
                {
                    string file = refs[texture][i];
                    if (!IsSimilar(texture, file))
                    {
                        refOut = true;
                        Debug.LogWarning(texture + " 被 " + file + " 引用--");
                    }
                }

                if (refOut)
                {
                    Debug.LogError(texture + " x需要移动路径！！！");
                }
            }
            else
            {
                Debug.LogWarning("贴图未被引用： " + texture);
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log("检测模型贴图完毕---");
    }

    static bool IsSimilar(string str1, string str2)
    {
        string teture = str1.Replace("Assets/", "").Replace("AssetsRaw/", "").Replace("Animation/", "");
        string[] tetureArr = teture.Split('/');

        string subStr = str2.Replace("Assets/", "").Replace("ResData/", "").Replace("AssetsRaw/", "").Replace("Animation/", "");
        string[] items = subStr.Split('/');

        if (tetureArr[0].Equals(items[0]))
        {
            return true;
        }

        return false;
    }

    private static void ReadFile()
    {
        //refs.Clear();

        //string refFile = Application.dataPath + "/StreamingAssets/World_Bundles" + "/refInfo.txt";
        //if (!File.Exists(refFile)) return;

        //string[] files = File.ReadAllLines(refFile);
        //string lastFile = string.Empty;
        //for (int i = 0; i < files.Length; i++)
        //{
        //    string file = files[i];
        //    if (file.StartsWith("Assets/") && file.IsTexture())
        //    {
        //        lastFile = file;
        //        refs.Add(lastFile, new List<string>());
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(file))
        //        {
        //            lastFile = string.Empty;
        //        }

        //        if (!string.IsNullOrEmpty(lastFile))
        //        {
        //            string s = ": ";
        //            string[] items = file.Split(s.ToCharArray());
        //            refs[lastFile].Add(items[items.Length-1]);
        //        }
        //    } 
        //}
    }

    private static void FindTextures()
    {
        string[] guids = null;
        List<string> path = new List<string>();
        textures.Clear();

        UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        if (objs.Length > 0)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetType() == typeof(Texture))
                {
                    string assetPath = AssetDatabase.GetAssetPath(objs[i]);
                    Texture texture = GetTextureAtPath(assetPath);
                    if (texture != null)
                    {
                        textures.Add(assetPath, texture);
                    }
                }
                else
                {
                    path.Add(AssetDatabase.GetAssetPath(objs[i]));
                }
            }

            if (path.Count > 0)
            {
                guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Texture).ToString().Replace("UnityEngine.", "")), path.ToArray());
            }
            else
            {
                guids = new string[] { };
            }
        }

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            Texture texture = GetTextureAtPath(assetPath);
            if (texture != null)
            {
                textures.Add(assetPath, texture);
            }
        }
    }

    static void CheckFormat()
    {
        int n = 0;
        foreach (var texture in textures)
        {
            UpdateProgress(++n, textures.Count, texture.Key);

            TextureImporter import = AssetImporter.GetAtPath(texture.Key) as TextureImporter;
            if(import.textureFormat != TextureImporterFormat.AutomaticCompressed)
            {
                import.textureFormat = TextureImporterFormat.AutomaticCompressed;

                //import.SetPlatformTextureSettings("Standalone", 512, TextureImporterFormat.AutomaticCompressed);
                //import.SetPlatformTextureSettings("iPhone", 512, TextureImporterFormat.AutomaticCompressed);
                //import.SetPlatformTextureSettings("Android", 512, TextureImporterFormat.ETC2_RGBA8);

                AssetDatabase.ImportAsset(texture.Key);
                Debug.LogError(string.Format("模型贴图 {0} 压缩格式有问题--", texture.Key));
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log("检测模型贴图压缩格式完毕---");
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    static Texture GetTextureAtPath(string path)
    {
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
        return texture;
    }
}
