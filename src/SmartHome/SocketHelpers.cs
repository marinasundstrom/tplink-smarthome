using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartHome
{
    internal static class SocketHelpers
    {
        private static UdpClient s_client;

        public static UdpClient GetUdpClient() => s_client ?? (s_client = new UdpClient(9998));

        public static Task<byte[]> Send(IPAddress ipAddress, byte[] data)
        {
            return Send(GetUdpClient(), ipAddress, data);
        }

        public static async Task<byte[]> Send(UdpClient client, IPAddress ipAddress, byte[] data)
        {
            client.MulticastLoopback = false;
            var remoteEp = new IPEndPoint(ipAddress, 9999);
            await client.SendAsync(data, data.Length, remoteEp).ConfigureAwait(false);
            return client.Receive(ref remoteEp);
        }
    }
}
