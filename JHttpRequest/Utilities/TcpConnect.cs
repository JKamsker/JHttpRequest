using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
namespace JHttpRequest.Utilities
{
    class CTcpConnect
    {
        IPAddress Ip;
        Int32 Port;
        TimeSpan Timeout;

        public int? ConnectionTime; 
        public CTcpConnect(string ip, Int32 port, TimeSpan timeout)
        {
            try
            {
                if (!IPAddress.TryParse(ip, out Ip))
                {
                    Ip = Dns.GetHostEntry(ip).AddressList[0];
                }
            }
            catch
            {
                throw new Exception("NOT_CONNECTED");
            }

            Port = port;
            Timeout = timeout;
        }
        public CTcpConnect(IPAddress ip, Int32 port, TimeSpan timeout)
        {
            Ip = ip;
            Port = port;
            Timeout = timeout;
        }
        public TcpClient Connect()
        {
            TcpClient tcp = new TcpClient();
            tcp.ReceiveTimeout = Convert.ToInt32(Timeout.TotalMilliseconds);
            tcp.SendTimeout = Convert.ToInt32(Timeout.TotalMilliseconds);

            Stopwatch clock = Stopwatch.StartNew();

            IAsyncResult ar = tcp.BeginConnect(Ip, Port, null, null);
                System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                try
                {
                    if (!ar.AsyncWaitHandle.WaitOne(Timeout, false))
                    {
                        tcp.Close();
                        // If its a proxy, it should probably be removed from the pool
                        throw new Exception("NOT_CONNECTED", new Exception(Ip.ToString()+":"+ Port.ToString())) ;
                   
                    }

                    tcp.EndConnect(ar);
                }
                finally
                {
                    wh.Close();
                    ConnectionTime = Convert.ToInt32(clock.ElapsedMilliseconds);
                    clock.Stop();
                }

            return tcp;
        }
    }
}
