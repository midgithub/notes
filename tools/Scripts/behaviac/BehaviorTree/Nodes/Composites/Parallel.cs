using XLua;
using System;
using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    /// Enumerates the options for when a parallel node is considered to have failed.
    /**
    - FAIL_ON_ONE indicates that the node will return failure as soon as one of its children fails.
    - FAIL_ON_ALL indicates that all of the node's children must fail before it returns failure.

    If FAIL_ON_ONE and SUCEED_ON_ONE are both active and are both trigerred in the same time step, failure will take precedence.
    */
    public enum FAILURE_POLICY
    {
        FAIL_ON_ONE,
        FAIL_ON_ALL
    }

    /// Enumerates the options for when a parallel node is considered to have succeeded.
    /**
    - SUCCEED_ON_ONE indicates that the node will return success as soon as one of its children succeeds.
    - SUCCEED_ON_ALL indicates that all of the node's children must succeed before it returns success.
    */
    public enum SUCCESS_POLICY
    {
        SUCCEED_ON_ONE,
        SUCCEED_ON_ALL  
    }

    /// Enumerates the options when a parallel node is exited
    /**
    - EXIT_NONE indicates that the parallel node just exit.
    - EXIT_ABORT_RUNNINGSIBLINGS indicates that the parallel node abort all other running siblings.
    */
    public enum EXIT_POLICY
    {
        EXIT_NONE,
        EXIT_ABORT_RUNNINGSIBLINGS
    }

    /// Enumerates the options of what to do when a child finishes
    /**
    - CHILDFINISH_ONCE indicates that the child node just executes once.
    - CHILDFINISH_LOOP indicates that the child node run again and again.
    */
    public enum CHILDFINISH_POLICY
    {
        CHILDFINISH_ONCE,
        CHILDFINISH_LOOP    
    }

