namespace MackkadoITFramework.Utils
{
    /// <summary>
    /// Class with every constant used in the system
    /// </summary>
    public class MakConstant
    {
        /// <summary>
        /// It represents the attribute for the database ConnectionString 
        /// </summary>
        public class ConfigXml
        {
            public static string ConnectionString = "ConnectionString";
            public static string ConnectionStringMySql = "ConnectionStringMySql";
            public static string ConnectionStringServer = "ConnectionStringServer";
            public static string ConnectionStringLocal = "ConnectionStringLocal";
            public static string ConnectionStringODBC = "ConnectionStringODBC";
            public static string ConnectionStringFramework = "ConnectionStringFramework";
            public static string LocalAssemblyFolder = "LocalAssemblyFolder";
            public static string ServerAssemblyFolder = "ServerAssemblyFolder";
            public static string EnableLocalDB = "EnableLocalDB";
            public static string EnableServerDB = "EnableServerDB";
            public static string DefaultDB = "DefaultDB";
            public static string AuditLogPath = "AuditLogPath";
            public static string StopGeneration = "StopGeneration";
            public static string GUFCWebAPIURI = "GUFCWebAPIURI";
            public static string GUFCConnectionString = "GUFCConnectionString ";

        }

        /// <summary>
        /// It represents the attribute for the database ConnectionString 
        /// </summary>
        public enum DataBaseType
        {
            SQLSERVER, 
            MYSQL
        }

        
        /// <summary>
        /// Integer represent the sequence of the image on the Image List.
        /// </summary>
        public struct Image
        {
            public const int Selected = 0;
            public const int Document = 1;
            public const int Folder = 2;
            public const int Client = 3;
            public const int Appendix = 4;
            public const int Excel = 5;
            public const int PDF = 6;
            public const int Undefined = 7;
            public const int Checked = 8;
            public const int Unchecked = 9;

            public const int Word32 = 10;
            public const int WordFileExists32 = 11;
            public const int WordFileNotFound32 = 12;

            public const int WordFileSourceNoDestinationNo = 13;
            public const int WordFileSourceNoDestinationYes = 14;
            public const int WordFileSourceYesDestinationNo = 15;
            public const int WordFileSourceYesDestinationYes = 16;

            public const int ExcelFileSourceNoDestinationNo = 17;
            public const int ExcelFileSourceNoDestinationYes = 18;
            public const int ExcelFileSourceYesDestinationNo = 19;
            public const int ExcelFileSourceYesDestinationYes = 20;

            public const int PDFFileSourceNoDestinationNo = 21;
            public const int PDFFileSourceNoDestinationYes = 22;
            public const int PDFFileSourceYesDestinationNo = 23;
            public const int PDFFileSourceYesDestinationYes = 24;

        }

        /// <summary>
        /// This is the name of the image file representing the file type
        /// </summary>
        public struct ImageFileName
        {
            public const string Selected = "ImageSelected.jpg";
            public const string Document = "ImageWordDocument.jpg";
            public const string Folder = "ImageFolder.jpg";
            public const string Client = "ImageClient.jpg";
            public const string Appendix = "Appendix.jpg";
            public const string Excel = "Excel.jpg";
            public const string PDF = "PDF.jpg";
            public const string Undefined = "ImageWordDocument.jpg";
            public const string Checked = "Checked.jpg";
            public const string Unchecked = "Unchecked.jpg";

            public const string WordFile32 = "WordFile32.jpg";
            public const string WordFileExists32 = "WordFileExists32.jpg";
            public const string WordFileNotFound32 = "WordFileNotFound32.jpg";

            public const string WordFileSourceNoDestinationNo = "WordFileSourceNoDestinationNo";
            public const string WordFileSourceNoDestinationYes = "WordFileSourceNoDestinationYes";
            public const string WordFileSourceYesDestinationNo = "WordFileSourceYesDestinationNo";
            public const string WordFileSourceYesDestinationYes = "";

            public const string ExcelFileSourceNoDestinationNo = "ExcelFileSourceNoDestinationNo";
            public const string ExcelFileSourceNoDestinationYes = "ExcelFileSourceNoDestinationYes";
            public const string ExcelFileSourceYesDestinationNo = "ExcelFileSourceYesDestinationNo";
            public const string ExcelFileSourceYesDestinationYes = "ExcelFileSourceYesDestinationYes";

            public const string PDFFileSourceNoDestinationNo = "PDFFileSourceNoDestinationNo";
            public const string PDFFileSourceNoDestinationYes = "PDFFileSourceNoDestinationYes";
            public const string PDFFileSourceYesDestinationNo = "PDFFileSourceYesDestinationNo";
            public const string PDFFileSourceYesDestinationYes = "PDFFileSourceYesDestinationYes";
        }

        /// <summary>
        /// List of code types
        /// </summary>
        public struct CodeTypeString
        {
            public const string RoleType = "ROLETYPE";
            public const string ErrorCode = "ERRORCODE";
            public const string SYSTSET = "SYSTSET";
            public const string SCREENCODE = "SCREENCODE";
            public const string ERRORCODE = "ERRORCODE";

        }


        /// <summary>
        /// Represents action to database
        /// </summary>
        public struct SQLAction
        {
            public const string CREATE = "CREATE";
            public const string UPDATE = "UPDATE";
        }

        /// <summary>
        /// List of folder variables
        /// </summary>
        public struct SYSFOLDER
        {
            public const string TEMPLATEFOLDER = "%TEMPLATEFOLDER%";
            public const string CLIENTFOLDER = "%CLIENTFOLDER%";
            public const string VERSIONFOLDER = "%VERSIONFOLDER%";
            public const string PDFEXEPATH = "PDFEXEPATH";
            public const string LOGOFOLDER = "%LOGOFOLDER%";
            public const string WEBLOGOFOLDER = "%WEBLOGOFOLDER%";

