using XLua;
﻿using UnityEngine;
using System.Collections;

namespace Bundle
{
[Hotfix]
    public class Md5Tool
    {
        public static string Md5Sum(string input)
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            return Md5Sum(inputBytes);
        }

        public static string Md5Sum(byte[] input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(input);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

