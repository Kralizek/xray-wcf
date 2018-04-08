using System;
using System.Runtime.Serialization;

namespace Contracts
{
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
