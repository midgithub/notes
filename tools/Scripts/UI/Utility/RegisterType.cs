using UnityEngine;
using UnityEngine.UI;
using XLua;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using SG;


public static class RegisterType 
{

    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action),
            typeof(Action<bool>),
            typeof(Action<int>),
            typeof(Action<float>),
            typeof(UnityAction),
            typeof(UnityAction<float>),
            typeof(UnityAction<bool>),            
            typeof(UnityAction<string>),
            typeof(UnityAction<int>),
            typeof(UnityEvent<float>),
            typeof(UnityEvent<string>),
            typeof(UnityAction<Vector2>),
            typeof(UpdateItemDelegate),
            typeof(Comparison<Buff>),
			typeof(RectTransform),
            typeof(Button),
            typeof(Event),
            typeof(RectTransform),
            typeof(Action<PanelBase>), 
            typeof(TipAutoClose.GetTimerNoteDelegate),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Button.ButtonClickedEvent),
            typeof(UnityEngine.Events.UnityEventBase),
            typeof(UnityEngine.Events.UnityEvent),

        };


    [LuaCallCSharp]
    [ReflectionUse]
    public static List<Type> dotween_lua_call_cs_list = new List<Type>()
    {
        typeof(DG.Tweening.AutoPlay),
        typeof(DG.Tweening.AxisConstraint),
        typeof(DG.Tweening.Ease),
        typeof(DG.Tweening.LogBehaviour),
        typeof(DG.Tweening.LoopType),
        typeof(DG.Tweening.PathMode),
        typeof(DG.Tweening.PathType),
        typeof(DG.Tweening.RotateMode),
        typeof(DG.Tweening.ScrambleMode),
        typeof(DG.Tweening.TweenType),
        typeof(DG.Tweening.UpdateType),

        typeof(DG.Tweening.DOTween),
        typeof(DG.Tweening.DOVirtual),
        typeof(DG.Tweening.EaseFactory),
        typeof(DG.Tweening.Tweener),
        typeof(DG.Tweening.Tween),
        typeof(DG.Tweening.Sequence),
        typeof(DG.Tweening.TweenParams),
        typeof(DG.Tweening.Core.ABSSequentiable),

        typeof(DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions>),

        typeof(DG.Tweening.TweenCallback),
        typeof(DG.Tweening.TweenExtensions),
        typeof(DG.Tweening.TweenSettingsExtensions),
        typeof(DG.Tweening.ShortcutExtensions),
        typeof(DG.Tweening.ShortcutExtensions43),
        typeof(DG.Tweening.ShortcutExtensions46),
        typeof(DG.Tweening.ShortcutExtensions50),
       
        //dotween pro 的功能
        //typeof(DG.Tweening.DOTweenPath),
        //typeof(DG.Tweening.DOTweenVisualManager),
    };
}

