/**
* @file     : RideModelDrag.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using XLua;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class RideModelDrag : MonoBehaviour
    {
        void Awake()
        {
            uGUI.UIEventListener.Get(gameObject).onDrag = OnDragEvent;
            uGUI.UIEventListener.Get(gameObject).onPointerClick = OnDropEvent;
        }

        public void OnDragEvent(GameObject go, PointerEventData data)
        {
            OnDrag(data.delta);
        }


        public void OnDropEvent(GameObject go)
        {
            OnClick(go);
        } 
        public void OnEnable()
        {
            Invoke("DelaySetRaw", 0.001f);
        }
        public void DelaySetRaw()
        {
            RawImage raw = this.GetComponent<RawImage>();
            if (raw)
            {
                raw.texture = RideModelShow.CurModeShow.mCarmera.targetTexture;
                raw.material = AtlasSpriteManager.Instance.RTMaterial;
            }

        }

        void OnClick(GameObject go)
        {
            RideModelShow.ShowHorseActionAnimation();
        }

        void OnDrag(Vector2 delta)
        {
            if (Math.Abs(delta.x) > 0)
            {
                float dt = delta.x / Math.Abs(delta.x) * 4;
                RideModelShow.Rotate(new Vector3(0f, -dt, 0f));
            }
        }
    }
}

