using System;
using System.Configuration;
using System.Web.Configuration;
using FCMMySQLBusinessLibrary;

namespace MackkadoITFramework.Utils
{
    public class ConnString
    {
        private static string connectionString;
        private static string connectionStringServer;
        private static string connectionStringFramework;
        private static string connectionStringLocal;
        private static string connectionStringMySql;
        private static string connectionStringODBC;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = XmlConfig.Read(MakConstant.ConfigXml.ConnectionStringMySql);
                    
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

        public static string ConnectionStringODBC
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringODBC))
                {
                    connectionString = XmlConfig.Read(MakConstant.ConfigXml.ConnectionStringODBC);

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
                    connectionStringServer = XmlConfig.Read(MakConstant.ConfigXml.ConnectionStringServer);
                }

                return connectionStringServer;
            }
            set
            {
                connectionStringServer = value;
            }
        }

        public static string ConnectionStringFramework
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringFramework))
                {
                    connectionStringServer = XmlConfig.Read(MakConstant.ConfigXml.ConnectionStringFramework);
                }

                if (string.IsNullOrEmpty(connectionStringFramework))
                {

                    string webconstring = GetConnectionStringFromWebConfig("makkframework");
                    if (!(string.IsNullOrEmpty(webconstring) || webconstring == "Empty"))   
                    {
                        return webconstring;
                    }
                }

                return connectionStringFramework;
            }
            set
            {
                connectionStringFramework = value;
            }
        }

        public static string ConnectionStringLocal
        {
            get
            {
                if (string.IsNullOrEmpty(connectionStringLocal))
                {
                    connectionStringLocal = XmlConfig.Read(MakConstant.ConfigXml.ConnectionStringLocal);
                }

                return connectionStringLocal;
            }
            set
            {
                connectionStringLocal = value;
            }
        }

        public static string GetConnectionStringFromWebConfig(string csapp = "mainapplication")
        {
            Configuration rootWebConfig = WebConfigurationManager.OpenWebConfiguration("/MyWebSiteRoot");

            string connstring = "Not found";
            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count > 0)
            {
                ConnectionStringSettings connString = rootWebConfig.ConnectionStrings.ConnectionStrings[csapp];
                if (connString != null)
                    connstring = connString.ConnectionString;
                else
                    connstring = "Empty";
            }

            return connstring;
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
