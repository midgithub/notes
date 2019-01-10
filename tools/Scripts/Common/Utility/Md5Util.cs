using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using XLua;

[LuaCallCSharp]
[Hotfix]
public class Md5Util
{
    

    public static string GetFileHash(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            int len = (int)fs.Length;
            byte[] data = new byte[len];
            fs.Read(data, 0, len);
            fs.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMD5 = "";
            foreach (byte b in result)
            {
                fileMD5 += Convert.ToString(b, 16);
            }
            return fileMD5;
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return "";
        }
    }

    ///   <summary>
    ///   给一个字符串进行MD5加密
    ///   </summary>
    ///   <param   name="strText">待加密字符串</param>
    ///   <returns>加密后的字符串</returns>
    public static string MD5Encrypt(string strText)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
        StringBuilder strbul = new StringBuilder(40);
        // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串 
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

        }
        return strbul.ToString();
    }


}
 

