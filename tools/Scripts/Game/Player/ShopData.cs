/**
* @file     : ShopData.cs
* @brief    : 商店数据管理(已转由Lua层实现)
* @details  : 
* @author   : XuXiang
* @date     : 2017-07-13 14:07
*/

using UnityEngine;
using System.Collections;
using XLua;
using System.Collections.Generic;
using System;

namespace SG
{
    /// <summary>
    /// 商店数据。
    /// </summary>
    [LuaCallCSharp]
    [Hotfix]
    public class ShopData
    {
        [CSharpCallLua]
        public delegate void ShoppingCall(LuaTable tb, int id, int num);

        /// <summary>
        /// 发送购买请求。
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="num">购买数量</param>
        public static void SendShoppingRequest(int id, int num)
        {
            //转调Lua层
            LuaTable G = LuaMgr.Instance.GetLuaEnv().Global;
            LuaTable model = G.GetInPath<LuaTable>("ModelManager.ShopModel");
            ShoppingCall fun = model.Get<ShoppingCall>("SendShoppingRequest");
            if (fun != null)
            {
                fun(model, id, num);
            }
        }        
    }
}