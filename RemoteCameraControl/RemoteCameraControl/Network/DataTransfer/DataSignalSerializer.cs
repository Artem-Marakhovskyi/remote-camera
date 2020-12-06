using System;
using Newtonsoft.Json;

namespace RemoteCameraControl.Network.DataTransfer
{
    public static class DataSignalSerializer
    {
        public static byte[] ToBytes(DataSignal dataSignal)
        {
            var json = JsonConvert.SerializeObject(dataSignal);
            var payload = EnvironmentService.GetBytes(json);
            var wrappedBytes = new byte[json.Length + SignalStore.StartMark.Length + SignalStore.EndMark.Length];

            SignalStore.StartMark.CopyTo(wrappedBytes, 0);
            payload.CopyTo(wrappedBytes, SignalStore.StartMark.Length);
            SignalStore.EndMark.CopyTo(wrappedBytes, SignalStore.StartMark.Length + payload.Length);

            return wrappedBytes;
        }

        public static DataSignal ToSignal(byte[] bytes)
        {
            var wrapper = EnvironmentService.GetString(bytes);

            var payloadJson = wrapper.Substring(SignalStore.StartText.Length, wrapper.Length - SignalStore.StartText.Length - SignalStore.EndText.Length);
            var dataSignal = JsonConvert.DeserializeObject<DataSignal>(payloadJson);

            return dataSignal;
        }
    }
}
