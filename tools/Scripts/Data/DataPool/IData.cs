
/**
 * @file IData.cs
 * @brief 数据接口
 *
 * @author lixiaojiang
 * @version 1.0.0
 * @date 2012-12-16
 */

using XLua;
using System;
using System.Collections.Generic;
//using System.Diagnostics;

namespace SG
{

  /**
   * @brief 数据接口
   */
  public interface IData
  {
    /**
     * @brief 提取数据
     *
     * @param node
     *
     * @return 
     */
    bool CollectDataFromDBC(DBC_Row node);

    /**
     * @brief 获取数据ID
     *
     * @return 
     */
    int GetId();
  }
}



