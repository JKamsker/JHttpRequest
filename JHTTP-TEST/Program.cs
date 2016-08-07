using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using JHttpRequest;
using JHttpRequest.Utilities;
using JHttpRequest.Utilities.Definitions;
using System.Net;

namespace JHTTP_TEST
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Theorie:
                    reqestGenerator generiert den request Header string und gibt ihn an den reqMaker weiter, welcher den request Ausführt
                    reqestGenerator generates the request Header string and forwards it to the reqMaker, which executes the request
            */
            RequestHandler reqMaker = new RequestHandler();
            /*
                    ReqType.GET For GET requests
                    ReqType.POST For Post requests

                    The RequestGenerator automatically detects wether the url is http or https, the reqMaker automatically handles the request
             */
            RequestGenerator reqestGenerator = new RequestGenerator(new Uri("http://j-kit.de/"), ReqType.GET); //ReqType.GET for GET requests

            //req.setRequestHTTPVer("HTTP/1.1"); Custom HttpVersion if needed, standard is HTTP/1.0

            reqestGenerator.setHeader("Accept", "*/*");
            reqestGenerator.setHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 4.0.30319.17929");
            reqestGenerator.setHeader("Content-Type", "application/x-www-form-urlencoded");
            // reqestGenerator.addPost("setYo", "HeyIAmAPost"); // Add the post variables if needed
            //reqestGenerator.setPost("MyPost=Cool");           //Overwrites previous addPost & Setpost (no urlEscapeStrings!!)
            Console.WriteLine(reqestGenerator.GetReqDat());
           
            //Set the proxy for this request (OPTIONAL)
            proxyInfo proxy = new proxyInfo();
            proxy.ip = IPAddress.Parse("5.159.232.129");
            proxy.port = 60088;
            proxy.type = proxyType.socks5;
            reqMaker.proxy = proxy;
            //Some debugging info...
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
