using System;
using System.Text;

namespace RemoteCameraControl
{
    public static class EnvironmentService
    {
        public static byte[] GetBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static string GetString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
