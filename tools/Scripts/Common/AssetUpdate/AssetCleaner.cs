/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using SG;

namespace Bundle
{
[Hotfix]
    public class AssetCleaner
    {
        private static string rootPath = string.Empty;
        public static void StartClean(List<AssetBundleConfig> remoteConfig, List<AssetBundleConfig> localConfig, List<AssetBundleConfig> streamConfig)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                rootPath = Application.persistentDataPath + "/" + BundleCommon.ResVersion;
            }

            new AssetCleaner(remoteConfig, localConfig, streamConfig);
        }

        private List<AssetBundleConfig> remoteConfig;
        private List<AssetBundleConfig> localConfig;
        private List<AssetBundleConfig> streamConfig;
        private AssetCleaner(List<AssetBundleConfig> remoteConfig, List<AssetBundleConfig> localConfig, List<AssetBundleConfig> streamConfig)
        {
            this.remoteConfig = remoteConfig;
            this.localConfig = localConfig;
            this.streamConfig = streamConfig;

            Thread thread = new Thread(DoClean);
            thread.Start();
        }

        private bool NeedRemove(string abName)
        {
            for (int i = 0; i < remoteConfig.Count; i++)
            {
                if (remoteConfig[i].ABName.Equals(abName))
                {
                    return false;
                }
            }

            return true;
        }

        private void DoClean()
        {
            if (null == remoteConfig || remoteConfig.Count == 0)
            {
                return;
            }

            if ((null == localConfig || localConfig.Count == 0) && (null == streamConfig || streamConfig.Count == 0))
            {
                return;
            }

            List<string> fileList = new List<string>();
            if (null != localConfig)
            {
                for (int i = 0; i < localConfig.Count; i++)
                {
                    AssetBundleConfig config = localConfig[i];
                    if (NeedRemove(config.ABName))
                    {
                        fileList.Add(string.Format("{0}/Out{1}" , rootPath, config.RelativePath));
                    }
                }
            }
            if(null != streamConfig)
            {
                for(int i = 0;i < streamConfig.Count;i++)
                {
                    AssetBundleConfig config = streamConfig[i];
                    if (NeedRemove(config.ABName))
                    {
                        fileList.Add(string.Format("{0}/Inner{1}" , rootPath, config.RelativePath));
                    }
                }
            }

            for(int i = 0;i < fileList.Count;i++)
            {
                string f = fileList[i];
                if(File.Exists(f))
                {
                    File.Delete(f);
                }
                else
                {
                    LogMgr.UnityError("clean file doesn't exist at path:" + f);
                }
            }
        }
    }
}

