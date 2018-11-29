using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.Client
{
    public class ClientExtraInformation
    {
        #region Properties
        public int UID { get; set; } // bigint
        public int FKClientUID { get; set; } // bigint
        public DateTime DateToEnterOnPolicies { get; set; } // date
        public string ScopeOfServices { get; set; } // varchar(200)

        [Required(ErrorMessage = "Action plan date is required.")]
        [Display(Name = "Action plan date")]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy")]
        public DateTime ActionPlanDate { get; set; } // date
        public DateTime CertificationTargetDate { get; set; } // date
        public string TimeTrading { get; set; } // varchar(200)
        public string RegionsOfOperation { get; set; } // varchar(200)
        public string OperationalMeetingsFrequency { get; set; } // varchar(50)
        public string ProjectMeetingsFrequency { get; set; } // varchar(50)

        public string IsVoid { get; set; }

        public string UserIdCreatedBy { get; set; }
        public string UserIdUpdatedBy { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int RecordVersion { get; set; }

        #endregion Properties

        #region Associations
        public Client client;
        #endregion Associations

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FKClientUID = "FKClientUID";
            public const string DateToEnterOnPolicies = "DateToEnterOnPolicies";
            public const string ScopeOfServices = "ScopeOfServices"; 
            public const string ActionPlanDate = "ActionPlanDate"; 
            public const string CertificationTargetDate = "CertificationTargetDate"; 
            public const string TimeTrading = "TimeTrading";
            public const string RegionsOfOperation = "RegionsOfOperation";
            public const string OperationalMeetingsFrequency = "OperationalMeetingsFrequency";
            public const string ProjectMeetingsFrequency = "ProjectMeetingsFrequency";
            //
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
            public const string RecordVersion = "RecordVersion";
            public const string IsVoid = "IsVoid";
        }


        /// <summary>
        /// Get Client Extra Information
        /// </summary>
        public void Read()
        {

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " +
                ClientFieldString() +
                "  FROM [management].[dbo].[ClientExtraInformation]" +
                " WHERE FKClientUID = @UID";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = this.FKClientUID;

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
        /// Get Client Extra Information
        /// </summary>
        public static ResponseStatus Read(HeaderInfo headerInfo, int clientUID)
        {

            ResponseStatus response = new ResponseStatus();
            response.ReturnCode = 1;
            response.ReasonCode = 1;
            response.Message = "Successful";

            ClientExtraInformation clientExtraInfo = new ClientExtraInformation();
            clientExtraInfo.FKClientUID = clientUID;
            clientExtraInfo.Read();

            response.Contents = clientExtraInfo;

            return response;

        }

        // -----------------------------------------------------
        //    Add new Client
        // -----------------------------------------------------
        public static ResponseStatus Insert(
            HeaderInfo headerInfo, 
            ClientExtraInformation clientExtraInfo,
            SqlConnection connection)
        {

            ResponseStatus response = new ResponseStatus();
            response.ReturnCode = 1;
            response.ReasonCode = 1;
            response.Message = "Client Added Successfully";

            int _uid = 0;

            int nextUID = GetLastUID() + 1; // 2010100000
            clientExtraInfo.UID = nextUID;
            DateTime _now = DateTime.Today;

            clientExtraInfo.RecordVersion = 1;
            clientExtraInfo.IsVoid = "N";
            clientExtraInfo.CreationDateTime = headerInfo.CurrentDateTime;
            clientExtraInfo.UpdateDateTime = headerInfo.CurrentDateTime;
            clientExtraInfo.UserIdCreatedBy = headerInfo.UserID;
            clientExtraInfo.UserIdUpdatedBy = headerInfo.UserID;

            var commandString =
            (
                "INSERT INTO [ClientExtraInformation] " +
                "(" +
                ClientFieldString() +
                ")" +
                    " VALUES " +
                "( @UID     " +
                ", @FKClientUID    " +
                ", @DateToEnterOnPolicies    " +
                ", @ScopeOfServices " +
                ", @ActionPlanDate " +
                ", @CertificationTargetDate " +
                ", @TimeTrading " +
                ", @RegionsOfOperation " +
                ", @OperationalMeetingsFrequency " +
                ", @ProjectMeetingsFrequency " +
                ", @IsVoid " +
                ", @RecordVersion " +
                ", @UpdateDateTime " +
                ", @UserIdUpdatedBy " +
                ", @CreationDateTime  " +
                ", @UserIdCreatedBy ) "
                );



            using (var command = new SqlCommand(commandString, connection))
            {
                clientExtraInfo.RecordVersion = 1;
                clientExtraInfo.IsVoid = "N";
                AddSQLParameters(command, FCMConstant.SQLAction.CREATE, clientExtraInfo);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                command.ExecuteNonQuery();
            }

            response.Contents = _uid;

            return response;
        }


        /// <summary>
        /// Update client details
        /// </summary>
        /// <returns></returns>
        internal static ResponseStatus Update(
                        HeaderInfo headerInfo, 
                        ClientExtraInformation clientExtraInfo)
        {

            var response = new ResponseStatus {ReturnCode = 1, ReasonCode = 1, Message = "Client Updated Successfully."};

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = (
                                        "UPDATE [ClientExtraInformation] " +
                                        " SET  " +
                                        FieldName.ActionPlanDate + " = @" + FieldName.ActionPlanDate + ", " +
                                        FieldName.CertificationTargetDate + " = @" + FieldName.CertificationTargetDate + ", " +
                                        FieldName.DateToEnterOnPolicies + " = @" + FieldName.DateToEnterOnPolicies + ", " +
                                        FieldName.OperationalMeetingsFrequency + " = @" + FieldName.OperationalMeetingsFrequency + ", " +
                                        FieldName.ProjectMeetingsFrequency + " = @" + FieldName.ProjectMeetingsFrequency + ", " +
                                        FieldName.RecordVersion + " = @" + FieldName.RecordVersion + ", " +
                                        FieldName.RegionsOfOperation + " = @" + FieldName.RegionsOfOperation + ", " +
                                        FieldName.ScopeOfServices + " = @" + FieldName.ScopeOfServices + ", " +
                                        FieldName.TimeTrading + " = @" + FieldName.TimeTrading + ", " +
                                        FieldName.IsVoid + " = @" + FieldName.IsVoid + ", " +
                                        FieldName.UpdateDateTime + " = @" + FieldName.UpdateDateTime + ", " +
                                        FieldName.UserIdUpdatedBy + " = @" + FieldName.UserIdUpdatedBy +
                                        "    WHERE FKClientUID = @FKClientUID "
                                    );


                clientExtraInfo.RecordVersion++;
                clientExtraInfo.UpdateDateTime = DateTime.Now;
                clientExtraInfo.UserIdUpdatedBy = headerInfo.UserID;
                clientExtraInfo.IsVoid = "N";

                using (var command = new SqlCommand(
                                            cmdText: commandString,
                                            connection: connection))
                {

                    AddSQLParameters(command, FCMConstant.SQLAction.UPDATE, clientExtraInfo);
                    
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
                        response.Message = "Error saving Client Extra Information. " + ex.ToString();
                        return response;
                    }
                }
            }
            return response;
        }


        /// <summary>
        /// Retrieve last UID
        /// </summary>
        /// <returns></returns>
        public static int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([UID]) LASTUID FROM [ClientExtraInformation]";

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
        /// Add SQL Parameters
        /// </summary>
        /// <param name="_uid"></param>
        /// <param name="command"></param>
        /// <param name="action"></param>
        private static void AddSQLParameters(SqlCommand command, string action, ClientExtraInformation clientExtraInfo)
        {

            command.Parameters.Add("@UID", SqlDbType.BigInt).Value = clientExtraInfo.UID;
            command.Parameters.Add("@FKClientUID", SqlDbType.VarChar).Value = clientExtraInfo.FKClientUID;
            command.Parameters.Add("@ActionPlanDate", SqlDbType.Date).Value = clientExtraInfo.ActionPlanDate;
            command.Parameters.Add("@CertificationTargetDate", SqlDbType.Date).Value = clientExtraInfo.CertificationTargetDate;
            command.Parameters.Add("@DateToEnterOnPolicies", SqlDbType.Date).Value = clientExtraInfo.DateToEnterOnPolicies;
            command.Parameters.Add("@OperationalMeetingsFrequency", SqlDbType.VarChar).Value = clientExtraInfo.OperationalMeetingsFrequency;
            command.Parameters.Add("@ProjectMeetingsFrequency", SqlDbType.VarChar).Value = clientExtraInfo.ProjectMeetingsFrequency;
            command.Parameters.Add("@RegionsOfOperation", SqlDbType.VarChar).Value = clientExtraInfo.RegionsOfOperation;
            command.Parameters.Add("@ScopeOfServices", SqlDbType.VarChar).Value = clientExtraInfo.ScopeOfServices;
            command.Parameters.Add("@TimeTrading", SqlDbType.VarChar).Value = clientExtraInfo.TimeTrading;

            command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime).Value = clientExtraInfo.UpdateDateTime;
            command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = clientExtraInfo.UserIdUpdatedBy;
            command.Parameters.Add("@IsVoid", SqlDbType.VarChar).Value = clientExtraInfo.IsVoid;
            command.Parameters.Add("@RecordVersion", SqlDbType.Int).Value = clientExtraInfo.RecordVersion;

            if (action == FCMConstant.SQLAction.CREATE)
            {
                command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime, 8).Value = clientExtraInfo.CreationDateTime;
                command.Parameters.Add("@UserIdCreatedBy", SqlDbType.VarChar).Value = clientExtraInfo.UserIdCreatedBy;
            }

        }


        /// <summary>
        /// Client string of fields.
        /// </summary>
        /// <returns></returns>
        private static string ClientFieldString()
        {
            string ret =
                        FieldName.UID
                + "," + FieldName.FKClientUID
                + "," + FieldName.DateToEnterOnPolicies
                + "," + FieldName.ScopeOfServices
                + "," + FieldName.ActionPlanDate
                + "," + FieldName.CertificationTargetDate
                + "," + FieldName.TimeTrading
                + "," + FieldName.RegionsOfOperation
                + "," + FieldName.OperationalMeetingsFrequency 
                + "," + FieldName.ProjectMeetingsFrequency 
                + "," + FieldName.IsVoid
                + "," + FieldName.RecordVersion
                + "," + FieldName.UpdateDateTime
                + "," + FieldName.UserIdUpdatedBy
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UserIdCreatedBy;

            return ret;
        }

        /// <summary>
        /// Load client object
        /// </summary>
        /// <param name="reader"></param>
        private static void LoadClientObject(SqlDataReader reader, ClientExtraInformation clientExtraInfo)
        {
            clientExtraInfo.UID = Convert.ToInt32(reader[FieldName.UID]);
            try { clientExtraInfo.ActionPlanDate = Convert.ToDateTime(reader[FieldName.ActionPlanDate].ToString()); }
            catch { clientExtraInfo.ActionPlanDate = Utils.MinDate; }
            try { clientExtraInfo.CertificationTargetDate = Convert.ToDateTime(reader[FieldName.CertificationTargetDate].ToString()); }
            catch { clientExtraInfo.CertificationTargetDate = Utils.MinDate; }
            try { clientExtraInfo.DateToEnterOnPolicies = Convert.ToDateTime(reader[FieldName.DateToEnterOnPolicies].ToString()); }
            catch { clientExtraInfo.DateToEnterOnPolicies = Utils.MinDate; }
            clientExtraInfo.FKClientUID = Convert.ToInt32(reader[FieldName.FKClientUID]);
            clientExtraInfo.OperationalMeetingsFrequency = (reader[FieldName.OperationalMeetingsFrequency].ToString());
            clientExtraInfo.ProjectMeetingsFrequency = (reader[FieldName.ProjectMeetingsFrequency].ToString());

            clientExtraInfo.RegionsOfOperation = (reader[FieldName.RegionsOfOperation].ToString());
            clientExtraInfo.ScopeOfServices = (reader[FieldName.ScopeOfServices].ToString());
            clientExtraInfo.TimeTrading = (reader[FieldName.TimeTrading].ToString());

            // Audit Info
            //
            try { clientExtraInfo.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString()); }
            catch { clientExtraInfo.UpdateDateTime = DateTime.Now; }
            try { clientExtraInfo.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString()); }
            catch { clientExtraInfo.CreationDateTime = DateTime.Now; }
            try { clientExtraInfo.IsVoid = reader[FieldName.IsVoid].ToString(); }
            catch { clientExtraInfo.IsVoid = "N"; }
            try { clientExtraInfo.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { clientExtraInfo.UserIdCreatedBy = "N"; }
            try { clientExtraInfo.UserIdUpdatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { clientExtraInfo.UserIdCreatedBy = "N"; }
            clientExtraInfo.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
        }
    }
}
