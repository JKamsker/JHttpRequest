using System;
using System.Diagnostics;
using System.Net;
using JHttpRequest.Utilities.Definitions;

namespace JHttpRequest
{
    class Examples
    {
        void PostReqest()
        {

            /*Theorie:
                    reqestGenerator generiert den request Header string und gibt ihn an den reqMaker weiter, welcher den request Ausführt
             */
            RequestHandler reqMaker = new RequestHandler();
            RequestGenerator reqestGenerator = new RequestGenerator(new Uri("http://www.vb-paradise.de/index.php"), ReqType.GET); //ReqType.GET für GET requests

            //req.setRequestHTTPVer("HTTP/1.1"); Custom HttpVersion, standard is HTTP/1.0

            reqestGenerator.setHeader("Accept", "*/*");
            reqestGenerator.setHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 4.0.30319.17929");
            reqestGenerator.setHeader("Content-Type", "application/x-www-form-urlencoded");
           // reqestGenerator.addPost("setYo", "HeyIAmAPost");
            Console.WriteLine(reqestGenerator.GetReqDat());
         //   reqestGenerator.setPost("MyPost=Cool"); //Überschreibt vorherige addPost & Setpost
            //Setze den request proxy (OPTIONAL)
            proxyInfo proxy = new proxyInfo();
            proxy.ip = IPAddress.Parse("5.159.232.129");
            proxy.port = 60088;
            proxy.type =  proxyType.socks5;
            reqMaker.proxy = proxy;

            //Hier ein wenig Debug info
            HttpResult result = reqMaker.Execute(reqestGenerator);
            Console.WriteLine("-----Result Header-----");
            Console.WriteLine("ResponseCode: " + result.ResponseCode);
            Console.WriteLine("ResponseCodeText: " + result.ResponseCodeText);
            Console.WriteLine("Server Http Version: " + result.ResponseServerVer);

            foreach (var item in result.Headers.AllKeys)
            {
                Console.WriteLine(item + ": " + result.Headers[item]);
            }
            Console.WriteLine("-----Result Content-----");
            Console.Write(result.Content);
            Console.ReadLine();

            Debugger.Break();

        }
    }
}
