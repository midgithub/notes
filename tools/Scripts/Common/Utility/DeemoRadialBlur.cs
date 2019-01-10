using XLua;
using UnityEngine;

[ExecuteInEditMode]
[Hotfix]
public class DeemoRadialBlur : DeemoImageEffectBase
{
    //[HideInInspector]
    public int iteration = 1;
    //[HideInInspector]
    public float blurStrength = 2.2f;
    //[HideInInspector]
    public float blurWidth = 1.0f;
    //[HideInInspector]
    public Vector2 center = new Vector2(0.5f, 0.5f);

    protected override void Start()
    {
        if (shader == null)
        {
            Debug.LogError("shader missing!", this);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        material.SetFloat("_BlurStrength", blurStrength);
        material.SetFloat("_BlurWidth", blurWidth);
        material.SetVector("_Center", center);
        for(int i = 0; i < iteration - 1; i++)
        {
            Graphics.Blit(source, source, material);
        }
        Graphics.Blit(source, dest, material);
    }
}

