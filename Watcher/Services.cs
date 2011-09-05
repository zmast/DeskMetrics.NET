// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Services.cs                                     //
//     Copyright (c) 2010-2011 DeskMetrics Limited                       //
//                                                                       //
//     http://deskmetrics.com                                            //
//     http://support.deskmetrics.com                                    //
//                                                                       //
//     support@deskmetrics.com                                           //
//                                                                       //
//     This code is provided under the DeskMetrics Modified BSD License  //
//     A copy of this license has been distributed in a file called      //
//     LICENSE with this source code.                                    //
//                                                                       //
// **********************************************************************//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace DeskMetrics
{
    public class Services
    {
        /// <summary>
        /// The proxy host
        /// </summary>
        public string ProxyHost { get; set; }

        /// <summary>
        /// The proxy username
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// The proxy password
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// The proxy port
        /// </summary>
        public int ProxyPort { get; set; }

        /// <summary>
        /// The server to post data to
        /// </summary>
        public string PostServer { get; set; }
        
        /// <summary>
        /// The server port to post data to
        /// </summary>
        public int PostPort { get; set; }

        /// <summary>
        /// The data post timeout (in milliseconds)
        /// </summary>
        public int PostTimeOut { get; set; }
        

        /// <summary>
        /// Main constructor
        /// </summary>
        internal Services()
        {
            PostServer = Settings.DefaultServer;
            PostPort = Settings.DefaultPort;
            PostTimeOut = Settings.DefaultTimeout;
        }
        
        /// <summary>
        /// Posts a json request to the webservice
        /// </summary>
        /// <param name="endpoint">The webservice endpoint</param>
        /// <param name="json">the json data array content (excluded the [ ])</param>
        internal void PostData(string applicationId, string json)
        {
            string url;

            if (PostPort == 443)
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslError)
                    {
                        bool validationResult = true;
                        return validationResult;
                    };

                url = "https://" + applicationId + "." + PostServer + Settings.ApiEndpoint;
            }
            else
            {
                url = "http://" + applicationId + "." + PostServer + Settings.ApiEndpoint;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = PostTimeOut;

            if (!string.IsNullOrEmpty(ProxyHost))
            {
                string uri;

                WebProxy myProxy = new WebProxy();

                if (ProxyPort != 0)
                    uri = ProxyHost + ":" + ProxyPort;
                else
                    uri = ProxyHost;

                myProxy.Address = new Uri(uri);
                myProxy.Credentials = new NetworkCredential(ProxyUserName, ProxyPassword);
                request.Proxy = myProxy;
            }
            else
            {
                request.Proxy = WebRequest.DefaultWebProxy;
            }

            request.UserAgent = Settings.UserAgent;
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes("data=[" + json + "]");

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                string responseText = streamReader.ReadToEnd();
                Debug.WriteLine(responseText);
            }
        }
    }
}
