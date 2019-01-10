using XLua;
﻿using UnityEngine;
using System.Collections;
namespace SG
{
[Hotfix]
    public class CameraBossProcedure : MonoBehaviour
    {
        //相机切换到预警流程
        public static bool ChangeToCameraCircleProcedure()
        {
            var MC = GameObject.FindGameObjectWithTag("MainCamera");            
            var CF = MC.GetComponent<CameraFollow>();
            Destroy(CF);
            MC.AddComponent<CameraCircle>();

            return true;
        }

        public static bool ChangeToCameraCircleProcedureEx()
        {
            var MC = GameObject.FindGameObjectWithTag("MainCamera");
            var CF = MC.GetComponent<CameraFollow>();
            Destroy(CF);
            MC.AddComponent<CameraLookAtBoss>();

            return true;
        }

        //相机切换到延时跟随流程
        public static bool ChangeToCameraLookAtBossProcedure()
        {
            var MC = GameObject.FindGameObjectWithTag("MainCamera");
            var CF = MC.GetComponent<CameraCircle>();
            Destroy(CF);
            MC.AddComponent<CameraLookAtBoss>();
            return true;
        }

        //相机切换到延时跟随流程
        public static bool ChangeToCameraFllowProcedure()
        {
            var MC = GameObject.FindGameObjectWithTag("MainCamera");
            var CLAB = MC.GetComponent<CameraLookAtBoss>();
            if (CLAB)
            {
                Destroy(CLAB);
            }
            var CF = MC.GetComponent<CameraFollow>();
            if (!CF)
            {
                MC.AddComponent<CameraFollow>();
            }            
            return true;
        }

        //相机切换到看固定位置流程
        public static bool ChangeToCameraFixedProcedure(Vector3 vCamera, Vector3 vTarget)
        {
            var MC = GameObject.FindGameObjectWithTag("MainCamera");
            var CLAB = MC.GetComponent<CameraFollow>();
            if (CLAB)
            {
                Destroy(CLAB);
            }
            var CF = MC.GetComponent<CameraLookatPos>();
            if (!CF)
            {
                MC.AddComponent<CameraLookatPos>();
                CF = MC.GetComponent<CameraLookatPos>();
                CF.SetCameraLookatPos(vCamera, vTarget);
            }
            return true;
        }
    }
}

