using UnityEngine;
using System.Collections.Generic;

public class LoadAnimation : MonoBehaviour {

    private static List<int> _loadAnim = new List<int>(); //记录添加完动画事件 模型id

    void Awake()
    {
        _loadAnim.Clear();
    }

    public static void Load(Animation animation, int modleId)
    {
        if (_loadAnim.Contains(modleId))
            return;

        _loadAnim.Add(modleId);

        switch (modleId)
        {
            case 1002:
                loadTanglanAnim(animation);
                break;
            case 1001:
                loadTianniuAnim(animation);
                break;
            case 1003:
                loadQiumalu1Anim(animation);
                break;
            case 1005:
                loadQiumalu2Anim(animation);
                break;
            case 1004:
                loadBeetle1Anim(animation);
                break;
            default:
                loadFlower1Anim(animation);
                break;
        }
    }

    private static void loadTanglanAnim(Animation animation)
    {
        //攻击
        AnimationEvent eStrike = new AnimationEvent();
        eStrike.functionName = "OnStrike";
        AnimationEvent eStrikeEnd = new AnimationEvent();
        eStrikeEnd.time = 0.96f;
        eStrikeEnd.functionName = "OnStrikeEnd";
        AnimationClip strikeClip = animation.GetClip("tanglan_strike");
        strikeClip.events = new AnimationEvent[] { eStrike, eStrikeEnd };

        //受击
        AnimationEvent eStriked = new AnimationEvent();
        eStriked.time = 0.46f;
        eStriked.functionName = "OnStrikedEnd";
        AnimationClip strikedClip = animation.GetClip("tanglan_striked");
        strikedClip.events = new AnimationEvent[] { eStriked };

        //死亡
        AnimationEvent eDyingEnd = new AnimationEvent();
        eDyingEnd.time = 1.13f;
        eDyingEnd.functionName = "OnDyingEnd";
        AnimationClip dieClip = animation.GetClip("tanglan_die");
        dieClip.events = new AnimationEvent[] { eDyingEnd };
    }

    private static void loadTianniuAnim(Animation animation)
    {
        //攻击
        AnimationEvent eStrike = new AnimationEvent();
        eStrike.functionName = "OnStrike";
        AnimationEvent eStrikeEnd = new AnimationEvent();
        eStrikeEnd.time = 1.63f;
        eStrikeEnd.functionName = "OnStrikeEnd";
        AnimationClip strikeClip = animation.GetClip("tianniu_strike");
        strikeClip.events = new AnimationEvent[] { eStrike, eStrikeEnd };

        //受击
        AnimationEvent eStriked = new AnimationEvent();
        eStriked.time = 0.46f;
        eStriked.functionName = "OnStrikedEnd";
        AnimationClip strikedClip = animation.GetClip("tianniu_striked");
        strikedClip.events = new AnimationEvent[] { eStriked };

        //死亡
        AnimationEvent eDyingEnd = new AnimationEvent();
        eDyingEnd.time = 1.46f;
        eDyingEnd.functionName = "OnDyingEnd";
        AnimationClip dieClip = animation.GetClip("tianniu_die");
        dieClip.events = new AnimationEvent[] { eDyingEnd };
    }

    private static void loadQiumalu1Anim(Animation animation)
    {
        //攻击
        AnimationEvent eStrike = new AnimationEvent();
        eStrike.functionName = "OnStrike";
        AnimationEvent eStrikeEnd = new AnimationEvent();
        eStrikeEnd.time = 2.96f;
        eStrikeEnd.functionName = "OnStrikeEnd";
        AnimationClip strikeClip = animation.GetClip("qiumalu1_strike");
        strikeClip.events = new AnimationEvent[] { eStrike, eStrikeEnd };

        //受击
        AnimationEvent eStriked = new AnimationEvent();
        eStriked.time = 0.63f;
        eStriked.functionName = "OnStrikedEnd";
        AnimationClip strikedClip = animation.GetClip("qiumalu1_striked");
        strikedClip.events = new AnimationEvent[] { eStriked };

        //死亡
        AnimationEvent eDyingEnd = new AnimationEvent();
        eDyingEnd.time = 2.3f;
        eDyingEnd.functionName = "OnDyingEnd";
        AnimationClip dieClip = animation.GetClip("qiumalu1_die");
        dieClip.events = new AnimationEvent[] { eDyingEnd };
    }

    private static void loadQiumalu2Anim(Animation animation)
    {
        //攻击
        AnimationEvent eStrike = new AnimationEvent();
        eStrike.functionName = "OnStrike";
        AnimationEvent eStrikeEnd = new AnimationEvent();
        eStrikeEnd.time = 2.96f;
        eStrikeEnd.functionName = "OnStrikeEnd";
        AnimationClip strikeClip = animation.GetClip("qiumalu2_strike");
        strikeClip.events = new AnimationEvent[] { eStrike, eStrikeEnd };

        //受击
        AnimationEvent eStriked = new AnimationEvent();
        eStriked.time = 0.63f;
        eStriked.functionName = "OnStrikedEnd";
        AnimationClip strikedClip = animation.GetClip("qiumalu2_striked");
        strikedClip.events = new AnimationEvent[] { eStriked };

        //死亡
        AnimationEvent eDyingEnd = new AnimationEvent();
        eDyingEnd.time = 2.3f;
        eDyingEnd.functionName = "OnDyingEnd";
        AnimationClip dieClip = animation.GetClip("qiumalu2_die");
        dieClip.events = new AnimationEvent[] { eDyingEnd };
    }

    private static void loadBeetle1Anim(Animation animation)
    {
        //攻击
        AnimationEvent eStrike = new AnimationEvent();
        eStrike.functionName = "OnStrike";
        AnimationEvent eStrikeEnd = new AnimationEvent();
        eStrikeEnd.time = 1.63f;
        eStrikeEnd.functionName = "OnStrikeEnd";
        AnimationClip strikeClip = animation.GetClip("beetle1_strike");
        strikeClip.events = new AnimationEvent[] { eStrike, eStrikeEnd };

        //受击
        AnimationEvent eStriked = new AnimationEvent();
        eStriked.time = 0.66f;
        eStriked.functionName = "OnStrikedEnd";
        AnimationClip strikedClip = animation.GetClip("beetle1_striked");
        strikedClip.events = new AnimationEvent[] { eStriked };

        //死亡
        AnimationEvent eDyingEnd = new AnimationEvent();
        eDyingEnd.time = 1.3f;
        eDyingEnd.functionName = "OnDyingEnd";
        AnimationClip dieClip = animation.GetClip("beetle1_die");
        dieClip.events = new AnimationEvent[] { eDyingEnd };
    }

    private static void loadFlower1Anim(Animation animation)
    {
        //攻击
        AnimationEvent eStrikeEnd = new AnimationEvent();
        eStrikeEnd.time = 1.8f;
        eStrikeEnd.functionName = "OnStrikeEnd";
        AnimationClip strikeClip = animation.GetClip("flower1_strike");
        strikeClip.events = new AnimationEvent[] { eStrikeEnd };

        //死亡
        AnimationEvent eDyingEnd = new AnimationEvent();
        eDyingEnd.time = 1.7f;
        eDyingEnd.functionName = "OnDyingEnd";
        AnimationClip dieClip = animation.GetClip("flower1_die");
        dieClip.events = new AnimationEvent[] { eDyingEnd };
    }
}
