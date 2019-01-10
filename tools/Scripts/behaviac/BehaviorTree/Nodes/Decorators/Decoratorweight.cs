using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class DecoratorWeight : DecoratorNode
    {
        public DecoratorWeight()
        {
		}

        ~DecoratorWeight()
        {
            m_weight_var = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "Weight")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_weight_var = Condition.LoadRight(p.value, propertyName, ref typeName);
                }
            }
        }

        protected virtual int GetWeight(behaviac.Agent pAgent)
        {
            if (this.m_weight_var != null)
            {
                Debug.Check(this.m_weight_var != null);
                int count = (int)this.m_weight_var.GetValue(pAgent);

                return count;
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            DecoratorWeightTask pTask = new DecoratorWeightTask();

            return pTask;
        }

        Property m_weight_var;

[Hotfix]
        public class DecoratorWeightTask : DecoratorTask
        {
            public DecoratorWeightTask() : base()
            {
            }

            public int GetWeight(Agent pAgent)
            {
                Debug.Check(this.GetNode() is DecoratorWeight);
                DecoratorWeight pNode = (DecoratorWeight)(this.GetNode());

                return pNode != null ? pNode.GetWeight(pAgent) : 0;
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                return status;
            }
        }
    }
}

