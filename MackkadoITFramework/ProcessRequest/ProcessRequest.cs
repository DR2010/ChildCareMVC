using System;
using System.Collections.Generic;
using FCMMySQLBusinessLibrary;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.ProcessRequest
{
    public class ProcessRequest
    {
       
        #region Fields 
        public int UID { get; set; }
        public string Description { get; set; }
        public int FKClientUID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string WhenToProcess { get; set; }
        public string RequestedByUser { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime StatusDateTime { get; set; }
        public DateTime PlannedDateTime { get; set; }

        public List<ProcessRequestArguments> argumentList { get; set; }

        #endregion Fields 

        #region Fields Building Blocks

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string Description = "Description";
            public const string FKClientUID = "FKClientUID";
            public const string Type = "Type";
            public const string Status = "Status";
            public const string WhenToProcess = "WhenToProcess";
            public const string RequestedByUser = "RequestedByUser";
            public const string CreationDateTime = "CreationDateTime";
            public const string StatusDateTime = "StatusDateTime";
            public const string PlannedDateTime = "PlannedDateTime";

        }

        /// <summary>
        /// Document string of fields.
        /// </summary>
        /// <returns></returns>
        private static string FieldString()
        {
            return (
                        FieldName.UID
                + "," + FieldName.Description
                + "," + FieldName.FKClientUID
                + "," + FieldName.Type
                + "," + FieldName.Status
                + "," + FieldName.WhenToProcess
                + "," + FieldName.RequestedByUser
                + "," + FieldName.CreationDateTime
                + "," + FieldName.PlannedDateTime
                + "," + FieldName.StatusDateTime
            );
        }

        /// <summary>
        /// Load from Reader
        /// </summary>
        /// <param name="processRequest"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="reader"></param>
        public static void LoadFromReader(
                               ProcessRequest processRequest,
                               MySqlDataReader reader)
        {

            processRequest.UID = Convert.ToInt32(reader[FieldName.UID].ToString());
            try
            {
                processRequest.FKClientUID = Convert.ToInt32(reader[FieldName.FKClientUID].ToString());
            }
            catch(Exception ex)
            {
                processRequest.FKClientUID = 0;
            }
            processRequest.Description = reader[FieldName.Description].ToString();
            processRequest.Status = reader[FieldName.Status].ToString();
            processRequest.Type = reader[FieldName.Type].ToString();
            processRequest.WhenToProcess = reader[FieldName.WhenToProcess].ToString();
            processRequest.RequestedByUser = reader[FieldName.RequestedByUser].ToString();
            processRequest.PlannedDateTime = Convert.ToDateTime(reader[FieldName.PlannedDateTime]);
            processRequest.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime]);
            processRequest.StatusDateTime = Convert.ToDateTime(reader[FieldName.StatusDateTime]);

            return;
        }

        #endregion Fields Building Blocks

        #region Permitted Values

        public enum StatusValue
        {
            OPEN, STARTED, COMPLETED, FAILED, ALL
        }

        public enum WhenToProcessValue
        {
            NOW, FUTURE
        }

        public enum TypeValue
        {
            DOCUMENTGENERATION
        }


        #endregion Permitted Values

        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessRequest()
        {
            this.WhenToProcess = ProcessRequest.WhenToProcessValue.NOW.ToString();
            this.Status = ProcessRequest.StatusValue.OPEN.ToString();
            this.PlannedDateTime = System.DateTime.Now;
            this.CreationDateTime = System.DateTime.Now;
            this.StatusDateTime = System.DateTime.Now;
            this.RequestedByUser = Helper.Utils.UserID;
        }


        /// <summary>
        /// Retrieve Last UID
        /// </summary>
        /// <returns></returns>
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString = "SELECT MAX(UID) LASTUID FROM ProcessRequest";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

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
        /// Add New Process Request
        /// </summary>
        public ResponseStatus Add()
        {
            ResponseStatus response = new ResponseStatus();

            int _uid = 0;

            _uid = GetLastUID() + 1;
            this.UID = _uid;

            DateTime _now = DateTime.Today;

            if (UID == null)
            {
                ResponseStatus responseError = new ResponseStatus(messageType: MessageType.Error);
                responseError.Message = "UID Not Supplied";
                return responseError;
            }

            if (Description == null)
            {
                ResponseStatus responseError = new ResponseStatus(messageType: MessageType.Error);
                responseError.Message = "Description Not Supplied";
                return responseError;
            }

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                        "INSERT INTO ProcessRequest " +
                        "( " +
                        FieldString() +
                        ")" +
                            " VALUES " +
                        "( "+
                    "  @" + FieldName.UID +
                    ", @" + FieldName.Description +
                    ", @" + FieldName.FKClientUID +
                    ", @" + FieldName.Type +
                    ", @" + FieldName.Status +
                    ", @" + FieldName.WhenToProcess +
                    ", @" + FieldName.RequestedByUser +
                    ", @" + FieldName.CreationDateTime +
                    ", @" + FieldName.PlannedDateTime +
                    ", @" + FieldName.StatusDateTime +
                    " )"
                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", MySqlDbType.Int32).Value = UID;
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@FKClientUID", MySqlDbType.Int32).Value = FKClientUID;
                    command.Parameters.Add("@Type", MySqlDbType.VarChar).Value = Type;
                    command.Parameters.Add("@Status", MySqlDbType.VarChar).Value = Status;
                    command.Parameters.Add("@WhenToProcess", MySqlDbType.VarChar).Value = WhenToProcess;
                    command.Parameters.Add("@RequestedByUser", MySqlDbType.VarChar).Value = RequestedByUser;
                    command.Parameters.Add("@CreationDateTime", MySqlDbType.DateTime).Value = CreationDateTime;
                    command.Parameters.Add("@PlannedDateTime", MySqlDbType.DateTime).Value = PlannedDateTime;
                    command.Parameters.Add("@StatusDateTime", MySqlDbType.DateTime).Value = StatusDateTime;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            response.Contents = _uid;
            return response;
        }

        /// <summary>
        /// Update Status to Completed
        /// </summary>
        /// <returns></returns>
        public ResponseStatus SetStatusToCompleted()
        {
            ResponseStatus ret = new ResponseStatus();
            ret.Message = "Item updated successfully";

            this.Status = ProcessRequest.StatusValue.COMPLETED.ToString();
            this.Update();

            return ret;
        }

        /// <summary>
        /// Update Status to Completed
        /// </summary>
        /// <returns></returns>
        public ResponseStatus SetStatusToStarted()
        {
            ResponseStatus ret = new ResponseStatus();
            ret.Message = "Item updated successfully";

            this.Status = ProcessRequest.StatusValue.STARTED.ToString();
            this.Update();

            return ret;
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Update()
        {
            ResponseStatus ret = new ResponseStatus();
            ret.Message = "Item updated successfully";

            this.StatusDateTime = System.DateTime.Now;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "UPDATE ProcessRequest " +
                   " SET  " +
                   FieldName.Status + " = @" + FieldName.Status + ", " +
                   FieldName.StatusDateTime + " = @" + FieldName.StatusDateTime +
                   "   WHERE    UID = @UID "
                );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.Add(FieldName.UID, MySqlDbType.VarChar).Value = UID;
                    command.Parameters.Add(FieldName.Status, MySqlDbType.VarChar).Value = Status;
                    command.Parameters.Add(FieldName.StatusDateTime, MySqlDbType.DateTime).Value = StatusDateTime;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return ret;
        }

        /// <summary>
        /// List requests
        /// </summary>
        /// <param name="StatusIn"></param>
        /// <returns></returns>
        public static List<ProcessRequest> List(ProcessRequest.StatusValue StatusIn)
        {
            var result = new List<ProcessRequest>();

            var checktype = " WHERE  Status = '" + StatusIn.ToString() + "'";

            if (StatusIn == ProcessRequest.StatusValue.ALL)
            {
                checktype = "";
            }

            using (var connection = new MySqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM    ProcessRequest " +
                checktype +
                "  ORDER BY 1 DESC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var _ProcessRequest = new ProcessRequest();
                            ProcessRequest.LoadFromReader(_ProcessRequest, reader);

                            // Load Arguments
                            //
                            _ProcessRequest.argumentList = ProcessRequestArguments.List(_ProcessRequest.UID);

                            result.Add(_ProcessRequest);
                        }
                    }
                }
            }

            return result;
        }

    }
}
