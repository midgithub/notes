using XLua;
﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpriteSwith), true)]
[Hotfix]
public class SpriteNameSelectInspector : Editor
{
    SpriteSwith mTarget;
    List<string> mSpriteNameGroup;
    //string[] mSpriteNameArray;
    //int SelectIndex = 0; 
    public override void OnInspectorGUI()
    {
        if (mTarget == null)
        {
            mTarget = target as SpriteSwith;
            List<string> spriteNameGroup  = new List<string>();
            mTarget.mTargetAtlas.spriteList.ApplyAllItem(C => spriteNameGroup.Add(C.name));
            mSpriteNameGroup = spriteNameGroup;
            //mSpriteNameArray = spriteNameGroup.ToArray();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            mTarget.mSelectSpriteNameGroup.Add(mSpriteNameGroup[0]);
        }
        if (GUILayout.Button("Remove"))
        {
            if (mTarget.mSelectSpriteNameGroup.Count > 0)
                mTarget.mSelectSpriteNameGroup.RemoveAt(mTarget.mSelectSpriteNameGroup.Count-1);
        }
        GUILayout.EndHorizontal();
        for (int i = 0; i < mTarget.mSelectSpriteNameGroup.Count; i++)
        {
            //string btnName = i.ToString() + mTarget.mSelectSpriteNameGroup[i];
            //int selectIndex = mSpriteNameGroup.IndexOf(mTarget.mSelectSpriteNameGroup[i]);
            if (GUILayout.Button(mTarget.mSelectSpriteNameGroup[i]))
            {
                SpriteNameSelectUI.ShowSelectUI(mTarget.mTargetAtlas,i, (index,name) => {
                    mTarget.mSelectSpriteNameGroup[index] = name;
                    //Debug.Log(name);
                });
            }
            //selectIndex = EditorGUILayout.Popup("索引:" + i.ToString(), selectIndex, mSpriteNameArray);
            //if (mTarget.mSelectSpriteNameGroup[i] != mSpriteNameGroup[selectIndex])
            //{
            //    mTarget.mSelectSpriteNameGroup[i] = mSpriteNameGroup[selectIndex];
            //    //mTarget.SetSpriteByIndex(i);
            //}
        }
        
        
    }

}

