using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class DecoratorFrames : DecoratorNode
    {
        public DecoratorFrames()
        {
		}
        ~DecoratorFrames()
        {
            this.m_frames_var = null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            foreach (property_t p in properties)
            {
                if (p.name == "Time")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_frames_var = Condition.LoadRight(p.value, propertyName, ref typeName);
                }
            }
        }

        protected virtual int GetFrames(Agent pAgent)
        {
            if (this.m_frames_var != null)
            {
                Debug.Check(this.m_frames_var != null);
                int frames = (int)this.m_frames_var.GetValue(pAgent);

                return frames;
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            DecoratorFramesTask pTask = new DecoratorFramesTask();

            return pTask;
        }

        Property m_frames_var;

[Hotfix]
        class DecoratorFramesTask : DecoratorTask
        {
            public DecoratorFramesTask()
            {
            }

            ~DecoratorFramesTask()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is DecoratorFramesTask);
                DecoratorFramesTask ttask = (DecoratorFramesTask)target;

                ttask.m_start = this.m_start;
                ttask.m_frames = this.m_frames;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID startId = new CSerializationID("start");
                node.setAttr(startId, this.m_start);

                CSerializationID framesId = new CSerializationID("frames");
                node.setAttr(framesId, this.m_frames);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);

                this.m_start = 0;
                this.m_frames = this.GetFrames(pAgent);

                return (this.m_frames > 0);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                this.m_start += (int)(Workspace.GetDeltaFrames());
                if (this.m_start >= this.m_frames)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                return EBTStatus.BT_RUNNING;
            }

            int GetFrames(Agent pAgent)
            {
                Debug.Check(this.GetNode() is DecoratorFrames);
                DecoratorFrames pNode = (DecoratorFrames)(this.GetNode());

                return pNode != null ? pNode.GetFrames(pAgent) : 0;
            }

            int m_start;
            int m_frames = 0;
        }
    }
}

