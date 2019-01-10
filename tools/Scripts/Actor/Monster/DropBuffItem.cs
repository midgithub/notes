using XLua;
﻿using UnityEngine;
using System.Collections;



namespace SG
{


[Hotfix]
    public class DropBuffItem : MonoBehaviour
    {
        public int BuffID;

        // Use this for initialization
        void Start()
        {

        }

        void AutoEnd()
        {
            CancelInvoke("AutoEnd");
            Destroy(this.gameObject);
        }

        //触发器
        void OnTriggerEnter(Collider other)
        {
            ActorObj actorBase = other.transform.root.gameObject.GetComponent<ActorObj>();
            if (actorBase == null)
            {
                actorBase = other.transform.gameObject.GetComponent<ActorObj>();
                
            }

            if (actorBase == null)
            {
                return;
            }
 
            if (actorBase.mActorType == ActorType.AT_LOCAL_PLAYER)
            {
                AutoEnd();

                //增加BUFF的逻辑
              //  actorBase.Addbuff(BuffID, actorBase,0);
            }  
               
        }
    }


}

