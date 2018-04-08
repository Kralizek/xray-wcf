#if !NETSTANDARD

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Internal.Utils;
using Amazon.XRay.Recorder.Core.Sampling;

namespace Kralizek.XRayRecorder.Internal
{
    public class AWSXRayDispatchMessageInspector : IDispatchMessageInspector
    {
        private readonly string _segmentName;

        public AWSXRayDispatchMessageInspector(string segmentName)
        {
            _segmentName = segmentName;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var recorder = AWSXRayRecorder.Instance;

            var rawHeader = FindHeader(request.Headers);

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

                if (traceHeader.Sampled == SampleDecision.Unknown || traceHeader.Sampled == SampleDecision.Requested)
                {
                    var host = request.Headers.To.Host;
                    var path = request.Headers.To.PathAndQuery;
                    var method = request.Headers.To.Scheme;

                    traceHeader.Sampled = recorder.SamplingStrategy.Sample(host, path, method);
                }

                if (!recorder.IsTracingDisabled())
                {
                    recorder.BeginSegment(_segmentName, traceHeader.RootTraceId, traceHeader.ParentId, traceHeader.Sampled);
                    recorder.AddWcfInformation(ref request, channel, instanceContext);

                    var entity = TraceContext.GetEntity();

                    return entity;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string FindHeader(MessageHeaders headers)
        {
            if (headers.FindHeader(TraceHeader.HeaderKey, AWSConstants.TraceHeaderNamespace) >= 0)
            {
                return headers.GetHeader<string>(TraceHeader.HeaderKey, AWSConstants.TraceHeaderNamespace);
            }

            return string.Empty;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState is Entity entity)
            {
                TraceContext.SetEntity(entity);

                if (reply.IsFault)
                {
                    AWSXRayRecorder.Instance.MarkError();
                }

                AWSXRayRecorder.Instance.EndSegment();

                if (TraceHeader.TryParse(entity, out var header) && header.Sampled == SampleDecision.Sampled)
                {
                    var typedHeader = new MessageHeader<string>(header.ToString());
                    var untypedHeader = typedHeader.GetUntypedHeader(TraceHeader.HeaderKey, AWSConstants.TraceHeaderNamespace);

                    reply.Headers.Add(untypedHeader);
                }
            }
        }
    }
}

#endif
