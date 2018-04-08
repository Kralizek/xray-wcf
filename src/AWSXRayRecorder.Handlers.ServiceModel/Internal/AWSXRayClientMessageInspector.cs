using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using Amazon.XRay.Recorder.Core.Internal.Utils;

namespace Kralizek.XRayRecorder.Internal
{
    public class AWSXRayClientMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var instance = AWSXRayRecorder.Instance;

            if (!instance.IsTracingDisabled())
            {
                if (TraceHeader.TryParse(TraceContext.GetEntity(), out var header))
                {
                    var typedHeader = new MessageHeader<string>(header.ToString());
                    var untypedHeader = typedHeader.GetUntypedHeader(TraceHeader.HeaderKey, AWSConstants.TraceHeaderNamespace);

                    request.Headers.Add(untypedHeader);
                }
            }

            return null;
        }
    }
}