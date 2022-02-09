using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Lab3
{
    class LSBMethod
    {
        static public bool WriteMessageToBMP(string pathToInput, string pathToOutput, string message, int bitsToReplace)
        {
            if (pathToInput == "" || pathToInput == null) return false;
            if (pathToOutput == "" || pathToOutput == null) return false;
            if (message == "" || message == null) return false;
            if (bitsToReplace < 1 || bitsToReplace > 8) return false;

            try
            {
                int imageWidth = 0, imageHeight = 0;
                int imageOffset = 0, WidthOffset = 0;

                using (BinaryReader reader = new BinaryReader(File.Open(pathToInput, FileMode.Open)))
                {
                    reader.BaseStream.Seek(0x00, SeekOrigin.Begin);
                    string Signature = new string(reader.ReadChars(2));
                    if (Signature != "BM")
                        return false;

                    reader.BaseStream.Seek(0x0A, SeekOrigin.Begin);
                    imageOffset = reader.ReadInt32();

                    reader.BaseStream.Seek(0x12, SeekOrigin.Begin);
                    imageWidth = reader.ReadInt32();

                    reader.BaseStream.Seek(0x16, SeekOrigin.Begin);
                    imageHeight = reader.ReadInt32();

                    reader.BaseStream.Seek(0x1C, SeekOrigin.Begin);
                    int bitPerPixel = reader.ReadInt32();

                    if (bitPerPixel != 24)
                        return false;
                    if (bitsToReplace * 3 * imageWidth * imageHeight < message.Length * 8 + sizeof(int))
                        return false;

                    WidthOffset = 4 - ((3 * imageWidth) % 4);
                    WidthOffset = WidthOffset == 4 ? 0 : WidthOffset;
                }

                if (File.Exists(pathToOutput))
                    File.Delete(pathToOutput);
                File.Copy(pathToInput, pathToOutput);

                List<bool> bitMessage = new List<bool>();

                foreach (bool bit in new BitArray(BitConverter.GetBytes(message.Length)))
                    bitMessage.Add(bit);

                foreach (char ch in message)
                {
                    int i = 0;
                    foreach (bool bit in new BitArray(BitConverter.GetBytes(ch)))
                    {
                        if (i < 8)
                            bitMessage.Add(bit);
                        ++i;
                    }
                }

                using (BinaryWriter writer = new BinaryWriter(File.Open(pathToOutput, FileMode.Open)))
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(pathToInput, FileMode.Open)))
                    {
                        byte[] PowerTwo = new byte[8] { 1, 2, 4, 8, 16, 32, 64, 128 };

                        int currBit = 0;
                        int bitMessageSize = bitMessage.Count;

                        writer.Seek(imageOffset, SeekOrigin.Begin);
                        reader.BaseStream.Seek(imageOffset, SeekOrigin.Begin);
                        for (int i = 0; i < imageHeight; ++i)
                        {
                            for (int j = 0; j < imageWidth; ++j)
                            {
                                for (int f = 0; f < 3; ++f)
                                {
                                    byte color = reader.ReadByte();
                                    if (currBit < bitMessageSize)
                                    {
                                        for (int k = bitsToReplace - 1; k >= 0; --k)
                                        {
                                            if (currBit >= bitMessageSize)
                                                break;
                                            if (bitMessage[currBit])
                                                color = (byte)(color | PowerTwo[k]);
                                            else
                                                color = (byte)(color & (~PowerTwo[k]));
                                            currBit++;
                                        }
                                        writer.Write(color);
                                    }
                                    else
                                        writer.Seek(1, SeekOrigin.Current);
                                }
                            }
                            writer.Seek(WidthOffset, SeekOrigin.Current);
                            reader.BaseStream.Seek(WidthOffset, SeekOrigin.Current);
                        }
                    }
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                if (File.Exists(pathToOutput))
                    File.Delete(pathToOutput);
                return false;
            }
            return true;
        }

        static public string ReadMessageFromBMP(string pathToInput, int bitsToReplace)
        {
            if (pathToInput == "" || pathToInput == null) return null;
            if (bitsToReplace < 1 || bitsToReplace > 8) return null;

            try
            {
                int imageWidth = 0, imageHeight = 0;
                int imageOffset = 0, WidthOffset = 0;

                using (BinaryReader reader = new BinaryReader(File.Open(pathToInput, FileMode.Open)))
                {
                    reader.BaseStream.Seek(0x00, SeekOrigin.Begin);
                    string Signature = new string(reader.ReadChars(2));
                    if (Signature != "BM")
                        return null;

                    reader.BaseStream.Seek(0x0A, SeekOrigin.Begin);
                    imageOffset = reader.ReadInt32();

                    reader.BaseStream.Seek(0x12, SeekOrigin.Begin);
                    imageWidth = reader.ReadInt32();

                    reader.BaseStream.Seek(0x16, SeekOrigin.Begin);
                    imageHeight = reader.ReadInt32();

                    reader.BaseStream.Seek(0x1C, SeekOrigin.Begin);
                    int bitPerPixel = reader.ReadInt32();

                    if (bitPerPixel != 24)
                        return null;

                    WidthOffset = 4 - ((3 * imageWidth) % 4);
                    WidthOffset = WidthOffset == 4 ? 0 : WidthOffset;



                    byte[] PowerTwo = new byte[8] { 1, 2, 4, 8, 16, 32, 64, 128 };

                    List<bool> bitMessage = new List<bool>();

                    reader.BaseStream.Seek(imageOffset, SeekOrigin.Begin);
                    for (int i = 0; i < imageHeight; ++i)
                    {
                        for (int j = 0; j < imageWidth; ++j)
                        {
                            for (int f = 0; f < 3; ++f)
                            {
                                byte color = reader.ReadByte();
                                for (int k = bitsToReplace - 1; k >= 0; --k)
                                {
                                    if (color == (color | PowerTwo[k]))
                                        bitMessage.Add(true);
                                    else
                                        bitMessage.Add(false);
                                }
                            }
                        }
                        reader.BaseStream.Seek(WidthOffset, SeekOrigin.Current);
                    }

                    BitArray messageSizeBitArray = new BitArray(32);
                    for (int i = 0; i < 32; ++i)
                        messageSizeBitArray[i] = bitMessage[i];
                    byte[] messageSizeByteArray = new byte[4];
                    messageSizeBitArray.CopyTo(messageSizeByteArray, 0);
                    int messageSize = BitConverter.ToInt32(messageSizeByteArray);

                    BitArray messageBitArray = new BitArray(bitMessage.Count - 32);
                    for (int i = 0; i < bitMessage.Count - 32; ++i)
                        messageBitArray[i] = bitMessage[i + 32];
                    byte[] messageByteArray = new byte[(bitMessage.Count - 32) / 8 + 1];
                    messageBitArray.CopyTo(messageByteArray, 0);

                    string message = "";
                    for (int i = 0; i < messageSize; ++i)
                        message += Convert.ToChar(messageByteArray[i]);
                    return message;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
