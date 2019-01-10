using XLua;
﻿using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.GZip;
using SG;

[Hotfix]
public class ZipMgr  {




    /// <summary>
    /// 压缩
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Compress(string input)
    {
        //string result = string.Empty;
        byte[] buffer = Encoding.UTF8.GetBytes(input);
        using (MemoryStream outputStream = new MemoryStream())
        {
            using (BZip2OutputStream zipStream = new BZip2OutputStream(outputStream))
            {
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();
            }
            return Convert.ToBase64String(outputStream.ToArray());
        }
    }
    /// <summary>
    /// 解压缩
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Decompress(string input)
    {
        string result = string.Empty;
        byte[] buffer = Convert.FromBase64String(input);
        using (Stream inputStream = new MemoryStream(buffer))
        {
            BZip2InputStream zipStream = new BZip2InputStream(inputStream);

            using (StreamReader reader = new StreamReader(zipStream, Encoding.UTF8))
            {
                //输出
                result = reader.ReadToEnd();
            }
        }

        return result;
    }


    public static void CopyStream(Stream input, Stream output)
    {
        //int bufferSize = 1048576;
        int bufferSize = 4096;
        byte[] buffer = new byte[bufferSize];
        while (true)
        {
            int read = input.Read(buffer, 0, buffer.Length);
            if (read <= 0)
            {
                return;
            }
            output.Write(buffer, 0, read);
        }
    }

    /// <summary>  bzip 压缩算法   速度奇慢  压缩比高一丁点
    /// 压缩文件
    /// </summary>
    /// <param name="fileToCompress"></param>
    public static void CompressFileBzip(string  inputPath , string outPath)
    { 

        FileStream inputStream = new FileStream(inputPath, FileMode.Open); 
        //string result = string.Empty;

        FileStream outputStream = new FileStream(outPath, FileMode.Create);

        using (BZip2OutputStream zipStream = new BZip2OutputStream(outputStream))
        {
            CopyStream(inputStream, zipStream);             
            zipStream.Close();
        }
        inputStream.Close();
        outputStream.Close(); 
    }

    /// <summary>
    /// 解压缩文件
    /// </summary>
    /// <param name="fileToDecompress"></param>
    public static void DecompressFileBzip(string inputPath, string outPath)
    {
        FileStream outputStream = new FileStream(outPath, FileMode.Create);
        FileStream inputStream = new FileStream(inputPath, FileMode.Open);

 
        using (BZip2InputStream zipStream = new BZip2InputStream(inputStream))
        {
            CopyStream( zipStream, outputStream);
            outputStream.Close();
        }
        inputStream.Close();
        outputStream.Close(); 

    }





    /// <summary>   Gzip 压缩算法   比上面快10 多倍 
    /// 压缩文件
    /// </summary>
    /// <param name="fileToCompress"></param>
    public static void CompressFileGz(string inputPath, string outPath)
    {

        using (FileStream inputStream = new FileStream(inputPath, FileMode.Open))
        {
            using (FileStream outputStream = new FileStream(outPath, FileMode.Create))
            {
                using (GZipOutputStream zipStream = new GZipOutputStream(outputStream))
                {
                    CopyStream(inputStream, zipStream);
                    zipStream.Flush();
                    zipStream.Close();


                }
            }
        }
       
    }

    /// <summary>
    /// 解压缩文件
    /// </summary>
    /// <param name="fileToDecompress"></param>
    public static bool  DecompressFileGz(string inputPath, string outPath)
    {
        //FileStream outputStream = new FileStream(outPath, FileMode.Create);
        bool re = true ; 
        try
        {
            FileStream inputStream = new FileStream(inputPath, FileMode.Open);


            using (FileStream outputStream = new FileStream(outPath, FileMode.Create))
            {

                using (GZipInputStream zipStream = new GZipInputStream(inputStream))
                {
                    CopyStream(zipStream, outputStream);
                    outputStream.Flush();

                    outputStream.Close();
                }
            }
        }
        catch (Exception e)
        {
            re = false; 
            LogMgr.print(e); 
        }

        return re; 

    }

    /// <summary>
    /// 解压缩文件
    /// </summary>
    /// <param name="fileToDecompress"></param>
    public static bool DecompressFileGz(byte[] inputBytes, string outPath)
    {
        //FileStream outputStream = new FileStream(outPath, FileMode.Create);
        bool re = true;
        try
        {
            MemoryStream inputStream = new MemoryStream(inputBytes);
            using (FileStream outputStream = new FileStream(outPath, FileMode.Create))
            {
                using (GZipInputStream zipStream = new GZipInputStream(inputStream))
                {
                    CopyStream(zipStream, outputStream);
                    outputStream.Flush();
                    outputStream.Close();
                }
            }
        }
        catch (Exception e)
        {
            re = false;
            LogMgr.print(e);
        }

        return re;

    }
	 
}

