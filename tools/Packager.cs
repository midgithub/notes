using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class Packager
{
    private enum BuildType
    {
        AssetBundleBuild,
        AssetImporter,
    }
    static BuildType buildType = BuildType.AssetImporter;

    private enum MType
    {
        UI,
        Lua,
        World,
        All,
    }

    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();
    static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();
    static Dictionary<string, List<string>> depends = new Dictionary<string, List<string>>();
    static List<string> bundleFiles = new List<string>();
    static List<string> searched = new List<string>();

    static bool keepManifet = true;
    static bool justSetNames = false;  //查看依赖使用
    static List<string> folderBundles = new List<string>();
    static MType curBundleType = MType.Lua;

    [MenuItem("SenTools/Set Bundle Name/UI", false, 101)]
    static void SetUIBundleNames()
    {
        SetBundleNames(MType.UI);
    }

    [MenuItem("SenTools/Set Bundle Name/Lua", false, 102)]
    static void SetLuaBundleNames()
    {
        SetBundleNames(MType.Lua);
    }

    [MenuItem("SenTools/Set Bundle Name/World", false, 103)]
    static void SetWorldBundleNames()
    {
        SetBundleNames(MType.World);
    }

    [MenuItem("SenTools/Set Bundle Name/All", false, 104)]
    static void SetAllBundleNames()
    {
        SetBundleNames(MType.All);
    }

    private static void SetBundleNames(MType mType)
    {
        ClearBundleName();

        BuildType defaultType = buildType;
        justSetNames = true;
        buildType = BuildType.AssetImporter;

        if(mType == MType.UI)
        {
            HandleUIBundle();
        }
        else if (mType == MType.Lua)
        {
            HandleLuaBundle();
        }
        else if (mType == MType.World)
        {
            HandleWorldBundle();
        }
        else if (mType == MType.All)
        {
            HandleAllBundle();
        }

        buildType = defaultType;
        justSetNames = false;

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("Set Assets Bundle Name Over----");
    }

    [MenuItem("SenTools/Clear Assets Bundle Name", false, 104)]
    static void ClearBundleNames()
    {
        ClearBundleName();
        AssetDatabase.Refresh();
        Debug.Log("Clear Asset Bundle Name Over----");
    }

    [MenuItem("SenTools/Publish/Export All Assets", false, 100)]
    static void HandleAllBundle()
    {
        HandleUIBundle();
        HandleLuaBundle();
        HandleWorldBundle();
    }

    [MenuItem("SenTools/Publish/Export UI Assets", false, 101)]
    static void HandleUIBundle()
    {
        curBundleType = MType.UI;
        // 清空文件夹
        string strDir = "/StreamingAssets/UI_Bundles";
        string bundlePath = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;

        if (!justSetNames)
        {
            DelAndNewDir(strDirPath);
            ClearBundleName();
        }

        BuildInit();
        SetUIfolderBundles();

        string searchDir = AppConst.ResDataDir + "UI/";
        string dataPath = Application.dataPath + "/";

        Recursive(dataPath + searchDir);

        string ext = string.Empty;
        string bundleName = string.Empty;
        string bundleFile = string.Empty;

        int n = 0;
        foreach (string f in files)
        {
            ext = Path.GetExtension(f);
            bundleFile = "Assets/" + f.Replace(dataPath, "");
            bundleName = "ui/" + (f.Replace(dataPath, "").Replace(searchDir, "").Replace(ext, "") + AppConst.ExtName).ToLower();

            AddDepends(bundleFile);

            SetFileBundleName(bundleFile, bundleName);
            UpdateProgress(n++, files.Count, bundleFile);
        }

        UpdateProgress(files.Count, files.Count, "Export UI bundles over");

        BuildDependsBundle();
        WriteRefInfoFile(strDirPath);

        StartBuild(bundlePath);

        BuildEnd(strDirPath);
    }

    [MenuItem("SenTools/Publish/Export Lua Assets", false, 102)]
    static void HandleLuaBundle()
    {
        curBundleType = MType.Lua;
        // 清空文件夹
        string strDir = "/StreamingAssets/Lua_Bundles";
        string bundlePath = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;

        if (!justSetNames)
        {
            DelAndNewDir(strDirPath);
            ClearBundleName();
        }

        BuildInit();

        string dataPath = Application.dataPath + "/";

        Recursive(dataPath + AppConst.ResDataDir + "Lua/");
        Recursive(dataPath + AppConst.ResDataDir + "Data/");

        string ext = string.Empty;
        string dir = string.Empty;
        string bundleName = string.Empty;
        string bundleFile = string.Empty;

        int n = 0;
        foreach (string f in files)
        {
            ext = Path.GetExtension(f);
            dir = Path.GetDirectoryName(f);
            bundleFile = "Assets/" + f.Replace(dataPath, "");
            //bundleName = f.Replace(dataPath, "").Replace(AppConst.ResDataDir, "").Replace(ext, "").ToLower() + AppConst.ExtName;
            bundleName = dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "").ToLower() + AppConst.ExtName;

            SetFileBundleName(bundleFile, bundleName);

            UpdateProgress(n++, files.Count, bundleFile);
        }

        UpdateProgress(files.Count, files.Count, "Export Lua bundles over");

        StartBuild(bundlePath);

        BuildEnd(strDirPath);
    }

    [MenuItem("SenTools/Publish/Export World Assets", false, 103)]
    static void HandleWorldBundle()
    {
        curBundleType = MType.World;
        // 清空文件夹
        string strDir = "/StreamingAssets/World_Bundles";
        string bundlePath = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;

        if (!justSetNames)
        {
            DelAndNewDir(strDirPath);
            ClearBundleName();
        }

        BuildInit();
        SetWorldfolderBundles();

        string searchDir = AppConst.ResDataDir;
        string dataPath = Application.dataPath + "/";

        Recursive(dataPath + searchDir);

        string ext = string.Empty;
        string bundleName = string.Empty;
        string bundleFile = string.Empty;

        int n = 0;
        foreach (string f in files)
        {
            UpdateProgress(n++, files.Count, bundleFile);

            ext = Path.GetExtension(f);
            bundleFile = "Assets/" + f.Replace(dataPath, "");
            bundleName = (f.Replace(dataPath, "").Replace(searchDir, "").Replace(ext, "") + AppConst.ExtName).ToLower();
            //var items = bundleName.Split('.');
            //if (items.Length > 2)
            //{
            //    bundleName = items[0] + "." + items[items.Length - 1];
            //}

            //过滤 lua 和 UI Data
            string head = bundleName.Split('/')[0];
            if (head.Equals("lua") || head.Equals("ui") || head.Equals("data"))
            {
                continue;
            }

            AddDepends(bundleFile);
            SetFileBundleName(bundleFile, bundleName);
        }

        UpdateProgress(files.Count, files.Count, "Export 3d Assets bundles over");

        HandleSceneBundle();

        BuildDependsBundle();
        WriteRefInfoFile(strDirPath);

        StartBuild(bundlePath);

        BuildEnd(strDirPath);
    }

    /// <summary>
    /// 处理scene
    /// </summary>
    static void HandleSceneBundle()
    {
        string dataPath = Application.dataPath + "/";
        string scenePath = dataPath + "Scene/";

        paths.Clear();
        files.Clear();
        Recursive(scenePath);

        string ext = string.Empty;
        string sceneName = string.Empty;
        string bundleName = string.Empty;
        string bundleFile = string.Empty;

        int n = 0;
        foreach (string f in files)
        {
            if (f.EndsWith(".unity"))
            {
                ext = Path.GetExtension(f);
                sceneName = f.Substring(f.LastIndexOf("/") + 1);

                bundleFile = "Assets/" + f.Replace(dataPath, "");
                bundleName = "scenes/" + (sceneName.Replace(ext, "") + AppConst.ExtName).ToLower();

                AddDepends(bundleFile);
                SetFileBundleName(bundleFile, bundleName);

                UpdateProgress(n++, files.Count, bundleFile);
            }
        }

        UpdateProgress(files.Count, files.Count, "Scenes bundle over");
    }

    static void DelAndNewDir(string dir)
    {
        DirectoryInfo dirOutDir = new DirectoryInfo(dir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(dir);
        }
        else
        {
            Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
        }
    }

    static void BuildInit()
    {
        maps.Clear();
        depends.Clear();
        bundleFiles.Clear();
        searched.Clear();

        paths.Clear();
        files.Clear();

        folderBundles.Clear();
    }

    static void StartBuild(string outputPath)
    {
        if (justSetNames) return;

        if (buildType == BuildType.AssetBundleBuild)
        {
            BuildPipeline.BuildAssetBundles(outputPath, maps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
        else
        {
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
    }

    static void BuildEnd(string path)
    {
        if (justSetNames) return;

        ClearBundleName();
        ClearManifestFile(path);

        AssetDatabase.Refresh();
    }

    static void AddDepends(string bundleFile)
    {
        if (searched.Contains(bundleFile)) return;
        searched.Add(bundleFile);

        string[] ds = AssetDatabase.GetDependencies(bundleFile);
        string dependFile = string.Empty;
        for (int i = 0; i < ds.Length; i++)
        {
            dependFile = ds[i];
            if (dependFile.Equals(bundleFile) || dependFile.EndsWith(".cs") || dependFile.EndsWith(".asset") || dependFile.EndsWith(".exr") || dependFile.EndsWith(".js"))
            {
                continue;
            }

            if (!depends.ContainsKey(dependFile))
            {
                depends.Add(dependFile, new List<string>());
            }

            if (!depends[dependFile].Contains(bundleFile))
            {
                depends[dependFile].Add(bundleFile);
            }
            else
            {
                Debug.LogError(dependFile + " 多次添加被依赖----- " + bundleFile);
            }

            AddDepends(dependFile);
        }
    }

    static void BuildDependsBundle()
    {
        string baseFolder = "";
        if(curBundleType == MType.UI)
        {
            baseFolder = "ui/";
        }

        string bundleFile = string.Empty;
        string bundleName = string.Empty;
        string ext = string.Empty;

        int n = 0;
        foreach (var item in depends)
        {
            UpdateProgress(n++, depends.Count, item.Key);
            if (item.Value.Count > 1)
            {
                bundleFile = item.Key;

                ext = bundleFile.Substring(bundleFile.LastIndexOf("."));
                bundleName = baseFolder + "depends/" + (bundleFile.Replace("Assets/","").Replace(ext, "") + AppConst.ExtName).ToLower(); //单独打包

                bool checkFolderBendle = true;
                if (ext.IsTexture())
                {
                    checkFolderBendle = false;

                    string shader = "Assets/" + AppConst.ResDataDir + "Shaders";
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        if (item.Value[i].StartsWith(shader) && item.Value[i].EndsWith(".shader"))
                        {
                            bundleName = baseFolder + "shaders" + AppConst.ExtName;
                            break;
                        }
                    }
                }

                SetFileBundleName(bundleFile, bundleName, checkFolderBendle);
            }
        }

        UpdateProgress(depends.Count, depends.Count, "depend assets build over");
    }

    static void ClearManifestFile(string path)
    {
        if (justSetNames) return;

        paths.Clear();
        files.Clear();
        Recursive(path);

        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            if (!keepManifet && file.EndsWith(".manifest"))
            {
                File.Delete(file);
                continue;
            }
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }


    // 清除AssetBundle设置
    public static void ClearBundleName()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (string name in names)
        {
            AssetDatabase.RemoveAssetBundleName(name, true);
        }
    }

    private static void SetUIfolderBundles()
    {
        string dataPath = Application.dataPath + "/";
        string[] dirs = Directory.GetDirectories(dataPath + "ResData/UI");
        foreach (string dir in dirs)
        {
            string bundleName = dir.Replace('\\', '/').Replace(dataPath, "").Replace("ResData/", "");
            if(!bundleName.Equals("UI/Atlas") && !bundleName.Equals("UI/Prefabs") && !bundleName.Equals("UI/depends"))
            {
                folderBundles.Add(bundleName.ToLower());
            }
            else if (bundleName.Equals("UI/Prefabs"))
            {
                string[] dirs1 = Directory.GetDirectories(dataPath + "ResData/UI/Prefabs");
                foreach (string dir1 in dirs1)
                {
                    string bundleName1 = dir1.Replace('\\', '/').Replace(dataPath, "").Replace("ResData/", "");
                    folderBundles.Add(bundleName1.ToLower());
                }
            }
        }

        folderBundles.Add("ui/depends/igsoft_resources");
        folderBundles.Add("ui/depends/resdata/shaders");
    }

    private static void SetWorldfolderBundles()
    {
        folderBundles.Add("shaders");
        folderBundles.Add("depends/icefire");
        folderBundles.Add("depends/igsoft_resources");
        folderBundles.Add("depends/projector");
        folderBundles.Add("depends/t4m");
        folderBundles.Add("depends/t4mobj");

        string dataPath = Application.dataPath + "/";
        AddFolderBundles(dataPath + "AssetsRaw/Animation");
        AddFolderBundles(dataPath + "AssetsRaw/Effect");
        AddFolderBundles(dataPath + "AssetsRaw/Scene");

        Debug.Log("SetWorldfolderBundles ---");
    }

    static bool AddFolderBundles(string path)
    {
        bool addChild = false;
        string dataPath = Application.dataPath + "/";
        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            if (!dir.EndsWith("Materials") && !dir.EndsWith("ClipCurve") && !dir.EndsWith("path"))
            {
                string bundleName = "depends/" + dir.Replace('\\', '/').Replace(dataPath, "").ToLower();
                if (!AddFolderBundles(dir))
                {
                    bool hasChild = false;
                    for (int i = 0; i < folderBundles.Count; i++)
                    {
                        if (folderBundles[i].StartsWith(bundleName))
                        {
                            hasChild = true;
                        }
                    }

                    if (!hasChild)
                    {
                        folderBundles.Add(bundleName);
                        addChild = true;
                    }
                }
            }
        }

        return addChild;
    }

    private static void SetFileBundleName(string file, string abName,bool checkFolderBundle = true)
    {
        if (bundleFiles.Contains(file)) return;
        bundleFiles.Add(file);

        //临时特殊处理下
        //if (file.StartsWith("Assets/AssetsRaw/Effect/Textures"))
        //{
        //    string baseFolder = "";
        //    if (curBundleType == MType.UI)
        //    {
        //        baseFolder = "ui/";
        //    }

        //    var fileName = Path.GetFileName(file);
        //    if (fileName.EndsWith(".mat"))
        //    {
        //        if(file.Replace(fileName, "").EndsWith("Material/"))
        //        {
        //            abName = baseFolder + "depends/" + file.Replace("Assets/", "").Replace(".mat","").ToLower() + AppConst.ExtName;
        //        }
        //        else
        //        {
        //            abName = baseFolder + "depends/" + file.Replace("Assets/", "").Replace(fileName, "").Replace("Materials/", "").ToLower() + "mats" + AppConst.ExtName;
        //        }
        //    }
        //    else
        //    {
        //        abName = baseFolder + "depends/" + file.Replace("Assets/", "").Replace("/" + fileName, "").ToLower() + AppConst.ExtName;
        //    }
        //}

        if(checkFolderBundle)
        {
            for (int i = 0; i < folderBundles.Count; i++)
            {
                if (abName.StartsWith(folderBundles[i]))
                {
                    abName = folderBundles[i] + AppConst.ExtName;
                    break;
                }
            }
        }

        if (buildType == BuildType.AssetBundleBuild)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = abName;
            build.assetNames = new string[] { file };
            maps.Add(build);
        }
        else
        {
            AssetImporter importer = AssetImporter.GetAtPath(file);
            if (importer == null)
            {
                Debug.LogError("[路径错误] path: " + file);
                return;
            }

            importer.assetBundleName = abName;
        }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        string ext = string.Empty;
        foreach (string filename in names)
        {
            ext = Path.GetExtension(filename);
            if (ext.Equals(".meta") || ext.Equals(".cs")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    //写引用文件
    static void WriteRefInfoFile(string folder)
    {
        ///----------------------创建文件列表-----------------------
        string newFilePath = folder + "/refInfo.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);

        foreach (var item in depends)
        {
            if (item.Value.Count > 1)
            {
                sw.WriteLine(item.Key);

                for (int i = 0; i < item.Value.Count; i++)
                {
                    sw.WriteLine(string.Format("    {0}: {1}", i, item.Value[i]));
                }

                sw.WriteLine("");
            }
        }

        sw.Close();
        fs.Close();
    }

    static void BuildFileIndex()
    {
        string resPath = Application.dataPath + "/StreamingAssets/";
        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear();
        files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.EndsWith(".manifest")) continue;

            string md5 = Util.md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }

        sw.Close();
        fs.Close();
    }

    [MenuItem("SenTools/Apk Build", false, 106)]
    static void PublishAndroidAPK()
    {
        BuildFileIndex();

        string curDir =  Directory.GetCurrentDirectory();
        curDir = curDir.Replace('\\', '/');
        curDir = curDir + "/AndroidApks/";
        DirectoryInfo dirOutDir = new DirectoryInfo(curDir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(curDir);
        }

        string apkName = DateTime.Now.ToString("yyyymmddhhmmss") +"-rpg.apk";
        string path = curDir + apkName;

        var err = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        bool success = string.IsNullOrEmpty(err);

        if (success)
        {
            Debug.Log(path + " Publish Done! "); 
        }
        else
        {
            Debug.Log(path + " Publish error! ");
        }
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null) continue;
            if (e.enabled && (e.path.Contains("ThirdPartyUI")))
            {
                names.Add(e.path);
            } 
        }

        return names.ToArray();
    }
}