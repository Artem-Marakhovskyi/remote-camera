using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteCamera.HubClient
{
    public class ConnectionSignalsHandlerBase : IConnectionSignalsHandler
    {
        public ConnectionSignalsHandlerBase()
        {
        }

        private Dictionary<Guid, List<PartialDataMessage>> _partialMesages = new Dictionary<Guid, List<PartialDataMessage>>();

        public virtual void OnControlMessageReceived(ControlMessage controlMessage)
        {
        }

        public virtual void OnDataMessageReceived(DataMessage dataMessage)
        {
        }

        public virtual void OnPartialDataMessageReceived(PartialDataMessage dataMessage)
        {
            if (_partialMesages.ContainsKey(dataMessage.PhotoId))
            {
                _partialMesages[dataMessage.PhotoId].Add(dataMessage);
            }
            else
            {
                _partialMesages[dataMessage.PhotoId] = new List<PartialDataMessage>() { dataMessage };
            }

            var list = _partialMesages[dataMessage.PhotoId];
            _partialMesages[dataMessage.PhotoId] = list.OrderBy(x => x.CurrentPartNumber).ToList();
            list = _partialMesages[dataMessage.PhotoId];

            if (list.Count == dataMessage.TotalPartsCount)
            {
                var bytesCount = list.Sum(x => x.Payload.Length);
                var bytes = new byte[bytesCount];
                var writeFrom = 0;
                foreach (var item in list)
                {
                    item.Payload.CopyTo(bytes, writeFrom);
                }

                _partialMesages.Remove(dataMessage.PhotoId);
                OnPartialDataMessageCompleted(bytes, dataMessage.PhotoId.ToString() + ".png");
            }
        }

        public virtual void OnPartialDataMessageCompleted(byte[] bytes, string filename)
        {

        }

        public virtual void OnRcConnected()
        {
        }

        public virtual void OnSessionFinished()
        {
        }

        public virtual void OnTextReceived(string text)
        {
        }

        public virtual void SetInner(IConnectionSignalsHandler inner)
        {
        }
    }
}
