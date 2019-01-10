using UnityEngine;
using System.Collections;
using SG;
using XLua;

[Hotfix]
public class DllUpdateScene : MonoBehaviour {

	void Start () {
        CoreEntry.gResLoader.ClonePre("UI/Prefabs/DllUpdate/DllUpdateUIRoot");
    }
}
