using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    public static class Utils
    {
        public static byte[] Encrypt(byte[] input, byte key = 0xAB)
        {
            for (var i = 0; i < input.Length; i++)
            {
                var c = input[i];
                input[i] = (byte)(c ^ key);
                key = input[i];
            }
            return input;
        }

        public static byte[] Decrypt(byte[] buffer, byte key = 0xAB)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                var c = buffer[i];
                buffer[i] = (byte)(c ^ key);
                key = c;
            }
            return buffer;
        }
    }
}
