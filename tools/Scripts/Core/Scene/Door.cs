/**
* @file     : Door.cs
* @brief    : 
* @details  : 阻挡门组件,采用碰撞体实现。
* @author   : 
* @date     : 2014-9-18 
*/
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif
using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{

[Hotfix]
public class Door : MonoBehaviour {
    //id
    public int m_InDoor = 0;        //类型为1的时候表示进入某个区域就显示出来的门(一般用于boss战不准玩家出去),清完该区域的怪后消失
    public int m_id = 0;
	public bool isUsed = true;
    
    public GameObject m_beginEfxPrefab = null;
    private GameObject m_beginEfxObj = null;
	public GameObject m_holdEfxPrefab = null;
	private GameObject m_holdEfxObj = null;
	public GameObject m_overEfxPrefab = null;
	private GameObject m_overEfxObj = null;
	private NavMeshObstacle m_navObs = null;
    
	// Use this for initialization
	void Start () 
	{

		if (m_beginEfxPrefab != null && m_holdEfxPrefab != null && m_overEfxPrefab != null)
        {
			m_beginEfxObj = Instantiate(m_beginEfxPrefab) as GameObject;
			if (m_beginEfxObj != null)
			{
                m_beginEfxObj.transform.position = this.transform.position;
                m_beginEfxObj.transform.localRotation = this.transform.localRotation;
             } else
                {
                    LogMgr.UnityError("beginEfxObj create failed!");
                }

			m_holdEfxObj = Instantiate(m_holdEfxPrefab) as GameObject;
			if (m_holdEfxObj != null)
			{
                m_holdEfxObj.transform.position = this.transform.position;
                m_holdEfxObj.transform.localRotation = this.transform.localRotation;
                //yy
                m_holdEfxObj.AddComponent<DetailObject>();
                m_holdEfxObj.SetActive(false);

			}else
                {
                    LogMgr.UnityError("holdEfxObj create failed!");
                }


                m_overEfxObj = Instantiate(m_overEfxPrefab) as GameObject;
			if (m_overEfxObj != null)
			{
                m_overEfxObj.transform.position = this.transform.position;
                m_overEfxObj.transform.localRotation = this.transform.localRotation;
                m_overEfxObj.SetActive(false);
			}else
                {
                    LogMgr.UnityError("overEfxObj create failed!");
                }


                Close();
        }

		GameObject obstacle = new GameObject();
		obstacle.name = "obstacle";
		NavMeshObstacle navObs = obstacle.AddComponent<NavMeshObstacle>();
		m_navObs = navObs;
		navObs.radius = 1.0f;
		navObs.carving = true;
		obstacle.transform.parent = this.transform;
		obstacle.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Open();
        }
    }

    void OnEnable()
    {
        if (isUsed == false)
        {
            Open();
            //gameObject.SetActive(false);
        }
    }

    //开门
    public void Open()
    {
        this.gameObject.SetActive(false);

		if (m_beginEfxObj != null && m_holdEfxObj != null && m_overEfxObj != null)
		{
			CancelInvoke("PlayHoldEfxObj");
			m_beginEfxObj.SetActive(false);
			m_holdEfxObj.SetActive(false);
			m_overEfxObj.SetActive(true);
		}
		
		if (m_navObs != null)
		{
			m_navObs.carving = false;
		}
    }   

    public void Close()
    {
        if (isUsed == false)
        {
            return;
        }

        this.gameObject.SetActive(true);

		if (m_beginEfxObj != null && m_holdEfxObj != null && m_overEfxObj != null)
		{
			m_beginEfxObj.SetActive(true);
			m_holdEfxObj.SetActive(false);
			m_overEfxObj.SetActive(false);
			float maxEfxTime = 0;
			NcCurveAnimation[] efxAnimations = m_beginEfxObj.GetComponentsInChildren<NcCurveAnimation>();
			for (int i = 0; i < efxAnimations.Length; ++i)
			{
				float efxTime = efxAnimations[i].m_fDelayTime + efxAnimations[i].m_fDurationTime;
				if (efxTime > maxEfxTime)
				{
					maxEfxTime = efxTime;
				}                
			}

			Invoke("PlayHoldEfxObj", maxEfxTime);
		}

		if (m_navObs != null)
		{
			m_navObs.carving = true;
		}
    }

	void PlayHoldEfxObj()
	{
		m_beginEfxObj.SetActive(false);
		m_holdEfxObj.SetActive(true);
		CancelInvoke("PlayHoldEfxObj");
	}
    
    public bool IsOpen()
    {
        return !this.gameObject.activeSelf;
    }
}

};  //end SG

