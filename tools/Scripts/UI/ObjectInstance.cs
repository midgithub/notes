/**
* @file     : ObjectInstantiate.cs
* @brief    : 用于对象实例化
* @details  : 
* @author   : XuXiang
* @date     : 2017-11-21 11:26
*/

using UnityEngine;
using System.Collections;
using SG;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class ObjectInstance : MonoBehaviour
{
    /// <summary>
    /// 要实例化的prefab路径。
    /// </summary>
    [SerializeField]
    private string m_Path;

    /// <summary>
    /// 要实例化的对象。
    /// </summary>
    [SerializeField]
    private GameObject m_Object;

    /// <summary>
    /// 初始位置。
    /// </summary>
    [SerializeField]
    private Vector3 m_Position = Vector3.zero;

    /// <summary>
    /// 初始缩放。
    /// </summary>
    [SerializeField]
    private Vector3 m_Scale = Vector3.one;

    /// <summary>
    /// 是否清除子节点。
    /// </summary>
    [SerializeField]
    private bool m_ClearChild;

    /// <summary>
    /// 实例出来的对象。
    /// </summary>
    private GameObject m_Instance;

    /// <summary>
    /// 获取实例对象。
    /// </summary>
    public GameObject Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private void Awake()
    {
        if (m_ClearChild)
        {
            UiUtil.ClearAllChildImmediate(transform);
        }
        
        if (!string.IsNullOrEmpty(m_Path))
        {
            GameObject prefab = (GameObject)CoreEntry.gResLoader.Load(m_Path, typeof(GameObject));
            if (prefab != null)
            {
                m_Instance = Instantiate(prefab) as GameObject;
            }
        }
        else if (m_Object != null)
        {
            m_Instance = Instantiate(m_Object) as GameObject;
        }

        //初始化
        if (m_Instance != null)
        {
            Transform t = m_Instance.transform;
            t.SetParent(transform);
            if (t is RectTransform)
            {
                RectTransform rt = t as RectTransform;
                rt.anchoredPosition3D = m_Position;
            }
            else
            {
                t.localPosition = m_Position;
            }
            t.localScale = m_Scale;
            t.rotation = Quaternion.Euler(Vector3.zero);
            m_Instance.SetActive(true);
        }
    }
}

