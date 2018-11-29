using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using FCMMySQLBusinessLibrary;
using MackkadoITFramework.APIDocument;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.ReferenceData;
using MackkadoITFramework.UserSettingsNS;
using MackkadoITFramework.Utils;

namespace MackkadoITFramework.Helper
{
    public static class Utils
    {
        public static ImageList imageList;
        
        private static string userID;
        private static int clientID;
        private static int clientSetID;
        private static string clientSetText;
        private static string clientName;
        //private static List<Client.Client> clientList;
        private static int clientIndex;
        private static int imageLogoStartsFrom;
        // private static string fcmenvironment;
        private static UserSettings _UserSettingsCache;
        public static DateTime MinDate
        {
            get
            {
                return new DateTime(1901, 01, 01);
            }
        }
        public static UserSettings UserSettingsCache
        {
            set { _UserSettingsCache = value; }
            get { return _UserSettingsCache; }
        }

        /// <summary>
        /// Read or Write the userID in memory.
        /// </summary>
        public static string UserID
        {
            set 
            { 
                userID = value;

                // Save last user id to database
                CodeValue cv = new CodeValue();
                cv.FKCodeType = "LASTINFO";
                cv.ID = "USERID";
                cv.Read(false);
                cv.ValueExtended = UserID;

                cv.Save();
            }
    
            get { return userID; }
        }

        public static string ClientSetText
        {
            set { clientSetText = value; }
            get { return clientSetText; }
        }


        public static int ClientSetID
        {
            set { clientSetID = value; }
            get { return clientSetID; }
        }
        public static int ClientIndex
        {
            // set { clientIndex = value; }
            get { return clientIndex; }
        }
        public static string ClientName
        {
            set { clientName = value; }
            get { return clientName; }
        }

        public static int ImageLogoStartsFrom
        {
            set { imageLogoStartsFrom = value; }
            get { return imageLogoStartsFrom; }
        }

        //public static List<Client.Client> ClientList
        //{
        //    set { clientList = value; }
        //    get { return clientList; }
        //}

        public static string FCMenvironment { get; set; }


        public struct InformationType
        {
            public const string IMAGE = "IMAGE";
            public const string FIELD = "FIELD";
            public const string VARIABLE = "VARIABLE";
        }

        public struct EnvironmentList
        {
            public const string WEB = "WEB";
            public const string LOCAL = "LOCAL";
        }


        public struct SaveType
        {
            public const string NEWONLY = "NEWONLY";
            public const string UPDATE = "UPDATE";
        }

        /// <summary>
        /// Indicates the source of the document. It could be FCM or Client
        /// </summary>
        public struct SourceCode
        {
            public const string CLIENT = "CLIENT";
            public const string FCM = "FCM";
        }

        
        public struct RecordType
        {
            public const string FOLDER = "FOLDER";
            public const string DOCUMENT = "DOCUMENT";
            public const string APPENDIX = "APPENDIX";
        }


        public struct DocumentType
        {
            public const string FOLDER = "FOLDER";
            public const string WORD = "WORD";
            public const string EXCEL = "EXCEL";
            public const string PDF = "PDF";
            public const string UNDEFINED = "UNDEFINED";
            public const string APPENDIX = "APPENDIX";
        }


        public struct UserRole
        {
            public const string Admin = "ADMIN";
            public const string PowerUser = "POWERUSER";
            public const string Client = "CLIENT";
            public const string User = "USER";
        }


        /// <summary>
        /// Represents action to database
        /// </summary>
        public struct SQLAction
        {
            public const string CREATE = "CREATE";
            public const string UPDATE = "UPDATE";
        }

        public struct SYSTSET
        {
            public const string WEBPORT = "%WEBPORT%";
            public const string HOSTIPADDRESS = "%HOSTIPADDRESS%";
        }

        public struct DocumentLinkType
        {
            public const string PROJECTPLAN = "PROJPLAN";
            public const string APPENDIX = "APPENDIX";
        }

        // It transforms the reference path into a physical path and add name to it
        //
        public static string getFilePathName( string path, string name = "" )
        {
            string filePathName = path + "\\" + name;
            string fullPathFileName = "";

            var fcmPort = CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, SYSTSET.WEBPORT);
            var fcmHost = CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, SYSTSET.HOSTIPADDRESS);

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended("SYSTSET", MakConstant.SYSFOLDER.TEMPLATEFOLDER);

            // Get template folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended("SYSTSET", MakConstant.SYSFOLDER.CLIENTFOLDER);

