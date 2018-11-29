using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FCMBusinessLibrary.Document
{
    public class DocumentSetDocumentLinkList
    {
        public List<DocumentSetDocumentLink> documentSetDocumentLinkList;

        public static DocumentSetDocumentLinkList ListRelatedDocuments(int documentSetUID, int documentUID, string type)
        {
            DocumentSetDocumentLinkList ret = new DocumentSetDocumentLinkList();

            ret.documentSetDocumentLinkList = new List<DocumentSetDocumentLink>();

            string linktype = "";
            if (type == "ALL" || string.IsNullOrEmpty(type))
            {
                // do nothing
            }
            else
            {
                linktype = "  AND DSDL.LinkType = '" + type + "'";
            }

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format
                (
                   "SELECT DSDL.[UID]                        DSDLUID                 " +
                   "      ,DSDL.[FKParentDocumentUID]        DSDLFKParentDocumentUID " +
                   "       ,DSDL.[FKChildDocumentUID]        DSDLFKChildDocumentUID " +
                   "       ,DSDL.[LinkType]                  DSDLLinkType " +
                   "       ,DSDL.[IsVoid]                    DSDLIsVoid " +
                   "       ," +
                   Document.SQLDocumentConcat("DOCUMENTCHILD") +
                   "       ," +
                   Document.SQLDocumentConcat("DOCUMENTPARENT") +
                   "       ," +
                   DocumentSetDocument.SQLDocumentConcat("DSDCHILD") +
                   "       ," +
                   DocumentSetDocument.SQLDocumentConcat("DSDPARENT") +

                   "  " +
                   "   FROM [management].[dbo].[DocumentSetDocumentLink] DSDL " +
                   "       ,[management].[dbo].[DocumentSetDocument] DSDCHILD " +
                   "       ,[management].[dbo].[DocumentSetDocument] DSDPARENT " +
                   "       ,[management].[dbo].[Document] DOCUMENTCHILD " +
                   "       ,[management].[dbo].[Document] DOCUMENTPARENT " +
                   "  WHERE  " +
                   "        DSDL.[IsVoid] = 'N' " +
                   "    AND DSDL.[FKParentDocumentUID] = {0} " +
                   linktype +
                   "    AND DSDL.FKDocumentSetUID = {1}  " +
                   "    AND DSDL.FKChildDocumentUID = DSDCHILD.UID  " +
                   "    AND DSDL.FKParentDocumentUID = DSDPARENT.UID  " +
                   "    AND DSDCHILD.FKDocumentUID = DOCUMENTCHILD.UID " +
                   "    AND DSDPARENT.FKDocumentUID = DOCUMENTPARENT.UID "
                   , documentUID
                   , documentSetUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DocumentSetDocumentLink _Document = new DocumentSetDocumentLink();
                            _Document.documentChild = new Document();
                            _Document.documentParent = new Document();
                            _Document.documentSetDocumentChild = new DocumentSetDocument();
                            _Document.documentSetDocumentParent = new DocumentSetDocument();

                            // Link information
                            //
                            _Document.FKChildDocumentUID = Convert.ToInt32(reader["DSDLFKChildDocumentUID"].ToString());
                            _Document.FKParentDocumentUID = Convert.ToInt32(reader["DSDLFKParentDocumentUID"].ToString());
                            _Document.LinkType = reader["DSDLLinkType"].ToString();

                            _Document.documentParent.UID = Convert.ToInt32(reader["DOCUMENTPARENTUID"].ToString());
                            _Document.documentParent.SimpleFileName = reader["DOCUMENTPARENTUID"].ToString();
                            _Document.documentParent.CUID = reader["DOCUMENTPARENTSimpleFileName"].ToString();
                            _Document.documentParent.Name = reader["DOCUMENTPARENTName"].ToString();

                            _Document.documentChild.UID = Convert.ToInt32(reader["DOCUMENTCHILDUID"].ToString());
                            _Document.documentChild.CUID = reader["DOCUMENTCHILDSimpleFileName"].ToString();
                            _Document.documentChild.Name = reader["DOCUMENTCHILDName"].ToString();
                            _Document.documentChild.SequenceNumber = Convert.ToInt32(reader["DOCUMENTCHILDSequenceNumber"].ToString());
                            _Document.documentChild.IssueNumber = Convert.ToInt32(reader["DOCUMENTCHILDIssueNumber"].ToString());
                            _Document.documentChild.Location = reader["DOCUMENTCHILDLocation"].ToString();
                            _Document.documentChild.Comments = reader["DOCUMENTCHILDComments"].ToString();
                            _Document.documentChild.SourceCode = reader["DOCUMENTCHILDSourceCode"].ToString();
                            _Document.documentChild.FileName = reader["DOCUMENTCHILDFileName"].ToString();
                            _Document.documentChild.SimpleFileName = reader["DOCUMENTCHILDSimpleFileName"].ToString();
                            _Document.documentChild.FKClientUID = Convert.ToInt32(reader["DOCUMENTCHILDFKClientUID"].ToString());
                            _Document.documentChild.ParentUID = Convert.ToInt32(reader["DOCUMENTCHILDParentUID"].ToString());
                            _Document.documentChild.RecordType = reader["DOCUMENTCHILDRecordType"].ToString();
                            _Document.documentChild.IsProjectPlan = reader["DOCUMENTCHILDIsProjectPlan"].ToString();
                            _Document.documentChild.DocumentType = reader["DOCUMENTCHILDDocumentType"].ToString();

                            _Document.documentSetDocumentChild.UID = Convert.ToInt32(reader["DSDCHILDUID"].ToString());
                            _Document.documentSetDocumentChild.FKDocumentUID = Convert.ToInt32(reader["DSDCHILDFKDocumentUID"].ToString());
                            _Document.documentSetDocumentChild.FKDocumentSetUID = Convert.ToInt32(reader["DSDCHILDFKDocumentSetUID"].ToString());
                            _Document.documentSetDocumentChild.Location = reader["DSDCHILDLocation"].ToString();
                            _Document.documentSetDocumentChild.StartDate = Convert.ToDateTime(reader["DSDCHILDStartDate"].ToString());
                            _Document.documentSetDocumentChild.EndDate = Convert.ToDateTime(reader["DSDCHILDEndDate"].ToString());
                            _Document.documentSetDocumentChild.FKParentDocumentUID = Convert.ToInt32(reader["DSDCHILDFKParentDocumentUID"].ToString());
                            _Document.documentSetDocumentChild.SequenceNumber = Convert.ToInt32(reader["DSDCHILDSequenceNumber"].ToString());
                            _Document.documentSetDocumentChild.FKParentDocumentSetUID = Convert.ToInt32(reader["DSDCHILDFKParentDocumentSetUID"].ToString());

                            ret.documentSetDocumentLinkList.Add(_Document);
                        }
                    }
                }
            }
            return ret;

        }

        public static void ListInTree(
            TreeView fileList,
            DocumentSetDocumentLinkList documentList,
            Document root)
        {

            // Find root folder
            //
            Document rootDocument = new Document();

            rootDocument.CUID = root.CUID;
            rootDocument.RecordType = root.RecordType;
            rootDocument.UID = root.UID;
            rootDocument.Read();

            // Create root
            //
            var rootNode = new TreeNode(rootDocument.Name, FCMConstant.Image.Folder, FCMConstant.Image.Folder);

            // Add root node to tree
            //
            fileList.Nodes.Add(rootNode);
            rootNode.Tag = rootDocument;
            rootNode.Name = rootDocument.Name;

            foreach (var document in documentList.documentSetDocumentLinkList)
            {
                int image = 0;
                image = Utils.ImageSelect(document.documentChild.RecordType.Trim());

                var treeNode = new TreeNode(document.documentChild.Name, image, image);
                treeNode.Tag = document;
                treeNode.Name = document.LinkType;

                rootNode.Nodes.Add(treeNode);
            }
        }

    }
}