            public const string WEBTEMPLATEFOLDER = "%WEBTEMPLATEFOLDER%";
            public const string WEBCLIENTFOLDER = "%WEBCLIENTFOLDER%";
            public const string WEBVERSIONFOLDER = "%WEBVERSIONFOLDER%";
            public const string LOGFILEFOLDER = "%LOGFILEFOLDER%";

        }

        /// <summary>
        /// Document list mode
        /// </summary>
        public struct DocumentListMode
        {
            public const string SELECT = "SELECT";
            public const string MAINTAIN = "MAINTAIN";
        }


        /// <summary>
        /// Screen Codes
        /// </summary>
        public struct ScreenCode
        {
            public const string Document = "DOCUMENT";
            public const string ClientRegistration = "CLNTREG";
            public const string ClientList = "CLNTLIST";
            public const string ClientDocument = "CLNTDOC";
            public const string ClientDocumentSet = "CLNTDOCSET";
            public const string ClientDocumentSetLink = "CLNTDOCSETLINK";
            public const string DocumentList = "DOCLIST";
            public const string DocumentLink = "DOCLINK";
            public const string DocumentSetLink = "DOCSETLINK";
            public const string DocumentSetList = "DOCSETLIST";
            public const string UserAccess = "USERACCESS";

            public const string ImpactedDocuments = "IMPACTEDDOCUMENTS";
            public const string ProcessRequest = "PROCESSREQUEST";
            public const string ReferenceData = "REFERENCEDATA";
            public const string ReportMetadata = "REPORTMETADATA";
            public const string Users = "USERS";
            public const string UserSettings = "USERSETTINGS";
        }

        /// <summary>
        /// Screen Codes
        /// </summary>
        public struct ScreenControl
        {
            public const string TreeViewClientDocumentList = "TVCLNTDOCLIST";
            public const string TreeViewClientDocumentListDocSet = "TVCLNTDOCLISTDOCSET";
            public const string TreeViewDocumentList = "TVCLNTDOCLIST";
        }

        /// <summary>
        /// Font Size
        /// </summary>
        public struct ScreenProperty
        {
            public const string FontSize = "FONTSIZE";
            public const string IconSize = "ICONSIZE";
        }


        /// <summary>
        /// List of error codes
        /// </summary>
        //public struct ErrorCode
        //{

        //    // ---------------------- //
        //    // ERROR MESSAGES //
        //    // ---------------------- //
        //    /// <summary>
        //    /// Invalid logon.
        //    /// </summary>
        //    public const string FCMERR00000001 = "FCMERR00.00.0001";

        //    /// <summary>
        //    /// Error deleting User Role.
        //    /// </summary>
        //    public const string FCMERR00000002 = "FCMERR00.00.0002";

        //    /// <summary>
        //    /// User ID is mandatory.
        //    /// </summary>
        //    public const string FCMERR00000003 = "FCMINF00.00.0003";

        //    /// <summary>
        //    /// Password is mandatory.
        //    /// </summary>
        //    public const string FCMERR00000004 = "FCMINF00.00.0004";

        //    /// <summary>
        //    /// Salt is mandatory.
        //    /// </summary>
        //    public const string FCMERR00000005 = "FCMINF00.00.0005";

        //    /// <summary>
        //    /// Error creating new version. Source file not found.
        //    /// </summary>
        //    public const string FCMERR00000006 = "FCMINF00.00.0006";

        //    /// <summary>
        //    /// Client name is mandatory.
        //    /// </summary>
        //    public const string FCMERR00000007 = "FCMINF00.00.0007";

        //    /// <summary>
        //    /// Role Name is mandatory.
        //    /// </summary>
        //    public const string FCMERR00000008 = "FCMINF00.00.0008";


        //    /// <summary>
        //    /// Error.
        //    /// </summary>
        //    public const string FCMERR00009999 = "FCMINF00.00.9999";
            
        //    // ---------------------- //
        //    // INFORMATIONAL MESSAGES //
        //    // ---------------------- //
        //    /// <summary>
        //    /// Generic Successful.
        //    /// </summary>
        //    public const string FCMINF00000001 = "FCMINF00.00.0001";

        //    /// <summary>
        //    /// User role added successfully.
        //    /// </summary>
        //    public const string FCMINF00000002 = "FCMINF00.00.0002";

        //    /// <summary>
        //    /// User role deleted successfully.
        //    /// </summary>
        //    public const string FCMINF00000003 = "FCMINF00.00.0003";

        //    /// <summary>
        //    /// User added successfully.
        //    /// </summary>
        //    public const string FCMINF00000004 = "FCMINF00.00.0004";

        //    /// <summary>
        //    /// User updated successfully.
        //    /// </summary>
        //    public const string FCMINF00000005 = "FCMINF00.00.0005";

        //    /// <summary>
        //    /// User added successfully.
        //    /// </summary>
        //    public const string FCMINF00000006 = "FCMINF00.00.0006";

        //    /// <summary>
        //    /// Client deleted successfully.
        //    /// </summary>
        //    public const string FCMINF00000007 = "FCMINF00.00.0007";


        //    // ---------------------- //
        //    // WARNING MESSAGES //
        //    // ---------------------- //
        //    /// <summary>
        //    /// Generic Warning.
        //    /// </summary>
        //    public const string FCMWAR00000001 = "FCMWAR00.00.0001";

        //    /// <summary>
        //    /// No valid contract was found for client.
        //    /// </summary>
        //    public const string FCMWAR00000002 = "FCMWAR00.00.0002";

        //}


    }
}
