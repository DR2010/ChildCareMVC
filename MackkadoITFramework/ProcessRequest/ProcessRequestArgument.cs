using System;
using System.Collections.Generic;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.ProcessRequest
{
    public class ProcessRequestArguments
    {
        #region Fields 
        public int FKRequestUID { get; set; }
        public string Code { get; set; }
        public string ValueType { get; set; } // INT, STRING, DATE
        public string Value { get; set; }
        #endregion Fields

        #region Fields Building Blocks

        #region Permitted Values

        public enum ProcessRequestCodeValues
        {
            CLIENTUID, OVERRIDE, CLIENTSETID, CLIENTDOCUID
        }
        public enum ValueTypeValue
        {
            NUMBER, STRING
        }

        #endregion Permitted Values

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string FKRequestUID = "FKRequestUID";
            public const string Code = "Code";
            public const string ValueType = "ValueType";
            public const string Value = "Value";
        }

        /// <summary>
        /// Process Request Arguments string of fields.
        /// </summary>
        /// <returns></returns>
        private static string FieldString()
        {
            return (
                        FieldName.FKRequestUID
                + "," + FieldName.Code
                + "," + FieldName.ValueType
                + "," + FieldName.Value
            );
        }


        /// <summary>
        /// Load from Reader
        /// </summary>
        /// <param name="processRequest"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="reader"></param>
        public static void LoadFromReader(
                               ProcessRequestArguments processRequest,
                               MySqlDataReader reader)
        {

            processRequest.FKRequestUID = Convert.ToInt32(reader[FieldName.FKRequestUID].ToString());
            processRequest.Code = reader[FieldName.Code].ToString();
            processRequest.ValueType = reader[FieldName.ValueType].ToString();
            processRequest.Value = reader[FieldName.Value].ToString();

            return;
        }

        #endregion Fields Building Blocks

        /// <summary>
        /// Check if request code exists for request.
        /// </summary>
        /// <param name="requestUID"></param>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        public static bool Exists(int requestUID, string requestCode)
        {
            int xUID = 0;
            bool exist = false;

            // 
            // EA SQL database
            // 

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString = "SELECT FKRequestUID UID FROM ProcessRequestArguments WHERE FKRequestUID = " + requestUID.ToString() +
                                    " AND Code = '" + requestCode + "'";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            xUID = Convert.ToInt32(reader["UID"]);
                            exist = true;
                        }
                        catch (Exception)
                        {
                            xUID = 0;
                        }
                    }
                }
            }

            return exist;
        }

        /// <summary>
        /// Add New Process Request Argument
        /// </summary>
        public ResponseStatus Add()
        {
            ResponseStatus responseSuccessful = new ResponseStatus();
            ResponseStatus responseError = new ResponseStatus(messageType: MessageType.Error);

            // Check if request has already been added
            //
            if (ProcessRequestArguments.Exists(this.FKRequestUID, this.Code))
            {
                responseError.Message = "Request Argument already exists " + this.Code;
                responseError.Contents = this;
                return responseError;
            }

            DateTime _now = DateTime.Today;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                        "INSERT INTO ProcessRequestArguments " +
                        "( " +
                        FieldString() +
                        ")" +
                            " VALUES " +
                        "( " +
                    "  @" + FieldName.FKRequestUID +
                    ", @" + FieldName.Code +
                    ", @" + FieldName.ValueType +
                    ", @" + FieldName.Value +

                    " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRequestUID", MySqlDbType.Int32).Value = FKRequestUID;
                    command.Parameters.Add("@Code", MySqlDbType.VarChar).Value = Code;
                    command.Parameters.Add("@ValueType", MySqlDbType.VarChar).Value = ValueType;
                    command.Parameters.Add("@Value", MySqlDbType.VarChar).Value = Value;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return responseSuccessful;
        }

        /// <summary>
        /// List requests
        /// </summary>
        /// <param name="StatusIn"></param>
        /// <returns></returns>
        public static List<ProcessRequestArguments> List(int requestID)
        {
            var result = new List<ProcessRequestArguments>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM     ProcessRequestArguments " +
                "  WHERE  " +
                "    FKRequestUID = '" + requestID.ToString() + "'" +
                "  "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var _ProcessRequestArguments = new ProcessRequestArguments();
                            ProcessRequestArguments.LoadFromReader(_ProcessRequestArguments, reader);

                            result.Add(_ProcessRequestArguments);
                        }
                    }
                }
            }

            return result;
        }


    }
}
