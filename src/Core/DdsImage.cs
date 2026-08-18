using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GUITools
{
    /// <summary>
    /// TalesRunner GUI textures are often DDS bytes saved with a .png extension.
    /// </summary>
    public static class DdsImage
    {
        public static Image LoadFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            if (data.Length >= 4 && data[0] == (byte)'D' && data[1] == (byte)'D' && data[2] == (byte)'S')
                return LoadDds(data);
            using (var ms = new MemoryStream(data))
                return new Bitmap(ms);
        }

        static Image LoadDds(byte[] data)
        {
            int height = BitConverter.ToInt32(data, 12);
            int width = BitConverter.ToInt32(data, 16);
            int pfFlags = BitConverter.ToInt32(data, 80);
            string fourcc = Encoding.ASCII.GetString(data, 84, 4).TrimEnd('\0');
            int bpp = BitConverter.ToInt32(data, 88);
            int off = 128;
            int[] pixels;
            if ((pfFlags & 0x4) != 0) // DDPF_FOURCC
            {
                int mode = 0;
                if (fourcc == "DXT1") mode = 1;
                else if (fourcc == "DXT2" || fourcc == "DXT3") mode = 3;
                else if (fourcc == "DXT4" || fourcc == "DXT5") mode = 5;
                else
                    throw new InvalidDataException("Unsupported DDS FourCC " + fourcc);
                pixels = DecodeDxt(data, off, width, height, mode);
            }
            else if (bpp == 32)
            {
                pixels = new int[width * height];
                Buffer.BlockCopy(data, off, pixels, 0, Math.Min(pixels.Length * 4, data.Length - off));
            }
            else
                throw new InvalidDataException("Unsupported DDS bpp " + bpp);

            bool anyAlpha = false;
            for (int i = 0; i < pixels.Length; i++)
            {
                if (((pixels[i] >> 24) & 255) != 0) { anyAlpha = true; break; }
            }
            if (!anyAlpha)
            {
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = (pixels[i] & 0x00FFFFFF) | unchecked((int)0xFF000000);
            }

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, width, height);
            var bits = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(pixels, 0, bits.Scan0, pixels.Length);
            bmp.UnlockBits(bits);
            return bmp;
        }

        static int[] DecodeDxt(byte[] data, int off, int width, int height, int mode)
        {
            var pixels = new int[width * height];
            int blocksX = (width + 3) / 4;
            int blocksY = (height + 3) / 4;
            int pos = off;
            for (int by = 0; by < blocksY; by++)
            {
                for (int bx = 0; bx < blocksX; bx++)
                {
                    byte[] alpha = null;
                    if (mode == 5)
                    {
                        alpha = DecodeDxt5Alpha(data, pos);
                        pos += 8;
                    }
                    else if (mode == 3)
                    {
                        alpha = DecodeDxt3Alpha(data, pos);
                        pos += 8;
                    }
                    DecodeDxtColorBlock(data, pos, pixels, bx * 4, by * 4, width, height, alpha);
                    pos += 8;
                }
            }
            return pixels;
        }

        static byte[] DecodeDxt3Alpha(byte[] data, int pos)
        {
            var alpha = new byte[16];
            for (int i = 0; i < 8; i++)
            {
                int b = data[pos + i];
                alpha[i * 2] = (byte)((b & 0xF) * 17);
                alpha[i * 2 + 1] = (byte)(((b >> 4) & 0xF) * 17);
            }
            return alpha;
        }

        static byte[] DecodeDxt5Alpha(byte[] data, int pos)
        {
            int a0 = data[pos];
            int a1 = data[pos + 1];
            var table = new byte[8];
            table[0] = (byte)a0; table[1] = (byte)a1;
            if (a0 > a1)
            {
                for (int i = 2; i < 8; i++)
                    table[i] = (byte)(((8 - i) * a0 + (i - 1) * a1) / 7);
            }
            else
            {
                for (int i = 2; i < 6; i++)
                    table[i] = (byte)(((6 - i) * a0 + (i - 1) * a1) / 5);
                table[6] = 0; table[7] = 255;
            }
            ulong bits = 0;
            for (int i = 0; i < 6; i++)
                bits |= ((ulong)data[pos + 2 + i]) << (8 * i);
            var alpha = new byte[16];
            for (int i = 0; i < 16; i++)
                alpha[i] = table[(int)((bits >> (3 * i)) & 7)];
            return alpha;
        }

        public static void ForceOpaque(Bitmap bmp)
        {
            if (bmp == null) return;
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bits = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int[] pix = new int[bmp.Width * bmp.Height];
            Marshal.Copy(bits.Scan0, pix, 0, pix.Length);
            for (int i = 0; i < pix.Length; i++)
                pix[i] = (pix[i] & 0x00FFFFFF) | unchecked((int)0xFF000000);
            Marshal.Copy(pix, 0, bits.Scan0, pix.Length);
            bmp.UnlockBits(bits);
        }

        public static void PunchDarkTransparent(Bitmap bmp, int threshold)
        {
            if (bmp == null) return;
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bits = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int[] pix = new int[bmp.Width * bmp.Height];
            Marshal.Copy(bits.Scan0, pix, 0, pix.Length);
            for (int i = 0; i < pix.Length; i++)
            {
                int c = pix[i];
                int r = (c >> 16) & 255, g = (c >> 8) & 255, b = c & 255;
                if (r <= threshold && g <= threshold && b <= threshold)
                    pix[i] = c & 0x00FFFFFF;
            }
            Marshal.Copy(pix, 0, bits.Scan0, pix.Length);
            bmp.UnlockBits(bits);
        }

        static void DecodeDxtColorBlock(byte[] data, int pos, int[] pixels, int x, int y, int width, int height, byte[] alpha)
        {
            ushort c0 = BitConverter.ToUInt16(data, pos);
            ushort c1 = BitConverter.ToUInt16(data, pos + 2);
            uint codes = BitConverter.ToUInt32(data, pos + 4);
            int[] colors = new int[4];
            colors[0] = Rgb565(c0, 255);
            colors[1] = Rgb565(c1, 255);
            if (alpha != null || c0 > c1)
            {
                colors[2] = Lerp(colors[0], colors[1], 2, 1);
                colors[3] = Lerp(colors[0], colors[1], 1, 2);
            }
            else
            {
                colors[2] = Lerp(colors[0], colors[1], 1, 1);
                colors[3] = 0;
            }
            for (int py = 0; py < 4; py++)
            {
                for (int px = 0; px < 4; px++)
                {
                    int dx = x + px, dy = y + py;
                    if (dx >= width || dy >= height) continue;
                    int code = (int)((codes >> (2 * (py * 4 + px))) & 3);
                    int argb = colors[code];
                    if (alpha != null)
                    {
                        int a = alpha[py * 4 + px];
                        argb = (a << 24) | (argb & 0x00FFFFFF);
                    }
                    pixels[dy * width + dx] = argb;
                }
            }
        }

        static int Rgb565(ushort c, int a)
        {
            int r = ((c >> 11) & 31) * 255 / 31;
            int g = ((c >> 5) & 63) * 255 / 63;
            int b = (c & 31) * 255 / 31;
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        static int Lerp(int c0, int c1, int w0, int w1)
        {
            int a0 = (c0 >> 24) & 255, r0 = (c0 >> 16) & 255, g0 = (c0 >> 8) & 255, b0 = c0 & 255;
            int a1 = (c1 >> 24) & 255, r1 = (c1 >> 16) & 255, g1 = (c1 >> 8) & 255, b1 = c1 & 255;
            int w = w0 + w1;
            return (((w0 * a0 + w1 * a1) / w) << 24)
                 | (((w0 * r0 + w1 * r1) / w) << 16)
                 | (((w0 * g0 + w1 * g1) / w) << 8)
                 | ((w0 * b0 + w1 * b1) / w);
        }
    }
}
