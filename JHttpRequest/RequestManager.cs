using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using JHttpRequest.Utilities.Definitions;
namespace JHttpRequest
{

    //Generates The request Stream
    /// <summary>
    /// Generates the settings for the Request
    /// </summary>
    public class RequestGenerator
    {
        
        Uri Url;
        NameValueCollection Headers;
        ReqType RequestType;
        httpScheme RequestScheme;
        UInt16 port=0;
        string HTTPver= "HTTP/1.0";


        string postdata=null;
        bool lastsetpost=false;
        

        /// <summary>
        /// Used for the request string
        /// </summary>
        /// <param name="url"></param>
        public RequestGenerator(Uri url, ReqType Type)
        {
            Url = url; RequestType = Type;
            Headers = new NameValueCollection();
            switch (Url.Scheme)
            {
                case "http":
                    port = 80;
                    RequestScheme = httpScheme.http;
                    break;
                case "https":
                    port = 443;
                    RequestScheme = httpScheme.https;
                    break;
                default:
                    throw new Exception("Unsupported Scheme!");
            }
        }
        public void setRequestHTTPVer(string setVer)
        {
            this.HTTPver = setVer;
        }
        public void setRequestPort(UInt16 reqPort)
        {
            port = reqPort;
        }
        public void setRequestScheme(httpScheme reqScheme)
        {
            RequestScheme = reqScheme;
        }
        public void setHeader(string key,string val)
        {
            if (!Headers.AllKeys.Contains(key))
                Headers.Add(key, val);
            else
                Headers["key"] = val;
        }
        public void setPost(string postdat)
        {
            lastsetpost = true;
            postdata = postdat;
        }
        public void clearPost()
        {
            postdata = null;
        }
        public void addPost(string key,string val)
        {
            if (lastsetpost)
                postdata = null;
            lastsetpost = false;
            //System.Uri.EscapeDataString() 
            key = System.Uri.EscapeDataString(key);
            val = Uri.EscapeDataString(val);
            //key = System.Web.HttpUtility.UrlEncode(key);
            //val = System.Web.HttpUtility.UrlEncode(val);

            if (postdata == null)
                postdata = key + "=" + val;
            else
                postdata += "&" + key + "=" + val;
        }
        private void checkStandardHeaders()
        {
            if (!Headers.AllKeys.Contains("User-Agent"))
                setHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 4.0.30319.17929");
            if (!Headers.AllKeys.Contains("Host"))
                setHeader("Host", Url.DnsSafeHost);

        }
        public string GetReqDat()
        {
            checkStandardHeaders();
            string reqHeader = String.Format("{0} {1} {2}{3}", RequestType.ToString(), Url.LocalPath, HTTPver, "\r\n" );
            if (RequestType == ReqType.POST)
            {
                setHeader("Content-Length", Encoding.UTF8.GetByteCount(postdata).ToString());
            }

            foreach (var Header in Headers.AllKeys)
            {
                reqHeader += String.Format("{0}: {1}{2}", Header, Headers[Header], "\r\n");
            }
            reqHeader += "\r\n";
            if (RequestType == ReqType.POST)
            {
                reqHeader +=  postdata  ;
            }
            return reqHeader;
        }
        public string DNSaveHost()
        {
            return Url.DnsSafeHost;
        }
        public httpScheme getHttpScheme()
        {
            return RequestScheme;
        }
        public UInt16 getRequestPort()
        {
            return port;
        }
    }

    class ProxyManager
    {
        proxyInfo proxy;
        ProxyManager(string setProxy, proxyType proxytype = proxyType.Unknown)
        {
            var tmp = setProxy.Split(new[] { ':' });
            if (tmp.Length != 2)
                throw new Exception("INVALID PROXYSTRING");

            IPAddress prIp;
            UInt16 prPort;

            try
            {
                if (!IPAddress.TryParse(tmp[0], out prIp))
                {
                    prIp = Dns.GetHostEntry(tmp[0]).AddressList[0];
                }
            }
            catch
            {
                throw new Exception("IP Missconfigured");
            }
            if (!UInt16.TryParse(tmp[1], out prPort))
                throw new Exception("Proxy Missconfigured");
              
            proxy = new proxyInfo();
        }
    }

    class HeaderParser
    {
        public WebHeaderCollection Headers;
        public int ResponseCode = -1;
        public string ServerVer;
        public string ResponseCodeText;

        public HeaderParser(string input)
        {
            WebHeaderCollection cHeaders = new WebHeaderCollection();
            MatchCollection matches = new Regex("[^\r\n]+").Matches(input.TrimEnd('\r', '\n'));
            for (int n = 1; n < matches.Count; n++)
            {
                string[] strItem = matches[n].Value.Split(new char[] { ':' }, 2);
                if (strItem.Length > 1)
                {
                    if (!strItem[0].Trim().ToLower().Equals("set-cookie"))
                    {
                        cHeaders.Add(strItem[0].Trim(), strItem[1].Trim());
                    }
                    else
                    {
                        cHeaders.Add(strItem[0].Trim(), strItem[1].Trim());
                        // HTTPProtocol.AddHttpCookie(strItem[1].Trim(), cookieCollection);
                    }
                }
            }
            Headers = cHeaders;
            if (matches.Count > 0)
            {
                try
                {
                    string[] firstLine = matches[0].Value.Split(new[] {' '});
                    ServerVer = firstLine[0];
                    ResponseCode = Int32.Parse(firstLine[1]);
                    for (int i = 2; i < firstLine.Length; i++)
                    {
                        if(i==firstLine.Length-1)
                            ResponseCodeText += firstLine[i];
                        else
                            ResponseCodeText += firstLine[i]+" ";
                    }
                }
                catch (Exception ex)
                {
                   
                }
            }
          
        }

    }
}
