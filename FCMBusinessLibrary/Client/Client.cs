using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;
using FCMBusinessLibrary.Document;

namespace FCMBusinessLibrary.Client
{
    public partial class Client : EventArgs
    {
        #region Properties
        public int UID { get; set; }
        public int LogoImageSeqNum { get; set; }
        public string ABN { get; set; }

        [Required(ErrorMessage = "Client name is mandatory.")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string LegalName { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Logo1Location { get; set; }
        public string Logo2Location { get; set; }
        public string Logo3Location { get; set; }

        public string EmailAddress { get; set; }
        public string MainContactPersonName { get; set; }
        public char DisplayLogo { get; set; }
        public string FKUserID { get; set; }
        public int FKDocumentSetUID { get; set; }
        public string DocSetUIDDisplay { get; set; }

        public string UserIdCreatedBy { get; set; }
        public string UserIdUpdatedBy { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int RecordVersion { get; set; }
        public string IsVoid { get; set; }
        public ClientExtraInformation clientExtraInformation;

        public FCMConstant.DataBaseType databasetype;

        #endregion Properties

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string ABN = "ABN";
            public const string Name = "Name";
            public const string LegalName = "LegalName";
            public const string Address = "Address";
            public const string Phone= "Phone";
            public const string Fax = "Fax";
            public const string Mobile = "Mobile";
            public const string Logo1Location = "Logo1Location";
            public const string Logo2Location = "Logo2Location";
            public const string Logo3Location = "Logo3Location";
            public const string EmailAddress = "EmailAddress";
            public const string MainContactPersonName= "MainContactPersonName";
            public const string IsVoid = "IsVoid";
            public const string FKUserID = "FKUserID";
            public const string FKDocumentSetUID = "FKDocumentSetUID";
            public const string DocSetUIDDisplay = "DocSetUIDDisplay";
            public const string DisplayLogo = "DisplayLogo";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
            public const string RecordVersion = "recordVersion";

        }

        public Client(FCMConstant.DataBaseType dbtype = FCMConstant.DataBaseType.SQLSERVER)
        {
            databasetype = dbtype;
        }

        // -----------------------------------------------------
        //    Get Client details
        // -----------------------------------------------------
        internal void Read()
        {

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " + 
                ClientFieldString() +
                "  FROM [management].[dbo].[Client]" +
                " WHERE UID = @UID";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadClientObject(reader, this);
                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Check if user is already connected to a client
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus ReadLinkedUser()
        {

            var response = new ResponseStatus();

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " +
                ClientFieldString() +
                "  FROM [management].[dbo].[Client]" +
                " WHERE FKUserID = @FKUserID ";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKUserID", SqlDbType.BigInt).Value = FKUserID;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            int cliendUIDRead = Convert.ToInt32(reader[FieldName.UID]);
                            // If client uid read is the same as user id passed in
                            // no issues
                            if (cliendUIDRead == this.UID)
                            {
                                // User ID is already connect to a client.
                                // User is connected to client supplied
                                //
                                response.ReturnCode = 0001;
                                response.ReasonCode = 0003;
                                response.Message = "User is linked to client supplied.";
                            }
                            else
                            {
                                // User ID is already connect to a client.
                                // User is NOT connected to client supplied
                                //
                                response.ReturnCode = 0001;
                                response.ReasonCode = 0001;
                                response.Message = "User is linked to a different client.";
                            }
                            LoadClientObject(reader, this);
                        }
                        catch (Exception)
                        {
                            UID = 0;
                            response.ReturnCode = -0010;
                            response.ReasonCode = 0001;
                            response.Message = "Error retrieving client.";
                        }
                    }
                    else
                    {
                        UID = 0;
                        response.ReturnCode = 0001;
                        response.ReasonCode = 0010;
                        response.Message = "User ID is not linked to a Client.";
                    }
                }

            }

            return response;
        }

