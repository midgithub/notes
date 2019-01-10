using XLua;
﻿using UnityEngine;
using System.Collections;


namespace SG
{

[Hotfix]
    public class CurveUtil
    {



        //
        public static void scaleCurve(   AnimationCurve curve, float time)
        {
            int len = curve.length;
            float scale = time / curve.keys[curve.length - 1].time;
            Keyframe[] newKeys = new Keyframe[len];

            for (int i = 0; i < len; i++)
            {
                Keyframe key = curve.keys[i];
                newKeys[i] = new Keyframe(key.time * scale, key.value, key.inTangent, key.outTangent);
            }
            curve.keys = newKeys;
        }


        public static AnimationCurve CreateDialogIn(float scaleTime)
        {
            Keyframe[] ks = new Keyframe[4];

            float scale = scaleTime / 10f;

            ks[0] = new Keyframe(0, 0);
            //ks[0].inTangent = 0;
            ks[1] = new Keyframe(6f * scale, 1.1f);
            //ks[1].inTangent = 45;
            ks[2] = new Keyframe(8f * scale, 0.95f);
            //ks[2].inTangent = 45;
            ks[3] = new Keyframe(10f * scale, 1f);
            //ks[3].inTangent = 45;

            return new AnimationCurve(ks);
        }


        public static AnimationCurve CreateAlphaIn(float scaleTime)
        {
            Keyframe[] ks = new Keyframe[4];

            float scale = scaleTime / 10f;

            ks[0] = new Keyframe(0, 0);
            //ks[0].inTangent = 0;
            ks[1] = new Keyframe(5f * scale, 0.8f);
            //ks[1].inTangent = 45;
            ks[2] = new Keyframe(7f * scale, 0.95f);
            //ks[2].inTangent = 45;
            ks[3] = new Keyframe(10f * scale, 1f);
            //ks[3].inTangent = 45;
            AnimationCurve ac = new AnimationCurve(ks);
            ac.SmoothTangents(1, 5);
            ac.SmoothTangents(2, 11);
            ac.SmoothTangents(3, 0); 

            return ac;
        }


    }


}

