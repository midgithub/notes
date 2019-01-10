/**
* @file     : a
* @brief    : b
* @details  : d
* @author   : a
* @date     : 2014-xx-xx
*/

using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{

	[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[Hotfix]
	public class RectangleZone : MonoBehaviour {

		private Vector3 minPoint = new Vector3(0, 0, 0);
		private Vector3 maxPoint = new Vector3(1, 1, 1);

		private Mesh trigZoneMesh = null;
		private Color rectangleColor = Color.green;
		private Vector3[] vertices;
		private Color[]	colors;
		private int[] triangles;

		private bool bShowState = true;

		public void SetZonePoint(Vector3 minP, Vector3 maxP)
		{
			minPoint = minP;
			maxPoint = maxP;

			UpdateMeshData();
		}

		public void Show()
		{
			SetShowState(true);
		}

		public void Hide()
		{
			SetShowState(false);
		}

		void SetShowState(bool bShow)
		{
			if (bShowState == false && bShow == true)
			{
				GetComponent<MeshRenderer>().enabled = true;
			}
			else if (bShowState == true && bShow == false)
			{
				GetComponent<MeshRenderer>().enabled = false;
			}
			bShowState = bShow;
		}

		public bool IsShown()
		{
			return bShowState;
		}

		// Use this for initialization
		void Start ()
		{
			GetComponent<MeshRenderer>().material = new Material(CoreEntry.gResLoader.LoadShader("VertexColor"));
			CreateMesh();
			UpdateMeshData();
		}

		void OnDestroy ()
		{
			DestoryMesh();
		}
	
		void CreateMesh()
		{
			if (trigZoneMesh == null) 
			{
				GetComponent<MeshFilter> ().mesh = trigZoneMesh = new Mesh ();
				trigZoneMesh.name = "RectangleZoneMesh";
			}
		}
		
		void DestoryMesh()
		{
			if (trigZoneMesh != null) 
			{
				GetComponent<MeshFilter>().mesh = null;
				DestroyImmediate(trigZoneMesh);
			}
		}
		
		void UpdateMeshData()
		{
			if (trigZoneMesh != null)
			{
				vertices = new Vector3[4];
				colors = new Color[4];
				triangles = new int[6];
				
				vertices[0] = minPoint;
				vertices[1] = new Vector3 (minPoint.x, minPoint.y, maxPoint.z);
				vertices[2] = new Vector3 (maxPoint.x, maxPoint.y, minPoint.z);
				vertices[3] = maxPoint;
				trigZoneMesh.vertices = vertices;
				
				triangles [0] = 0;
				triangles [1] = 1;
				triangles [2] = 2;
				triangles [3] = 1;
				triangles [4] = 3;
				triangles [5] = 2;
				trigZoneMesh.triangles = triangles;
				
				for (int i = 0; i < 4; ++i)
				{
					colors[i] = rectangleColor;
				}
				trigZoneMesh.colors = colors;
			}
		}
		
		// Update is called once per frame
		//void Update () 
		//{	
		//}
	}

};//End SG

