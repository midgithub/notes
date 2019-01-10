using UnityEngine;
using System.Collections;
using XLua;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
[LuaCallCSharp]
[Hotfix]
public class InitOnStart : MonoBehaviour {
	void Start () {
        GetComponent<UnityEngine.UI.LoopScrollRect>().RefillCells();
	}
}

