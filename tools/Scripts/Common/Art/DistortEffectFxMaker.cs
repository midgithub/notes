using XLua;
using UnityEngine;

// Glow uses the alpha channel as a source of "extra brightness".
// All builtin Unity shaders output baseTexture.alpha * color.alpha, plus
// specularHighlight * specColor.alpha into that.
// Usually you'd want either to make base textures to have zero alpha; or
// set the color to have zero alpha (by default alpha is 0.5).
 
[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
//[AddComponentMenu("Image Effects/Other/DistortFxMaker")]
[Hotfix]
public class DistortEffectFxMaker : MonoBehaviour
{

	public RenderTexture cameraRenderTexture; 
	private GameObject shaderCamera;

    public Shader compositeShader;
    Material m_CompositeMaterial = null;
	protected Material compositeMaterial {
		get {
			if (m_CompositeMaterial == null) {
                m_CompositeMaterial = new Material(compositeShader);
				m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_CompositeMaterial;
		} 
	}
	

//    public Shader distortShader;
//    Material m_DistortMaterial = null;
//	protected Material distortMaterial {
//		get {
//			if (m_DistortMaterial == null) {
//				m_DistortMaterial = new Material(distortShader);
//				m_DistortMaterial.hideFlags = HideFlags.HideAndDontSave;
//			}
//			return m_DistortMaterial;
//		} 
//	}
	





	protected void OnDisable()
	{
		if( m_CompositeMaterial ) {
			DestroyImmediate( m_CompositeMaterial );
		}
//		if( m_DistortMaterial ) {
//			DestroyImmediate( m_DistortMaterial );
//		}
		DestroyImmediate (shaderCamera);
	}
	
	protected void Start()
	{
        if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }

		// Disable if we don't support image effects
		if (!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
		if( !compositeMaterial.shader.isSupported )
			enabled = false;
//		if( !distortMaterial.shader.isSupported )
//			enabled = false;
	}

	void OnPreRender()
	{
        if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }

		if(cameraRenderTexture !=null)
		{
			RenderTexture.ReleaseTemporary(cameraRenderTexture);
			cameraRenderTexture = null;
		}
		cameraRenderTexture = RenderTexture.GetTemporary((int)GetComponent<Camera>().pixelWidth,(int)GetComponent<Camera>().pixelHeight,16);
		compositeMaterial.SetTexture ("_DistortTex", cameraRenderTexture);
		
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
		cam.targetTexture = cameraRenderTexture;
//		cam.RenderWithShader (distortShader, "RenderType");
		cam.Render ();
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
        if (!SG.CoreEntry.m_bUseDistort)
        {
            return;
        }

		// Clamp parameters to sane values
//		glowIntensity = Mathf.Clamp( glowIntensity, 0.0f, 10.0f );
//		blurIterations = Mathf.Clamp( blurIterations, 0, 30 );
//		blurSpread = Mathf.Clamp( blurSpread, 0.5f, 1.0f );
//		
//		int rtW = source.width/4;
//		int rtH = source.height/4;
//		RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
//		
//		// Copy source to the 4x4 smaller texture.
//		DownSample4x (source, buffer);
//		
//		// Blur the small texture
//		float extraBlurBoost = Mathf.Clamp01( (glowIntensity - 1.0f) / 4.0f );
//		blurMaterial.color = new Color( 1F, 1F, 1F, 0.25f + extraBlurBoost );
//		
//		for(int i = 0; i < blurIterations; i++)
//		{
//			RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
//			FourTapCone (buffer, buffer2, i);
//			RenderTexture.ReleaseTemporary(buffer);
//			buffer = buffer2;
//		}
//		Graphics.Blit(source,destination);
//				
//		BlitGlow(buffer, destination);
//		
//		RenderTexture.ReleaseTemporary(buffer);

		Graphics.Blit (source, destination, compositeMaterial);
	}
	
//	public void BlitGlow( RenderTexture source, RenderTexture dest )
//	{
//		compositeMaterial.color = new Color(1F, 1F, 1F, Mathf.Clamp01(glowIntensity));
//		Graphics.Blit (source, dest, compositeMaterial);
//	}	
}

