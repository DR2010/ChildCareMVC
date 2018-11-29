using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace fcm
{
    public class DataColector
    {
        public static string Get( ReportMetadata metadata )
        {

            string ret = "";

            string sql = "";

            switch (metadata.TableName.ToUpper())
            {
                case "CLIENT" :

                    Client.ReadFieldClient(metadata.FieldName.ToUpper(), Utils.ClientID);
                    break;


                case "EMPLOYEE" :
                    //  Write in Employee Class
                    break;
            }
 
            return ret;
        }
    }
}
