using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary
{
    public class ConnString
    {
        private static string connectionString;
        private static string connectionStringServer;
        private static string connectionStringLocal;
        private static string connectionStringMySql;

        public  static string ConnectionStringMySql
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringMySql))
                {
                    connectionStringMySql = FCMXmlConfig.Read(FCMConstant.fcmConfigXml.ConnectionStringMySql);
                }

                return connectionStringMySql;
            }
            set
            {
                connectionStringMySql = value;
            }
        }

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    //connectionString =
                    //"Data Source=LAPTOPHP;" +
                    //"Initial Catalog=management;" +
                    //"Persist Security info=false;" +
                    //"integrated security=sspi;";

                    //connectionString =
                    //"Data Source=LAPTOPHP;" +
                    //"Initial Catalog=management;" +
                    //"User ID=servicefcm;Password=servicefcm;";

                    //connectionString =
                    //"Data Source=DESKTOPHP\\SQLEXPRESS;" +
                    //"Initial Catalog=management;" +
                    //"User ID=service_fcm;Password=service_fcm;";

                    // 1) Connection String
                    //
                    connectionString = FCMXmlConfig.Read(FCMConstant.fcmConfigXml.ConnectionString);
                    
                    //"Persist Security info=false;" +
                    //"integrated security=sspi;";

                }

                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public static string ConnectionStringServer
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringServer))
                {
                    connectionStringServer = FCMXmlConfig.Read(FCMConstant.fcmConfigXml.ConnectionStringServer);
                }

                return connectionStringServer;
            }
            set
            {
                connectionStringServer = value;
            }
        }

        public static string ConnectionStringLocal
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringLocal))
                {
                    connectionStringLocal = FCMXmlConfig.Read(FCMConstant.fcmConfigXml.ConnectionStringLocal);
                }

                return connectionStringLocal;
            }
            set
            {
                connectionStringLocal = value;
            }
        }


        private static string Toshiba
        {
            get 
            { 
                return
                    "Data Source=TOSHIBAPC\\SQLEXPRESS;" +
                    "Initial Catalog=management;" +
                    "Persist Security info=false;" +
                    "integrated security=sspi;";

            }
        }

        private static string DeanPC
        {
            get
            {
                return
                    "Data Source=DEAN-PC\\SQLEXPRESS;" +
                    "Initial Catalog=management;" +
                    "Persist Security info=false;" +
                    "integrated security=sspi;";

            }
        }

        private static string HPLaptop
        {
            get
            {
                return
                    "Data Source=HPLAPTOP\\SQLEXPRESS;" +
                    "Initial Catalog=management;" +
                    "Persist Security info=false;" +
                    "integrated security=sspi;";

            }
        }

        private static string Desktop
        {
            get
            {
                return
                    "Data Source=DESKTOHP\\SQLEXPRESS;" +
                    "Initial Catalog=management;" +
                    "Persist Security info=false;" +
                    "integrated security=sspi;";

            }
        }

        private static string HPMINI
        {
            get
            {
                return
                    "Data Source=HPMINI\\SQLEXPRESS;" +
                    "Initial Catalog=management;" +
                    "Persist Security info=false;" +
                    "integrated security=sspi;";

            }
        }


    }
}
