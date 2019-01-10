using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class AfterImage : MonoBehaviour
{
    // 残影保留的帧数
    private int frameCount = 3;
    public Shader transparentShader = Shader.Find("Transparent/Diffuse");

    // 残影对象
[Hotfix]
    private class ImageNode
    {
        public GameObject go;
        public Mesh mesh;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public int tick;
    };
    // 残影对象数组
    ImageNode[] imageNodes = null;
    // 内部残影对象数组的大小
    int currentArraySize = 0;
    SkinnedMeshRenderer skinnedMeshRenderer = null;
    // Use this for initialization
    void Start()
    {        
        skinnedMeshRenderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {               
        if (null == skinnedMeshRenderer)
        {                   
            return;
        }

        if (frameCount != currentArraySize)
        {
            ReCreateImageNodes();
        }
        if (null == imageNodes)
        {
            return;
        }

        for(int i=0; i<currentArraySize; ++i)
        {
            ImageNode node = imageNodes[i];
            ++node.tick;
            if(node.tick<0)
            {
                continue;
            }
            node.tick = node.tick % currentArraySize;
            if(node.tick==0)
            {
                skinnedMeshRenderer.BakeMesh(node.mesh);
                node.go.SetActive(true);
                //node.go.transform.position = this.transform.position;
                //node.go.transform.rotation = this.transform.rotation;
            }
            Material[] mats = node.meshRenderer.materials;
            for (int j = 0; j < mats.Length; ++j)
            {
                Color clr = mats[j].GetColor("_Color");
                clr.a = (float)(currentArraySize - node.tick) / (float)(currentArraySize + 1);
                mats[j].SetColor("_Color", clr);
            }
        }
    }
    void ReCreateImageNodes()
    {
        for (int i = 0; i < currentArraySize; ++i)
        {
            Object.Destroy(imageNodes[i].go);
        }
        imageNodes = null;
        if (frameCount > 0)
        {
            imageNodes = new ImageNode[frameCount];
            currentArraySize = frameCount;
        }
        if (null == imageNodes)
        {
            return;
        }

        const string ROOTNODENAME = "__imagenodes";
        Transform roottrans = transform.Find(ROOTNODENAME);
        if (roottrans)
        {
            Object.Destroy(roottrans.gameObject);
        }
        GameObject rootgo = new GameObject(ROOTNODENAME);
        roottrans = rootgo.transform;
        roottrans.parent = this.transform;
        
        roottrans.transform.localPosition = skinnedMeshRenderer.transform.localPosition;
        roottrans.transform.localRotation = skinnedMeshRenderer.transform.localRotation;

        for (int i = 0; i < currentArraySize; ++i)
        {
            ImageNode node = new ImageNode();
            imageNodes[i] = node;
            node.go = new GameObject(i.ToString());
            node.go.transform.parent = roottrans;
            node.go.transform.localPosition = Vector3.zero;
            node.go.transform.localRotation = Quaternion.Euler(Vector3.zero);                                    

            node.mesh = new Mesh();
            node.meshFilter = node.go.AddComponent<MeshFilter>();
            node.meshFilter.mesh = node.mesh;
            node.meshRenderer = node.go.AddComponent<MeshRenderer>();
            node.meshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
            Material[] mats = node.meshRenderer.materials;
            for (int j = 0; j < mats.Length; ++j)
            {
                mats[j].shader = transparentShader;
            }
            node.tick = -i-1;
            node.go.SetActive(false);
        }
    }
}

