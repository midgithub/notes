/**
* @file     : ItemFlyAttr.cs
* @brief    : 属性变化飘字
* @details  : 
* @author   : 
* @date     : 
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
namespace SG
{
    [LuaCallCSharp]
[Hotfix]
    public class ItemFlyAttr : MonoBehaviour
    {
        public Text NameText;
        public Text ValueText;
        public FlyAttrCurve AniCurve;
        public NumberScroll Scroll;
        public Text ChangeText;
        public SpriteAnimation EffectFire;
        public GameObject Effect;

        public RectTransform PowerBackLeft;
        public RectTransform PowerBackRight;

        public int ShowType { get; set; }

        private BasicAttrEnum mType;
        //private int mValue;

        public void Init(BasicAttrEnum type, double oldvalue, double newvalue, float x, float y, float delay)
        {

            mType = type;
            var change = newvalue - oldvalue;            
            if (type == BasicAttrEnum.Power)
            {
                //战斗力表现方式不一样                
                Scroll.SetValue((long)oldvalue, false);
                Scroll.SetValue((long)newvalue, true);
                ChangeText.text = change > 0 ? ("+" + change.ToString()) : change.ToString();

                PowerToText fun = LuaMgr.Instance.GetLuaEnv().Global.GetInPath<PowerToText>("Common.PowerToText");
                int len =  (oldvalue > newvalue ? oldvalue : newvalue).ToString().Length * 30;
                if (fun != null)
                {
                    len = fun((oldvalue > newvalue ? oldvalue : newvalue)).Length * 30;
                }

                int clen = ChangeText.text.Length * 30;
                RectTransform valuert = Scroll.GetComponent<RectTransform>();
                //valuert.anchoredPosition = new Vector2(-len / 2, 0);
                valuert.sizeDelta = new Vector2(len, valuert.sizeDelta.y);
                PowerBackLeft.sizeDelta = new Vector2(len / 2 + 100, PowerBackLeft.sizeDelta.y);
                PowerBackRight.sizeDelta = new Vector2(len / 2 + clen + 70, PowerBackRight.sizeDelta.y);
                if (EffectFire != null)
                {
                    EffectFire.Play();
                }
                // if (Effect != null)
                // {
                //     Effect.SetActive(false);
                //     Effect.SetActive(true);
                // }                
            }
            else
            {
                NameText.text = BaseAttr.GetBasicAttrName((int)mType);
                ValueText.text = string.Format("{0}{1}", change > 0 ? "+" : "", change);
            }            
            AniCurve.Init(x, y, delay);
        }
    }
}

