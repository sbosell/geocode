using System;
using System.Collections.Specialized;

namespace Lucuma.Libs
{
    public interface IGeoProviderConfig
    {
        void SetDefaults();
//        IGeoProviderConfig SetKey(string key);
//      IGeoProviderConfig SetUrl(string url);
        
        // api key value
        string Key {get;set;}
        // url to call
        string Url { get; set; }
        // search query string builder
        string SearchQuery { get; set; }
        // key query string builder
        string KeyQuery { get; set; }
        // user agent for web call
        string KeyRoute { get; set; }

        // oauth secret
        string Secret { get; set; }
        // oath secret querystring param
        string SecretQuery { get; set; }
        // useragent for request
        string UserAgent { get; set; }
        // requires oath
        bool OAuthRequired { get; set; }
        // query string

        string ConsumerKey { get; set; }
        NameValueCollection QueryString { get; set; }        
    }
}
