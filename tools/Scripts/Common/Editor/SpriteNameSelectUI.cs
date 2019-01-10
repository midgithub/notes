using XLua;
﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[Hotfix]
public class SpriteNameSelectUI : EditorWindow{
    public delegate void SelectSpriteDelegate(int index, string name);
    SelectSpriteDelegate SelectSpriteCallBack;

    UIAtlas mAtlas;
    int SetIndex;

    public static void ShowSelectUI(UIAtlas atlas,int index,SelectSpriteDelegate callBack)
    {
        var instance = GetWindow<SpriteNameSelectUI>();
        instance.mAtlas = atlas;
        instance.SetIndex = index;
        instance.SelectSpriteCallBack = callBack;
    }


    Vector2 ScrollViewPos = Vector2.zero;
    void OnGUI()
    {
        ScrollViewPos = GUILayout.BeginScrollView(ScrollViewPos);
        for (int i = 0; i < mAtlas.spriteList.Count; i++)
        {
            //int xNum = i % 5;
            //int yNum = i / 5;
            //if (GUI.Button(new Rect(xNum * 150, yNum * 50, 120, 30), mAtlas.spriteList[i].name))
            if (GUILayout.Button(mAtlas.spriteList[i].name))
            {
                Close();
                SelectSpriteCallBack(SetIndex,mAtlas.spriteList[i].name);
            }
        }
        GUILayout.EndScrollView();
    }

}

