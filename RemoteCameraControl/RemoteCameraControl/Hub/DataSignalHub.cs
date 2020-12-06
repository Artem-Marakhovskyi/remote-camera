using System;
using System.Collections.Generic;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Hub
{
    public class DataSignalHub : IDataSignalHubManager, IDataSignalPublisher
    {
        private List<IDataSignalListener> _dataSignalListeners = new List<IDataSignalListener>();

        public void AddListener(IDataSignalListener dataSignalListener)
        {
            _dataSignalListeners.Add(dataSignalListener);
        }

        public void RemoveListener(IDataSignalListener dataSignalListener)
        {
            _dataSignalListeners.Remove(dataSignalListener);
        }

        public void PublishDataSignal(DataSignal dataSignal)
        {
            foreach (var listener in _dataSignalListeners)
            {
                listener.OnDataSignalReceived(dataSignal);
            }
        }
    }
}
