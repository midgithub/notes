using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
[Hotfix]
    public class True : ConditionBase
    {
	    public True()
	    {
		}

        ~True()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
			if (!(pTask.GetNode() is True))
			{
				return false;
			}
		
			return base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
        {
			TrueTask pTask = new TrueTask();

			return pTask;
		}


	    // ============================================================================
[Hotfix]
	    class TrueTask : ConditionBaseTask
	    {
	    	public  TrueTask() : base()
	        {
	        }

	        ~TrueTask()
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
        		return EBTStatus.BT_SUCCESS;
    		}
   		}
    }
}

