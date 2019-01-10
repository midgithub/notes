using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    // ============================================================================
[Hotfix]
    public class Noop : BehaviorNode
    {
        public Noop()
        {
		}
        ~Noop()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Noop))
            {
                return false;
            }

            //used in unittest
            //return base.IsValid(pAgent, pTask);
            return true;
        }

        protected override BehaviorTask createTask()
        {
            NoopTask pTask = new NoopTask();


            return pTask;
        }

        /**
        nothing to do, just return success
        */
[Hotfix]
        class NoopTask : LeafTask
        {
            public NoopTask() : base()
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

            protected override bool onenter(Agent pAgent)
            {
                return true;
            }
            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                return EBTStatus.BT_SUCCESS;
            }
        }
    }
}

