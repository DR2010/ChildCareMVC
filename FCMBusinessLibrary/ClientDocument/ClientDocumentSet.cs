using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;


namespace FCMBusinessLibrary
{
    public class ClientDocumentSet
    {
        public int UID { get; set; }
        public int FKClientUID { get; set; }
        public string Description { get; set; }
        public string CombinedIDName { get; set; }
        public string Folder { get; set; }
        public string FolderOnly { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string IsVoid { get; set; }
        public int ClientSetID { get; set; }
        public string SourceFolder { get; set; }
        public string Status { get; set; }
        public string UserIdCreatedBy { get; set; }
        public string UserIdUpdatedBy { get; set;  }
        public DateTime CreationDateTime { get; set;  }
        public DateTime UpdateDateTime { get; set ; }


        /// <summary>
        /// Constructor - Basic
        /// </summary>
        /// <param name="iClientUID"></param>
        /// <param name="iClientDocumentSetUID"></param>
        public ClientDocumentSet()
        {
        }

        /// <summary>
        /// Constructor retrieving client set information
        /// </summary>
        /// <param name="iClientUID"></param>
        /// <param name="iClientDocumentSetUID"></param>
        public ClientDocumentSet(int iClientUID, int iClientDocumentSetUID)
        {
            this.Get(iClientUID, iClientDocumentSetUID);
        }

        /// <summary>
        /// Constructor to set the client
        /// </summary>
        /// <param name="iClientUID"></param>
        /// <param name="iClientDocumentSetUID"></param>
        public ClientDocumentSet(int iClientUID)
        {
            FKClientUID = iClientUID;
        }


        
        // -----------------------------------------------------
        //    Get Client set
        // -----------------------------------------------------
        public bool Get(int iClientUID, int iClientDocumentSetUID)
        {
            bool ret = false;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " +
                ClientDocumentSetFieldString() +
                "  FROM [ClientDocumentSet]" +
                " WHERE " +
                " ClientSetID = " + iClientDocumentSetUID +
                " AND FKClientUID = " + iClientUID;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadObject(this, reader);
                            ret = true;
                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }
            }

            return ret;
        }

