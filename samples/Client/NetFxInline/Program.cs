using System;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using Contracts;
using Kralizek.XRayRecorder;

namespace NetFxInline
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var binding = new NetTcpBinding();
            var endpointAddress = new EndpointAddress(new Uri("net.tcp://localhost:8001"));

            var channelFactory = new ChannelFactory<IService>(binding, endpointAddress);
            channelFactory.Endpoint.EndpointBehaviors.Add(new AWSXRayBehavior());

            var client = channelFactory.CreateChannel();

            try
            {
                var items = await client.ReturnsEmptyAsync();

                (client as IClientChannel).Close();

                Console.WriteLine($"Received {items.Length} items");
            }
            catch (Exception)
            {
                (client as IClientChannel).Abort();

                throw;
            }
        }
    }
}
