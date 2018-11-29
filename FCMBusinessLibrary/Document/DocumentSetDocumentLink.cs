using System;
using System.Data.SqlClient;
using System.Data;

namespace FCMBusinessLibrary.Document
{
    public class DocumentSetDocumentLink
    {
        public int UID;
        public int FKDocumentSetUID;
        public int FKParentDocumentUID;
        public int FKChildDocumentUID;
        public string LinkType;
        public char IsVoid;
        public Document documentParent;
        public Document documentChild;
        public DocumentSetDocument documentSetDocumentParent;
        public DocumentSetDocument documentSetDocumentChild;


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
                    "SELECT MAX([UID]) LASTUID FROM [DocumentSetDocumentLink]";

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
        //    Add new Link
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
                   "INSERT INTO [DocumentSetDocumentLink] " +
                   " ( " +
                   " [UID] " +
                   ",[FKDocumentSetUID] " +
                   ",[FKParentDocumentUID] " +
                   ",[FKChildDocumentUID]" +
                   ",[LinkType]" +
                   ",[IsVoid]" +
                   ")" +
                        " VALUES " +
                   " ( " +
                   "  @UID    " +
                   ", @FKDocumentSetUID  " +
                   ", @FKParentDocumentUID  " +
                   ", @FKChildDocumentUID " +
                   ", @LinkType " +
                   ", @IsVoid" +
                   " ) "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = _uid;
                    command.Parameters.Add("@FKDocumentSetUID", SqlDbType.BigInt).Value = FKDocumentSetUID;
                    command.Parameters.Add("@FKParentDocumentUID", SqlDbType.BigInt).Value = FKParentDocumentUID;
                    command.Parameters.Add("@FKChildDocumentUID", SqlDbType.BigInt).Value = FKChildDocumentUID;
                    command.Parameters.Add("@LinkType", SqlDbType.VarChar).Value = LinkType;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'N';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        // -----------------------------------------------------
        //          Delete Link
        // -----------------------------------------------------
        public static void DeleteAllRelated(int DocumentSetDocumentUID)
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = 
                (
                   "DELETE FROM [DocumentSetDocumentLink]  " +
                   " WHERE [FKParentDocumentUID] = @DocumentSetDocumentUID " +
                   "   OR [FKChildDocumentUID] = @DocumentSetDocumentUID " 
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@DocumentSetDocumentUID", SqlDbType.BigInt).Value = DocumentSetDocumentUID;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;

        }


        // -----------------------------------------------------
        //    Save Links
        // -----------------------------------------------------
        public static void LinkDocuments(int DocumentSetUID, int ParentID, int ChildID, string LinkType)
        {
            DocumentSetDocumentLink findOne = new DocumentSetDocumentLink();
            if (findOne.Read(DocumentSetUID, ParentID, ChildID, LinkType))
            {
                // Already exists
            }
            else
            {
                findOne.LinkType = LinkType;
                findOne.FKDocumentSetUID = DocumentSetUID;
                findOne.FKParentDocumentUID = ParentID;
                findOne.FKChildDocumentUID = ChildID;

                findOne.Add();
            }

        }

        // -----------------------------------------------------
        //    Get Link details
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
                "       ,[FKParentDocumentUID] " +
                "       ,[FKChildDocumentUID] " +
                "       ,[LinkType] " +
                "  FROM [DocumentSetDocumentLink]" +
                " WHERE IsVoid = 'N' " +
                "   AND CUID = '{0}'", this.UID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            this.UID = Convert.ToInt32(reader["UID"].ToString());
                            this.FKParentDocumentUID = Convert.ToInt32(reader["FKParentDocumentUID "].ToString());
                            this.FKChildDocumentUID = Convert.ToInt32(reader["FKChildDocumentUID"].ToString());
                            this.LinkType = reader["LinkType"].ToString();
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
        //    Get Link details
        // -----------------------------------------------------
        public bool Read(int DocumentSetUID, int ParentID, int ChildID, string LinkType)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT [UID] " +
                "       ,[FKDocumentSetUID] " +
                "       ,[FKParentDocumentUID] " +
                "       ,[FKChildDocumentUID] " +
                "       ,[LinkType] " +
                "  FROM [DocumentSetDocumentLink]" +
                " WHERE IsVoid = 'N' " +
                "   AND FKDocumentSetUID = '{0}'" +
                "   AND FKParentDocumentUID = '{1}'" +
                "   AND FKChildDocumentUID = '{2}'" +
                "   AND LinkType = '{3}' ",
                DocumentSetUID,
                ParentID,
                ChildID,
                LinkType);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            this.UID = Convert.ToInt32(reader["UID"].ToString());
                            this.FKDocumentSetUID = Convert.ToInt32(reader["FKDocumentSetUID"].ToString());
                            this.FKParentDocumentUID = Convert.ToInt32(reader["FKParentDocumentUID"].ToString());
                            this.FKChildDocumentUID = Convert.ToInt32(reader["FKChildDocumentUID"].ToString());
                            this.LinkType = reader["LinkType"].ToString();
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
        // Logical Delete
        // -----------------------------------------------------
        public void Delete(int UID)
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [DocumentSetDocumentLink] " +
                   " SET " +
                   " [IsVoid] = @IsVoid" +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }
    }
}
