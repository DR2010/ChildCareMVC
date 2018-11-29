using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.Document
{
    public class DocumentVersion
    {

        #region Properties
        public int UID { get { return _UID; } set { _UID = value; } }
        public int FKDocumentUID { get { return _FKDocumentUID; } set { _FKDocumentUID = value; } }
        public string FKDocumentCUID { get { return _FKDocumentCUID; } set { _FKDocumentCUID = value; } }
        public decimal IssueNumber { get { return _IssueNumber; } set { _IssueNumber = value; } }
        public string Location { get { return _Location; } set { _Location = value; } }
        public string FileName { get { return _FileName; } set { _FileName = value; } }

        public string UserIdCreatedBy { get { return _UserIdCreatedBy; } set { _UserIdCreatedBy = value; } }
        public string UserIdUpdatedBy { get { return _UserIdUpdatedBy; } set { _UserIdUpdatedBy = value; } }
        public DateTime CreationDateTime { get { return _CreationDateTime; } set { _CreationDateTime = value; } }
        public DateTime UpdateDateTime { get { return _UpdateDateTime; } set { _UpdateDateTime = value; } }

        #endregion Properties

        #region Attributes
        public int _UID;
        public int _FKDocumentUID;
        public string _FKDocumentCUID;
        public decimal _IssueNumber;
        public string _Location;
        public string _FileName;

        public DateTime _CreationDateTime;
        public DateTime _UpdateDateTime;
        public string _UserIdCreatedBy;
        public string _UserIdUpdatedBy;

        
        #endregion Attributes

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string FKDocumentUID = "FKDocumentUID";
            public const string FKDocumentCUID = "FKDocumentCUID";
            public const string IssueNumber = "IssueNumber";
            public const string Location = "Location";
            public const string FileName = "FileName";

            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
        }

        public const string TableName = "DocumentVersion";

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
                " SELECT " +
                DocumentVersionFieldString() +
                "  FROM [DocumentVersion]" +
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
                            this.FKDocumentUID = Convert.ToInt32( reader["FKDocumentUID"].ToString() );
                            this.FKDocumentCUID = reader["FKDocumentCUID"].ToString();
                            this.IssueNumber = Convert.ToInt32(reader["IssueNumber"].ToString());
                            this.Location = reader["Location"].ToString();
                            this.FileName = reader["FileName"].ToString();
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
                    "SELECT MAX([UID]) LASTUID FROM [DocumentVersion]";

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
                   "INSERT INTO [DocumentVersion] " +
                   " ( " +
                   DocumentVersionFieldString() +
                   ")" +
                        " VALUES " +
                   " ( " +
                   "  @UID    " +
                   ", @FKDocumentUID  " +
                   ", @FKDocumentCUID " +
                   ", @IssueNumber " +
                   ", @Location " +
                   ", @FileName " +
                   ", @CreationDateTime  " +
                   ", @UpdateDateTime " +
                   ", @UserIdCreatedBy " +
                   ", @UserIdUpdatedBy " +
                   " ) "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    this.UID = _uid;
                    AddSQLParameters(command, FCMConstant.SQLAction.CREATE);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        /// <summary>
        /// Add sql parameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="action"></param>
        private void AddSQLParameters(SqlCommand command, string action)
        {

            command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
            command.Parameters.Add("@FKDocumentUID", SqlDbType.BigInt).Value = FKDocumentUID;
            command.Parameters.Add("@FKDocumentCUID", SqlDbType.VarChar).Value = FKDocumentCUID;
            command.Parameters.Add("@IssueNumber", SqlDbType.Decimal).Value = IssueNumber;
            command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
            command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;

            command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime, 8).Value = DateTime.Now;

            command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = Utils.UserID;

            if (action == FCMConstant.SQLAction.CREATE)
            {
                command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime, 8).Value = DateTime.Now;
                command.Parameters.Add("@UserIdCreatedBy", SqlDbType.VarChar).Value = Utils.UserID;
            }
        }


        /// <summary>
        /// Load reader information into object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="documentIssue"></param>
        private static void LoadObject(SqlDataReader reader, DocumentVersion documentIssue)
        {

            documentIssue.UID = Convert.ToInt32(reader["UID"].ToString());
            documentIssue.FKDocumentUID = Convert.ToInt32(reader["FKDocumentUID"].ToString());
            documentIssue.FKDocumentCUID = reader["FKDocumentCUID"].ToString();
            documentIssue.IssueNumber = Convert.ToInt32(reader["IssueNumber"].ToString());
            documentIssue.Location = reader["Location"].ToString();
            documentIssue.FileName = reader["FileName"].ToString();
        }

        /// <summary>
        /// Client string of fields.
        /// </summary>
        /// <returns></returns>
        private static string DocumentVersionFieldString()
        {
            return (
                        FieldName.UID
                + "," + FieldName.FKDocumentUID
                + "," + FieldName.FKDocumentCUID
                + "," + FieldName.IssueNumber
                + "," + FieldName.Location
                + "," + FieldName.FileName

                + "," + FieldName.UpdateDateTime
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UserIdCreatedBy
                + "," + FieldName.UserIdUpdatedBy
            );

        }



        // -----------------------------------------------------
        //    List Documents
        // -----------------------------------------------------
        public static List<DocumentVersion> List(Document document)
        {

            var documentIssueList = new List<DocumentVersion>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                    " SELECT " +
                    DocumentVersionFieldString() +
                    "  FROM " +
                    DocumentVersion.TableName +
                    " WHERE FKDocumentUID = '{0}'", document.UID);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DocumentVersion documentIssue = new DocumentVersion();
                            LoadObject(reader, documentIssue);

                            documentIssueList.Add(documentIssue);

                        }
                    }
                }
            }

            return documentIssueList;
        }
    }
}
