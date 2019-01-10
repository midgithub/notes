using UnityEngine;
using System.Collections;
using XLua;

namespace SG
{
    //byte转换为string
[Hotfix]
    public class ByteToString
    {
        public static string toString(byte[] src)
        {
            //byte[] actionName = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, src);
            string sActionName = System.Text.Encoding.UTF8.GetString(src).Replace("\0", null);

            return sActionName;
        }
    }

    //声音
[Hotfix]
    public class AudioCore
    {
        public static bool GenerateAudio(int iAudioId, ref string fileName)
        {
            bool bSuc = false;
            //gamedb.AudioDesc desc = Res_FileDB.g_resFileDB.GetAudioDescByID(iAudioId);
            if (iAudioId == 0)
            {
                return bSuc;
            }

            LuaTable desc = ConfigManager.Instance.Common.GetAudioConfig(iAudioId);

            if (desc != null)
            {
                Random.seed = System.Guid.NewGuid().GetHashCode();
                int trigRanValue = Random.Range(0, 100);
                if (trigRanValue <= desc.Get<int>("triggerRatio"))
                {
                    //int fileRanValue = Random.Range(0, desc.Get<int>("filesNum") - 1);

                    fileName = desc.Get<string>("fileName_0");
                    bSuc = true;
                }
            }

            return bSuc;
        }
    }

[Hotfix]
    public class BaseTool : MonoBehaviour
    {
        private int m_intQueueValue = 1;
        private static BaseTool m_Instance = null;

        public static BaseTool instance
        {
            get { return m_Instance; }
        }

        void Awake()
        {
            m_Instance = this;
        }

        //获取32位序列值
        public int GetIntQueueValue()
        {
            return m_intQueueValue++;
        }

        //点是否在扇形内2D,x-z平面
        public bool PointInFunXZ(Vector3 srcPos, Vector3 srcEulerAngle, float angle, float radius, Vector3 dstPos, float bodyRadius)
        {
            if (!IsPointInCircleXZ(srcPos, dstPos, radius, bodyRadius))
            {
                return false;
            }

            float r = Mathf.Sqrt((srcPos.x - dstPos.x) * (srcPos.x - dstPos.x) + (srcPos.z - dstPos.z) * (srcPos.z - dstPos.z));
            if (r < bodyRadius)
            {
                return true;
            }

            float angleRevise = Mathf.Asin(bodyRadius / r) / Mathf.PI * 180 * 2 + angle;
            angleRevise = angleRevise > 360 ? 360 : angleRevise;

            Quaternion faceQuaternion = Quaternion.Euler(srcEulerAngle.x, srcEulerAngle.y, srcEulerAngle.z);

            Vector3 v1 = dstPos - srcPos;
            v1.y = 0;
            v1.Normalize();
            Vector3 v2 = faceQuaternion * Vector3.forward;
            v2.Normalize();

            float vDot = Vector3.Dot(v1, v2);

            if (vDot > 1f)
            {
                //浮点误差会导致Acos越界
                vDot = 1f;
            }
            float angleDelta = Mathf.Acos(vDot) / Mathf.PI * 180;

            if (angleDelta < angleRevise / 2)
            {
                return true;
            }

            return false;
        }


        //点是否在矩形内X-Z平面
        public bool IsPointInRectangleXZ(
            Vector3 srcPos, Vector3 srcEulerAngle, float srcLength, float srcWidth,
            Vector3 dstPos)
        {
            //近2个点        
            Quaternion dir0 = Quaternion.Euler(srcEulerAngle.x, srcEulerAngle.y - 90, srcEulerAngle.z);
            Quaternion dir1 = Quaternion.Euler(srcEulerAngle.x, srcEulerAngle.y + 90, srcEulerAngle.z);
            Vector3 pos0 = srcPos + dir0 * Vector3.forward * (srcWidth / 2);
            Vector3 pos1 = srcPos + dir1 * Vector3.forward * (srcWidth / 2);

            //远端2个点
            float angle = Mathf.Atan2(srcWidth / 2, srcLength) * Mathf.Rad2Deg;
            float length = Mathf.Sqrt((srcLength * srcLength + (srcWidth / 2) * (srcWidth / 2)));

            Quaternion dir2 = Quaternion.Euler(srcEulerAngle.x, srcEulerAngle.y - angle, srcEulerAngle.z);
            Quaternion dir3 = Quaternion.Euler(srcEulerAngle.x, srcEulerAngle.y + angle, srcEulerAngle.z);
            Vector3 pos2 = srcPos + dir2 * Vector3.forward * length;
            Vector3 pos3 = srcPos + dir3 * Vector3.forward * length;

            if (IsInTriangle(dstPos, pos0, pos1, pos2) || IsInTriangle(dstPos, pos3, pos1, pos2))
            {
                return true;
            }

            return false;
        }


