using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace GUITools
{
    public class PkgFileEntry
    {
        public string Path;
        public int Parts;
        public int Offset;
    }

    /// <summary>
    /// TalesRunner / Rhaon .pkg reader (same crypto as recovered tr_pkgtool).
    /// </summary>
    public static class PkgArchive
    {
        static readonly byte[] AesKey1 = {
            0x0D, 0x68, 0x07, 0x6F, 0x0A, 0x09, 0x07, 0x6C,
            0x65, 0x73, 0x0D, 0x75, 0x6E, 0x0A, 0x65, 0x0D
        };
        static readonly byte[] XorKey1 = {
            0x05, 0x5B, 0xCB, 0x64, 0xFB, 0xC2, 0xCE, 0xB4,
            0x77, 0x8B, 0x1B, 0xB8, 0xE9, 0xB5, 0x9C, 0xC6
        };
        static readonly byte[] AesKey2 = {
            0xFD, 0xD7, 0x15, 0xCB, 0xBE, 0xBF, 0xA5, 0xFF, 0xEF, 0x9E, 0xED, 0x97, 0xCE, 0x96, 0xD3, 0x0F,
            0x4C, 0xDC, 0xA0, 0x1D, 0xAF, 0x5F, 0xCF, 0xA2, 0xD8, 0xB1, 0x58, 0x08, 0xB9, 0xB6, 0xC1, 0x0A
        };
        static readonly byte[] XorKey2 = {
            0x20, 0x44, 0xB2, 0xA3, 0x63, 0xC7, 0x47, 0x88,
            0x4D, 0x1E, 0x2F, 0x12, 0x90, 0x39, 0x3C, 0x8E
        };

        public static byte[] Extract(string pkgPath, int bodyOff, int partNum)
        {
            using (var f = File.OpenRead(pkgPath))
            using (var ms = new MemoryStream())
            {
                f.Seek(bodyOff, SeekOrigin.Begin);
                var br = new BinaryReader(f);
                for (int i = 0; i < partNum; i++)
                {
                    f.Seek(8, SeekOrigin.Current);
                    int fileSize = br.ReadInt32();
                    f.Seek(4, SeekOrigin.Current);
                    int encryptType = br.ReadInt32();
                    byte[] fileData = br.ReadBytes(fileSize);
                    if ((encryptType & 1) != 0)
                        fileData = ZlibDecompress(fileData);
                    if ((encryptType & 2) != 0)
                        fileData = Decrypt(fileData, AesKey2, XorKey2);
                    ms.Write(fileData, 0, fileData.Length);
                }
                return ms.ToArray();
            }
        }

        public static List<PkgFileEntry> ListEntries(string pkgPath)
        {
            var list = new List<PkgFileEntry>();
            using (var f = File.OpenRead(pkgPath))
            {
                var br = new BinaryReader(f);
                byte[] hdr = br.ReadBytes(12);
                byte[] dec = Decrypt(hdr, AesKey1, XorKey1);
                if (Encoding.ASCII.GetString(dec) != "ACAC35E5-4B7")
                    throw new InvalidDataException("Not a TalesRunner pkg");
                f.Seek(0x14, SeekOrigin.Begin);
                int toc = br.ReadInt32();
                f.Seek(toc, SeekOrigin.Begin);
                f.Seek(4, SeekOrigin.Current);
                int fileNum = br.ReadInt32();
                f.Seek(4, SeekOrigin.Current);
                Encoding kr;
                try { kr = Encoding.GetEncoding(949); }
                catch { kr = Encoding.Default; }
                for (int i = 0; i < fileNum; i++)
                {
                    int entrySize = br.ReadInt32();
                    byte[] entryData = br.ReadBytes(entrySize);
                    byte[] entry = ZlibDecompress(entryData);
                    int z = 0;
                    while (z < entry.Length && entry[z] != 0) z++;
                    string name = kr.GetString(entry, 0, z);
                    int parts = 1, body = 0;
                    if (entry.Length >= 0x418)
                    {
                        parts = BitConverter.ToInt32(entry, 0x410);
                        body = BitConverter.ToInt32(entry, 0x414);
                    }
                    list.Add(new PkgFileEntry { Path = name, Parts = parts, Offset = body });
                }
            }
            return list;
        }

        public static bool LooksLikePkg(string pkgPath)
        {
            try
            {
                using (var f = File.OpenRead(pkgPath))
                {
                    byte[] hdr = new byte[12];
                    if (f.Read(hdr, 0, 12) != 12) return false;
                    byte[] dec = Decrypt(hdr, AesKey1, XorKey1);
                    return Encoding.ASCII.GetString(dec) == "ACAC35E5-4B7";
                }
            }
            catch { return false; }
        }

        static byte[] Decrypt(byte[] data, byte[] aesKey, byte[] xorKey)
        {
            byte[] result = (byte[])data.Clone();
            int n = (data.Length / 16) * 16;
            if (n > 0)
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.None;
                    aes.BlockSize = 128;
                    aes.KeySize = aesKey.Length * 8;
                    aes.Key = aesKey;
                    using (var dec = aes.CreateDecryptor())
                    {
                        byte[] tmp = dec.TransformFinalBlock(data, 0, n);
                        Buffer.BlockCopy(tmp, 0, result, 0, n);
                    }
                }
            }
            for (int i = n; i < data.Length; i++)
                result[i] = (byte)(data[i] ^ xorKey[i % 16]);
            return result;
        }

        static byte[] ZlibDecompress(byte[] data)
        {
            if (data == null || data.Length < 6)
                throw new InvalidDataException("zlib too small");
            using (var input = new MemoryStream(data, 2, data.Length - 2))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                var buf = new byte[8192];
                int n;
                while ((n = deflate.Read(buf, 0, buf.Length)) > 0)
                    output.Write(buf, 0, n);
                return output.ToArray();
            }
        }
    }
}
