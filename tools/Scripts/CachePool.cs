using XLua;
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    /// <summary>
    /// 缓存接口，实现此接口的类可通过CachePool进行缓存管理。
    /// </summary>
    public interface ICacheable
    {
        /// <summary>
        /// 被创建时调用。
        /// </summary>
        void OnCreate();

        /// <summary>
        /// 被使用时调用。
        /// </summary>
        void OnUse(object data = null);

        /// <summary>
        /// 被回收时调用。
        /// </summary>
        void OnRecycle();

        /// <summary>
        /// 若缓存池被销毁或已到上限时调用。
        /// </summary>
        void OnRelease();
    }

    /// <summary>
    /// 缓存对象类，提供默认操作。
    /// </summary>
[Hotfix]
    public class CacheObject : ICacheable
    {
        public virtual void OnCreate() { }
        public virtual void OnUse(object data = null) { }
        public virtual void OnRecycle() { }
        public virtual void OnRelease() { }        
    }

    /// <summary>
    /// CachePool。
    /// </summary>
[Hotfix]
    public class CachePool<T> where T : ICacheable, new()
    {
        /// <summary>
        /// 创建缓存对象的函数委托。
        /// </summary>
        /// <returns>缓存对象类型。</returns>
        public delegate T NewDelegate();

        #region 对外操作----------------------------------------------------------------

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="size">缓存大小。</param>
        public CachePool(int size = 10, NewDelegate newfun = null)
        {
            m_Size = size;
            m_NewFunction = newfun;
        }

        /// <summary>
        /// 释放缓存。
        /// </summary>
        public void Clear()
        {
            while (m_CacheObjects.Count > 0)
            {
                T top = m_CacheObjects.Pop();
                top.OnRelease();
            }
        }

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <param name="data">传递给OnUse的参数数据。</param>
        /// <returns>缓存对象。</returns>
        public T Get(object data = null)
        {
            if (m_CacheObjects.Count > 0)
            {
                T top = m_CacheObjects.Pop();
                top.OnUse(data);
                return top;
            }

            T newobj = m_NewFunction == null ? new T() : m_NewFunction();
            if (newobj != null)
            {
                newobj.OnCreate();
                newobj.OnUse(data);
            }
            return newobj;
        }

        public void Cache(T obj)
        {
            if (obj == null)
            {
                return;
            }
            if (m_CacheObjects.Contains(obj))
            {
#if UNITY_EDITOR
                LogMgr.LogError("重复回收");
#endif
                return;
            }

            obj.OnRecycle();
            if (m_CacheObjects.Count < m_Size)
            {
                m_CacheObjects.Push(obj);
            }
            else
            {
                obj.OnRelease();

#if UNITY_EDITOR
                //LogMgr.LogWarning("The cache pool is full and the obj has be release. Size:{0}", m_Size);
#endif
            }
        }

        #endregion

        #region 对外属性----------------------------------------------------------------

        /// <summary>
        /// 当前缓存的数量。
        /// </summary>
        public int Count
        {
            get { return m_CacheObjects.Count; }
        }

        /// <summary>
        /// 获取或设置缓存大小。
        /// </summary>
        public int Size
        {
            get { return m_Size; }
            set
            {
                //移除超过缓存大小的对象。
                m_Size = Mathf.Max(0, value);
                while (m_CacheObjects.Count > m_Size)
                {
                    T top = m_CacheObjects.Pop();
                    top.OnRelease();

#if UNITY_EDITOR
                    LogMgr.LogWarning("The new size is too small. Need release a object. Size:{0} Count:{1}", m_Size, m_CacheObjects.Count + 1);
#endif
                }
            }
        }

        #endregion

        #region 内部操作----------------------------------------------------------------

        #endregion

        #region 内部数据----------------------------------------------------------------

        /// <summary>
        /// 创建对象的函数，若该函数为null则直接new。
        /// </summary>
        private NewDelegate m_NewFunction = null;

        /// <summary>
        /// 缓存堆栈。
        /// </summary>
        private Stack<T> m_CacheObjects = new Stack<T>();

        /// <summary>
        /// 缓存上限，超过上限的将不会再缓存，并调用缓存对象的OnRelease。
        /// </summary>
        private int m_Size;

        #endregion
    }
}

