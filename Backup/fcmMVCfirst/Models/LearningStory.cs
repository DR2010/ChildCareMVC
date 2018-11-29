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
    public class LearningStory
    {
        #region properties
        
        [Display(Name = "UID")]
        public int UID { get; set; }                     // bigint(20) NOT NULL,

        [Display(Name = "Child")]
        public int FKChildUID { get; set; }

        [Display(Name = "Room")]
        public string FKRoomCode { get; set; }

        [Display(Name = "Educator")]
        public int FKEducatorUID { get; set; }      

        [Display(Name = "The Learning Story")]
        [StringLength(4096, MinimumLength = 1)]
        public string Story { get; set; }

        [Display(Name = "Story Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }          // date DEFAULT NULL,

        [Display(Name = "Analysis of Learning")]
        public string AnalysisOfLearning { get; set; }

        [Display(Name = "Extension of Learning")]
        public string ExtensionOfLearning { get; set; }

        [Display(Name = "Parents Comments")]
        public string ParentsComments { get; set; }

        public List<LearningStoryItem> Principles { get; set; }
        public List<LearningStoryItem> Practices { get; set; }
        public List<LearningStoryItem> LearningOutcomes { get; set; }

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

        public List<LearningStory> LearningStoryList;

        public Child child;
        public Educator educator;

        private HeaderInfo _headerInfo { get; set; }

        #endregion properties

        public LearningStory()
        {
            LearningOutcomes = new List<LearningStoryItem>();
            Principles = new List<LearningStoryItem>();
            Practices = new List<LearningStoryItem>();
        }


        // ----------------------------------------------
        // Public methods
        // ----------------------------------------------


        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FKChildUID = "FKChildUID";
            public const string FKRoomCode = "FKRoomCode";
            public const string FKEducatorUID = "FKEducatorUID";
            public const string Date = "Date";
            public const string Story = "Story";
            public const string AnalysisOfLearning = "AnalysisOfLearning";
            public const string ExtensionOfLearning = "ExtensionOfLearning";
            public const string ParentsComments = "ParentsComments";

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
        /// Get Employee details
        /// </summary>
        public void Read(bool includeOutcome)
        {
            // 
            // EA SQL database
            // 
            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                var commandString =
                " SELECT " +
                FieldString()
                + "  FROM learningStory" +
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
                            LoadObject(reader, this, includeOutcome);
                           

                        }
                        catch (Exception ex)
                        {
                            UID = 0;
                        }
                    }
                }
            }

        }



        /// <summary>
        /// Add Learning Story
        /// </summary>
        public LearningStoryAddResponse Add(LearningStoryAddRequest learningStoryAddRequest)
        {

            var response = new LearningStoryAddResponse();
            response.responseStatus = new ResponseStatus();

            this.UserIdCreatedBy = learningStoryAddRequest.headerInfo.UserID;
            this.UserIdUpdatedBy = learningStoryAddRequest.headerInfo.UserID;

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                UID = CommonDB.GetLastUID("LearningStory") + 1;
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
                        " ,@" + FieldName.FKEducatorUID +
                        " ,@" + FieldName.FKRoomCode +
                        " ,@" + FieldName.Date +
                        " ,@" + FieldName.Story +
                        " ,@" + FieldName.AnalysisOfLearning +
                        " ,@" + FieldName.ExtensionOfLearning +
                        " ,@" + FieldName.ParentsComments +

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
                    IsVoid = "N";
                    AddSqlParameters(command);

                    command.Parameters.Add("@" + FieldName.CreationDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@" + FieldName.UserIdCreatedBy, MySqlDbType.VarChar).Value = this.UserIdCreatedBy;
                    //command.Parameters.Add("@" + FieldName.IsVoid, MySqlDbType.VarChar).Value = "N";

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        var respError = new LearningStoryAddResponse();
                        respError.responseStatus = new ResponseStatus();
                        respError.responseStatus.ReturnCode = -0025;
                        respError.responseStatus.ReasonCode = 0001;
                        respError.responseStatus.Message = "Error adding Learning Story. " + ex;
                    }


                }
            }

            // Add items


            if (LearningOutcomes != null)
            {
                foreach (var learningOutcome in LearningOutcomes)
                {
                    if (learningOutcome.Action == "TBD")
                        continue;
                    var lit = new LearningStoryItem().AddItem( this, learningOutcome, _headerInfo );
                }
            }

            if (Practices != null)
            {

                foreach (var practice in Practices)
                {
                    if (practice.Action == "TBD")
                        continue;
                    var lit = new LearningStoryItem().AddItem(this, practice, _headerInfo);
                }
            }
            if (Principles != null)
            {
                foreach (var principle in Principles)
                {
                    if (principle.Action == "TBD")
                        continue;
                    var lit = new LearningStoryItem().AddItem(this, principle, _headerInfo);
                }
            }
            return response;
        }

        public class LearningStoryAddRequest
        {
            public HeaderInfo headerInfo;
            public LearningStory learningStory;

            //public FCMMySQLBusinessLibrary.Client.Client eventClient;
            //public string linkInitialSet;
        }

        public class LearningStoryAddResponse
        {
            public ResponseStatus responseStatus;
            public int storyUID;
        }

        /// <summary>
        /// Update Learning Story details
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
                                             "UPDATE LearningStory " +
                                             " SET  " +
                                             FieldName.UID + "= @" + FieldName.UID + ", " +
                                             FieldName.FKChildUID + "= @" + FieldName.FKChildUID + ", " +
                                             FieldName.FKEducatorUID + "= @" + FieldName.FKEducatorUID + ", " +
                                             FieldName.FKRoomCode + "= @" + FieldName.FKRoomCode + ", " +
                                             FieldName.Date + "= @" + FieldName.Date + ", " +
                                             FieldName.Story + "= @" + FieldName.Story + ", " +
                                             FieldName.AnalysisOfLearning + "= @" + FieldName.AnalysisOfLearning + ", " +
                                             FieldName.ExtensionOfLearning + "= @" + FieldName.ExtensionOfLearning + ", " +
                                             FieldName.ParentsComments + "= @" + FieldName.ParentsComments + ", " +
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
                    IsVoid = "N";
                    AddSqlParameters(command);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        return new ResponseStatus() { ReturnCode = -0020, ReasonCode = 0001, Message = "Error saving client. " + ex };
                    }
                }
            }

            // Delete Items
            //
            new LearningStoryItem().DeleteItems(this.UID, _headerInfo);

            // Add items
            //

            if (LearningOutcomes != null)
            {
                foreach (var learningOutcome in LearningOutcomes)
                {
                    var lit = new LearningStoryItem().AddItem(this, learningOutcome, _headerInfo);
                }
            }

            if (Practices != null)
            {

                foreach (var practice in Practices)
                {
                    var lit = new LearningStoryItem().AddItem(this, practice, _headerInfo);
                }
            }
            if (Principles != null)
            {
                foreach (var principle in Principles)
                {
                    var lit = new LearningStoryItem().AddItem(this, principle, _headerInfo);
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
                   "UPDATE LearningStory " +
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

                        return new ResponseStatus() { ReturnCode = -0022, ReasonCode = 0001, Message = "Error deleting LearningStory. " + ex };

                    }
                }
            }
            return new ResponseStatus();
        }


        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        public void List(string userID, int childUID)
        {
            LearningStoryList = new List<LearningStory>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM LearningStory " +
                "  WHERE IsVoid = 'N' " +
                " AND FKChildUID = @FKChildUID " +
                "  ORDER BY UID ASC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {

                    command.Parameters.AddWithValue("@FkChildUID", childUID);

                    connection.Open();

                    try
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var client = new LearningStory();
                                LoadObject(reader, client, false);

                                LearningStoryList.Add(client);

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
                var commandString = "SELECT recordversion FROM LearningStory WHERE UID = " + inputUID.ToString(CultureInfo.InvariantCulture);

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
            command.Parameters.AddWithValue("@" + FieldName.FKChildUID, FKChildUID);
            command.Parameters.AddWithValue("@" + FieldName.FKEducatorUID, FKEducatorUID);
            command.Parameters.AddWithValue("@" + FieldName.FKRoomCode, FKRoomCode);
            command.Parameters.AddWithValue("@" + FieldName.Date, Date);
            command.Parameters.AddWithValue("@" + FieldName.Story, Story);
            command.Parameters.AddWithValue("@" + FieldName.AnalysisOfLearning, AnalysisOfLearning);
            command.Parameters.AddWithValue("@" + FieldName.ExtensionOfLearning, ExtensionOfLearning);
            command.Parameters.AddWithValue("@" + FieldName.ParentsComments, ParentsComments);
            // -----------------------------------------------------------------------------------------
            command.Parameters.Add("@" + FieldName.UpdateDateTime, MySqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@" + FieldName.UserIdUpdatedBy, MySqlDbType.VarChar).Value = UserIdUpdatedBy;

            command.Parameters.Add("@" + FieldName.RecordVersion, MySqlDbType.Int32).Value = RecordVersion;
            command.Parameters.AddWithValue("@" + FieldName.IsVoid, IsVoid);


        }

        private string FieldString()
        {
            return
                "  " + FieldName.UID +
                " ," + FieldName.FKChildUID +
                " ," + FieldName.FKEducatorUID +
                " ," + FieldName.FKRoomCode +
                " ," + FieldName.Date +
                " ," + FieldName.Story +
                " ," + FieldName.AnalysisOfLearning +
                " ," + FieldName.ExtensionOfLearning +
                " ," + FieldName.ParentsComments +
                " ," + FieldName.UpdateDateTime +
                " ," + FieldName.UserIdUpdatedBy +
                " ," + FieldName.CreationDateTime +
                " ," + FieldName.UserIdCreatedBy +
                " ," + FieldName.RecordVersion +
                " ," + FieldName.IsVoid;
        }

        /// <summary>
        /// Load object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="learningStory"></param>
        private static void LoadObject(MySqlDataReader reader, LearningStory learningStory, bool includeOutcome)
        {
            learningStory.UID = Convert.ToInt32(reader[FieldName.UID]);
            learningStory.FKChildUID = Convert.ToInt32(reader[FieldName.FKChildUID]);
            learningStory.FKRoomCode = reader[FieldName.FKRoomCode] as string;
            learningStory.FKEducatorUID = Convert.ToInt32(reader[FieldName.FKEducatorUID]);
            learningStory.Date = Convert.ToDateTime((reader[FieldName.Date]));
            learningStory.Story = reader[FieldName.Story] as string;
            learningStory.AnalysisOfLearning = reader[FieldName.AnalysisOfLearning] as string;
            learningStory.ExtensionOfLearning = reader[FieldName.ExtensionOfLearning] as string;
            learningStory.ParentsComments = reader[FieldName.ParentsComments] as string;
            // -----------------------------------------------------------------------------------------
            learningStory.UpdateDateTime = Convert.ToDateTime((reader[FieldName.UpdateDateTime]));
            learningStory.UserIdUpdatedBy = reader[FieldName.UserIdUpdatedBy] as string;
            learningStory.CreationDateTime = Convert.ToDateTime((reader[FieldName.CreationDateTime]));
            learningStory.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy] as string;
            learningStory.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
            learningStory.IsVoid = reader[FieldName.IsVoid] as string;

            learningStory.child = new Child();
            learningStory.child.UID = learningStory.FKChildUID;
            learningStory.child.Read();

            learningStory.educator = new Educator();
            learningStory.educator.UID = learningStory.FKEducatorUID;
            learningStory.educator.Read();

            // Loading outcomes, practices and principles
            //
            if (includeOutcome)
            {
                learningStory.LearningOutcomes = LearningStoryItem.ListItem(learningStory.UID, "LESI");
                learningStory.Principles = LearningStoryItem.ListItem(learningStory.UID, "PRIN");
                learningStory.Practices = LearningStoryItem.ListItem(learningStory.UID, "PRAC");
            }


        }
    }
}
