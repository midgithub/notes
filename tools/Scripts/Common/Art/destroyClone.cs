using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class destroyClone : MonoBehaviour {
    public float destroyTime = 1f;
    Mesh cloneMesh;
    Vector3[] vertices;
    Color[] colors;
    Color rgba;

    public Gradient getKey;
    Gradient setKey = new Gradient();
    public float tTime;
    float getTime;
    public float lifeTime;
    void Start () {
        
        Destroy(this.gameObject, destroyTime);
        cloneMesh = GetComponent<MeshFilter>().mesh;
        vertices = cloneMesh.vertices;
        colors = new Color[vertices.Length];
        rgba = new Color();
        getTime = Time.time;
        setKey = getKey;
        Update();
    }

    void Update() {

        //rgba.a -= 1 / destroyTime * Time.deltaTime * 2;
        tTime = (Time.time - getTime) / lifeTime;
        rgba = setKey.Evaluate(tTime);

        int i = 0;
        while (i < vertices.Length)
        {
           colors[i] = rgba;
           i++;
        }
        cloneMesh.colors = colors;

    }
}

