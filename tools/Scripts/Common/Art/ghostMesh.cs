using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class ghostMesh : MonoBehaviour {

	public SkinnedMeshRenderer[] characterMesh;
    public float delayTime = 0f;
    public float intervalTime = 0.05f;
    public float ghostLife = 0.2f;
    public float stopTime = 1f;
    public Material ghostMat;
    public Gradient alphaKey;
    private float m_fCurrentTime;

//    void Awake()
//    {
//        characterMesh = GetComponentsInChildren<SkinnedMeshRenderer>();
//    }

    // Use this for initialization
    void Start () {
        m_fCurrentTime = 0;
        CancelInvoke("bakeMesh");
        InvokeRepeating("bakeMesh", delayTime, intervalTime);

    }

    public void OnEnable()
    {
        for (int i = 0; i < characterMesh.Length; i++)
        {
            characterMesh[i] = null;
        }
        CancelInvoke("bakeMesh");
        CancelInvoke("Start");
        Invoke("Start", 0.000001f);
    }

    public void OnDisable()
    {
        for (int i = 0; i < characterMesh.Length; i++)
        {
            characterMesh[i] = null;
        }
        CancelInvoke("bakeMesh");
        CancelInvoke("Start");
    }
	
	// Update is called once per frame
	void Update () {
        if (m_fCurrentTime > stopTime)
            CancelInvoke("bakeMesh");
        m_fCurrentTime += Time.deltaTime;
    }

    void bakeMesh()
    {
//        if (characterMesh.Length != null)
            
	    for (int i = 0; i < characterMesh.Length; i++)
	    {
			if(characterMesh[i] != null)
			{
		        Mesh newMesh = new Mesh();
		        newMesh.name = "cloneMesh";
		        characterMesh[i].BakeMesh(newMesh);
		        GameObject newObj = new GameObject();
		        newObj.transform.position = transform.position;
		        newObj.transform.eulerAngles = new Vector3(characterMesh[i].transform.eulerAngles.x, transform.eulerAngles.y, 0);
		        newObj.name = "cloneMesh";
		        newObj.AddComponent<MeshFilter>().mesh = newMesh;
		        newObj.AddComponent<MeshRenderer>();
		        Material[] cloneMat;
		        cloneMat = characterMesh[i].materials;
		        for(int m = 0; m < cloneMat.Length; m++)
		        {
		            cloneMat[m] = ghostMat;
		        }
		        newObj.GetComponent<MeshRenderer>().materials = cloneMat;
		        destroyClone dc = newObj.AddComponent<destroyClone>();
		        
		        dc.destroyTime = ghostLife;
		        dc.getKey = alphaKey;
		        dc.lifeTime = ghostLife;
			}
	    }
    }
}

