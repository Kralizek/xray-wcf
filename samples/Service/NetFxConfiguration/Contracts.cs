using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NetFxConfiguration
{
    [ServiceContract(Namespace = "http://localtest.me/")]
    public interface IService
    {
        [OperationContract]
        Task<Contact> ReturnsNullAsync();

        [OperationContract]
        Task<Contact> ThrowSomething();

        [OperationContract]
        Task<Contact[]> ReturnsEmptyAsync();
    }

    [DataContract(Namespace = "http://localtest.me/")]
    public class Contact
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }
    }
}
