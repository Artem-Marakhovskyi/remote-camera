using System;
using Newtonsoft.Json;

namespace RemoteCameraControl.Network.DataTransfer
{
    public class ControlSignalSerializer
    {
        public static byte[] ToBytes(ControlSignal controlSignal)
        {
            var json = JsonConvert.SerializeObject(controlSignal);
            var payload = EnvironmentService.GetBytes(json);
            var wrappedBytes = new byte[json.Length + SignalStore.StartMark.Length + SignalStore.EndMark.Length];

            SignalStore.StartMark.CopyTo(wrappedBytes, 0);
            payload.CopyTo(wrappedBytes, 2);
            SignalStore.EndMark.CopyTo(wrappedBytes, wrappedBytes.Length - SignalStore.EndMark.Length);

            return wrappedBytes;
        }

        public static ControlSignal ToSignal(byte[] bytes)
        {
            var wrapper = EnvironmentService.GetString(bytes);

            var payloadJson = wrapper.Substring(SignalStore.StartText.Length, wrapper.Length - SignalStore.StartText.Length - SignalStore.EndText.Length);
            var controlSignal = (ControlSignal)JsonConvert.DeserializeObject(payloadJson);

            return controlSignal;
        }
    }
}
