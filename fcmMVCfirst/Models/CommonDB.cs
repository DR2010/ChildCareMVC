using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace fcmMVCfirst.Models
{
    public static class CommonDB
    {
        /// <summary>
        /// Check if the record version is the same. If it is not, deny update
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="inputUID"></param>
        /// <param name="recordVersion"></param>
        /// <returns></returns>
        public static bool IsTheSameRecordVersion(string tablename, int inputUID, int recordVersion, ResponseStatus responseStatus)
        {
            // 
            // EA SQL database
            // 
            int currentVersion = 0;

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                var commandString = "SELECT recordversion FROM " + tablename + " WHERE UID = " +
                                    inputUID.ToString(CultureInfo.InvariantCulture);

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
            {
                responseStatus.ReturnCode = -0010;
                responseStatus.ReasonCode = 0001;
                responseStatus.Message = "Record updated previously by another user.";
                ret = false;
            }
            else
            {
                responseStatus.ReturnCode = 0001;
                responseStatus.ReasonCode = 0001;
                responseStatus.Message = "Record Versions are matching. All good.";
                ret = true;
            }

            return ret;
        }


        /// <summary>
        /// Retrieve last UID
        /// </summary>
        /// <returns></returns>
        public static int GetLastUID(string tablename)
        {
            int lastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new MySqlConnection(ConnectionString.GetConnectionString()))
            {
                var commandString = "SELECT MAX(UID) LASTUID FROM " + tablename;

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
                                    ex.ToString(), HeaderInfo.Instance.UserID, "", "LearningStory.cs");

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
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UpdateDateTime = "UpdateDateTime";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string RecordVersion = "RecordVersion";
            public const string IsVoid = "IsVoid";
        }

    }
}