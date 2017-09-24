﻿using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    static class CommandHelper
    {
        public static async Task<string> SendCommand(IPAddress ipAddress, string command)
        {
            var data = Encoding.UTF8.GetBytes(command);
            var encryptedData = EncryptionHelpers.Encrypt(data);

            var results = await SocketHelpers.Send(
                ipAddress,
                encryptedData);

            var decryptedResults = EncryptionHelpers.Decrypt(results);
            return Encoding.UTF8.GetString(decryptedResults);
        }
    }
}
