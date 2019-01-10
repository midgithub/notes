using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;
[ExecuteInEditMode]
[Hotfix]
public class CirleTexture : MonoBehaviour
{ 
    void Awake()
    {
        
    }

    void Start()
    { 
    }
    void OnEnable()
    {
        //SetSpriteName();
    }
    UIAtlas atlas;
    UITexture uiTexture;
    Texture2D atTex;
    public void SetSpriteName()
    {
        uiTexture = this.GetComponent<UITexture>();
        if (uiTexture == null)
            uiTexture = this.gameObject.AddComponent<UITexture>();
        uiTexture.material = new Material(CoreEntry.gResLoader.LoadShader("Self/Circular")); //Resources.Load("UI/Atlas/headIcons/Naruto", typeof(Material)) as Material;
    
        UISprite tempsprite = this.GetComponent<UISprite>();
        if (tempsprite == null)
            return;
        uiTexture.height = tempsprite.height;
        uiTexture.width = tempsprite.width;
        uiTexture.depth = tempsprite.depth;
        atlas = tempsprite.atlas;
        if(atlas!=null)
        atTex = atlas.texture as Texture2D;
     
         
        if (uiTexture != null && atlas != null)
        {
            UISpriteData sprite = atlas.GetSprite(tempsprite.spriteName);
            if(sprite!=null)
            {
                Texture2D outTex = new Texture2D((int)(sprite.width), (int)(sprite.height), TextureFormat.RGBA32, false, true);
                outTex.anisoLevel = 0;
                outTex.wrapMode = TextureWrapMode.Clamp;
                outTex.filterMode = FilterMode.Trilinear;
                try
                {
                    outTex.SetPixels(atTex.GetPixels((int)(sprite.x), (int)(atTex.height - sprite.y - sprite.height), (int)(sprite.width), (int)(sprite.height)));
                }
                catch (System.Exception e)
                {
                    LogMgr.LogError(e.Message);
                }

                outTex.Apply();
                uiTexture.mainTexture = outTex;
            }

        }

        tempsprite.enabled = false;
    }
  
}
