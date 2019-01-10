using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class DecoratorAlwaysFailure : DecoratorNode
    {
        public DecoratorAlwaysFailure()
        {
		}
        ~DecoratorAlwaysFailure()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorAlwaysFailure))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorAlwaysFailureTask pTask = new DecoratorAlwaysFailureTask();

            return pTask;
        }

[Hotfix]
        class DecoratorAlwaysFailureTask : DecoratorTask
        {
            public DecoratorAlwaysFailureTask() : base()
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

            protected override EBTStatus decorate(EBTStatus status)
            {
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

