using UnityEngine;
using System.Collections;
using XLua;


namespace SG
{
    /// <summary>
    /// 新手引导使用的控件。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class GuideWidget : MonoBehaviour
    {
        /// <summary>
        /// 控件名称。
        /// </summary>
        public string Name;

        private RectTransform m_RT;

        private PanelBase m_Panel;

        public PanelBase Panel
        {
            get
            {
                if (m_Panel == null && m_RT != null)
                {
                    //获取所属的画布
                    Transform t = m_RT.parent;
                    while (t != null && m_Panel == null)
                    {
                        m_Panel = t.GetComponent<PanelBase>();
                        t = t.parent;
                    }
                }
                return m_Panel;
            }
        }

        private void Awake()
        {
            m_RT = transform as RectTransform;
            if (string.IsNullOrEmpty(Name))
            {
                Name = this.name;
            }
        }

        private void OnEnable()
        {
            Invoke("OnEnableEvent", 0);         //延迟一帧，有可能Panel.PanelName还没赋值
        }

        private void OnEnableEvent()
        {
            if (Panel != null)
            {
                EventParameter ep = EventParameter.Get(string.Format("{0},{1}", Panel.PanelName, Name));
                ep.intParameter = 1;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UI_GUIDE_ENABLE_CHANGE, ep);
            }
        }

        private void OnDisable()
        {
            Invoke("OnDisableEvent", 0);         //延迟一帧，有可能Panel.PanelName还没赋值
        }
        private void OnDisableEvent()
        {
            if (Panel != null)
            {
                EventParameter ep = EventParameter.Get(string.Format("{0},{1}", Panel.PanelName, Name));
                ep.intParameter = 0;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UI_GUIDE_ENABLE_CHANGE, ep);
            }            
        }

        /// <summary>
        /// 点击按钮。
        /// </summary>
        public void OnButtonClick()
        {
            if (Panel != null)
            {
                EventParameter ep = EventParameter.Get(string.Format("{0},{1}", Panel.PanelName, Name));
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_UI_GUIDE_CLICK, ep);
            }
        }

        /// <summary>
        /// 获取字节相对画布的位置。
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            if (m_RT == null || Panel == null)
            {
                return Vector3.zero;
            }

            Vector3 v = Panel.transform.InverseTransformPoint(m_RT.position);
            return v;
        }
    }
}

