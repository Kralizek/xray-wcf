using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Internal.Utils;

namespace Kralizek.XRayRecorder
{
    public class AWSXRayEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var inspector = new AWSXRayClientMessageInspector();
            clientRuntime.ClientMessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint endpoint) { }

        private class AWSXRayClientMessageInspector : IClientMessageInspector
        {
            public void AfterReceiveReply(ref Message reply, object correlationState)
            {

            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                var instance = AWSXRayRecorder.Instance;

                if (!instance.IsTracingDisabled())
                {
                    if (TraceHeader.TryParse(TraceContext.GetEntity(), out var header))
                    {
                        var typedHeader = new MessageHeader<string>(header.ToString());
                        var untypedHeader = typedHeader.GetUntypedHeader(TraceHeader.HeaderKey, "amzn");

                        request.Headers.Add(untypedHeader);
                    }
                }

                return null;
            }
        }

    }
}