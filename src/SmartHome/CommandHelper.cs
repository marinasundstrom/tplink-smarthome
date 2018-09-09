using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    internal static class CommandHelper
    {
        public static async Task<string> SendCommand(UdpClient client, IPAddress ipAddress, string command)
        {
            byte[] data = Encoding.UTF8.GetBytes(command);
            byte[] encryptedData = EncryptionHelpers.Encrypt(data);

            byte[] results = await SocketHelpers.Send(
                client, ipAddress, encryptedData)
                .ConfigureAwait(false);

            byte[] decryptedResults = EncryptionHelpers.Decrypt(results);
            return Encoding.UTF8.GetString(decryptedResults);
        }

        public static Task<string> SendCommand(
            IPAddress ipAddress, string command) =>
                SendCommand(SocketHelpers.CreateUdpClient(), ipAddress, command);
    }
}
