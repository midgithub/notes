using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using XLua;

namespace SG
{
    [LuaCallCSharp]

    public class UIEquipStar : MonoBehaviour {
	    // Use this for initialization
	    void Start () {
        }

        int m_curValue = 0;
        public int CurValue
        {
            set{
                SetCurValue(value);
            }
            get
            {
                return m_curValue;
            }
        } 
        public void SetCurValue(int nValue)
        { 
            m_curValue = nValue;
            int i = 0;
            if (nValue == 0)
            {
                for (; i < mStartArr.Length; i++)
                {
                    mStartArr[i].enabled = false;
                }
                return;
            }
            int iconIndex = (nValue - 1 + mStartArr.Length) / mStartArr.Length;
            if (iconIndex > IconList.Count)
                return;

            string strStarImg = IconList[iconIndex - 1];
            string strDefaultImg = "";
            if(iconIndex - 1 <= IconList.Count - 1 && iconIndex > 1)
            {
                strDefaultImg = IconList[iconIndex - 2];
            }
            int starValue = nValue;
            if (iconIndex > 1)
            {
                starValue = nValue - (iconIndex - 1) * mStartArr.Length;
            }
            for (i = 0; mStartArr.Length >= starValue && i < starValue; i++)
            {
                mStartArr[i].enabled = true;
                mStartArr[i].sprite = AtlasSpriteManager.Instance.GetSprite(strStarImg);
            }
            for (; i < mStartArr.Length; i++)
            {
                mStartArr[i].enabled = true;
                if (string.IsNullOrEmpty(strDefaultImg))
                {
                    mStartArr[i].enabled = false;
                    //mStartArr[i].sprite = AtlasSpriteManager.Instance.GetSprite(strDefaultImg);
                }
                else
                {
                    mStartArr[i].sprite = AtlasSpriteManager.Instance.GetSprite(strDefaultImg);
                }
            }
        }
   
        public List<string> IconList = new List<string>();
        public Image []mStartArr;
    }

}

