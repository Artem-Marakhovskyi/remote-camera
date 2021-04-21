using System;
using System.Collections.Generic;
using System.Linq;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient
{
    public class ConnectionSignalsHandlerBase : IConnectionSignalsHandler
    {
        public ConnectionSignalsHandlerBase(ILogger logger)
        {
            _logger = logger;
        }

        private Dictionary<Guid, List<PartialDataMessage>> _partialMesages = new Dictionary<Guid, List<PartialDataMessage>>();
        private readonly ILogger _logger;

        public virtual void OnControlMessageReceived(ControlMessage controlMessage)
        {
            _logger.LogInfo($"Control message received: {controlMessage}");
        }

        public virtual void OnDataMessageReceived(DataMessage dataMessage)
        {
            _logger.LogInfo($"Data message received: {dataMessage}");
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
                    writeFrom += item.Payload.Length;
                }

                _partialMesages.Remove(dataMessage.PhotoId);
                OnPartialDataMessageCompleted(bytes, dataMessage.PhotoId.ToString() + ".png");
            }
        }

        public virtual void OnPartialDataMessageCompleted(byte[] bytes, string filename)
        {
            _logger.LogInfo($"On partial data message completed: length - {bytes.Length}, filename: {filename}");
        }

        public virtual void OnRcConnected()
        {
            _logger.LogInfo($"RC connected");
        }

        public virtual void OnSessionFinished()
        {
            _logger.LogInfo($"Session finished");
        }

        public virtual void OnTextReceived(string text)
        {
            _logger.LogInfo($"Text received: {text}");
        }

        public virtual void SetInner(IConnectionSignalsHandler inner)
        {
            _logger.LogInfo($"Inner set");
        }
    }
}
