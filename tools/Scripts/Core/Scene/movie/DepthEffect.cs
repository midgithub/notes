using XLua;
using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
    public class DepthEffect : ImageEffectBase
    {

        public Color blendColor = Color.cyan;
        Camera cam;
        RenderTexture rt = null;
        GameObject obj = null;
        //GameObject root = null;

        Material genDepth = null;
        public float ZOffset = 0.5f;
        public string[] MaskLayer = {"player"};
        public bool Simple = true;
        bool isInit = false;
      
        // Use this for initialization
        protected override void Start()
        {
            Init();

            if (obj == null)
            {
                obj = new GameObject("tempCam");

                obj.transform.parent = gameObject.transform;


                cam = obj.AddComponent<Camera>();
                if (cam == null) return;
                cam.CopyFrom(GetComponent<Camera>());
                if (Simple == false)
                {
                    GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
                    cam.depthTextureMode = DepthTextureMode.Depth;
                    cam.enabled = false;
                    genDepth = new Material(CoreEntry.gResLoader.LoadShader("Custom/GenDepthTex"));
                    genDepth.hideFlags = HideFlags.HideAndDontSave;
                }

                int layerMask = LayerMask.GetMask(MaskLayer);
                cam.cullingMask = layerMask;
                //不渲染天空盒，避免不透明
                cam.clearFlags = CameraClearFlags.SolidColor;
                GetComponent<Camera>().cullingMask -= layerMask;
                //root = GameObject.FindGameObjectWithTag("Passable");
                if (rt == null && cam != null)
                {
                    int rtW = Screen.width;// source.width;
                    int rtH = Screen.height;
                    rt = RenderTexture.GetTemporary(rtW, rtH, 16, RenderTextureFormat.ARGB32);

                    cam.targetTexture = rt;

                    //偏移，使得摄像机不于原来的重叠，避免深度值相同
                    obj.transform.localPosition = new Vector3(0f, 0f, ZOffset);
                }
            }
        }

        void Init()
        {
            if (isInit == false)
            {
                if (Simple)
                {
                    MaterialPath = "AutoMaterial/simple depth effect";
                }
                else
                {
                    MaterialPath = "AutoMaterial/depth effect";
                }

                isInit = true;
            }
        }

        void OnDestroy()
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        void OnEnable()
        {
            Init();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (rt != null)
            {
                RenderTexture.ReleaseTemporary(rt);
            }

            isInit = false;

        }

      
      
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
          


            if (Simple)
            {
                material.SetColor("_BlendColor", blendColor);
                material.SetTexture("_ColorTex", rt);
                Graphics.Blit(source, destination, material);
            }
            else
            {

                //创建新的renderbuffer的depth一定要与source的一致
                RenderTexture MainDepthColor = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);

                //生成第一张深度图
                Graphics.Blit(source, MainDepthColor, genDepth);

                material.SetColor("_BlendColor", blendColor);


                //将第二张深度图放到显卡中
                cam.Render();
                //将原深度图加入到shader中
                material.SetTexture("_MainDepthTex", MainDepthColor);
                //将另一个camera中的颜色信息传入到shader
                material.SetTexture("_ColorTex", rt);
                Graphics.Blit(source, destination, material);

                RenderTexture.ReleaseTemporary(MainDepthColor);
            }
           
        }
    }
}