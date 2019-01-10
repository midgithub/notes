using XLua;
using System;
using System.Collections;
using System.Collections.Generic;


namespace behaviac
{
[Hotfix]
    public class DecoratorNot : DecoratorNode
    {
        public DecoratorNot()
        {
		}
        ~DecoratorNot()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorNot))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorNotTask pTask = new DecoratorNotTask();


            return pTask;
        }


[Hotfix]
        class DecoratorNotTask : DecoratorTask
        {
            public DecoratorNotTask() : base()
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
                if (status == EBTStatus.BT_FAILURE)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                if (status == EBTStatus.BT_SUCCESS)
                {
                    return EBTStatus.BT_FAILURE;
                }

                return status;
            }
        }
    }
}

