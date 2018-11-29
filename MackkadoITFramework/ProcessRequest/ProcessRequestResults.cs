using System;
using System.Collections.Generic;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.ProcessRequest
{
    public class ProcessRequestResults
    {
        public int FKRequestUID { get; set; }
        public int FKClientUID { get; set; }
        public int SequenceNumber { get; set; }
        public string Type { get; set; }
        public string Results { get; set; }

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string FKRequestUID = "FKRequestUID";
            public const string FKClientUID = "FKClientUID"; 
            public const string SequenceNumber = "SequenceNumber";
            public const string Type = "Type";
            public const string Results = "Results";

        }

        #region Permitted Values

        public enum TypeValue
        {
            INFORMATIONAL, ERROR
        }

        #endregion Permitted Values


        // -----------------------------------------------------
        //          Retrieve last ProcessRequest UID
        // -----------------------------------------------------
        public int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString = "SELECT MAX(SEQUENCENUMBER) LASTUID FROM ProcessRequestResults WHERE FKRequestUID = " + this.FKRequestUID.ToString();

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

        // -----------------------------------------------------
        //    Add new Process Request Results row
        // -----------------------------------------------------
        public ResponseStatus Add()
        {

            ResponseStatus responseSuccessful = new ResponseStatus();
            ResponseStatus responseError = new ResponseStatus(messageType: MessageType.Error);

            string ret = "Item updated successfully";
            int _uid = 0;

            _uid = GetLastUID() + 1;
            this.SequenceNumber = _uid;

            DateTime _now = DateTime.Today;

            if (SequenceNumber == 0)
            {
                responseError.Message = "Sequence Number Not Supplied.";
                responseError.Contents = this;
                return responseError;
            }

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                        "INSERT INTO ProcessRequestResults " +
                        "( " +
                        FieldString() +
                        ")" +
                            " VALUES " +
                        "( " +
                    "  @" + FieldName.FKRequestUID +
                    ", @" + FieldName.FKClientUID +
                    ", @" + FieldName.SequenceNumber +
                    ", @" + FieldName.Type +
                    ", @" + FieldName.Results +
                    " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRequestUID", MySqlDbType.Int32).Value = FKRequestUID;
                    command.Parameters.Add("@FKClientUID", MySqlDbType.Int32).Value = FKClientUID;
                    command.Parameters.Add("@SequenceNumber", MySqlDbType.Int32).Value = SequenceNumber;
                    command.Parameters.Add("@Type", MySqlDbType.VarChar).Value = Type;
                    command.Parameters.Add("@Results", MySqlDbType.VarChar).Value = Results;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return responseSuccessful; 
        }

        /// <summary>
        /// List Request Results
        /// </summary>
        /// <param name="FKRequestUID"></param>
        /// <returns></returns>
        public static List<ProcessRequestResults> List(int FKRequestUID)
        {
            var result = new List<ProcessRequestResults>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM     ProcessRequestResults " +
                "  WHERE  " +
                "    FKRequestUID = " + FKRequestUID.ToString() +
                "  ORDER BY 1 ASC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var _ProcessRequest = new ProcessRequestResults();
                            ProcessRequestResults.LoadFromReader(_ProcessRequest, reader);
                            result.Add(_ProcessRequest);
                        }
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// Load from Reader
        /// </summary>
        /// <param name="processRequest"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="reader"></param>
        public static void LoadFromReader(
                               ProcessRequestResults processRequest,
                               MySqlDataReader reader)
        {

            processRequest.FKRequestUID = Convert.ToInt32(reader[FieldName.FKRequestUID].ToString());
            processRequest.FKClientUID = Convert.ToInt32(reader[FieldName.FKClientUID].ToString());
            processRequest.SequenceNumber = Convert.ToInt32(reader[FieldName.SequenceNumber].ToString());
            processRequest.Type = reader[FieldName.Type].ToString();
            processRequest.Results = reader[FieldName.Results].ToString();
            return;
        }


        /// <summary>
        /// Document string of fields.
        /// </summary>
        /// <returns></returns>
        private static string FieldString()
        {
            return (
                        FieldName.FKRequestUID
                + "," + FieldName.FKClientUID
                + "," + FieldName.SequenceNumber
                + "," + FieldName.Type
                + "," + FieldName.Results
            );
        }

    }
}
