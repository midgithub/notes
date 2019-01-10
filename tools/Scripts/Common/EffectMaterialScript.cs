
// by artsyli
// 2014.4.22

#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;
using SG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EffectMaterialScript : MonoBehaviour 
{
	public enum BlendMode
	{
		AlphaBlend,
		Additive
	}

	public enum LayerMode
	{
		Front,
		Middle,
		Back
	}

	public enum SeqType
	{
		TimeSequence,
		TimeRandom,
		FixedFrame
	}

	private const float EPSILON = 0.00001f;
	
	public Shader AlphaBlendShader; // Shader.Find("Effect_Mid/Alpha Blend")
	public Shader AdditiveShader;   // Shader.Find("Effect_Mid/Additive")

	// DO NOT USE 'private' !!!
	public Material material = null;

	public BlendMode blend = BlendMode.AlphaBlend;
	public LayerMode layer = LayerMode.Middle;
	public SeqType seqType = SeqType.TimeSequence;

	public bool savematfile = true;

	public Color color = Color.white;
	public float brightness = 1.0f;
	public Texture mainTex = null;
	public float uoffset = 0.0f;
	public float voffset = 0.0f;
	public float uscale = 1.0f;
	public float vscale = 1.0f;
	public float uvrotate = 0.0f;

	public Texture maskTex = null;
	public float maskuoffset = 0.0f;
	public float maskvoffset = 0.0f;
	public float maskuscale = 1.0f;
	public float maskvscale = 1.0f;
	public float maskuvrotate = 0.0f;

	public int tileCol = 1;
	public int tileRow = 1;
	public float seqValue = 0.0f;

	public float delay = 0.0f;
	public bool once = false;

	private bool bDelayStart = false;
	private bool bDelayEnd = false;
	private float begintime = 0.0f;

	private float startTime;
	private int tile;
#if UNITY_EDITOR
	static private string FXMaterialPath = "Assets/Resources/AutoMaterial/"; 
#endif

	private string EncodePropertyLess()
	{
		int texsize = 256;
		int size = texsize * 2 + 1;
		byte[] data = new byte[size];
		for(int i = 0; i < size; i++)
			data[i] = 0;

		if(mainTex != null)
		{
			System.Text.Encoding.UTF8.GetBytes(mainTex.name).CopyTo(data, 0);
		}
		if(maskTex != null)
		{
			System.Text.Encoding.UTF8.GetBytes(maskTex.name).CopyTo(data, 256);
		}
		data[512] = (byte)blend;

		System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] _ret = md5.ComputeHash(data);
		return System.BitConverter.ToString(_ret).Replace("-", "");
	}

	//private string EncodeProperty()
	//{
	//	int texsize = 256 + 5 * 4;
	//	int size = texsize * 2 + 3 + 2 + 24;
	//	byte[] data = new byte[size];
	//	for(int i = 0; i < size; i++)
	//		data[i] = 0;

	//	int iBufIndex = 0;
	//	if(mainTex != null)
	//	{
	//		System.Text.Encoding.UTF8.GetBytes(mainTex.name).CopyTo(data, iBufIndex);
	//		iBufIndex += 256;
	//		System.BitConverter.GetBytes(uoffset).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(voffset).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(uscale).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(vscale).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(uvrotate).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//	}

	//	iBufIndex = texsize;
	//	if(maskTex != null)
	//	{
	//		System.Text.Encoding.UTF8.GetBytes(maskTex.name).CopyTo(data, iBufIndex);
	//		iBufIndex += 256;
	//		System.BitConverter.GetBytes(maskuoffset).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(maskvoffset).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(maskuscale).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(maskvscale).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//		System.BitConverter.GetBytes(maskuvrotate).CopyTo(data, iBufIndex);
	//		iBufIndex += 4;
	//	}

	//	iBufIndex = 2 * texsize;
	//	data[iBufIndex++] = (byte)blend;
	//	data[iBufIndex++] = (byte)layer;
	//	data[iBufIndex++] = (byte)seqType;
	//	data[iBufIndex++] = (byte)tileCol;
	//	data[iBufIndex++] = (byte)tileRow;

	//	System.BitConverter.GetBytes(color.a).CopyTo(data, iBufIndex);
	//	iBufIndex += 4;
	//	System.BitConverter.GetBytes(color.r).CopyTo(data, iBufIndex);
	//	iBufIndex += 4;
	//	System.BitConverter.GetBytes(color.g).CopyTo(data, iBufIndex);
	//	iBufIndex += 4;
	//	System.BitConverter.GetBytes(color.b).CopyTo(data, iBufIndex);
	//	iBufIndex += 4;

	//	System.BitConverter.GetBytes(brightness).CopyTo(data, iBufIndex);
	//	iBufIndex += 4;
	//	System.BitConverter.GetBytes(seqValue).CopyTo(data, iBufIndex);
	//	iBufIndex += 4;

	//	System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
	//	byte[] _ret = md5.ComputeHash(data);
	//	return System.BitConverter.ToString(_ret).Replace("-", "");
	//}

	public EffectMaterialScript()
	{
	}

	void Awake()
	{
		if(material == null)
		{
			if(blend == BlendMode.AlphaBlend)
				gameObject.GetComponent<Renderer>().material = new Material(AlphaBlendShader); 
			else
				gameObject.GetComponent<Renderer>().material =  new Material(AdditiveShader); 

			material = gameObject.GetComponent<Renderer>().material;
			startTime = Time.realtimeSinceStartup;
			RefreshTexture();
			RefreshParameter();
		}
		else
		{
			material = gameObject.GetComponent<Renderer>().material;
			startTime = Time.realtimeSinceStartup;
			RefreshParameter();
		}
	}

	void Start()
	{
		startTime = Time.realtimeSinceStartup;
	}
	#if UNITY_EDITOR
	public void EditorChange(bool bChangeShader, bool bChangeMode)
	{
		if(material == null)
			return;

		if(bChangeShader || bChangeMode || savematfile)
		{
			if(material.name == AlphaBlendShader.name || material.name == AdditiveShader.name)
			{
				DestroyObject(material);
				material = null;
			}

			CreateMaterial();
		}

		startTime = Time.realtimeSinceStartup;
		RefreshTexture();
		RefreshParameter();

		bDelayStart = false;
		bDelayEnd = false;
		begintime = 0.0f;
	}

	void CreateMaterial()
	{
		if(savematfile)
		{
			string md5name = EncodePropertyLess();
			string matPath = ("AutoMaterial/" + md5name);

            Material newMat = CoreEntry.gResLoader.LoadMaterial(matPath); //Bundle.AssetBundleLoadManager.Instance.Load(matPath, typeof(Material)) as Material;
			if(newMat == null)
			{
				if(blend == BlendMode.AlphaBlend)
					newMat = new Material(AlphaBlendShader); 
				else
					newMat = new Material(AdditiveShader);
				
				md5name = EncodePropertyLess();
				matPath = (FXMaterialPath + md5name + ".mat");
				AssetDatabase.CreateAsset(newMat, matPath);
			}
			
			gameObject.GetComponent<Renderer>().sharedMaterial = newMat;
		}
		else
		{
			if(blend == BlendMode.AlphaBlend)
				gameObject.GetComponent<Renderer>().sharedMaterial = new Material(AlphaBlendShader); 
			else
				gameObject.GetComponent<Renderer>().sharedMaterial = new Material(AdditiveShader); 
		}
		
		material = gameObject.GetComponent<Renderer>().sharedMaterial;
	}
	
	public void EditorEnable()
	{
		if(material == null)
		{
			CreateMaterial();
		}

		startTime = Time.realtimeSinceStartup;
		RefreshTexture();
		RefreshParameter();

		bDelayStart = false;
		bDelayEnd = false;
		begintime = 0.0f;
	}
	#endif
	public void EditorDisable()
	{
	}

	public void ForceUpdate()
	{
		Update();
	}

	void OnValidate()
	{
	}

	private void RefreshTexture()
	{
		material.SetTexture("_MainTex", mainTex);
		if(maskTex != null)
			material.SetTexture("_MaskTex", maskTex);
	}

	private void RefreshParameter()
	{
		var keywords = new List<string> { maskTex != null ? "MASK_ON" : "MASK_OFF"};
		material.shaderKeywords = keywords.ToArray ();

		material.color = color;

		Vector4 vec;
		vec.x = uoffset;
		vec.y = voffset;
		vec.z = uscale;
		vec.w = vscale;
		material.SetVector("_UVParam", vec);
		material.SetFloat("_Rotate", uvrotate);

		vec.x = maskuoffset;
		vec.y = maskvoffset;
		vec.z = maskuscale;
		vec.w = maskvscale;
		material.SetVector("_MaskUVParam", vec);
		material.SetFloat("_MaskRotate", maskuvrotate);
		material.SetFloat("_Brightness", brightness);

		switch(layer)
		{
		case LayerMode.Front: 	material.renderQueue = 2999; break;
		case LayerMode.Middle:	material.renderQueue = 3000; break;
		case LayerMode.Back:	material.renderQueue = 3001; break;
		}
	}
	
	void OnDestroy()
	{
		if(material != null)
		{
			DestroyObject(material);
			material = null;
		}
	}
	
	void Update ()
	{
		if(material != null)
		{
			bool bEnable = true;
			if(delay > 0.0f && !bDelayStart)
			{
				if(begintime > delay)
				{
					bDelayStart = true;
					startTime = Time.realtimeSinceStartup;
				}
				else
				{
					begintime =  Time.realtimeSinceStartup - startTime;
					bEnable = false;
				}
			}

			if(bEnable && !bDelayEnd)
			{
				Vector4 vec;
				if(tileCol == 1 && tileRow == 1)
				{
					vec.x = vec.y = 0.0f;
					vec.z = vec.w = 1.0f;
					tile = 0;
				}
				else
				{
					int totalTile = tileCol * tileRow;
					
					if(seqType == SeqType.TimeSequence || seqType == SeqType.TimeRandom)
					{
						if(seqValue <= 0.0f)
							seqValue = EPSILON;
						
						if(Time.realtimeSinceStartup - startTime >= seqValue)
						{
							if(seqType == SeqType.TimeSequence)
							{
								tile++;

								if(once && tile >= totalTile)
									bDelayEnd = true;
							}
							else
							{
								tile = (int)(Random.Range(0.0f, 1.0f) * totalTile);
							}
							tile = tile % totalTile;
							startTime = Time.realtimeSinceStartup;
						}
					}
					else
					{
						tile = (int)seqValue;
						tile = tile % totalTile;
					}
					
					int iRow = tile / tileCol;
					
					// Fixed V direction
					iRow = (tileRow - 1) - iRow;
					
					int iCol = tile % tileCol;
					
					vec.x = iCol / (float)tileCol;
					vec.y = iRow / (float)tileRow;
					vec.z = 1.0f / tileCol;
					vec.w = 1.0f / tileRow;
				}
				material.SetVector("_UVTile", vec);
			}

			Color src = material.color;
			
			float alphaResult = 1;
			
			Color clr = color;



			if (bDelayEnd || !bEnable)
			{
				alphaChanged = true;
				clr.a = 0.0f;
				alphaResult = 0;
				src.a = alphaResult;
				material.color = src;
			}
			else
			{
				if (alphaChanged)//只改变一次//
				{
					src.a = clr.a;
					alphaResult = clr.a;
					src.a = alphaResult;
					material.color = src;
					alphaChanged = false;
				}
			}

//			material.color = clr;
		}
	}
	bool alphaChanged = true;
}
