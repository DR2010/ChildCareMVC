using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MackkadoITFramework.Utils;
using System.Configuration;

namespace MackkadoITFramework.Helper
{
    public class WebAPIHelper
    {
        private static string gufcWebAPIURI;
        private static string gufcConnectionString;

        public static string GUFCWebAPIURI
        {
            get
            {
                if (string.IsNullOrEmpty(gufcWebAPIURI))
                {
                    // gufcWebAPIURI = XmlConfig.GUFCRead(MakConstant.ConfigXml.GUFCWebAPIURI);

                    gufcWebAPIURI = ConfigurationManager.AppSettings["gufcapiuri"];

                }

                return gufcWebAPIURI;
            }
            set
            {
                gufcWebAPIURI = value;
            }
        }

        public static string GUFCConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(gufcWebAPIURI))
                {
                    gufcWebAPIURI = XmlConfig.GUFCRead(MakConstant.ConfigXml.GUFCConnectionString);

                }

                return gufcWebAPIURI;
            }
            set
            {
                gufcWebAPIURI = value;
            }
        }

    }
}
