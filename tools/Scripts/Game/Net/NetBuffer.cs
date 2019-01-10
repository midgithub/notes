using UnityEngine;
using System.Collections;
using XLua;
using System;
using System.Text;
using System.Collections.Generic;

namespace SG
{
    /// <summary>
    /// 网络读取数据缓存区。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class NetReadBuffer
    {
        //缓存使用
        public static NetReadBuffer CacheBuff
        {
            get
            {
                if (m_CacheBuff == null)
                {
                    m_CacheBuff = new NetReadBuffer();
                }
                m_CacheBuff.Reset();
                return m_CacheBuff;
            }
        }

        private static NetReadBuffer m_CacheBuff;

        public int pos;
        public int maxpos;
        public byte[] buff;

        /// <summary>
        /// 初始化。
        /// </summary>
        public void Init(byte[] data, int index, int len)
        {
            buff = data;
            pos = 0;
            maxpos = len; // 最大位置
        }

        public void Reset()
        {
            pos = 0;
            maxpos = 0;
            buff = null;
        }

        public sbyte ReadInt8()
        {
            sbyte ret = (sbyte)buff[pos];
            pos += sizeof(sbyte);
            return ret;
        }

        public byte ReadUInt8()
        {
            byte ret = buff[pos];
            pos += sizeof(byte);
            return ret;
        }

        public short ReadInt16()
        {
            short ret = BitConverter.ToInt16(buff, pos);
            pos += sizeof(short);
            return ret;
        }

        public ushort ReadUInt16()
        {
            return (ushort)ReadInt16();
        }

        public int ReadInt32()
        {
            int ret = BitConverter.ToInt32(buff, pos);
            pos += sizeof(int);
            return ret;
        }

        public uint ReadUInt32()
        {
            return (uint)ReadInt32();
        }

        public long ReadInt64()
        {
            long ret = BitConverter.ToInt64(buff, pos);
            pos += sizeof(long);
            return ret;
        }

        public ulong ReadUInt64()
        {
            return (ulong)ReadInt64();
        }

        public float ReadFloat()
        {
            float ret = BitConverter.ToSingle(buff, pos);
            pos += sizeof(float);
            return ret;
        }

        public double ReadDouble()
        {
            double ret = BitConverter.ToDouble(buff, pos);
            pos += sizeof(double);
            return ret;
        }

        public string ReadString(int n = 0)
        {
            string ret = string.Empty;
            if (n <= 0)
            {
                //变长字符串
                ushort len = ReadUInt16();
                ret = Encoding.UTF8.GetString(buff, pos, len);
                pos += len + 1;         //多一个'\0'字符
            }
            else
            {
                //固定长度字符串 
                ret = CommonTools.BytesToString2(buff,pos,n);
                pos += n;
            }

            return ret;
        }

        public void ReadBytes(byte[] dest)
        {
            for (int i = 0; i < dest.Length; i++)
            {
                dest[i] = buff[pos + i];
            }
            pos += dest.Length;
        }
        public void ReadBytes(List<byte> dest, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dest.Add(buff[pos + i]);
            }
            pos += len;
        }
    }

    /// <summary>
    /// 网络写入数据缓存区。
    /// </summary>
    [LuaCallCSharp]
