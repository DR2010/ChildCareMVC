using System;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.Document
{
    public class DocumentSetDocument
    {
        
        public int UID;
        public int FKDocumentUID;
        public int FKDocumentSetUID;
        public string Location;
        public DateTime StartDate;
        public DateTime EndDate;
        public char IsVoid;
        public int FKParentDocumentUID;
        public int FKParentDocumentSetUID;
        public int SequenceNumber;

        public void LinkDocument(DocumentSet documentSet, Document document)
        {
            // This method creates a document set link
            //

          using (var connection = new SqlConnection(ConnString.ConnectionString))
          {

              var commandString =
              (
                 "INSERT INTO [DocumentSetDocument] " +
                 "([UID] "+ 
                 ",[FKDocumentUID] " +
                 ",[FKDocumentSetUID]" +
                 ",[SequenceNumber]" +
                 ",[Location]" +
                 ",[StartDate]" +
                 ",[EndDate]" +
                 ",[IsVoid]" +
                 ",[FKParentDocumentUID]" +
                 ",[FKParentDocumentSetUID]" +
                 ")" +
                      " VALUES " +
                 "( @UID     " +
                 ", @FKDocumentUID    " +
                 ", @FKDocumentSetUID " +
                 ", @SequenceNumber" +
                 ", @Location" +
                 ", @StartDate" +
                 ", @EndDate" +
                 ", @IsVoid" +
                 ", @FKParentDocumentUID" +
                 ", @FKParentDocumentSetUID" +
                 " ) "
                 );

              using (var command = new SqlCommand(
                                          commandString, connection))
              {
                  command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                  command.Parameters.Add("@FKDocumentUID", SqlDbType.BigInt).Value = FKDocumentUID;
                  command.Parameters.Add("@FKDocumentSetUID", SqlDbType.BigInt).Value = FKDocumentSetUID;
                  command.Parameters.Add("@SequenceNumber", SqlDbType.BigInt).Value = SequenceNumber;
                  command.Parameters.Add("@Location", SqlDbType.Text).Value = Location;
                  command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                  command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = EndDate;
                  command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = IsVoid;
                  command.Parameters.Add("@FKParentDocumentUID", SqlDbType.BigInt).Value = FKParentDocumentUID;
                  command.Parameters.Add("@FKParentDocumentSetUID", SqlDbType.BigInt).Value = documentSet.UID;

                  connection.Open();
                  command.ExecuteNonQuery();
              }
          }
          return;
        }

        public void UnlinkDocument(DocumentSet documentSet, Document document)
        {
            // This method deletes a document set link
            //

            // 1) Look for connection that is not voided
            // 2) Update the IsVoid flag to "Y"; EndDate to Today

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [DocumentSetDocument] " +
                   " SET " +
                   " [EndDate] = @EndDate" +
                   ",[IsVoid] = @IsVoid " +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = DateTime.Today;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public void LinkDocumentListToSet(ListOfscDocoSetDocumentLink docListToLink)
        {
            // for each document in the list
            // check if it is already linked with document set
            // if it is not linked, add a new link record
            // otherwise, ignore link.

            foreach (var doco in docListToLink.list)
            {

                DocumentSetDocument dslLocate = new DocumentSetDocument();
                dslLocate.StartDate = DateTime.Today;
                dslLocate.IsVoid = 'N';
                dslLocate.FKDocumentUID = doco.document.UID;
                dslLocate.FKDocumentSetUID = doco.documentSet.UID;

                if (dslLocate.Find(doco.document.UID, doco.documentSet.UID, 'N' ))
                {
                    // Fact: There is an existing non-voided row
                    // Intention (1): Make it void
                    // Intention (2): Do nothing
                    //

                    // Check for Intention (1)
                    //
                    if (doco.DocumentSetDocument.IsVoid == 'Y')
                    {
                        // Update row to make it voided...
                        //
                        Update(doco.DocumentSetDocument.UID);
                    }

                    // else, do nothing
                }
                else
                {

                    // if the pair does not exist, check if it is void.
                    // If void = Y, just ignore.

                    if (doco.DocumentSetDocument.IsVoid == 'Y')
                    {
                        // just ignore. The pair was not saved initially.
                    }
                    else
                    {
                        // add document to set

                        DocumentSetDocument dslAdd = new DocumentSetDocument();
                        dslAdd.StartDate = DateTime.Today;
                        dslAdd.IsVoid = 'N';
                        dslAdd.FKDocumentUID = doco.document.UID;
                        dslAdd.FKDocumentSetUID = doco.documentSet.UID;

                        dslAdd.Add();
                    }
                }
            }
        }

        // -----------------------------------------------------
        //    Get DocumentSetDocument details
        // -----------------------------------------------------
        public bool Read(bool readVoid)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;
            string voidRead = "";

            if (!readVoid)
            {
                voidRead = "AND IsVoid <> 'Y'";
            }

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT [UID] " +
                "      ,[FKDocumentUID] " +
                "      ,[FKDocumentSetUID] " +
                "      ,[StartDate] " +
                "      ,[EndDate] " +
                "      ,[IsVoid] " +
                "  FROM [DocumentSetDocument]" +
                " WHERE UID = '{0}' "+
                voidRead 
                , this.UID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            this.UID = Convert.ToInt32( reader["UID"].ToString() );
                            this.FKDocumentUID = Convert.ToInt32( reader["FKDocumentUID"].ToString());
                            this.FKDocumentSetUID = Convert.ToInt32( reader["FKDocumentSetUID"].ToString() );
                            this.StartDate = Convert.ToDateTime( reader["StartDate"].ToString());
                            this.EndDate = Convert.ToDateTime( reader["EndDate"].ToString());
                            this.IsVoid = Convert.ToChar(reader["IsVoid"]);

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
        //    Get Client details
        // -----------------------------------------------------
        public bool Find(int documentUID, int docSetUID, char voidRead)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT [UID] " +
                "      ,[FKDocumentUID] " +
                "      ,[FKDocumentSetUID] " +
                "      ,[StartDate] " +
                "      ,[EndDate] " +
                "      ,[IsVoid] " +
                "  FROM [DocumentSetDocument]" +
                " WHERE FKDocumentUID = '{0}' " +
                " AND   FKDocumentSetUID = '{1}' " +
                " AND   IsVoid = '{2}' " 
                , documentUID
                , docSetUID
                , voidRead);

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
                            this.FKDocumentUID = Convert.ToInt32(reader["FKDocumentUID"].ToString());
                            this.FKDocumentSetUID = Convert.ToInt32(reader["FKDocumentSetUID"].ToString());
                            this.StartDate = Convert.ToDateTime(reader["StartDate"].ToString());
                            this.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                            this.IsVoid = Convert.ToChar(reader["IsVoid"]);

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
        //    Add new Document Set Link
        // -----------------------------------------------------
        public void Add()
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                (
                   "INSERT INTO [DocumentSetDocument] " +
                   "([UID] " +
                   ",[FKDocumentUID] " +
                   ",[FKDocumentSetUID]" +
                   ",[Location]" +
                   ",[IsVoid]" +
                   ",[StartDate]" +
                   ",[EndDate]" +
                   ",[FKParentDocumentUID]" +
                   ",[FKParentDocumentSetUID]" +
                   ",[SequenceNumber]" +

                   ")" +
                        " VALUES " +
                   "( @UID " +
                   ", @FKDocumentUID " +
                   ", @FKDocumentSetUID " +
                   ", @Location" +
                   ", @IsVoid " +
                   ", @StartDate " +
                   ", @EndDate " +
                   " ,@FKParentDocumentUID " +
                   " ,@FKParentDocumentSetUID " +
                   ", @SequenceNumber" +
                   " ) "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = GetLastUID() + 1;
                    command.Parameters.Add("@FKDocumentUID", SqlDbType.BigInt).Value = FKDocumentUID;
                    command.Parameters.Add("@FKDocumentSetUID", SqlDbType.BigInt).Value = FKDocumentSetUID;
                    command.Parameters.Add("@Location", SqlDbType.Text).Value = Location;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = IsVoid;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = StartDate;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = System.DateTime.MaxValue;
                    command.Parameters.Add("@FKParentDocumentUID", SqlDbType.BigInt).Value = FKParentDocumentUID;
                    command.Parameters.Add("@FKParentDocumentSetUID", SqlDbType.BigInt).Value = FKParentDocumentSetUID;
                    command.Parameters.Add("@SequenceNumber", SqlDbType.BigInt).Value = SequenceNumber;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        // -----------------------------------------------------
        //    Update Document Set Link
        // -----------------------------------------------------
        private static void Update(int docLinkUID )
        {

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [DocumentSetDocument] " +
                   " SET " +
                   " [IsVoid] = @IsVoid" +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = docLinkUID;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        // -----------------------------------------------------
        //    Update Document Set Document Sequence Number
        // -----------------------------------------------------
        public static void UpdateSequenceNumber(int DocumentSetUID, int DocumentUID, int SequenceNumber)
        {
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [DocumentSetDocument] " +
                   " SET " +
                   " [SequenceNumber] = @SequenceNumber" +
                   " WHERE [FKDocumentUID] = @FKDocumentUID " +
                   "   AND [FKDocumentSetUID] = @FKDocumentSetUID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKDocumentUID", SqlDbType.Char).Value = DocumentUID;
                    command.Parameters.Add("@FKDocumentSetUID", SqlDbType.Char).Value = DocumentSetUID;
                    command.Parameters.Add("@SequenceNumber", SqlDbType.Char).Value = SequenceNumber;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        // -----------------------------------------------------
        //          Retrieve last Document Set UID
        // -----------------------------------------------------
        public int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([UID]) LASTUID FROM [DocumentSetDocument]";

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
        //          Delete Link
        // -----------------------------------------------------
        public static void Delete(int DocumentSetUID, int DocumentUID)
        {

            // Links have to be deleted first
            //

            DocumentSetDocument dsd = new DocumentSetDocument();

            dsd.Find(documentUID: DocumentUID, docSetUID: DocumentSetUID, voidRead: 'N');

            if (dsd.UID <= 0)
                return;

            DocumentSetDocumentLink.DeleteAllRelated(dsd.UID); 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = 
                (
                   "DELETE FROM [DocumentSetDocument] " +
                   " WHERE [FKDocumentUID] = @FKDocumentUID " +
                   "   AND [FKDocumentSetUID] = @FKDocumentSetUID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKDocumentUID", SqlDbType.BigInt).Value = DocumentUID;
                    command.Parameters.Add("@FKDocumentSetUID", SqlDbType.BigInt).Value = DocumentSetUID;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }



        /// <summary>
        /// Load a document from a given reader
        /// </summary>
        /// <param name="retDocument"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="reader"></param>
        public static void LoadDocumentFromReader(
                               Document retDocument,
                               string tablePrefix,
                               SqlDataReader reader)
        {

            retDocument.UID = Convert.ToInt32(reader[tablePrefix + "UID"].ToString());
            retDocument.CUID = reader[tablePrefix + "SimpleFileName"].ToString();
            retDocument.Name = reader[tablePrefix + "Name"].ToString();
            retDocument.SequenceNumber = Convert.ToInt32(reader[tablePrefix + "SequenceNumber"].ToString());
            retDocument.IssueNumber = Convert.ToInt32(reader[tablePrefix + "IssueNumber"].ToString());
            retDocument.Location = reader[tablePrefix + "Location"].ToString();
            retDocument.Comments = reader[tablePrefix + "Comments"].ToString();
            retDocument.SourceCode = reader[tablePrefix + "SourceCode"].ToString();
            retDocument.FileName = reader[tablePrefix + "FileName"].ToString();
            retDocument.SimpleFileName = reader[tablePrefix + "SimpleFileName"].ToString();
            retDocument.FKClientUID = Convert.ToInt32(reader[tablePrefix + "FKClientUID"].ToString());
            retDocument.ParentUID = Convert.ToInt32(reader[tablePrefix + "ParentUID"].ToString());
            retDocument.RecordType = reader[tablePrefix + "RecordType"].ToString();
            retDocument.IsProjectPlan = reader[tablePrefix + "IsProjectPlan"].ToString();
            retDocument.DocumentType = reader[tablePrefix + "DocumentType"].ToString();

            return;
        }
        /// <summary>
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        public static string SQLDocumentConcat(string tablePrefix)
        {
            string ret = " " +
            tablePrefix + ".[UID]                    " + tablePrefix + "UID,              " +
            tablePrefix + ".[FKDocumentUID]          " + tablePrefix + "FKDocumentUID,    " +
            tablePrefix + ".[FKDocumentSetUID]       " + tablePrefix + "FKDocumentSetUID, " +
            tablePrefix + ".[Location]               " + tablePrefix + "Location, " +
            tablePrefix + ".[IsVoid]                 " + tablePrefix + "IsVoid, " +
            tablePrefix + ".[StartDate]              " + tablePrefix + "StartDate, " +
            tablePrefix + ".[EndDate]                " + tablePrefix + "EndDate, " +
            tablePrefix + ".[FKParentDocumentUID]    " + tablePrefix + "FKParentDocumentUID, " +
            tablePrefix + ".[SequenceNumber]         " + tablePrefix + "SequenceNumber, " +
            tablePrefix + ".[FKParentDocumentSetUID] " + tablePrefix + "FKParentDocumentSetUID, " +
            tablePrefix + ".[DocumentType]           " + tablePrefix + "DocumentType    ";

            return ret;
        }
    }
}
