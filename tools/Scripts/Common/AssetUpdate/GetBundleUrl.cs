using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using System.IO;

namespace Bundle
{
[Hotfix]
    public class GetBundleUrl
    {
        private AssetUpdateMgr mParent;
        private DownLoadTxtFileCallBack mFinishCallback;

        /// <summary>
        /// 获取远程更新地址
        /// </summary>
        /// <param name="finishCallback"></param>
        /// <param name="parent"></param>
        public void GetRemoteDownloadUrl(DownLoadTxtFileCallBack finishCallback, AssetUpdateMgr parent)
        {
            mParent = parent;
            mFinishCallback = finishCallback;

            new TextDownLoadTask(BundleCommon.RootUrl + BundleCommon.RootUrlFileName, GetUrlCallback, UpdateGetUrlProgress);
        }

        /// <summary>
        /// 远程地址获取回调
        /// </summary>
        /// <param name="success"></param>
        /// <param name="content"></param>
        private void GetUrlCallback(bool success, string content)
        {
            if (success)
            {
                BundleCommon.BaseUrl = content.Split(new char[2]{'\r', '\n'})[0];

                BulletinMgr.Instance.WriteBulletin(content);
            }

            if(null != mFinishCallback)
            {
                mFinishCallback(success, content);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progress"></param>
        private void UpdateGetUrlProgress(float progress)
        {
            mParent.UpdateProgress("正在获取更新资源地址", progress);
        }
    }
}

