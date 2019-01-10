/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : 
* @date     : 2014-11-03
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public static class PrefabPoolUtils
    {
        /// <summary>
        /// Changes gameObjects parent and resets transform and layer.
        /// </summary>
        /// <param name="parent">New parent</param>
        /// <param name="child">Child whose parent needs to be changed</param>
        public static void AddChild(GameObject parent, GameObject child)
        {
            if (child == null)
            {
                throw new System.NullReferenceException("Child GameObject must not be null");
            }

            var t = child.transform;
            t.parent = parent == null ? null : parent.transform;

            ResetTransform(t);

            if (parent != null) { child.layer = parent.layer; }
        }

        private static void ResetTransform(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }

};//End SG

