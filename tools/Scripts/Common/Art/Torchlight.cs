using XLua;
﻿using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
[Hotfix]
public class Torchlight : MonoBehaviour {
	
	private Matrix4x4 VP;
	private Matrix4x4 V;
	private Matrix4x4 P;
//	private Camera cam;
	public RenderTexture lightRT;

	private Vector4 r0 = new Vector4(0.05000f,	0.00000f,	0.00000f,	0.00000f);
	private Vector4 r1 = new Vector4(0.00000f,	0.050000f,	0.00000f,	0.00000f);
	private Vector4 r2 = new Vector4(0.00000f,	0.00000f,	-0.01005f,	-1.01005f);
	private Vector4 r3 = new Vector4(0.00000f,	0.00000f,	0.00000f,	1.00000f);
	bool d3d;
	
	// Use this for initialization
	void Start () {
		d3d = SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D") > -1;
		P.SetRow (0, r0);
		P.SetRow (1, r1);
		P.SetRow (2, r2);
		P.SetRow (3, r3);
		if (d3d) {
			// Invert Y for rendering to a render texture
			for (int i = 0; i < 4; i++) {
				P[1,i] = -P[1,i];
			}
			// Scale and bias from OpenGL -> D3D depth range
			for (int i = 0; i < 4; i++) {
				P[2,i] = P[2,i]*0.5f + P[3,i]*0.5f;
			}
		}
		lightRT = new RenderTexture (512, 512, 16);
		GetComponent<Camera>().targetTexture = lightRT;
        GetComponent<Camera>().projectionMatrix = P;
		//LogMgr.UnityLog (camera.projectionMatrix);
        Shader.EnableKeyword("_TORCHLIGHT_ON");

	}
	// Update is called once per frame
	void Update () {

		V = GetComponent<Camera>().worldToCameraMatrix;
//		P = camera.projectionMatrix;

		VP = P*V;
		Shader.SetGlobalMatrix ("TopCameraVP", VP);
		Shader.SetGlobalTexture ("projTex", lightRT);

		//		LogMgr.UnityLog (P);
	}
}

