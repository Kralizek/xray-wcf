using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Contracts;

namespace NetFxConfiguration
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(Service)))
            {
                host.Open();

                foreach (var endpoint in host.Description.Endpoints)
                {
                    Console.WriteLine($"The service is ready at {endpoint.Address.Uri}");
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
