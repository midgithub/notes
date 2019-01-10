using XLua;
﻿using UnityEngine;
using System.Collections;
using SevenZip.Compression.LZMA;
using System.IO;
using System;

namespace Bundle
{
[Hotfix]
    public class LZMATool
    {
        public static void CompressFileLZMA(string inFile, string outFile)
        {
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            CompressLZMA(input, output);
        }

        public static void DecompressFileLZMA(string inFile, string outFile)
        {
            FileStream input = new FileStream(inFile, FileMode.Open);
            FileStream output = new FileStream(outFile, FileMode.Create);

            DeCompressLZMA(input, output);
        }

        public static byte[] CompressBytesLZMA(byte[] inbytes)
        {
            MemoryStream input = new MemoryStream(inbytes);
            MemoryStream output = new MemoryStream();

            return ((MemoryStream)CompressLZMA(input, output)).ToArray();
        }

        public static byte[] DecompressBytesLZMA(byte[] inbytes)
        {
            MemoryStream input = new MemoryStream(inbytes);
            MemoryStream output = new MemoryStream();

            return ((MemoryStream)DeCompressLZMA(input, output)).ToArray();
        }

        static Stream CompressLZMA(Stream input, Stream output)
        {
            Encoder coder = new Encoder();

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            output.Write(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            output.Flush();
            output.Close();
            input.Close();

            return output;
        }

        static Stream DeCompressLZMA(Stream input, Stream output)
        {
            Decoder decoder = new Decoder();

            // Read the decoder properties
            byte[] properties = new byte[5];
            input.Read(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            // Decompress the file.
            decoder.SetDecoderProperties(properties);
            decoder.Code(input, output, input.Length, fileLength, null);
            output.Flush();
            output.Close();
            input.Close();

            return output;
        }
    }
}

