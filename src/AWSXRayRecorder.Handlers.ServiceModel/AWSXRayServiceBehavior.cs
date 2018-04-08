#if !NETSTANDARD

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Kralizek.XRayRecorder.Internal;

namespace Kralizek.XRayRecorder
{
    public class AWSXRayServiceBehavior : Attribute, IServiceBehavior
    {
        private readonly string _prefix;

        public AWSXRayServiceBehavior(string prefix = null)
        {
            _prefix = prefix;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var dispatcher in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>())
            {
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    var segmentName = $"{_prefix ?? "WCF"}:{serviceHostBase.Description.Name}";
                    endpoint.DispatchRuntime.MessageInspectors.Add(new AWSXRayDispatchMessageInspector(segmentName));
                }
            }
        }
    }
}

#endif
