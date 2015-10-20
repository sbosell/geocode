using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lucuma.Libs.Config
{
    public class GeoProviderConfig : Lucuma.Libs.IGeoProviderConfig
    {
        public string UrlPattern { get; set; }
        public string Key { get; set; }
        public string SearchQuery { get; set; }
        public string KeyQuery { get; set; }
        public string UserAgent { get; set; }
        public string KeyRoute { get; set; }
        public string Secret { get; set; }
        public string SecretQuery { get; set; }
        public string ConsumerKey { get; set; }

        public bool OAuthRequired { get; set; }

        public List<string> UrlValues;
        public NameValueCollection QueryString { get; set; }
        

        public string Url {
            get
            {
                return UrlPattern;
            }
            set
            {

            }
        }
        

        public GeoProviderConfig()
        {
            UrlPattern = String.Empty;
            Key = String.Empty;
            UserAgent = String.Empty;
            QueryString = new NameValueCollection();
            Secret = string.Empty;
            SecretQuery = string.Empty;
            OAuthRequired = false;
            SetDefaults();
       
        }

      
       
      

        /*public IGeoProviderConfig SetUrl(string url) {
            return this.seturl(url);
             //return (IGeoProviderConfig) this;
         } 

         public IGeoProviderConfig SetKey(string key)
         {
             Key = key;
             return this;
         }*/

        public virtual void  SetDefaults()
        {
            // required to set default values for the implementation.
            

        }
    }

    public static class ConfigExtensions {
    public static T SetUrl<T>( this T GeoProviderConfig, string url)  where T: GeoProviderConfig 
    {
        GeoProviderConfig.UrlPattern = url;
        return GeoProviderConfig;
    }
    public static T SetKey<T>(this T GeoProviderConfig, string key) where T : GeoProviderConfig
    {
        GeoProviderConfig.Key = key;
        
        return GeoProviderConfig;
    }
    public static T SetConsumerKey<T>(this T GeoProviderConfig, string key) where T : GeoProviderConfig
    {
        GeoProviderConfig.ConsumerKey = key;

        return GeoProviderConfig;
    }

    public static T SetSearchQuery<T>(this T GeoProviderConfig, string searchquery) where T : GeoProviderConfig
    {
        GeoProviderConfig.SearchQuery = searchquery;

        return GeoProviderConfig;
    }
    public static T SetKeyQuery<T>(this T GeoProviderConfig, string keyquery) where T : GeoProviderConfig
    {
        GeoProviderConfig.KeyQuery = keyquery;

        return GeoProviderConfig;
    }
    public static T SetKeyRoute<T>(this T GeoProviderConfig, string keyroute) where T : GeoProviderConfig
    {
        GeoProviderConfig.KeyRoute = keyroute;

        return GeoProviderConfig;
    }
    public static T SetUserAgent<T>(this T GeoProviderConfig, string useragent) where T : GeoProviderConfig
    {
        GeoProviderConfig.UserAgent = useragent;
        return GeoProviderConfig;
    }

    public static T AddQueryString<T>(this T GeoProviderConfig, string querystring, string querystringvalue) where T : GeoProviderConfig
    {
        GeoProviderConfig.QueryString[querystring] = querystringvalue;
        return GeoProviderConfig;
    }

    public static T SetSecret<T>(this T GeoProviderConfig, string secret) where T : GeoProviderConfig
    {
        GeoProviderConfig.Secret = secret;
        return GeoProviderConfig;
    }
    public static T SetSecretQuery<T>(this T GeoProviderConfig, string secretquery) where T : GeoProviderConfig
    {
        GeoProviderConfig.SecretQuery = secretquery;
        return GeoProviderConfig;
    }
   
    public static string ToQueryString(this NameValueCollection source)
    {
        return String.Join("&", source.AllKeys
            .SelectMany(key => source.GetValues(key)
                .Select(value => String.Format("{0}={1}", System.Uri.EscapeDataString(key), System.Uri.EscapeDataString(value))))
            .ToArray());
    }

    public static string ToQueryString(this NameValueCollection source, bool UrlEncode)
    {
        if (UrlEncode)
        {
            return String.Join("&", source.AllKeys
                .SelectMany(key => source.GetValues(key)
                    .Select(value => String.Format("{0}={1}", System.Uri.EscapeDataString(key), System.Uri.EscapeDataString(value))))
                .ToArray());
        }
        else
        {
            return String.Join("&", source.AllKeys
               .SelectMany(key => source.GetValues(key)
                   .Select(value => String.Format("{0}={1}",key,value)))
               .ToArray());
        }
    }

    }

}
