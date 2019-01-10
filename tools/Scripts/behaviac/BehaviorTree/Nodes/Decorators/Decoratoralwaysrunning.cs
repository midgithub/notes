using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class DecoratorAlwaysRunning : DecoratorNode
    {
        public DecoratorAlwaysRunning()
        {
		}
        ~DecoratorAlwaysRunning()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorAlwaysRunning))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorAlwaysRunningTask pTask = new DecoratorAlwaysRunningTask();

            return pTask;
        }

[Hotfix]
        class DecoratorAlwaysRunningTask : DecoratorTask
        {
            public DecoratorAlwaysRunningTask() : base()
            {
            }

            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);
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
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}

