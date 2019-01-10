using XLua;
using UnityEngine;

 

[RequireComponent (typeof(Camera))]
[AddComponentMenu("Image Effects/Other/Distort")]
[Hotfix]
public class DistortEffect : MonoBehaviour
{
    [HideInInspector]
	public RenderTexture cameraRenderTexture;
    [HideInInspector]
    public RenderTexture distortRenderTexture; 
	private GameObject shaderCamera;

    public Shader compositeShader;
    Material m_CompositeMaterial = null;
    [HideInInspector]
    public Material compositeMaterial
    {
		get {
			if (m_CompositeMaterial == null) {
                m_CompositeMaterial = new Material(compositeShader);
				m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_CompositeMaterial;
		} 
	}
	

	protected void OnDisable()
	{
		if( m_CompositeMaterial ) {
			DestroyImmediate( m_CompositeMaterial );
		}
		DestroyImmediate (shaderCamera);
	}
	
	protected void Start()
	{
        if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }


        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
        if (!compositeMaterial.shader.isSupported)
            enabled = false;
        cameraRenderTexture = new RenderTexture((int)GetComponent<Camera>().pixelWidth, (int)GetComponent<Camera>().pixelHeight, 16);
        distortRenderTexture = new RenderTexture((int)GetComponent<Camera>().pixelWidth / 4, (int)GetComponent<Camera>().pixelHeight / 4, 16);
        compositeMaterial.SetTexture("_DistortTex", distortRenderTexture);
        GetComponent<Camera>().targetTexture = cameraRenderTexture;
	}

	void OnPreRender()
	{
        if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }
		
		if(shaderCamera == null)
		{
			shaderCamera = new GameObject("ShaderCamera", typeof (Camera));
			shaderCamera.GetComponent<Camera>().enabled = false;
			shaderCamera.hideFlags = HideFlags.HideAndDontSave;
		}
		Camera cam = shaderCamera.GetComponent<Camera>();
		cam.CopyFrom(GetComponent<Camera>());
		cam.backgroundColor = new Color(0f,0f,0f,0f);
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.cullingMask = 1 << LayerMask.NameToLayer ("Distort");
        cam.targetTexture = distortRenderTexture;
		cam.Render ();
        
	}
    //void OnPostRender()
    //{
    //    Graphics.Blit(distortRenderTexture, (RenderTexture)null);
    //}

}

