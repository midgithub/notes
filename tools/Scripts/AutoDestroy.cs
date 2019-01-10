/**
* @file     : AutoDestroy.cs
* @brief    : 
* @details  : 文件用途说明
* @author   : 
* @date     : 
*/

using XLua;
using UnityEngine;
using System.Collections;

/// <summary>
/// 自动销毁组建。
/// </summary>
[Hotfix]
public class AutoDestroy : MonoBehaviour
{
    /// <summary>
    /// 销毁延时。
    /// </summary>
    public float Delay = 2;
    
    /// <summary>
    /// 销毁计数。
    /// </summary>
    private float _count;

    private void Awake()
    {
        Reset();
    }

    // Update is called once per frame
    void Update ()
	{
        if (_count >= Delay)
        {
            return;
        }

        _count += Time.deltaTime;
        if (_count >= Delay)
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        _count = 0;
    }
}