        public bool IsPointInCircleXZ(Vector3 srcPos, Vector3 dstPos, float radius, float bodyRadius)
        {
            float r2 = (srcPos.x - dstPos.x) * (srcPos.x - dstPos.x) + (srcPos.z - dstPos.z) * (srcPos.z - dstPos.z);
            float reviseRadius = radius + bodyRadius;
            return r2 < reviseRadius * reviseRadius ? true : false;
        }

        bool IsInTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            float x = point.x;
            float y = point.z;

            float v0x = v0.x;
            float v0y = v0.z;

            float v1x = v1.x;
            float v1y = v1.z;

            float v2x = v2.x;
            float v2y = v2.z;

            float t = TriangleArea(v0x, v0y, v1x, v1y, v2x, v2y);

            float a = TriangleArea(v0x, v0y, v1x, v1y, x, y) + TriangleArea(v0x, v0y, x, y, v2x, v2y) + TriangleArea(x, y, v1x, v1y, v2x, v2y);

            if (Mathf.Abs(t - a) <= 0.01f)
            {
                return true;
            }

            return false;
        }

        float TriangleArea(float v0x, float v0y, float v1x, float v1y, float v2x, float v2y)
        {
            return Mathf.Abs((v0x * v1y + v1x * v2y + v2x * v0y - v1x * v0y - v2x * v1y - v0x * v2y) / 2f);
        }

        //圆是否在矩形范围内
        public bool IsCircleInRectangleXZ(Vector3 srcPos, Vector3 srcDir, float length, float width,
                Vector3 dstPos, float dstRadius)
        {
            dstPos.y = srcPos.y;

            Vector3 dstDir = dstPos - srcPos;

            //夹角大于90度，直接去掉     
            float angle = Vector3.Angle(srcDir.normalized, dstDir.normalized);
            if (angle >= 90)
            {
                return false;
            }

            //目标的垂直+水平距离
            float distance = Vector3.Distance(srcPos, dstPos);
            float dstLength = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dstWidth = distance * Mathf.Sin(angle * Mathf.Deg2Rad);

            //LogMgr.UnityLog("distance=" + distance + ", dstLength=" + dstLength + ", dstWidth=" + dstWidth+
            //    ", dstRadius=" + dstRadius + ", width=" + width);

            if (dstLength >= length + dstRadius)
            {
                return false;
            }

            if (dstWidth >= width / 2 + dstRadius)
            {
                return false;
            }

            return true;
        }

        //获取给定点的XZ屏幕距离
        public float GetDistanceInXZ(Vector3 srcPos, Vector3 dstPos)
        {
            dstPos.y = srcPos.y;

            return Vector3.Distance(srcPos, dstPos);
        }

        //获取给定点对应地面上点
        public Vector3 GetGroundPoint(Vector3 srcPos)
        {
            Vector3 dstPos = new Vector3(srcPos.x, srcPos.y, srcPos.z);
            Vector3 rayPos = srcPos + Vector3.up * 10;

            //垂直向下发射射线，找到对应的目标点
            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("ground");
            if (Physics.Raycast(rayPos, -Vector3.up, out beHit, Mathf.Infinity, layerMask))
            {
                dstPos = beHit.point;
            }
            else
            {
                dstPos = Vector3.zero;
            }
            return dstPos;
        }

        //是否在地面下
        public bool IsGroundDown(Vector3 aimPos)
        {
            //地面有方向，只能向下发射。2点之间有地面
            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("ground");

            Vector3 rayPos = aimPos + Vector3.up * 5;

            if (Physics.Raycast(rayPos, -Vector3.up, out beHit, 5, layerMask))
            {
                return true;
            }

            return false;
        }

        //是否在地面的正上方
        public bool IsAboveTheGround(Vector3 aimPos)
        {
            //地面有方向，只能向下发射。2点之间有地面
            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("ground");

            Vector3 rayPos = aimPos + Vector3.up * 5;

            if (Physics.Raycast(rayPos, -Vector3.up, out beHit, 30, layerMask))
            {
                return true;
            }

            return false;
        }

