using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class DecoratorLog : DecoratorNode
    {
        public DecoratorLog()
        {
		}
        ~DecoratorLog()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "Log")
                {
                    this.m_message = p.value;
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorLog))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorLogTask pTask = new DecoratorLogTask();

            return pTask;
        }

        protected string m_message;

[Hotfix]
        class DecoratorLogTask : DecoratorTask
        {
            public DecoratorLogTask()
                : base()
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
                Debug.Check(this.GetNode() is DecoratorLog);
                DecoratorLog pDecoratorLogNode = (DecoratorLog)(this.GetNode());
                behaviac.Debug.LogWarning(string.Format("DecoratorLogTask:{0}\n", pDecoratorLogNode.m_message));

                return status;
            }
        }
    }
}

