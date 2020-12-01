using System;
using System.Collections.Generic;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Hub
{
    public class ControlSignalHub : IControlSignalPublisher, IControlSignalHubManager
    {
        private List<IControlSignalListener> _controlSignalListeners = new List<IControlSignalListener>();

        public void AddListener(IControlSignalListener controlSignalListener)
        {
            _controlSignalListeners.Add(controlSignalListener);
        }

        public void RemoveListener(IControlSignalListener controlSignalListener)
        {
            _controlSignalListeners.Remove(controlSignalListener);
        }

        public void PublishControlSignal(ControlSignal controlSignal)
        {
            foreach (var listener in _controlSignalListeners)
            {
                listener.OnControlSignalReceived(controlSignal);
            }
        }
    }
}
