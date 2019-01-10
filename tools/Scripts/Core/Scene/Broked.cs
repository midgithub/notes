/**
* @file     : Broked.cs
* @brief    : 
* @details  : 物件破碎脚本
* @author   : 
* @date     : 2014-12-17
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

[Hotfix]
    public class Broked : MonoBehaviour
    {
        Transform m_transform = null;

        public MeshRenderer m_staticMesh = null;
        public SkinnedMeshRenderer m_skinMesh = null;



        //质量
        public float m_Mass = 1;
        public float m_displayTime = 5f;

        public int m_nBrokenSound = 0;

        //是否已经破碎了
        private bool m_isHadBroked = false;
        private AudioSource m_audioSource = null;

        // Use this for initialization
        void Start()
        {
            m_isHadBroked = false;
            m_transform = this.transform;

            m_audioSource = this.gameObject.GetComponent<AudioSource>();
            if (m_audioSource == null)
            {
                m_audioSource = this.gameObject.AddComponent<AudioSource>();
                m_audioSource.playOnAwake = false;
            }

            //设置layer
            Transform[] childs = this.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i].gameObject.layer = LayerMask.NameToLayer("broked");
            }

            //设置mesh convex
            MeshCollider[] colliders = this.gameObject.GetComponentsInChildren<MeshCollider>();
            for (int i = 0; i < colliders.Length; ++i)
            {
                colliders[i].convex = true;
            }

            //所有的刚体效果关闭        
            Rigidbody[] bodys = this.gameObject.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodys.Length; ++i)
            {
                bodys[i].isKinematic = true;
                bodys[i].useGravity = false;
                bodys[i].Sleep();
            }
        }

        // Update is called once per frame
        //void Update()
        //{

        //}

        public void DoBroked(GameObject castObject, int skillWeight = 1)
        {
            if (m_isHadBroked)
            {
                return;
            }

            m_isHadBroked = true;
            Rigidbody[] bodys = this.gameObject.GetComponentsInChildren<Rigidbody>();
            if (bodys.Length == 0)
            {
                return;
            }

            Vector3 backDir = castObject.transform.forward;

            //夹角大于60度的使用跟释放者连线方向
            Vector3 brokeDir = m_transform.position - castObject.transform.position;
            float angle = Vector3.Angle(castObject.transform.forward.normalized, brokeDir.normalized);
            if (angle > 60)
            {
                backDir = brokeDir;
            }

            float force1 = 80 + skillWeight * 20f / m_Mass;
            float force2 = 120 + skillWeight * 10f / m_Mass;

            for (int i = 0; i < bodys.Length; ++i)
            {
                bodys[i].isKinematic = false;
                bodys[i].useGravity = true;
                bodys[i].WakeUp();

                Random.seed = System.Guid.NewGuid().GetHashCode();
                float x = Random.Range(0.5f, 1.0f);
                float y = Random.Range(0.5f, 1.0f);
                float z = Random.Range(0.5f, 1.0f);

                Vector3 dir = new Vector3(x, y, z);

                bodys[i].AddForce(dir.normalized * force1);
                bodys[i].AddForce(backDir.normalized * force2);
                bodys[i].AddForce(Vector3.up * force1);
            }
            if (m_staticMesh != null)
            {
                m_staticMesh.enabled = false;  // 隐藏静态模型
            }

            if (m_skinMesh != null)
            {
                m_skinMesh.enabled = true;   // 显示动态模型

            }

            int nRand = Random.Range(0, 2);
            if (nRand == 1)
            {
                //随机生成掉落的BUFF
                string[] effectlist = { "Effect/scence/sf_buff_baoji",
                                "Effect/scence/sf_buff_fafang",
                                "Effect/scence/sf_buff_shanbi" };

                int rangeEffect = Random.Range(0, effectlist.Length);

                GameObject newObj = Instantiate(CoreEntry.gResLoader.LoadResource(effectlist[rangeEffect])) as GameObject;
                if (newObj != null)
                    newObj.transform.position = new Vector3(m_transform.position.x, m_transform.position.y, m_transform.position.z);
            }


            string brokedSound = "Sound/common/broked/xiangzi01";
            //bool ret = AudioCore.GenerateAudio(m_nBrokenSound, ref brokedSound);
            if (brokedSound.Length > 0)
            {
                StopSound();
                PlaySound(brokedSound);
            }

            //10秒后，取消物理
            Invoke("AutoCancelGravity", 10);

            //5秒后，消失
            Invoke("AutoDisplay", m_displayTime);
        }

        //播放声音
        void PlaySound(string strClip)
        {
            //add by Alex 20150416 音效控制开关
            if (!CoreEntry.cfg_bEaxToggle)
            {
                return;
            }

            CoreEntry.SoundEffectMgr.PlaySoundEffect(m_audioSource, strClip);

        }

        void StopSound()
        {
            if (m_audioSource)
            {
                m_audioSource.Stop();
            }
        }

        void AutoCancelGravity()
        {
            CancelInvoke("AutoCancelGravity");

            //所有的刚体效果关闭        
            Rigidbody[] bodys = this.gameObject.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodys.Length; ++i)
            {
                bodys[i].isKinematic = true;
                bodys[i].useGravity = false;
                bodys[i].Sleep();
            }
        }

        void AutoDisplay()
        {
            CancelInvoke("AutoDisplay");

            //场景删除
            CoreEntry.gSceneMgr.RemoveBroked(this.gameObject);

            Destroy(this.gameObject);
        }
    }
}