        //获取有空气墙阻挡对应的点
        public Vector3 GetWallHitPoint(Vector3 srcPos, Vector3 dstPos)
        {
            //float distance = Vector3.Distance(srcPos, dstPos);
            //Vector3 dir = dstPos - srcPos;
            //dir.Normalize();

            //Vector3 retPos = dstPos;

            ////垂直向下发射射线，找到对应的目标点
            //RaycastHit beHit;
            //int layerMask = 1 << LayerMask.NameToLayer("wall");

            //srcPos.y += 0.2f;
            //if (Physics.Raycast(srcPos, dir, out beHit, distance, layerMask))
            //{
            //    //LogMgr.UnityLog("beHit.point=" + beHit.point.ToString("f4"));
            //    retPos = beHit.point;
            //}

            return GetLineReachablePos(srcPos, dstPos);
        }

        public Vector3 GetLineReachablePos(Vector3 srcPos, Vector3 dstPos)
        {
            bool bSlide = false;
            return SceneDataMgr.Instance.LineSegementDection(srcPos, dstPos, false, ref bSlide);
        }

        //获取有怪物阻挡对应的点
        public Vector3 GetMonsterHitPoint(Vector3 srcPos, Vector3 dstPos)
        {
            float distance = Vector3.Distance(srcPos, dstPos);
            Vector3 dir = dstPos - srcPos;
            dir.Normalize();

            Vector3 retPos = dstPos;

            //垂直向下发射射线，找到对应的目标点
            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("monster");

            if (Physics.Raycast(srcPos, dir, out beHit, distance, layerMask))
            {
                retPos = beHit.transform.root.position;
            }

            return retPos;
        }

        //获取有怪物
        public GameObject GetActorRaycast(Vector3 srcPos, Vector3 dstPos, ActorType aimActorType)
        {
            float distance = Vector3.Distance(srcPos, dstPos);
            Vector3 dir = dstPos - srcPos;
            dir.Normalize();

            //垂直向下发射射线，找到对应的目标点
            RaycastHit beHit;
            int layerMask = 0;

            switch (aimActorType)
            {
                case ActorType.AT_MONSTER:
                    layerMask = 1 << LayerMask.NameToLayer("monster");
                    break;

                case ActorType.AT_MECHANICS:
                    layerMask = 1 << LayerMask.NameToLayer("monster");
                    break;

                case ActorType.AT_BOSS:
                    layerMask = 1 << LayerMask.NameToLayer("boss");
                    break;

                case ActorType.AT_LOCAL_PLAYER:
                    layerMask = 1 << LayerMask.NameToLayer("mainplayer");
                    break;
                default:
                    return null;
            }

            if (Physics.Raycast(srcPos, dir, out beHit, distance, layerMask))
            {
                return beHit.transform.gameObject;
            }

            return null;
        }

        public Transform FindChildTransform(GameObject aimObj, string transformName)
        {
            Transform[] transforms = aimObj.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < transforms.Length; ++i)
            {
                if (transforms[i].name.Equals(transformName))
                {
                    return transforms[i];
                }
            }

