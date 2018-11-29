using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.Client
{
    public class ClientOtherInfo
    {
        public int UID { get; set; }
        public int FKClientUID { get; set; }
        public string RCFieldCode { get; set; }
        public string FieldValueText { get; set; }

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FKClientUID = "FKClientUID";
            public const string RCFieldCode = "RCFieldCode";
            public const string FieldValueText = "FieldValueText";
        }

        /// <summary>
        /// Get Last UID
        /// </summary>
        /// <returns></returns>
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([UID]) LASTUID FROM [ClientOtherInfo]";

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

        /// <summary>
        /// Add Client Other Information Field
        /// </summary>
        public void Add()
        {

            int nextUID = GetLastUID() + 1;

            DateTime _now = DateTime.Today;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [ClientOtherInfo] " +
                   "([UID], [FKClientUID], [RCFieldCode], [FieldValueText]" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @UID      " +
                   ", @FKClientUID   " +
                   ", @RCFieldCode " +
                   ", @FieldValueText " +
                   " )"

                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = nextUID;
                    command.Parameters.Add("@FKClientUID", SqlDbType.VarChar).Value = FKClientUID;
                    command.Parameters.Add("@RCFieldCode", SqlDbType.VarChar).Value = RCFieldCode;
                    command.Parameters.Add("@FieldValueText", SqlDbType.VarChar).Value = FieldValueText;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        /// <summary>
        /// Update Client Other Information Field
        /// </summary>
        public void Update()
        {

            DateTime _now = DateTime.Today;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientOtherInfo] " +
                   "   SET [FKClientUID] = @FKClientUID " +
                   "  ,SET [RCFieldCode] = @RCFieldCode " +
                   "  ,SET [FieldValueText] = @FieldValueText " +
                   "  WHERE UID = @UID " 
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;
                    command.Parameters.Add("@FKClientUID", SqlDbType.VarChar).Value = FKClientUID;
                    command.Parameters.Add("@RCFieldCode", SqlDbType.VarChar).Value = RCFieldCode;
                    command.Parameters.Add("@FieldValueText", SqlDbType.VarChar).Value = FieldValueText;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        /// <summary>
        /// List employees
        /// </summary>
        /// <param name="clientID"></param>
        public static List<ClientOtherInfo> List(int clientID)
        {
            var clientOtherList = new List<ClientOtherInfo>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT           " +
                " UID,             " +
                " FKClientUID,     " +
                " RCFieldCode,     " +
                " FieldValueText   " +
                "   FROM [ClientOtherInfo] " +
                "   WHERE  FKClientUID = {0}",
                clientID);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var clientOther = new ClientOtherInfo();
                            LoadObject(reader, clientOther);

                            clientOtherList.Add(clientOther);
                        }
                    }
                }
            }

            return clientOtherList;
        }

        /// <summary>
        /// This method loads the information from the sqlreader into the Employee object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="employee"></param>
        private static void LoadObject(SqlDataReader reader, ClientOtherInfo clientOther)
        {
            clientOther.UID = Convert.ToInt32(reader[FieldName.UID].ToString());
            clientOther.FKClientUID = Convert.ToInt32(reader[FieldName.FKClientUID]);
            clientOther.RCFieldCode = reader[FieldName.RCFieldCode].ToString();
            clientOther.FieldValueText = reader[FieldName.FieldValueText].ToString();
        }
    }
}
