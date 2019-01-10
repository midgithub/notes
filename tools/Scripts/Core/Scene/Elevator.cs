/**
* @file     : Elevator.cs
* @brief    : 场景里电梯相关的功能
* @details  : 支持三个方向曲线可调
* @author   : 
* @date     : 2014-11-18
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class Elevator : MonoBehaviour
    {
        public float m_fTime = 5f;              //运行时间
        public float m_fRadius = 3.0f;          //触发电梯启动的半径
        public float m_fStartDelayTime = 1.0f;  //电梯启动的延迟时间
        public bool m_bLoop = false;            //电梯是否循环
        public float m_fLoopStopTime = 2f;      //循环电梯的停顿时间
        public int m_iUp = 1;                   //循环电梯的运行方向

        public AnimationCurve m_ElevatorXCurve;
        public AnimationCurve m_ElevatorZCurve;
        public AnimationCurve m_ElevatorYCurve;

        public GameObject m_efxPrefab = null;
        private GameObject m_efxObj = null;

        public string m_SoundPath = null;
        private AudioSource m_AudioSource = null;

        // Use this for initialization
        void Start()
        {
            m_floor = this.gameObject.transform.FindChild("floor");

            GameObject efxObj = Instantiate(m_efxPrefab) as GameObject;
            if (efxObj != null)
            {
                if (m_floor != null)
                {
                    efxObj.transform.position = m_floor.position;
                    efxObj.SetActive(false);
                }

                m_efxObj = efxObj;
            }

            if (m_SoundPath != null)
            {
                m_AudioSource = this.gameObject.GetComponent<AudioSource>();
                if (m_AudioSource == null)
                {
                    m_AudioSource = this.gameObject.AddComponent<AudioSource>();
                    m_AudioSource.playOnAwake = false;
                }

                AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(m_SoundPath, typeof(AudioClip));
                if (clip == null)
                {
                    LogMgr.UnityLog("load clip error! clip=" + m_SoundPath);
                    return;
                }

                m_AudioSource.clip = clip;
            }
        }

        //启动
        public void Open()
        {
            m_bActive = true;

            m_wall = this.gameObject.transform.FindChild("wall");
            if (m_wall != null)
            {
                m_wall.gameObject.SetActive(true);
                //m_wall.gameObject.active = true;
            }

            m_edoor1 = this.gameObject.transform.FindChild("edoor1");
            if (m_edoor1 != null)
            {
                m_edoor1.gameObject.SetActive(true);
                //m_edoor1.active = true;
            }

            m_edoor2 = this.gameObject.transform.FindChild("edoor2");
            if (m_edoor2 != null)
            {
                m_edoor2.gameObject.SetActive(true);
                //m_edoor2.active = true;
            }

            m_edoor3 = this.gameObject.transform.FindChild("edoor3");
            if (m_edoor3 != null)
            {
                m_edoor3.gameObject.SetActive(true);
                //m_edoor3.active = true;
            }

            if (m_efxObj != null)
            {
                m_efxObj.SetActive(true);
            }

            if (m_AudioSource != null)
            {
                m_AudioSource.Play();
            }

            if (m_floor)
            {
                for (int i = 0; i < m_floor.childCount; i++)
                {
                    Transform transChild = m_floor.GetChild(i);
                    Animation ani = transChild.GetComponent<Animation>();
                    if (ani)
                    {
                        ani.Play();
                    }
                }
            }

            m_fCurrentTime = 0.0f;
            if(m_floor!=null)
            {
                //将玩家向电梯中间拉一段位移，避免玩家掉落电梯
                ActorObj playerObject = CoreEntry.gActorMgr.MainPlayer;
                if(playerObject!=null)
                {
                    Vector3 vFloor = m_floor.position;
                    vFloor.y = playerObject.transform.position.y;
                    float fDis = Vector3.Distance(vFloor, playerObject.transform.position) - m_fRadius;
                    if (playerObject && m_floor && fDis > 0)
                    {
                        playerObject.transform.position += Vector3.Normalize(vFloor - playerObject.transform.position) * (fDis + 0.2f);
                    }
                }
            }

        }

        //停止
        public void Close()
        {
            if (m_efxObj)
            {
                m_efxObj.SetActive(false);
            }

            this.gameObject.SetActive(false);
            m_bActive = false;

            if (m_AudioSource != null)
            {
                m_AudioSource.Stop();
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!m_bActive)
            {
                if (CoreEntry.gActorMgr.MainPlayer == null)
                {
                    return;
                }

                ActorObj playerObject = CoreEntry.gActorMgr.MainPlayer;
                if (m_floor != null)
                {
                    //Vector3 vFloor = m_floor.position;
                    //vFloor.y = playerObject.transform.position.y;
                    //if (Vector3.Distance(playerObject.m_transform.position, vFloor) < m_fRadius)
                    //{
                    //    m_fCurrentDelayTime += Time.deltaTime;
                    //    if (m_bReset && (m_fCurrentDelayTime > m_fStartDelayTime))
                    //    {
                    //        Open();
                    //    }
                    //}

                    //判断主角是否在电梯上,如果在的话准备开始启动电梯
                    RaycastHit beHit;
                    int layerMask = 1 << LayerMask.NameToLayer("ground");
                    if (playerObject && Physics.Raycast(playerObject.GetPosition() + Vector3.up * 10, -Vector3.up, out beHit, 20, layerMask))
                    {
                        if (beHit.collider.gameObject == m_floor.gameObject)
                        {
                            m_fCurrentDelayTime += Time.deltaTime;
                            if (m_bReset && (m_fCurrentDelayTime > m_fStartDelayTime))
                            {
                                Open();
                            }
                        }
                        else
                        {
                            if (!m_bReset)
                            {
                                //玩家出了电梯之后电梯重置
                                m_bReset = true;
                            }
                            m_fCurrentDelayTime = 0.0f;
                        }
                    }
                }

                return;
            }

            m_fCurrentTime += Time.deltaTime;
            if (m_fCurrentTime >= m_fTime)
            {
                if (m_wall != null)
                {
                    //m_wall.active = false;
                    m_wall.gameObject.SetActive(false);
                }

                //美术要求动画做成单次播放的,不需要程序停止
                //if (m_floor != null)
                //{
                //    for (int i = 0; i < m_floor.childCount; i++)
                //    {
                //        Transform transChild = m_floor.GetChild(i);
                //        Animation ani = transChild.GetComponent<Animation>();
                //        if (ani)
                //        {
                //            ani.Stop();
                //        }
                //    }
                //}

                //循环电梯
                if (m_bLoop)
                {
                    if (m_iUp == 1)
                    {
                        if (m_edoor2 != null)
                        {
                            m_edoor2.gameObject.SetActive(false);
                            //m_edoor2.active = false;
                        }
                    }
                    else
                    {
                        if (m_edoor1 != null)
                        {
                            m_edoor1.gameObject.SetActive(false);
                            //m_edoor1.active = false;
                        }
                    }

                    if (m_edoor3 != null)
                    {
                        m_edoor3.gameObject.SetActive(false);
                        //m_edoor3.active = false;
                    }

                    if (m_fCurrentTime > m_fTime + m_fLoopStopTime)
                    {
                        m_iUp = -1 * m_iUp;

                        if (m_fLoopStopTime > 0)
                        {
                            Open();
                        }
                        else
                        {
                            m_bActive = false;
                            m_bReset = false;
                        }
                        m_deltaPos = Vector3.zero;
                        m_LastPos = Vector3.zero;
                        m_fCurrentTime = 0f;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                m_CurPos.x = m_ElevatorXCurve.Evaluate(m_fCurrentTime / m_fTime);
                m_CurPos.y = m_ElevatorYCurve.Evaluate(m_fCurrentTime / m_fTime);
                m_CurPos.z = m_ElevatorZCurve.Evaluate(m_fCurrentTime / m_fTime);

                m_deltaPos = m_CurPos - m_LastPos;

                if (m_floor != null)
                {
                    m_floor.position += m_iUp * m_deltaPos;
                }

                if (m_wall != null)
                {
                    m_wall.position += m_iUp * m_deltaPos;
                }

                //判断主角是否在电梯上,如果在的话移动主角位置
                ActorObj playerObject = CoreEntry.gActorMgr.MainPlayer;
                RaycastHit beHit;
                int layerMask = 1 << LayerMask.NameToLayer("ground");
                if (Physics.Raycast(playerObject.transform.position + Vector3.up * 10, -Vector3.up, out beHit, 20, layerMask))
                {
                    if(m_floor!=null)
                    {
                        if (beHit.collider.gameObject == m_floor.gameObject)
                        {
                            playerObject.transform.position += m_iUp * m_deltaPos;
                        }
                    }
                }

                m_LastPos = m_CurPos;
            }
        }

        private float m_fCurrentTime = 0.0f;
        private float m_fCurrentDelayTime = 0.0f;
        private bool m_bReset = true;              //电梯重置完毕,可以开始运行
        private Vector3 m_CurPos = new Vector3(0f, 0f, 0f);
        private Vector3 m_LastPos = new Vector3(0f, 0f, 0f);
        private Vector3 m_deltaPos = new Vector3(0f, 0f, 0f);
        private bool m_bActive = false;

        private Transform m_wall;
        private Transform m_floor;
        private Transform m_edoor1;
        private Transform m_edoor2;
        private Transform m_edoor3;
    }

};//End SG

