using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlinkPinvokeVIP.Core
{
    public class Common
    {
        public static IntPtr Utf8StringToIntptr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return IntPtr.Zero;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            IntPtr intPtr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, intPtr, bytes.Length);
            Marshal.WriteByte(intPtr, bytes.Length, 0);
            return intPtr;
        }

        public static string Utf8IntptrToString(IntPtr ptr)
        {
            var data = new List<byte>();
            var off = 0;
            if (ptr != IntPtr.Zero)
            {
                while (true)
                {
                    var ch = Marshal.ReadByte(ptr, off++);
                    if (ch == 0)
                    {
                        break;
                    }
                    data.Add(ch);
                }
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }
        public static Image byteToImage(byte[] myByte)
        {
            MemoryStream ms = new MemoryStream(myByte);
            Image _Image = Image.FromStream(ms);
            return _Image;
        }
        public static byte[] StreamToBytes(Stream stream)

        {
            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 

            stream.Seek(0, SeekOrigin.Begin);

            return bytes;

        }
    }
}
