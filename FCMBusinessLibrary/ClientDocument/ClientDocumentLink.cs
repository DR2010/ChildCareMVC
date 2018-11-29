using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using FCMBusinessLibrary.Document;

namespace FCMBusinessLibrary
{
    public class ClientDocumentLink
    {
        public int UID;
        public int FKClientUID;
        public int FKParentDocumentUID;
        public int FKChildDocumentUID;
        public int FKClientDocumentSetUID;
        public string LinkType;
        public char isVoid;
        public ClientDocument parentClientDocument;
        public ClientDocument childClientDocument;
        public ClientDocumentSet clientDocumentSet;
        public Document.Document parentDocument;
        public Document.Document childDocument;

        public List<scClientDocSetDocLink> clientDocSetDocLink;

        // -----------------------------------------------------
        //    Add new Link
        // -----------------------------------------------------
        public void Add()
        {

            int _uid = 0;

            _uid = GetLastUID() + 1;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [ClientDocumentLink] " +
                   " ( " +
                   " [UID] " +
                   ",[FKParentDocumentUID] " +
                   ",[FKChildDocumentUID]" +
                   ",[FKClientDocumentSetUID]" +
                   ",[FKClientUID]" +
                   ",[LinkType]" +
                   ",[IsVoid]" +
                   ")" +
                        " VALUES " +
                   " ( " +
                   "  @UID    " +
                   ", @FKParentDocumentUID  " +
                   ", @FKChildDocumentUID " +
                   ", @FKClientDocumentSetUID " +
                   ", @FKClientUID " +
                   ", @LinkType " +
                   ", @IsVoid" +
                   " ) "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = _uid;
                    command.Parameters.Add("@FKParentDocumentUID", SqlDbType.BigInt).Value = FKParentDocumentUID;
                    command.Parameters.Add("@FKChildDocumentUID", SqlDbType.BigInt).Value = FKChildDocumentUID;
                    command.Parameters.Add("@FKClientDocumentSetUID ", SqlDbType.BigInt).Value = FKClientDocumentSetUID;
                    command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = FKClientUID;
                    command.Parameters.Add("@LinkType", SqlDbType.VarChar).Value = LinkType;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'N';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
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
                    "SELECT MAX([UID]) LASTUID FROM [ClientDocumentLink]";

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

        //
        // List documents for a client
        //
        public void List(int clientID, int clientDocumentSetUID)
        {
            clientDocSetDocLink = new List<scClientDocSetDocLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "  [UID] " +
                "  ,[FKClientUID] " +
                "  ,[FKClientDocumentSetUID] " +
                "  ,[FKDocumentUID] " +
                "  ,[SequenceNumber] " +
                "  ,[StartDate] " +
                "  ,[EndDate] " +
                "  ,[IsVoid] " +
                "  ,[SourceLocation] " +
                "  ,[SourceFileName] " +
                "  ,[Location] " +
                "  ,[FileName] " +
                "  ,[SourceIssueNumber] " +
                "  ,[Generated] " +
                "  ,[RecordType] " +
                "  ,[ParentUID] " +
                "   FROM [ClientDocument] " +
                "   WHERE [FKClientUID] = {0} " +
                "   AND [FKClientDocumentSetUID] = {1} " +
                "   ORDER BY ParentUID ASC, SequenceNumber ASC ",
                clientID,
                clientDocumentSetUID
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

                            var docItem = new scClientDocSetDocLink();

                            // Get document
                            //
                            docItem.document = new Document.Document();
                            docItem.document.UID = Convert.ToInt32(reader["FKDocumentUID"]);
                            docItem.document.Read();

                            // Get Client Document Set
                            //
                            docItem.clientDocumentSet = new ClientDocumentSet();
                            docItem.clientDocumentSet.UID = Convert.ToInt32(reader["FKClientDocumentSetUID"].ToString());
                            docItem.clientDocumentSet.FKClientUID = Convert.ToInt32(reader["FKClientUID"].ToString());

                            // Set Client Document 
                            //
                            docItem.clientDocument = new ClientDocument();
                            docItem.clientDocument.UID = Convert.ToInt32(reader["UID"].ToString());
                            docItem.clientDocument.FKDocumentUID = Convert.ToInt32(reader["FKDocumentUID"].ToString());
                            docItem.clientDocument.SequenceNumber = Convert.ToInt32(reader["SequenceNumber"].ToString());
                            docItem.clientDocument.FKClientDocumentSetUID = Convert.ToInt32(reader["FKClientDocumentSetUID"].ToString());
                            docItem.clientDocument.IsVoid = Convert.ToChar(reader["IsVoid"].ToString());
                            docItem.clientDocument.StartDate = Convert.ToDateTime(reader["StartDate"].ToString());
                            docItem.clientDocument.SourceLocation = reader["SourceLocation"].ToString();
                            docItem.clientDocument.SourceFileName = reader["SourceFileName"].ToString();
                            docItem.clientDocument.Location = reader["Location"].ToString();
                            docItem.clientDocument.FileName = reader["FileName"].ToString();
                            docItem.clientDocument.SourceIssueNumber = Convert.ToInt32(reader["SourceIssueNumber"].ToString());
                            docItem.clientDocument.Generated = Convert.ToChar(reader["Generated"]);
                            docItem.clientDocument.RecordType = reader["RecordType"].ToString();
                            docItem.clientDocument.ParentUID = Convert.ToInt32(reader["ParentUID"].ToString());


                            try
                            {
                                docItem.clientDocument.EndDate = Convert.ToDateTime(reader["EndDate"].ToString());
                            }
                            catch
                            {
                                docItem.clientDocument.EndDate = System.DateTime.MaxValue;
                            }

                            this.clientDocSetDocLink.Add(docItem);
                        }
                    }
                }
            }

        }

        // ---------------------------------------------------------
        //      Copy document links to client document links
        // ---------------------------------------------------------
        public void ReplicateDocLinkToClientDeprecated(int ClientUID, int ClientSetUID)
        {

            // 1... GetClientDocument
            // List documents for a Client Set Document
            //
            var clientDocumentList = new ClientDocument();
            clientDocumentList.List(ClientUID, ClientSetUID);

            // 2... foreach( clientDocument )
            // For each client document, retrieve the linked documents 
            //
            foreach (var clientDocument in clientDocumentList.clientDocSetDocLink)
            {
                // 3...... GetDocumentChildren( currentClientDocument)
                // This part retrieves Document Links and not ClientDocument Links
                // that's why we need to get the equivalent ClientDocumentUID...
                var children = DocumentLinkList.ListRelatedDocuments(clientDocument.document.UID);

                // 4...... foreach (GetDocumentChildren)
                foreach (var child in children.documentLinkList)
                {
                    // 5..... CreateClientDocumentLink(Client,ClientSet,DocumentParent,DocumentChild, Type)

                    ClientDocumentLink newLink = new ClientDocumentLink();
                    // This is the client document UID 
                    newLink.FKParentDocumentUID = clientDocument.clientDocument.UID;

                    // Get clientUID for child document
                    ClientDocument childDocument = new ClientDocument();
                    //childDocument.FKDocumentUID = child.UID;
                    childDocument.FKDocumentUID = child.documentTo.UID;
                    
                    childDocument.Find(child.documentTo.UID, clientDocument.clientDocumentSet.UID, 'N', ClientUID);

                    if (childDocument.UID > 0)
                    {
                        newLink.FKChildDocumentUID = childDocument.UID;

                        newLink.LinkType = child.LinkType; // Replace by link type
                        newLink.Add();
                    }
                }
            }


            //using (var connection = new SqlConnection(ConnString.ConnectionString))
            //{

            //    var commandString = string.Format(
            //    " SELECT " +
            //    "   [UID] " +
            //    "   FROM [ClientDocument] " +
            //    "   WHERE " +
            //    "       [FKClientUID] = {0} " +
            //    "   AND [FKClientDocumentSetUID] = {1} ",
            //    ClientUID,
            //    ClientSetUID
            //    );

            //    using (var command = new SqlCommand(
            //                          commandString, connection))
            //    {
            //        connection.Open();
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                int ClientDocumentUID;
            //                ClientDocumentUID = Convert.ToInt32(reader["UID"].ToString());


            //            }
            //        }
            //    }
            //}

        }


        // ---------------------------------------------------------
        //      Copy document links to client document links
        // ---------------------------------------------------------
        public void ReplicateDocLinkToClientX(int ClientUID, int ClientSetUID)
        {

            // 1... GetClientDocument
            // List documents for a Client Set Document
            //
            var clientDocumentList = new ClientDocument();
            clientDocumentList.List(ClientUID, ClientSetUID);

            // 2... foreach( clientDocument )
            // For each client document, retrieve the linked documents 
            //
            foreach (var clientDocument in clientDocumentList.clientDocSetDocLink)
            {
                // 3...... GetDocumentChildren( currentClientDocument)
                // This part retrieves Document Links and not ClientDocument Links
                var children = DocumentLinkList.ListRelatedDocuments(clientDocument.document.UID);

                // 4...... foreach (GetDocumentChildren)
                foreach (var child in children.documentLinkList)
                {
                    // 5..... CreateClientDocumentLink(Client,ClientSet,DocumentParent,DocumentChild, Type)

                    ClientDocumentLink newLink = new ClientDocumentLink();
                    newLink.FKClientDocumentSetUID = ClientSetUID;
                    newLink.FKClientUID = ClientUID;
                    newLink.FKParentDocumentUID = clientDocument.document.UID;
                    newLink.FKChildDocumentUID = child.FKChildDocumentUID;
                    newLink.LinkType = child.LinkType;
                    newLink.Add();

                }
            }
        }

        // ------------------------------------------------------------
        //      Cop\y document links from Document Set Document Link
        // ------------------------------------------------------------
        public void ReplicateDocSetDocLinkToClient(int ClientUID, int ClientSetUID, int DocumentSetUID)
        {

            // 1... GetClientDocument
            // List documents for a Client Set Document
            //
            var clientDocumentList = new ClientDocument();
            clientDocumentList.List(ClientUID, ClientSetUID);

            // 2... foreach( clientDocument )
            // For each client document, retrieve the linked documents 
            //
            foreach (var clientDocument in clientDocumentList.clientDocSetDocLink)
            {
                // 3...... GetDocumentChildren( currentClientDocument)
                // This part retrieves Document Links and not ClientDocument Links
                var children = DocumentSetDocumentLinkList.ListRelatedDocuments(DocumentSetUID, clientDocument.document.UID, type: "ALL");

                // 4...... foreach (GetDocumentChildren)
                foreach (var child in children.documentSetDocumentLinkList)
                {
                    // 5..... CreateClientDocumentLink(Client,ClientSet,DocumentParent,DocumentChild, Type)

                    ClientDocumentLink newLink = new ClientDocumentLink();
                    newLink.FKClientDocumentSetUID = ClientSetUID;
                    newLink.FKClientUID = ClientUID;
                    newLink.FKParentDocumentUID = clientDocument.document.UID;
                    newLink.FKChildDocumentUID = child.FKChildDocumentUID;
                    newLink.LinkType = child.LinkType;
                    newLink.Add();

                }
            }
        }


        // -----------------------------------------------------
        //    Get Link details
        // -----------------------------------------------------
        public bool Read(int ParentID, int ChildID, string LinkType, int clientUID, int clientDocumentSetUID)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;

            if (ParentID == 0 || ChildID == 0 || string.IsNullOrEmpty(LinkType) || clientUID == 0 || clientDocumentSetUID == 0)
            {
                return false;
            }

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT [UID] " +
                "       ,[FKParentDocumentUID] " +
                "       ,[FKChildDocumentUID] " +
                "       ,[FKClientDocumentSetUID] " +
                "       ,[FKClientUID] " +
                "       ,[LinkType] " +
                "  FROM [ClientDocumentLink]" +
                " WHERE IsVoid = 'N' " +
                "   AND FKParentDocumentUID = {0}" +
                "   AND FKChildDocumentUID = {1}" +
                "   AND LinkType = '{2}' " + 
                "   AND FKClientUID = {3} " +
                "   AND FKClientDocumentSetUID = {4} ",
                ParentID,
                ChildID,
                LinkType,
                clientUID,
                clientDocumentSetUID);

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
                            this.FKParentDocumentUID = Convert.ToInt32(reader["FKParentDocumentUID"].ToString());
                            this.FKChildDocumentUID = Convert.ToInt32(reader["FKChildDocumentUID"].ToString());
                            this.FKClientDocumentSetUID = Convert.ToInt32(reader["FKClientDocumentSetUID"].ToString());
                            this.FKClientUID = Convert.ToInt32(reader["FKClientUID"].ToString());
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
        //    Save Links
        // -----------------------------------------------------
        public static void LinkDocuments(
            int clientUID, 
            int clientDocumentSetUID,
            int parentDocumentUID, int childDocumentUID,
            string LinkType)
        {
            ClientDocumentLink findOne = new ClientDocumentLink();
            if (findOne.Read(
                    ParentID: parentDocumentUID,
                    ChildID: childDocumentUID,
                    LinkType: LinkType, 
                    clientUID: clientUID, 
                    clientDocumentSetUID:clientDocumentSetUID ))
            {
                // Already exists
            }
            else
            {
                findOne.FKClientUID = clientUID;
                findOne.FKClientDocumentSetUID = clientDocumentSetUID;
                findOne.LinkType = LinkType;
                findOne.FKParentDocumentUID = parentDocumentUID;
                findOne.FKChildDocumentUID = childDocumentUID;

                findOne.Add();
            }

        }

        // -----------------------------------------------------
        // Logical Delete
        // -----------------------------------------------------
        public void Delete(int UID)
        {
            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [ClientDocumentLink] " +
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
