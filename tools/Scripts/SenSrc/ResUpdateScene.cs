using UnityEngine;
using System.Collections;
using SG;
using XLua;

[Hotfix]
public class ResUpdateScene : MonoBehaviour {

	void Start () {
       CoreEntry.gResLoader.ClonePre("UI/Prefabs/AssetUpdateUI/AssetUpdateUIRoot");
    }
}
