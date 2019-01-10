using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class TaskCheckScrollBar : MonoBehaviour, IEndDragHandler
    {
        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void SetLightState(int questType)
        {
            LogMgr.UnityWarning("TaskCheckScrollBar.SetLightState 已废弃");
        }
    }
}

