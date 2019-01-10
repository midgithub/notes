using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
using UnityEngine.EventSystems;
using System;

[LuaCallCSharp]

    public class KeyBoardIOS : InputField {
 
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        onClickInput();
        base.OnPointerClick(eventData);
    }

    private void onClickInput()
    {
        XYSDK.activeInitializeIosUI();
    } 
}
 

