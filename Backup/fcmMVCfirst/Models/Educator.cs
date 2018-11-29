using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.ReferenceData;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace fcmMVCfirst.Models
{
    public class Educator
    {

        [Display(Name = "UID")]
        public int UID { get; set; }                     // bigint(20) NOT NULL,

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name must be supplied.")]
        [Display(Name = "First Name")]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; }            // varchar(100) DEFAULT NULL,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname must be supplied.")]
        [Display(Name = "Surname")]
        [StringLength(100, MinimumLength = 1)]
        public string Surname { get; set; }              // varchar(100) DEFAULT NULL,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname must be supplied.")]
        [Display(Name = "Worker Level")]
        [StringLength(10, MinimumLength = 1)]
        public string WLVL_Level { get; set; }              // varchar(10) DEFAULT NULL,

        public List<Educator> workerList;


        // --------------------------------------------------------// 
        public string UserIdCreatedBy { get; set; }
        public string UserIdUpdatedBy { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationDateTime { get; set; }

        [Display(Name = "Update Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDateTime { get; set; }

        public int RecordVersion { get; set; }
        public string IsVoid { get; set; }


        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        public void List(string userID)
        {
            workerList = new List<Educator>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM Worker " +
                "  WHERE IsVoid = 'N' " +
                "  ORDER BY UID ASC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();

                    try
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var worker = new Educator();
                                LoadObject(reader, worker);

                                workerList.Add(worker);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        LogFile.WriteToTodaysLogFile(ex.ToString(), userID, "", "Client.cs");

                    }
                }
            }
        }

        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        public static List<Educator> ListS()
        {
            var workerListX = new List<Educator>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM Worker " +
                "  WHERE IsVoid = 'N' " +
                "  ORDER BY UID ASC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();

                    try
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var worker = new Educator();
                                LoadObject(reader, worker);

                                workerListX.Add(worker);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        LogFile.WriteToTodaysLogFile(ex.ToString(), "", "", "Client.cs");

                    }
                }
            }

            return workerListX;
        }

        /// <summary>
        /// Retrieve last UID
        /// </summary>
        /// <returns></returns>
        private static int GetLastUID()
        {
            int lastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                var commandString = "SELECT MAX(UID) LASTUID FROM Worker";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    try
                    {
                        connection.Open();

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            try
                            {
                                lastUID = Convert.ToInt32(reader["LASTUID"]);
                            }
                            catch (Exception ex)
                            {
                                string error = ex.ToString();
                                LogFile.WriteToTodaysLogFile("Last UID set to ZERO. " +
                                    ex.ToString(), HeaderInfo.Instance.UserID, "", "Child.cs");

                                lastUID = 0;
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        LogFile.WriteToTodaysLogFile(ex2.ToString());
                    }
                }
            }

            return lastUID;
        }

        /// <summary>
        /// Add Worker
        /// </summary>
        public WorkerAddResponse Add(WorkerAddRequest workerAddRequest)
        {
            var workerAddResponse = new WorkerAddResponse();
            workerAddResponse.XResponseStatus = new ResponseStatus();

            this.UserIdCreatedBy = workerAddRequest.XHeaderInfo.UserID;
            this.UserIdUpdatedBy = workerAddRequest.XHeaderInfo.UserID;

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                UID = GetLastUID() + 1;
                RecordVersion = 1;

                var commandString =
                    (
                        "INSERT INTO Worker (" +
                        FieldString() +
                        ")" +
                        " VALUES " +
                        "( " +
                        "  @" + FieldName.UID +
                        " ,@" + FieldName.FirstName +
                        " ,@" + FieldName.Surname +
                        " ,@" + FieldName.WLVL_Level +
                        ", @" + FieldName.UpdateDateTime +
                        ", @" + FieldName.UserIdUpdatedBy +
                        ", @" + FieldName.CreationDateTime +
                        ", @" + FieldName.UserIdCreatedBy +
                        ", @" + FieldName.RecordVersion +
                        ", @" + FieldName.IsVoid +
                        " )"
                    );

                using (var command = new MySqlCommand(
                    commandString, connection))
                {
                    AddSqlParameters(command);

                    command.Parameters.Add("@" + FieldName.CreationDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@" + FieldName.UserIdCreatedBy, MySqlDbType.VarChar).Value = this.UserIdCreatedBy;
                    command.Parameters.Add("@" + FieldName.IsVoid, MySqlDbType.VarChar).Value = "N";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), workerAddRequest.XHeaderInfo.UserID);

                        workerAddResponse.XResponseStatus.ReturnCode = -0025;
                        workerAddResponse.XResponseStatus.ReasonCode = 0001;
                        workerAddResponse.XResponseStatus.Message = "Error adding child. " + ex;
                        return workerAddResponse;
                    }

                    return workerAddResponse;
                }
            }
        }

        /// <summary>
        /// Update child details
        /// </summary>
        /// <returns></returns>
        public WorkerUpdateResponse Update(WorkerUpdateRequest workerAddRequest)
        {
            WorkerUpdateResponse workerUpdateResponse = new WorkerUpdateResponse();
            workerUpdateResponse.XResponseStatus = new ResponseStatus();

            // Check record version. Do not allow update if version is different

            if (! CommonDB.IsTheSameRecordVersion(  tablename: "Worker", 
                                                    inputUID: UID,
                                                    recordVersion: RecordVersion,
                                                    responseStatus: workerUpdateResponse.XResponseStatus))
            {
                return workerUpdateResponse;
            }

            string commandString = (
                                             "UPDATE Worker " +
                                             " SET  " +
                                             FieldName.UID + "= @" + FieldName.UID + ", " +
                                             FieldName.FirstName + "= @" + FieldName.FirstName + ", " +
                                             FieldName.Surname + "= @" + FieldName.Surname + ", " +
                                             FieldName.WLVL_Level + "= @" + FieldName.WLVL_Level + ", " +
                                             FieldName.UpdateDateTime + "= @" + FieldName.UpdateDateTime + ", " +
                                             FieldName.UserIdUpdatedBy + "= @" + FieldName.UserIdUpdatedBy + ", " +
                                             FieldName.RecordVersion + "= @" + FieldName.RecordVersion + ", " +
                                             FieldName.IsVoid + "= @" + FieldName.IsVoid +

                                             "    WHERE UID = @UID "
                                         );



            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                using (var command = new MySqlCommand(cmdText: commandString, connection: connection))
                {
                    RecordVersion++;
                    AddSqlParameters(command);

                    try
                    {
                        command.Parameters.AddWithValue("@" + FieldName.IsVoid, "N");

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), workerAddRequest.XHeaderInfo.UserID);

                        workerUpdateResponse.XResponseStatus.ReturnCode = -0020;
                        workerUpdateResponse.XResponseStatus.ReasonCode = 0001;
                        workerUpdateResponse.XResponseStatus.Message = "Error saving client. " + ex;
                        return workerUpdateResponse;

                    }
                }
            }

            return workerUpdateResponse;
        }

        /// <summary>
        /// Get Employee details
        /// </summary>
        public void Read()
        {
            // 
            // EA SQL database
            // 
            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                var commandString =
                " SELECT " +
                FieldString()
                + "  FROM Worker" +
                " WHERE UID = " + this.UID;

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadObject(reader, this);

                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }
            }

        }

        private void AddSqlParameters(MySqlCommand command)
        {
            command.Parameters.Add("@" + FieldName.UID, MySqlDbType.Int32).Value = UID;
            command.Parameters.Add("@" + FieldName.FirstName, MySqlDbType.VarChar).Value = FirstName;
            command.Parameters.Add("@" + FieldName.Surname, MySqlDbType.VarChar).Value = Surname;
            command.Parameters.Add("@" + FieldName.WLVL_Level, MySqlDbType.VarChar).Value = WLVL_Level;
            command.Parameters.Add("@" + FieldName.UpdateDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@" + FieldName.UserIdUpdatedBy, MySqlDbType.VarChar).Value = UserIdUpdatedBy;

            command.Parameters.Add("@" + FieldName.RecordVersion, MySqlDbType.Int32).Value = RecordVersion;

        }

        private static string FieldString()
        {
            return
                "  " + FieldName.UID +
                " ," + FieldName.FirstName +
                " ," + FieldName.Surname +
                " ," + FieldName.WLVL_Level+

                " ," + FieldName.UpdateDateTime +
                " ," + FieldName.UserIdUpdatedBy +
                " ," + FieldName.CreationDateTime +
                " ," + FieldName.UserIdCreatedBy +
                " ," + FieldName.RecordVersion +
                " ," + FieldName.IsVoid;
        }

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FirstName = "FirstName";
            public const string Surname = "Surname";
            public const string WLVL_Level = "WLVL_Level";

            public const string UpdateDateTime = "UpdateDateTime";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string RecordVersion = "RecordVersion";
            public const string IsVoid = "IsVoid";
        }

        /// <summary>
        /// Load Object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="worker"></param>
        private static void LoadObject(MySqlDataReader reader, Educator worker)
        {
            worker.UID = Convert.ToInt32(reader[FieldName.UID]);
            worker.FirstName = reader[FieldName.FirstName] as string;
            worker.Surname = reader[FieldName.Surname] as string;
            worker.WLVL_Level = reader[FieldName.WLVL_Level] as string;
            // -----------------------------------------------------------------------------------------
            worker.UpdateDateTime = Convert.ToDateTime((reader[FieldName.UpdateDateTime]));
            worker.UserIdUpdatedBy = reader[FieldName.UserIdUpdatedBy] as string;
            worker.CreationDateTime = Convert.ToDateTime((reader[FieldName.CreationDateTime]));
            worker.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy] as string;
            worker.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
            worker.IsVoid = reader[FieldName.IsVoid] as string;

        }
    }


    public class WorkerAddRequest
    {
        public HeaderInfo XHeaderInfo;
        public Educator XEducator;
    }

    public class WorkerAddResponse
    {
        public ResponseStatus XResponseStatus;
        public Educator XEducator;
    }


    public class WorkerUpdateRequest
    {
        public HeaderInfo XHeaderInfo;
        public Educator XEducator;
    }

    public class WorkerUpdateResponse
    {
        public ResponseStatus XResponseStatus;
        public Educator XEducator;
    }
}