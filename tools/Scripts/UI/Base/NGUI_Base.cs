using XLua;
﻿using UnityEngine;
using System.Collections;
using SG;

[Hotfix]
public class NGUI_Base : Common_Base{

    public void SetLabelText(UILabel label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }

    public void SetSpriteName(UISprite sprite, string spriteName)
    {
        if (sprite != null)
        {

            sprite.spriteName = spriteName;
        }
    }

    public void SetUITexture(UITexture tex, string path)
    {
        if (tex != null)
        {
            Texture2D texture = CoreEntry.gResLoader.Load(path, typeof(Texture2D)) as Texture2D;
            if (texture != null)
            {
                tex.mainTexture = texture;
            }
        }
    }

    /// <summary>
    /// 创建灰度图
    /// </summary>
    /// <param name="prefabPath"></param>
    /// <returns></returns>
    public UIAtlas CreateGreyAtlas(string prefabPath)
    {
        //动态组装灰色图集
        GameObject itemPre = (GameObject)CoreEntry.gResLoader.Load(prefabPath);
        if(itemPre == null)return null;
        UISprite itemSprite = itemPre.GetComponent<UISprite>();

        return CreateGreyAtlas(itemSprite);
    }

    public UIAtlas CreateGreyAtlas(UISprite itemSprite)
    {
        if (itemSprite != null)
        {
            UIAtlas atlasGrey = Instantiate(itemSprite.atlas) as UIAtlas;//复制图集
            Material greyMaterial = new Material(Shader.Find("Unlit/Transparent Colored Grey"));//创建灰色材质
            greyMaterial.mainTexture = itemSprite.atlas.spriteMaterial.mainTexture;//给灰色材质一张纹理
            atlasGrey.spriteMaterial = greyMaterial;//给克隆的灰色图集赋予灰色材质

            return atlasGrey;
        }
        return null;
    }

    public void DeleteGreyAtlas(UIAtlas v)
    {
        if (v != null)
        {
            Destroy(v.gameObject);
        }
    }


   
}

