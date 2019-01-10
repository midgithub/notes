using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{

    [LuaCallCSharp]
    public class EventMgr : MonoBehaviour
    {
        [CSharpCallLua]
        public delegate void EventFunction(GameEvent ge, EventParameter parameter);

        [CSharpCallLua]
        public delegate void EventFunctionLua(int ge, EventParameter parameter);

        [LuaCallCSharp]
        public struct EventNode
        {
            public EventNode(EventFunction fun, bool remove)
            {
                function = fun;
                functionLua = null;
                removeOnLoadScene = remove;
            }

            public EventNode(EventFunctionLua fun, bool remove)
            {
                function = null;
                functionLua = fun;
                removeOnLoadScene = remove;
            }

            public void Trigger(int ge, EventParameter parameter)
            {
                if (function != null)
                {
                    function((GameEvent)ge, parameter);
                }
                else if (functionLua != null)
                {
                    functionLua(ge, parameter);
                }
            }

            /// <summary>
            /// 判断两个事件节点是否相等。
            /// </summary>
            /// <param name="node">要比较的事件节点。</param>
            /// <returns>在不为null的情况下，function或functionLua其中一个相等则为true。</returns>
            public bool IsEqual(EventNode node)
            {
                if (function != null)
                {
                    return function == node.function;
                }
                if (functionLua != null)
                {
                    return functionLua == node.functionLua;
                }
                return node.function == null && node.functionLua == null;
            }

            public EventFunction function;
            public EventFunctionLua functionLua;
            public bool removeOnLoadScene;
        }


        private Dictionary<int, List<EventNode>> eventMap = new Dictionary<int, List<EventNode>>();


        // removeOnLoadScene决定此监听注册是否在加载场景时自动移除，大部分监听都应该选择自动移除，这是防止内存泄漏的保障机制
        public void AddListener(GameEvent ge, EventFunction ef, bool removeOnLoadScene = true)
        {
            AddListener((int)ge, new EventNode(ef, removeOnLoadScene));
        }

        public void AddListener(int ge, EventFunctionLua ef, bool removeOnLoadScene = true)
        {
            AddListener(ge, new EventNode(ef, removeOnLoadScene));
        }

        private void AddListener(int ge, EventNode node)
        {
            // 如果第一次注册此类事件，需要创建对应链表
            List<EventNode> list;
            if (!eventMap.TryGetValue(ge, out list))
            {
                list = new List<EventNode>();
                eventMap.Add(ge, list);
            }

            // 如果重复注册则返回
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsEqual(node))
                {
                    return;
                }
            }
            list.Add(node);
        }

        public void RemoveListener(GameEvent ge, EventFunction ef)
        {
            RemoveListener((int)ge, new EventNode(ef, false));
        }

        public void RemoveListener(int ge, EventFunctionLua ef)
        {
            RemoveListener(ge, new EventNode(ef, false));
        }

        public void RemoveListener(int ge, EventNode node)
        {
            List<EventNode> list;
            if (!eventMap.TryGetValue(ge, out list))
            {
                return;
            }

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsEqual(node))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }


        public struct EventData
        {
            public EventData(int ge, EventParameter ep)
            {
                gameevent = ge;
                parameter = ep;
            }

            public int gameevent;
            public EventParameter parameter;
        }

        Queue<EventData> eventQueue = new Queue<EventData>();

        public void TriggerEventThread(int ge, EventParameter parameter)
        {
            EventData data = new EventData(ge, parameter);
            lock (eventQueue)
            {
                eventQueue.Enqueue(data);
            }
        }

        List<EventData> dataList = new List<EventData>();

        void Update()
        {
            lock (eventQueue)
            {
                while (eventQueue.Count > 0)
                {
                    dataList.Add(eventQueue.Dequeue());
                }
            }

            for (int i = 0; i < dataList.Count; ++i)
            {
                EventData ed = dataList[i];
                TriggerEvent(ed.gameevent, ed.parameter);
            }
            dataList.Clear();
        }


        public void TriggerEvent(GameEvent ge, EventParameter parameter)
        {
            TriggerEvent((int)ge, parameter);
        }

[Hotfix]
        private class Triggerlist : CacheObject
        {
            public List<EventNode> Nodes = new List<EventNode>();

            public override void OnRecycle()
            {
                Nodes.Clear();
            }
        }

        private CachePool<Triggerlist> m_CacheTriggerlist = new CachePool<Triggerlist>(20);

        public void TriggerEvent(int ge, EventParameter parameter)
        {
            //将事件响应队列复制一个副本，因为事件在响应的过场中有可能会改变响应列表
            List<EventNode> list;
            if (!eventMap.TryGetValue(ge, out list))
            {
                if (parameter != null && parameter.autoRecycle)
                {
                    EventParameter.Cache(parameter);
                }
                return;
            }

            //有可能在触发的过程中重复调用TriggerEvent，所以列表副本不能共享一下，需要动态创建
            Triggerlist triggerlist = m_CacheTriggerlist.Get();
            triggerlist.Nodes.AddRange(list);

            //逐个触发
            for (int i = 0; i < triggerlist.Nodes.Count; i++)
            {
                EventNode node = triggerlist.Nodes[i];
                try
                {
                    node.Trigger(ge, parameter);
                }
                catch (MissingReferenceException e)
                {
                    //如果向一个已经被销毁的对象发送消息，则清除之
                    list.Remove(node);
#if UNITY_EDITOR
                    LogMgr.LogError(e.ToString());
#endif
                }
            }
            m_CacheTriggerlist.Cache(triggerlist);
            if (parameter != null && parameter.autoRecycle)
            {
                EventParameter.Cache(parameter);
            }            

            // 切换场景时移除相关的监听对象
            if (ge == (int)GameEvent.GE_BEFORE_LOADSCENE)
            {
                OnLoadSceneRecycle();
            }
        }

        /// <summary>
        /// 切换场景时移除removeOnLoadScene标志为true的监听对象
        /// </summary>
        public void OnLoadSceneRecycle()
        {
            var enumerator = eventMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var list = enumerator.Current.Value;
                for (int i = 0; i < list.Count;)
                {
                    if (list[i].removeOnLoadScene)
                    {
                        list.RemoveAt(i);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
            enumerator.Dispose();
        }
    }
}

