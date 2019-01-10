using XLua;
﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.uGUI
{
[Hotfix]
    internal class UISortingOrder : MonoBehaviour
    {
        public int m_Order = 0;
        public bool m_IsUI = true;
        static public UISortingOrder Get(GameObject go)
        {
            UISortingOrder compoment = go.GetComponent<UISortingOrder>();
            if (compoment == null)
            {
                compoment = go.AddComponent<UISortingOrder>();
            }
            return compoment;
        }
      
        public void SetOrder(int order, bool isUI)
        {
            m_Order = order;
            m_IsUI = isUI;
            SetOrder();
        }
        
        /// <summary>
        /// From MonoBehaviour
        /// </summary>
        private void Start()
        {
            SetOrder();
        }
        private void SetOrder()
        {
            if (m_IsUI)
            {
                Canvas canvas = GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = gameObject.AddComponent<Canvas>();
                }
                canvas.overrideSorting = true;
                canvas.sortingOrder = m_Order;
            }
            else
            {
                Renderer[] renders = GetComponentsInChildren<Renderer>();
                foreach (Renderer render in renders)
                {
                    render.sortingOrder = m_Order;
                }
            }
        }
    }
}