[Hotfix]
    public class NetWriteBuffer
    {
        //缓存使用
        public static NetWriteBuffer CacheBuff
        {
            get
            {
                if (m_CacheBuff == null)
                {
                    m_CacheBuff = new NetWriteBuffer();
                }
                m_CacheBuff.Reset();
                return m_CacheBuff;
            }
        }

        private static NetWriteBuffer m_CacheBuff;

        public int pos = 0;
        public byte[] buff = new byte[1024 * 5];

        /// <summary>
        /// 获取数据。
        /// </summary>
        public byte[] Data
        {
            get { return buff; }
        }

        /// <summary>
        /// 获取数据长度。
        /// </summary>
        public int Length
        {
            get { return pos; }
        }

        public void Reset()
        {
            pos = 0;
        }

        public void WriteUInt8(byte src)
        {
            buff[pos] = src;
            pos += sizeof(byte);
        }

        public void WriteInt8(sbyte src)
        {
            WriteUInt8((byte)src);
        }

        public void WriteUInt16(ushort src)
        {
            Utils.WriteUInt16(buff, pos, src);
            pos += sizeof(ushort);
        }

        public void WriteInt16(short src)
        {
            WriteUInt16((ushort)src);
        }

        public void WriteUInt32(uint src)
        {
            Utils.WriteUInt32(buff, pos, src);
            pos += sizeof(uint);
        }

        public void WriteInt32(int src)
        {
            WriteUInt32((uint)src);
        }

        public void WriteUInt64(ulong src)
        {
            Utils.WriteUInt64(buff, pos, src);
            pos += sizeof(ulong);
        }

        public void WriteInt64(long src)
        {
            WriteUInt64((ulong)src);
        }

        public void WriteFloat(float src)
        {
            Utils.WriteFloat(buff, pos, src);
            pos += sizeof(float);
        }

        public void WriteDouble(double src)
        {
            Utils.WriteDouble(buff, pos, src);
            pos += sizeof(double);
        }

        public void WriteString(string src, int n = 0)
        {
            byte[] temp = Encoding.UTF8.GetBytes(src);
            if (n <= 0)
            {
                //变长写入
                ushort len = (ushort)temp.Length;
                WriteUInt16(len);
                temp.CopyTo(buff, pos);
                pos += len + 1;
            }
            else
            {
                //固定长度写入，不足长度补0，超出长度截断
                int num = Math.Min(n, temp.Length);
                int index = 0;
                for (; index < num; ++index)
                {
                    buff[pos + index] = temp[index];
                }
                for (; index < n; ++index)
                {
                    buff[pos + index] = 0;
                }
                pos += n;
            }

        }

        public void WriteBytes(byte[] src)
        {
            src.CopyTo(buff, pos);
            pos += src.Length;
        }

        public void WriteBytes(List<byte> src)
        {
            src.CopyTo(buff, pos);
            pos += src.Count;
        }
    }

    /// <summary>
    /// 数据包头。
    /// </summary>
    public struct PacketHeader
    {
        public Int16 HeaderSign; //始终为固定值 0x52FA
        public Int32 BodySize;      // 包体大小（字节数）
        public Byte RandSeed; //随机种子，用来生成CheckSum
        public Byte CheckSum; //检验和, 生成见后文函数	
        public Int32 Compress;//是否压缩，高8位表示压缩算法，低24位表示压缩前数据长度,0表示不压缩 
        public Int16 CMD; //消息命令码
        public Int16 Index; //包的序列号，从0 ~9999 ，必须循环递增
        /// <summary>
        /// 数据头长度。
        /// </summary>
        public static int Length = sizeof(Int16) * 3 + sizeof(Byte) * 2 + sizeof(Int32) * 2;

        /// <summary>
        /// 获取压缩类型。
        /// </summary>
        /// <returns>压缩类型。</returns>
        public int GetCompressType()
        {
            return (int)((Compress & 0xFF000000) >> 24);
        }

        /// <summary>
        /// 获取解压长度。
        /// </summary>
        /// <returns>解压长度。</returns>
        public int GetUnCompressLength()
        {
            return Compress & 0x00FFFFFF;
        }
    }

    /// <summary>
    /// 发送的缓存数据。
    /// </summary>
