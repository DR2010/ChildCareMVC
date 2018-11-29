using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace fcmMVCfirst.Models
{
    public class Room
    {
        #region properties
        
        [Display(Name = "UID")]
        public int UID { get; set; }                     // bigint(20) NOT NULL,

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name must be supplied.")]
        [Display(Name = "Name")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }            // varchar(100) DEFAULT NULL,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description must be supplied.")]
        [Display(Name = "Description")]
        [StringLength(100, MinimumLength = 1)]
        public string Description { get; set; }              // varchar(100) DEFAULT NULL,

        public List<Room> roomList;

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

        private HeaderInfo _headerInfo { get; set; }

        #endregion properties

        // ----------------------------------------------
        // Public methods
        // ----------------------------------------------

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
                var commandString = "SELECT MAX(UID) LASTUID FROM Room";

                using (var command = new MySqlCommand(commandString, connection))
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
                                    ex.ToString(), HeaderInfo.Instance.UserID, "", "Room.cs");

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
                + "  FROM Room" +
                " WHERE UID = @UID ";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.AddWithValue("@UID", this.UID);

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
        
        /// <summary>
        /// Add Room
        /// </summary>
        public ResponseStatus Add(string userID)
        {
            this.UserIdCreatedBy = userID;
            this.UserIdUpdatedBy = userID;

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                UID = GetLastUID() + 1;
                RecordVersion = 1;

                var commandString =
                    (
                        "INSERT INTO Room (" +
                        FieldString() +
                        ")" +
                        " VALUES " +
                        "( " +
                        "  @" + FieldName.UID                +
                        " ,@" + FieldName.Name               +
                        " ,@" + FieldName.Description     +
                        ", @" + FieldName.UpdateDateTime     +
                        ", @" + FieldName.UserIdUpdatedBy    +
                        ", @" + FieldName.CreationDateTime   +
                        ", @" + FieldName.UserIdCreatedBy    +
                        ", @" + FieldName.RecordVersion      +
                        ", @" + FieldName.IsVoid             +
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
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        return new ResponseStatus() { ReturnCode = -0025, ReasonCode = 0001, Message = "Error adding Room. " + ex };

                    }

                    return new ResponseStatus();
                }
            }
        }

        /// <summary>
        /// Update Room details
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Update(string userID)
        {

            // Check record version. Do not allow update if version is different

            string commandString = (
                                             "UPDATE room " +
                                             " SET  " +
                                             FieldName.UID                     +  "= @" + FieldName.UID                 + ", " +
                                             FieldName.Name                    +  "= @" + FieldName.Name                + ", " +
                                             FieldName.Description          +  "= @" + FieldName.Description      + ", " +
                                             FieldName.UpdateDateTime          +  "= @" + FieldName.UpdateDateTime      + ", " +
                                             FieldName.UserIdUpdatedBy         +  "= @" + FieldName.UserIdUpdatedBy     + ", " +
                                             FieldName.RecordVersion           +  "= @" + FieldName.RecordVersion       + ", " +
                                             FieldName.IsVoid                  +  "= @" + FieldName.IsVoid              +          
  
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
                        //command.Parameters.Add("@" + FieldName.UpdateDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
                        //command.Parameters.Add("@" + FieldName.UserIdUpdatedBy, MySqlDbType.VarChar).Value = userID;
                        command.Parameters.AddWithValue("@" + FieldName.IsVoid, 'N');

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        return new ResponseStatus() { ReturnCode = -0020, ReasonCode = 0001, Message = "Error saving client. " + ex};
                    }
                }
            }

            return new ResponseStatus();
        }

        /// <summary>
        /// Logical Delete room
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public ResponseStatus Delete()
        {

            using (var connection = new MySqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE Room " +
                   " SET  " +
                   "     IsVoid = @IsVoid " +
                   "    WHERE UID = @UID "
                );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", MySqlDbType.VarChar).Value = UID;
                    command.Parameters.Add("@IsVoid", MySqlDbType.VarChar).Value = "Y";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        return new ResponseStatus() { ReturnCode = -0022, ReasonCode = 0001, Message = "Error deleting Room. " + ex };

                    }
                }
            }
            return new ResponseStatus();
        }

        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        public void List(string userID)
        {
            roomList = new List<Room>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM Room " +
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
                                var client = new Room();
                                LoadObject(reader, client);

                                roomList.Add(client);

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
        /// List Rooms
        /// </summary>
        /// <param name="userID"></param>
        public static List<Room> ListS()
        {
            var roomListX = new List<Room>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM Room " +
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
                                var client = new Room();
                                LoadObject(reader, client);

                                roomListX.Add(client);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        LogFile.WriteToTodaysLogFile(ex.ToString(), "", "", "Client.cs");

                    }
                }

                return roomListX;
            }
        }


        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string Name = "Name";
            public const string Description = "Description";
            public const string UpdateDateTime = "UpdateDateTime";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string RecordVersion = "RecordVersion";
            public const string IsVoid = "IsVoid";             
        }

        // ----------------------------------------------
        // Private methods
        // ----------------------------------------------

        /// <summary>
        /// Check if the record version is the same. If it is not, deny update
        /// </summary>
        /// <param name="inputUID"></param>
        /// <param name="recordVersion"></param>
        /// <returns></returns>
        private static bool IsTheSameRecordVersion(int inputUID, int recordVersion)
        {
            // 
            // EA SQL database
            // 
            int currentVersion = 0;

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                var commandString = "SELECT recordversion FROM Room WHERE UID = " + inputUID.ToString(CultureInfo.InvariantCulture);

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

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

        private void AddSqlParameters(MySqlCommand command)
        {
            command.Parameters.Add("@" + FieldName.UID, MySqlDbType.Int32).Value = UID;
            command.Parameters.Add("@" + FieldName.Name, MySqlDbType.VarChar).Value = Name;
            command.Parameters.Add("@" + FieldName.Description, MySqlDbType.VarChar).Value = Description;
            command.Parameters.Add("@" + FieldName.UpdateDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@" + FieldName.UserIdUpdatedBy, MySqlDbType.VarChar).Value = UserIdUpdatedBy;

            command.Parameters.Add("@" + FieldName.RecordVersion, MySqlDbType.Int32).Value = RecordVersion;

        }

        private static string FieldString()
        {
            return
                "  " + FieldName.UID +
                " ," + FieldName.Name +
                " ," + FieldName.Description +

                " ," + FieldName.UpdateDateTime +
                " ," + FieldName.UserIdUpdatedBy +
                " ," + FieldName.CreationDateTime +
                " ," + FieldName.UserIdCreatedBy +
                " ," + FieldName.RecordVersion +
                " ," + FieldName.IsVoid;
        }

        /// <summary>
        /// Load Object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="Room"></param>
        private static void LoadObject(MySqlDataReader reader, Room room)
        {
            room.UID = Convert.ToInt32(reader[FieldName.UID]);
            room.Name = reader[FieldName.Name] as string;
            room.Description = reader[FieldName.Description] as string;
            // ----------------------------------------------------------------------------------------
            room.UpdateDateTime = Convert.ToDateTime((reader[FieldName.UpdateDateTime]));
            room.UserIdUpdatedBy = reader[FieldName.UserIdUpdatedBy] as string;
            room.CreationDateTime = Convert.ToDateTime((reader[FieldName.CreationDateTime]));
            room.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy] as string;
            room.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
            room.IsVoid = reader[FieldName.IsVoid] as string;

        }
    }
}