            return null;
        }

        //能否进行到下一个位置,判断阻挡
        public bool CanMoveToPos(Vector3 curPos, Vector3 nextPos, float curRadius)
        {
            nextPos.y = curPos.y;
            if (curPos == nextPos)
            {
                return true;
            }

            return !SceneDataMgr.Instance.IsBlocked(nextPos);

            //nextPos.y = curPos.y;        
            //Vector3 dstDir = nextPos - curPos;
            //dstDir.Normalize();

            //if (curPos == nextPos)
            //{
            //    return true;
            //}

            ////1,是否碰到阻挡
            //Vector3 leftDir = Quaternion.Euler(new Vector3(0, 90, 0)) * dstDir;
            //Vector3 rightDir = Quaternion.Euler(new Vector3(0, -90, 0)) * dstDir;
            //Vector3 wallPos = GetWallHitPoint(curPos, nextPos);

            //const float Wall_Check_Scale = 1.414f;

            //if (wallPos != nextPos || IsWallPoint(leftDir, nextPos, (curRadius + 0.2f) * Wall_Check_Scale)
            //    || IsWallPoint(rightDir, nextPos, (curRadius + 0.2f) * Wall_Check_Scale)
            //    || IsWallPoint(dstDir, nextPos, (curRadius + 0.2f) * Wall_Check_Scale))
            //{
            //    return false;
            //}

            //return true;
        }

        public bool IsWallPoint(Vector3 dir, Vector3 srcPos, float distance)
        {
            RaycastHit beHit;
            int layerMask = 1 << LayerMask.NameToLayer("wall");

            //向上提高一点
            srcPos.y += 0.2f;
            if (Physics.Raycast(srcPos, dir.normalized, out beHit, distance, layerMask))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 统一设置position入口，方便查找来源
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="pos"></param>
        static public void SetPosition(Transform tr, Vector3 pos)
        {
            //ActorObj actor = tr.GetComponent<ActorObj>();
            // if(actor != null)
            // {
            //    if( actor.mActorType == ActorType.AT_REMOTE_PLAYER)
            //     {
            //             int nBreak = 1;
            //     }
            // }

            tr.position = pos;
        }

        static public void SetLocalPosition(Transform tr, Vector3 pos)
        {
            tr.localPosition = pos;
        }

        static public void ResetTransform(Transform tr)
        {
            if (tr != null)
            {
                tr.localPosition = Vector3.zero;
                tr.localRotation = Quaternion.identity;
                tr.localScale = Vector3.one;
            }
        }

        static public void CopyTransform(Transform tr, Transform target)
        {
            if (tr != null && target != null)
            {
                tr.localPosition = target.localPosition;
                tr.localRotation = target.localRotation;
                tr.localScale = target.localScale;
            }
        }

        static public Color FormatColor(string col)
        {
            string[] colorStr = col.Split(new char[] { ',' });
            float r, g, b, a;
            r = g = b = a = 1f;
            if (colorStr.Length > 0)
            {
                r = FromatColorComponent(colorStr[0]);
            }

            if (colorStr.Length > 1)
            {
                g = FromatColorComponent(colorStr[1]);
            }

            if (colorStr.Length > 2)
            {
                b = FromatColorComponent(colorStr[2]);
            }

            if (colorStr.Length > 3)
            {
                a = FromatColorComponent(colorStr[3]);
            }

            return new Color(r, g, b, a);
        }

        static public float FromatColorComponent(string com)
        {
            float ret = 1f;
            int r_int;
            if (int.TryParse(com, out r_int))
            {
                ret = (float)r_int / 255;
            }

            return ret;
        }


        static public object DeepCopy(object src, System.Reflection.BindingFlags flag =
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        {
            object dest = null;
            if (src != null)
            {

                System.Type type = src.GetType();

                System.Reflection.FieldInfo[] fields = type.GetFields(flag);

                dest = System.Activator.CreateInstance(type);
           

                if (dest != null)
                {
                    System.Type destType = dest.GetType();

                    for (int i = 0; i < fields.Length; ++i)
                    {
                        object val = null;

                        if (fields[i].FieldType.IsArray)
                        {
                            object srcObj = fields[i].GetValue(src);
                            System.Array array = srcObj as System.Array;
                            if (array != null)
                            {
                                if (array.Length > 0)
                                {
                                    object first = array.GetValue(0);
                                    System.Array destArray = System.Array.CreateInstance(first.GetType(), array.Length);
                                    for (int j = 0; j < destArray.Length; ++j)
                                    {
                                        object v = array.GetValue(j);
                                        destArray.SetValue(v, j);
                                    }

                                    val = destArray;
                                }
                            }
                        }
                        //处于性能考虑，先不深拷贝泛型数组的内容
                        //else if (fields[i].FieldType.IsGenericType)
                        //{

                        //    object srcObj = fields[i].GetValue(src);
                        //    IList list = srcObj as IList;
                        //    if (list != null)
                        //    {
                        //        if (list.Count > 0)
                        //        {
                        //            object first = list[0];
                        //            IList destArray = System.Activator.CreateInstance(fields[i].FieldType) as IList;
                        //            for (int j = 0; j < list.Count; ++j)
                        //            {
                        //                object v = list[j];
                        //                destArray.Add(v);
                        //            }

                        //            val = destArray;
                        //        }
                        //    }
                        //}
                        else
                        {
                            val = fields[i].GetValue(src);
                        }
                        System.Reflection.FieldInfo finfo = destType.GetField(fields[i].Name, flag);
                        finfo.SetValue(dest, val);


                    }
                }

            }
            return dest;
        }

        static bool CheckValid(float v, float min, float max = float.MaxValue)
        {

            if (float.IsNaN(v))
            {
                return false;
            }


            if (v >= min && v <= max)
            {
                return true;
            }

            return false;
        }

        static bool CheckValid(int v, int min, int max = int.MaxValue)
        {

            if (v >= min && v <= max)
            {
                return true;
            }

            return false;
        }
    }
}

