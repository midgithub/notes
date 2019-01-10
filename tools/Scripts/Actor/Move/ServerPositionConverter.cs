using XLua;
﻿using System;
using UnityEngine;

namespace SG
{
[Hotfix]
    public class ServerPositionConverter
    {
        public static float ServerScale = 1000.0f;
        public static float ClientScale = 0.001f;
        public static Vector2 ConvertToServerPosition(Vector3 position)
        {
            return new Vector2(position.x * ServerScale, position.z * ServerScale);
        }
        public static Vector3 ConvertToClientPosition(Vector2 position)
        {
            return new Vector3(position.x * ClientScale, 0, position.y * ClientScale);
        }
        public static Vector2 ConvertToClientPositionVector2(Vector2 position)
        {
            return new Vector2(position.x * ClientScale, position.y * ClientScale);
        }
        public static float ConvertToServerRotation(Quaternion rotation)
        {
            return (float)(rotation.eulerAngles.y * Math.PI / 180.0f * ServerScale) ;
        }
        public static float ConvertToServerRotation(float yaw)
        {
            return (float)(yaw * Math.PI / 180.0f * ServerScale);
        }
        public static Quaternion ConvertToClientRotation(float serverYaw , Quaternion roation)
        {
            return Quaternion.Euler(roation.eulerAngles.x, serverYaw * ClientScale, roation.eulerAngles.z);
        }
        public static float ConvertToClientRotation(float serverYaw)
        {
            return serverYaw * ClientScale;
        }
    }
}

