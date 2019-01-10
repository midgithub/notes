using XLua;
﻿using UnityEngine;
using System.Collections;

namespace SG
{
[Hotfix]
	public class RoleUtility
	{
        public static long CurRoleID
        {
            get
            {
                return PlayerData.Instance.RoleID;
            }
        }

        public static string GetRoleKey(string key)
        {
            return string.Format("{0}_{1}", CurRoleID, key);
        }

        //
        // 摘要:
        //     Removes key and its corresponding value from the preferences.
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(GetRoleKey(key));
        }

        //
        // 摘要:
        //     Returns true if key exists in the preferences.
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(GetRoleKey(key));
        }

        //
        // 摘要:
        //     Returns the value corresponding to key in the preference file if it exists.
        public static float GetFloat(string key, float defaultValue = 0)
        {
            return PlayerPrefs.GetFloat(GetRoleKey(key), defaultValue);
        }

        //
        // 摘要:
        //     Returns the value corresponding to key in the preference file if it exists.
        public static int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(GetRoleKey(key), defaultValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(GetRoleKey(key), defaultValue ? 1 : 0) != 0;
        }


        //
        // 摘要:
        //     Returns the value corresponding to key in the preference file if it exists.
        public static string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(GetRoleKey(key), defaultValue);
        }

        //
        // 摘要:
        //     Sets the value of the preference identified by key.
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(GetRoleKey(key), value);
        }

        //
        // 摘要:
        //     Sets the value of the preference identified by key.
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(GetRoleKey(key), value);
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(GetRoleKey(key), value ? 1 : 0);
        }

        //
        // 摘要:
        //     Sets the value of the preference identified by key.
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(GetRoleKey(key), value);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}


