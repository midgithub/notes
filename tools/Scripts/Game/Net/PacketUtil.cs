using XLua;
﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

///(客户端<->服务器)网络消息头
public struct PackHeader
{
    public Int16 HeaderSign; //始终为固定值 0x52FA
    public Int32 BodySize;      // 包体大小（字节数）
    public Byte RandSeed; //随机种子，用来生成CheckSum
    public Byte CheckSum; //检验和, 生成见后文函数	
    public Int32 Compress;//是否压缩，我们弃用,0表示不压缩
    public Int16 CMD; //消息命令码
    public Int16 Index; //包的序列号，从0 ~9999 ，必须循环递增
}

public enum CompressType
{
    None = 0 ,
    ZLib = 1 ,
    LZ4 = 2 ,
}

[Hotfix]
public class PacketUtil
{
    public static readonly byte[] EncryptKey = {
                                                0x90, 0xee, 0x70, 0x07, 0x02,
                                                0x77, 0xe9, 0x9e, 0x0e, 0x79,
                                                0xe0, 0x87, 0x03, 0x7e, 0xe6,
                                                0x90, 0x1d, 0x3a, 0xe3, 0x81,
                                                0x1a, 0x6d, 0xf4, 0x52, 0x13,
                                                0xfc, 0xf0, 0x8b, 0x14, 0x63};
    public static void Encrypt(byte[] buffer, int Len)
    {
        //Int32 Len = buffer.Length;
        Int32 HalfLen = Len >> 1;
        for (var i = 0; i < HalfLen; i++)
        {
            byte tmp = buffer[i];
            buffer[i] = (byte)(buffer[Len - 1 - i] ^ (EncryptKey)[(i + Len) % EncryptKey.Length]);
            buffer[Len - 1 - i] = (byte)(tmp ^ (EncryptKey)[(Len - 1 - i + Len) % EncryptKey.Length]);
        }
        if (Len % 2 == 1)
        {
            buffer[Len / 2] = (byte)(buffer[Len / 2] ^ (EncryptKey)[(Len / 2 + Len) % EncryptKey.Length]);
        }
    }


    public static void Decrypt(byte[] buffer, int offset, int len)
    {
        Int32 Len = len;
        Int32 HalfLen = len >> 1;
        for (var i = 0; i < HalfLen; i++)
        {
            byte tmp = buffer[i + offset];
            buffer[i + offset] = (byte)(buffer[Len - 1 - i + offset] ^ (EncryptKey)[(Len - 1 - i + Len) % EncryptKey.Length]);
            buffer[Len - 1 - i + offset] = (byte)(tmp ^ (EncryptKey)[(i + Len) % EncryptKey.Length]);
        }

        if (Len % 2 == 1)
        {
            buffer[Len / 2 + offset] = (byte)(buffer[Len / 2 + offset] ^ (EncryptKey)[(Len / 2 + Len) % EncryptKey.Length]);
        }


    }

    public static void Decrypt(byte[] buffer)
    {
        Int32 Len = buffer.Length;
        Int32 HalfLen = buffer.Length >> 1;
        for (var i = 0; i < HalfLen; i++)
        {
            byte tmp = buffer[i];
            buffer[i] = (byte)(buffer[Len - 1 - i] ^ (EncryptKey)[(Len - 1 - i + Len) % EncryptKey.Length]);
            buffer[Len - 1 - i] = (byte)(tmp ^ (EncryptKey)[(i + Len) % EncryptKey.Length]);
        }

        if (Len % 2 == 1)
        {
            buffer[Len / 2] = (byte)(buffer[Len / 2] ^ (EncryptKey)[(Len / 2 + Len) % EncryptKey.Length]);
        }
    }


    public static Byte CheckSum(Byte randSeed, byte[] buffer, int Len)
    {
        //Int32 Len = buffer.Length;
        byte ret = 0;
        for (var i = 0; i < Len; i++)
        {
            ret = (byte)(ret ^ EncryptKey[(randSeed + buffer[i] + Len) % EncryptKey.Length]);
        }
        return ret;
    }
    
    //*
    public static bool Unpack<T>(List<byte> buffer ,out T obj) where T : new()
    {
        obj = new T();
        var bytebuffer = buffer.ToArray();
        int offset = 0;

        var fields = typeof(T).GetFields();
        foreach (var f in fields)
        {
            if(!f.FieldType.IsArray)
            {
                f.SetValue(obj, UnpackElement(bytebuffer ,ref offset ,f.FieldType));
            }
            else
            {
                var array = f.GetValue(obj) as Array;
                var elementType = f.FieldType.GetElementType();
                for (var index=0; index < array.Length;index++)
                {
                    array.SetValue(UnpackElement(bytebuffer, ref offset ,elementType), index);
                }
            }
        }
        return true;
    }

    protected static object UnpackElement(byte[] buffer , ref int offset ,Type type)
    {        
        object result = null;
        if(type==typeof(ulong))
        {
            result = BitConverter.ToUInt64(buffer, offset);
            offset += 8;
        }
        else if(type==typeof(long))
        {
            result = BitConverter.ToInt64(buffer, offset);
            offset += 8;
        }
        else if(type==typeof(uint))
        {
            result = BitConverter.ToUInt32(buffer, offset);
            offset += 4;
        }
        else if(type == typeof(int))
        {
            result = BitConverter.ToInt32(buffer, offset);
            offset += 4;
        }
        else if(type==typeof(ushort))
        {
            result = BitConverter.ToUInt16(buffer, offset);
            offset += 2;
        }
        else if(type == typeof(short))
        {
            result = BitConverter.ToInt16(buffer, offset);
            offset += 2;
        }
        else if(type == typeof(byte) || type==typeof(sbyte))
        {
            result = buffer[offset];
            offset += 1;
        }
        else if(type==typeof(float))
        {
            result = BitConverter.ToSingle(buffer, offset);
            offset += 4;
        }
        else if (type == typeof(double))
        {
            result = BitConverter.ToDouble(buffer, offset);
            offset += 8;
        }

        return result;
    }
   
    public static byte[] compress_lz4(byte[] source ,int offset, int length ,out int compressLength)
    {
        try
        {
            int maxDestSize = LZ4ps.LZ4Codec.MaximumOutputLength(length);
            byte[] dest = new byte[maxDestSize];
             
            compressLength = LZ4ps.LZ4Codec.Encode32(source,offset,length,dest, 0,maxDestSize); 

            if (compressLength == 0)
                dest = null;

            return dest;
        }
        catch(Exception e)
        {
            SG.LogMgr.LogError(e.ToString());
        }

        compressLength = 0;
        return null;
    }


    public static int uncompress_lz4(byte[] source ,int offset, int length , byte[] dest, int destLength)
    {
        int len = 0;
        try
        {
            len = LZ4ps.LZ4Codec.Decode32(source, offset, length, dest, 0, destLength, false);           
        }
        catch(Exception e)
        {
            SG.LogMgr.LogError(e.ToString());
        }
        return len;
    }
}

