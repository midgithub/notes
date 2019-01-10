/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using UnityEngine;
using System.Collections;
using SG;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class CrownModelShow : MonoBehaviour 
{
    public string mModelParentPath;
    private static RenderTexture[] mTargetTextures;
    private static GameObject mRootGo;
    private static Transform[] mParents;
    private static GameObject[] mModels;

    public void LoadParents(int max)
    {
        GameObject obj = CoreEntry.gResLoader.Load(mModelParentPath) as GameObject;

        if (obj != null)
        {
            mRootGo = (GameObject)Instantiate(obj);
            mRootGo.transform.parent = MainPanelMgr.Instance.UIRoot3d.transform;
            mRootGo.transform.localPosition = Vector3.zero;
            mRootGo.transform.localScale = Vector3.one;
            mRootGo.transform.localRotation = Quaternion.Euler(Vector3.zero);
            mRootGo.SetActive(true);

            mTargetTextures = new RenderTexture[max];
            mParents = new Transform[max];
            mModels = new GameObject[max];

            GameObject go = mRootGo.transform.Find("Camera").gameObject;
            for (int i = 0; i < max; i++)
            {
                Camera camera;
                if (0 == i)
                {
                    mParents[i] = go.transform.Find("modelParent");
                    camera = go.GetComponent<Camera>();
                }
                else
                {
                    GameObject o = (GameObject)Instantiate(go);
                    o.transform.parent = mRootGo.transform;
                    o.transform.localPosition = new Vector3(0f, 100f * i, 0f);
                    o.transform.localScale = Vector3.one;
                    o.transform.localRotation = Quaternion.identity;
                    o.transform.gameObject.SetActive(true);
                    mParents[i] = o.transform.Find("modelParent");
                    camera = o.GetComponent<Camera>();
                }

                mTargetTextures[i] = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
                mTargetTextures[i].antiAliasing = 1;
                mTargetTextures[i].wrapMode = TextureWrapMode.Clamp;
                mTargetTextures[i].filterMode = FilterMode.Trilinear;
                camera.targetTexture = mTargetTextures[i];
            }
        }
    }

    public Texture SetModel(int id, int index, int max)
    {
        if (null == mRootGo)
        {
            LoadParents(max);
        }
        else
        {
            mRootGo.SetActive(true);
        }

        LuaTable mModelCfg = ConfigManager.Instance.Common.GetModelConfig(id);
        if (mModelCfg == null)
        {
            LogMgr.UnityLog("no model : " + id);
            return null;
        }

        string mModelPath = mModelCfg.Get<string>("skl");
        GameObject rolePrefab = CoreEntry.gResLoader.Load(mModelPath) as GameObject;
        if (rolePrefab == null)
        {
            LogMgr.UnityLog("模型路径错误: " + mModelPath);
            return null;
        }

        if (null != mModels[index])
        {
            GameObject.DestroyImmediate(mModels[index]);
        }

        mModels[index] = (GameObject)Instantiate(rolePrefab);
        mModels[index].transform.parent = mParents[index];
        mModels[index].transform.localPosition = Vector3.zero;
        mModels[index].transform.localScale = Vector3.one;
        mModels[index].transform.localRotation = Quaternion.identity;
        mModels[index].SetActive(true);

        string mStandActionName = mModelCfg.Get<string>("san_idle");
        Animation mAnimator = mModels[index].GetComponent<Animation>();
        if (mAnimator != null && !string.IsNullOrEmpty(mStandActionName))
        {
            mAnimator.Play(mStandActionName);
        }

        return mTargetTextures[index];
    }

    public void RemoveModel()
    {
        for (int i = 0; i < mModels.Length; i++)
        {
            if (null != mModels[i])
            {
                GameObject.DestroyImmediate(mModels[i]);

                mModels[i] = null;
            }
        }

        if (null != mRootGo)
        {
            mRootGo.SetActive(false);
        }
    }

    public void OnEnable()
    {
        if (mRootGo != null)
            mRootGo.SetActive(true);
    }
    public void OnDisable()
    {
        if (mRootGo != null)
            mRootGo.SetActive(false);
    }
    public void OnDestroy()
    {
        if (mRootGo != null)
        {
            GameObject.DestroyImmediate(mRootGo);
            mRootGo = null;
        }
    }
}

