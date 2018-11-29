using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FCMBusinessLibrary
{
    public class FCMXmlConfig
    {
        public static string Read(string attribute)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("C:\\Program Files\\FCM\\FCMConfig.xml");

            XmlTextReader textReader = new XmlTextReader("C:\\Program Files\\FCM\\FCMConfig.xml");
            string constring = "";
            string pickNext = "N";

            while (textReader.Read())
            {
                // Move to first element
                textReader.MoveToNextAttribute();
                if (pickNext == "Y")
                {
                    constring = textReader.Value;
                    constring = constring.Replace(System.Environment.NewLine, string.Empty);
                    constring = constring.TrimStart();
                    constring = constring.TrimEnd();

                    break;
                }
                if (textReader.Name == attribute)
                {
                    pickNext = "Y";
                }
            }

            return constring;

        }
    }
}
