/**
* @file     : UIEventListener.cs
* @brief    : 
* @details  :  UI事件封闭类 
* @author   : 
* @date     : 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XLua;

namespace uGUI
{ 
    /// <summary>
    /// Event Hook class lets you easily add remote event listener functions to an object.
    /// Example usage: UIEventListener.Get(gameObject).onClick += MyClickFunction;
    /// </summary>
    [LuaCallCSharp]

    public class UIEventListener : UnityEngine.EventSystems.EventTrigger
    {
        [CSharpCallLua]
        public delegate void VoidDelegate(GameObject go);
        [CSharpCallLua]
        public delegate void PointerEventDelegate(GameObject go, PointerEventData data);
        [CSharpCallLua]
        public delegate void AxisEventDelegate(GameObject go, AxisEventData data);
        public PointerEventDelegate onDrag;
        public PointerEventDelegate onDrop;        
        public AxisEventDelegate onMove;
        public VoidDelegate onPointerClick;
        public VoidDelegate onPointerClickSelf;
        public VoidDelegate onPointerDown;
        public VoidDelegate onPointerEnter;
        public VoidDelegate onPointerExit;
        public VoidDelegate onPointerUp;
        public VoidDelegate onScroll;
        public VoidDelegate onSelect;
        public VoidDelegate onSubmit;
        public VoidDelegate onUpdateSelected;
        public VoidDelegate onDragOut;
        public VoidDelegate onDragBegin;


        /// <summary>
        /// Get or add an event listener to the specified game object.
        /// </summary>
        static public UIEventListener Get(GameObject go)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null) listener = go.AddComponent<UIEventListener>();
            return listener;
        }

        //public virtual void OnBeginDrag(PointerEventData eventData);
        //public virtual void OnCancel(BaseEventData eventData);
        //public virtual void OnDeselect(BaseEventData eventData);
        //public virtual void OnDrag(PointerEventData eventData);
        //public virtual void OnDrop(PointerEventData eventData);
        //public virtual void OnEndDrag(PointerEventData eventData);
        //public virtual void OnInitializePotentialDrag(PointerEventData eventData);
        //public virtual void OnMove(AxisEventData eventData);
        //public virtual void OnPointerClick(PointerEventData eventData);
        //public virtual void OnPointerDown(PointerEventData eventData);
        //public virtual void OnPointerEnter(PointerEventData eventData);
        //public virtual void OnPointerExit(PointerEventData eventData);
        //public virtual void OnPointerUp(PointerEventData eventData);
        //public virtual void OnScroll(PointerEventData eventData);
        //public virtual void OnSelect(BaseEventData eventData);
        //public virtual void OnSubmit(BaseEventData eventData);
        //public virtual void OnUpdateSelected(BaseEventData eventData);

        public override void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null) onDrag(gameObject, eventData);
        }
        public override void OnDrop(PointerEventData eventData)
        {
            if (onDrop != null) onDrop(gameObject, eventData);
        }
        public override void OnMove(AxisEventData eventData)
        {
            if (onMove != null) onMove(gameObject, eventData);
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onPointerClickSelf != null && eventData.pointerPressRaycast.gameObject == this.gameObject)
            {
                onPointerClickSelf(gameObject);
            }
            if (onPointerClick != null) 
            { 
                onPointerClick(gameObject);
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDown != null) onPointerDown(gameObject);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onPointerEnter != null) onPointerEnter(gameObject);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onPointerExit != null) onPointerExit(gameObject);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onPointerUp != null) onPointerUp(gameObject);
        }
        //public override void OnScroll(PointerEventData eventData)
        //{
        //    if (onScroll != null) onScroll(gameObject);
        //}
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelected != null) onUpdateSelected(gameObject);
        }


        public override void OnEndDrag(PointerEventData data)
        {
            if (onDragOut != null) onDragOut(gameObject);
        }
        public override void OnBeginDrag(PointerEventData data)
        {
            if (onDragBegin != null) onDragBegin(gameObject);
        }
    }

}

