using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucuma.Libs.Config;
using Lucuma.Helper;

namespace Lucuma.Libs
{
    public class GeoProvider 
    {
        public IGeoProviderConfig Config { get; set; }

        
        public string BuildSearchQuery(string search)
        {
            string url = Config.Url;
            Config.QueryString[Config.SearchQuery] = search;
            string param;
            Uri uri;
            
            url = !Config.Url.Contains("?") ? Config.Url + "?" : Config.Url + "&";
            // we don't url encode if using oauth
            url = url + Config.QueryString.ToQueryString(!Config.OAuthRequired);

          
                if (!String.IsNullOrEmpty(Config.Key) && !String.IsNullOrEmpty(Config.KeyQuery))
                {
                    // api keys shouldn't be encoded
                    url = string.Format("{0}&{1}={2}", url, Config.KeyQuery, Config.Key);
                }

                if (!String.IsNullOrEmpty(Config.KeyRoute) && !String.IsNullOrEmpty(Config.Key))
                {
                    url = url.Replace("{" + Config.KeyRoute + "}", Config.Key);
                }

            
            if (Config.OAuthRequired)
            {
                var oAuth = new Lucuma.Helper.OAuthBase();
                var nonce = oAuth.GenerateNonce();
                var timeStamp = oAuth.GenerateTimeStamp();
                uri = new Uri(url);
                var signature = oAuth.GenerateSignature(uri, Config.ConsumerKey,
                Config.Secret, string.Empty, string.Empty, "GET", timeStamp, nonce,
                OAuthBase.SignatureTypes.HMACSHA1, out url, out param);

                url = string.Format("{0}?{1}&oauth_signature={2}", url, param, signature);
            }

            return url;
        }
       

    }
}
