using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.DataAccess
{
    public class ClientContract
    {
        
        #region Properties

        public int FKCompanyUID { get { return _FKCompanyUID; } set { _FKCompanyUID = value; } }
        public int UID { get { return _UID; } set { _UID = value; } }
        public string ExternalID { get { return _ExternalID; } set { _ExternalID = value; } }
        public string Status { get { return _Status; } set { _Status = value; } }
        public string Type { get { return _Type; } set { _Type = value; } }
        public DateTime StartDate { get { return _StartDate; } set { _StartDate = value; } }
        public DateTime EndDate { get { return _EndDate; } set { _EndDate = value; } }
        public string UserIdCreatedBy { get { return _UserIdCreatedBy; } set { _UserIdCreatedBy = value; } }
        public string UserIdUpdatedBy { get { return _UserIdUpdatedBy; } set { _UserIdUpdatedBy = value; } }
        public DateTime CreationDateTime { get { return _CreationDateTime; } set { _CreationDateTime = value; } }
        public DateTime UpdateDateTime { get { return _UpdateDateTime; } set { _UpdateDateTime = value; } }

        #endregion Properties

        #region Attributes
       
        private int _FKCompanyUID;
        private int _UID;
        private string _ExternalID;
        private string _Status;
        private string _Type;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private DateTime _CreationDateTime;
        private DateTime _UpdateDateTime;
        private string _UserIdCreatedBy;
        private string _UserIdUpdatedBy;

        #endregion Attributes

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string FKCompanyUID = "FKCompanyUID";
            public const string UID = "UID";
            public const string ExternalID = "ExternalID";
            public const string Status = "Status";
            public const string Type = "Type";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
        }


        // var student1 = new Student{firstName = "Bruce", lastName = "Willis"};

        /// <summary>
        /// Get Employee details
        /// </summary>
        internal static ClientContract Read(int clientContractUID)
        {
            // 
            // EA SQL database
            // 

            ClientContract clientContract = new ClientContract();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " +
                ClientContractFieldsString()
                + "  FROM [ClientContract]" +
                " WHERE UID = @UID";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();

                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = clientContractUID;

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadClientContractObject(reader, clientContract);

                        }
                        catch (Exception)
                        {
                            clientContract.UID = 0;
                        }
                    }
                }
            }

            return clientContract;
        }

        /// <summary>
        /// List client contracts
        /// </summary>
        /// <param name="clientID"></param>
        internal static List<ClientContract> List(int clientID)
        {
            List<ClientContract> clientContractList = new List<ClientContract>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                ClientContractFieldsString() +
                "   FROM [ClientContract] " +
                "   WHERE  FKCompanyUID = {0}",
                clientID);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientContract _clientContract = new ClientContract();
                            LoadClientContractObject(reader, _clientContract);

                            clientContractList.Add(_clientContract);
                        }
                    }
                }
            }

            return clientContractList;
        }

        // -----------------------------------------------------
        //          Retrieve last Contract UID
        // -----------------------------------------------------
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX(UID) LASTUID FROM [ClientContract] ";

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
        /// Add new contract
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus Insert()
        {
            var rs = new ResponseStatus();
            rs.Message = "Client Contract Added Successfully";
            rs.ReturnCode = 1;
            rs.ReasonCode = 1;

            int _uid = 0;

            _uid = GetLastUID() + 1;
            this.UID = _uid;

            DateTime _now = DateTime.Today;
            this.CreationDateTime = _now;
            this.UpdateDateTime = _now;

            if (ExternalID == null)
                ExternalID = "";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [ClientContract] " +
                   "( " +
                    ClientContractFieldsString() +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @" + FieldName.FKCompanyUID +
                   ", @" + FieldName.UID +
                   ", @" + FieldName.ExternalID +
                   ", @" + FieldName.Status +
                   ", @" + FieldName.Type +
                   ", @" + FieldName.StartDate +
                   ", @" + FieldName.EndDate +
                   ", @" + FieldName.UserIdCreatedBy +
                   ", @" + FieldName.UserIdUpdatedBy +
                   ", @" + FieldName.CreationDateTime +
                   ", @" + FieldName.UpdateDateTime +
                   " )"
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    AddSQLParameters(command);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return rs;
        }

        /// <summary>
        /// Update Contract
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus Update()
        {
            ResponseStatus ret = new ResponseStatus();
            ret.Message = "Item updated successfully";

            if (ExternalID == null)
                ExternalID = "";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientContract] " +
                   " SET  " +
                   FieldName.ExternalID + " = @" + FieldName.ExternalID + ", " +
                   FieldName.UpdateDateTime + " = @" + FieldName.UpdateDateTime + ", " +
                   FieldName.Type + " = @" + FieldName.Type + ", " +
                   FieldName.UserIdUpdatedBy + " = @" + FieldName.UserIdUpdatedBy + ", " +
                   FieldName.Status + " = @" + FieldName.Status +
                   "   WHERE    UID = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    AddSQLParameters(command);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return ret;
        }

        /// <summary>
        /// Delete Employee 
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Delete()
        {

            var ret = new ResponseStatus();
            ret.Message = "Employee Deleted successfully";

            if (ExternalID == null)
                ExternalID = "";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "DELETE [ClientContract] " +
                   "   WHERE    UID = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return ret;
        }

        /// <summary>
        /// Add Employee parameters to the SQL Command.
        /// </summary>
        /// <param name="command"></param>
        private void AddSQLParameters(SqlCommand command)
        {
            command.Parameters.Add(FieldName.FKCompanyUID, SqlDbType.BigInt).Value = FKCompanyUID;
            command.Parameters.Add(FieldName.UID, SqlDbType.BigInt).Value = UID;
            command.Parameters.Add(FieldName.ExternalID, SqlDbType.VarChar).Value = ExternalID;
            command.Parameters.Add(FieldName.Status, SqlDbType.VarChar).Value = Status;
            command.Parameters.Add(FieldName.Type, SqlDbType.VarChar).Value = Type;
            command.Parameters.Add(FieldName.StartDate, SqlDbType.DateTime).Value = StartDate;
            command.Parameters.Add(FieldName.EndDate, SqlDbType.DateTime).Value = EndDate;
            command.Parameters.Add(FieldName.UserIdCreatedBy, SqlDbType.Char).Value = UserIdCreatedBy;
            command.Parameters.Add(FieldName.UserIdUpdatedBy, SqlDbType.Char).Value = UserIdUpdatedBy;
            command.Parameters.Add(FieldName.CreationDateTime, SqlDbType.DateTime).Value = CreationDateTime;
            command.Parameters.Add(FieldName.UpdateDateTime, SqlDbType.DateTime).Value = UpdateDateTime;
        }


        private static string ClientContractFieldsString()
        {
            return (
                        FieldName.FKCompanyUID
                + "," + FieldName.UID
                + "," + FieldName.ExternalID
                + "," + FieldName.Status
                + "," + FieldName.Type
                + "," + FieldName.StartDate
                + "," + FieldName.EndDate
                + "," + FieldName.UserIdUpdatedBy
                + "," + FieldName.UserIdCreatedBy
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UpdateDateTime
                );

        }

        /// <summary>
        /// This method loads the information from the sqlreader into the Employee object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="clientContract"> </param>
        private static void LoadClientContractObject(SqlDataReader reader, ClientContract clientContract)
        {
            clientContract.FKCompanyUID = Convert.ToInt32(reader[FieldName.FKCompanyUID]);
            clientContract.UID = Convert.ToInt32(reader[FieldName.UID].ToString());
            clientContract.ExternalID = reader[FieldName.ExternalID].ToString();
            try { clientContract.Status = reader[FieldName.Status].ToString(); }
            catch { clientContract.Status = "ACTIVE"; }
            try { clientContract.Type = reader[FieldName.Type].ToString(); }
            catch { clientContract.Type = "BASIC"; }
            try { clientContract.StartDate = Convert.ToDateTime(reader[FieldName.StartDate].ToString()); }
            catch { clientContract.StartDate = DateTime.Now; }
            try { clientContract.EndDate = Convert.ToDateTime(reader[FieldName.EndDate].ToString()); }
            catch { clientContract.EndDate = DateTime.Now; }

            clientContract.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString();
            clientContract.UserIdUpdatedBy = reader[FieldName.UserIdUpdatedBy].ToString();

            try { clientContract.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString()); }
            catch { clientContract.UpdateDateTime = DateTime.Now; }
            try { clientContract.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString()); }
            catch { clientContract.CreationDateTime = DateTime.Now; }
        
        }

        public static ResponseStatus GetValidContractOnDate(int clientID, DateTime date)
        {
            ResponseStatus ret = new ResponseStatus();

            ret.Message = "Valid contract not found.";
            ret.ReturnCode = 0001;
            ret.ReasonCode = 0002;
            ret.UniqueCode = ResponseStatus.MessageCode.Warning.FCMWAR00000002;

            ClientContract clientContract = new ClientContract();

            string dateString = date.ToString().Substring(0, 10);

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                " SELECT " +
                ClientContractFieldsString() +
                "   FROM [ClientContract] " +
                "   WHERE  FKCompanyUID  = @FKCompanyUID " +
                "     AND  StartDate    <= @StartDate    " +
                "     AND  EndDate      >= @EndDate      ";

                using (var command = new SqlCommand(commandString, connection))
                {

                    command.Parameters.Add("@FKCompanyUID", SqlDbType.BigInt).Value = clientID;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = date;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = date;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientContract _clientContract = new ClientContract();
                            LoadClientContractObject(reader, _clientContract);

                            clientContract = _clientContract;

                            ret.Message = "Valid contract found.";
                            ret.ReturnCode = 0001;
                            ret.ReasonCode = 0001;
                            ret.XMessageType = MessageType.Informational;
                            ret.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;

                            break;
                        }
                    }
                }
            }

            ret.Contents = clientContract;

            return ret;
        }


    }
}
