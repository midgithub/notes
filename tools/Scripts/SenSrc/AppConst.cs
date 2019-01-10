using System.Collections.Generic;

public class AppConst
{
    public static bool DebugMode
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return true;
#endif
        }
    }

    public static bool UpdateMode
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }
    }

    /// <summary>
    /// 是否从bundle中加载资源
    /// </summary>
    public static bool UseAssetBundle
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            return true;
#endif
        }
    }

    public const bool UseResources = false;

    public const int SyncCount = 3;                             //同时加载并发数
    public const float AssetCacheTime = 60;						// 资源缓存时间
    public const int GameFrameRate = 30;                        //游戏帧频

    public const string AppName = "SenUnity";                //解压更新数据目录

    public const string ResDataDir = "ResData/";                   //资源目录
    public const string ExtName = ".unity3d";                   //扩展名
    public const string AssetDir = "StreamingAssets";           //素材目录
}
