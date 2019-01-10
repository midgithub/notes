using XLua;
﻿using SG;
using System;
using System.Text;
using UnityEngine;

[Hotfix]
public class sgUtils
{
    public delegate void GameObjectCallback(GameObject go);

    public static GameObject GetInstance(string prefabName)
    {
        var heroPrefab = CoreEntry.gResLoader.Load(prefabName) as GameObject;
        return (GameObject) GameObject.Instantiate(heroPrefab);
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        return CoreEntry.gResLoader.Load ("Prefabs/" + prefabName) as GameObject;
    }

    public static string BytesToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes).Trim('\0');
    }

    public static Vector3 StringToVector3(byte[] coordinate)
    {
        var str = BytesToString(coordinate);
        var vectors = str.Split(new[] {',', '(', ')'}, StringSplitOptions.RemoveEmptyEntries);
        if (vectors.Length != 3)
        {
            return Vector3.zero;
        }
        return new Vector3(float.Parse(vectors[0]), float.Parse(vectors[1]), float.Parse(vectors[2]));
    }

    public static bool RandomMatch(int proba)
    {
        var nextNum = new System.Random().Next(100000);
        return nextNum%proba == 0;
    }

    public static GameObject GetOrCreate(string goName, GameObjectCallback callback = null)
    {
        var go = GameObject.Find(goName);
        if (!go)
        {
            go = new GameObject(goName);
            if (callback != null)
            {
                callback(go);
            }
        }
        return go;
    }

    public static void CreatePrefabAndAttachParent(GameObject parent, string prefabPath)
    {
        if (null == parent)
        {
            return;
        }
        GameObject go = GameObject.Instantiate(sgUtils.LoadPrefab(prefabPath)) as GameObject;
        if (null == go)
        {
            return;
        }
        go.transform.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = new Quaternion();
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}

