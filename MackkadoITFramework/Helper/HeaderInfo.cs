using System;

namespace MackkadoITFramework.Utils
{
    public class HeaderInfo
    {

        private static HeaderInfo instance;

        public string UserID;
        public string Passcode; // encrypted password
        public DateTime CurrentDateTime;

        private HeaderInfo() { }

        public static HeaderInfo Instance
        {
            get { return instance ?? (instance = new HeaderInfo()); }
        }
    }

}
