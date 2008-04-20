using System;
using System.Net;
using System.Net.Sockets;

namespace YouWebIt
{
    /// <summary>
    /// Thank to Mehran Ghanizadeh : http://www.codeproject.com/KB/vb/TinyWebServer.aspx
    /// </summary>
    public static class PortHelper
    {
        // Methods
        public static bool IsPortAvailable(int iPort)
        {
            TcpListener listener;
            IPAddress[] addressList = Dns.GetHostEntry(Environment.MachineName).AddressList;
            bool isPortAvailable;
            try
            {
                listener = new TcpListener(addressList[0], iPort);
                listener.Start();
                listener.Stop();
                isPortAvailable = true;
            }
            catch
            {
                isPortAvailable = false;
            }
            return isPortAvailable;
        }

        public static int GetRandomPortAvailable()
        {
            Random random = new Random(DateTime.Now.Millisecond);

            int port = random.Next(22000, 22500);
            while (!IsPortAvailable(port))
            {
                port = random.Next(22000, 22500);
            }
            return port;
        }
    }


}
