using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;

namespace FCMBusinessLibrary.Document
{
    public class DocumentLinkList
    {
        public List<DocumentLink> documentLinkList;

        // -----------------------------------------------------
        //    List related documents
        // -----------------------------------------------------
        public static DocumentLinkList ListRelatedDocuments(int docUID, string type)
        {
            DocumentLinkList ret = new DocumentLinkList();

            ret.documentLinkList = new List<DocumentLink>();
            string linktype="";
            if (type == "ALL" || string.IsNullOrEmpty(type))
            {
                // do nothing
            }
            else
            {
                linktype = "  AND link.LinkType = '" + type + "'";
            }
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                    " SELECT " +
                    " link.FKParentDocumentUID " +
                    " ,doc.[UID] DOCUID " +
                    " ,link.[LinkType] " +
                    " ,doc.[CUID] "+ 
                    " ,doc.[Name] "+
                    " ,doc.[SequenceNumber],doc.[IssueNumber] " +
                    " ,doc.[Location],doc.[Comments],doc.[SourceCode],doc.[FileName] " +
                    " ,doc.[FKClientUID],doc.[ParentUID] " +
                    " ,doc.[RecordType],doc.[IsProjectPlan] " +
                    " ,docFrom.[CUID] docFromCUID "+
                    " ,docFrom.[Name] docFromName " + 
                    " ,docFrom.[SequenceNumber] " +
                    " ,docFrom.[IssueNumber] " +
                    " ,docFrom.[UID] docFromUID " +
                    " ,docFrom.[Location] docFromLocation " + 
                    " ,docFrom.[Comments] " +
                    " ,docFrom.[SourceCode],docFrom.[FileName]  " +
                    " ,docFrom.[FKClientUID],docFrom.[ParentUID] " +
                    " ,docFrom.[RecordType],docFrom.[IsProjectPlan] " +
                    " ,link.[UID],link.[IsVoid]  " +
                    " FROM [Document] doc,  " +
                    "      [DocumentLink] link, " + 
                    "      [Document] docFrom  " +
                    " WHERE  " +
                    "      link.IsVoid            = 'N'   " +
                    linktype +
                    "  AND link.FKParentDocumentUID = {0} " +
                    "  AND doc.UID                = link.FKChildDocumentUID " +
                    "  AND docFrom.UID            = link.FKParentDocumentUID ",
                    docUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DocumentLink _Document = new DocumentLink();
                            _Document.FKParentDocumentUID = docUID;

                            _Document.documentFrom = new Document();
                            _Document.documentTo = new Document();

                            _Document.documentTo.UID = Convert.ToInt32(reader["DOCUID"].ToString());
                            _Document.FKChildDocumentUID = _Document.documentTo.UID;
                            _Document.documentTo.CUID = reader["CUID"].ToString();
                            _Document.documentTo.Name = reader["Name"].ToString();
                            _Document.documentTo.SequenceNumber = Convert.ToInt32(reader["SequenceNumber"].ToString());
                            _Document.documentTo.IssueNumber = Convert.ToInt32(reader["IssueNumber"].ToString());
                            _Document.documentTo.Location = reader["Location"].ToString();
                            _Document.documentTo.Comments = reader["Comments"].ToString();
                            _Document.documentTo.SourceCode = reader["SourceCode"].ToString();
                            _Document.documentTo.FileName = reader["FileName"].ToString();
                            _Document.documentTo.FKClientUID = Convert.ToInt32(reader["FKClientUID"].ToString());
                            _Document.documentTo.ParentUID = Convert.ToInt32(reader["ParentUID"].ToString());
                            _Document.documentTo.RecordType = reader["RecordType"].ToString();
                            _Document.documentTo.IsProjectPlan = reader["IsProjectPlan"].ToString();
                            _Document.LinkType = reader["LinkType"].ToString();
                            _Document.documentFrom.UID = Convert.ToInt32(reader["docFromUID"].ToString());
                            _Document.documentFrom.Name = reader["docFromName"].ToString();
                            string isVoid = reader["IsVoid"].ToString();

                            ret.documentLinkList.Add(_Document);
                        }
                    }
                }
            }
            return ret;
        }

        // -----------------------------------------------------
        //           Load documents in a tree
        // -----------------------------------------------------
        public static void ListInTree(
            TreeView fileList,
            DocumentLinkList documentList,
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

            foreach (var document in documentList.documentLinkList)
            {
                // Ignore root folder
                if (document.documentTo.CUID == "ROOT") continue;

                // Check if folder has a parent
                string cdocumentUID = document.UID.ToString();
                string cparentIUID = document.documentTo.ParentUID.ToString();

                int image = 0;

                document.documentTo.RecordType = document.documentTo.RecordType.Trim();

                image = Utils.ImageSelect(document.documentTo.RecordType);

                if (document.documentTo.ParentUID == 0)
                {
                    var treeNode = new TreeNode(document.documentTo.Name, image, image);
                    treeNode.Tag = document;
                    treeNode.Name = cdocumentUID;

                    rootNode.Nodes.Add(treeNode);
                }
                else
                {
                    // Find the parent node
                    //
                    var node = fileList.Nodes.Find(cparentIUID, true);

                    if (node.Count() > 0)
                    {

                        var treeNode = new TreeNode(document.documentTo.Name, image, image);
                        treeNode.Tag = document;
                        treeNode.Name = cdocumentUID;

                        node[0].Nodes.Add(treeNode);
                    }
                    else
                    {
                        // Add Element to the root
                        //
                        var treeNode = new TreeNode(document.documentTo.Name, image, image);
                        treeNode.Tag = document;
                        treeNode.Name = cdocumentUID;

                        rootNode.Nodes.Add(treeNode);

                    }
                }
            }
        }
 


        // -----------------------------------------------------
        //    List related documents
        // -----------------------------------------------------
        public static DocumentLinkList ListRelatedDocuments(int docUID)
        {
            return ListRelatedDocuments(docUID, "ALL");
        }
    }
}
