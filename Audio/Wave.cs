using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio.WAVE
{
    public class WaveFile
    {
        public static class Chunk
        {
            public struct Header
            {
                public int ID;      // 4ByteR// 文件标识
                public int Size;    // 4Byte // 文件大小

                public Header(BinaryReader binaryReader)
                {
                    this.ID = Chunk.Reverse(binaryReader.ReadInt32());
                    this.Size = binaryReader.ReadInt32();
                }
            }

            public struct RIFF
            {
                public const int ID = 0x52494646; // "RIFF"

                public int Size;    // 4Byte // 文件大小-8(Byte)
                public int Type;    // 4ByteR// 文件类型 0x57315645(WAVE)

                public RIFF(ref Header header, BinaryReader binaryReader)
                {
                    this.Size = header.Size;

                    this.Type = Reverse(binaryReader.ReadInt32());
                }
            }

            public struct Format
            {
                public const int ID = 0x666D7420; // "fmt "
                public int Size;

                public short AudioFormat;       // 2Byte // 音频格式
                public short NumChannels;       // 2Byte // 声道数目 1:单声道;2:双声道
                public int SampleRate;          // 4Byte // 采样率
                public int ByteRate;            // 4Byte // 每秒数据字节数 
                public short BlockAlign;        // 2Byte // 数据块对齐 (每个采样需要的字节数) 
                public short BitsPerSample;     // 2Byte // 采样位数 (每个采样需要的bit数)
                public short AdditionalInfo;    // 2Byte // 附加信息（可选，通过Size来判断有无）

                public Format(ref Header header, BinaryReader binaryReader)
                {
                    this.Size = header.Size;

                    this.AudioFormat = binaryReader.ReadInt16();
                    this.NumChannels = binaryReader.ReadInt16();
                    this.SampleRate = binaryReader.ReadInt32();
                    this.ByteRate = binaryReader.ReadInt32();
                    this.BlockAlign = binaryReader.ReadInt16();
                    this.BitsPerSample = binaryReader.ReadInt16();
                    if (header.Size == 0x12)
                    {
                        this.AdditionalInfo = binaryReader.ReadInt16();
                    }
                    else
                    {
                        this.AdditionalInfo = 0x00;
                    }
                }
            }

            public struct List
            {
                public const int ID = 0x4C495354; // "LIST"
                public int Size;

                public string[] Data;

                public List(ref Header header, BinaryReader binaryReader)
                {
                    this.Size = header.Size;

                    byte[] buffer = new byte[header.Size];
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = binaryReader.ReadByte();
                    }
                    this.Data = Encoding.ASCII.GetString(buffer).Split('\0').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                }
            }

            public struct Data
            {
                public const int ID = 0x64617461; // "data"
                public int Size;
                //private byte[] AudioData;   // NByte // 音频数据 Length=ByteRate*seconds

                public short[] LeftData;
                public short[] RightData;

                public Data(ref Header header, BinaryReader binaryReader)
                {
                    Size = header.Size >> 2;
                    this.LeftData = new short[this.Size];
                    this.RightData = new short[this.Size];
                    //this.AudioData = new byte[header.Size];
                    for (int i = 0; i < this.Size; i++)
                    {
                        //this.AudioData[i] = binaryReader.ReadByte();
                        this.LeftData[i] = binaryReader.ReadInt16();
                        this.RightData[i] = binaryReader.ReadInt16();
                    }
                }
            }

            public struct Unknown
            {
                public Header header;

                public byte[] data;

                public Unknown(ref Header header, BinaryReader binaryReader)
                {
                    this.header = header;

                    data = new byte[this.header.Size];
                    for (int i = 0; i < this.data.Length; i++)
                    {
                        this.data[i] = binaryReader.ReadByte();
                    }
                }
            }

            private static int Reverse(int input)
            {
                return BitConverter.ToInt32(BitConverter.GetBytes(input).Reverse().ToArray(), 0);
            }
        }

        public readonly Chunk.RIFF RIFF;
        public readonly Chunk.Format Format;
        public readonly Chunk.List List;
        public readonly Chunk.Data Data;

        List<Chunk.Unknown> Unknowns = new List<Chunk.Unknown>();

        public WaveFile(Uri uri)
        {
            using (BinaryReader binaryReader = new BinaryReader(new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                while(binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    Chunk.Header header = new Chunk.Header(binaryReader);
                    switch(header.ID)
                    {
                        case Chunk.RIFF.ID: this.RIFF = new Chunk.RIFF(ref header, binaryReader); break;
                        case Chunk.Format.ID: this.Format = new Chunk.Format(ref header, binaryReader); break;
                        case Chunk.List.ID: this.List = new Chunk.List(ref header, binaryReader); break;
                        case Chunk.Data.ID: this.Data = new Chunk.Data(ref header, binaryReader); break;
                        default: this.Unknowns.Add(new Chunk.Unknown(ref header, binaryReader)); break;
                    }
                }
                binaryReader.Close();
            }
        }

        public int GetDataLocation()
        {
            return (12) + (8 + this.Format.Size) + (8 + this.List.Size) + (8);
        }
    }
}
