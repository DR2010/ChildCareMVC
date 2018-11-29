using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace fcm
{
    public class DocumentSetLink
    {
        
        public int UID;
        public int FKDocumentUID;
        public int FKDocumentSetUID;
        public string Location;
        public DateTime StartDate;
        public DateTime EndDate;
        public char IsVoid;
        public int ParentUID;
        public int SequenceNumber;

        public void LinkDocument(DocumentSet documentSet, Document document)
        {
            // This method creates a document set link
            //

          using (var connection = new SqlConnection(ConnString.ConnectionString))
          {

              var commandString =
              (
                 "INSERT INTO [DocumentSetLink] " +
                 "([UID] "+ 
                 ",[FKDocumentUID] " +
                 ",[FKDocumentSetUID]" +
                 ",[SequenceNumber]" +
                 ",[Location]" +
                 ",[StartDate]" +
                 ",[EndDate]" +
                 ",[IsVoid]" +
                 ",[ParentUID]" +
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
                 ", @ParentUID" +
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
                  command.Parameters.Add("@ParentUID", SqlDbType.BigInt).Value = ParentUID;

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
                   "UPDATE [DocumentSetLink] " +
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

        public void LinkDocumentListToSet(FCMStructures.ListOfscDocoSetDocumentLink docListToLink)
        {
            // for each document in the list
            // check if it is already linked with document set
            // if it is not linked, add a new link record
            // otherwise, ignore link.

            foreach (var doco in docListToLink.list)
            {

                DocumentSetLink dslLocate = new DocumentSetLink();
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
                    if (doco.documentSetLink.IsVoid == 'Y')
                    {
                        // Update row to make it voided...
                        //
                        Update(doco.documentSetLink.UID);
                    }

                    // else, do nothing
                }
                else
                {

                    // if the pair does not exist, check if it is void.
                    // If void = Y, just ignore.

                    if (doco.documentSetLink.IsVoid == 'Y')
                    {
                        // just ignore. The pair was not saved initially.
                    }
                    else
                    {
                        // add document to set

                        DocumentSetLink dslAdd = new DocumentSetLink();
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
        //    Get Client details
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
                "  FROM [DocumentSetLink]" +
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
                "  FROM [DocumentSetLink]" +
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

            string ret = "Item added successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                (
                   "INSERT INTO [DocumentSetLink] " +
                   "([UID] " +
                   ",[FKDocumentUID] " +
                   ",[FKDocumentSetUID]" +
                   ",[Location]" +
                   ",[IsVoid]" +
                   ",[StartDate]" +
                   ",[EndDate]" +
                   ",[ParentUID]" +
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
                   " ,@ParentUID " +
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
                    command.Parameters.Add("@ParentUID", SqlDbType.BigInt).Value = ParentUID;
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
                   "UPDATE [DocumentSetLink] " +
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
                var commandString = "SELECT MAX([UID]) LASTUID FROM [DocumentSetLink]";

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
    }
}
