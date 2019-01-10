using UnityEngine;
using System.Collections;
using SG;
using XLua;
[Hotfix]
public class FollowPlayer : MonoBehaviour
{
    public   float MaxSquareDistance = 8f;
    public   float MinSquareDistance = 7.5f;
    public   float DelayTimeRange = 0.5f;
    public   float UpdatePeriod = 0.1f;
    public   float fTerrainHight = 1f;
    public   bool bCanFollowed = true;
    public   float speed = 6.0f;


    public ActorObj Master;
    public float lastUpdateTime;
    public bool isFollowed = false;

    public string idle = "stand";//待机动作
    public string leisure = "idle"; //休闲动作
    public string run = "run"; //休闲动作

    public static int ActionSwitchTime = 0;//Action switching
    public float interval = 0;
    public bool bleisurePlaying = false;

    public Animation mAnimator;

    public float ViewSquareDistance = 100;


    void Start()
    {
        if (ActionSwitchTime == 0)
        {
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            ActionSwitchTime = G.GetInPath<int>("ConfigData.ConstsConfig.NameToValue.standbyCd.val1");
        }
        lastUpdateTime = Time.time;    
        if(Master != null)
        {
            Vector3 position = Master.transform.position;
            position.y = CommonTools.GetTerrainHeight(transform.position) + fTerrainHight;
            transform.position = position;
        }

        if (Master != null)
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = Master;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MAGICKEY_INFO_CREATE, ep);
        }


        mAnimator = (Animation)this.GetComponent("Animation");
        if (mAnimator)
        {
            PlayIdle();
        }

        DelayFollow();
    }
    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        if(Master != null)
        {
            EventParameter ep = EventParameter.Get();
            ep.objParameter = Master;
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_MAGICKEY_INFO_DESTORY, ep);
        }
    }
    void Update()
    {
        if (Master == null)
        {
            return;
        }
        interval = interval - Time.deltaTime;
        if (interval <= 0 && !isFollowed)
        {
            interval = ActionSwitchTime;
            if (!string.IsNullOrEmpty(leisure))
            {
                mAnimator.Play(leisure);
                bleisurePlaying = true;
            }
        }
        //leisure播放动作切回idle
        if (bleisurePlaying && !isFollowed)
        {
            if (!mAnimator.isPlaying)
            {
                bleisurePlaying = false;
                PlayIdle();
            }
        }
        UpdateFollow();

        if (Time.time - lastUpdateTime > UpdatePeriod )
        {
            float dist = (transform.position - Master.transform.position).sqrMagnitude;
            if(dist >= ViewSquareDistance )
            {
                if (Master != null)
                {
                    Vector3 position = Master.transform.position;
                    position.y = CommonTools.GetTerrainHeight(transform.position) + fTerrainHight;
                    transform.position = position;
                }
            }
            else if (dist >= MaxSquareDistance)
            {
                if (!isFollowed)
                {
                    if (bCanFollowed)
                    {
                        Invoke("DelayFollow", Random.Range(0.1f, DelayTimeRange));
                    }
                    else
                    {
                        isFollowed = true;
                    }
                }
                
            }
            else
            {
                //Vector3 dir = (Master.transform.position - transform.position).normalized;
                //dir.y = 0;

                //transform.rotation = Quaternion.LookRotation(dir);
                
            }

            lastUpdateTime = Time.time;
        }
    }

    void PlayIdle()
    {
        if (mAnimator)
        {
            mAnimator.Play(idle);
        }
    }
    void DelayFollow()
    {
        CancelInvoke("DelayFollow");
        isFollowed = true;
        if (!string.IsNullOrEmpty(run))
        { 
            mAnimator.Play(run); 
        }

    }
    void UpdateFollow()
    {
        if (!isFollowed)
        {
            return;
        }
        interval = ActionSwitchTime;

        Vector3 dir = Master.transform.position - transform.position;
        if(dir.sqrMagnitude <= MinSquareDistance)
        {
            if(isFollowed)
            {
                PlayIdle();
            }
            isFollowed = false;
        }

        // speed = Master.m_Speed * 1.5f; 

        dir.Normalize();
        dir.y = 0;


        Vector3 position = transform.position + dir * Time.deltaTime * speed;
        position.y = CommonTools.GetTerrainHeight(transform.position) + fTerrainHight;
        transform.position = position;
        
        if (dir == Vector3.zero)        
            return;        
        transform.rotation = Quaternion.LookRotation(dir);
        
    }   
   
}

