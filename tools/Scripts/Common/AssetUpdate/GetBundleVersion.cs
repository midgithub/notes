using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SG;
using System.IO;

namespace Bundle
{

[Hotfix]
    public class GetBundleVersion
    {
        private AssetUpdateMgr mParent;
        private DownLoadTxtFileCallBack mFinishCallback;

        public void GetVersion(DownLoadTxtFileCallBack finishCallback, AssetUpdateMgr parent)
        {
            mParent = parent;
            mFinishCallback = finishCallback;

            new TextDownLoadTask(BundleCommon.RootUrl + BundleCommon.RootUrlVersionFileName, GetVersionCallback, UpdateGetVersionProgress);
        }

        private void GetVersionCallback(bool success, string content)
        {
            if (success)
            {
                BundleCommon.RootUrlVersion = content.Replace("\r", "").Replace("\n", "");
            }

            if (null != mFinishCallback)
            {
                mFinishCallback(success, content);
            }
        }

        private void UpdateGetVersionProgress(float progress)
        {
            mParent.UpdateProgress("正在获取资源版本", progress);
        }
    }
}

