using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

namespace SG
{
    [LuaCallCSharp]

    public class UIStar : MonoBehaviour {
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
            if (starEffect)
                starEffect.gameObject.SetActive(false);
            m_curValue = nValue;
            int i = 0;
            for (i = 0; mStartArr.Length >= nValue && i < nValue; i++)
            {
                mStartArr[i].sprite = AtlasSpriteManager.Instance.GetSprite(strStarImg);
            }
            for (; i < mStartArr.Length; i++)
            {
                mStartArr[i].sprite = AtlasSpriteManager.Instance.GetSprite(strDefaultImg);
            }
        }

        public void SetEffectCurValue(int nValue)
        {
            if(mStartArr.Length < nValue)return;
            if (nValue < 1) return;
            if (starEffect==null)
            {
                if (nValue - 1 < 0) return;
                starEffect = (GameObject)CoreEntry.gResLoader.LoadResource(strStarEffect);
                if (starEffect == null) return;
                starEffect = GameObject.Instantiate(starEffect);
                starEffect.gameObject.SetActive(true);
            }
            if (starEffect)
            {
                starEffect.transform.parent = (mStartArr[nValue - 1].transform);
                starEffect.transform.localPosition = Vector3.zero;
                starEffect.transform.localScale = Vector3.one;
                starEffect.SetActive(false);
                starEffect.SetActive(true);
            }
        }

        public string strStarEffect = "Effect/ui/uf_xingji";
        public GameObject starEffect = null;

        public string strDefaultImg;
        public string strStarImg;
       public Image []mStartArr;
    }

}

