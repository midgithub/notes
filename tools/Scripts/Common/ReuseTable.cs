using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace SG
{
    /// <summary>
    /// 更新单元项委托。
    /// </summary>
    /// <param name="index">单元格索引。</param>
    /// <param name="item">单元格对象。</param>
    public delegate void UpdateItemDelegate(int index, RectTransform item);

    /// <summary>
    /// 滑动列表单元格复用表格。PS:单元格和容器的锚点都要设置成(0,1)，对齐点为父节点的左上角。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class ReuseTable : MonoBehaviour
    {
        #region 对外操作----------------------------------------------------------------

        /// <summary>
        /// 滑动方向。
        /// </summary>
        public enum ScrollDirection
        {
            /// <summary>
            /// 水平。
            /// </summary>
            Horizontal,

            /// <summary>
            /// 竖直。
            /// </summary>
            Vertical
        }

        /// <summary>
        /// 添加更新监听者。
        /// </summary>
        /// <param name="listener">监听者。</param>
        public void AddItemUpdateListener(UpdateItemDelegate listener)
        {
            OnItemUpdate += listener;
        }

        /// <summary>
        /// 移除更新监听者。
        /// </summary>
        /// <param name="listener">监听者。</param>
        public void RemoveItemUpdateListener(UpdateItemDelegate listener)
        {
            OnItemUpdate -= listener;
        }

        /// <summary>
        /// 设置滚动位置。
        /// </summary>
        /// <param name="normalized">滚动比例。</param>
        public void SetNormalized(float normalized)
        {
            if (m_Direction == ScrollDirection.Horizontal)
            {
                ScrollRectUtil.SetHorizontalNormalized(m_ScrollView, normalized);
            }
            else if (m_Direction == ScrollDirection.Vertical)
            {
                ScrollRectUtil.SetVerticalNormalized(m_ScrollView, normalized);
            }
        }

        /// <summary>
        /// 刷新所有格子。
        /// </summary>
        public void RefreshAllItem()
        {
            if (OnItemUpdate != null)
            {
                int n = Mathf.Min(m_Count, m_CellItems.Count);
                for (int i = 0; i < n; ++i)
                {
                    OnItemUpdate(m_FirstIndex + i, m_CellItems[i]);
                }
            }            
        }

        /// <summary>
        /// 更新某个格子。
        /// </summary>
        /// <param name="index">格子索引。</param>
        public void UpdateItem(int index)
        {
            int i = index - m_FirstIndex;
            if (i < 0 || i >= m_CellItems.Count)
            {
                return;
            }
            UpdateItem(index, m_CellItems[i]);
        }

        /// <summary>
        /// 获取某个格子。
        /// </summary>
        /// <param name="index">格子索引。</param>
        /// <returns>格子对象，没显示的格子返回null。</returns>
        public RectTransform GetItem(int index)
        {
            int i = index - m_FirstIndex;
            if (i < 0 || i >= m_CellItems.Count)
            {
                return null;
            }
            return m_CellItems[i];
        }

        #endregion

        #region 对外属性----------------------------------------------------------------

        /// <summary>
        /// 单元格更新事件。
        /// </summary>
        public event UpdateItemDelegate OnItemUpdate;

        /// <summary>
        /// 获取或设置表格单元格数量。
        /// </summary>
        public int Count
        {
            get { return m_Count; }
            set
            {
                //重置起始索引和单元格位置
                m_Count = value;
                m_FirstIndex = 0;
                for (int i = 0; i < m_CellItems.Count; ++i)
                {
                    RectTransform rt = m_CellItems[i];
                    rt.gameObject.SetActive(i < m_Count);
                    rt.anchoredPosition3D = GetPosition(i);
                }

                //设置容器尺寸
                int line = (int)Math.Ceiling(m_Count * 1.0f / m_BasicNumber);
                Vector2 vs = CacheRectTransform.sizeDelta;
                if (m_Direction == ScrollDirection.Horizontal)
                {
                    vs.x = Mathf.Max(line * m_CellSize.x + m_Space * 2, m_ScrollView.viewport.sizeDelta.x);
                }
                else if (m_Direction == ScrollDirection.Vertical)
                {
                    vs.y = Mathf.Max(line * m_CellSize.y + m_Space * 2, m_ScrollView.viewport.sizeDelta.y);
                }
                CacheRectTransform.sizeDelta = vs;

                //滑动位置归位并刷新所有单元格
                m_CurNormalized = m_Direction == ScrollDirection.Horizontal ? 0 : 1;
                ScrollRectUtil.SetVerticalNormalized(m_ScrollView, m_CurNormalized);
                RefreshAllItem();
            }
        }

        public RectTransform CacheRectTransform
        {
            get
            {
                if (mCacheRectTransform == null)
                {
                    mCacheRectTransform = GetComponent<RectTransform>();
                }
                return mCacheRectTransform;
            }
        }

        #endregion

        #region 内部操作----------------------------------------------------------------

        /// <summary>
        /// 唤醒。
        /// </summary>
        protected void Awake()
        {
            UiUtil.ClearAllChildImmediate(CacheRectTransform);
            m_CellItems.Clear();
            m_ScrollView.horizontal = m_Direction == ScrollDirection.Horizontal;
            m_ScrollView.vertical = m_Direction == ScrollDirection.Vertical;
            m_ScrollView.onValueChanged.AddListener(OnScrollViewChanged);

            Vector2 vsize = m_ScrollView.viewport.sizeDelta;
            int line = (int)(m_Direction == ScrollDirection.Horizontal ? vsize.x / m_CellSize.x : vsize.y / m_CellSize.y);
            InitGridCache(m_BasicNumber * (line + 2));
        }

        /// <summary>
        /// 滑动区域改变。
        /// </summary>
        /// <param name="v"></param>
        private void OnScrollViewChanged(Vector2 v)
        {
            if (m_Count <= 0)
            {
                return;
            }

            float normalized = Mathf.Clamp01(m_Direction == ScrollDirection.Horizontal ? v.x : v.y);
            if (Mathf.Abs(m_CurNormalized - normalized) > 0.00001f)
            {
                OnScroll(normalized);
                m_CurNormalized = normalized;
            }
        }

        /// <summary>
        /// 初始化格子缓存。
        /// </summary>
        /// <param name="n">缓存数量。</param>
        private void InitGridCache(int n)
        {
            for (int i=0; i<n; ++i)
            {
                GameObject obj = Instantiate(m_ItemPrefab) as GameObject;
                RectTransform rt = obj.GetComponent<RectTransform>();
                obj.SetActive(false);
                rt.SetParent(CacheRectTransform);
                rt.localScale = Vector3.one;
                rt.anchoredPosition3D = Vector3.zero;
                m_CellItems.Add(rt);
            }
        }

        /// <summary>
        /// 发生滚动。
        /// </summary>
        /// <param name="normalized">滚动比例。</param>
        private void OnScroll(float normalized)
        {
            int starindex = GetCurStartIndex();
            if (m_FirstIndex < starindex)
            {
                OnScrollForward(starindex);
            }
            else if (m_FirstIndex > starindex)
            {
                OnScrollBackward(starindex);
            }
        }

        /// <summary>
        /// 获取当前起始索引。
        /// </summary>
        /// <returns>当前起始索引。</returns>
        private int GetCurStartIndex()
        {
            if (m_Count <= 0)
            {
                return 0;
            }

            int line = 0;
            if (m_Direction == ScrollDirection.Horizontal)
            {
                float x = -CacheRectTransform.anchoredPosition.x - m_Space;
                line = (int)(x / m_CellSize.x);
            }
            else if (m_Direction == ScrollDirection.Vertical)
            {
                float y = CacheRectTransform.anchoredPosition.y - m_Space;
                line = (int)(y / m_CellSize.y);
            }
            int index = Math.Max(0, line) * m_BasicNumber;
            return Mathf.Clamp(index, 0, m_Count - 1);
        }

        /// <summary>
        /// 向序号大的方向滚动。
        /// </summary>
        /// <param name="startindex">新的起始索引。</param>
        private void OnScrollForward(int startindex)
        {
            int n = Math.Min(m_CellItems.Count, startindex - m_FirstIndex);
            List<RectTransform> items = m_CellItems;
            m_CellItems = new List<RectTransform>();
            for (int i = n; i < items.Count; ++i)
            {
                m_CellItems.Add(items[i]);
            }

            int updateindex = startindex + (items.Count - n);
            for (int i = 0; i < n; ++i)
            {
                RectTransform rt = items[i];
                m_CellItems.Add(rt);

                //更新位置和刷新信息
                int ii = updateindex + i;
                rt.anchoredPosition3D = GetPosition(ii);
                rt.gameObject.SetActive(ii < m_Count);
                if (ii < m_Count)
                {
                    UpdateItem(ii, rt);
                }
            }
            m_FirstIndex = startindex;
        }

        /// <summary>
        /// 向序号小的方向滚动。
        /// </summary>
        /// <param name="startindex">新的起始索引。</param>
        private void OnScrollBackward(int startindex)
        {
            int n = Math.Min(m_CellItems.Count, m_FirstIndex - startindex);             //要移动的数量
            int ln = m_CellItems.Count - n;                                         //剩余不需要移动的数量
            List<RectTransform> items = m_CellItems;
            m_CellItems = new List<RectTransform>();

            for (int i = 0; i < n; ++i)
            {
                RectTransform rt = items[ln + i];
                m_CellItems.Add(rt);

                //更新位置和刷新信息
                int ii = startindex + i;
                rt.anchoredPosition3D = GetPosition(ii);
                rt.gameObject.SetActive(ii >= 0);       //手动用力滑时，有可能一口气滑两行
                if (ii >= 0)
                {
                    UpdateItem(ii, rt);
                }                
            }

            for (int i = 0; i < ln; ++i)
            {
                m_CellItems.Add(items[i]);
            }
            m_FirstIndex = startindex;
        }

        /// <summary>
        /// 更新某个格子。
        /// </summary>
        /// <param name="index">格子索引。</param>
        /// <param name="item">格子对象。</param>
        private void UpdateItem(int index, RectTransform item)
        {
            if (OnItemUpdate != null)
            {
                OnItemUpdate(index, item);
            }
        }

        /// <summary>
        /// 获取水平滚动时，单元格的位置。
        /// </summary>
        /// <param name="index">单元格索引。</param>
        /// <returns>单元格的位置。</returns>
        private Vector3 GetPosition(int index)
        {
            Vector3 v = Vector3.zero;
            if (m_Direction == ScrollDirection.Horizontal)
            {
                v.x = (index / m_BasicNumber) * m_CellSize.x + m_Space;
                v.y = -(index % m_BasicNumber) * m_CellSize.y;
            }
            else if (m_Direction == ScrollDirection.Vertical)
            {
                v.x = (index % m_BasicNumber) * m_CellSize.x;
                v.y = -((index / m_BasicNumber) * m_CellSize.y + m_Space);
            }            
            return v;
        }

        #endregion

        #region 内部数据----------------------------------------------------------------

        /// <summary>
        /// 缓存组件。
        /// </summary>
        private RectTransform mCacheRectTransform;

        /// <summary>
        /// 单元格原型。
        /// </summary>
        [SerializeField]
        private GameObject m_ItemPrefab = null;

        /// <summary>
        /// 滚动视图。
        /// </summary>
        [SerializeField]
        private ScrollRect m_ScrollView = null;

        /// <summary>
        /// 滑动方向。
        /// </summary>
        [SerializeField]
        private ScrollDirection m_Direction = ScrollDirection.Horizontal;

        /// <summary>
        /// 边缘预留多大尺寸。
        /// </summary>
        [SerializeField]
        private float m_Space = 0;

        /// <summary>
        /// 摆放基数。
        /// </summary>
        [SerializeField]
        private int m_BasicNumber = 1;

        /// <summary>
        /// 单元格尺寸。
        /// </summary>
        [SerializeField]
        private Vector2 m_CellSize = new Vector2(50, 50);

        /// <summary>
        /// 当前的滑动比例。
        /// </summary>
        private float m_CurNormalized = 0;

        /// <summary>
        /// 单元格数量。
        /// </summary>
        private int m_Count;

        /// <summary>
        /// 单元格的起始索引。
        /// </summary>
        private int m_FirstIndex;

        /// <summary>
        /// 单元格列表。
        /// </summary>
        private List<RectTransform> m_CellItems = new List<RectTransform>();

        #endregion
    }
}

