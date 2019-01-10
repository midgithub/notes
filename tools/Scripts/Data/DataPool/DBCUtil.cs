/**
 * @file DBCUtil.cs
 * @brief Xml辅助接口
 *
 * @author lixiaojiang
 * @version 0.0.1
 * @date 2012-12-12
 */

using XLua;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
namespace SG
{

  /**
   * @brief Xml辅助接口
   */
[Hotfix]
  public class DBCUtil
  {
    /**
     * @brief 从Xml节点中读取字符串
     *
     * @param node xml节点
     * @param nodeName 节点名字
     * @param defualtVal 默认值
     * @param isMust 是否强制不能为空
     *
     * @return 
     */
    public static string ExtractString(DBC_Row node, string nodeName, string defualtVal, bool isMust)
    {
      string result = defualtVal;

      if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtactString Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}

        return result;
      }

      string nodeText = node.SelectFieldByName(nodeName);
      if (Helper.StringIsNullOrEmpty(nodeText)) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtactString Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}
      } else {
        result = nodeText;
      }

      return result;
    }

    /**
     * @brief 从Xml节点中读取字符串数组
     *
     * @param node xml节点
     * @param nodeName 节点名字
     * @param defualtVal 默认值
     * @param isMust 是否强制不能为空
     *
     * @return 
     */
    public static List<string> ExtractStringList(DBC_Row node, string nodeName, string defualtVal, bool isMust)
    {
      List<string> result = new List<string>();

      if (node == null || !node.HasFields) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractStringList Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}

        return result;
      }

      string nodeText = node.SelectFieldByName(nodeName);
      if (Helper.StringIsNullOrEmpty(nodeText)) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractStringList Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}
      } else {
        result = Converter.ConvertStringList(nodeText);
      }

      return result;
    }

    /**
     * @brief 从Xml节点中读取布尔值
     *
     * @param node xml节点
     * @param nodeName 节点名字
     * @param defualtVal 默认值
     * @param isMust 是否强制不能为空
     *
     * @return 
     */
    public static bool ExtractBool(DBC_Row node, string nodeName, bool defualtVal, bool isMust)
    {
      bool result = defualtVal;

      if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractBool Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}

        return result;
      }

      string nodeText = node.SelectFieldByName(nodeName);
      if (nodeText == null) return result;
      if (Helper.StringIsNullOrEmpty(nodeText)) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractBool Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}
      } else {
        if (nodeText.Trim().ToLower() == "true" || nodeText.Trim().ToLower() == "1") {
          result = true;
        }

        if (nodeText.Trim().ToLower() == "false" || nodeText.Trim().ToLower() == "0") {
          result = false;
        }
      }

      return result;
    }

    /**
     * @brief 从Xml节点中读取数值类型，使用时，必须在函数中指明数值类型
     *          如: int id = ExtractNumeric<int>(xmlNode, "Id", -1, true);
     *
     * @param node xml节点
     * @param nodeName 节点名字
     * @param defualtVal 默认值
     * @param isMust 是否强制不能为空
     *
     * @return 
     */
    public static T ExtractNumeric<T>(DBC_Row node, string nodeName, T defualtVal, bool isMust)
    {
      T result = defualtVal;

      if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractNumeric Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}

        return result;
      }

      string nodeText = node.SelectFieldByName(nodeName);
      if (Helper.StringIsNullOrEmpty(nodeText)) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractNumeric Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}
      } else {
        try {
          result = (T)Convert.ChangeType(nodeText, typeof(T));
        } catch (System.Exception ex) {
          string info = string.Format("ExtractNumeric Error node:{0} nodeName:{1} ex:{2} stacktrace:{3}",
            node.RowIndex, nodeName, ex.Message, ex.StackTrace);
          LogMgr.UnityLog(info);
          //LogSystem.Debug(info);
          
          Helper.LogCallStack();
        }
      }

      return result;
    }

    /**
     * @brief 从Xml节点中读取数值类型，使用时，必须在函数中指明数值类型
     *          如: int id = ExtractNumeric<int>(xmlNode, "Id", -1, true);
     *
     * @param node xml节点
     * @param nodeName 节点名字
     * @param defualtVal 默认值
     * @param isMust 是否强制不能为空
     *
     * @return 
     */
    public static List<T> ExtractNumericList<T>(DBC_Row node, string nodeName, T defualtVal, bool isMust)
    {
      List<T> result = new List<T>();

      if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractNumericList Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}

        return result;
      }

      string nodeText = node.SelectFieldByName(nodeName);
      if (Helper.StringIsNullOrEmpty(nodeText)) {
        //if (isMust)
        //{
        //  string errorInfo = string.Format("ExtractNumericList Error node:{0} nodeName:{1}", node.RowIndex, nodeName);
        //  LogSystem.Assert(false, errorInfo);
        //}
      } else {
        result = Converter.ConvertNumericList<T>(nodeText);
      }

      return result;
    }

    /**
     * @brief 从Xml节点中抽取所有以prefix为前缀的节点
     *
     * @param node xml节点
     * @param prefix 前缀字符串
     *
     * @return 
     */
    public static List<string> ExtractNodeByPrefix(DBC_Row node, string prefix)
    {

      if (node == null || !node.HasFields) {
        return null;
      }

      return node.SelectFieldsByPrefix(prefix);
    }
  }
}

