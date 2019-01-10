/**
* @file     : 
* @brief    : 
* @details  : 
* @author   : yuxj
* @date     : 2015-5-19
*/

#define USE_POOL_MGR

using XLua;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{

[Hotfix]
public class GameObjPoolMgr : MonoBehaviour 
{
    private GameObject gameObjPool = null;
    private SpawnPool spawnPool = null;
    private Dictionary<string, string> hashName = null;

    void Awake()
    {
        hashName = new Dictionary<string, string>();
        gameObjPool = new GameObject("GameObjPool");
        GameObject.DontDestroyOnLoad(gameObjPool);
        spawnPool = gameObjPool.AddComponent<SpawnPool>();
        CoreEntry.gEventMgr.AddListener(GameEvent.GE_FuBen_exit, OnExitFuBen);



    }

    public void OnExitFuBen(GameEvent ge, EventParameter parameter)
    {
        m_autoReleaseList.Clear();  //清理自动释放列表
        DestroyImmediate(spawnPool);
        spawnPool = gameObjPool.AddComponent<SpawnPool>();
    }

    void OnDestroy()
    {
        //PoolManager.Pools.Destroy("GameObjPoolMgr");
    }

    void OnEnable()
    {
    }
    void Start()
    {
        StartCoroutine(AutoReleaseTick()); 

    }

    List<float> releaseKeys = new List<float>();
    IEnumerator AutoReleaseTick()
    {
        while (true)
        {
            releaseKeys.Clear();
            float curTime = Time.time; 
            foreach (KeyValuePair<float, GameObject> pair in m_autoReleaseList)
            {

                if (curTime > pair.Key)
                {
                    Destroy(pair.Value); 
                    releaseKeys.Add(pair.Key);
                }
            }

            for (int i = 0; i < releaseKeys.Count; i++)
            {
                m_autoReleaseList.Remove(releaseKeys[i]);
            }

            yield return new WaitForSeconds(0.2f); 
        }
    }

    SortedList<float, GameObject> m_autoReleaseList = new SortedList<float, GameObject>(); 
    public GameObject InstantiateEffectAutoRelease(string prefabName, float time)
    {
        GameObject go = InstantiateEffect(prefabName);
        float releaseTime = Time.time + time;

        while (m_autoReleaseList.ContainsKey(releaseTime))
        {
            releaseTime += 0.001f; 
        }

        m_autoReleaseList.Add(releaseTime, go); 
        return go; 
    }
   

    public GameObject InstantiateEffect(string prefabName)
    {
#if USE_POOL_MGR
        string remanEfx;
        remanEfx = prefabName;
        if (CoreEntry.m_bUseExEff && prefabName.Length > 0)
        {
            remanEfx = prefabName + "_test";
        }


        GameObject effect = Instantiate(remanEfx);

        if (effect == null)
        {
            effect = Instantiate(prefabName);
        }


        if (effect != null)
        {
            SetRanderQueue.ChangeAllChild(-1, effect.transform); // 重置材质渲染顺序
        }
        return effect;
#else
        return GameObject.Instantiate(Bundle.AssetBundleLoadManager.Instance.LoadPrefab(prefabName)) as GameObject;
#endif
    }

    public GameObject InstantiateSkillBase(string prefabName)
    {
#if USE_POOL_MGR
        return Instantiate(prefabName);
#else
        return GameObject.Instantiate(Bundle.AssetBundleLoadManager.Instance.LoadPrefab(prefabName)) as GameObject;
#endif
    }

    public GameObject InstantiateSkillCell(string prefabName)
    {
#if USE_POOL_MGR
        return Instantiate(prefabName);
#else
        return GameObject.Instantiate(Bundle.AssetBundleLoadManager.Instance.LoadPrefab(prefabName)) as GameObject;
#endif
    }

    // 出栈 new       Instantiate
    public GameObject Instantiate(string prefabName)
    {
#if USE_POOL_MGR
        Transform tran = null;
        string objName = null;
        if (hashName.ContainsKey(prefabName))
            objName = hashName[prefabName];

        if (objName == null || !spawnPool.prefabPools.ContainsKey(objName))
        {
            GameObject obj = CoreEntry.gResLoader.LoadResource(prefabName) as GameObject;
            if (obj!=null)
                tran = obj.transform;
            //tran = Resources.Load<Transform>(prefabName);
            if (tran != null)
            {
                PrefabPool poolVaule = new PrefabPool(tran);
                //默认初始化1个Prefab
                poolVaule.preloadAmount = 1;
                //开启限制
                poolVaule.limitInstances = true;
                //关闭无限取Prefab
                poolVaule.limitFIFO = false;
                //限制池子里最大的Prefab数量
                poolVaule.limitAmount = 1500;
                //开启自动清理池子
                poolVaule.cullDespawned = false;
                //最终保留
                poolVaule.cullAbove = 0;
                //多久清理一次
                poolVaule.cullDelay = 180;
                //每次清理几个
                poolVaule.cullMaxPerPass = 5;
                //初始化内存池
                //spawnPool._perPrefabPoolOptions.Add(poolVaule);
                spawnPool.CreatePrefabPool(poolVaule);

                hashName[prefabName] = tran.gameObject.name;
            }
            else
            {
                //LogMgr.UnityError("GameObjPoolMgr:Instantiate 加载出错！ Prefab: " + prefabName);
                return null;
            }
        }

        if (hashName.ContainsKey(prefabName))
        {
            tran = spawnPool.Spawn(hashName[prefabName],null);
            if (tran != null)
            {
                tran.parent = null;
                return tran.gameObject;
            }
        }

        //LogMgr.UnityError("GameObjPoolMgr:Instantiate 没有找到！ Prefab: " + prefabName);
        return null;
#else
        return GameObject.Instantiate(Bundle.AssetBundleLoadManager.Instance.LoadPrefab(prefabName)) as GameObject; // 暂时屏蔽
#endif
    }

    // 入栈 delete    Destroy
    public void Destroy(GameObject instance)
    {            
#if USE_POOL_MGR
            if (spawnPool.IsSpawned(instance.transform))
        {
            //instance.SetActive(false);
            instance.transform.parent = gameObjPool.transform;
            spawnPool.Despawn(instance.transform);
        }
#else
        if (instance != null && instance.transform != null)
            MonoBehaviour.Destroy(instance);
        return;
#endif
    }

}

};//End SG

