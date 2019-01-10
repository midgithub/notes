

/**
* @file     : ISkillCell.cs
* @brief    : 
* @details  : 相机拉升功能
* @author   : 
* @date     : 2014-11-28 9:31
*/

using XLua;
using UnityEngine;
using System.Collections;

//namespace SG

using SG;

//{

[Hotfix]
public class CameraEvent : MonoBehaviour
{
    protected Transform m_transform;
    private Transform m_target = null;
    private CameraFollow m_cameraFollow;

    //最大拉升距离         
    public float m_OffsetLen = 15f;
    //拉升速度
    public float m_Speed = 0.4f;

    //private float m_fFollowHeight = 5f;
    private float m_MinLen = 7f;

    private float m_fHeight = 7f;
    private float m_MaxLen = 15f;


    private bool m_bActive = false;
    private Vector3 m_LastPos;

    private bool m_bMove = false;

    private EventMgr m_eventMgr;

    ////距离跟随目标的距离        
    //private float m_distance;

    ////和目标的偏移角度
    //private float m_rotationAngle;

    GameObject mainCameraObj = null;

    void Awake()
    {
        m_eventMgr = CoreEntry.gEventMgr;
        RegisterEvent();
    }

    // Use this for initialization
    void Start()
    {
        mainCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        m_transform = mainCameraObj.transform;
    }


    void ResetCarmeraFollow()
    {
        CameraFollow follow = mainCameraObj.GetComponent<CameraFollow>();
        //m_distance = follow.m_distance;
        //m_rotationAngle = follow.m_rotationAngle;
        m_target = follow.m_target;
        //m_fFollowHeight = follow.m_height;
        m_LastPos = m_target.position;

        m_MinLen = Vector3.Distance(m_transform.position, m_target.position);
        m_fHeight = m_MinLen;

        m_MaxLen = m_fHeight + m_OffsetLen;

    }

    public Transform UpdateCamera()
    {
        float targetHeight = m_target.position.y;
        float fDist = targetHeight - m_LastPos.y;
        float fDir = 1f;
        if (fDist == 0)
        {
            return m_transform;
        }

        if (fDist < 0)
        {
            fDir = -1f;
        }


        float fDistance = fDir * m_Speed * 0.1f;//Time.deltaTime;

        m_fHeight += fDistance;

        if (m_fHeight > m_MaxLen)
        {
            m_fHeight = m_MaxLen;
        }

        if (m_fHeight < m_MinLen)
        {
            m_fHeight = m_MinLen;
        }

        Quaternion currentRotation = Quaternion.Euler(m_transform.rotation.eulerAngles);

        // Set the position of the camera on the x-z plane to:	    
        m_transform.position = m_target.position;
        m_transform.position += currentRotation * Vector3.forward * m_fHeight * -1;
        m_LastPos = m_target.position;

        return m_transform;
    }

    void Update()
    {

        if (m_bActive)
        {
            float fDist = Vector3.Distance(m_LastPos, m_target.position);
            if (fDist > 0.1f)
            {
                m_bMove = true;
            }

        }

        if (!m_bActive || !m_bMove)
        {
            return;
        }

        UpdateCamera();
    }


    // Update is called once per frame
    void LateUpdate()
    {

    }

    void OnDestroy()
    {
        RemoveEvent();
    }

    //注册技能释放事件
    void RegisterEvent()
    {
        m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
        m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);

        m_eventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_BEGIN, EventFunction);
        m_eventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_STOP, EventFunction);
        m_eventMgr.AddListener(GameEvent.GE_PLAYER_MOVE_END, EventFunction);

    }


    void RemoveEvent()
    {
        m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
        m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);

        m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_MOVE_BEGIN, EventFunction);
        m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_MOVE_STOP, EventFunction);
        m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_MOVE_END, EventFunction);

    }

    public void EventFunction(GameEvent ge, EventParameter parameter)
    {
        switch (ge)
        {
            case GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE:
                {
                    if (!m_bActive)
                    {
                        ResetCarmeraFollow();
                    }

                    m_bActive = true;
                }
                break;

            case GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE:
                {
                    m_bActive = false;
                }
                break;


            case GameEvent.GE_PLAYER_MOVE_BEGIN:
                {
                    m_bMove = true;

                }
                break;

            case GameEvent.GE_PLAYER_MOVE_STOP:
            case GameEvent.GE_PLAYER_MOVE_END:
                {
                    m_bMove = false;

                }
                break;



            default:
                break;
        }
    }
}

