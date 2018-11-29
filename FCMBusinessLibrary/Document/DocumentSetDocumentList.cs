using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.Document
{
    public class DocumentSetDocumentList
    {
        public List<scDocoSetDocumentLink> documentSetDocumentList;

        //
        // It returns a list of links for a given document set UID
        //
        public void List(DocumentSet documentSet)
        {
            documentSetDocumentList = new List<scDocoSetDocumentLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "  [UID] " +
                " ,[FKDocumentUID] " +
                " ,[FKDocumentSetUID] " +
                " ,[StartDate] " +
                " ,[EndDate] " +
                " ,[IsVoid] " +
                "   FROM [DocumentSetDocument] " +
                "   WHERE FKDocumentSetUID = {0} ",
                documentSet.UID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Ignore voids
                            if (Convert.ToChar(reader["IsVoid"]) == 'Y')
                                continue;

                            var docItem = new scDocoSetDocumentLink();

                            // Get document
                            //
                            docItem.document = new Document();
                            docItem.document.UID = Convert.ToInt32( reader["FKDocumentUID"] );
                            docItem.document.Read();

                            // Get DocumentSet
                            //
                            docItem.documentSet = new DocumentSet();
                            docItem.documentSet.UID = Convert.ToInt32(reader["FKDocumentSetUID"].ToString());

                            // Set DocumentSetDocument
                            docItem.DocumentSetDocument = new DocumentSetDocument();
                            docItem.DocumentSetDocument.UID = Convert.ToInt32(reader["UID"].ToString());
                            docItem.DocumentSetDocument.FKDocumentUID = Convert.ToInt32(reader["FKDocumentUID"].ToString());
                            docItem.DocumentSetDocument.FKDocumentSetUID = Convert.ToInt32(reader["FKDocumentSetUID"].ToString());
                            docItem.DocumentSetDocument.IsVoid = Convert.ToChar(reader["IsVoid"].ToString());
                            docItem.DocumentSetDocument.StartDate = Convert.ToDateTime(reader["StartDate"].ToString());

                            if (reader["EndDate"] == null)
                            {
                                docItem.DocumentSetDocument.EndDate = System.DateTime.MaxValue;
                            }
                            else
                            {
                                docItem.DocumentSetDocument.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                            }
                            documentSetDocumentList.Add(docItem);
                        }
                    }
                }

                return;
            }
        }

        // This method links the list of documents requested to
        // the document set requested
        //
        public void CopyDocumentList(DocumentList documentList, DocumentSet documentSet)
        {

        }
    }
}
