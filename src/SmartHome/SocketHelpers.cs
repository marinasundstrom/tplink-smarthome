using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SmartHome
{
    internal static class SocketHelpers
    {
        public static UdpClient CreateUdpClient() => new UdpClient(9998);

        public static async Task<byte[]> Send(IPAddress ipAddress, byte[] data)
        {
            using (UdpClient client = CreateUdpClient())
            {
                return await Send(client, ipAddress, data).ConfigureAwait(false);
            }
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