        // -----------------------------------------------------
        //    Get Client set using UID
        // -----------------------------------------------------
        public bool Read()
        {
            bool ret = false;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = String.Format(
                " SELECT " +
                ClientDocumentSetFieldString() +
                "  FROM [ClientDocumentSet]" +
                " WHERE " +
                " UID = {0} AND FKClientUID = {1}",
                this.UID,
                this.FKClientUID)
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
                            LoadObject(this, reader);
                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }
            }

            return ret;
        }

        // -----------------------------------------------------
        //   Retrieve last Document Set id
        // -----------------------------------------------------
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                    "SELECT MAX([UID]) LASTUID FROM [ClientDocumentSet]";

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
        //   Retrieve last Document Set for a client
        // -----------------------------------------------------
        private int GetLastUID(int iClientID)
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                    "SELECT FKClientUID , MAX([ClientSetID]) LASTUID FROM [ClientDocumentSet]" +
                    // "WHERE FKClientUID = " + iClientID +
                    "GROUP BY FKClientUID " +
                    "HAVING FKClientUID = " + iClientID;

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

        /// <summary>
        /// Calculate the number of documents in the set
        /// </summary>
        /// <param name="iClientID"></param>
        /// <returns></returns>
        public int GetNumberOfDocuments()
        {
            int DocoCount = 0;
            DocoCount = ClientDocument.GetNumberOfDocuments(this.FKClientUID, this.ClientSetID);

            return DocoCount;
        }


        // -----------------------------------------------------
        //    Add new Client Document Set
        // -----------------------------------------------------
        public ResponseStatus Add(HeaderInfo headerInfo, SqlConnection connection = null)
        {

            ResponseStatus response = new ResponseStatus();

            response.Message = "Client Document Set Added Successfully";

            UID = GetLastUID() + 1;
            ClientSetID = GetLastUID(this.FKClientUID) + 1;
            Description = "Client Set Number " + ClientSetID;
            IsVoid = "N";
            Status = "DRAFT";

            FolderOnly =
                "CLIENT" + FKClientUID.ToString().Trim() +
                "SET" + ClientSetID.ToString().Trim().PadLeft(4, '0');

            Folder = FCMConstant.SYSFOLDER.CLIENTFOLDER + @"\" + this.FolderOnly;

            CreationDateTime = headerInfo.CurrentDateTime;
            UpdateDateTime = headerInfo.CurrentDateTime;
            UserIdCreatedBy = headerInfo.UserID;
            UserIdUpdatedBy = headerInfo.UserID;

            // Default values
            DateTime _now = DateTime.Today;

            if (connection == null)
            {
                connection = new SqlConnection(ConnString.ConnectionString);
                connection.Open();
            }

            var commandString =
            (

                "INSERT INTO [ClientDocumentSet] " +
                "(" +
                ClientDocumentSetFieldString() +
                ")" +
                    " VALUES " +
                "( @UID     " +
                ", @FKClientUID    " +
                ", @ClientSetID    " +
                ", @Description " +
                ", @Folder " +
                ", @FolderOnly " +
                ", @Status " +
                ", @StartDate " +
                ", @EndDate " +
                ", @SourceFolder " +
                ", @IsVoid " +
                ", @CreationDateTime  " +
                ", @UpdateDateTime " +
                ", @UserIdCreatedBy " +
                ", @UserIdUpdatedBy " +
                ")"
            );

            using (var command = new SqlCommand(
                                        commandString, connection))
            {
                command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = FKClientUID;
                command.Parameters.Add("@ClientSetID", SqlDbType.BigInt).Value = ClientSetID;
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                command.Parameters.Add("@Folder", SqlDbType.VarChar).Value = Folder;
                command.Parameters.Add("@FolderOnly", SqlDbType.VarChar).Value = FolderOnly;
                command.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
                command.Parameters.Add("@SourceFolder", SqlDbType.VarChar).Value = SourceFolder;
                command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = _now;
                command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.MaxValue;
                command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = IsVoid;
                command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime).Value = CreationDateTime;
                command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime).Value = UpdateDateTime;
                command.Parameters.Add("@UserIdCreatedBy", SqlDbType.VarChar).Value = UserIdCreatedBy;
                command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = UserIdUpdatedBy;
                
                command.ExecuteNonQuery();
                }
            return response;
        }

        /// <summary>
        /// Create new document set. (Sub transaction)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqltransaction"></param>
        /// <param name="headerInfo"></param>
        /// <returns></returns>
        public ResponseStatus AddSubTransaction(            
                                    SqlConnection connection, 
                                    SqlTransaction sqltransaction,
                                    HeaderInfo headerInfo)
        {

            ResponseStatus response = new ResponseStatus();

            response.Message = "Client Document Set Added Successfully";

            this.UID = GetLastUID() + 1;
            this.ClientSetID = GetLastUID(this.FKClientUID) + 1;
            this.Description = "Client Set Number " + ClientSetID;
            this.IsVoid = "N";
            this.Status = "DRAFT";

            this.FolderOnly =
                "CLIENT" + this.FKClientUID.ToString().Trim() +
                   "SET" + this.ClientSetID.ToString().Trim().PadLeft(4, '0');

            this.Folder = FCMConstant.SYSFOLDER.CLIENTFOLDER + @"\" + this.FolderOnly;

            this.CreationDateTime = System.DateTime.Now;
            this.UpdateDateTime = System.DateTime.Now;
            this.UserIdCreatedBy = Utils.UserID;
            this.UserIdUpdatedBy = Utils.UserID;

            // Default values
            DateTime _now = DateTime.Today;

            var commandString =
            (

                "INSERT INTO [ClientDocumentSet] " +
                "(" +
                ClientDocumentSetFieldString() +
                ")" +
                    " VALUES " +
                "( @UID     " +
                ", @FKClientUID    " +
                ", @ClientSetID    " +
                ", @Description " +
                ", @Folder " +
                ", @FolderOnly " +
                ", @Status " +
                ", @StartDate " +
                ", @EndDate " +
                ", @SourceFolder " +
                ", @IsVoid " +
                ", @CreationDateTime  " +
                ", @UpdateDateTime " +
                ", @UserIdCreatedBy " +
                ", @UserIdUpdatedBy " +
                ")"
             );

            var command = new SqlCommand(commandString, connection, sqltransaction);

            command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
            command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = FKClientUID;
            command.Parameters.Add("@ClientSetID", SqlDbType.BigInt).Value = ClientSetID;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
            command.Parameters.Add("@Folder", SqlDbType.VarChar).Value = Folder;
            command.Parameters.Add("@FolderOnly", SqlDbType.VarChar).Value = FolderOnly;
            command.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
            command.Parameters.Add("@SourceFolder", SqlDbType.VarChar).Value = SourceFolder;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = _now;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.MaxValue;
            command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = IsVoid;
            command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime).Value = CreationDateTime;
            command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime).Value = UpdateDateTime;
            command.Parameters.Add("@UserIdCreatedBy", SqlDbType.VarChar).Value = UserIdCreatedBy;
            command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = UserIdUpdatedBy;

            command.ExecuteNonQuery();
            return response;
        }


        // -----------------------------------------------------
        //    Update Client Document Set
        // -----------------------------------------------------
        public void Update()
        {

            string ret = "Item updated successfully";

            // Default values
            this.UpdateDateTime = DateTime.Today;
            this.UserIdUpdatedBy = Utils.UserID;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocumentSet] " +
                   " SET " +
                   " [Description] =  @Description " +
                   ",[Folder] = @Folder " +
                   ",[SourceFolder] = @SourceFolder " +
                   ",[UpdateDateTime] = @UpdateDateTime " +
                   ",[UserIdUpdatedBy] = @UserIdUpdatedBy " +

                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;
                    command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@Folder", SqlDbType.VarChar).Value = Folder;
                    command.Parameters.Add("@SourceFolder ", SqlDbType.VarChar).Value = SourceFolder;
                    command.Parameters.Add("@UpdateDateTime ", SqlDbType.DateTime).Value = UpdateDateTime;
                    command.Parameters.Add("@UserIdUpdatedBy ", SqlDbType.VarChar).Value = UserIdUpdatedBy;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FKClientUID = "FKClientUID";
            public const string ClientSetID = "ClientSetID"; 
            public const string Description = "Description";
            public const string Folder = "Folder";
            public const string FolderOnly = "FolderOnly";
            public const string Status = "Status"; 
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string SourceFolder = "SourceFolder";
            public const string IsVoid = "IsVoid";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
        }


        /// <summary>
        /// Client string of fields.
        /// </summary>
        /// <returns></returns>
        private static string ClientDocumentSetFieldString()
        {
            return (
                        FieldName.UID
                + "," + FieldName.FKClientUID
                + "," + FieldName.ClientSetID
                + "," + FieldName.Description
                + "," + FieldName.Folder
                + "," + FieldName.FolderOnly
                + "," + FieldName.Status
                + "," + FieldName.StartDate
                + "," + FieldName.EndDate
                + "," + FieldName.SourceFolder
                + "," + FieldName.IsVoid
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UpdateDateTime
                + "," + FieldName.UserIdCreatedBy
                + "," + FieldName.UserIdUpdatedBy
            );

        }

        /// <summary>
        /// Load db data into memory
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reader"></param>
        private static void LoadObject(ClientDocumentSet obj, SqlDataReader reader)
        {
            obj.UID = Convert.ToInt32(reader[FieldName.UID]);
            obj.FKClientUID = Convert.ToInt32(reader[FieldName.FKClientUID]);
            try { obj.Description = reader[FieldName.Description].ToString(); }
            catch { obj.Description = ""; }
            obj.Folder = reader[FieldName.Folder].ToString();
            obj.FolderOnly = reader[FieldName.FolderOnly].ToString();
            obj.StartDate = Convert.ToDateTime(reader[FieldName.StartDate].ToString());
            obj.EndDate = Convert.ToDateTime(reader[FieldName.EndDate].ToString());
            obj.ClientSetID = Convert.ToInt32(reader[FieldName.ClientSetID].ToString());
            obj.SourceFolder = reader[FieldName.SourceFolder].ToString();
            obj.Status = reader[FieldName.Status].ToString();
            obj.IsVoid = reader[FieldName.IsVoid].ToString();
            
            // Derived field
            obj.CombinedIDName = obj.FKClientUID + ";" + obj.ClientSetID + "; " + obj.Description + "; " + obj.Status;
            
            try { obj.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString()); }
            catch { obj.UpdateDateTime = DateTime.Now; }
            try { obj.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString()); }
            catch { obj.CreationDateTime = DateTime.Now; }
            try { obj.IsVoid = reader[FieldName.IsVoid].ToString(); }
            catch { obj.IsVoid = "N"; }
            try { obj.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { obj.UserIdCreatedBy = "N"; }
            try { obj.UserIdUpdatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { obj.UserIdCreatedBy = "N"; }


        }


        /// <summary>
        /// Return a list of document sets for a given client.
        /// </summary>
        /// <param name="iClientUID"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public static List<ClientDocumentSet> List(int iClientUID, string sortOrder = "DESC")
        {
            List <ClientDocumentSet> documentSetList = new List<ClientDocumentSet>();

            // cds.FKClientUID + ";" + cds.ClientSetID + "; " + cds.Description + "; " +cds.Status
            //
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT  " +
                ClientDocumentSetFieldString() +
                "   FROM [ClientDocumentSet] " +
                " WHERE FKClientUID = '{0}' " +
                " ORDER BY ClientSetID " +
                sortOrder
                ,
                iClientUID);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientDocumentSet _clientDocumentSet = new ClientDocumentSet();
                            LoadObject(_clientDocumentSet, reader);

                            documentSetList.Add(_clientDocumentSet);

                        }
                    }
                }
            }

            return documentSetList;
        }
    }
}
