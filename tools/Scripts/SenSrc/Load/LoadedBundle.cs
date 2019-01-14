using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using SenLib;

[Hotfix]
public class LoadedBundle: BundleLoaded
{
    public LoadedBundle(string abName, AssetBundle ab, int refCount)
         : base(abName, ab, refCount)
    {
       
    }

    protected override void UnloadDependencies()
    {
        string[] dependencies = LoadModule.Instance.GetDependencies(m_abName);
        for (int i = 0; i < dependencies.Length; ++i)
        {
            LoadedBundleCtrl.Instance.UnReferenceLoadedBundle(dependencies[i]);
        }
    }
}