        /// <summary>
        /// Retrieve client's field information
        /// </summary>
        /// <param name="field">Field name - use Client.FieldName</param>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        internal static string ReadFieldClient(string field, int clientUID)
        {
            var ret = "";
            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                const string commandString = " SELECT @field FROM Client " +
                                             " WHERE UID = @clientUID ";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.Add("@field", SqlDbType.BigInt).Value = field;
                    command.Parameters.Add("@clientUID", SqlDbType.BigInt).Value = clientUID;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            ret = reader[field].ToString();
                        }
                        catch (Exception)
                        {
                            ret = "Error retrieving data. (ReadFieldClient) " + commandString;
                        }
                    }
                }
            }
            return ret;

        }


        // -----------------------------------------------------
        //    Get Client details
        // -----------------------------------------------------
        internal ResponseStatus Read(int iUID)
        {
            var response = new ResponseStatus();

            var retClient = new Client();

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " +
                ClientFieldString() +
                "  FROM [management].[dbo].[Client]" +
                " WHERE UID = @UID " ;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadClientObject(reader, retClient);
                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }

            }

            response.Contents = retClient;
            return response;
        }


        /// <summary>
        /// Add new Client
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus Insert(HeaderInfo headerInfo, SqlConnection connection = null)
        {

            var response = new ResponseStatus();
            response.ReturnCode = 1;
            response.ReasonCode = 1;
            response.Message = "Client Added Successfully";


            int uid = 0;

            int nextUID = GetLastUID() + 1; // 2010100000

            if (nextUID == 1)
            {
                nextUID = DateTime.Now.Year * 100000 + 1;
            }

            uid =  DateTime.Now.Year * 100000 + ( Convert.ToInt32( nextUID.ToString(CultureInfo.InvariantCulture).Substring(4,5) ) );

            UID = uid;

            RecordVersion = 1;

            if (String.IsNullOrEmpty( Name ))
            {
                response.ReturnCode = -10;
                response.ReasonCode = 1;
                 response.Message = "Error: Client Name is Mandatory.";
                return response;
            }
            if (Address == null)
                Address = "";

            if (MainContactPersonName == null)
                MainContactPersonName = "";

//            using (var connection = new SqlConnection(ConnString.ConnectionString))

            if (connection == null)
            {
                connection = new SqlConnection(ConnString.ConnectionString);
                connection.Open();
            }

            var commandString = 
            (
                "INSERT INTO [Client] " +
                "(" +
                ClientFieldString() +
                ")" +
                    " VALUES " +
                ClientFieldValue()

                ); 

            using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    RecordVersion = 1;
                    AddSqlParameters(command, FCMConstant.SQLAction.CREATE, headerInfo);

                    command.ExecuteNonQuery();
                }

            response.Contents = uid;

            return response;
        }

        /// <summary>
        /// Update client details
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus Update( HeaderInfo headerInfo )
        {

            var response = new ResponseStatus();
            response.ReturnCode = 1;
            response.ReasonCode = 1;
            response.Message = "Client Updated Successfully.";

            if (Name == null)
                Name = "";

            if (Address == null)
                Address = "";

            if (MainContactPersonName == null)
                MainContactPersonName = "";


            // Check record version. Do not allow update if version is different

            if (!IsTheSameRecordVersion(this.UID, this.RecordVersion))
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "Record updated previously by another user.";
                return response;
            }

            string commandString = (
                                             "UPDATE [Client] " +
                                             " SET  " +
                                             FieldName.ABN + " = @" + FieldName.ABN + ", " +
                                             FieldName.RecordVersion + " = @" + FieldName.RecordVersion + ", " +
                                             FieldName.Name + " = @" + FieldName.Name + ", " +
                                             FieldName.LegalName + " = @" + FieldName.LegalName + ", " +
                                             FieldName.Address + " = @" + FieldName.Address + ", " +
                                             FieldName.Phone + " = @" + FieldName.Phone + ", " +
                                             FieldName.Fax + " = @" + FieldName.Fax + ", " +
                                             FieldName.Mobile + " = @" + FieldName.Mobile + ", " +
                                             FieldName.Logo1Location + " = @" + FieldName.Logo1Location + ", " +
                                             FieldName.Logo2Location + " = @" + FieldName.Logo2Location + ", " +
                                             FieldName.Logo3Location + " = @" + FieldName.Logo3Location + ", " +
                                             FieldName.FKUserID + " = @" + FieldName.FKUserID + ", " +
                                             FieldName.FKDocumentSetUID + " = @" + FieldName.FKDocumentSetUID + ", " +
                                             FieldName.EmailAddress + " = @" + FieldName.EmailAddress + ", " +
                                             FieldName.MainContactPersonName + " = @" + FieldName.MainContactPersonName + ", " +
                                             FieldName.UpdateDateTime + " = @" + FieldName.UpdateDateTime + ", " +
                                             FieldName.UserIdUpdatedBy + " = @" + FieldName.UserIdUpdatedBy +
                                             "    WHERE UID = @UID "
                                         );



            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                using (var command = new SqlCommand(cmdText: commandString, connection: connection))
                {
                    RecordVersion++;
                    AddSqlParameters(command, FCMConstant.SQLAction.UPDATE, headerInfo);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), headerInfo.UserID);

                        response.ReturnCode = -0020;
                        response.ReasonCode = 0001;
                        response.Message = "Error saving client. " + ex.ToString();
                        return response;

                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Logical Delete client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        internal ResponseStatus Delete()
        {

            var response = new ResponseStatus();
            response.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000007;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [Client] " +
                   " SET  " +
                   "     [IsVoid] = @IsVoid " +
                   "    WHERE UID = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;
                    command.Parameters.Add("@IsVoid", SqlDbType.VarChar).Value = "Y";

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }



        // -----------------------------------------------------
        //          Retrieve last Client UID
        // -----------------------------------------------------
        private static int GetLastUID()
        {
            int lastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([UID]) LASTUID FROM [Client]";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            lastUID = Convert.ToInt32(reader["LASTUID"]);
                        }
                        catch (Exception)
                        {
                            lastUID = 0;
                        }
                    }
                }
            }

            return lastUID;
        }

        /// <summary>
        /// Check if the record version is the same. If it is not, deny update
        /// </summary>
        /// <param name="clientUID"></param>
        /// <param name="recordVersion"></param>
        /// <returns></returns>
        private static bool IsTheSameRecordVersion(int clientUID, int recordVersion)
        {
            // 
            // EA SQL database
            // 
            int currentVersion = 0;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT recordversion FROM [Client] WHERE UID = " + clientUID.ToString(CultureInfo.InvariantCulture);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            currentVersion = Convert.ToInt32(reader["recordversion"]);
                        }
                        catch (Exception)
                        {
                            currentVersion = 0;
                        }
                    }
                }
            }

            bool ret = false;
            if (currentVersion == 0 || currentVersion != recordVersion)
                ret = false;
            else
                ret = true;

            return ret;
        }

        // -----------------------------------------------------
        //    List clients
        // -----------------------------------------------------
        internal static List<Client> List(HeaderInfo headerInfo)
        {
            var clientList  = new List<Client>();

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString = string.Format(
                " SELECT " +
                ClientFieldString() +
                "   FROM [Client] " +
                "  WHERE IsVoid = 'N' " +
                "  ORDER BY UID ASC " 
                );

                using (var command = new SqlCommand(
                                      commandString, connection ))
                {
                    connection.Open();

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new Client();
                                Client.LoadClientObject(reader, client);

                                // Retrieve status of the logo. Enabled or disabled
                                //
                                var rmd = new ReportMetadata();
                                rmd.Read(client.UID, ReportMetadata.MetadataFieldCode.COMPANYLOGO);

                                client.DisplayLogo = rmd.Enabled;

                                clientList.Add(client);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        LogFile.WriteToTodaysLogFile(ex.ToString(), headerInfo.UserID, "", "Client.cs");

                    }
                }
            }
            return clientList;
        }


        /// <summary>
        /// Get Logo location for a client.
        /// </summary>
        /// <param name="clientUID"></param>
        /// <param name="curEnvironment"> </param>
        /// <returns></returns>
        public static string GetClientLogoLocation(int clientUID, string curEnvironment = Utils.EnvironmentList.LOCAL)
        {

            string logoPath = "";
            string logoName = "";
            string logoPathName = "";

            Utils.FCMenvironment = curEnvironment;

            // Get Company Logo
            //
            //ReportMetadata rmd = new ReportMetadata();
            //rmd.ClientUID = clientUID;
            //rmd.RecordType = Utils.MetadataRecordType.CLIENT;
            //rmd.FieldCode = "COMPANYLOGO";

            //rmd.Read(clientUID: clientUID, fieldCode: "COMPANYLOGO");

            Client client = new Client();
            client.UID = clientUID;
            client.Read();

            // Set no icon image if necessary
            //
            logoPath = FCMConstant.SYSFOLDER.LOGOFOLDER;
            logoName = "imgNoImage.jpg";

            if (client.Logo1Location != null)
            {
                logoName = client.Logo1Location.Replace(FCMConstant.SYSFOLDER.LOGOFOLDER, string.Empty);
            }

            logoPathName = Utils.getFilePathName(logoPath, logoName);

            return logoPathName;
        }


        /// <summary>
        /// Load client object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="client"> </param>
        private static void LoadClientObject(SqlDataReader reader, Client client)
        {
            client.UID = Convert.ToInt32(reader[FieldName.UID]);
            client.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
            client.ABN = reader[FieldName.ABN].ToString();
            client.Name = reader[FieldName.Name].ToString();
            client.LegalName = reader[FieldName.LegalName].ToString();
            client.Address = reader[FieldName.Address].ToString();
            client.EmailAddress = reader[FieldName.EmailAddress].ToString();
            client.Phone = reader[FieldName.Phone].ToString();
            try { client.FKUserID = reader[FieldName.FKUserID].ToString(); }
            catch { client.FKUserID = ""; }
            try { client.FKDocumentSetUID = Convert.ToInt32(reader[FieldName.FKDocumentSetUID]); }
            catch { client.FKDocumentSetUID = 0; }
            try { client.Fax = reader[FieldName.Fax].ToString(); }
            catch { client.Fax = ""; }
            try { client.Mobile = reader[FieldName.Mobile].ToString(); }
            catch { client.Mobile = ""; }
            try { client.Logo1Location = reader[FieldName.Logo1Location].ToString(); }
            catch { client.Logo1Location = ""; }
            try { client.Logo2Location = reader[FieldName.Logo2Location].ToString(); }
            catch { client.Logo2Location = ""; }
            try { client.Logo3Location = reader[FieldName.Logo3Location].ToString(); }
            catch { client.Logo3Location = ""; }

            try { client.MainContactPersonName = reader[FieldName.MainContactPersonName].ToString(); }
            catch { client.MainContactPersonName = ""; }
            try {  client.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString()); } 
            catch  { client.UpdateDateTime = DateTime.Now;  }
            try { client.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString()); }
            catch { client.CreationDateTime = DateTime.Now; }
            try { client.IsVoid = reader[FieldName.IsVoid].ToString(); }
            catch { client.IsVoid = "N"; }
            try { client.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { client.UserIdCreatedBy = "N"; }
            try { client.UserIdUpdatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { client.UserIdCreatedBy = "N"; }

            client.DocSetUIDDisplay = "0; 0";
            if (client.FKDocumentSetUID > 0)
            {
                DocumentSet ds = new DocumentSet();
                ds.UID = client.FKDocumentSetUID;
                ds.Read('N');
                client.DocSetUIDDisplay = ds.UID + "; " + ds.TemplateType;
            }

        }



        /// <summary>
        /// Add SQL Parameters
        /// </summary>
        /// <param name="_uid"></param>
        /// <param name="command"></param>
        /// <param name="action"></param>
        /// <param name="headerInfo"> </param>
        private void AddSqlParameters(SqlCommand command, string action, HeaderInfo headerInfo)
        {
            command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
            command.Parameters.Add("@ABN", SqlDbType.VarChar).Value = ABN;
            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Name;
            command.Parameters.Add("@LegalName", SqlDbType.VarChar).Value = LegalName;
            command.Parameters.Add("@Address", SqlDbType.VarChar).Value = Address;
            command.Parameters.Add("@Phone", SqlDbType.VarChar).Value = Phone;
            command.Parameters.Add("@EmailAddress", SqlDbType.VarChar).Value = EmailAddress;
            command.Parameters.Add("@Fax", SqlDbType.VarChar).Value = Fax;
            command.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = Mobile;
            command.Parameters.Add("@Logo1Location", SqlDbType.VarChar).Value = Logo1Location;
            command.Parameters.Add("@Logo2Location", SqlDbType.VarChar).Value = Logo2Location;
            command.Parameters.Add("@Logo3Location", SqlDbType.VarChar).Value = Logo3Location;
            command.Parameters.Add("@MainContactPersonName", SqlDbType.VarChar).Value = MainContactPersonName;
            command.Parameters.Add("@DisplayLogo", SqlDbType.VarChar).Value = DisplayLogo;
            command.Parameters.Add("@IsVoid", SqlDbType.VarChar).Value = "N";
            command.Parameters.Add("@FKUserID", SqlDbType.VarChar).Value = FKUserID;
            command.Parameters.Add("@FKDocumentSetUID", SqlDbType.BigInt).Value = FKDocumentSetUID;
            command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime, 8).Value = headerInfo.CurrentDateTime;
            command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = headerInfo.UserID;

            if (action == FCMConstant.SQLAction.CREATE)
            {
                command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime, 8).Value = headerInfo.CurrentDateTime;
                command.Parameters.Add("@UserIdCreatedBy", SqlDbType.VarChar).Value = headerInfo.UserID;
            }

            command.Parameters.Add("@recordVersion", SqlDbType.BigInt).Value = RecordVersion;

        }


        /// <summary>
        /// Client string of fields.
        /// </summary>
        /// <returns></returns>
        private static string ClientFieldString()
        {
            string ret =
                        FieldName.UID
                + "," + FieldName.ABN
                + "," + FieldName.Name
                + "," + FieldName.LegalName
                + "," + FieldName.Address
                + "," + FieldName.Phone
                + "," + FieldName.EmailAddress
                + "," + FieldName.Fax
                + "," + FieldName.Mobile
                + "," + FieldName.Logo1Location
                + "," + FieldName.Logo2Location
                + "," + FieldName.Logo3Location
                + "," + FieldName.MainContactPersonName
                + "," + FieldName.IsVoid
                + "," + FieldName.FKUserID
                + "," + FieldName.FKDocumentSetUID
                + "," + FieldName.UpdateDateTime
                + "," + FieldName.UserIdUpdatedBy
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UserIdCreatedBy
                + "," + FieldName.RecordVersion;

            return ret;
        }

            /// <summary>
        /// Client string of fields.
        /// </summary>
        /// <returns></returns>
        private static string ClientFieldValue()
        {
            string ret =
                "( @UID     " +
                ", @ABN    " +
                ", @Name    " +
                ", @LegalName    " +
                ", @Address " +
                ", @Phone " +
                ", @EmailAddress " +
                ", @Fax " +
                ", @Mobile " +
                ", @Logo1Location " +
                ", @Logo2Location " +
                ", @Logo3Location " +
                ", @MainContactPersonName " +
                ", @IsVoid " +
                ", @FKUserID " +
                ", @FKDocumentSetUID " +
                ", @UpdateDateTime " +
                ", @UserIdUpdatedBy " +
                ", @CreationDateTime  " +
                ", @UserIdCreatedBy " +
                ", @recordVersion ) ";

            return ret;
        }
    }

}
