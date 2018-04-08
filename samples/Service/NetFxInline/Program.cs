using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Kralizek.XRayRecorder;

namespace NetFxInline
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri[] baseAddresses = new Uri[]
            {
                new Uri("http://localhost:8000/"),
                new Uri("net.tcp://localhost:8001"),
                new Uri("net.pipe://localhost/xraytest"),
            };

            using (ServiceHost host = new ServiceHost(typeof(Service), baseAddresses))
            {
                var smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true
                };

                host.Description.Behaviors.Add(smb);
                host.Description.Behaviors.Add(new AWSXRayBehavior());

                host.Open();

                foreach (var uri in baseAddresses)
                {
                    Console.WriteLine($"The service is ready at {uri}");
                }

                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                host.Close();
            }
        }
    }

    public class Service : IService
    {
        public Task<Contact> ReturnsNullAsync()
        {
            return Task.FromResult(null as Contact);
        }

        public Task<Contact[]> ReturnsEmptyAsync()
        {
            return Task.FromResult(new Contact[0]);
        }

        public Task<Contact> ThrowSomething()
        {
            throw new Exception("This is an expected error");
        }
    }
}
