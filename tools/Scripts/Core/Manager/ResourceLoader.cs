using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace SG
{
    [LuaCallCSharp]
    [Hotfix]
    public class ResourceLoader : MonoBehaviour
    {
        private Dictionary<string, GameObject> m_prefabs = new Dictionary<string, GameObject>();
        public void ClearPrefabs()
        {
            if (AppConst.UseAssetBundle)
            {
                foreach (string prefab in m_prefabs.Keys)
                {
                    if (null != m_prefabs[prefab])
                    {
                        DestroyImmediate(m_prefabs[prefab],true);
                    }
                }
            }
            m_prefabs.Clear();
        }

        //只加载 GameObject 预设
        private GameObject LoadPrefabFromPool(string path)
        {
            GameObject prefab;
            m_prefabs.TryGetValue(path, out prefab);
            if (null == prefab)
            {
                prefab = LoadPrefab(path);
                if (m_prefabs.ContainsKey(path))
                {
                    m_prefabs[path] = prefab;
                }else
                {
                    m_prefabs.Add(path, prefab);
                }
            }

            return prefab;
        }

        //加载 GameObject 预设入口
        private GameObject LoadPrefab(string path)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(path, typeof(GameObject));
#endif
            if (AppConst.UseResources)
            {
                return Resources.Load(path, typeof(GameObject)) as GameObject;
            }

            return LoadModule.Instance.LoadPrefab(path);
        }


        //只加载 GameObject 预设
        public GameObject LoadResource(string path)
        {
            return LoadPrefabFromPool(path);
        }

        public GameObject ClonePre(string path, Transform parent = null, bool reset = true)
        {
            GameObject pre = LoadPrefabFromPool(path);
            if(null != pre)
            {
                GameObject go = GameObject.Instantiate(pre) as GameObject;
                if(null != parent)
                {
                    go.transform.SetParent(parent);
                }

                if (reset)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.zero;
                }
                go.SetActive(true);

                return go;
            }

            return null;
        }

        //加载 GameObject 预设
        public GameObject Load(string path)
        {
            return LoadPrefabFromPool(path);
        }

        //加载 TextAsset
        public TextAsset LoadTextAsset(string strPath, SenLib.AssetType assetType)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(strPath, typeof(TextAsset));
#endif
            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(TextAsset)) as TextAsset;
            }

            return LoadModule.Instance.LoadTextAsset(strPath, assetType);
        }

        //加载 Material
        public Material LoadMaterial(string strPath)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(strPath, typeof(Material));
#endif
            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(Material)) as Material;
            }

            return LoadModule.Instance.LoadMaterial(strPath);
        }

        //加载 Material
        public Shader LoadShader(string shaderName)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(shaderName, typeof(Shader));
#endif
            if (!AppConst.UseAssetBundle)
            {
                return Shader.Find(shaderName);
            }

            string name = shaderName.Replace("/", "-").Replace(" ", "");
            return LoadModule.Instance.LoadShader(name);
        }

        //加载 AudioClip
        public AudioClip LoadAudioClip(string strPath)
        {
#if UNITY_EDITOR
            ResourceRecorder.Instance.RecordResource(strPath, typeof(AudioClip));
#endif
            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(AudioClip)) as AudioClip;
            }

            return LoadModule.Instance.LoadAudio(strPath, SenLib.AssetType.AudioWav);
        }

        //
        public Object Load(string path, System.Type resType)
        {
            if (typeof(GameObject) == resType)
            {
                return LoadPrefabFromPool(path);
            }
            else if (typeof(Material) == resType)
            {
                return LoadMaterial(path);
            }
            else if (typeof(AudioClip) == resType)
            {
                return LoadAudioClip(path);
            }

            return null;
        }

        public string getPersistentDataPath()
        {
            string re = ".";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                re = Application.dataPath + "/..";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                re = Application.persistentDataPath;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                re = Application.persistentDataPath;
            }
            return re;
        }
    }
}
