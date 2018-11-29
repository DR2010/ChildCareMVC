using System.Configuration;
using System.Web.Configuration;
using System.Xml;

namespace MackkadoITFramework.Utils
{
    public class XmlConfig
    {
        public static string Read( string attribute )
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( "C:\\Program Files\\FCM\\FCMConfig.xml" );

            XmlTextReader textReader = new XmlTextReader( "C:\\Program Files\\FCM\\FCMConfig.xml" );
            string constring = "";
            string pickNext = "N";

            while ( textReader.Read() )
            {
                // Move to first element
                textReader.MoveToNextAttribute();
                if ( pickNext == "Y" )
                {
                    constring = textReader.Value;
                    constring = constring.Replace( System.Environment.NewLine, string.Empty );
                    constring = constring.TrimStart();
                    constring = constring.TrimEnd();

                    break;
                }
                if ( textReader.Name == attribute )
                {
                    pickNext = "Y";
                }
            }

            return constring;

        }

        public static string ReadLocal( string attribute )
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( "FCMLocalConfig.xml" );

            XmlTextReader textReader = new XmlTextReader( "FCMLocalConfig.xml" );
            string constring = "";
            string pickNext = "N";

            while ( textReader.Read() )
            {
                // Move to first element
                textReader.MoveToNextAttribute();
                if ( pickNext == "Y" )
                {
                    constring = textReader.Value;
                    constring = constring.Replace( System.Environment.NewLine, string.Empty );
                    constring = constring.TrimStart();
                    constring = constring.TrimEnd();

                    break;
                }
                if ( textReader.Name == attribute )
                {
                    pickNext = "Y";
                }
            }

            return constring;

        }

        public static string GUFCRead(string attribute)
        {
            string filelocation = "C:\\Program Files\\GUFC\\GUFCConfig.xml";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filelocation);

            XmlTextReader textReader = new XmlTextReader(filelocation);
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
