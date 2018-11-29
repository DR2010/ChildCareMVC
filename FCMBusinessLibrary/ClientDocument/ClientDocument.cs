using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using FCMBusinessLibrary.ReferenceData;


namespace FCMBusinessLibrary
{
    public class ClientDocument
    {

        public ClientDocumentSet clientDocumentSet { get { return _clientDocumentSet; } set { _clientDocumentSet = value; } }
        public List<scClientDocSetDocLink> clientDocSetDocLink { get { return _clientDocSetDocLink; } set { _clientDocSetDocLink = value; } }

        #region properties
        public int UID { get { return _UID; } set { _UID = value; } }
        public string DocumentCUID { 
            get { return _DocumentCUID; } 
            set { 
                _DocumentCUID = value;

                if (_DocumentCUID.Trim().ToUpper() == "ROOT")
                    _IsRoot = 'Y';
                else
                    _IsRoot = 'N';
            } 
        }
        public int FKClientUID { get { return _FKClientUID; } set { _FKClientUID = value; } }
        public int FKClientDocumentSetUID { get { return _FKClientDocumentSetUID; } set { _FKClientDocumentSetUID = value; } }
        public int FKDocumentUID { get { return _FKDocumentUID; } set { _FKDocumentUID = value; } }
        public string SourceLocation { get { return _SourceLocation; } set { _SourceLocation = value; } }
        public string SourceFileName { get { return _SourceFileName; } set { _SourceFileName = value; } }
        public string Location { get { return _Location; } set { _Location = value; } }
        public string FileName { get { return _FileName; } set { _FileName = value; } }
        public int SourceIssueNumber { get { return _SourceIssueNumber; } set { _SourceIssueNumber = value; } }
        public int ClientIssueNumber { get { return _ClientIssueNumber; } set { _ClientIssueNumber = value; } }
        public int SequenceNumber { get { return _SequenceNumber; } set { _SequenceNumber = value; } }
        public char Generated { get { return _Generated; } set { _Generated = value; } }
        public char SourceFilePresent { get { return _SourceFilePresent; } set { _SourceFilePresent = value; } }
        public char DestinationFilePresent { get { return _DestinationFilePresent; } set { _DestinationFilePresent = value; } }
        public DateTime StartDate { get { return _StartDate; } set { _StartDate = value; } }
        public DateTime EndDate { get { return _EndDate; } set { _EndDate= value; } }
        public string RecordType { 
            get { return _RecordType; } 
            set 
            { 
                _RecordType = value;

                if (_RecordType.Trim() == Utils.RecordType.FOLDER)
                    _IsFolder = 'Y';
                else
                    IsFolder = 'N';

            } 
        }
        public int ParentUID { get { return _ParentUID; } set { _ParentUID = value; } }
        public string IsProjectPlan { get { return _IsProjectPlan; } set { _IsProjectPlan = value; } }
        public string DocumentType { get { return _DocumentType; } set { _DocumentType = value; } }
        public string ComboIssueNumber { get { return _ComboIssueNumber; } set { _ComboIssueNumber = value; } }
        public char IsVoid { get { return _IsVoid; } set { _IsVoid = value; } }
        public char IsRoot { get { return _IsRoot; } set { _IsRoot = value; } }
        public char IsFolder { get { return _IsFolder; } set { _IsFolder = value; } }

        #endregion properties


        #region attributes
        private int _UID;
        private string _DocumentCUID; 
        private int _FKClientUID; // denormalise?
        private int _FKClientDocumentSetUID;
        private int _FKDocumentUID;
        private string _SourceLocation;
        private string _SourceFileName;
        private string _Location;
        private string _FileName;
        private int _SourceIssueNumber;
        private int _ClientIssueNumber;
        private int _SequenceNumber;
        private char _Generated;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private string _RecordType;
        private int _ParentUID;
        private string _IsProjectPlan;
        private string _DocumentType;
        private string _ComboIssueNumber;
        private char _IsVoid;
        private char _IsRoot;
        private char _IsFolder;
        private char _SourceFilePresent;
        private char _DestinationFilePresent;
        private ClientDocumentSet _clientDocumentSet;
        private List<scClientDocSetDocLink> _clientDocSetDocLink;

        #endregion attributes

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string DocumentCUID = "DocumentCUID";
            public const string FKClientUID = "FKClientUID";
            public const string FKClientDocumentSetUID = "FKClientDocumentSetUID";
            public const string FKDocumentUID = "FKDocumentUID";

            public const string SourceLocation = "SourceLocation";
            public const string SourceFileName = "SourceFileName";
            public const string Location = "Location";
            public const string FileName = "FileName";
            public const string SourceIssueNumber = "SourceIssueNumber";
            public const string ClientIssueNumber = "ClientIssueNumber";
            public const string SequenceNumber = "SequenceNumber";
            public const string Generated = "Generated";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string RecordType = "RecordType";
            public const string ParentUID = "ParentUID";
            public const string IsProjectPlan = "IsProjectPlan";
            public const string DocumentType = "DocumentType";
            public const string ComboIssueNumber = "ComboIssueNumber";
            public const string IsVoid = "IsVoid";
            public const string IsRoot = "IsRoot";
            public const string IsFolder = "IsFolder";

            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
        }





        public ClientDocument()
        {
            clientDocumentSet = new ClientDocumentSet();
        }

        public void Get()
        {
        }

