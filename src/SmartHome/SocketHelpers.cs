using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    static class SocketHelpers
    {
        public static async Task<byte[]> Send(IPAddress ipAddress, byte[] data)
        {
            using (var client = new UdpClient(9998))
            {
                client.MulticastLoopback = false;
                var remoteEp = new IPEndPoint(ipAddress, 9999);
                await client.SendAsync(data, data.Length, remoteEp);
                return client.Receive(ref remoteEp);
            }
        }
    }
}
