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

namespace Bundle
{
    public enum ABFileState
    {
        NONE,
        STREAMEXIT,
        DOWNLOADED,
    }

[Hotfix]
    public class ABFileMgr
    {
        private static ABFileMgr _instance;
        public static ABFileMgr Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new ABFileMgr();
                }

                return _instance;
            }
        }

        private Dictionary<string, AssetBundleConfig> mABAllDict;
        private Dictionary<string, ABFileState> mABStateDict;
        private ABFileMgr()
        {
            mABStateDict = new Dictionary<string, ABFileState>();
            mABAllDict = new Dictionary<string, AssetBundleConfig>();
        }

        public void AddABFile(AssetBundleConfig config, ABFileState state = ABFileState.NONE)
        {
            mABStateDict[config.RelativePath] = state;
            mABAllDict[config.RelativePath] = config;
        }

        public void SetAllState(ABFileState state)
        {
            foreach (string key in mABStateDict.Keys)
            {
                mABStateDict[key] = state;
            }
        }

        public void SetABFileState(string abRelative, ABFileState state)
        {
            if (mABStateDict.ContainsKey(abRelative))
            {
                mABStateDict[abRelative] = state;
            }
        }

        public ABFileState GetABFileState(string abRelative)
        {
            ABFileState state = ABFileState.NONE;
            if (mABStateDict.ContainsKey(abRelative))
            {
                state = mABStateDict[abRelative];
            }

            return state;
        }

        public AssetBundleConfig GetABConfig(string abRelative)
        {
            if (mABAllDict.ContainsKey(abRelative))
            {
                return mABAllDict[abRelative];
            }

            return null;
        }
    }
}