[Hotfix]
    public class SendBuffer : CacheObject
    {
        public static int HeaderSize = sizeof(Int16) * 2;
        public static byte[] cmddata = new byte[1024 * 4];
        public static byte[] tmpBuff = new byte[1024 * 4];
        static int COMPRESS_THRESHOLD = 128;

        public int pos;
        public int sendlen;
        public byte[] msg = new byte[1024 * 4];

        public void Init(Int16 cmd, Int16 index, byte[] data, int len)
        {
            PacketHeader header = new PacketHeader();
            header.HeaderSign = 0x52FA;
            header.RandSeed = (Byte)UnityEngine.Random.Range(0, 0xff);
            header.Compress = 0;
            header.CMD = cmd;
            header.Index = index;
            int cmddatalen = data != null ? len + HeaderSize : HeaderSize;
            if (data != null)
            {
                Utils.WriteUInt16(cmddata, 0, (ushort)header.CMD);
                Utils.WriteUInt16(cmddata, sizeof(Int16), (ushort)header.Index);


                int compressLen = 0;
                if (len > COMPRESS_THRESHOLD)
                {
                    Utils.WriteUInt16(tmpBuff, 0, (ushort)header.CMD);
                    Utils.WriteUInt16(tmpBuff, sizeof(Int16), (ushort)header.Index);
                    Utils.CopyTo(data, 0, tmpBuff, HeaderSize, len);
                    header.CheckSum = PacketUtil.CheckSum(header.RandSeed, tmpBuff, cmddatalen);

                    data = PacketUtil.compress_lz4(data, 0, len, out compressLen);
                    cmddatalen = compressLen + HeaderSize;
                    header.Compress = (int)((uint)CompressType.LZ4 << 24 | (uint)len & 0x00FFFFFF);
                    PacketUtil.Encrypt(cmddata, HeaderSize);
                    PacketUtil.Encrypt(data, compressLen);
                    Utils.CopyTo(data, 0, cmddata, HeaderSize, compressLen);
                    header.BodySize = compressLen;
                    LogMgr.Log("#### compress: " + header.CMD);
                }
                else {

                    Utils.CopyTo(data, 0, cmddata, HeaderSize, len);
                    header.BodySize = len;
                    header.CheckSum = PacketUtil.CheckSum(header.RandSeed, cmddata, cmddatalen);
                    PacketUtil.Encrypt(cmddata, cmddatalen);
                }
               
                
            }
            else
            {
                Utils.WriteUInt16(cmddata, 0, (ushort)header.CMD);
                Utils.WriteUInt16(cmddata, sizeof(Int16), (ushort)header.Index);
                header.CheckSum = PacketUtil.CheckSum(header.RandSeed, cmddata, cmddatalen);
                PacketUtil.Encrypt(cmddata, cmddatalen);
                header.BodySize = 0;
            }

            pos = 0;
                 
           
            sendlen = PacketHeader.Length + header.BodySize;

            Utils.WriteUInt16(msg, pos, (ushort)header.HeaderSign);
            pos += sizeof(Int16);
            Utils.WriteUInt32(msg, pos, (uint)header.BodySize);
            pos += sizeof(Int32);
            msg[pos] = header.RandSeed;
            pos += sizeof(Byte);
            msg[pos] = header.CheckSum;
            pos += sizeof(Byte);
            Utils.WriteUInt32(msg, pos, (uint)header.Compress);
            pos += sizeof(Int32);

            if (cmddata != null)
            {
                Utils.CopyTo(cmddata, 0, msg, pos, cmddatalen);
            }
        }

        public int Length
        {
            get { return sendlen; }
        }

        public override void OnUse(object data = null)
        {
            pos = 0;
            sendlen = 0;
        }
    }

    /// <summary>
    /// 接收数据缓存。
    /// </summary>
[Hotfix]
    public class RecvBuffer : CacheObject
    {
        public Int16 cmd;                               //协议号
        public int length;                              //数据长度
        public long tick;                               //接收到的时间戳
        public byte[] data = new byte[1024 * 256];      //数据内容

        public override void OnUse(object data = null)
        {
            cmd = 0;
            length = 0;
            tick = 0;
        }
    }
}

