using XLua;
﻿using UnityEngine;
using System.Collections;

[Hotfix]
public class DetailObject : MonoBehaviour
{
	private Transform camTransform;
	private Transform[] childrenTransform;
	private bool[] childrenActive;
	private float updateTimer = 0.0f;
	public float cullDistance = 40.0f;

	void Awake()
	{
		Transform thisTransform = transform;
        if(null != SG.CoreEntry.gCameraMgr.MainCamera)
            camTransform = SG.CoreEntry.gCameraMgr.MainCamera.transform;
		int childCount = thisTransform.childCount;
		childrenTransform = new Transform[childCount];
		childrenActive = new bool[childCount];

		for( int i=0; i<childCount; ++i )
		{
			childrenTransform[i] = thisTransform.GetChild(i);
			childrenActive[i] = true;
		}
	}

	void Start()
	{  
	}
	
	void Update()
	{
		updateTimer += Time.deltaTime;
		if( updateTimer < 0.3f )
			return;

		updateTimer = 0.0f;
		float sqrDistance = cullDistance * cullDistance;

		for( int i=0; i<childrenTransform.Length; ++i )
		{
			if( childrenTransform[i] == null )
				continue;

			if( (childrenTransform[i].position - camTransform.position).sqrMagnitude <= sqrDistance )
			{
				if( !childrenActive[i] )
				{
					childrenTransform[i].gameObject.SetActive(true);
					childrenActive[i] = true;
				}
			}
			else
			{
				if( childrenActive[i] )
				{
					childrenTransform[i].gameObject.SetActive(false);
					childrenActive[i] = false;
				}
			}
		}
	}
}

