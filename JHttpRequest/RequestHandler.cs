using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using JHttpRequest.Utilities.Definitions;

namespace JHttpRequest
{
   public class RequestHandler
    {
        public bool proxy_nconnect_https_luckymode=false;
      

        public proxyInfo proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        private proxyInfo _proxy = null;

        public HttpResult Execute(RequestGenerator requestData)
        {
            HttpResult result = new HttpResult();

           
            ProxyMethods pm = new ProxyMethods();
            if(_proxy != null)
            {
                if(requestData.getHttpScheme() == httpScheme.http)
                {
                    if (_proxy.type == proxyType.http)
                    {
                        //Direct Connect is allowed
                        pm.ProxyDirect(_proxy.ip.ToString(), _proxy.port);
                    }else if (_proxy.type == (proxyType.httpCONNECT))
                    {
                        //ONLY HTTP CONNECT() ALLOWED
                        pm.ProxyConnect(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else if (_proxy.type == proxyType.socks5)
                    { //YIPPEEEY SOCKS5 :)
                        pm.Socks5Conn(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else if (_proxy.type == proxyType.socks4a)
                    {
                        pm.Socks4aConn(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else if (_proxy.type == proxyType.socks4)
                    {
                        pm.Socks4Conn(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else
                    {
                        throw new Exception("PROXY_NOT_SUPPORTED exception");
                    }
                }
                else if(requestData.getHttpScheme() == httpScheme.https){
                 if (_proxy.type == proxyType.httpCONNECT || (_proxy.type == proxyType.http && proxy_nconnect_https_luckymode)) //Trying connect anyways
                    {
                        //ONLY HTTP CONNECT() ALLOWED, good enough for https
                        pm.ProxyConnect(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else if (_proxy.type == proxyType.socks5)
                    { //YIPPEEEY SOCKS5 :)
                        pm.Socks5Conn(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else if (_proxy.type == proxyType.socks4a)
                    {
                        pm.Socks4aConn(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else if (_proxy.type == proxyType.socks4)
                    {
                        pm.Socks4Conn(requestData.DNSaveHost(), requestData.getRequestPort(), _proxy.ip.ToString(), _proxy.port);
                    }
                    else
                    {
                        throw new Exception("PROXY_NOT_SUPPORTED",new Exception("This scheme does not support that kind of proxy"));
                    }
                }
            }
            else
            {
                pm.ProxyDirect(requestData.DNSaveHost(), requestData.getRequestPort()); //Direct connect without proxy
            }

          
            switch (requestData.getHttpScheme())
            {
                case httpScheme.http:
                    var HCC = get_http(pm, requestData.GetReqDat());
                    result.Content = HCC.Content;
                    result.rawHeaders = HCC.Header;
                    break;
                case httpScheme.https:
                    var HCCS = get_https(pm, requestData.DNSaveHost(), requestData.GetReqDat());
                    result.Content = HCCS.Content;
                    result.rawHeaders = HCCS.Header;
                    break;
                default:
                    throw new Exception("Unsupported Scheme!");
            }
            

            var headers = new HeaderParser(result.rawHeaders);
            result.Headers = headers.Headers;


            result.ResponseCode = headers.ResponseCode;
            result.ResponseCodeText = headers.ResponseCodeText;
            result.ResponseServerVer = headers.ServerVer;
            headers = null;
            return result;
        }

        private HeaderContentCollection get_http(ProxyMethods pm, string req = null)
        {

            HeaderContentCollection result = new HeaderContentCollection();

            bool isHeader = true;
            using (NetworkStream ns = pm.nsCurr)
            {  
                System.IO.StreamWriter sw = new System.IO.StreamWriter(ns);
                System.IO.StreamReader sr = new System.IO.StreamReader(ns);

                sw.Write(req);
                sw.Flush();
                string tmp;

                while (sr.Peek() >= 0)
                {
                    tmp = sr.ReadLine();
                    if(isHeader)
                    {
                        if(tmp == "")
                        {
                            isHeader = false;
                            continue;
                        }
                        result.Header += tmp + "\r\n";
                    }
                    if (!isHeader)
                    {
                        result.Content += tmp + "\r\n";
                    }
                }
                //Debugger.Break();
            }
            pm.Dispose();
            return result;
        }


        //ProxyMethods pm = new ProxyMethods();
        //var nsa = pm.Socks4Conn(IP, conport, "78.135.114.76", 1080);
        private HeaderContentCollection get_https(ProxyMethods pm,string authority, string req )
        {
            HeaderContentCollection result = new HeaderContentCollection();
            bool isHeader=true;

            byte[] buffer = new byte[2048];
            var request = Encoding.UTF8.GetBytes(req);
                   
            SslStream sslStream = new SslStream(pm.nsCurr);
            sslStream.AuthenticateAsClient(authority);

            sslStream.Write(request, 0, request.Length);
            sslStream.Flush();
            SsLUtils sslStrReadr = new SsLUtils();
            string tmpres;
            while (!sslStrReadr.EoF)
            {
                tmpres  = sslStrReadr.readLine(ref sslStream);
                if(isHeader)
                {
                    if (tmpres == "\r\n")
                    {
                        isHeader = false;
                        continue;
                    }
                    result.Header += tmpres;
                }
                if (!isHeader)
                {
                    result.Content += tmpres;
                } 
            } 
            sslStream.Close();
            pm.Dispose();
            return result;
        }
        private class SsLUtils
        {
            public bool EoF = false;
            public  string readLine(ref SslStream stream)
            {
                bool done = false;
                byte[] undefBuf = new byte[1024];
                UInt32 bufcnt = 0;

                string res;

                int buf = 0;
                while ((buf = stream.ReadByte()) != -1)
                {
                    undefBuf[bufcnt] = Convert.ToByte(buf);
                    switch (undefBuf[bufcnt])
                    {
                        case 10:   //\r = 13 \n=10
                            if (bufcnt - 1 >= 0 && undefBuf[bufcnt - 1] == 13)
                            {
                                done = true;
                            }
                            break;
                        default:
                            break;
                    }
                    bufcnt++;
                    if (done)
                        break;
                    if (bufcnt >= undefBuf.Length - 1)
                    {
                        Array.Resize(ref undefBuf, undefBuf.Length + 1024); //Resize if too big
                    }
                }
                if(buf == -1)
                {
                    EoF = true;
                }
                res = Encoding.UTF8.GetString(undefBuf, 0, Convert.ToInt32(bufcnt));
                return res;
            }
        }
        private static HttpStatusCode? GetStatusCode(string response)
        {
            string rawCode = response.Split(' ').Skip(1).FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(rawCode) && rawCode.Length > 2)
            {
                rawCode = rawCode.Substring(0, 3);

                int code;

                if (int.TryParse(rawCode, out code))
                {
                    return (HttpStatusCode)code;
                }
            }

            return null;
        }
    }
}
