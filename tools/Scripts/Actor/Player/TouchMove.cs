using XLua;
﻿using UnityEngine;
using System.Collections;
#if !UNITY_5_3_8
using UnityEngine.AI;
#endif

namespace SG
{

//点击地面组件
[Hotfix]
public class TouchMove : MonoBehaviour {
    private int groundLayerMask;
	//private int uiLayerMask;

    public delegate bool gc_MoveToPos(Vector3 pos, bool bChangeState = true);
    gc_MoveToPos m_callbackMove = null; 

    //设置回调函数    
    public void SetCallbackMoveFun(gc_MoveToPos func)
    {
        m_callbackMove = func;
    }  

	// Use this for initialization
	void Start () {        
        groundLayerMask = 1 << LayerMask.NameToLayer("ground");
		//uiLayerMask = 1 << LayerMask.NameToLayer("UI"); 
	}	

		bool IsMouseOverUI()
		{
            return CommonTools.CheckGuiRaycastObjects();
		}

        void HitBack()
        {
            CoreEntry.gActorMgr.MainPlayer.PlayAction("hit002");
            CoreEntry.gActorMgr.MainPlayer.UseCurveData1("hit002", 1, true);
        }

    void Update()
    {
        //add by lzp 

        if (!MapMgr.Instance.InMainCity  )
        {
            if (!CoreEntry.GameStart || !CoreEntry.InFightScene)
                return;
        }

			
        if (true == IsMouseOverUI())
				return;

        Vector3 touchPos = Vector3.zero;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            //if (Input.GetMouseButtonDown(1))
            //{


            //    HitBack();
            //    //Invoke("HitBack",0.3f);
            //}
            if (Input.GetMouseButtonDown(0))
            {
                touchPos = Input.mousePosition;

             
                {
                    ////取消自动战斗
                    //PlayerObj player = CoreEntry.gActorMgr.MainPlayer ;
                    //if (player != null && CoreEntry.gGameMgr.AutoFight)
                    //{
                    //        CoreEntry.gGameMgr.StopAutoTask();
                    //    CoreEntry.gGameMgr.AutoFight = false;
                    //        player.CancleAutoSet();
                    //}

                 }
                
               
                 
            }
        }
        else if (Input.touchCount > 0)
        {
            int count = Input.touchCount;
            Touch touch = Input.GetTouch(count - 1);
            touchPos = touch.position;

            // PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
       
            //{

            //        // 取消自动战斗
            //        CoreEntry.gGameMgr.StopAutoTask();
            //        if (player != null && CoreEntry.gGameMgr.AutoFight)
            //         player.CancleAutoSet();
          

            //}
           

        //    PlayerObj player = CoreEntry.gTeamMgr.MainPlayer.GetComponent<PlayerObj>();
         //   player.m_bAutoMove = false;

            

        }


        if (touchPos != Vector3.zero)
        {
            if (CoreEntry.gCameraMgr.MainCamera == null)
            {
                return;
            }

            Ray ray = CoreEntry.gCameraMgr.MainCamera.ScreenPointToRay(touchPos);
            //Ray uiRay = CoreEntry.uiMgr.uiCamera.ScreenPointToRay(touchPos);
            //if (Physics.Raycast(uiRay, 1000.0f, uiLayerMask))
            //{
            //    return;
            //}
            PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
       
            //{
            //    //取消自动战斗
                
            //    if (player != null && CoreEntry.gGameMgr.AutoFight)
            //        player.CancleAutoSet();

            //}
          
         
            player.m_bAutoMove = false;

            PlayerAutoMove playerMove = player.GetComponent<PlayerAutoMove>();
            if (playerMove)
            {
                playerMove.Stop(true);
            }


            RaycastHit rayCastHit;
           
             if (Physics.Raycast(ray, out rayCastHit, 1000.0f, groundLayerMask))
            {
                if (m_callbackMove != null)
                {
                    m_callbackMove(rayCastHit.point);             
                }                
            }
             else
             {
                 //如果没取到，就使用NAVMESH
                 NavMeshHit hit;
                 if (NavMesh.SamplePosition(CoreEntry.gCameraMgr.MainCamera.ScreenToWorldPoint(touchPos), out hit, 100.0f, -1))
                 {
                     if (null != m_callbackMove)
                     {
                         m_callbackMove(hit.position);
                     }
                 }

             }

            
        }
        else
        {
            PlayerObj player = CoreEntry.gActorMgr.MainPlayer;
            if (player != null)
            {
                player.m_bAutoMove = true;
            }
        }
    }
}

};  //end SG

