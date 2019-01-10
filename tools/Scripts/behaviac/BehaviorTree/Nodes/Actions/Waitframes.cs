using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class WaitFrames : BehaviorNode
    {
        public WaitFrames()
        {
		}

        ~WaitFrames()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t p in properties)
            {
                if (p.name == "Frames")
                {
                    string propertyName = null;

                    int pParenthesis = p.value.IndexOf('(');
                    if (pParenthesis == -1)
                    {
                        string typeName = null;
                        this.m_frames_var = Condition.LoadRight(p.value, propertyName, ref typeName);
                    }
                    else
                    {
                        //method
                        this.m_frames_method = Action.LoadMethod(p.value);
                    }
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
            else if (this.m_frames_method != null)
            {
                ParentType pt = this.m_frames_method.GetParentType();
                Agent pParent = pAgent;
                if (pt == ParentType.PT_INSTANCE)
                {
                    pParent = Agent.GetInstance(this.m_frames_method.GetInstanceNameString(), pParent.GetContextId());
					Debug.Check(pParent != null || Utils.IsStaticClass(this.m_frames_method.GetInstanceNameString()));
                }

                int frames = (int)this.m_frames_method.run(pParent, pAgent);

                return frames;
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            WaitFramesTask pTask = new WaitFramesTask();

            return pTask;
        }

        Property m_frames_var;
        CMethodBase m_frames_method;

[Hotfix]
        class WaitFramesTask : LeafTask
        {
            public WaitFramesTask()
            {
            }

            ~WaitFramesTask()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is WaitFramesTask);
                WaitFramesTask ttask = (WaitFramesTask)target;
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
                this.m_start = 0;
                this.m_frames = this.GetFrames(pAgent);

                if (this.m_frames <= 0)
                {
                    return false;
                }

                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
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
                Debug.Check(this.GetNode() is WaitFrames);
                WaitFrames pWaitNode = (WaitFrames)(this.GetNode());

                return pWaitNode != null ? pWaitNode.GetFrames(pAgent) : 0;
            }

            int m_start;
            int m_frames;
        }
    }
}

