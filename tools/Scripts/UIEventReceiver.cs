using XLua;
﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SG
{
    /// <summary>
    /// 不进行任何绘制的图像，只用于接收事件。
    /// </summary>
[Hotfix]
	public class UIEventReceiver : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// UIEventReceiver的编辑页面。
    /// </summary>
    [CustomEditor(typeof(UIEventReceiver))]
    [ExecuteInEditMode]
[Hotfix]
    public class UIEventReceiverEditor : Editor
    {
        /// <summary>
        /// 绘制面板。
        /// </summary>
        public override void OnInspectorGUI()
        {
            //什么也不绘制
        }
    }

#endif
}


