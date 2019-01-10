
using XLua;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SG
{

    //主相机必须带有的
[Hotfix]
    public class CameraLookAtBoss : CameraBase
    {
        enum ST
        {
            SmoothDamp,         //阻尼
            EnterMinDist,       //进入最小区域
            Circle,             //旋转
            ResetSmoothDamp,    //重置数据
            CameraBossCorrect,  //相机和BOSS位置修正
            BossToSky,          //BOSS飞天处理
            BossToSkyEnd,
        }

        private EventMgr m_eventMgr = null;

        protected Transform m_transform;
        public Transform m_playertransform = null;
        public Transform m_monstertransform = null;        

        //距离跟随目标的高度
        public float m_height = 13;               
        private bool m_bActive = false;

        private float xVelocity = 0.0F;
        private float yVelocity = 0.0F;
        private float zVelocity = 0.0f;

        private float currentX;
        private float currentY;
        private float currentZ;
        
        //镜头滞后
        public float distanceSnapTime = 0.3f;
        public float distwithplayer = 13f;
        //与相机的最小距离
        public float distwithcamera = 15f;

        //相机到人物直接的向量
        public Vector3 cmaera2player = new Vector3();
        public Vector3 player2monster = new Vector3();

        //镜头是否跟随
        //bool m_Update = true;
        //bool m_PlayerDeath = false;

        ////保留上
        //Vector3 m_lastpbVector = new Vector3();

        //Vector3 m_pbVector = new Vector3();
        Vector3 m_monsterOldPositon = new Vector3();


        //bool bResetV = false;

        ST sta = ST.SmoothDamp;

        //外留接口
        public float minDist = 3.2f;


        void Awake()
        {
            m_eventMgr = CoreEntry.gEventMgr;
            RegisterEvent(); 
        }

        // Use this for initialization
        void Start()
        {
            m_transform = this.transform;
            currentX = m_transform.position.x;
            currentY = m_transform.position.y;
            currentZ = m_transform.position.z;
          //  m_playertransform = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();

            m_playertransform = CoreEntry.gActorMgr.MainPlayer.transform;
            
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (m_playertransform != CoreEntry.gActorMgr.MainPlayer.transform)
            {
                //Debug.LogError("XXXX Not Found Player");
               // m_playertransform = GameObject.FindGameObjectWithTag("player").GetComponent<Transform>();       
                m_playertransform = CoreEntry.gActorMgr.MainPlayer.transform;
               // m_PlayerDeath = false;

                return;
            }

            if (!m_monstertransform)
            {
				var obj = GameObject.FindGameObjectWithTag("boss");
				if(obj)
				{
					m_monstertransform = obj.GetComponent<Transform>();                        
				}                
                return;
            }

            if (!m_bActive)
            {             
                UpdateMainCameraTransform();
            }

            //第一个条件是是否开启显示帧数或者调试模式
            if (true && this.gameObject.GetComponent<ShowFps>() == null)
            {
                this.gameObject.AddComponent<ShowFps>();
            }
        }


        public override Transform UpdateMainCameraTransform()
        {
            var pb_Vector = new Vector3(m_playertransform.position.x, 0, m_playertransform.position.z) - new Vector3(m_monstertransform.position.x, 0, m_monstertransform.position.z);
            var cb_Vector = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(m_monstertransform.position.x, 0, m_monstertransform.position.z);
            var pb_dst = pb_Vector.magnitude;
            var cb_dst = cb_Vector.magnitude;
            var angle = Quaternion.LookRotation(pb_Vector).eulerAngles.y - Quaternion.LookRotation(cb_Vector).eulerAngles.y;
            //Debug.LogWarning("pb_dst:" + pb_dst + "cb_dst:" + pb_dst);

            switch (sta)
            {
                case ST.SmoothDamp:
                    {
                        pb_Vector.Normalize();
                        var wantedPosition = m_playertransform.position + pb_Vector * distwithplayer;
                        //延时模拟
                        currentX = Mathf.SmoothDamp(currentX, wantedPosition.x, ref xVelocity, distanceSnapTime);
                        currentY = Mathf.SmoothDamp(currentY, wantedPosition.y, ref yVelocity, distanceSnapTime);
                        currentZ = Mathf.SmoothDamp(currentZ, wantedPosition.z, ref zVelocity, distanceSnapTime);
                        m_transform.position = new Vector3(currentX, m_height, currentZ);
                        m_transform.LookAt(new Vector3(m_monstertransform.position.x, 7.5f, m_monstertransform.position.z));

                        if (pb_dst < minDist)
                        {
                            sta = ST.EnterMinDist;
                        }
                    }
                    break;
                case ST.Circle:
                    {
                        var rot = Quaternion.Euler(0f, 2f, 0f);
                        if (angle < 0 || Mathf.Abs(angle)>180)
                        {
                            rot = Quaternion.Euler(0f, -2f, 0f);
                        }
                        
                        cb_Vector = transform.position - m_monstertransform.position;                        
                        var wp = m_monstertransform.position + rot * cb_Vector; 
                        currentX = Mathf.SmoothDamp(currentX, wp.x, ref xVelocity, 0.01f);
                        currentZ = Mathf.SmoothDamp(currentZ, wp.z, ref zVelocity, 0.01f);
                        m_transform.position = new Vector3(currentX,m_height,currentZ);
                        m_transform.LookAt(new Vector3(m_monstertransform.position.x, 7.5f, m_monstertransform.position.z));
                        //Debug.LogWarning(angle);
                        if (Mathf.Abs(angle) < 30)
                        {
                            sta = ST.SmoothDamp;
                            return transform;
                        }
                    }
                    break;
                case ST.EnterMinDist:
                    if (pb_dst >= minDist)
                    {
                        if (Mathf.Abs(angle) < 30)
                        {
                            sta = ST.SmoothDamp;
                        }
                        else
                        {
                            xVelocity = 0;
                            yVelocity = 0;
                            zVelocity = 0;
                            sta = ST.Circle;
                        }
                    }
                    //镜头
                    else if (pb_dst < minDist && cb_dst < distwithcamera)
                    {
                        sta = ST.CameraBossCorrect;
                    }
                    break;
                case ST.ResetSmoothDamp:
                    {
                        //currentX = m_transform.position.x;
                        //currentY = m_transform.position.y;
                        //currentZ = m_transform.position.z;
                        //xVelocity = 0;
                        //yVelocity = 0;
                        //zVelocity = 0;
                        sta = ST.SmoothDamp;
                    }
                    break;
                case ST.CameraBossCorrect:
                    {
                        //pb_Vector.Normalize();
                        //var wantedPosition = m_playertransform.position + pb_Vector * distwithplayer;
                        cb_Vector.Normalize();
                        var wantedPosition = m_monstertransform.position + cb_Vector * distwithcamera;
                        //延时模拟
                        currentX = Mathf.SmoothDamp(currentX, wantedPosition.x, ref xVelocity, 0.01f);
                        currentY = Mathf.SmoothDamp(currentY, wantedPosition.y, ref yVelocity, 0.01f);
                        currentZ = Mathf.SmoothDamp(currentZ, wantedPosition.z, ref zVelocity, 0.01f);
                        m_transform.position = new Vector3(currentX, m_height, currentZ);
                        m_transform.LookAt(new Vector3(m_monstertransform.position.x, 7.5f, m_monstertransform.position.z));

                         //镜头
                        if (pb_dst < minDist && cb_dst >= distwithcamera)
                        {
                            sta = ST.EnterMinDist;
                        }
                        else if (pb_dst > minDist && cb_dst >= distwithcamera)
                        {
                            if (Mathf.Abs(angle) < 30)
                            {
                                sta = ST.SmoothDamp;
                            }
                            else
                            {
                                xVelocity = 0;
                                yVelocity = 0;
                                zVelocity = 0;
                                sta = ST.Circle;
                            }
                        }
                    }
                    break;
                case ST.BossToSky:
                    {
                        //相机直接跟随                
                        var wantedPosition_ = new Vector3();
                        wantedPosition_ = m_playertransform.position + cmaera2player;
                        //延时模拟
                        currentX = Mathf.SmoothDamp(currentX, wantedPosition_.x, ref xVelocity, distanceSnapTime);
                        currentY = Mathf.SmoothDamp(currentY, wantedPosition_.y, ref yVelocity, distanceSnapTime);
                        currentZ = Mathf.SmoothDamp(currentZ, wantedPosition_.z, ref zVelocity, distanceSnapTime);
                        m_transform.position = new Vector3(currentX, m_height, currentZ);
                        //相机看着某点
                        Vector3 _positon = new Vector3();
                        _positon = m_playertransform.position - player2monster;
                        m_transform.LookAt(new Vector3(_positon.x, 7.5f, _positon.z));                        
                    }
                    break;
                case ST.BossToSkyEnd:
                    {
                        if (cb_dst < distwithcamera)
                        {
                            sta = ST.CameraBossCorrect;
                        }
                        else
                        {
                            sta = ST.Circle;
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            //if (!m_Update)
            //{                               
            //    //相机直接跟随                
            //    var wantedPosition_ = new Vector3();
            //    wantedPosition_ = m_playertransform.position + cmaera2player;                
            //    //延时模拟
            //    currentX = Mathf.SmoothDamp(currentX, wantedPosition_.x, ref xVelocity, distanceSnapTime);
            //    currentY = Mathf.SmoothDamp(currentY, wantedPosition_.y, ref yVelocity, distanceSnapTime);
            //    currentZ = Mathf.SmoothDamp(currentZ, wantedPosition_.z, ref zVelocity, distanceSnapTime);
            //    m_transform.position = new Vector3(currentX, m_height, currentZ);
            //    //相机看着某点
            //    Vector3 _positon = new Vector3();
            //    _positon = m_playertransform.position - player2monster;
            //    m_transform.LookAt(new Vector3(_positon.x, 7.5f, _positon.z));
            //    return m_transform;                              
            //}
            //else
            //{
            //    m_transform.LookAt(new Vector3(m_monstertransform.position.x, 7.5f, m_monstertransform.position.z));                                
            //}            

            ////判断相机和BOSS的距离
            //////Debug.LogWarning("cb_dst" + cb_dst);
            //////Debug.LogWarning("pb_dst" + pb_dst);
            //////Debug.LogWarning("cb_Vector 2 pb_Vector angle" + Vector3.Angle(cb_Vector, pb_Vector));

            ////最小距离                        
            //var wantedPosition = new Vector3();                        
            //pb_Vector.Normalize();            
            ////判断是否为0向量
            //if (!pb_Vector.Equals(Vector3.zero))
            //{
            //    //判断和上一帧的向量角度(角度不可能一下子
            //    if (Vector3.Angle(m_pbVector, pb_Vector) < 150)
            //    {
            //        m_pbVector = pb_Vector;
            //    }              
            //}

            //if (pb_dst < 2)
            //{
            //    //相机并不移动
            //    return m_transform;
            //}

            //wantedPosition = m_playertransform.position + m_pbVector * distwithplayer;

            
            ////延时模拟
            //currentX = Mathf.SmoothDamp(currentX, wantedPosition.x, ref xVelocity, distanceSnapTime);
            //currentY = Mathf.SmoothDamp(currentY, wantedPosition.y, ref yVelocity, distanceSnapTime);
            //currentZ = Mathf.SmoothDamp(currentZ, wantedPosition.z, ref zVelocity, distanceSnapTime);
                        
            ////如果当前点和BOSS的距离小于某个距离
            //var will_pos =  new Vector3(currentX, 0, currentZ) - m_monstertransform.position;
            //var wb_dist = will_pos.magnitude;
            
            //angle = Quaternion.LookRotation(m_pbVector).eulerAngles.y - Quaternion.LookRotation(cb_Vector).eulerAngles.y;
            //if (wb_dist < 15 && Mathf.Abs(angle) > 30)
            //{
            //    //Debug.LogWarning("if (wb_dist < 15 && Mathf.Abs(angle) > 30)");
            //    /*
            //    // Set the height of the camera
            //    var a = Quaternion.Slerp(Quaternion.LookRotation(cb_Vector), Quaternion.LookRotation(m_pbVector), Time.time);

            //    var f = Quaternion.LookRotation(cb_Vector).eulerAngles.y;
            //    var t = Quaternion.LookRotation(m_pbVector).eulerAngles.y;
            //    var st = Mathf.SmoothDampAngle(f, f, ref yVelocity, 0.4f);
            //    var v = Quaternion.Euler(0, st, 0) * cb_Vector * 14;
            //    m_transform.position = new Vector3(wantedPosition.x, m_height, wantedPosition.z);    

            //    will_pos.Normalize();                                
            //    //var rot = Quaternion.Lerp(Quaternion.LookRotation(cb_Vector), Quaternion.LookRotation(m_pbVector), Time.deltaTime);
            //    Quaternion rot = new Quaternion();
            //    if (angle > 0)
            //    {
            //        rot = Quaternion.Euler(0, 2f, 0);
            //    }
            //    else
            //    {
            //        rot = Quaternion.Euler(0, -2f, 0);
            //    }
                
                
            //    if (a > 0)
            //    {
            //        //Debug.LogWarning("++++++++++++++++++++");
            //        rot = Quaternion.Euler(0, 2f, 0);    
            //    }
            //    else
            //    {
            //        //Debug.LogWarning("--------------------");
            //        rot = Quaternion.Euler(0, -2f, 0);
            //    }
           
            //    var p = m_monstertransform.position + rot * will_pos * 15;                
            //    wantedPosition = p;
            //    if (bResetV == false)
            //    {
            //        xVelocity = 0;
            //        zVelocity = 0;
            //        bResetV = true;
            //    }
            //    currentX = Mathf.SmoothDamp(currentX, wantedPosition.x, ref xVelocity, 0.1f);
            //    currentY = Mathf.SmoothDamp(currentY, wantedPosition.y, ref yVelocity, 0.1f);
            //    currentZ = Mathf.SmoothDamp(currentZ, wantedPosition.z, ref zVelocity, 0.1f);
            //    */
            //}            
            //else if (wb_dist < 13)
            //{
            //    //Debug.LogWarning(" else if (wb_dist < 13)");
            //    /**
            //    will_pos.Normalize();                
            //    var p = m_monstertransform.position +  will_pos * 13;
            //    wantedPosition = p;
            //    //if (bResetV == false)
            //    //{
            //    //    xVelocity = 0;
            //    //    zVelocity = 0;
            //    //    bResetV = true;
            //    //}                
            //    currentX = Mathf.SmoothDamp(currentX, wantedPosition.x, ref xVelocity, 0.1f);
            //    currentY = Mathf.SmoothDamp(currentY, wantedPosition.y, ref yVelocity, 0.1f);
            //    currentZ = Mathf.SmoothDamp(currentZ, wantedPosition.z, ref zVelocity, 0.1f);
            //    */
            //    if()
            //    var p = m_monstertransform.position + rot * will_pos * 15;
            //    return m_transform;
            //}
            //m_transform.position = new Vector3(currentX, m_height, currentZ);    
       
            return m_transform;
        }

        //注册技能释放事件
        void RegisterEvent()
        {
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_LOADING_OVER, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);
            //m_eventMgr.AddListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);

            //BOSS腾空技能
            m_eventMgr.AddListener(GameEvent.GE_BOSS_RISE_TO_SKY_START, EventFunction);          
            m_eventMgr.AddListener(GameEvent.GE_BOSS_RISE_TO_SKY_STATED, EventFunction);
            m_eventMgr.AddListener(GameEvent.GE_BOSS_RISE_TO_SKY_END, EventFunction);

            //Boss死亡
            m_eventMgr.AddListener(GameEvent.GE_MONSTER_DEATH, EventFunction);
            //Player死亡
            m_eventMgr.AddListener(GameEvent.GE_PLAYER_DEATH, EventFunction);
            
        }

        void MonsterDeathProcedure()
        {
            /*
             * 屏蔽主角为空
             */
            //m_Update = false;
            distanceSnapTime = 1.0f;
            distwithplayer = 20f;

            //记录相机和Player直接的向量
            cmaera2player = transform.position - m_playertransform.position;
            player2monster = m_playertransform.position - m_monsterOldPositon;
        }

        void NotFoundMonsterProcedure()
        {
            //m_Update = false;
            distanceSnapTime = 1.0f;
            distwithplayer = 20f;

            //记录相机和Player直接的向量
            cmaera2player = transform.position - m_playertransform.position;
            player2monster = m_playertransform.position - m_monstertransform.position;
            //                       
            m_monsterOldPositon = m_monstertransform.position;
        }

        void OnDestroy()
        {            
            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_LOADING_OVER, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_ACTIVE, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);
            //m_eventMgr.RemoveListener(GameEvent.GE_CAMERA_EVENT_HEIGHT_DISABLE, EventFunction);

            //BOSS腾空技能
            m_eventMgr.RemoveListener(GameEvent.GE_BOSS_RISE_TO_SKY_START, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_BOSS_RISE_TO_SKY_STATED, EventFunction);
            m_eventMgr.RemoveListener(GameEvent.GE_BOSS_RISE_TO_SKY_END, EventFunction);

            //Boss死亡
            m_eventMgr.RemoveListener(GameEvent.GE_MONSTER_DEATH, EventFunction);
            //Player死亡
            m_eventMgr.RemoveListener(GameEvent.GE_PLAYER_DEATH, EventFunction);            
        }

        public void EventFunction(GameEvent ge, EventParameter parameter)
        {
            switch (ge)
            {
                //case GameEvent.GE_PLAYER_LOADING_OVER:
                //    {
                        
                //    }
                //    break;

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
                case GameEvent.GE_BOSS_RISE_TO_SKY_START:
                    {
                        NotFoundMonsterProcedure();
                        sta = ST.BossToSky;
                    }
                    break;
                case GameEvent.GE_BOSS_RISE_TO_SKY_END:
                    {
                        //m_Update = true;
                        distanceSnapTime = 0.3f;
                        distwithplayer = 9f;
                        sta = ST.BossToSkyEnd;
                    }
                    break;
                //case GameEvent.GE_BOSS_RISE_TO_SKY_STATED:
                //    {
                //        //m_Update = true;
                //    }
                //    break;
                case GameEvent.GE_MONSTER_DEATH:
                    {                        
                        MonsterDeathProcedure();
                    }
                    break;
                //case GameEvent.GE_PLAYER_DEATH:
                //    {
                //        m_PlayerDeath = true;
                //    }
                    //break;
                default:
                    break;
            }
        }

        public override bool IsDisableCameraShake()
        {
            return true;
        }
    }
}; //end SG

