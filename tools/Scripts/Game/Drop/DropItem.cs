/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using XLua;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SG
{
[Hotfix]
    public class DropItem : MonoBehaviour
    {
        private AudioSource m_AudioSource = null;
        public bool CanPick;
        private Transform mTransfm;
        private Transform mFallTran;
        private Transform mStayTran;
        private Image mItemImage;

        public DropMgr.DROP_TYPE itemType;
        private int count;
        private string itemName;
        private int quality = -1;

        public void Init(DropMgr.DROP_TYPE type, string itemName, int count, string icon,int quality)
        {
            this.count = count;
            this.itemName = itemName;
            this.quality = quality;
            this.itemType = type;

            if (type == DropMgr.DROP_TYPE.DROPTYPE_COMMON)
            {
                if (null != mItemImage)
                {
                    mItemImage.sprite = AtlasSpriteManager.Instance.GetSprite(icon);
                    mItemImage.SetNativeSize();
                }
            }
        }

        void Awake()
        {
            mTransfm = transform;

            mFallTran = mTransfm.FindChild("fallObj");
            mStayTran = mTransfm.FindChild("dropItem");
            Transform imageTran = mStayTran.FindChild("Image");
            if (null != imageTran)
            {
                mItemImage = imageTran.GetComponent<Image>();
            }

            m_AudioSource = this.gameObject.GetComponent<AudioSource>();
            if (m_AudioSource == null)
            {
                m_AudioSource = this.gameObject.AddComponent<AudioSource>();
                m_AudioSource.playOnAwake = false;
            }

            mFallTran.gameObject.SetActive(false);
            mStayTran.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            CanPick = false;
            

            float f = 1.0f;

            mFallTran.gameObject.SetActive(true);
            mStayTran.gameObject.SetActive(false);
            Invoke("PlayStayEfxObj", f);
            
            f = ConfigManager.Instance.Consts.GetValue<float>(427, "fval");
            Invoke("SetCanPick", f);

            if (CoreEntry.cfg_bEaxToggle)
            {
                //CoreEntry.SoundEffectMgr.PlaySoundEffect(m_AudioSource, @"Sound/UI/MoneyDrop");
            }
        }

        void OnDisable()
        {
            quality = -1;
        }

        void PlayStayEfxObj()
        {
            mFallTran.gameObject.SetActive(false);
            mStayTran.gameObject.SetActive(true);

            EventToUI.SetArg(UIEventArg.Arg1, gameObject);
            EventToUI.SetArg(UIEventArg.Arg2, string.Format("{0}x{1}", itemName, count));
            EventToUI.SetArg(UIEventArg.Arg3, (int)quality);
            EventToUI.SendEvent("EU_ADD_DROPITEMTEXT");

            CancelInvoke("PlayStayEfxObj");
        }

        void SetCanPick()
        {
            CanPick = true;

            CancelInvoke("SetCanPick");
        }

        void LateUpdate()
        {
            if (null != CoreEntry.gCameraMgr.MainCamera)
            {
                mTransfm.rotation = CoreEntry.gCameraMgr.MainCamera.transform.rotation;
            }
        }
    }
}

