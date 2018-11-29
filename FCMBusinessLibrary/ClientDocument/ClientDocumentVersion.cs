using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace FCMBusinessLibrary
{
    public class ClientDocumentVersion
    {
        public int UID;
        public int FKClientDocumentUID;
        public int FKClientUID;         //
        public int ClientIssueNumber;   // 03 - This is the version number of the client
        public int SourceIssueNumber;   // 01
        public string IssueNumberText;  // 03 - This is the version number of the client in text
        public string Location;         // %LOCATION%\TEST\TEST
        public string FileName;         // POL-05-01-0001-01 FILENAME.DOC
        public string ComboIssueNumber; // POL-05-01-0002-03
        public string DocumentCUID;     // POL-05-01

        // -----------------------------------------------------
        //    Get Client details
        // -----------------------------------------------------
        public bool Read()
        {
            // 
            // EA SQL database
            // 
            bool ret = false;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT [UID] " +
                "       ,[FKClientDocumentUID] " +
                "       ,[DocumentCUID] " +
                "       ,[FKClientUID] " +
                "       ,[IssueNumberText] " +
                "       ,[ClientIssueNumber] " +
                "       ,[SourceIssueNumber] " +
                "       ,[Location] " +
                "       ,[FileName] " +
                "       ,[ComboIssueNumber] " +
                "  FROM [ClientDocumentVersion]" +
                " WHERE CUID = '{0}'", this.UID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            this.UID = Convert.ToInt32( reader["UID"].ToString());
                            this.FKClientDocumentUID = Convert.ToInt32(reader["FKClientDocumentUID"].ToString());
                            this.FKClientUID = Convert.ToInt32(reader["FKClientUID"].ToString());
                            this.IssueNumberText = reader["IssueNumberText"].ToString();
                            this.ClientIssueNumber = Convert.ToInt32(reader["ClientIssueNumber"].ToString());
                            this.SourceIssueNumber = Convert.ToInt32(reader["SourceIssueNumber"].ToString());
                            this.Location = reader["Location"].ToString();
                            this.FileName = reader["FileName"].ToString();
                            this.ComboIssueNumber = reader["ComboIssueNumber"].ToString();
                            ret = true;
                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }
            }
            return ret;
        }

        // -----------------------------------------------------
        //   Retrieve last Document Set id
        // -----------------------------------------------------
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                    "SELECT MAX([UID]) LASTUID FROM [ClientDocumentVersion]";

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
        //    Add new Issue
        // -----------------------------------------------------
        public void Add()
        {

            string ret = "Item updated successfully";

            int _uid = 0;

            _uid = GetLastUID() + 1;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [ClientDocumentVersion] " +
                   " ( " +
                   " [UID] " +
                   ",[FKClientDocumentUID] " +
                   ",[FKClientUID] " +
                   ",[IssueNumberText]" +
                   ",[ComboIssueNumber]" +
                   ",[ClientIssueNumber]" +
                   ",[SourceIssueNumber]" +
                   ",[Location]" +
                   ",[FileName]" +
                   ")" +
                        " VALUES " +
                   " ( " +
                   "  @UID    " +
                   ", @FKClientDocumentUID  " +
                   ", @FKClientUID  " +
                   ", @IssueNumberText " +
                   ", @ComboIssueNumber " +
                   ", @ClientIssueNumber " +
                   ", @SourceIssueNumber " +
                   ", @Location " +
                   ", @FileName " +
                   " ) "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = _uid;
                    command.Parameters.Add("@FKClientDocumentUID", SqlDbType.BigInt).Value = FKClientDocumentUID;
                    command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = FKClientUID;
                    command.Parameters.Add("@IssueNumberText", SqlDbType.VarChar).Value = IssueNumberText;
                    command.Parameters.Add("@ComboIssueNumber", SqlDbType.VarChar).Value = ComboIssueNumber;
                    command.Parameters.Add("@ClientIssueNumber", SqlDbType.Decimal).Value = ClientIssueNumber;
                    command.Parameters.Add("@SourceIssueNumber", SqlDbType.VarChar).Value = SourceIssueNumber;
                    command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                    command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }
    }
}
