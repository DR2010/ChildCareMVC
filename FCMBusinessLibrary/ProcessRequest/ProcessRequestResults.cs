using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using fcm.Interfaces;

namespace FCMBusinessLibrary
{
    public class ProcessRequestResults
    {
        public int FKRequestUID { get; set; }
        public int FKClientUID { get; set; }
        public int SequenceNumber { get; set; }
        public string Type { get; set; }
        public string LongText { get; set; }

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string FKRequestUID = "FKRequestUID";
            public const string FKClientUID = "FKClientUID"; 
            public const string SequenceNumber = "SequenceNumber";
            public const string Type = "Type";
            public const string LongText = "LongText";

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([SEQUENCENUMBER]) LASTUID FROM [ProcessRequestResults] WHERE FKRequestUID = " + this.FKRequestUID.ToString();

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                        "INSERT INTO [ProcessRequestResults] " +
                        "( " +
                        FieldString() +
                        ")" +
                            " VALUES " +
                        "( " +
                    "  @" + FieldName.FKRequestUID +
                    ", @" + FieldName.FKClientUID +
                    ", @" + FieldName.SequenceNumber +
                    ", @" + FieldName.Type +
                    ", @" + FieldName.LongText +
                    " )"

                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRequestUID", SqlDbType.BigInt).Value = FKRequestUID;
                    command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = FKClientUID;
                    command.Parameters.Add("@SequenceNumber", SqlDbType.BigInt).Value = SequenceNumber;
                    command.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                    command.Parameters.Add("@LongText", SqlDbType.VarChar).Value = LongText;

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                FieldString() +
                "   FROM     [ProcessRequestResults] " +
                "  WHERE  " +
                "    [FKRequestUID] = " + FKRequestUID.ToString() +
                "  ORDER BY 1 ASC "
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
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
                               SqlDataReader reader)
        {

            processRequest.FKRequestUID = Convert.ToInt32(reader[FieldName.FKRequestUID].ToString());
            processRequest.FKClientUID = Convert.ToInt32(reader[FieldName.FKClientUID].ToString());
            processRequest.SequenceNumber = Convert.ToInt32(reader[FieldName.SequenceNumber].ToString());
            processRequest.Type = reader[FieldName.Type].ToString();
            processRequest.LongText = reader[FieldName.LongText].ToString();
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
                + "," + FieldName.LongText
            );
        }

    }
}
