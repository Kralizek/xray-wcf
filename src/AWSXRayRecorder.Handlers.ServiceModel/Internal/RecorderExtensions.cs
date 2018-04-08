using System.ServiceModel;
using System.ServiceModel.Channels;
using Amazon.XRay.Recorder.Core;

namespace Kralizek.XRayRecorder.Internal
{
    public static class RecorderExtensions
    {
#if !NETSTANDARD
        public static void AddWcfInformation(this AWSXRayRecorder recorder, ref Message message, IClientChannel channel, InstanceContext context)
        {
            recorder.AddAnnotation("Service", context.Host.Description.ServiceType.FullName);

            recorder.AddAnnotation("Action", message.Headers.Action);

            if (message.Headers.MessageId != null)
            {
                recorder.AddAnnotation("MessageId", message.Headers.MessageId.ToString());
            }
        }
#endif
    }
}