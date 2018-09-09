using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome
{
    public static class EncryptionHelpers
    {
        public static byte[] Encrypt(byte[] input, byte key = 0xAB)
        {
            for (int i = 0; i < input.Length; i++)
            {
                byte c = input[i];
                input[i] = (byte)(c ^ key);
                key = input[i];
            }
            return input;
        }

        public static byte[] Decrypt(byte[] buffer, byte key = 0xAB)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                byte c = buffer[i];
                buffer[i] = (byte)(c ^ key);
                key = c;
            }
            return buffer;
        }
    }
}
