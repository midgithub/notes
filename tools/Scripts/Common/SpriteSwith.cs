using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[Hotfix]
public class SpriteSwith : MonoBehaviour {

    public UIAtlas mTargetAtlas;
    public UISprite mTargetSprite;
    public List<string> mSelectSpriteNameGroup = new List<string>();

    void Awake()
    {
        if (mTargetAtlas == null)
        {
            mTargetSprite=GetComponent<UISprite>();
            mTargetAtlas = mTargetSprite.atlas;
        }
    }

    public void SetSpriteByIndex(int index)
    {
        if (index < 0 || index >= mSelectSpriteNameGroup.Count)
        {
            SG.LogMgr.LogError("图片索引越界:{0}", index.ToString());
        }
        else
        {
            mTargetSprite.spriteName = mSelectSpriteNameGroup[index];
        }
    }
    

}

