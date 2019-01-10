using XLua;
﻿using UnityEngine;
using System.Collections.Generic;
using SG;

[System.Serializable]
public enum CLIPSHAPE_TYPE{
	CST_SQUARE = 0,
	CST_SECTOR,
};

[Hotfix]
public class DecalBuilder {

	private static List<Vector3> bufVertices = new List<Vector3>();
	private static List<Vector3> bufNormals = new List<Vector3>();
	private static List<Vector2> bufTexCoords = new List<Vector2>();
	private static List<int> bufIndices = new List<int>();


	public static void BuildDecalForObject(Decal decal, CLIPSHAPE_TYPE eShapeType, float fAngle, GameObject affectedObject) {
		Mesh affectedMesh = null;
		MeshCollider mc = affectedObject.GetComponent<MeshCollider>();
		if (mc != null)
		{
			affectedMesh = mc.sharedMesh;
		}
		else
		{
			MeshFilter mf = affectedObject.GetComponent<MeshFilter>();
			if (mf != null)
			{
				affectedMesh = mf.sharedMesh;
			}
		}

	
		if(affectedMesh == null || affectedMesh.isReadable == false)
		{
			return;
		}

		float maxSlopeAngle = decal.maxSlopeAngle;

		Vector3[] vertices = affectedMesh.vertices;
		int[] triangles = affectedMesh.triangles;
		int startVertexCount = bufVertices.Count;

		Matrix4x4 matrix = decal.transform.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

		for(int i=0; i<triangles.Length; i+=3) {
			int i1 = triangles[i];
			int i2 = triangles[i+1];
			int i3 = triangles[i+2];
			
			Vector3 v1 = matrix.MultiplyPoint( vertices[i1] );
			Vector3 v2 = matrix.MultiplyPoint( vertices[i2] );
			Vector3 v3 = matrix.MultiplyPoint( vertices[i3] );

			Vector3 side1 = v2 - v1;
			Vector3 side2 = v3 - v1;
			Vector3 normal = Vector3.Cross(side1, side2).normalized;

			if( Vector3.Angle(Vector3.up, normal) >= maxSlopeAngle ) continue;

			if (eShapeType == CLIPSHAPE_TYPE.CST_SQUARE)
			{
				DecalPolygon poly = ClipPolygonBySquare(v1, v2, v3);
				if(poly != null) AddPolygon( poly, normal );
			}
			else if (eShapeType == CLIPSHAPE_TYPE.CST_SECTOR)
			{
				if (fAngle <= 0f && fAngle >= 360f)
				{
					LogMgr.UnityError("Sector angle beyond Range! Must limit it greater than 0 degree and less than 360 degree.");
					continue;
				}

				if (fAngle <= 180f)
				{
					DecalPolygon poly = ClipPolygonByConvexSector(fAngle, v1, v2, v3);
					if(poly != null) AddPolygon( poly, normal );
				}
				else
				{
					DecalPolygon poly = ClipPolygonByNonConvexSector(fAngle, v1, v2, v3, false);
					if(poly != null) AddPolygon( poly, normal );
					
					DecalPolygon polySec = ClipPolygonByNonConvexSector(fAngle, v1, v2, v3, true);
					if(polySec != null) AddPolygon( polySec, normal );
				}
			}
		}

		//GenerateTexCoords(startVertexCount, decal.sprite);
		GenerateTexCoords(startVertexCount, decal);
	}

	private static DecalPolygon ClipPolygonBySquare(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		DecalPolygon poly = new DecalPolygon( v1, v2, v3 );
		
		Plane top = new Plane( Vector3.up, Vector3.up/2f );
		Plane bottom = new Plane( -Vector3.up, -Vector3.up/2f );
		
		poly = DecalPolygon.ClipPolygon(poly, top);
		if(poly == null) return null;
		poly = DecalPolygon.ClipPolygon(poly, bottom);
		if(poly == null) return null;
		
		Plane right = new Plane( Vector3.right, Vector3.right/2f );
		Plane left = new Plane( -Vector3.right, -Vector3.right/2f );
		
		Plane front = new Plane( Vector3.forward, Vector3.forward/2f );
		Plane back = new Plane( -Vector3.forward, -Vector3.forward/2f );
		
		poly = DecalPolygon.ClipPolygon(poly, right);
		if(poly == null) return null;
		poly = DecalPolygon.ClipPolygon(poly, left);
		if(poly == null) return null;
		
		poly = DecalPolygon.ClipPolygon(poly, front);
		if(poly == null) return null;
		poly = DecalPolygon.ClipPolygon(poly, back);
		if(poly == null) return null;

		return poly;
	}

