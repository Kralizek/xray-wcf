#if !NETSTANDARD

using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Kralizek.XRayRecorder
{
    public class AWSXRayBehaviorExtension : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new AWSXRayBehavior(NullIfEmpty(Prefix));
        }

        public override Type BehaviorType { get; } = typeof(AWSXRayBehavior);

        private const string PrefixPropertyName = "prefix";

        [ConfigurationProperty(PrefixPropertyName)]
        public string Prefix
        {
            get => (string)base[PrefixPropertyName];
            set => base[PrefixPropertyName] = value;
        }

        private string NullIfEmpty(string value) => value == string.Empty ? null : value;
    }
}

#endif
