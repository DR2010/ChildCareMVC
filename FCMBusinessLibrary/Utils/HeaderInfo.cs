using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary
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
            get
            {
                if (instance == null)
                {
                    instance = new HeaderInfo();
                }
                return instance;
            }
        }
    }

}
