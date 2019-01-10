using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XLua;

namespace SG
{

    //管理器    
    [LuaCallCSharp]
[Hotfix]
    public class AudioMgr : MonoBehaviour
    {
        //游戏开始入口
        void Awake()
        {
            Random.seed = System.Guid.NewGuid().GetHashCode();
        }
        void Start()
        {
        }


        public static string GetAudioPath(int audioID)
        {
            string fileName = null;

            if (audioID == 0)
            {
                return null;
            }
          
            LuaTable desc = ConfigManager.Instance.Common.GetAudioConfig(audioID);

            if (desc != null)
            {
                int trigRanValue = Random.Range(0, 100);
                if (trigRanValue <= desc.Get<int>("triggerRatio"))
                {
                    //int fileRanValue = Random.Range(0, desc.Get<int>("filesNum"));
                    fileName = (desc.Get<string>("fileName_0")).Trim();

                    if (fileName.Length < 1)
                    {
                        fileName = null;
                    }
                }
            }

            return fileName;
        }

        public bool IsPlaying(GameObject obj = null)
        {
            AudioSource source = null;
            if (obj != null)
                source = obj.GetComponent<AudioSource>();
            else
                source = GetComponent<AudioSource>();
            if (source != null)
            {
                return source.isPlaying;
            }
            return false;
        }

        public void PlayUISound(int id, GameObject obj = null)
        {
            if (!CoreEntry.cfg_bEaxToggle)
            {
                return;
            }

            if (id == 0)
            {
                return;
            }

            string path = GetAudioPath(id);
            if (path == null)
            {
                LogMgr.UnityError("无效音效ID:" + id);
                return;
            }

            AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(path, typeof(AudioClip));
            if (clip == null)
            {
                LogMgr.UnityError("音效配置错误 路径:" + path + "  id" + id);
                return;
            }

            AudioSource source = null;
            if (obj != null)
                source = obj.GetComponent<AudioSource>();
            else
                source = GetComponent<AudioSource>();
            if (source == null)
            {
                if (obj != null)
                    source = obj.AddComponent<AudioSource>();
                else
                    source = gameObject.AddComponent<AudioSource>();
                source.minDistance = 1.0f;
                source.maxDistance = 5000;
                source.rolloffMode = AudioRolloffMode.Linear;
                //source.transform.position = transform.position;
            }
            source.clip = clip;

            source.Play();
        }
    }
};//end SG

