using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace JHttpRequest.Utilities.Definitions
{
    public enum ReqType
    {
        GET, //GET /docs/index.html HTTP/1.1
        POST, //POST /account/auth HTTP/1.1
        HEAD
    }
    public enum httpScheme
    {
        http,
        https,
    }
    public enum proxyType
    {
        http,
        httpCONNECT,
        socks4,
        socks4a,
        socks5,
        None,
        Unknown,
    }
    public class proxyInfo
    {
        public IPAddress ip;
        public UInt16 port;
        public proxyType type;
    }
    public class HeaderContentCollection
    {
        public string Header;
        public string Content;
    }
    public class HttpResult
    {
        public WebHeaderCollection Headers;
        public int ResponseCode = -1;
        public string ResponseServerVer;
        public string ResponseCodeText;

        public string rawHeaders;
        public string Content;

    }
}
