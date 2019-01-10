/**
* @file     :  
* @brief    : 
* @details  :  
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using XLua;
using UnityEngine.UI;
namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class PlayerModelDragInfo : MonoBehaviour
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
        public void OnEnable()
        { 
            Invoke("DelaySetRaw", 0.001f);
        }
        public void DelaySetRaw()
        {
            RawImage raw = this.GetComponent<RawImage>();
            if (raw != null && PlayerModelShowInfo.CurModeShow  != null&& PlayerModelShowInfo.CurModeShow.mCarmera != null)
            {
                raw.texture = PlayerModelShowInfo.CurModeShow.mCarmera.targetTexture;
                raw.material = AtlasSpriteManager.Instance.RTMaterial; 
            }

        }
        void OnDrag(Vector2 delta)
        {
            if (Math.Abs(delta.x) > 0)
            {
                float dt = delta.x / Math.Abs(delta.x) * 8;
                PlayerModelShowInfo.Rotate(new Vector3(0f, -dt, 0f));
            }
        }
    }
}

