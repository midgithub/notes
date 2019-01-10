using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public abstract class DecoratorCount : DecoratorNode
    {
        public DecoratorCount()
        {
		}
        ~DecoratorCount()
        {
            this.m_count_var = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "Count")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_count_var = Condition.LoadRight(p.value, propertyName, ref typeName);
                }
            }
        }

        protected virtual int GetCount(Agent pAgent)
        {
            if (this.m_count_var != null)
            {
                Debug.Check(this.m_count_var != null);
                int count = (int)this.m_count_var.GetValue(pAgent);

                return count;
            }

            return 0;
        }

        Property m_count_var;

[Hotfix]
        protected abstract class DecoratorCountTask : DecoratorTask
        {
            public DecoratorCountTask()
            {
            }

            ~DecoratorCountTask()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is DecoratorCountTask);
                DecoratorCountTask ttask = (DecoratorCountTask)target;

                ttask.m_n = this.m_n;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID countId = new CSerializationID("count");
                node.setAttr(countId, this.m_n);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);

                //don't reset the m_n if it is restarted
                if (this.m_n == 0 || !this.NeedRestart())
                {
                    int count = this.GetCount(pAgent);

                    if (count == 0)
                    {
                        return false;
                    }

                    this.m_n = count;
                }
                else
                {
                    Debug.Check(true);
                }

                return true;
            }

            public int GetCount(Agent pAgent)
            {
                Debug.Check(this.GetNode() is DecoratorCount);
                DecoratorCount pDecoratorCountNode = (DecoratorCount)(this.GetNode());

                return pDecoratorCountNode != null ? pDecoratorCountNode.GetCount(pAgent) : 0;
            }

            protected int m_n;
        }
    }
}

