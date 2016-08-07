using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Starksoft.Net.Proxy;
using JHttpRequest;
using JHttpRequest.Utilities;

namespace JHttpRequest
{
    class ProxyMethods
    {
        /*
         Http over Http: 2 Methods:
              1: Tcp.Connect(proxyip,proxyport) -> Normal request
              2: Tcp.Connect(proxyip,proxyport) -> CONNECT

         Https over Http
            DIRECT CONNECT(http/s proxy) && HTTPS DOESN'T Work
            HoH (2) 
         */

        public void Dispose()
        {
            CloseStream();
            GC.SuppressFinalize(this);
        }
        void _ProxyMethods()
        {
            CloseStream();
        }

        public TcpClient tc;
        public NetworkStream nsCurr;

        public void CloseStream()
        {
            if (nsCurr != null)
                nsCurr.Close();
            if (tc != null)
                tc.Close();
            nsCurr = null;
            tc = null;
        }

        public NetworkStream ProxyConnect(string dstHost, UInt16 dstPort, //dstPort=443||80
        string proxyHost, int proxyPort)
        {
            if (tc != null)
                throw new Exception("ProxyClient Already in use");

            var CTt = new CTcpConnect(proxyHost, proxyPort, Settings.ConnectTimeout); //same as tcp.connect but with timeout
            tc = CTt.Connect();

            NetworkStream nsa = tc.GetStream();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(nsa);
            System.IO.StreamReader sr = new System.IO.StreamReader(nsa);

            sw.Write(String.Format("CONNECT {0}:{1} HTTP/1.1\r\nHost: {0}\r\n\r\n", dstHost,dstPort));
            sw.Flush();
            var result0 = sr.ReadLine();
            var ressplit = result0.Split(new[] {' '});

            if(ressplit.Length > 1 && ressplit[1] == "200")
            {
                nsCurr = nsa;
                return nsa;
            }
            try
            {
                result0 += sr.ReadToEnd();
            }catch{}
            sr.Close();
            sw.Close();
            nsa.Close();
            tc.Close();
            throw new Exception("CONNECT Refused", new Exception(result0));
        }
        /// <summary>
        /// Can also be used for direct connect
        /// </summary>
        /// <param name="proxyHost"></param>
        /// <param name="proxyPort"></param>
        /// <returns></returns>
        public NetworkStream ProxyDirect(string proxyHost , int proxyPort )
        {
            if (tc != null)
                throw new Exception("ProxyClient Already in use");

            var CTt = new CTcpConnect(proxyHost, proxyPort, Settings.ConnectTimeout); //same as tcp.connect but with timeout
            tc = CTt.Connect();

            NetworkStream nsa = tc.GetStream();
            nsCurr = nsa;
            return nsa;
        } 
        public NetworkStream Socks5Conn(string dstHost, UInt16 dstPort,string proxyHost , int proxyPort )
        {
            return StarkSoftConnectoar(dstHost, dstPort, proxyHost, proxyPort, ProxyType.Socks5);
        }
        public NetworkStream Socks4Conn(string dstHost, UInt16 dstPort, string proxyHost , int proxyPort)
        {
            return StarkSoftConnectoar(dstHost, dstPort, proxyHost, proxyPort, ProxyType.Socks4);
        }
        public NetworkStream Socks4aConn(string dstHost, UInt16 dstPort, string proxyHost , int proxyPort)
        {
            return StarkSoftConnectoar(dstHost,  dstPort,  proxyHost,  proxyPort,ProxyType.Socks4a);
        }
        private NetworkStream StarkSoftConnectoar( string dstHost, UInt16 dstPort, string proxyHost, int proxyPort, ProxyType Proxytype)
        {
            if (tc != null)
                throw new Exception("ProxyClient Already in use");

            ProxyClientFactory factory = new ProxyClientFactory();
            IProxyClient proxy = factory.CreateProxyClient(Proxytype, proxyHost, proxyPort);
            tc = proxy.CreateConnection(dstHost, dstPort);
            NetworkStream nsa = tc.GetStream();
            nsCurr = nsa;
            return nsa;
        }
    }
}
