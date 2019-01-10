using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class DecoratorLoop : DecoratorCount
    {
        public DecoratorLoop()
        {
		}
        ~DecoratorLoop()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorLoop))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorLoopTask pTask = new DecoratorLoopTask();

            return pTask;
        }

        ///Returns EBTStatus.BT_FAILURE for the specified number of iterations, then returns EBTStatus.BT_SUCCESS after that
[Hotfix]
        class DecoratorLoopTask : DecoratorCountTask
        {
            public DecoratorLoopTask()
            {
            }

            ~DecoratorLoopTask()
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
                if (this.m_n > 0)
                {
                    this.m_n--;

                    if (this.m_n == 0)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }

                    return EBTStatus.BT_RUNNING;
                }

                if (this.m_n == -1)
                {
                    return EBTStatus.BT_RUNNING;
                }

                Debug.Check(this.m_n == 0);

                return EBTStatus.BT_SUCCESS;
            }
        }
    }
}

