/**
* @file     : PlayerModelDrag.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using XLua;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class PlayerModelDrag : MonoBehaviour
    {
        /// <summary>
        /// 是否其它玩家。
        /// </summary>
        public bool IsOther = false;

        void Awake()
        {
            uGUI.UIEventListener.Get(gameObject).onDrag = OnDragEvent;
        }

        public void OnDragEvent(GameObject go, PointerEventData data)
        {
            OnDrag(data.delta);
        }

        void OnDrag(Vector2 delta)
        {
            if (Math.Abs(delta.x) > 0)
            {
                float dt = delta.x / Math.Abs(delta.x) * 8;
                if (IsOther)
                {
                    OtherPlayerModelShow.Rotate(new Vector3(0f, -dt, 0f));
                }
                else
                {
                    PlayerModelShowInfo.Rotate(new Vector3(0f, -dt, 0f));
                }                
            }
        }
    }
}

