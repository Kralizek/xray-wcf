using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Contracts;
using Kralizek.XRayRecorder;

namespace NetFxAttribute
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

    [AWSXRayBehavior]
    [ServiceBehavior(Name = "MyTest")]
    public class Service : IService
    {
        public Task<Contact> ReturnsNullAsync()
        {
            return Task.FromResult(null as Contact);
        }

        public Task<Contact[]> ReturnsSomethingAsync()
        {
            return Task.FromResult(new[]
            {
                new Contact
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Renato",
                    LastName = "Golia"
                }
            });
        }

        public Task<Contact> ThrowSomething()
        {
            throw new Exception("This is an expected error");
        }
    }
}
