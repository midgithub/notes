using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    /// <summary>
    /// 跳动标记。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class JumpMark : MonoBehaviour
    {
        static Keyframe[] DefaultCurve = new Keyframe[3] { new Keyframe(0, 0), new Keyframe(0.3f, 1), new Keyframe(0.6f, 0)};

        public AnimationCurve JumpCurve = new AnimationCurve(DefaultCurve);
        public float Range = 5;

        private RectTransform selfRT;
        private Vector3 initPosition;
        private float timeCount;

		protected void Awake()
		{
            selfRT = this.GetComponent<RectTransform>();
            initPosition = selfRT.localPosition;
            JumpCurve.postWrapMode = WrapMode.Loop;
        }

		void Update ()
		{
            timeCount += Time.deltaTime;
            float y = JumpCurve.Evaluate(timeCount) * Range;
            selfRT.localPosition = new Vector3(initPosition.x, initPosition.y + y, initPosition.z);
        }
        
        public void Stop(bool resetpos = true)
        {
            if (resetpos)
            {
                timeCount = 0;
                GetComponent<RectTransform>().localPosition = initPosition;
            }
            this.enabled = false;
        }
    }
}


