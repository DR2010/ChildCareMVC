using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace fcmMVCfirst.Models
{
    public class Child
    {
        #region properties
        
        [Display(Name = "UID")]
        public int UID { get; set; }                     // bigint(20) NOT NULL,

        [Display(Name = "Current Room")]
        public int CurrentRoom { get; set; }

        [Display(Name = "Room Name")]
        public string RoomName { get; set; } 

        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name must be supplied.")]
        [Display(Name = "First Name")]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; }            // varchar(100) DEFAULT NULL,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Surname must be supplied.")]
        [Display(Name = "Surname")]
        [StringLength(100, MinimumLength = 1)]
        public string Surname { get; set; }              // varchar(100) DEFAULT NULL,

        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }          // date DEFAULT NULL,

        [Display(Name = "Address")]
        [StringLength(100)]
        public string AddressStreetName { get; set; }    // varchar(100) DEFAULT NULL,

        [Display(Name = "Street Number")]
        [StringLength(45)]
        public string AddressStreetNumber { get; set; }  // varchar(45) DEFAULT NULL,

        [Display(Name = "Suburb")]
        [StringLength(100)]
        public string AddressSuburb { get; set; }        // varchar(100) DEFAULT NULL,

        [Display(Name = "City")]
        [StringLength(100)]
        public string AddressCity { get; set; }          // varchar(100) DEFAULT NULL,

        [Display(Name = "Post Code")]
        [StringLength(10)]
        public string AddressPostCode { get; set; }      // varchar(10) DEFAULT NULL,

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

        public List<Child> childList;

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
                var commandString = "SELECT MAX(UID) LASTUID FROM child";

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
                FieldString() +
                " , room.Name " +
                "   FROM child, room " +
                "  WHERE child.IsVoid = 'N' " +
                "  AND   child.CurrentRoom = room.UID " +
                "  AND child.UID = @UID" ;

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@UID", this.UID);

                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            try
                            {
                                LoadObject(reader, this);
                                this.RoomName = reader.GetString(FieldName.RoomName);

                            }
                            catch (Exception exx)
                            {
                                LogFile.WriteToTodaysLogFile(exx.ToString(), _headerInfo.UserID);
                                UID = 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);
                        
                    }
                }
            }

        }
        
        /// <summary>
        /// Add child
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
                        "INSERT INTO child (" +
                        FieldString() +
                        ")" +
                        " VALUES " +
                        "( " +
                        "  @" + FieldName.UID +
                        " ,@" + FieldName.CurrentRoom +
                        " ,@" + FieldName.FirstName +
                        " ,@" + FieldName.Surname +
                        " ,@" + FieldName.DateOfBirth        +
                        " ,@" + FieldName.AddressStreetName  +
                        " ,@" + FieldName.AddressStreetNumber+
                        " ,@" + FieldName.AddressSuburb      +
                        " ,@" + FieldName.AddressCity        +
                        " ,@" + FieldName.AddressPostCode    +
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

                        return new ResponseStatus() { ReturnCode = -0025, ReasonCode = 0001, Message = "Error adding child. " + ex };

                    }

                    return new ResponseStatus();
                }
            }
        }

        /// <summary>
        /// Update child details
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Update(string userID)
        {

            // Check record version. Do not allow update if version is different

            if (!IsTheSameRecordVersion(this.UID, this.RecordVersion))
            {

                return new ResponseStatus()
                           {
                               ReturnCode = -0010, 
                               ReasonCode = 0001, 
                               Message = "Record updated previously by another user."
                           };

            }

            string commandString = (
                                             "UPDATE Child " +
                                             " SET  " +
                                             FieldName.UID + "= @" + FieldName.UID + ", " +
                                             FieldName.CurrentRoom + "= @" + FieldName.CurrentRoom + ", " +
                                             FieldName.FirstName + "= @" + FieldName.FirstName + ", " +
                                             FieldName.Surname + "= @" + FieldName.Surname + ", " +
                                             FieldName.DateOfBirth + "= @" + FieldName.DateOfBirth + ", " +
                                             FieldName.AddressStreetName + "= @" + FieldName.AddressStreetName + ", " +
                                             FieldName.AddressStreetNumber + "= @" + FieldName.AddressStreetNumber + ", " +
                                             FieldName.AddressSuburb + "= @" + FieldName.AddressSuburb + ", " +
                                             FieldName.AddressCity + "= @" + FieldName.AddressCity + ", " +
                                             FieldName.AddressPostCode + "= @" + FieldName.AddressPostCode + ", " +
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
                        //command.Parameters.Add("@" + FieldName.UpdateDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
                        //command.Parameters.Add("@" + FieldName.UserIdUpdatedBy, MySqlDbType.VarChar).Value = userID;

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
        /// Logical Delete client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public ResponseStatus Delete()
        {

            using (var connection = new MySqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE child " +
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

                        return new ResponseStatus() { ReturnCode = -0022, ReasonCode = 0001, Message = "Error deleting child. " + ex };

                    }
                }
            }
            return new ResponseStatus();
        }

        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roomUID"> </param>
        public void List(string userID, int roomUID = 0)
        {
            childList = new List<Child>();

            string roomCriteria = "";
            if (roomUID > 0)
            {
                roomCriteria = " and child.CurrentRoom = " + roomUID;

                // Get Room Name
                var room = new Room();
                room.UID = roomUID;
                room.Read();

                this.RoomName = room.Name;
            }

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                " , room.Name " +
                "   FROM child, room " +
                "  WHERE child.IsVoid = 'N' " +
                "  AND   child.CurrentRoom = room.UID " +
                roomCriteria + 
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
                                var client = new Child();
                                LoadObject(reader, client);
                                client.RoomName = reader.GetString(FieldName.RoomName);

                                childList.Add(client);

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
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string CurrentRoom = "CurrentRoom";
            public const string FirstName = "FirstName";
            public const string Surname = "Surname";
            public const string DateOfBirth = "DateOfBirth";
            public const string AddressStreetName = "AddressStreetName";
            public const string AddressStreetNumber = "AddressStreetNumber";
            public const string AddressSuburb = "AddressSuburb";
            public const string AddressCity = "AddressCity";
            public const string AddressPostCode = "AddressPostCode";
            public const string UpdateDateTime = "UpdateDateTime";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string RecordVersion = "RecordVersion";
            public const string IsVoid = "IsVoid";
            public const string RoomName = "Name";
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
                var commandString = "SELECT recordversion FROM child WHERE UID = " + inputUID.ToString(CultureInfo.InvariantCulture);

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
            command.Parameters.AddWithValue("@" + FieldName.UID, UID);
            command.Parameters.AddWithValue("@" + FieldName.CurrentRoom, CurrentRoom);
            command.Parameters.AddWithValue("@" + FieldName.FirstName, FirstName);
            command.Parameters.AddWithValue("@" + FieldName.Surname, Surname);
            command.Parameters.AddWithValue("@" + FieldName.DateOfBirth, DateOfBirth);
            command.Parameters.AddWithValue("@" + FieldName.AddressStreetName, AddressStreetName);
            command.Parameters.AddWithValue("@" + FieldName.AddressStreetNumber, AddressStreetNumber);
            command.Parameters.AddWithValue("@" + FieldName.AddressSuburb, AddressSuburb);
            command.Parameters.AddWithValue("@" + FieldName.AddressCity, AddressCity);
            command.Parameters.AddWithValue("@" + FieldName.AddressPostCode, AddressPostCode);
            command.Parameters.AddWithValue("@" + FieldName.UpdateDateTime, DateTime.Now);
            command.Parameters.AddWithValue("@" + FieldName.UserIdUpdatedBy, UserIdUpdatedBy);

            command.Parameters.AddWithValue("@" + FieldName.RecordVersion, RecordVersion);

        }

        private string FieldString()
        {
            return
                "  " + "child." + FieldName.UID +
                " ," + "child." + FieldName.CurrentRoom +
                " ," + "child." + FieldName.FirstName +
                " ," + "child." + FieldName.Surname +
                " ," + "child." + FieldName.DateOfBirth +
                " ," + "child." + FieldName.AddressStreetName +
                " ," + "child." + FieldName.AddressStreetNumber +
                " ," + "child." + FieldName.AddressSuburb +
                " ," + "child." + FieldName.AddressCity +
                " ," + "child." + FieldName.AddressPostCode +
                " ," + "child." + FieldName.UpdateDateTime +
                " ," + "child." + FieldName.UserIdUpdatedBy +
                " ," + "child." + FieldName.CreationDateTime +
                " ," + "child." + FieldName.UserIdCreatedBy +
                " ," + "child." + FieldName.RecordVersion +
                " ," + "child." + FieldName.IsVoid; 
        }

        /// <summary>
        /// Load Object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="child"></param>
        private static void LoadObject(MySqlDataReader reader, Child child)
        {
            child.UID = reader.GetInt32(FieldName.UID);
            child.CurrentRoom = reader.GetInt32(FieldName.CurrentRoom);

            child.FirstName = reader.GetString(FieldName.FirstName);
            child.Surname = reader.GetString(FieldName.Surname);
            child.DateOfBirth = reader.GetDateTime( FieldName.DateOfBirth );
            child.AddressStreetName = reader[FieldName.AddressStreetName] as string;
            child.AddressStreetNumber = reader[FieldName.AddressStreetNumber] as string;
            child.AddressSuburb = reader[FieldName.AddressSuburb] as string;
            child.AddressCity = reader[FieldName.AddressCity] as string;
            child.AddressPostCode = reader[FieldName.AddressPostCode] as string;
            // -----------------------------------------------------------------------------------------
            child.UpdateDateTime = Convert.ToDateTime((reader[FieldName.UpdateDateTime]));
            child.UserIdUpdatedBy = reader[FieldName.UserIdUpdatedBy] as string;
            child.CreationDateTime = Convert.ToDateTime((reader[FieldName.CreationDateTime]));
            child.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy] as string;
            child.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
            child.IsVoid = reader[FieldName.IsVoid] as string;

        }
    }
}