using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Sampling;

namespace Kralizek.XRayRecorder.Internal
{
    public class AWSXRayClientMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (correlationState is RecordingContext context)
            {
                var instance = AWSXRayRecorder.Instance;

                instance.SetEntity(context.Entity);

                if (context.RequiresSegmentTermination)
                {
                    instance.EndSegment();
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var instance = AWSXRayRecorder.Instance;

            if (!instance.IsTracingDisabled())
            {
                bool requiresSegmentTermination = false;

                if (!instance.IsEntityPresent() || !TraceHeader.TryParse(instance.GetEntity(), out var traceHeader))
                {
                    traceHeader = new TraceHeader
                    {
                        ParentId = null,
                        RootTraceId = TraceId.NewId(),
                        Sampled = SampleDecision.Unknown
                    };

                    instance.BeginSegment("Temporary", traceHeader.RootTraceId, traceHeader.ParentId);

                    requiresSegmentTermination = true;
                }

                var typedHeader = new MessageHeader<string>(traceHeader.ToString());
                var untypedHeader = typedHeader.GetUntypedHeader(TraceHeader.HeaderKey, AWSConstants.TraceHeaderNamespace);

                request.Headers.Add(untypedHeader);

                return new RecordingContext(instance.GetEntity(), requiresSegmentTermination);
            }

            return null;
        }

        private class RecordingContext
        {
            public Entity Entity { get; }

            public bool RequiresSegmentTermination { get; }

            public RecordingContext(Entity entity, bool requiresSegmentTermination)
            {
                Entity = entity;
                RequiresSegmentTermination = requiresSegmentTermination;
            }
        }
    }
}