using System;
using System.Runtime.InteropServices;

namespace Gpt4All.Utils
{
    public static class TextUtils
    {
        /// <summary>
        /// Copy null-terminated Utf8 string from native memory to managed byte buffer.
        /// </summary>
        public static byte[] BytesFromNativeUtf8(IntPtr nativeUtf8)
        {
            // check input null
            if (nativeUtf8 == IntPtr.Zero)
                return null;

            // find null terminator
            var len = 0;
            while (Marshal.ReadByte(nativeUtf8, len) != 0) ++len;

            // check empty string
            if (len == 0)
                return Array.Empty<byte>();

            var buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return buffer;
        }

    }
}