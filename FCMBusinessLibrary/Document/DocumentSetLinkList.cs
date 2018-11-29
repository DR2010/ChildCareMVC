using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace fcm
{
    public class DocumentSetLinkList
    {
        public List<FCMStructures.scDocoSetDocumentLink> documentSetLinkList;

        //
        // It returns a list of links for a given document set UID
        //
        public void List(DocumentSet documentSet)
        {
            documentSetLinkList = new List<FCMStructures.scDocoSetDocumentLink>();

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
                "   FROM [DocumentSetLink] " +
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

                            var docItem = new FCMStructures.scDocoSetDocumentLink();

                            // Get document
                            //
                            docItem.document = new Document();
                            docItem.document.UID = Convert.ToInt32( reader["FKDocumentUID"] );
                            docItem.document.Read();

                            // Get DocumentSet
                            //
                            docItem.documentSet = new DocumentSet();
                            docItem.documentSet.UID = Convert.ToInt32(reader["FKDocumentSetUID"].ToString());

                            // Set DocumentSetLink
                            docItem.documentSetLink = new DocumentSetLink();
                            docItem.documentSetLink.UID = Convert.ToInt32(reader["UID"].ToString());
                            docItem.documentSetLink.FKDocumentUID = Convert.ToInt32(reader["FKDocumentUID"].ToString());
                            docItem.documentSetLink.FKDocumentSetUID = Convert.ToInt32(reader["FKDocumentSetUID"].ToString());
                            docItem.documentSetLink.IsVoid = Convert.ToChar(reader["IsVoid"].ToString());
                            docItem.documentSetLink.StartDate = Convert.ToDateTime(reader["StartDate"].ToString());

                            if (reader["EndDate"] == null)
                            {
                                docItem.documentSetLink.EndDate = System.DateTime.MaxValue;
                            }
                            else
                            {
                                docItem.documentSetLink.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                            }
                            documentSetLinkList.Add(docItem);
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