	private static DecalPolygon ClipPolygonByConvexSector(float fAngle, Vector3 v1, Vector3 v2, Vector3 v3)
	{
		if (fAngle <= 0f && fAngle > 180f)
		{
			LogMgr.UnityError("Convex sector angle beyond Range! Must limit it greater than 0 degree and less than or equal to 180 degree.");
			return null;
		}

		DecalPolygon poly = ClipPolygonBySquare(v1, v2, v3);
		if(poly == null) return null;

		Plane start = new Plane( new Vector3(-Mathf.Sin(fAngle * Mathf.Deg2Rad / 2.0f), 0, Mathf.Cos(fAngle * Mathf.Deg2Rad / 2.0f)), Vector3.zero );
		Plane end = new Plane( new Vector3(-Mathf.Sin(fAngle * Mathf.Deg2Rad / 2.0f), 0, -Mathf.Cos(fAngle * Mathf.Deg2Rad / 2.0f)), Vector3.zero );

		poly = DecalPolygon.ClipPolygon(poly, start);
		if(poly == null) return null;
		poly = DecalPolygon.ClipPolygon(poly, end);
		if(poly == null) return null;

		return poly;
	}

	private static DecalPolygon ClipPolygonByNonConvexSector(float fAngle, Vector3 v1, Vector3 v2, Vector3 v3, bool bSecondBlock)
	{
		if (fAngle <= 180f && fAngle >= 360f)
		{
			LogMgr.UnityError("Non-convex sector angle beyond Range! Must limit it greater than 180 degree and less than 360 degree.");
			return null;
		}
		
		DecalPolygon poly = ClipPolygonBySquare(v1, v2, v3);
		if(poly == null) return null;

		Plane start = new Plane( new Vector3(-Mathf.Sin(fAngle * Mathf.Deg2Rad / 2.0f), 0, Mathf.Cos(fAngle * Mathf.Deg2Rad / 2.0f)), Vector3.zero );
		Plane end = new Plane( new Vector3(-Mathf.Sin(fAngle * Mathf.Deg2Rad / 2.0f), 0, -Mathf.Cos(fAngle * Mathf.Deg2Rad / 2.0f)), Vector3.zero );

		if (bSecondBlock == false)
		{
			Plane startInverse = new Plane( -start.normal, Vector3.zero );
			poly = DecalPolygon.ClipPolygon(poly, startInverse);
			if (poly == null) return null;
			poly = DecalPolygon.ClipPolygon(poly, end);
			if(poly == null) return null;
		}
		else
		{
			poly = DecalPolygon.ClipPolygon(poly, start);
			if(poly == null) return null;
		}

		return poly;
	}


	private static void AddPolygon(DecalPolygon poly, Vector3 normal) {
		int ind1 = AddVertex( poly.vertices[0], normal );
		for(int i=1; i<poly.vertices.Count-1; i++) {
			int ind2 = AddVertex( poly.vertices[i], normal );
			int ind3 = AddVertex( poly.vertices[i+1], normal );

			bufIndices.Add( ind1 );
			bufIndices.Add( ind2 );
			bufIndices.Add( ind3 );
		}
	}

	private static int AddVertex(Vector3 vertex, Vector3 normal) {
		int index = FindVertex(vertex);
		if(index == -1) {
			bufVertices.Add( vertex );
			bufNormals.Add( normal );
			index = bufVertices.Count-1;
		} else {
			Vector3 t = bufNormals[ index ] + normal;
			bufNormals[ index ] = t.normalized;
		}
		return (int) index;
	}

	private static int FindVertex(Vector3 vertex) {
		for(int i=0; i<bufVertices.Count; i++) {
			if( Vector3.Distance(bufVertices[i], vertex) < 0.01f ) {
				return i;
			}
		}
		return -1;
	}

	private static void GenerateTexCoords(int start, Decal decal) {
		for(int i=start; i<bufVertices.Count; i++) {
			Vector3 vertex = bufVertices[i];
			
			Vector2 uv = new Vector2((vertex.x+0.5f)*decal.tiling.x + decal.offset.x, 
			                         (vertex.z+0.5f)*decal.tiling.y + decal.offset.y);
			
			bufTexCoords.Add( uv );
		}
	}

	private static void GenerateTexCoords(int start, Sprite sprite) {
		Rect rect = sprite.rect;
		rect.x /= sprite.texture.width;
		rect.y /= sprite.texture.height;
		rect.width /= sprite.texture.width;
		rect.height /= sprite.texture.height;
		
		for(int i=start; i<bufVertices.Count; i++) {
			Vector3 vertex = bufVertices[i];
			
			Vector2 uv = new Vector2(vertex.x+0.5f, vertex.z+0.5f);
			uv.x = Mathf.Lerp( rect.xMin, rect.xMax, uv.x );
			uv.y = Mathf.Lerp( rect.yMin, rect.yMax, uv.y );
			
			bufTexCoords.Add( uv );
		}
	}

	public static void Push(float distance) {
		for(int i=0; i<bufVertices.Count; i++) {
			Vector3 normal = bufNormals[i];
			bufVertices[i] += normal * distance;
		}
	}


	public static Mesh CreateMesh() {
		if(bufIndices.Count == 0) {
			return null;
		}
		Mesh mesh = new Mesh();

		mesh.vertices = bufVertices.ToArray();
		mesh.normals = bufNormals.ToArray();
		mesh.uv = bufTexCoords.ToArray();
		mesh.uv2 = bufTexCoords.ToArray();
		mesh.triangles = bufIndices.ToArray();

		bufVertices.Clear();
		bufNormals.Clear();
		bufTexCoords.Clear();
		bufIndices.Clear();

		return mesh;
	}

}

