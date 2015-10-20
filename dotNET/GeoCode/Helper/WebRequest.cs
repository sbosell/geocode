using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Lucuma.Helper
{
    public class WebRequester
    {
        public string Uri { get; set; }
        public string UserAgent { get; set; }
        public  WebRequester()
        {
            Uri = String.Empty;
            UserAgent = String.Empty;
        }
        public WebRequester(string url) : this()
        {
            Uri = url;
            
        }
        public WebRequester(string url, string useragent) : this()
        {
            Uri = url;
            UserAgent = useragent;
        }
        
        public string GetData()
        {
            string json = string.Empty;


            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(Uri));

                webRequest.Method = WebRequestMethods.Http.Get;
//                webRequest.ContentType = "application/json; charset=utf-8";
                if (!String.IsNullOrEmpty(UserAgent))
                {

                    webRequest.Headers.Add("UserAgent", UserAgent);

                }
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    if (webResponse.StatusCode== HttpStatusCode.OK )
                    {
                        using (Stream stream = webResponse.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            String responseString = reader.ReadToEnd();
                            json = responseString;
                        }
                    }
                }

           
                return json;
        }
    }
}
