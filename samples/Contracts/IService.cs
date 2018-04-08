using System.ServiceModel;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract(Namespace = "http://localtest.me/")]
    public interface IService
    {
        [OperationContract]
        Task<Contact> ReturnsNullAsync();

        [OperationContract]
        Task<Contact> ThrowSomething();

        [OperationContract]
        Task<Contact[]> ReturnsSomethingAsync();
    }
}