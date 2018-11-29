using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;



namespace FCMBusinessLibrary
{
    public class ClientDocumentLinkList
    {
        public List<ClientDocumentLink> clientDocumentLinkList;

        // -----------------------------------
        //        List children documents
        // -----------------------------------
        public void ListChildrenDocuments(
            int clientUID,
            int clientDocumentSetUID, 
            int documentUID, 
            string type )
            
        {

            string linktype = "";
            if (type == "ALL" || string.IsNullOrEmpty( type ))
            {
                // do nothing
            }
            else
            {
                linktype = "  AND CDL.LinkType = '" + type + "'";
            }

            // The client document UID is unique
            // The link table does not have document set uid
            //
            
            clientDocumentLinkList = new List<ClientDocumentLink>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "   CDL.[UID]       " +
                "  ,CDL.[FKChildDocumentUID]     " +
                "  ,CDL.[FKParentDocumentUID]    " +
                "  ,CDL.[LinkType]       " +
                "  ,CDL.[IsVoid]         " +
                "  ,CD.[UID]                    childUID" +
                "  ,CD.[FKClientUID]            childFKClientUID" +
                "  ,CD.[FKClientDocumentSetUID] childFKClientDocumentSetUID" +
                "  ,CD.[FKDocumentUID]          childFKDocumentUID" +
                "  ,CD.[SequenceNumber]         childSequenceNumber" +
                "  ,CD.[StartDate]              childStartDate" +
                "  ,CD.[EndDate]                childEndDate" +
                "  ,CD.[SourceLocation]         childSourceLocation" +
                "  ,CD.[SourceFileName]         childSourceFileName" +
                "  ,CD.[Location]               childLocation" +
                "  ,CD.[FileName]               childFileName" +
                "  ,CD.[SourceIssueNumber]            childIssueNumber" +
                "  ,CD.[Generated]              childGenerated" +
                "  ,CD.[RecordType]             childRecordType" +
                "  ,CD.[ParentUID]              childParentUID" +
                "  ,CD.[IsVoid]                 childIsVoid" +
                "  ,PARENT.[UID]                    parentUID" +
                "  ,PARENT.[FKClientUID]            parentFKClientUID" +
                "  ,PARENT.[FKClientDocumentSetUID] parentFKClientDocumentSetUID" +
                "  ,PARENT.[FKDocumentUID]          parentFKDocumentUID" +
                "  ,PARENT.[SequenceNumber]         parentSequenceNumber" +
                "  ,PARENT.[StartDate]              parentStartDate" +
                "  ,PARENT.[EndDate]                parentEndDate" +
                "  ,PARENT.[SourceLocation]         parentSourceLocation" +
                "  ,PARENT.[SourceFileName]         parentSourceFileName" +
                "  ,PARENT.[Location]               parentLocation" +
                "  ,PARENT.[FileName]               parentFileName" +
                "  ,PARENT.[SourceIssueNumber]            parentIssueNumber" +
                "  ,PARENT.[Generated]              parentGenerated" +
                "  ,PARENT.[RecordType]             parentRecordType" +
                "  ,PARENT.[ParentUID]              parentParentUID" +
                "  ,PARENT.[IsVoid]                 parentIsVoid" +
                "   FROM [ClientDocument] CD, " +
                "        [ClientDocumentLink] CDL," +
                "        [ClientDocument] PARENT" +
                "   WHERE " +
                "        CDL.IsVoid = 'N' " +
                "    AND PARENT.IsVoid = 'N' " +
                "    AND CD.IsVoid = 'N' " +
                "    AND CDL.FKParentDocumentUID = {0} " +
                "    AND CDL.FKClientDocumentSetUID = {1}  " +
                "    AND CDL.FKClientUID = {2}  " +
                "    AND CDL.FKChildDocumentUID  = CD.FKDocumentUID " +
                "    AND CDL.FKParentDocumentUID = PARENT.FKDocumentUID  " +
                linktype +
                "   ORDER BY CD.ParentUID ASC, CD.SequenceNumber ASC "
                   , documentUID
                   , clientDocumentSetUID
                   , clientUID
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

                            var docItem = new ClientDocumentLink();

                            // Set Client Document Link Details
                            //
                            docItem.UID = Convert.ToInt32(reader["UID"].ToString());
                            docItem.FKParentDocumentUID = Convert.ToInt32(reader["FKParentDocumentUID"].ToString());
                            docItem.FKChildDocumentUID = Convert.ToInt32(reader["FKChildDocumentUID"].ToString());
                            docItem.isVoid = Convert.ToChar(reader["IsVoid"].ToString());
                            docItem.LinkType = reader["LinkType"].ToString();

                            // Set Client Document Child
                            //
                            docItem.childClientDocument = new ClientDocument();
                            docItem.childClientDocument.UID = Convert.ToInt32(reader["childUID"].ToString());
                            docItem.childClientDocument.FKDocumentUID = Convert.ToInt32(reader["childFKDocumentUID"].ToString());
                            docItem.childClientDocument.SequenceNumber = Convert.ToInt32(reader["childSequenceNumber"].ToString());
                            docItem.childClientDocument.FKClientDocumentSetUID = Convert.ToInt32(reader["childFKClientDocumentSetUID"].ToString());
                            docItem.childClientDocument.IsVoid = Convert.ToChar(reader["childIsVoid"].ToString());
                            docItem.childClientDocument.StartDate = Convert.ToDateTime(reader["childStartDate"].ToString());
                            docItem.childClientDocument.SourceLocation = reader["childSourceLocation"].ToString();
                            docItem.childClientDocument.SourceFileName = reader["childSourceFileName"].ToString();
                            docItem.childClientDocument.Location = reader["childLocation"].ToString();
                            docItem.childClientDocument.FileName = reader["childFileName"].ToString();
                            docItem.childClientDocument.SourceIssueNumber = Convert.ToInt32(reader["childIssueNumber"].ToString());
                            docItem.childClientDocument.Generated = Convert.ToChar(reader["childGenerated"]);
                            docItem.childClientDocument.RecordType = reader["childRecordType"].ToString();
                            docItem.childClientDocument.ParentUID = Convert.ToInt32(reader["childParentUID"].ToString());
                            //docItem.childClientDocument.EndDate = Convert.ToDateTime(reader["childEndDate"].ToString());
                            docItem.childClientDocument.EndDate = System.DateTime.MaxValue;

                            // Set Client Document Parent
                            //
                            docItem.parentClientDocument = new ClientDocument();
                            docItem.parentClientDocument.UID = Convert.ToInt32(reader["parentUID"].ToString());
                            docItem.parentClientDocument.FKDocumentUID = Convert.ToInt32(reader["parentFKDocumentUID"].ToString());
                            docItem.parentClientDocument.SequenceNumber = Convert.ToInt32(reader["parentSequenceNumber"].ToString());
                            docItem.parentClientDocument.FKClientDocumentSetUID = Convert.ToInt32(reader["parentFKClientDocumentSetUID"].ToString());
                            docItem.parentClientDocument.IsVoid = Convert.ToChar(reader["parentIsVoid"].ToString());
                            docItem.parentClientDocument.StartDate = Convert.ToDateTime(reader["parentStartDate"].ToString());
                            docItem.parentClientDocument.SourceLocation = reader["parentSourceLocation"].ToString();
                            docItem.parentClientDocument.SourceFileName = reader["parentSourceFileName"].ToString();
                            docItem.parentClientDocument.Location = reader["parentLocation"].ToString();
                            docItem.parentClientDocument.FileName = reader["parentFileName"].ToString();
                            docItem.parentClientDocument.SourceIssueNumber = Convert.ToInt32(reader["parentIssueNumber"].ToString());
                            docItem.parentClientDocument.Generated = Convert.ToChar(reader["parentGenerated"]);
                            docItem.parentClientDocument.RecordType = reader["parentRecordType"].ToString();
                            docItem.parentClientDocument.ParentUID = Convert.ToInt32(reader["parentParentUID"].ToString());
                            //docItem.parentClientDocument .EndDate = Convert.ToDateTime(reader["parentEndDate"].ToString());
                            docItem.parentClientDocument.EndDate = System.DateTime.MaxValue;


                            this.clientDocumentLinkList.Add(docItem);
                        }
                    }
                }
            }

        }

        public static ClientDocumentLinkList ListRelatedDocuments(
            int clientUID,
            int clientDocumentSetUID, 
            int documentUID, 
            string type)
        {
            ClientDocumentLinkList ret = new ClientDocumentLinkList();

            ret.clientDocumentLinkList = new List<ClientDocumentLink>();

            string linktype = "";
            if (type == "ALL" || string.IsNullOrEmpty(type))
            {
                // do nothing
            }
            else
            {
                linktype = "  AND CDL.LinkType = '" + type + "'";
            }

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format
                (
                   "SELECT  " +
                   "    CDL.[UID]                     CDLUID   " +
                   "   ,CDL.[FKParentDocumentUID]             CDLFKParentDocumentUID   " +
                   "   ,CDL.[FKChildDocumentUID]              CDLFKChildDocumentUID    " +
                   "   ,CDL.[LinkType]                CDLLinkType   " +
                   "   ,CDL.[IsVoid]                  CDLIsVoid   " +
                   "   ,CDL.[FKClientDocumentSetUID]  CDLFKClientDocumentSetUID   " +
                   "   ,CDL.[FKClientUID]             CDLFKClientUID   " +
                   "  " +
                   "   FROM [management].[dbo].[ClientDocumentLink] CDL " +
                   "  WHERE  " +
                   "        CDL.IsVoid = 'N' " +
                   "    AND CDL.FKParentDocumentUID = {0} " +
                   "    AND CDL.FKClientDocumentSetUID = {1}  " +
                   "    AND CDL.FKClientUID = {2}  " +
                   linktype 
                   , documentUID
                   , clientDocumentSetUID
                   , clientUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientDocumentLink clientDocumentLink = new ClientDocumentLink();
                            clientDocumentLink.UID = Convert.ToInt32(reader["CDLUID"].ToString());
                            clientDocumentLink.FKParentDocumentUID = Convert.ToInt32(reader["CDLFKParentDocumentUID"].ToString());
                            clientDocumentLink.FKChildDocumentUID = Convert.ToInt32(reader["CDLFKChildDocumentUID"].ToString());
                            clientDocumentLink.LinkType = reader["CDLLinkType"].ToString();
                            clientDocumentLink.FKClientDocumentSetUID = 
                                               Convert.ToChar(reader["CDLFKClientDocumentSetUID"].ToString());
                            clientDocumentLink.FKClientUID = Convert.ToInt32(reader["CDLFKClientUID"].ToString());

                            // Get the client document child
                            clientDocumentLink.childClientDocument = new ClientDocument();
                            clientDocumentLink.childClientDocument.UID = clientDocumentLink.FKChildDocumentUID;
                            clientDocumentLink.childClientDocument.Read();

                            // Get the document child
                            clientDocumentLink.childDocument = new Document.Document();
                            clientDocumentLink.childDocument.UID = clientDocumentLink.FKChildDocumentUID;
                            clientDocumentLink.childDocument.Read();

                            // Get the client document parent
                            clientDocumentLink.parentClientDocument = new ClientDocument();
                            clientDocumentLink.parentClientDocument.UID = clientDocumentLink.FKParentDocumentUID;
                            clientDocumentLink.parentClientDocument.Read();

                            // Get the document parent
                            clientDocumentLink.parentDocument = new Document.Document();
                            clientDocumentLink.parentDocument.UID = clientDocumentLink.FKParentDocumentUID;
                            clientDocumentLink.parentDocument.Read();

                            // Get the client document set
                            clientDocumentLink.clientDocumentSet = new ClientDocumentSet();
                            clientDocumentLink.clientDocumentSet.UID = clientDocumentLink.FKClientDocumentSetUID;
                            clientDocumentLink.clientDocumentSet.Read();

                            ret.clientDocumentLinkList.Add(clientDocumentLink);
                        }
                    }
                }
            }
            return ret;

        }


        //
        // Delete links to specific document
        //
        public static void VoidLinks( int clientUID, int clientDocumentSetUID, int documentUID )
        {
            //  11/09/2010 19:03
            //  Continuar daqui. Preciso criar um metodo para apagar todos os links relacionados a um documento
            //  Quando um documento e'deletado do cliente/ client document set preciso apagar o seguinte:
            //     1) ClientDocument
            //     2) ClientDocumentLink
            //     3) 
            // Ja existe metodo para listar os documentos linkados... ClientDocumentLinkList... both directions...


            string ret = "Item updated successfully";

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "UPDATE [management].[dbo].[ClientDocumentLink] " +
                   " SET " +
                   " [IsVoid] = @IsVoid" +
                   "  WHERE  " +
                   "        FKClientDocumentSetUID = @FKClientDocumentSetUID    " +
                   "    AND FKClientUID = @FKClientUID  " +
                   "    AND (    FKParentDocumentUID = @FKDocumentUID " +
                   "          OR FKChildDocumentUID  = @FKDocumentUID )" ); 

                using (var command = new SqlCommand(
                                            commandString, connection ))
                {

                    command.Parameters.Add( "@FKClientDocumentSetUID", SqlDbType.BigInt ).Value = clientDocumentSetUID;
                    command.Parameters.Add( "@FKClientUID", SqlDbType.BigInt ).Value = clientUID;
                    command.Parameters.Add( "@FKDocumentUID", SqlDbType.BigInt ).Value = documentUID;

                    command.Parameters.Add( "@IsVoid", SqlDbType.Char ).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        //
        // Physically Delete links to specific document
        //
        public static void DeleteLinks( int clientUID, int clientDocumentSetUID, int documentUID )
        {

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "DELETE [management].[dbo].[ClientDocumentLink] " +
                   "  WHERE  " +
                   "        FKClientDocumentSetUID = @FKClientDocumentSetUID    " +
                   "    AND FKClientUID = @FKClientUID  " +
                   "    AND (    FKParentDocumentUID = @FKDocumentUID " +
                   "          OR FKChildDocumentUID  = @FKDocumentUID )");

                using (var command = new SqlCommand(
                                            commandString, connection ))
                {

                    command.Parameters.Add( "@FKClientDocumentSetUID", SqlDbType.BigInt ).Value = clientDocumentSetUID;
                    command.Parameters.Add( "@FKClientUID", SqlDbType.BigInt ).Value = clientUID;
                    command.Parameters.Add( "@FKDocumentUID", SqlDbType.BigInt ).Value = documentUID;

                    command.Parameters.Add( "@IsVoid", SqlDbType.Char ).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


    }
}
