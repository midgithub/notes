/**
* @file     : UIPolygon.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using XLua;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SG
{
    /// <summary>
    /// 多边形检测，需要画布的渲染模式为"camera"
    /// </summary>
    [RequireComponent(typeof(PolygonCollider2D))]
[Hotfix]
    public class UIPolygon : Image
    {
        private PolygonCollider2D _polygon = null;
        private PolygonCollider2D polygon
        {
            get
            {
                if (_polygon == null)
                    _polygon = GetComponent<PolygonCollider2D>();
                return _polygon;
            }
        }
        protected UIPolygon()
        {
            useLegacyMeshGeneration = true;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (eventCamera == null)
            {
                LogMgr.LogWarning("UIPolygon need the canvas render mode is camera!");
                return false;
            }

            float z = canvas.planeDistance;
            Vector3 wp = eventCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, z));
            bool ok = polygon.OverlapPoint(wp);
            return ok;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            transform.localPosition = Vector3.zero;
            float w = (rectTransform.sizeDelta.x * 0.5f) + 0.1f;
            float h = (rectTransform.sizeDelta.y * 0.5f) + 0.1f;
            polygon.points = new Vector2[]
            {
            new Vector2(-w,-h),
            new Vector2(w,-h),
            new Vector2(w,h),
            new Vector2(-w,h)
              };
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIPolygon), true)]
[Hotfix]
    public class UIPolygonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
#endif
}

