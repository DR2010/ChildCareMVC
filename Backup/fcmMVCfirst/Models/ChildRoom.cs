using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace fcmMVCfirst.Models
{
    public class ChildRoom
    {
        #region properties

        [Display(Name = "UID")]
        public int UID { get; set; }                     // bigint(20) NOT NULL,

        [Display(Name = "Child ID")]
        public int FKChildUID { get; set; }

        [Display(Name = "Room ID")]
        public int FKRoomUID { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

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

        public Child child;
        public Room room;

        public List<ChildRoom> childRoomList;

        private HeaderInfo _headerInfo { get; set; }

        #endregion properties


        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FKChildUID = "FKChildUID";
            public const string FKRoomUID = "FKRoomCode";
            public const string StartDate = "StartDate";
            public const string EndDate = "EndDate";

        }


        public class ChildRoomAddRequest
        {
            public HeaderInfo headerInfo;
            public ChildRoom childRoom;
        }

        public class ChildRoomAddResponse
        {
            public ResponseStatus responseStatus;
            public int childRoomUID;
        }


        /// <summary>
        /// Add Learning Story
        /// </summary>
        public ChildRoomAddResponse Add(ChildRoomAddRequest childRoomAddRequest)
        {
            this.UserIdCreatedBy = childRoomAddRequest.headerInfo.UserID;
            this.UserIdUpdatedBy = childRoomAddRequest.headerInfo.UserID;

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                UID = CommonDB.GetLastUID("ChildRoom") + 1;
                RecordVersion = 1;

                var commandString =
                    (
                        "INSERT INTO LearningStory (" +
                        FieldString() +
                        ")" +
                        " VALUES " +
                        "( " +
                        "  @" + FieldName.UID +
                        " ,@" + FieldName.FKChildUID +
                        " ,@" + FieldName.FKRoomUID +
                        " ,@" + FieldName.StartDate +
                        " ,@" + FieldName.EndDate +

                        ", @" + CommonDB.FieldName.UpdateDateTime +
                        ", @" + CommonDB.FieldName.UserIdUpdatedBy +
                        ", @" + CommonDB.FieldName.CreationDateTime +
                        ", @" + CommonDB.FieldName.UserIdCreatedBy +
                        ", @" + CommonDB.FieldName.RecordVersion +
                        ", @" + CommonDB.FieldName.IsVoid +
                        " )"
                    );

                using (var command = new MySqlCommand(
                    commandString, connection))
                {
                    AddSqlParameters(command);

                    command.Parameters.Add("@" + LearningStory.FieldName.CreationDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@" + LearningStory.FieldName.UserIdCreatedBy, MySqlDbType.VarChar).Value = this.UserIdCreatedBy;
                    command.Parameters.Add("@" + LearningStory.FieldName.IsVoid, MySqlDbType.VarChar).Value = "N";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        return new ChildRoomAddResponse
                        {
                            responseStatus =
                            {
                                ReturnCode = -0025,
                                ReasonCode = 0001,
                                Message = "Error adding Learning Story. " + ex
                            }
                        };
                    }

                    return new ChildRoomAddResponse
                    {
                        responseStatus = { ReturnCode = 0001, ReasonCode = 0001 }
                    };
                }
            }
        }

        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        public void List(string userID)
        {
            childRoomList = new List<ChildRoom>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM ChildRoom " +
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
                                var childRoom = new ChildRoom();
                                LoadObject(reader, childRoom);

                                childRoomList.Add(childRoom);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();
                        LogFile.WriteToTodaysLogFile(ex.ToString(), userID, "", "ChildRoom.cs");

                    }
                }
            }
        }


        private void AddSqlParameters(MySqlCommand command)
        {
            command.Parameters.AddWithValue("@" + FieldName.UID, UID);
            command.Parameters.AddWithValue("@" + FieldName.FKChildUID, FKChildUID);
            command.Parameters.AddWithValue("@" + FieldName.FKRoomUID, FKRoomUID);
            command.Parameters.AddWithValue("@" + FieldName.StartDate, StartDate);
            command.Parameters.AddWithValue("@" + FieldName.EndDate, EndDate);
            // -----------------------------------------------------------------------------------------
            command.Parameters.AddWithValue("@" + CommonDB.FieldName.UpdateDateTime, DateTime.Now);
            command.Parameters.AddWithValue("@" + CommonDB.FieldName.UserIdUpdatedBy, UserIdUpdatedBy);
            command.Parameters.AddWithValue("@" + CommonDB.FieldName.IsVoid, IsVoid);

            command.Parameters.AddWithValue("@" + CommonDB.FieldName.RecordVersion, RecordVersion);

        }

        private string FieldString()
        {
            return
                "  " + FieldName.UID +
                " ," + FieldName.FKChildUID +
                " ," + FieldName.FKRoomUID +
                " ," + FieldName.StartDate +
                " ," + FieldName.EndDate +
                " ," + CommonDB.FieldName.UpdateDateTime +
                " ," + CommonDB.FieldName.UserIdUpdatedBy +
                " ," + CommonDB.FieldName.CreationDateTime +
                " ," + CommonDB.FieldName.UserIdCreatedBy +
                " ," + CommonDB.FieldName.RecordVersion +
                " ," + CommonDB.FieldName.IsVoid;
        }

        /// <summary>
        /// Load Object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="child"></param>
        private static void LoadObject(MySqlDataReader reader, ChildRoom childRoom)
        {
            childRoom.UID = reader.GetInt32(FieldName.UID);
            childRoom.FKChildUID = reader.GetInt32(FieldName.FKChildUID);
            childRoom.FKRoomUID = reader.GetInt32(FieldName.FKRoomUID);
            childRoom.StartDate = reader.GetDateTime(FieldName.StartDate);
            childRoom.EndDate = reader.GetDateTime(FieldName.EndDate);
            // -----------------------------------------------------------------------------------------
            childRoom.UpdateDateTime = Convert.ToDateTime((reader[CommonDB.FieldName.UpdateDateTime]));
            childRoom.UserIdUpdatedBy = reader[CommonDB.FieldName.UserIdUpdatedBy] as string;
            childRoom.CreationDateTime = Convert.ToDateTime((reader[CommonDB.FieldName.CreationDateTime]));
            childRoom.UserIdCreatedBy = reader[CommonDB.FieldName.UserIdCreatedBy] as string;
            childRoom.RecordVersion = Convert.ToInt32(reader[CommonDB.FieldName.RecordVersion]);
            childRoom.IsVoid = reader[CommonDB.FieldName.IsVoid] as string;

        }
    }
}