        // -----------------------------------------------------
        // Get Document for a client
        // This method locates a ClientDocument using DocumentUID
        // -----------------------------------------------------
        public bool Read()
        {

            ClientDocument clientDocument = new ClientDocument();
            bool ret = false;
            var rs = new ResponseStatus();
            rs.Message = "Client Document returned successfully.";
 
            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT " +
                  SQLConcat("CD") +
                "  FROM [ClientDocument] CD"  +
                " WHERE CD.UID = {0} "
                , this.UID
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        this.UID = Convert.ToInt32(reader["CDUID"].ToString());
                        this.FKClientUID = Convert.ToInt32(reader["CDFKClientUID"].ToString());
                        this.DocumentCUID = reader["CDDocumentCUID"].ToString();
                        this.ParentUID = Convert.ToInt32(reader["CDParentUID"].ToString());
                        this.FKDocumentUID = Convert.ToInt32(reader["CDFKDocumentUID"].ToString());
                        this.FKClientDocumentSetUID = Convert.ToInt32(reader["CDFKClientDocumentSetUID"].ToString());
                        this.SequenceNumber = Convert.ToInt32(reader["CDSequenceNumber"].ToString());
                        this.SourceLocation = reader["CDSourceLocation"].ToString();
                        this.SourceFileName = reader["CDSourceFileName"].ToString();
                        this.Location = reader["CDLocation"].ToString();
                        this.FileName = reader["CDFileName"].ToString();
                        this.StartDate = Convert.ToDateTime(reader["CDStartDate"].ToString());
                        try
                        {
                            this.EndDate = Convert.ToDateTime(reader["CDEndDate"].ToString());
                        }
                        catch (Exception)
                        {
                            this.EndDate = DateTime.MaxValue;
                        }

                        this.IsVoid = Convert.ToChar(reader["CDIsVoid"]);
                        this.IsProjectPlan = reader["CDIsProjectPlan"].ToString();
                        this.DocumentType = reader["CDDocumentType"].ToString();
                        this.RecordType = reader["CDRecordType"].ToString();

                        ret = true;
                    }
                }
            }

            // -----------------------------------------
            //       Populate client document set
            // -----------------------------------------
            this.clientDocumentSet.UID = clientDocument.FKClientDocumentSetUID;
            this.FKClientUID = clientDocument.FKClientUID;
            this.clientDocumentSet.Read();

            return ret;
        }

        // -----------------------------------------------------
        // Get Document for a client
        // This method locates a ClientDocument using DocumentUID
        // -----------------------------------------------------
        public static ResponseStatus ReadS(int clientDocumentUID)
        {

            var rs = new ResponseStatus();
            ClientDocument clientDocument = new ClientDocument();
            clientDocument.UID = clientDocumentUID;
            var ret = clientDocument.Read();

            if (ret) 
                rs.ReasonCode = 1;
            else
                rs.ReasonCode = 2;

            rs.Message = "Client Document returned successfully.";
            rs.Contents = clientDocument;

            return rs;

        }

        /// <summary>
        /// Get Document for a client. This method locates a ClientDocument using DocumentUID
        /// </summary>
        /// <param name="documentUID"></param>
        /// <param name="clientDocSetUID"></param>
        /// <param name="voidRead"></param>
        /// <returns></returns>
        public bool Find(int documentUID,
                    int clientDocSetUID,
                    char voidRead,
                    int clientUID)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;
            UID = 0;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT "+
                  SQLConcat("CD") +
                "  FROM [ClientDocument] CD" +
                " WHERE CD.FKDocumentUID = {0} " +
                " AND   CD.FKClientDocumentSetUID = {1} " +
                " AND   CD.IsVoid = '{2}' " +
                " AND   CD.FKClientUID = {3} "
                , documentUID
                , clientDocSetUID
                , voidRead
                , clientUID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        this.UID = Convert.ToInt32(reader["CDUID"].ToString());
                        this.FKDocumentUID = Convert.ToInt32(reader["CDFKDocumentUID"].ToString());
                        this.SequenceNumber = Convert.ToInt32(reader["CDSequenceNumber"].ToString());
                        this.FKClientDocumentSetUID = Convert.ToInt32(reader["CDFKClientDocumentSetUID"].ToString());
                        this.StartDate = Convert.ToDateTime(reader["CDStartDate"].ToString());

                        // Tentar....
                        //this.EndDate = (reader.IsDBNull(10) ? DateTime.MaxValue : Convert.ToDateTime(reader["CDEndDate"].ToString()));

                        // instead if this...

                        try
                        {
                            this.EndDate = Convert.ToDateTime(reader["CDEndDate"].ToString());
                        }
                        catch (Exception)
                        {
                            this.EndDate = DateTime.MaxValue;
                        }

                        this.IsVoid = Convert.ToChar(reader["CDIsVoid"]);

                        ret = true;
                    }
                }
            }
            return ret;

        }

        // -----------------------------------------------------
        //    Associate documents with document set
        // -----------------------------------------------------
        public void LinkDocumentListToSet(
            ListOfscClientDocSetDocLink docListToLink)
        {
            // for each document in the list
            // check if it is already linked with document set
            // if it is not linked, add a new link record
            // otherwise, ignore link.

            foreach (var doco in docListToLink.list)
            {
                LinkDocumentToClientSet(doco);
            }
        }


        // -----------------------------------------------------
        //    Associate documents with document set
        // -----------------------------------------------------
        public int LinkDocumentToClientSet( scClientDocSetDocLink doco)
        {
            int clientDocumentUID = 0;

            ClientDocument dslLocate = new ClientDocument();
            dslLocate.StartDate = DateTime.Today;
            dslLocate.IsVoid = 'N';
            dslLocate.IsProjectPlan = doco.clientDocument.IsProjectPlan;
            dslLocate.DocumentType = doco.clientDocument.DocumentType;
            dslLocate.FKDocumentUID = doco.document.UID;
            dslLocate.FKClientDocumentSetUID = doco.clientDocumentSet.UID;
            dslLocate.FKClientUID = doco.clientDocumentSet.FKClientUID;
            dslLocate.DocumentCUID = doco.clientDocument.DocumentCUID;
            dslLocate.SourceLocation = doco.clientDocument.SourceLocation;
            dslLocate.SourceFileName = doco.clientDocument.SourceFileName;
            dslLocate.Location = doco.clientDocument.Location;
            dslLocate.FileName = doco.clientDocument.FileName;
            dslLocate.SequenceNumber = doco.clientDocument.SequenceNumber;
            dslLocate.SourceIssueNumber = doco.clientDocument.SourceIssueNumber;
            dslLocate.Generated = 'N';
            dslLocate.ParentUID = doco.clientDocument.ParentUID;
            dslLocate.RecordType = doco.clientDocument.RecordType;
            dslLocate.IsRoot = doco.clientDocument.IsRoot;
            dslLocate.IsFolder = doco.clientDocument.IsFolder;

            // Prepare data to add or update
            ClientDocument dslAddUpdate = new ClientDocument();
            dslAddUpdate.StartDate = DateTime.Today;
            dslAddUpdate.IsVoid = 'N';
            dslAddUpdate.IsProjectPlan = doco.clientDocument.IsProjectPlan;
            dslAddUpdate.DocumentType = doco.clientDocument.DocumentType;
            dslAddUpdate.FKDocumentUID = doco.document.UID;
            dslAddUpdate.FKClientDocumentSetUID = doco.clientDocumentSet.UID;
            dslAddUpdate.FKClientUID = doco.clientDocumentSet.FKClientUID;
            dslAddUpdate.DocumentCUID = doco.clientDocument.DocumentCUID;
            dslAddUpdate.SourceLocation = doco.clientDocument.SourceLocation;
            dslAddUpdate.SourceFileName = doco.clientDocument.SourceFileName;
            dslAddUpdate.Location = doco.clientDocument.Location;
            dslAddUpdate.FileName = doco.clientDocument.FileName;
            dslAddUpdate.SequenceNumber = doco.clientDocument.SequenceNumber;
            dslAddUpdate.SourceIssueNumber = doco.clientDocument.SourceIssueNumber;
            dslAddUpdate.ClientIssueNumber = 0;
            dslAddUpdate.ComboIssueNumber = doco.clientDocument.ComboIssueNumber;
            dslAddUpdate.Generated = 'N';
            dslAddUpdate.ParentUID = doco.clientDocument.ParentUID;
            dslAddUpdate.RecordType = doco.clientDocument.RecordType;
            dslAddUpdate.IsRoot = doco.clientDocument.IsRoot;
            dslAddUpdate.IsFolder = doco.clientDocument.IsFolder;


            if (dslLocate.Find(doco.document.UID, doco.clientDocumentSet.UID, 'N', doco.clientDocumentSet.FKClientUID))
            {
                // Fact: There is an existing non-voided row
                // Intention (1): Make it void
                // Intention (2): Do nothing
                //

                // Check for Intention (1)
                //
                if (doco.clientDocument.IsVoid == 'Y')
                {
                    // Update row to make it voided...
                    //
                    SetToVoid( Utils.ClientID, doco.clientDocumentSet.UID, doco.clientDocument.UID );
                }
                else
                {
                    // Update details
                    //
                    dslAddUpdate.UID = doco.clientDocument.UID;

                    dslAddUpdate.Update();

                    clientDocumentUID = doco.clientDocument.UID;
                }

            }
            else
            {

                // if the pair does not exist, check if it is void.
                // If void = Y, just ignore.

                if (doco.clientDocument.IsVoid == 'Y')
                {
                    // just ignore. The pair was not saved initially.
                }
                else
                {
                    // add document to set

                    clientDocumentUID = dslAddUpdate.Add();

                }
            }

            return clientDocumentUID;
        }

        /// <summary>
        /// Calculate the number of documents in the set
        /// </summary>
        /// <param name="iClientID"></param>
        /// <returns></returns>
        public static int GetNumberOfDocuments(int clientUID, int clientDocumentSetUID)
        {
            int DocoCount = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                    "SELECT COUNT(*) SETCOUNT FROM [ClientDocument]" +
                    " WHERE FKClientUID = " + clientUID +
                    "   AND FKClientDocumentSetUID  = " + clientDocumentSetUID
                    ;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            DocoCount = Convert.ToInt32(reader["SETCOUNT"]);
                        }
                        catch (Exception)
                        {
                            DocoCount = 0;
                        }
                    }
                }
            }

            return DocoCount;
        }

        /// <summary>
        /// This method returns the complete logical path of a given Client Document.
        /// It walk through the structure, up, and then comes down
        /// It requires the root to be correctly set.
        /// </summary>
        /// <param name="clientDocument"></param>
        /// <returns></returns>
        public static ResponseStatus GetDocumentPath(ClientDocument clientDocument)
        {
            var rs = new ResponseStatus();
            List<ClientDocument> clientDocList = new List<ClientDocument>();

            string documentPath = "";

            // If root is supplied, return location
            //
            if (clientDocument.IsRoot == 'Y')
            {
                documentPath = clientDocument.Location;
                rs.Contents = documentPath;
                return rs;
            }

            int initialClientID = clientDocument.UID;
            int currentClientID = clientDocument.UID;

            int count = currentClientID;

            // walk up until it finds the root
            // 
            while (currentClientID > 0)
            {
                var clientObject = ClientDocument.ReadS(currentClientID);
                var client = (ClientDocument)clientObject.Contents;

                if (client == null)
                    break;
                if (client.RecordType == null)
                    break;

                clientDocList.Add(client);

                currentClientID = client.ParentUID;
            }

            // walk down building the path
            //
            for (int x = clientDocList.Count - 1;x > 0;x--)
            {
                var doco = clientDocList[x];
                if (doco.IsRoot == 'Y')
                {
                    documentPath = doco.Location + @"\" + doco.FileName;
                }
                else
                {
                    if (doco.IsFolder == 'Y')
                        documentPath = documentPath + @"\" + doco.FileName;
                }
            }


            rs.Contents = documentPath;

            return rs;
        }


        // -----------------------------------------------------
        //          Retrieve last Client UID
        // -----------------------------------------------------
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([UID]) LASTUID FROM [ClientDocument]";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LastUID = Convert.ToInt32(reader["LASTUID"]);
                        }
                        catch (Exception)
                        {
                            LastUID = 0;
                        }
                    }
                }
            }

            return LastUID;
        }

        // -----------------------------------------------------
        //        Retrieve last document id for a client
        // -----------------------------------------------------
        public static int GetLastClientCUID(int clientUID)
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT count(*) CNTCLIENT FROM [Document] WHERE FKClientUID = " + clientUID.ToString();

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LastUID = Convert.ToInt32(reader["CNTCLIENT"]);
                        }
                        catch (Exception)
                        {
                            LastUID = 0;
                        }
                    }
                }
            }

            return LastUID;
        }


        // -----------------------------------------------------
        //          Retrieve Unlink Document
        // -----------------------------------------------------
        public void UnlinkDocument(ClientDocument clientDocument)
        {
            // This method deletes a document set link
            //

            // 1) Look for connection that is not voided
            // 2) Update the IsVoid flag to "Y"; EndDate to Today

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   " [EndDate] = @EndDate" +
                   ",[IsVoid] = @IsVoid " +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Today;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        // -----------------------------------------------------
        //    Add root folder for client
        // -----------------------------------------------------
        public void AddRootFolder(int clientUID, int DocSetUID, string DestinationFolder)
        {
            this.RecordType = "FOLDER";
            this.ParentUID = 0;
            this.DocumentCUID = "ROOT";
            this.FKClientDocumentSetUID = DocSetUID;
            this.FKClientUID = clientUID;
            this.FKDocumentUID = 1;
            this.FileName = DestinationFolder;
            this.Location = FCMConstant.SYSFOLDER.CLIENTFOLDER;
            this.Generated = 'N';
            this.SourceIssueNumber = 1;
            this.SourceFileName = "Folder Source File Name";
            this.SourceLocation = "Folder Source File Location";
            this.StartDate = System.DateTime.Today;
            this.EndDate = System.DateTime.MaxValue;
            this.IsVoid = 'N';
            this.IsProjectPlan = "N";
            this.DocumentType = Utils.DocumentType.FOLDER;
            this.ComboIssueNumber = "Root";
            this.Add();

        }


        // -----------------------------------------------------
        //    Prepare root folder for client
        // -----------------------------------------------------
        public void PrepareRootFolder(int clientUID, int DocSetUID, string DestinationFolder)
        {
            this.RecordType = "FOLDER";
            this.ParentUID = 0;
            this.DocumentCUID = "ROOT";
            this.FKClientDocumentSetUID = DocSetUID;
            this.FKClientUID = clientUID;
            this.FKDocumentUID = 1;
            this.FileName = DestinationFolder;
            this.Location = FCMConstant.SYSFOLDER.CLIENTFOLDER;
            this.Generated = 'N';
            this.SourceIssueNumber = 1;
            this.SourceFileName = "Folder Source File Name";
            this.SourceLocation = "Folder Source File Location";
            this.StartDate = System.DateTime.Today;
            this.EndDate = System.DateTime.MaxValue;
            this.IsVoid = 'N';
            this.IsProjectPlan = "N";
            this.DocumentType = Utils.DocumentType.FOLDER;
            this.ComboIssueNumber = "Root";
        }

        // -----------------------------------------------------
        //    Add new Client Document
        // -----------------------------------------------------
        public int Add()
        {

            string ret = "Client Document Added Successfully";
            int _uid = 0;

            _uid = GetLastUID() + 1;

            // Default values
            DateTime _now = DateTime.Today;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (

                   "INSERT INTO [ClientDocument] " +
                   "( [UID], " +
                   "  [FKClientUID], " +
                   "  [FKClientDocumentSetUID], " +
                   "  [FKDocumentUID], " +
                   "  [DocumentCUID], " +
                   "  [SequenceNumber], " +
                   "  [SourceLocation], " +
                   "  [SourceFileName], " +
                   "  [Location], " +
                   "  [FileName], " +
                   "  [StartDate], " +
                   "  [EndDate], " +
                   "  [IsVoid], " +
                   "  [IsProjectPlan], " +
                   "  [DocumentType], " +
                   "  [Generated], " +
                   "  [SourceIssueNumber], " +
                   "  [ClientIssueNumber], " +
                   "  [ComboIssueNumber], " +
                   "  [ParentUID], " +
                   "  [RecordType], " +
                   "  [IsRoot], " +
                   "  [IsFolder] " +
                   ")" +
                        " VALUES " +
                   "( @UID     " +
                   ", @FKClientUID    " +
                   ", @FKClientDocumentSetUID " +
                   ", @FKDocumentUID " +
                   ", @DocumentCUID " +
                   ", @SequenceNumber " +
                   ", @SourceLocation " +
                   ", @SourceFileName " +
                   ", @Location " +
                   ", @FileName " +
                   ", @StartDate " +
                   ", @EndDate " +
                   ", @IsVoid " +
                   ", @IsProjectPlan " +
                   ", @DocumentType " +
                   ", @Generated " +
                   ", @SourceIssueNumber " +
                   ", @ClientIssueNumber " +
                   ", @ComboIssueNumber " +
                   ", @ParentUID " +
                   ", @RecordType " +
                   ", @IsRoot " +
                   ", @IsFolder " +
                   ")"
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = _uid;
                    command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = _FKClientUID;
                    command.Parameters.Add("@FKClientDocumentSetUID", SqlDbType.BigInt).Value = _FKClientDocumentSetUID;
                    command.Parameters.Add("@FKDocumentUID", SqlDbType.BigInt).Value = _FKDocumentUID;
                    command.Parameters.Add("@SequenceNumber", SqlDbType.BigInt).Value = _SequenceNumber;
                    command.Parameters.Add("@SourceLocation", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(SourceLocation)) ? " " : _SourceLocation;
                    command.Parameters.Add("@DocumentCUID", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(DocumentCUID)) ? " " : _DocumentCUID;
                    command.Parameters.Add("@SourceFileName", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(SourceFileName)) ? " " : _SourceFileName;
                    command.Parameters.Add("@Location", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(Location)) ? " " : _Location;
                    command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(FileName)) ? " " : _FileName;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = _now;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.MaxValue;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = _IsVoid;
                    command.Parameters.Add("@IsProjectPlan", SqlDbType.Char).Value = _IsProjectPlan;
                    command.Parameters.Add("@DocumentType", SqlDbType.Char).Value = _DocumentType;
                    command.Parameters.Add("@Generated", SqlDbType.Char).Value = _Generated;
                    command.Parameters.Add("@SourceIssueNumber", SqlDbType.Int).Value = _SourceIssueNumber;
                    command.Parameters.Add("@ClientIssueNumber", SqlDbType.Int).Value = _ClientIssueNumber;
                    command.Parameters.Add("@ComboIssueNumber", SqlDbType.Text).Value = _ComboIssueNumber;
                    command.Parameters.Add("@ParentUID", SqlDbType.Decimal).Value = _ParentUID;
                    command.Parameters.Add("@RecordType", SqlDbType.Char).Value = _RecordType;
                    command.Parameters.Add("@IsRoot", SqlDbType.Char).Value = _IsRoot;
                    command.Parameters.Add("@IsFolder", SqlDbType.Char).Value = _IsFolder;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return _uid;
        }

        // -----------------------------------------------------
        //    Update Client Document
        // -----------------------------------------------------
        public void Update()
        {

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   " [SequenceNumber] = @SequenceNumber" +
                   ",[SourceLocation] =  @SourceLocation " +
                   ",[SourceFileName] = @SourceFileName " +
                   ",[Location] = @Location" +
                   ",[FileName] = @FileName  " +
                   ",[ParentUID] = @ParentUID " +
                   ",[RecordType] = @RecordType " +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@SequenceNumber", SqlDbType.BigInt).Value = SequenceNumber;
                    command.Parameters.Add("@SourceLocation", SqlDbType.VarChar).Value = SourceLocation;
                    command.Parameters.Add("@SourceFileName", SqlDbType.VarChar).Value = SourceFileName;
                    command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                    command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;
                    command.Parameters.Add("@ParentUID", SqlDbType.BigInt).Value = ParentUID;
                    command.Parameters.Add("@RecordType", SqlDbType.VarChar).Value = RecordType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        // -----------------------------------------------------
        //    Update Client Document
        // -----------------------------------------------------
        public static void UpdateFieldString(int UID, string fieldName, string contents)
        {

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   fieldName + "= @contents "+
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@contents", SqlDbType.VarChar).Value = contents;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        // Physically Delete a file from windows
        public void DeleteFile()
        {
            this.Read();

            var fileNamePath = Utils.getFilePathName(clientDocumentSet.Folder + this.Location, this.FileName);

            if (File.Exists( fileNamePath ))
                File.Delete( fileNamePath );

            return;

        }

        //
        // Set void flag
        //
        public void SetToVoid( int clientUID, int clientDocumentSetUID, int documentUID )
        {
            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +    
                   " [IsVoid] = @IsVoid" +
                   "  WHERE  " +
                   "        FKClientDocumentSetUID = @FKClientDocumentSetUID    " +
                   "    AND FKClientUID = @FKClientUID  " +
                   "    AND FKDocumentUID = @FKDocumentUID " +
                   "    " 
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.Add( "@FKClientDocumentSetUID", SqlDbType.BigInt ).Value = clientDocumentSetUID;
                    command.Parameters.Add( "@FKClientUID", SqlDbType.BigInt ).Value = clientUID;
                    command.Parameters.Add( "@FKDocumentUID", SqlDbType.BigInt ).Value = documentUID;

                    command.Parameters.Add( "@IsVoid", SqlDbType.Char ).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        //
        // Set void flag
        //
        public ResponseStatus Delete( int clientUID, int clientDocumentSetUID, int documentUID )
        {
            ResponseStatus ret = new ResponseStatus();

            ret.Message = "Item updated successfully";
            ret.ReturnCode = 0001;
            ret.ReasonCode = 0001;

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "DELETE FROM [ClientDocument] " +
                   "  WHERE  " +
                   "        FKClientDocumentSetUID = @FKClientDocumentSetUID    " +
                   "    AND FKClientUID = @FKClientUID  " +
                   "    AND FKDocumentUID = @FKDocumentUID " +
                   "    "
                );


                try
                {
                    using (var command = new SqlCommand(
                                                commandString, connection ))
                    {

                        command.Parameters.Add( "@FKClientDocumentSetUID", SqlDbType.BigInt ).Value = clientDocumentSetUID;
                        command.Parameters.Add( "@FKClientUID", SqlDbType.BigInt ).Value = clientUID;
                        command.Parameters.Add( "@FKDocumentUID", SqlDbType.BigInt ).Value = documentUID;

                        command.Parameters.Add( "@IsVoid", SqlDbType.Char ).Value = 'Y';

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ret.Message = "Error deleting client document.";
                    ret.ReturnCode = -0010;
                    ret.ReasonCode =  0001;
                }
            }
            return ret;
        }

        /// <summary>
        /// Retrieve combo issue number
        /// </summary>
        /// <param name="documentCUID"></param>
        /// <param name="documentVersionNumber"></param>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        public static string GetComboIssueNumber(string documentCUID, int documentVersionNumber, int clientUID)
        {
            string comboIssueNumber = "";

            comboIssueNumber = documentCUID + '-' +
                           documentVersionNumber.ToString("00") + "-" +
                           clientUID.ToString("0000") + '-' +
                           "00"; // client issue number;

            return comboIssueNumber;
        }


        /// <summary>
        /// It sets all the destination names for a client document from source name.
        /// </summary>
        /// <param name="documentCUID"></param>
        /// <param name="documentVersionNumber"></param>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        public static ClientDocument SetClientDestinationFile(
                ClientDocument clientDocument,
                int clientUID,
                string documentCUID, 
                string sourceFileName,
                int sourceVersionNumber,
                string simpleFileName)
        {
            clientDocument._ComboIssueNumber = 
                           documentCUID + '-' +
                           sourceVersionNumber.ToString("00") + "-" +
                           clientUID.ToString("0000") + '-' +
                           "00"; // client issue number;

            clientDocument._FileName = clientDocument._ComboIssueNumber + " " +
                                       simpleFileName;
                                       
            return clientDocument;
        }

        //
        // Update document to Generated
        //
        public void SetGeneratedFlagVersion(char generated, decimal issueNumber)
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   " [Generated] = @Generated" +
                   " ,[SourceIssueNumber] = @SourceIssueNumber" +
                   " WHERE [UID] = @UID "
                ;

                using (var command = new SqlCommand(commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@Generated", SqlDbType.Char).Value = generated;
                    command.Parameters.Add( "@SourceIssueNumber", SqlDbType.Decimal ).Value = issueNumber;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        //
        // Update file name
        //
        public void UpdateFileName()
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   " [FileName] = @FileName" +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@FileName", SqlDbType.Char).Value = FileName;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        //
        // Update document to Generate
        //
        public void UpdateSourceFileName()
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   " [SourceFileName] = @SourceFileName" +
                   " ,[ComboIssueNumber] = @ComboIssueNumber" +
                   " ,[FileName] = @FileName" +
                   " ,[SourceIssueNumber] = @SourceIssueNumber" +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@SourceFileName", SqlDbType.Char).Value = SourceFileName;
                    command.Parameters.Add("@ComboIssueNumber", SqlDbType.Char).Value = ComboIssueNumber;
                    command.Parameters.Add("@FileName", SqlDbType.Char).Value = FileName;
                    command.Parameters.Add("@SourceIssueNumber", SqlDbType.BigInt).Value = SourceIssueNumber;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }
        // -----------------------------------------------------
        //    Create new client document version
        // -----------------------------------------------------
        public string NewVersion()
        {
            // string ClientDocSourceFolder

            // 1) Create a copy of current version in the version folder
            // 2) Create a Client Document Issue record to point to the versioned document in version folder
            // 3) Update the current client document with new details (file name, issue number etc)



            // Copy existing version to old folder version
            //
            // Old folder comes from %VERSIONFOLDER%
            // 
            var versionFolder = CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.VERSIONFOLDER);

            // Create a record to point to the old version
            //
            ClientDocumentVersion documentIssue = new ClientDocumentVersion();
            documentIssue.FKClientDocumentUID = this.UID;
            documentIssue.ClientIssueNumber = this.ClientIssueNumber; // Client Version
            documentIssue.SourceIssueNumber = this.SourceIssueNumber; // FCM Version
            documentIssue.IssueNumberText = documentIssue.ClientIssueNumber.ToString("0000");
            documentIssue.ComboIssueNumber = this.FileName.Substring(0,22);
            documentIssue.FKClientUID = this.FKClientUID;
            documentIssue.Location = Utils.GetVersionPath( "%VERSIONFOLDER%" + "\\Client" + this.FKClientUID.ToString( "000000000" ) + this.Location );
            documentIssue.FileName = this.FileName;

            documentIssue.Add();
            
            // Copy the current document into the version folder
            //
            // string sourceLocationFileName = Utils.getFilePathName(ClientDocSourceFolder + this.Location, this.FileName);
            string sourceLocationFileName = Utils.getFilePathName(this.Location, this.FileName);
            string destinationLocationFileName = Utils.getFilePathName( documentIssue.Location, documentIssue.FileName);
            string destinationLocation = Utils.GetPathName( documentIssue.Location);

            if (string.IsNullOrEmpty(sourceLocationFileName))
            {
                return "Location of file is empty. Please contact support.";
            }

            if (!System.IO.Directory.Exists(destinationLocation))
                System.IO.Directory.CreateDirectory(destinationLocation);

            File.Copy(sourceLocationFileName, destinationLocationFileName, true);


            // Generate the new version id

            // Increments issue number
            this.ClientIssueNumber++;

            // Create a new file name with version on it
            // POL-05-01-201000006-00 FILE NAME.doc
            // POL-05-01-XXXXHHHHH-YY FILE NAME.doc
            // |      |  |         |   
            // |      |  |         +---- Client Version
            // |      |  +--------- Client UID
            // |      +------------ FCM Version
            // +------------------- Document Identifier = CUID
            //
            string textversion = this.DocumentCUID + '-' + this.SourceIssueNumber.ToString("00") + "-" + 
                                 this.FKClientUID.ToString("000000000") + '-' + this.ClientIssueNumber.ToString("00");

            // FileName includes extension (.doc, .docx, etc.)
            //
            string newFileName = textversion + ' ' + this.FileName.Substring(23).Trim();

            // Copy file to new name
            //
            string newFilePathName = Utils.getFilePathName( this.Location, newFileName );

            File.Copy(sourceLocationFileName, newFilePathName, true);

            // Delete old version from main folder
            //
            File.Delete(sourceLocationFileName);

            // Update document details - version, name, etc
            // this.ClientIssueNumber = version;
            this.ComboIssueNumber = textversion;
            this.FileName = newFileName;
            this.UpdateVersion();

            return textversion;
        }


        // -----------------------------------------------------
        //    Update Document Version
        // -----------------------------------------------------
        private void UpdateVersion()
        {

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocument] " +
                   " SET " +
                   " [ClientIssueNumber] = @ClientIssueNumber" +
                   ",[Location] = @Location" +
                   ",[FileName] = @FileName" +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@ClientIssueNumber", SqlDbType.Decimal).Value = ClientIssueNumber;
                    command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                    command.Parameters.Add( "@FileName", SqlDbType.VarChar ).Value = FileName;
                    command.Parameters.Add( "@UID", SqlDbType.BigInt ).Value = UID;


                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }
    

        //
        // Listing...
        //

        //
        // List documents for a client
        //
        public void List(int clientID, int clientDocumentSetUID)
        {
            clientDocSetDocLink = new List<scClientDocSetDocLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var prefix = "CD";

                var commandString = string.Format(
                " SELECT " +
                SQLConcat(prefix) + 
                "   FROM [ClientDocument] "+ prefix + 
                "   WHERE " +
                "       IsVoid = 'N' " +
                "   AND FKClientUID = {0} " +
                "   AND FKClientDocumentSetUID = {1} " +
                "   ORDER BY ParentUID ASC, SequenceNumber ASC ",
                clientID,
                clientDocumentSetUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Ignore voids
                            if (Convert.ToChar(reader[prefix+FieldName.IsVoid]) == 'Y')
                                continue;

                            var docItem = SetDocumentItem(reader, prefix);

                            // Check if document exists
                            //

                            this.clientDocSetDocLink.Add(docItem);
                        }
                    }
                }
            }

        }

        //
        // (STATIC) List documents for a client 
        //
        public static List<scClientDocSetDocLink>  ListS(int clientID, int clientDocumentSetUID)
        {
            var clientDocSetDocLink = new List<scClientDocSetDocLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                string prefix = "CD";
                var commandString = string.Format(
                " SELECT " +
                SQLConcat(prefix) +
                "   FROM [ClientDocument] " + prefix +
                "   WHERE " +
                "       IsVoid = 'N' " +
                "   AND FKClientUID = {0} " +
                "   AND FKClientDocumentSetUID = {1} " +
                "   ORDER BY ParentUID ASC, SequenceNumber ASC ",
                clientID,
                clientDocumentSetUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Ignore voids
                            if (Convert.ToChar(reader[prefix + FieldName.IsVoid]) == 'Y')
                                continue;

                            var docItem = SetDocumentItem(reader, prefix);

                            clientDocSetDocLink.Add(docItem);
                        }
                    }
                }
            }
            return clientDocSetDocLink;
        }


        //
        // List documents for a client
        //
        public void ListImpacted(Document.Document document)
        {
            clientDocSetDocLink = new List<scClientDocSetDocLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                string prefix = "CD";
                var commandString = string.Format(
                " SELECT " +
                SQLConcat(prefix) +
                "   FROM [ClientDocument] " + prefix +
                "   WHERE FKDocumentUID = {0} ",
                document.UID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Ignore voids
                            if (Convert.ToChar(reader[prefix + FieldName.IsVoid]) == 'Y')
                                continue;

                            var docItem = SetDocumentItem(reader, prefix);

                            this.clientDocSetDocLink.Add(docItem);
                        }
                    }
                }
            }

        }

        // -----------------------------------------------------
        //    Load documents in a tree
        //    The list in tree expects that the list has been 
        //    called before to populate the instance
        // -----------------------------------------------------
        public void ListInTree(TreeView fileList, string listType)
        {

            // listType = CLIENT
            // listType = FCM = default;

            string ListType = listType;
            if (ListType == null)
                ListType = "FCM";


            foreach (var docLinkSet in this.clientDocSetDocLink)
            {
                // Check if folder has a parent

                string cdocumentUID = docLinkSet.clientDocument.UID.ToString();
                string cparentIUID = docLinkSet.clientDocument.ParentUID.ToString();
                TreeNode treeNode = new TreeNode();

                int image = 0;
                int imageSelected = 0;
                docLinkSet.clientDocument.RecordType = docLinkSet.clientDocument.RecordType.Trim();

                image = Utils.GetFileImage(docLinkSet.clientDocument.SourceFilePresent, docLinkSet.clientDocument.DestinationFilePresent, docLinkSet.clientDocument.DocumentType);
                imageSelected = image;

                //if (ListType == "CLIENT")
                //    treeNode = new TreeNode(docLinkSet.clientDocument.FileName, image, imageSelected);
                //else
                //    treeNode = new TreeNode(docLinkSet.document.Name, image, imageSelected);

                string treenodename = docLinkSet.document.DisplayName;

                if (string.IsNullOrEmpty(treenodename))
                    treenodename = docLinkSet.clientDocument.FileName;

                if (string.IsNullOrEmpty(treenodename))
                    treenodename = "Error: Name not found";

                treeNode = new TreeNode(treenodename, image, imageSelected);

                if (docLinkSet.clientDocument.ParentUID == 0)
                {

                    treeNode.Tag = docLinkSet;
                    treeNode.Name = cdocumentUID;
                    fileList.Nodes.Add(treeNode);

                }
                else
                {
                    // Find the parent node
                    //
                    var node = fileList.Nodes.Find(cparentIUID, true);

                    if (node.Count() > 0)
                    {

                        treeNode.Tag = docLinkSet;
                        treeNode.Name = cdocumentUID;

                        node[0].Nodes.Add(treeNode);
                    }
                    else
                    {
                        // Add Element to the root
                        //
                        treeNode.Tag = docLinkSet;
                        treeNode.Name = cdocumentUID;

                        fileList.Nodes.Add(treeNode);
                    }
                }
            }
        }

        // -----------------------------------
        //        List project plans
        // -----------------------------------
        public void ListProjectPlans(int clientID, int clientDocumentSetUID)
        {
            clientDocSetDocLink = new List<scClientDocSetDocLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                string prefix = "CD";
                var commandString = string.Format(
                " SELECT " +
                SQLConcat(prefix) + 
                "   FROM [ClientDocument] " +prefix +
                "   WHERE [FKClientUID] = {0} " +
                "   AND [FKClientDocumentSetUID] = {1} " +
                "   AND [IsProjectPlan] = 'Y' " +
                "   AND [IsVoid] = 'N' " +
                "   ORDER BY ParentUID ASC, SequenceNumber ASC ",
                clientID,
                clientDocumentSetUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Ignore voids
                            if (Convert.ToChar(reader[prefix + FieldName.IsVoid]) == 'Y')
                                continue;

                            var docItem = SetDocumentItem(reader, prefix);

                            this.clientDocSetDocLink.Add(docItem);
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Subset of code
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static scClientDocSetDocLink SetDocumentItem(SqlDataReader reader, string prefix)
        {
            var docItem = new scClientDocSetDocLink();

            // Get document
            //
            docItem.document = new Document.Document();
            docItem.document.UID = Convert.ToInt32(reader[prefix+FieldName.FKDocumentUID]);
            docItem.document.Read(includeVoid: true);

            // Get Client Document Set
            //
            docItem.clientDocumentSet = new ClientDocumentSet();
            docItem.clientDocumentSet.UID = Convert.ToInt32(reader[prefix+FieldName.FKClientDocumentSetUID].ToString());
            docItem.clientDocumentSet.FKClientUID = Convert.ToInt32(reader[prefix+FieldName.FKClientUID].ToString());
            docItem.clientDocumentSet.Read();

            // Set Client Document 
            //
            docItem.clientDocument = new ClientDocument();
            docItem.clientDocument.UID = Convert.ToInt32(reader[prefix + FieldName.UID].ToString());
            docItem.clientDocument.DocumentCUID = reader[prefix + FieldName.DocumentCUID].ToString();
            docItem.clientDocument.FKClientUID = Convert.ToInt32(reader[prefix + FieldName.FKClientUID].ToString());
            docItem.clientDocument.FKClientDocumentSetUID = Convert.ToInt32(reader[prefix + FieldName.FKClientDocumentSetUID].ToString());
            docItem.clientDocument.FKDocumentUID = Convert.ToInt32(reader[prefix + FieldName.FKDocumentUID].ToString());
            docItem.clientDocument.SourceLocation = reader[prefix + FieldName.SourceLocation].ToString();
            docItem.clientDocument.SourceFileName = reader[prefix + FieldName.SourceFileName].ToString();
            docItem.clientDocument.Location = reader[prefix + FieldName.Location].ToString();
            docItem.clientDocument.FileName = reader[prefix + FieldName.FileName].ToString();
            docItem.clientDocument.SourceIssueNumber = Convert.ToInt32(reader[prefix + FieldName.SourceIssueNumber].ToString());
            docItem.clientDocument.ClientIssueNumber = Convert.ToInt32(reader[prefix + FieldName.ClientIssueNumber].ToString());
            docItem.clientDocument.SequenceNumber = Convert.ToInt32(reader[prefix + FieldName.SequenceNumber].ToString());
            docItem.clientDocument.Generated = Convert.ToChar(reader[prefix + FieldName.Generated]);
            docItem.clientDocument.StartDate = Convert.ToDateTime(reader[prefix + FieldName.StartDate].ToString());
            docItem.clientDocument.RecordType = reader[prefix + FieldName.RecordType].ToString();
            docItem.clientDocument.RecordType = docItem.clientDocument.RecordType.Trim();

            docItem.clientDocument.ParentUID = Convert.ToInt32(reader[prefix + FieldName.ParentUID].ToString());
            docItem.clientDocument.IsProjectPlan = reader[prefix + FieldName.IsProjectPlan].ToString();
            docItem.clientDocument.DocumentType = reader[prefix + FieldName.DocumentType].ToString();
            docItem.clientDocument.ComboIssueNumber = reader[prefix + FieldName.ComboIssueNumber].ToString();
            docItem.clientDocument.IsVoid = Convert.ToChar(reader[prefix + FieldName.IsVoid].ToString());
            docItem.clientDocument.IsRoot = Convert.ToChar(reader[prefix + FieldName.IsRoot].ToString());
            docItem.clientDocument.IsFolder = Convert.ToChar(reader[prefix + FieldName.IsFolder].ToString());

            // SOURCE FILE is present?
            // 
            docItem.clientDocument.SourceFilePresent = 'N';

            if (string.IsNullOrEmpty(docItem.clientDocument.SourceLocation))
            {
                docItem.clientDocument.SourceFilePresent  = 'N';
            }
            else
            {
                string filePathName = Utils.getFilePathName(docItem.clientDocument.SourceLocation,
                                                            docItem.clientDocument.SourceFileName);
                // This is the source client file name
                //
                string clientSourceFileLocationName = Utils.getFilePathName(
                                docItem.clientDocument.SourceLocation.Trim(),
                                docItem.clientDocument.SourceFileName.Trim());

                if (File.Exists(clientSourceFileLocationName))
                {
                    docItem.clientDocument.SourceFilePresent = 'Y';
                }
            }

            // DESTINATION FILE is present?
            // 
            docItem.clientDocument.DestinationFilePresent = 'N';

            if (string.IsNullOrEmpty(docItem.clientDocument.Location))
            {
                docItem.clientDocument.DestinationFilePresent = 'N';
            }
            else
            {
                string filePathName = Utils.getFilePathName(docItem.clientDocument.Location,
                                                            docItem.clientDocument.FileName);
                // This is the destination client file name
                //
                string clientDestinationFileLocationName = Utils.getFilePathName(
                                docItem.clientDocument.Location.Trim(),
                                docItem.clientDocument.FileName.Trim());

                if (File.Exists(clientDestinationFileLocationName))
                {
                    docItem.clientDocument.DestinationFilePresent = 'Y';
                }
            }

            try
            {
                docItem.clientDocument.EndDate = Convert.ToDateTime(reader[prefix + FieldName.EndDate].ToString());
            }
            catch
            {
                docItem.clientDocument.EndDate = System.DateTime.MaxValue;
            }

            return docItem;
        }

        // -----------------------------------------------------
        //    Load project plan in a tree
        //    
        // -----------------------------------------------------
        public void ListProjectPlanInTree(int clientID, int clientDocumentSetUID, TreeView fileList)
        {

            int image = FCMConstant.Image.Document;
            int imageSelected = FCMConstant.Image.Document;

            ListProjectPlans(clientID, clientDocumentSetUID);

            foreach (var projectPlan in this.clientDocSetDocLink)
            {
                //
                // load plan in tree
                //
                var treeNode = new TreeNode(projectPlan.document.Name, image, imageSelected);
                treeNode.Tag = projectPlan;
                treeNode.Name = projectPlan.clientDocument.UID.ToString();
                fileList.Nodes.Add(treeNode);

                // List contents of the project plan
                //
                var cdl = new ClientDocumentLinkList();
                // cdl.ListChildrenDocuments( projectPlan.clientDocument.UID );

                if (clientID <= 0 || projectPlan.clientDocumentSet.UID <= 0 || projectPlan.document.UID <= 0)
                {
                    MessageBox.Show("Error listing children document. Please contact support");
                    return;
                }

                cdl.ListChildrenDocuments(clientUID: clientID,
                                           clientDocumentSetUID: projectPlan.clientDocumentSet.UID,
                                           documentUID: projectPlan.document.UID,
                                           type: Utils.DocumentLinkType.PROJECTPLAN);

                foreach (var planItem in cdl.clientDocumentLinkList)
                {
                    //
                    // load contents of the project plan in tree
                    //
                    var planItemNode = new TreeNode(planItem.childClientDocument.FileName, image, imageSelected);
                    planItemNode.Tag = planItem;
                    planItemNode.Name = planItem.childClientDocument.UID.ToString();
                    treeNode.Nodes.Add(planItemNode);
                }
            }
        }
        
        /// <summary>
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        public static string SQLConcat(string tablePrefix)
        {
            string ret = " " +

            tablePrefix + "."+ FieldName.UID + " " + tablePrefix + FieldName.UID + "," +
            tablePrefix + "."+ FieldName.DocumentCUID + " " + tablePrefix + FieldName.DocumentCUID + "," +
            tablePrefix + "."+ FieldName.FKClientUID + " " + tablePrefix + FieldName.FKClientUID + "," +
            tablePrefix + "."+ FieldName.FKClientDocumentSetUID + " " + tablePrefix + FieldName.FKClientDocumentSetUID + "," +
            tablePrefix + "."+ FieldName.FKDocumentUID + " " + tablePrefix + FieldName.FKDocumentUID + "," +
            tablePrefix + "."+ FieldName.SourceLocation + " " + tablePrefix + FieldName.SourceLocation + "," +
            tablePrefix + "."+ FieldName.SourceFileName + " " + tablePrefix + FieldName.SourceFileName + "," +
            tablePrefix + "."+ FieldName.Location + " " + tablePrefix + FieldName.Location + "," +
            tablePrefix + "."+ FieldName.FileName + " " + tablePrefix + FieldName.FileName + "," +
            tablePrefix + "."+ FieldName.SourceIssueNumber + " " + tablePrefix + FieldName.SourceIssueNumber + "," +
            tablePrefix + "."+ FieldName.ClientIssueNumber + " " + tablePrefix + FieldName.ClientIssueNumber + "," +
            tablePrefix + "."+ FieldName.SequenceNumber + " " + tablePrefix + FieldName.SequenceNumber + "," +
            tablePrefix + "."+ FieldName.Generated + " " + tablePrefix + FieldName.Generated + "," +
            tablePrefix + "."+ FieldName.StartDate + " " + tablePrefix + FieldName.StartDate + "," +
            tablePrefix + "."+ FieldName.EndDate + " " + tablePrefix + FieldName.EndDate + "," +
            tablePrefix + "."+ FieldName.RecordType + " " + tablePrefix + FieldName.RecordType + "," +
            tablePrefix + "."+ FieldName.ParentUID + " " + tablePrefix + FieldName.ParentUID + "," +
            tablePrefix + "."+ FieldName.IsProjectPlan + " " + tablePrefix + FieldName.IsProjectPlan + "," +
            tablePrefix + "."+ FieldName.DocumentType + " " + tablePrefix + FieldName.DocumentType + "," +
            tablePrefix + "."+ FieldName.ComboIssueNumber + " " + tablePrefix + FieldName.ComboIssueNumber + "," +
            tablePrefix + "."+ FieldName.IsVoid + " " + tablePrefix + FieldName.IsVoid + "," +
            tablePrefix + "."+ FieldName.IsRoot + " " + tablePrefix + FieldName.IsRoot + "," +
            tablePrefix + "."+ FieldName.IsFolder + " " + tablePrefix + FieldName.IsFolder;

            return ret;
        }
    }
}
