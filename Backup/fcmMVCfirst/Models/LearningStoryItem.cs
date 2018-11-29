using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.ReferenceData;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace fcmMVCfirst.Models
{
    public class LearningStoryItem
    {
        public int UID { get; set; }
        public int FKLearningStoryUID { get; set; }
        public string FKCodeType { get; set; }
        public string FKCodeValue { get; set; }
        public string Action { get; set; }

        /// <summary>
        /// Add Learning Story
        /// </summary>
        public int AddItem(LearningStory learningStory, LearningStoryItem item, HeaderInfo _headerInfo)
        {

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                int nextItemUID = CommonDB.GetLastUID("learningstoryitem") + 1;

                var commandString =
                    (

                       " INSERT INTO learningstoryitem " +
                            " (UID, " +
                            " FKLearningStoryUID, " +
                            " FKCodeType, " +
                            " FKCodeValue " +
                            " )" +
                            " VALUES " +
                            "( " +
                            "  @UID " +
                            " ,@FKLearningStoryUID " +
                            " ,@FKCodeType " +
                            " ,@FKCodeValue " +
                            " )"
                        );

                using (var command = new MySqlCommand(
                    commandString, connection))
                {
                    command.Parameters.AddWithValue("@UID", nextItemUID);
                    command.Parameters.AddWithValue("@FKLearningStoryUID", learningStory.UID);
                    command.Parameters.AddWithValue("@FKCodeType", item.FKCodeType);
                    command.Parameters.AddWithValue("@FKCodeValue", item.FKCodeValue);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        var respError = new LearningStory.LearningStoryAddResponse();
                        respError.responseStatus = new ResponseStatus();
                        respError.responseStatus.ReturnCode = -0025;
                        respError.responseStatus.ReasonCode = 0002;
                        respError.responseStatus.Message = "Error adding Learning Story Item. " + ex;
                    }


                }
            }

            return UID;
        }

        /// <summary>
        /// Logical Delete client
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public ResponseStatus DeleteItems(int learningStoryUID, HeaderInfo _headerInfo)
        {

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString =
                (
                   "DELETE FROM LearningStoryItem " +
                   "    WHERE FKLearningStoryUID = @FKLearningStoryUID "
                );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.AddWithValue("@FKLearningStoryUID", learningStoryUID);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), _headerInfo.UserID);

                        return new ResponseStatus() { ReturnCode = -0022, ReasonCode = 0001, Message = "Error deleting Learning Story Item. " + ex };

                    }
                }
            }
            return new ResponseStatus();
        }


        /// <summary>
        /// List clients
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="_FKLearningStoryUID"> </param>
        /// <param name="_headerInfo"> </param>
        public static List<LearningStoryItem> ListItem(int _FKLearningStoryUID, string codeType)
        {

            List<LearningStoryItem> ret = new List<LearningStoryItem>();

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {

                var commandString = string.Format(
                " SELECT " +
                "  UID, " +
                " FKLearningStoryUID, " +
                " FKCodeType, " +
                " FKCodeValue " +
                "   FROM LearningStoryItem " +
                "  WHERE FKLearningStoryUID = @FKLearningStoryUID and FKCodeType = @codeType" +
                "  ORDER BY UID ASC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {

                    command.Parameters.AddWithValue("@FKLearningStoryUID", _FKLearningStoryUID);
                    command.Parameters.AddWithValue("@codeType", codeType);

                    connection.Open();

                    try
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new LearningStoryItem();
                                item.UID = Convert.ToInt32(reader["UID"]);
                                item.FKLearningStoryUID = Convert.ToInt32(reader["FKLearningStoryUID"]);
                                item.FKCodeType = reader["FKCodeType"] as string;
                                item.FKCodeValue = reader["FKCodeValue"] as string;

                                ret.Add(item);
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

            return ret;
        }
    }
}