[Hotfix]
    public class Parallel : BehaviorNode
    {
        public Parallel()
        {
            m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
            m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
            m_exitPolicy = EXIT_POLICY.EXIT_NONE;
            m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
        }

        ~Parallel ()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "FailurePolicy")
                {
                    if (p.value == "FAIL_ON_ONE")
                    {
                        this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
                    }
                    else if (p.value == "FAIL_ON_ALL")
                    {
                        this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ALL;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else if (p.name == "SuccessPolicy")
                {
                    if (p.value == "SUCCEED_ON_ONE")
                    {
                        this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ONE;
                    }
                    else if (p.value == "SUCCEED_ON_ALL")
                    {
                        this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else if (p.name == "ExitPolicy")
                {
                    if (p.value == "EXIT_NONE")
                    {
                        this.m_exitPolicy = EXIT_POLICY.EXIT_NONE;
                    }
                    else if (p.value == "EXIT_ABORT_RUNNINGSIBLINGS")
                    {
                        this.m_exitPolicy = EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else if (p.name == "ChildFinishPolicy")
                {
                    if (p.value == "CHILDFINISH_ONCE")
                    {
                        this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_ONCE;
                    }
                    else if (p.value == "CHILDFINISH_LOOP")
                    {
                        this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else
                {
					// todo: enter exit action failed here by mistake
                    //Debug.Check(false);
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Parallel))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        void SetPolicy(FAILURE_POLICY failPolicy /*= FAILURE_POLICY.FAIL_ON_ALL*/,
            SUCCESS_POLICY successPolicy /*= SUCCESS_POLICY.SUCCEED_ON_ALL*/,
            EXIT_POLICY exitPolicty /*= EXIT_POLICY.EXIT_NONE*/)
        {
            m_failPolicy = failPolicy;
            m_succeedPolicy = successPolicy;
            m_exitPolicy = exitPolicty;
        }

        protected override BehaviorTask createTask()
        {
            ParallelTask pTask = new ParallelTask();

            return pTask;
        }

        protected FAILURE_POLICY m_failPolicy;
        protected SUCCESS_POLICY m_succeedPolicy;
        protected EXIT_POLICY m_exitPolicy;
        protected CHILDFINISH_POLICY m_childFinishPolicy;


        ///Execute behaviors in parallel
        /** There are two policies that control the flow of execution. The first is the policy for failure, 
        and the second is the policy for success.

        For failure, the options are "fail when one child fails" and "fail when all children fail".
        For success, the options are similarly "complete when one child completes", and "complete when all children complete".
        */
[Hotfix]
        class ParallelTask : CompositeTask
        {
            public ParallelTask()
            {
            }

            ~ParallelTask ()
            {
                this.m_children.Clear();
            }

            public override void Init(BehaviorNode node)
            {
                base.Init(node);
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
                Debug.Check(this.m_activeChildIndex == CompositeTask.InvalidChildIndex);

                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.GetNode() is Parallel);
                Parallel pParallelNode = (Parallel)(this.GetNode());

                bool sawSuccess = false;
                bool sawFail = false;
                bool sawRunning = false;
                bool sawAllFails = true;
                bool sawAllSuccess = true;

                bool bLoop = (pParallelNode.m_childFinishPolicy == CHILDFINISH_POLICY.CHILDFINISH_LOOP);

                // go through all m_children
                for (int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorTask pChild = this.m_children[i];

                    EBTStatus treeStatus = pChild.GetStatus();

                    if (bLoop || (treeStatus == EBTStatus.BT_RUNNING || treeStatus == EBTStatus.BT_INVALID))
                    {
                        EBTStatus status = pChild.exec(pAgent);

                        if (status == EBTStatus.BT_FAILURE)
                        {
                            sawFail = true;
                            sawAllSuccess = false;
                        }
                        else if (status == EBTStatus.BT_SUCCESS)
                        {
                            sawSuccess = true;
                            sawAllFails = false;
                        }
                        else if (status == EBTStatus.BT_RUNNING)
                        {
                            sawRunning = true;
                            sawAllFails = false;
                            sawAllSuccess = false;
                        }
                    }
                    else if (treeStatus == EBTStatus.BT_SUCCESS)
                    {
                        sawSuccess = true;
                        sawAllFails = false;
                    }
                    else
                    {
                        Debug.Check(treeStatus == EBTStatus.BT_FAILURE);

                        sawFail = true;
                        sawAllSuccess = false;
                    }
                }

                EBTStatus result = sawRunning ? EBTStatus.BT_RUNNING : EBTStatus.BT_FAILURE;
                if ((pParallelNode.m_failPolicy == FAILURE_POLICY.FAIL_ON_ALL && sawAllFails) ||
                    (pParallelNode.m_failPolicy == FAILURE_POLICY.FAIL_ON_ONE && sawFail))
                {
                    result = EBTStatus.BT_FAILURE;
                }
                else if ((pParallelNode.m_succeedPolicy == SUCCESS_POLICY.SUCCEED_ON_ALL && sawAllSuccess) ||
                    (pParallelNode.m_succeedPolicy == SUCCESS_POLICY.SUCCEED_ON_ONE && sawSuccess))
                {
                    result = EBTStatus.BT_SUCCESS;
                }
                //else if (m_failPolicy == FAIL_ON_ALL && m_succeedPolicy == SUCCEED_ON_ALL && sawRunning)
                //{
                //  return EBTStatus.BT_RUNNING;
                //}

                if (pParallelNode.m_exitPolicy == EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS && (result == EBTStatus.BT_FAILURE || result == EBTStatus.BT_SUCCESS))
                {
                    for (int i = 0; i < this.m_children.Count; ++i)
                    {
                        BehaviorTask pChild = this.m_children[i];
                        //Debug.Check(BehaviorTreeTask.DynamicCast(pChild));
                        EBTStatus treeStatus = pChild.GetStatus();

                        if (treeStatus == EBTStatus.BT_RUNNING)
                        {
                            pChild.abort(pAgent);
                        }
                    }
                }

                return result;
            }

            protected override bool isContinueTicking()
            {
                return true;
            }
        }
    }
}
