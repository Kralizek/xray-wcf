#if !NETSTANDARD

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Sampling;

namespace Kralizek.XRayRecorder
{
    public class AWSXRayServiceBehavior  : IServiceBehavior
    {
        private readonly string _segmentName;

        public AWSXRayServiceBehavior(string segmentName)
        {
            _segmentName = segmentName ?? throw new ArgumentNullException(nameof(segmentName));
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var dispatcher in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>())
            {
                foreach (var endpoint in dispatcher.Endpoints)
                {
                    endpoint.DispatchRuntime.MessageInspectors.Add(new AWSXRayDispatchMessageInspector(_segmentName));
                }
            }
        }

        private class AWSXRayDispatchMessageInspector : IDispatchMessageInspector
        {
            private readonly string _segmentName;

            public AWSXRayDispatchMessageInspector(string segmentName)
            {
                _segmentName = segmentName;
            }

            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                var rawHeader = request.Headers.GetHeader<string>(TraceHeader.HeaderKey, "amzn");

                try
                {
                    if (!TraceHeader.TryParse(rawHeader, out var traceHeader))
                    {
                        traceHeader = new TraceHeader
                        {
                            RootTraceId = TraceId.NewId(),
                            ParentId = null,
                            Sampled = SampleDecision.Unknown
                        };
                    }

                    if (!AWSXRayRecorder.Instance.IsTracingDisabled())
                    {
                        AWSXRayRecorder.Instance.BeginSegment(_segmentName, traceHeader.RootTraceId, traceHeader.ParentId, traceHeader.Sampled);

                        AWSXRayRecorder.Instance.AddAnnotation("MessageId", request.Headers.MessageId.ToString());
                        AWSXRayRecorder.Instance.AddAnnotation("Action", request.Headers.Action);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                }

                return null;
            }

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                AWSXRayRecorder.Instance.EndSegment();
            }
        }

    }
}

#endif
