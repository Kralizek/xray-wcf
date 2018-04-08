using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Contracts;
using Kralizek.XRayRecorder;

namespace NetFxConfiguration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IService client = null;

            try
            {
                var channelFactory = new ChannelFactory<IService>("MyTest");

                client = channelFactory.CreateChannel();

                var items = await client.ReturnsEmptyAsync();

                (client as IClientChannel).Close();

                Console.WriteLine($"Received {items.Length} items");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                (client as IClientChannel).Abort();
                
                //throw;
            }
        }
    }
}
