using Shrike.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Shrike.Logging
{
    public class UdpLogger
    {
        private const int Port = 22222;
        private readonly LoadBalancer _loadBalancer;

        public UdpLogger(LoadBalancer loadBalancer)
        {
            _loadBalancer = loadBalancer;
        }
        public void StartListening()
        {
            Task.Run(async () =>
            {
                using var udpClient = new UdpClient(Port);
                IPEndPoint remoteEP = new(IPAddress.Any, Port);

                while (true)
                {
                    var received = await udpClient.ReceiveAsync();
                    string message = Encoding.UTF8.GetString(received.Buffer);

                    if (message.Contains("where"))
                    {
                        var target = await _loadBalancer.GetRedirectHummingbird();
                        string response = target != null ? $"Redirecting to {target.Address}" : "No available Hummingbird";
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        await udpClient.SendAsync(responseData, responseData.Length, received.RemoteEndPoint);
                    }
                    Console.WriteLine($"[UDP Log] {message}");
                }
            });
        }
    }
}