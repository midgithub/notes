
using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

    //主相机必须带有的
[Hotfix]
    public class CameraCircle : MonoBehaviour
    {
        protected Transform m_transform;
        protected Transform m_playertransform = null;
        public Transform m_target = null;

        private CameraFollow m_cameraFollow;

        //距离跟随目标的高度
        public float m_height = 4.5f;
        private float HEIGHTSLOPE = 11f;

        public bool m_cameraShake = false;

        //public bool m_horizontal = false;

        private EventMgr m_eventMgr;

        private bool m_bActive = false;

        //距离跟随目标的距离
        public float m_initDistance = 40f;
        public float m_dstDistance = 23f;

        public float m_dist = 0f;
        public float m_distance { get { return m_dist; } set { m_dist = value; } }

        //旋转
        public int m_IncRotationAngle = 1;
        public float m_InitRotaionAngle = -100;
        public float rotationAngle = 0;
        public float m_rotationAngle { get { return rotationAngle; } set { rotationAngle = value; } }
        public float AccmulatedRotation = 90.0f;

        void Awake()
        {
            m_eventMgr = CoreEntry.gEventMgr;
            RegisterEvent();
        }

        // Use this for initialization
        void Start()
        {
            m_transform = this.transform;
            //  m_playertransform = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();        

            m_playertransform = CoreEntry.gActorMgr.MainPlayer.transform;

            m_distance = m_initDistance;
            m_rotationAngle = m_InitRotaionAngle;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!m_playertransform)
            {
                return;
            }
            if (!m_target)
            {
                var boss = GameObject.FindGameObjectWithTag("boss");
                if (boss)
                {
                    m_target = boss.GetComponent<Transform>();
                    //var pb = m_playertransform.position - m_target.position;
                    //pb.Normalize();
                    //m_distance = Vector3.Distance(m_playertransform.position, m_target.position);
                    //m_transform.position = pb * m_distance;                
                }
                return;
            }

            if (!m_bActive)
            {
                //UpdateInputCameraDistance();
                UpdateMainCameraTransform();
            }

            //第一个条件是是否开启显示帧数或者调试模式
            if (true && this.gameObject.GetComponent<ShowFps>() == null)
            {
                this.gameObject.AddComponent<ShowFps>();
            }
        }


        public Transform UpdateMainCameraTransform()
        {
            float ainHeight = m_target.position.y + m_height * (m_distance / HEIGHTSLOPE);
            Quaternion currentRotation = Quaternion.Euler(0, m_rotationAngle, 0);

            // Set the position of the camera on the x-z plane to:	    
            m_transform.position = m_target.position;
            m_transform.position += currentRotation * Vector3.forward * m_distance;

            // Set the height of the camera
            m_transform.position = new Vector3(m_transform.position.x, ainHeight, m_transform.position.z);

            // Always look at the target
            m_transform.LookAt(m_target);

            if (m_distance >= m_dstDistance)
            {
                m_distance--;
            }
            else if (m_rotationAngle <= m_InitRotaionAngle + AccmulatedRotation)
            {
                m_rotationAngle++;
            }
            else
            {
                //拉近和旋转都已经完成,应该切换相机
                CameraBossProcedure.ChangeToCameraLookAtBossProcedure();
            }
            return m_transform;
        }

        //注册技能释放事件
        void RegisterEvent()
        {
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_LOADING_OVER, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);
        }

        void OnDestroy()
        {
            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_LOADING_OVER, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);
        }

        public void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                case GameEvent.GE_PLAYER_LOADING_OVER:
                    {
                        //m_playertransform = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();
                        // reserve it                   
                    }
                    break;
                case GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE:
                    {
                        m_bActive = true;
                    }
                    break;
                case GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE:
                    {
                        m_bActive = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

