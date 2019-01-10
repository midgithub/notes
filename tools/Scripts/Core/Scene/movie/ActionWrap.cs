using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class ActionWrap : MonoBehaviour {


    Animation m_anim  ;
    void Awake()
    {
        if (m_anim == null)
        {
            m_anim = transform.GetComponentInChildren<Animation>();
            if (m_anim != null)
            {
                Component[] componet = m_anim.GetComponents(typeof(MonoBehaviour));
                for (int i = 0; i < componet.Length; i++)
                {
                    MonoBehaviour bh = (MonoBehaviour)componet[i];
                    bh.enabled = false;
                }
                m_anim.enabled = true;
            } 
        }
    }

    void OnEnable()
    {



    }
	// Use this for initialization
	void Start () {
	

	}

    public void PlayIdle()
    {
        if (m_anim != null)
        {
            m_anim.Play("idle");
        }
    }

    public void PlayDie001()
    {
        if (m_anim != null)
        {
            m_anim.Play("die001");
            //m_anim.PlayQueued("idle"); 
        }
    }

    public void PlayAttack001()
    {
        if (m_anim != null)
        {
            m_anim.Play("attack001");
            m_anim.PlayQueued("idle");
        }
    }

	public void PlayAction( string actionName)
    {
        if (m_anim != null)
        {
            m_anim.Play(actionName);
            //m_anim.PlayQueued("idle");
        }

    }


    Transform oldParent = null; 

    public void boundModel(GameObject go)
    {
        UiUtil.ClearAllChild(transform);

        m_anim = go.GetComponent<Animation>();
        PlayerAgent angent = m_anim.GetComponent<PlayerAgent>();
        if (angent != null)
        {
            angent.enabled = false;
            angent.PlayerObj.StopMove(false);
           // PlayerAgent angent = m_anim.GetComponent<PlayerAgent>();

        }

        oldParent = m_anim.transform.parent;


        //GameObject model =  Instantiate(go) as GameObject;
        //model.transform.parent = transform;
        //model.transform.localPosition = Vector3.zero;
        //model.transform.localRotation = Quaternion.EulerAngles(Vector3.zero);

        //m_anim = model.GetComponent<Animation>();
        //Component[] componet = m_anim.GetComponents(typeof(MonoBehaviour));
        //for (int i = 0; i < componet.Length; i++)
        //{
        //    MonoBehaviour bh = (MonoBehaviour)componet[i];
        //    bh.enabled = false;
        //}
        //m_anim.enabled = true;
    }
    void resetPosition()
    {

        m_anim.transform.parent = transform;
        m_anim.transform.localPosition = Vector3.zero;
        m_anim.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //Invoke("resetPosition", 0.05f); 

    }

    void Update()
    {
        if (oldParent != null && m_anim!=null )
        {
            resetPosition(); 
            
        }

    }
    public void releaseModel()
    {


        if (oldParent != null)
        {
            m_anim.transform.parent = oldParent;
 

            PlayerAgent angent = m_anim.GetComponent<PlayerAgent>();
            if (angent != null)
            {
                angent.enabled = true;
                //angent.PlayerObj.StopMove(false);
                // PlayerAgent angent = m_anim.GetComponent<PlayerAgent>();
            }


            oldParent = null;
            m_anim = null;
        }

        CancelInvoke(); 
        //UiUtil.ClearAllChild(transform);
        //oldParent = go.transform.parent;
        //go.transform.parent = transform;
        //go.transform.localPosition = Vector3.zero;
        //go.transform.localRotation = Quaternion.EulerAngles(Vector3.zero);
        //m_anim = model.GetComponent<Animation>();

    }

}

