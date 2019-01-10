using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class LoadAssetUpdateRoot : MonoBehaviour {


    void Awake()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("UI/Prefabs/AssetUpdateUI/AssetUpdateUIRoot"));
    }

    [ContextMenu("ShowUserPath")]
    void ShowUserPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }
}

