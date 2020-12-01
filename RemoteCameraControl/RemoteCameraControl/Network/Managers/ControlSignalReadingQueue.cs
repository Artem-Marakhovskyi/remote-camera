using System.Collections.Concurrent;
using System.Text;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Network.Managers
{
    public class DataReadingQueue
    {
        //private byte[] _source = new byte[EndlessStreamReader.ChunkSize];

        //public ConcurrentQueue<ControlSignal> _queue = new ConcurrentQueue<ControlSignal>();

        //public void PutChunk(string chunk)
        //{
        //    lock (_source)
        //    {
        //        _source.Append(chunk);
        //        var controlSignal = GetControlSignal();
        //        _queue.Enqueue(controlSignal);
        //    }
        //}

        //public string GetControlSignal()
        //{
        //    lock (_source)
        //    {
        //        var content = _source.ToString();

        //        if (content.Contains(SignalStore.StartText) && content.Contains(SignalStore.EndText))
        //        {
        //            var startIdx = content.IndexOf(SignalStore.StartText, 0) + SignalStore.StartText.Length;
        //            var endIdx = content.IndexOf(SignalStore.EndText, 0);

        //            return content.Substring(startIdx, endIdx - startIdx);
        //        }

        //        return null;
        //    }
        //}
    }
}
