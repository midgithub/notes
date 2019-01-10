/**
* @file     : ToggleText.cs
* @brief    : 
* @details  : 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using UnityEngine.EventSystems;
using SG;
[LuaCallCSharp]

public class ToggleText : MonoBehaviour {
    public GameObject MarkObj = null;
    //选中颜色
    public Color SelectOutLin;
    public Color SelectColor;
    public int SelectFontSize = 20;
    

    //默认颜色
    public Color DefaultOutLin;
    public Color DefaultColor;
    public int DefaultFontSize = 18;
     
    public int Index = 0;
    
    public Text mText;
    Outline mTextOutLine;
    Toggle mToggle;
    public Graphic mCheckEffect;
    public SpriteAnimation mEffectAni = null;
    [CSharpCallLua]
    public delegate void UnityAction(int arg,bool b);

    public UnityAction action;
    private bool bClickSound = false;
    void Awake () {
        GetText();
        mToggle = transform.GetComponent<Toggle>();
        if (mToggle != null)
        {
            mToggle.onValueChanged.AddListener(OnToggleClick);
        }
        if (mToggle != null)
        {
            OnToggleClick(mToggle.isOn);
        }

	}
    public void OnEnable()
    {
        bClickSound = false;
        Invoke("DelayClickSound", 0.01f);
    }
    public void DelayClickSound()
    {
        bClickSound = true;
    }
    private void GetText()
    {
        if (mText == null)
        {
            Transform trs = transform.FindChild("Label");
            if (trs != null)
            {
                mText = trs.GetComponent<Text>();
                mTextOutLine = trs.GetComponent<Outline>();
            }
        }
    }
    public void SetText(string txt)
    {
        GetText();
        if (mText)
        {
            mText.text = txt;
        }
    } 
   
    public void OnToggleClick(bool b)
    {
        var normbg = transform.FindChild("Background");
        if(normbg != null)
        {
            normbg.GetComponent<Image>().enabled = !b;
        }

        if(mCheckEffect != null)
        { 
            if (mCheckEffect.gameObject.activeSelf != b)
            {
                mCheckEffect.gameObject.SetActive(b);
            }
            if(mEffectAni == null)
            {
                mEffectAni = mCheckEffect.GetComponent<SpriteAnimation>();
                if(mEffectAni != null)
                {
                    mEffectAni.ReLoad("EffectAnniu:anniufx2_");
                }
            }
            //mCheckEffect.CrossFadeAlpha(b ? 1f : 0f, 0.1f, true);
        }
        if(bClickSound && b)
        {
           SG.CoreEntry.gAudioMgr.PlayUISound(900003);
        }
        if (mText)
        {
            mText.color = b ? SelectColor : DefaultColor;
            //if (SelectFontSize != 0 && SelectFontSize != DefaultFontSize)
            mText.fontSize = b ? SelectFontSize : DefaultFontSize;

        }
        if (mTextOutLine)
        mTextOutLine.effectColor = b ? SelectOutLin : DefaultOutLin;
        if (action != null)
            action(Index, b);
    } 
}

