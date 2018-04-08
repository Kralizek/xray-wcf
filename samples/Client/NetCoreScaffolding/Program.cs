using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Kralizek.XRayRecorder;
using ServiceReference;

namespace NetCoreScaffolding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new ServiceClient(ServiceClient.EndpointConfiguration.NetTcpBinding_IService);

            try
            {
                var response = await client.ReturnsSomethingAsync(new ReturnsSomethingRequest());

                await client.CloseAsync();

                Console.WriteLine($"Received {response.ReturnsSomethingResult.Length} items");

                foreach (var contact in response.ReturnsSomethingResult)
                {
                    Console.WriteLine($"\t{contact.FirstName} {contact.LastName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                client.Abort();

                //throw;
            }

            Console.WriteLine("Press <ENTER> to exit");
            Console.ReadLine();
        }
    }
}

namespace ServiceReference
{
    public partial class ServiceClient
    {
        // https://blogs.msdn.microsoft.com/webdev/2017/06/28/wcf-web-service-reference-mex-authentication/
        static partial void ConfigureEndpoint(ServiceEndpoint serviceEndpoint, ClientCredentials clientCredentials)
        {
            serviceEndpoint.EndpointBehaviors.Add(new AWSXRayBehavior());
        }
    }
}