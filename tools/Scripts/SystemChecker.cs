using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class SystemChecker : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        public IEnumerator Check()
        {
            if (SystemInfo.systemMemorySize <= 512)
           // if( true )
            {
                //string text = "当前设备内存："+SystemInfo.systemMemorySize.ToString()+"M, 请使用大于512M的设备";

                //TipsDialog.SetSingleText_Original(gameObject, text, "确定", new TipsDialog.OnDelegateClick(onClose));

                //while (true)
                //{
                //    yield return new WaitForSeconds(1f); ;
                //}
            }

            yield return null;
        }

        void onClose()
        {
            Application.Quit();
        }
    }
}

