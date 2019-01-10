using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class Or : ConditionBase
    {
        public Or()
        {
		}
        ~Or()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Or))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            OrTask pTask = new OrTask();

            return pTask;
        }


        // ============================================================================
[Hotfix]
        class OrTask : Selector.SelectorTask
        {
            public OrTask()
                : base()
            {
            }

            ~OrTask()
            {
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

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                //Debug.Check(this.m_children.Count == 2);
                for(int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorTask pBehavior = this.m_children[i];
                    EBTStatus s = pBehavior.exec(pAgent);

                    // If the child succeeds, succeeds
                    if (s == EBTStatus.BT_SUCCESS)
                    {
                        return s;
                    }

                    Debug.Check(s == EBTStatus.BT_FAILURE);
                }

                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

