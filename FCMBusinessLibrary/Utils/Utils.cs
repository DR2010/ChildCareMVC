using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FCMBusinessLibrary.ReferenceData;

namespace FCMBusinessLibrary
{
    public static class Utils
    {
        public static ImageList imageList;
        
        private static string userID;
        private static int clientID;
        private static int clientSetID;
        private static string clientSetText;
        private static string clientName;
        private static List<Client.Client> clientList;
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
                cv.ValueExtended = Utils.UserID;

                cv.Save();
            }
    
            get { return userID; }
        }

        public static string ClientSetText
        {
            set { clientSetText = value; }
            get { return clientSetText; }
        }

        public static int ClientID
        {
            set { 
                clientID = value;

                // Save last user id to database
                CodeValue cv = new CodeValue();
                cv.FKCodeType = "LASTINFO";
                cv.ID = "CLIENTID";
                cv.Read(false);
                cv.ValueExtended = Utils.ClientID.ToString();
                cv.Save();

                if (clientList == null)
                    return;

                int c=0;
                foreach (var client in clientList)
                {
                    if (client.UID == clientID)
                    {
                        Utils.clientIndex = c;
                        break;
                    }
                    c++;
                }

            }
            get { return clientID; }
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

        public static List<Client.Client> ClientList
        {
            set { clientList = value; }
            get { return clientList; }
        }

        public static string FCMenvironment { get; set; }

        // Print document for Location and Name
        //
        public static void PrintDocument( string Location, string Name, string Type )
        {
            if (Type == Utils.DocumentType.WORD)
            {
                string filePathName =
                    Utils.getFilePathName( Location, Name);

                WordDocumentTasks.PrintDocument( filePathName );
            }

            if (Type == Utils.DocumentType.EXCEL)
            {
                string filePathName =
                    Utils.getFilePathName( Location, Name);

                var Response = ExcelSpreadsheetTasks.PrintDocument( filePathName );
                if (Response.ReturnCode < 1)
                {
                    MessageBox.Show( Response.Message );
                }
            }
            if (Type == Utils.DocumentType.PDF)
            {
                string filePathName =
                    Utils.getFilePathName( Location, Name);
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                var adobe = CodeValue.GetCodeValueExtended( iCodeType: FCMConstant.CodeTypeString.SYSTSET, iCodeValueID: "PDFEXEPATH" );

                if (!File.Exists( adobe ))
                {
                    MessageBox.Show( "I can't find Adobe Reader. Please configure SYSTSET.PDFEXTPATH." );
                    return;
                }

                proc.StartInfo.FileName = adobe ;
                // Print PDF
                proc.StartInfo.Arguments = " /h /p "+ filePathName;
                proc.Start();


            }

        }


        /// <summary>
        /// Compare documents  
        /// </summary>
        /// <param name="Location"></param>
        /// <param name="Name"></param>
        /// <param name="Type"></param>
        /// <param name="DestinationFile"></param>
        public static void CompareDocuments(
            string Source, 
            string Destination, 
            string Type)
        {
            if (Type == Utils.DocumentType.WORD)
            {

                WordDocumentTasks.CompareDocuments(Source, Destination);
            }

        }


        public struct DocumentStatus
        {
            public const string ACTIVE = "ACTIVE";
            public const string INACTIVE = "INACTIVE";
        }

        public struct DocumentSetStatus
        {
            public const string COMPLETED = "COMPLETED";
            public const string DRAFT = "DRAFT";
        }

        public struct DocumentListType
        {
            public const string FCM = "FCM";
            public const string DOCUMENTSET = "DOCUMENTSET";
        }


        public struct MessageTypeX
        {
            public const string Error = "Error";
            public const string Warning = "Warning";
            public const string Informational = "Informational";
        }

        public struct MetadataRecordType
        {
            public const string CLIENT = "CL";
            public const string DEFAULT = "DF";
        }

        public struct FieldCode
        {
            public const string COMPANYLOGO = "COMPANYLOGO";
        }


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

        //public struct CodeTypeCategory
        //{
        //    public const string SYSTSET = "SYSTSET";
        //    public const string SCREENCODE = "SCREENCODE";
        //    public const string ERRORCODE = "ERRORCODE";
        //}

        public struct UserRole
        {
            public const string Admin = "ADMIN";
            public const string PowerUser = "POWERUSER";
            public const string Client = "CLIENT";
            public const string User = "USER";
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
        public static string getFilePathName( string path, string name )
        {
            string filePathName = path + "\\" + name;
            string fullPathFileName = "";

            var fcmPort = CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, Utils.SYSTSET.WEBPORT);
            var fcmHost = CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, Utils.SYSTSET.HOSTIPADDRESS);

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended("SYSTSET", FCMConstant.SYSFOLDER.TEMPLATEFOLDER);

            // Get template folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended("SYSTSET", FCMConstant.SYSFOLDER.CLIENTFOLDER);

            // Get version folder 
            var versionFolder =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.VERSIONFOLDER);

            // Get logo folder 
            var logoFolder =
                CodeValue.GetCodeValueExtended( FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.LOGOFOLDER);

            // Get log file folder 
            var logFileFolder =
                CodeValue.GetCodeValueExtended( FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.LOGFILEFOLDER);



            // WEB

            if (Utils.FCMenvironment == Utils.EnvironmentList.WEB)
            {
                string rpath = path.Replace(@"\", @"/");
                path = rpath;

                // Different for WEB
                filePathName = path + @"/" + name;

                // ----------------------
                // Get WEB template folder 
                // ----------------------
                templateFolder =
                CodeValue.GetCodeValueExtended( FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.WEBTEMPLATEFOLDER );

                templateFolder = templateFolder.Replace(Utils.SYSTSET.WEBPORT, fcmPort);
                templateFolder = templateFolder.Replace(Utils.SYSTSET.HOSTIPADDRESS, fcmHost);

                // ----------------------
                // Get WEB client folder 
                // ----------------------
                clientFolder =
                CodeValue.GetCodeValueExtended( FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.WEBCLIENTFOLDER );

                clientFolder = clientFolder.Replace(Utils.SYSTSET.WEBPORT, fcmPort);
                clientFolder = clientFolder.Replace(Utils.SYSTSET.HOSTIPADDRESS, fcmHost);

                // ----------------------
                // Get WEB version folder 
                // ----------------------
                versionFolder =
                CodeValue.GetCodeValueExtended( FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.WEBVERSIONFOLDER );

                versionFolder = versionFolder.Replace( Utils.SYSTSET.WEBPORT, fcmPort );
                versionFolder = versionFolder.Replace(Utils.SYSTSET.HOSTIPADDRESS, fcmHost);

                // ----------------------
                // Get WEB logo folder 
                // ----------------------
                logoFolder =
                CodeValue.GetCodeValueExtended( FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.WEBLOGOFOLDER );

                logoFolder = logoFolder.Replace(Utils.SYSTSET.WEBPORT, fcmPort);
                logoFolder = logoFolder.Replace(Utils.SYSTSET.HOSTIPADDRESS, fcmHost);

                // --------------------------------------------------------------
                // Get WEB LOG folder - This is LOG for recording what happened
                // --------------------------------------------------------------
                logFileFolder =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.LOGFILEFOLDER);

                logFileFolder = logFileFolder.Replace(Utils.SYSTSET.WEBPORT, fcmPort);
                logFileFolder = logFileFolder.Replace(Utils.SYSTSET.HOSTIPADDRESS, fcmHost);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace( FCMConstant.SYSFOLDER.TEMPLATEFOLDER, templateFolder );
            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.CLIENTFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.CLIENTFOLDER, clientFolder);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.VERSIONFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.VERSIONFOLDER, versionFolder);

            }

            if (filePathName.Contains( FCMConstant.SYSFOLDER.LOGOFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace( FCMConstant.SYSFOLDER.LOGOFOLDER, logoFolder);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.LOGFILEFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.LOGFILEFOLDER, logFileFolder);
            }

            if (string.IsNullOrEmpty(fullPathFileName))
                fullPathFileName = path + "\\" + name;

            return fullPathFileName;
        }

        // It transforms the reference path into a physical path and add name to it
        //
        public static string GetPathName(string path)
        {
            string filePathName = path;
            string fullPathFileName = "";

            // Get template folder 
            var templateFolder =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.TEMPLATEFOLDER);

            // Get main client folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.CLIENTFOLDER);

            // Get version folder 
            var versionFolder  =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.VERSIONFOLDER);

            // Get logo folder 
            var logoFolder =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.LOGOFOLDER);

            if (filePathName.Contains(FCMConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.TEMPLATEFOLDER, templateFolder);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.CLIENTFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.CLIENTFOLDER, clientFolder);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.VERSIONFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.VERSIONFOLDER, versionFolder);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.LOGOFOLDER))
            {
                fullPathFileName =
                    filePathName.Replace(FCMConstant.SYSFOLDER.LOGOFOLDER, logoFolder);
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
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.TEMPLATEFOLDER);

            // Get template folder 
            var clientFolder =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.CLIENTFOLDER);

            if (filePathName.Contains(templateFolder))
            {
                referencePathFileName =
                    filePathName.Replace(templateFolder, FCMConstant.SYSFOLDER.TEMPLATEFOLDER);

            }

            if (filePathName.Contains(clientFolder))
            {
                referencePathFileName =
                    filePathName.Replace(clientFolder, FCMConstant.SYSFOLDER.CLIENTFOLDER);

            }

            if (string.IsNullOrEmpty(referencePathFileName))
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

            if (filePathName.Contains(FCMConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                opposite =
                    filePathName.Replace(FCMConstant.SYSFOLDER.TEMPLATEFOLDER, 
                    FCMConstant.SYSFOLDER.CLIENTFOLDER);

            }

            if (filePathName.Contains(FCMConstant.SYSFOLDER.CLIENTFOLDER))
            {
                opposite =
                    filePathName.Replace(FCMConstant.SYSFOLDER.CLIENTFOLDER, FCMConstant.SYSFOLDER.TEMPLATEFOLDER);

            }

            if (string.IsNullOrEmpty(opposite))
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

            if (filePathName.Contains(FCMConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                opposite =
                    filePathName.Replace(FCMConstant.SYSFOLDER.TEMPLATEFOLDER, "");

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

            if (path.Contains(FCMConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                destination =
                    path.Replace(FCMConstant.SYSFOLDER.TEMPLATEFOLDER, FCMConstant.SYSFOLDER.CLIENTFOLDER);

            }

            // path = %TEMPLATEFOLDER%\\something\\
            // destinationPath = %CLIENTFOLDER%\\CLIENT01\\
            // destination = %CLIENTFOLDER%\\something\\
            // 

            // stripDestination = \\something\\
            string stripDestination = destination.Replace(FCMConstant.SYSFOLDER.CLIENTFOLDER, "");

            // ultimateDestination = \\Client01\\

            ultimateDestination = destinationPath.Replace(FCMConstant.SYSFOLDER.CLIENTFOLDER, "");
            ultimateDestination = FCMConstant.SYSFOLDER.CLIENTFOLDER +  // %CLIENTFOLDER%
                                  ultimateDestination + // \\client01\\
                                  stripDestination;     // \\something\\

            if (string.IsNullOrEmpty(ultimateDestination))
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

            if (path.Contains(FCMConstant.SYSFOLDER.TEMPLATEFOLDER))
            {
                destination =
                    path.Replace(FCMConstant.SYSFOLDER.TEMPLATEFOLDER, FCMConstant.SYSFOLDER.VERSIONFOLDER);

            }

            return destination;
        }

        /// <summary>
        /// Open document for Document type 
        /// </summary>
        /// <param name="document"></param>
        public static void OpenDocument(Document.Document document, object vkReadOnly)
        {
            if (document.DocumentType == Utils.DocumentType.WORD)
            {
                string filePathName =
                    Utils.getFilePathName(document.Location,
                                          document.Name );

                WordDocumentTasks.OpenDocument(filePathName, vkReadOnly);
            }

        }

        // Open document for Location and Name
        //
        public static void OpenDocument(string Location, string Name, string Type, object vkReadOnly)
        {
            if (Type == Utils.DocumentType.WORD)
            {
                string filePathName =
                    Utils.getFilePathName(Location,
                                          Name);

                WordDocumentTasks.OpenDocument(filePathName, vkReadOnly);
            }

            if (Type == Utils.DocumentType.EXCEL)
            {
                string filePathName =
                    Utils.getFilePathName(Location,
                                          Name);

                var Response = ExcelSpreadsheetTasks.OpenDocument(filePathName);
                if (Response.ReturnCode < 1)
                {
                    MessageBox.Show(Response.Message);
                }
            }
            if (Type == Utils.DocumentType.PDF)
            {
                string filePathName =
                    Utils.getFilePathName( Location,
                                          Name );
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                var adobe = CodeValue.GetCodeValueExtended( iCodeType: FCMConstant.CodeTypeString.SYSTSET, iCodeValueID: "PDFEXEPATH" );

                if (!File.Exists( adobe ))
                {
                    MessageBox.Show( "I can't find Adobe Reader. Please configure SYSTSET.PDFEXTPATH." );
                    return;
                }

                proc.StartInfo.FileName = adobe;
                proc.StartInfo.Arguments = filePathName;
                proc.Start();
                    

            }

        }

        /// <summary>
        /// Return image according to Record Type 
        /// </summary>
        /// <param name="RecordType"></param>
        /// <returns></returns>
        public static int ImageSelect(string RecordType)
        {
            int image = FCMConstant.Image.Document;

            switch (RecordType)
            {
                case Utils.RecordType.DOCUMENT:
                    image = FCMConstant.Image.Document;
                    break;
                case Utils.RecordType.APPENDIX:
                    image = FCMConstant.Image.Document;
                    break;
                case Utils.RecordType.FOLDER:
                    image = FCMConstant.Image.Folder;
                    break;
                default:
                    image = FCMConstant.Image.Document;
                    break;
            }

            return image;
        }


        /// <summary>
        /// Get Logo location for a client.
        /// </summary>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        public static string GetImageUrl( string DocumentType, string curEnvironment = Utils.EnvironmentList.LOCAL )
        {

            string image = "";

            string logoPath = "";
            string logoName = "";
            string logoPathName = "";

            Utils.FCMenvironment = curEnvironment;


            switch (DocumentType)
            {
                case Utils.DocumentType.WORD:
                    logoName = FCMConstant.ImageFileName.Document;
                    break;

                case Utils.DocumentType.EXCEL:
                    logoName = FCMConstant.ImageFileName.Excel;
                    break;

                case Utils.DocumentType.FOLDER:
                    logoName = FCMConstant.ImageFileName.Folder;
                    break;

                case Utils.DocumentType.PDF:
                    logoName = FCMConstant.ImageFileName.PDF;
                    break;

                default:
                    logoName = FCMConstant.ImageFileName.Document;
                    break;
            }


            // Set no icon image if necessary
            //
            logoPath = FCMConstant.SYSFOLDER.LOGOFOLDER;
            logoName = logoName.Replace( FCMConstant.SYSFOLDER.LOGOFOLDER, string.Empty );
            
            logoPathName = Utils.getFilePathName( logoPath, logoName );

            return logoPathName;
        }

        /// <summary>
        /// Return sequence number of the client's logo from the image list
        /// </summary>
        /// <returns></returns>
        public static int GetClientLogoImageSeqNum(int clientUID)
        {
            int image = 0;

            foreach (var client in Utils.ClientList)
            {
                if (client.UID == clientUID)
                {
                    image = client.LogoImageSeqNum;
                    break;
                }
            }

            return image;

        }


        /// <summary>
        /// Get image for file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static int GetFileImage(char source, char destination, string documentType )
        {
            int image = FCMConstant.Image.WordFileSourceNoDestinationNo;

            if (source == 'Y')
            {
                if (destination == 'Y')
                {
                    // Source = "Y"; Destination = "Y"
                    switch (documentType)
                    {
                        case Utils.DocumentType.WORD:
                            image = FCMConstant.Image.WordFileSourceYesDestinationYes;
                            break;
                        case Utils.DocumentType.EXCEL:
                            image = FCMConstant.Image.ExcelFileSourceYesDestinationYes;
                            break;
                        case Utils.DocumentType.PDF:
                            image = FCMConstant.Image.PDFFileSourceYesDestinationYes;
                            break;
                        case Utils.DocumentType.FOLDER:
                            image = FCMConstant.Image.Folder;
                            break;
                        case Utils.DocumentType.APPENDIX:
                            image = FCMConstant.Image.Appendix;
                            break;
                    }
                }
                else
                {
                    // Source = "Y"; Destination = "N"
                    image = FCMConstant.Image.WordFileSourceYesDestinationNo;

                    switch (documentType)
                    {
                        case Utils.DocumentType.WORD:
                            image = FCMConstant.Image.WordFileSourceYesDestinationNo;
                            break;
                        case Utils.DocumentType.EXCEL:
                            image = FCMConstant.Image.ExcelFileSourceYesDestinationNo;
                            break;
                        case Utils.DocumentType.PDF:
                            image = FCMConstant.Image.PDFFileSourceYesDestinationNo;
                            break;
                        case Utils.DocumentType.FOLDER:
                            image = FCMConstant.Image.Folder;
                            break;
                        case Utils.DocumentType.APPENDIX:
                            image = FCMConstant.Image.Appendix;
                            break;
                    }

                }
            }
            else
            {
                if (destination == 'Y')
                {
                    // Source = "N"; Destination = "Y"
                    image = FCMConstant.Image.WordFileSourceNoDestinationYes;

                    switch (documentType)
                    {
                        case Utils.DocumentType.WORD:
                            image = FCMConstant.Image.WordFileSourceNoDestinationYes;
                            break;
                        case Utils.DocumentType.EXCEL:
                            image = FCMConstant.Image.ExcelFileSourceNoDestinationYes;
                            break;
                        case Utils.DocumentType.PDF:
                            image = FCMConstant.Image.PDFFileSourceNoDestinationYes;
                            break;
                        case Utils.DocumentType.FOLDER:
                            image = FCMConstant.Image.Folder;
                            break;
                        case Utils.DocumentType.APPENDIX:
                            image = FCMConstant.Image.Appendix;
                            break;
                    }

                }
                else
                {
                    // Source = "N"; Destination = "N"
                    image = FCMConstant.Image.WordFileSourceNoDestinationNo;

                    switch (documentType)
                    {
                        case Utils.DocumentType.WORD:
                            image = FCMConstant.Image.WordFileSourceNoDestinationNo;
                            break;
                        case Utils.DocumentType.EXCEL:
                            image = FCMConstant.Image.ExcelFileSourceNoDestinationNo;
                            break;
                        case Utils.DocumentType.PDF:
                            image = FCMConstant.Image.PDFFileSourceNoDestinationNo;
                            break;
                        case Utils.DocumentType.FOLDER:
                            image = FCMConstant.Image.Folder;
                            break;
                        case Utils.DocumentType.APPENDIX:
                            image = FCMConstant.Image.Appendix;
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

            if (Utils.UserSettingsCache == null)
                return valueReturned;

            foreach (var userSet in Utils.UserSettingsCache.ListOfUserSettings)
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

        /// <summary>
        /// Refresh cache
        /// </summary>
        /// <returns></returns>
        public static void RefreshCache()
        {
            Utils.UserSettingsCache.ListOfUserSettings.Clear();

            Utils.UserSettingsCache.ListOfUserSettings = UserSettings.List(Utils.UserID);
        }

        /// <summary>
        /// Refresh cache
        /// </summary>
        /// <returns></returns>
        public static void LoadUserSettingsInCache()
        {
            Utils.UserSettingsCache = new UserSettings();

            Utils.UserSettingsCache.ListOfUserSettings = UserSettings.List(Utils.UserID);
        }



    }
}