            // Get version folder 
            var versionFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.VERSIONFOLDER);

            // Get logo folder 
            var logoFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGOFOLDER);

            // Get log file folder 
            var logFileFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGFILEFOLDER);



            // WEB

            if (FCMenvironment == EnvironmentList.WEB)
            {
                string rpath = path.Replace(@"\", @"/");
                path = rpath;

                // Different for WEB
                filePathName = path + @"/" + name;

                // ----------------------
                // Get WEB template folder 
                // ----------------------
                templateFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBTEMPLATEFOLDER);

                templateFolder = templateFolder.Replace(SYSTSET.WEBPORT, fcmPort);
                templateFolder = templateFolder.Replace(SYSTSET.HOSTIPADDRESS, fcmHost);

                // ----------------------
                // Get WEB client folder 
                // ----------------------
                clientFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBCLIENTFOLDER);

                clientFolder = clientFolder.Replace(SYSTSET.WEBPORT, fcmPort);
                clientFolder = clientFolder.Replace(SYSTSET.HOSTIPADDRESS, fcmHost);

                // ----------------------
                // Get WEB version folder 
                // ----------------------
                versionFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBVERSIONFOLDER);

                versionFolder = versionFolder.Replace( SYSTSET.WEBPORT, fcmPort );
                versionFolder = versionFolder.Replace(SYSTSET.HOSTIPADDRESS, fcmHost);

                // ----------------------
                // Get WEB logo folder 
                // ----------------------
                logoFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBLOGOFOLDER);

                logoFolder = logoFolder.Replace(SYSTSET.WEBPORT, fcmPort);
                logoFolder = logoFolder.Replace(SYSTSET.HOSTIPADDRESS, fcmHost);

                // --------------------------------------------------------------
                // Get WEB LOG folder - This is LOG for recording what happened
                // --------------------------------------------------------------
                logFileFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGFILEFOLDER);

                logFileFolder = logFileFolder.Replace(SYSTSET.WEBPORT, fcmPort);
                logFileFolder = logFileFolder.Replace(SYSTSET.HOSTIPADDRESS, fcmHost);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.TEMPLATEFOLDER, templateFolder);
            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.CLIENTFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.CLIENTFOLDER, clientFolder);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.VERSIONFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.VERSIONFOLDER, versionFolder);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.LOGOFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.LOGOFOLDER, logoFolder);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.LOGFILEFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.LOGFILEFOLDER, logFileFolder);
            }

            if (String.IsNullOrEmpty(fullPathFileName))
                fullPathFileName = path + "\\" + name;

            fullPathFileName = fullPathFileName.Replace("\r", "");

            return fullPathFileName;
        }

        // It transforms the reference path into a physical path and add name to it
        //
        public static string getFilePathNameLOCAL( string path, string name = "" )
        {
            string filePathName = "";

            if ( string.IsNullOrEmpty( name ) )
            {
                filePathName = path;
            }
            else
            {
                filePathName = path + "\\" + name;
            }

            string fullPathFileName = "";

            var fcmPort = CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, SYSTSET.WEBPORT );
            var fcmHost = CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, SYSTSET.HOSTIPADDRESS );

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended( "SYSTSET", MakConstant.SYSFOLDER.TEMPLATEFOLDER );

            // Get template folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended( "SYSTSET", MakConstant.SYSFOLDER.CLIENTFOLDER );

            // Get version folder 
            var versionFolder =
                CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.VERSIONFOLDER );

            // Get logo folder 
            var logoFolder =
                CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGOFOLDER );

            // Get log file folder 
            var logFileFolder =
                CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGFILEFOLDER );



            if ( filePathName.Contains( MakConstant.SYSFOLDER.TEMPLATEFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.TEMPLATEFOLDER, templateFolder );
            }


            if ( filePathName.Contains( MakConstant.SYSFOLDER.CLIENTFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.CLIENTFOLDER, clientFolder );

            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.VERSIONFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.VERSIONFOLDER, versionFolder );

            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.LOGOFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.LOGOFOLDER, logoFolder );

            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.LOGFILEFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.LOGFILEFOLDER, logFileFolder );
            }

            if ( String.IsNullOrEmpty( fullPathFileName ) )
                fullPathFileName = path + "\\" + name;

            fullPathFileName = fullPathFileName.Replace( "\r", "" );

            return fullPathFileName;
        }



        // It transforms the reference path into a physical path and add name to it
        //
        public static string getFilePathNameWEB( string path, string name = "" )
        {
            string filePathName = "";

            if ( string.IsNullOrEmpty( name ) )
            {
                filePathName = path;
            }
            else
            {
                filePathName = path + "\\" + name;
            }

            string fullPathFileName = "";

            var fcmPort = CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, SYSTSET.WEBPORT );
            var fcmHost = CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, SYSTSET.HOSTIPADDRESS );

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended( "SYSTSET", MakConstant.SYSFOLDER.TEMPLATEFOLDER );

            // Get template folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended( "SYSTSET", MakConstant.SYSFOLDER.CLIENTFOLDER );

            // Get version folder 
            var versionFolder =
                CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.VERSIONFOLDER );

            // Get logo folder 
            var logoFolder =
                CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGOFOLDER );

            // Get log file folder 
            var logFileFolder =
                CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGFILEFOLDER );


            // Get WEB Paths
            //

            string rpath = path.Replace( @"\", @"/" );
            path = rpath;

            // Different for WEB

            if ( string.IsNullOrEmpty( name ) )
            {
                filePathName = path ;
            }
            else
            {
                filePathName = path + @"/" + name;
            }

            // ----------------------
            // Get WEB template folder 
            // ----------------------
            templateFolder =
            CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBTEMPLATEFOLDER );

            templateFolder = templateFolder.Replace( SYSTSET.WEBPORT, fcmPort );
            templateFolder = templateFolder.Replace( SYSTSET.HOSTIPADDRESS, fcmHost );

            // ----------------------
            // Get WEB client folder 
            // ----------------------
            clientFolder =
            CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBCLIENTFOLDER );

            clientFolder = clientFolder.Replace( SYSTSET.WEBPORT, fcmPort );
            clientFolder = clientFolder.Replace( SYSTSET.HOSTIPADDRESS, fcmHost );

            // ----------------------
            // Get WEB version folder 
            // ----------------------
            versionFolder =
            CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBVERSIONFOLDER );

            versionFolder = versionFolder.Replace( SYSTSET.WEBPORT, fcmPort );
            versionFolder = versionFolder.Replace( SYSTSET.HOSTIPADDRESS, fcmHost );

            // ----------------------
            // Get WEB logo folder 
            // ----------------------
            logoFolder =
            CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.WEBLOGOFOLDER );

            logoFolder = logoFolder.Replace( SYSTSET.WEBPORT, fcmPort );
            logoFolder = logoFolder.Replace( SYSTSET.HOSTIPADDRESS, fcmHost );

            // --------------------------------------------------------------
            // Get WEB LOG folder - This is LOG for recording what happened
            // --------------------------------------------------------------
            logFileFolder =
            CodeValue.GetCodeValueExtended( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGFILEFOLDER );

            logFileFolder = logFileFolder.Replace( SYSTSET.WEBPORT, fcmPort );
            logFileFolder = logFileFolder.Replace( SYSTSET.HOSTIPADDRESS, fcmHost );

            if ( filePathName.Contains( MakConstant.SYSFOLDER.TEMPLATEFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.TEMPLATEFOLDER, templateFolder );
            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.CLIENTFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.CLIENTFOLDER, clientFolder );

            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.VERSIONFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.VERSIONFOLDER, versionFolder );

            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.LOGOFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.LOGOFOLDER, logoFolder );

            }

            if ( filePathName.Contains( MakConstant.SYSFOLDER.LOGFILEFOLDER ) )
            {
                fullPathFileName =
                    filePathName.Replace( MakConstant.SYSFOLDER.LOGFILEFOLDER, logFileFolder );
            }

            if ( String.IsNullOrEmpty( fullPathFileName ) )
                fullPathFileName = path + "\\" + name;

            fullPathFileName = fullPathFileName.Replace( "\r", "" );

            return fullPathFileName;
        }


        // It transforms the reference path into a physical path and add name to it
        //
        public static string GetPathNameTBD(string path)
        {
            string filePathName = path;
            string fullPathFileName = "";

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.TEMPLATEFOLDER);

            // Get main client folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.CLIENTFOLDER);

            // Get version folder 
            var versionFolder  =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.VERSIONFOLDER);

            // Get logo folder 
            var logoFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.LOGOFOLDER);

            if (filePathName.Contains(MakConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.TEMPLATEFOLDER, templateFolder);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.CLIENTFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.CLIENTFOLDER, clientFolder);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.VERSIONFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.VERSIONFOLDER, versionFolder);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.LOGOFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(MakConstant.SYSFOLDER.LOGOFOLDER, logoFolder);
            }

            return fullPathFileName;
        }


        //
        // It returns a reference path 
        //
        public static string getReferenceFilePathName(string path)
        {
            string filePathName = path;
            string referencePathFileName = "";

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.TEMPLATEFOLDER);

            var templateFolderPhysical =
                CodeValue.GetCodeValueExtraString( MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.TEMPLATEFOLDER );


            // Get template folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended(MakConstant.CodeTypeString.SYSTSET, MakConstant.SYSFOLDER.CLIENTFOLDER);

            if (filePathName.Contains(templateFolder))
            {
                referencePathFileName =
                    filePathName.Replace(templateFolder, MakConstant.SYSFOLDER.TEMPLATEFOLDER);
            }
            if ( filePathName.Contains( templateFolderPhysical ) )
            {
                referencePathFileName =
                    filePathName.Replace( templateFolderPhysical, MakConstant.SYSFOLDER.TEMPLATEFOLDER );
            }


            if (filePathName.Contains(clientFolder))
            {
                referencePathFileName =
                    filePathName.Replace(clientFolder, MakConstant.SYSFOLDER.CLIENTFOLDER);

            }

            if (String.IsNullOrEmpty(referencePathFileName))
                referencePathFileName = path;

            return referencePathFileName;
        }

        /// <summary>
        /// It returns the opposite path (client to template or vice-versa) 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string getOppositePath(string path)
        {
            string filePathName = path;
            string opposite = "";

            if (filePathName.Contains(MakConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                opposite =
                    filePathName.Replace(MakConstant.SYSFOLDER.TEMPLATEFOLDER, 
                    MakConstant.SYSFOLDER.CLIENTFOLDER);

            }

            if (filePathName.Contains(MakConstant.SYSFOLDER.CLIENTFOLDER))
            {
                opposite =
                    filePathName.Replace(MakConstant.SYSFOLDER.CLIENTFOLDER, MakConstant.SYSFOLDER.TEMPLATEFOLDER);

            }

            if (String.IsNullOrEmpty(opposite))
                opposite = path;

            return opposite;
        }

        /// <summary>
        /// It returns the path of the document inside the client path 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetClientPathInside(string path)
        {
            string filePathName = path;
            string opposite = "";

            if (filePathName.Contains(MakConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                opposite =
                    filePathName.Replace(MakConstant.SYSFOLDER.TEMPLATEFOLDER, "");

            }

            //if (string.IsNullOrEmpty(opposite))
            //    opposite = path;

            return opposite;
        }

        /// <summary>
        /// It returns the Client path 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public static string GetClientPath(string path, string destinationPath)
        {
            string destination = "";
            string ultimateDestination = "";

            if (path.Contains(MakConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                destination =
                    path.Replace(MakConstant.SYSFOLDER.TEMPLATEFOLDER, MakConstant.SYSFOLDER.CLIENTFOLDER);

            }

            // path = %TEMPLATEFOLDER%\\something\\
            // destinationPath = %CLIENTFOLDER%\\CLIENT01\\
            // destination = %CLIENTFOLDER%\\something\\
            // 

            // stripDestination = \\something\\
            string stripDestination = destination.Replace(MakConstant.SYSFOLDER.CLIENTFOLDER, "");

            // ultimateDestination = \\Client01\\

            ultimateDestination = destinationPath.Replace(MakConstant.SYSFOLDER.CLIENTFOLDER, "");
            ultimateDestination = MakConstant.SYSFOLDER.CLIENTFOLDER +  // %CLIENTFOLDER%
                                  ultimateDestination + // \\client01\\
                                  stripDestination;     // \\something\\

            if (String.IsNullOrEmpty(ultimateDestination))
                ultimateDestination = path;

            return ultimateDestination;
        }

        /// <summary>
        /// It returns the Client path 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetVersionPath(string path)
        {
            string destination = path;

            if (path.Contains(MakConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                destination =
                    path.Replace(MakConstant.SYSFOLDER.TEMPLATEFOLDER, MakConstant.SYSFOLDER.VERSIONFOLDER);

            }

            return destination;
        }

        ///// <summary>
        ///// Open document for Document type 
        ///// </summary>
        ///// <param name="document"></param>
        //public static void OpenDocument(Document.Document document, object vkReadOnly)
        //{
        //    if (document.DocumentType == Utils.DocumentType.WORD)
        //    {
        //        string filePathName =
        //            Utils.getFilePathName(document.Location,
        //                                  document.Name );

        //        WordDocumentTasks.OpenDocument(filePathName, vkReadOnly);
        //    }

        //}

        // Open document for Location and Name
        //
        //public static ResponseStatus OpenDocument( string Location, string Name, string Type, object vkReadOnly, bool isFromWeb )
        //{
        //    if (Type == DocumentType.WORD)
        //    {
        //        string filePathName =
        //            getFilePathName(Location,
        //                                  Name);

        //        var response = WordDocumentTasks.OpenDocument(filePathName, vkReadOnly, isFromWeb);

        //        if ( response.ReturnCode < 1 )
        //        {
        //            return response;
        //        }
        //    }

        //    if (Type == DocumentType.EXCEL)
        //    {
        //        string filePathName =
        //            getFilePathName(Location,
        //                                  Name);

        //        var response = ExcelSpreadsheetTasks.OpenDocument(filePathName);
        //        if (response.ReturnCode < 1)
        //        {
        //            return response;
        //        }
        //    }
        //    if (Type == DocumentType.PDF)
        //    {
        //        string filePathName =
        //            getFilePathName( Location,
        //                                  Name );
        //        Process proc = new Process();
        //        var adobe = CodeValue.GetCodeValueExtended( iCodeType: MakConstant.CodeTypeString.SYSTSET, iCodeValueID: "PDFEXEPATH" );

        //        if (!File.Exists( adobe ))
        //        {
        //            MessageBox.Show( "I can't find Adobe Reader. Please configure SYSTSET.PDFEXTPATH." );
                    
        //            var error = new ResponseStatus(MessageType.Error);
        //            error.Message = "Adobe Reader can't be found. Please configure SYSTSET.PDFEXTPATH.";
        //            return error;
        //        }

        //        proc.StartInfo.FileName = adobe;
        //        proc.StartInfo.Arguments = filePathName;
        //        proc.Start();

        //    }

        //    return new ResponseStatus( MessageType.Informational );
        //}

        /// <summary>
        /// Return image according to Record Type 
        /// </summary>
        /// <param name="RecordType"></param>
        /// <returns></returns>
        public static int ImageSelect(string RecordType)
        {
            int image = MakConstant.Image.Document;

            switch (RecordType)
            {
                case Utils.RecordType.DOCUMENT:
                    image = MakConstant.Image.Document;
                    break;
                case Utils.RecordType.APPENDIX:
                    image = MakConstant.Image.Document;
                    break;
                case Utils.RecordType.FOLDER:
                    image = MakConstant.Image.Folder;
                    break;
                default:
                    image = MakConstant.Image.Document;
                    break;
            }

            return image;
        }


        /// <summary>
        /// Get Logo location for a client.
        /// </summary>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        public static string GetImageUrl( string DocumentType, string curEnvironment = EnvironmentList.LOCAL )
        {

            string image = "";

            string logoPath = "";
            string logoName = "";
            string logoPathName = "";

            FCMenvironment = curEnvironment;


            switch (DocumentType)
            {
                case Utils.DocumentType.WORD:
                    logoName = MakConstant.ImageFileName.Document;
                    break;

                case Utils.DocumentType.EXCEL:
                    logoName = MakConstant.ImageFileName.Excel;
                    break;

                case Utils.DocumentType.FOLDER:
                    logoName = MakConstant.ImageFileName.Folder;
                    break;

                case Utils.DocumentType.PDF:
                    logoName = MakConstant.ImageFileName.PDF;
                    break;

                default:
                    logoName = MakConstant.ImageFileName.Document;
                    break;
            }


            // Set no icon image if necessary
            //
            logoPath = MakConstant.SYSFOLDER.LOGOFOLDER;
            logoName = logoName.Replace( MakConstant.SYSFOLDER.LOGOFOLDER, String.Empty );
            
            logoPathName = getFilePathName( logoPath, logoName );

            return logoPathName;
        }


        /// <summary>
        /// Get image for file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static int GetFileImage(char source, char destination, string documentType )
        {
            int image = MakConstant.Image.WordFileSourceNoDestinationNo;

            if (source == 'Y')
            {
                if (destination == 'Y')
                {
                    // Source = "Y"; Destination = "Y"
                    switch (documentType)
                    {
                        case DocumentType.WORD:
                            image = MakConstant.Image.WordFileSourceYesDestinationYes;
                            break;
                        case DocumentType.EXCEL:
                            image = MakConstant.Image.ExcelFileSourceYesDestinationYes;
                            break;
                        case DocumentType.PDF:
                            image = MakConstant.Image.PDFFileSourceYesDestinationYes;
                            break;
                        case DocumentType.FOLDER:
                            image = MakConstant.Image.Folder;
                            break;
                        case DocumentType.APPENDIX:
                            image = MakConstant.Image.Appendix;
                            break;
                    }
                }
                else
                {
                    // Source = "Y"; Destination = "N"
                    image = MakConstant.Image.WordFileSourceYesDestinationNo;

                    switch (documentType)
                    {
                        case DocumentType.WORD:
                            image = MakConstant.Image.WordFileSourceYesDestinationNo;
                            break;
                        case DocumentType.EXCEL:
                            image = MakConstant.Image.ExcelFileSourceYesDestinationNo;
                            break;
                        case DocumentType.PDF:
                            image = MakConstant.Image.PDFFileSourceYesDestinationNo;
                            break;
                        case DocumentType.FOLDER:
                            image = MakConstant.Image.Folder;
                            break;
                        case DocumentType.APPENDIX:
                            image = MakConstant.Image.Appendix;
                            break;
                    }

                }
            }
            else
            {
                if (destination == 'Y')
                {
                    // Source = "N"; Destination = "Y"
                    image = MakConstant.Image.WordFileSourceNoDestinationYes;

                    switch (documentType)
                    {
                        case DocumentType.WORD:
                            image = MakConstant.Image.WordFileSourceNoDestinationYes;
                            break;
                        case DocumentType.EXCEL:
                            image = MakConstant.Image.ExcelFileSourceNoDestinationYes;
                            break;
                        case DocumentType.PDF:
                            image = MakConstant.Image.PDFFileSourceNoDestinationYes;
                            break;
                        case DocumentType.FOLDER:
                            image = MakConstant.Image.Folder;
                            break;
                        case DocumentType.APPENDIX:
                            image = MakConstant.Image.Appendix;
                            break;
                    }

                }
                else
                {
                    // Source = "N"; Destination = "N"
                    image = MakConstant.Image.WordFileSourceNoDestinationNo;

                    switch (documentType)
                    {
                        case DocumentType.WORD:
                            image = MakConstant.Image.WordFileSourceNoDestinationNo;
                            break;
                        case DocumentType.EXCEL:
                            image = MakConstant.Image.ExcelFileSourceNoDestinationNo;
                            break;
                        case DocumentType.PDF:
                            image = MakConstant.Image.PDFFileSourceNoDestinationNo;
                            break;
                        case DocumentType.FOLDER:
                            image = MakConstant.Image.Folder;
                            break;
                        case DocumentType.APPENDIX:
                            image = MakConstant.Image.Appendix;
                            break;
                    }
                }
            }
            return image;
        }


        /// <summary>
        /// Retrieves cached value for user settings
        /// </summary>
        /// <returns></returns>
        public static string UserSettingGetCacheValue( UserSettings userSettings)
        {
            string valueReturned = "";

            if (UserSettingsCache == null)
                return valueReturned;

            foreach (var userSet in UserSettingsCache.ListOfUserSettings)
            {
                if (
                    userSet.FKUserID == userSettings.FKUserID &&
                    userSet.FKScreenCode == userSettings.FKScreenCode &&
                    userSet.FKControlCode == userSettings.FKControlCode &&
                    userSet.FKPropertyCode == userSettings.FKPropertyCode
                    )
                {
                    valueReturned = userSet.Value;
                }
            }
            valueReturned = valueReturned.Trim();

            return valueReturned;
        }


        public static string GetFileExtensionString(string filename)
        {
            string fileExtension = "";

            int pos = filename.IndexOf( '.' );
            // Check position
            //
            string charExt4 = filename.Substring( filename.Length - 4, 1 );
            if ( charExt4 == "." )
            {
                // It has 4 characters
                //
                pos = filename.Length - 4;
            }
            string charExt3 = filename.Substring( filename.Length - 3, 1 );
            if ( charExt3 == "." )
            {
                // It has 4 characters
                //
                pos = filename.Length - 4;
            }

            if ( pos > 1 )
                fileExtension = filename.Substring( pos + 1, filename.Length - pos - 1 );
            else
                fileExtension = "UNK";

            return fileExtension;
        }
    
    }
}
