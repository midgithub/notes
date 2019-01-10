/**
* @file     : GMButtom.cs
* @brief    : GM窗口控制button
* @details  : 
* @author   : nakewang
* @date     : 2014-12-05
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class GMButtom : MonoBehaviour
    {
#if !PUBLISH_RELEASE
		// Use this for initialization
		void Start () 
		{
			uGUI.UIEventListener.Get(this.gameObject).onPointerClick = GmBtnClick; 
              if (!SG.LogMgr.GMSwitch)
                  gameObject.SetActive(false);
        }

		// Gm按钮单击事件
		void GmBtnClick(GameObject btnObj)
		{
            MainPanelMgr.Instance.ShowDialog("UIGM");
		}
#else
        void Start()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
#endif
	}

};//End SG 

