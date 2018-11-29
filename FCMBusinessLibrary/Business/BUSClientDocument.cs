using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FCMBusinessLibrary.Document;

namespace FCMBusinessLibrary.Business
{
    public class BUSClientDocument
    {
        /// <summary>
        /// Client document read
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus ClientDocumentReadS(int clientDocumentUID)
        {
            ResponseStatus response = new ResponseStatus();
            response = ClientDocument.ReadS(clientDocumentUID);
            return response;
        }

        public static ResponseStatus GetClientDocumentPath(ClientDocument clientDocument)
        {
            ResponseStatus response = new ResponseStatus();
            response = ClientDocument.GetDocumentPath(clientDocument);
            return response;
        }

        /// <summary>
        /// Client document list
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static List<scClientDocSetDocLink> List(int clientUID, int clientDocumentSetUID)
        {
            var response = new List<scClientDocSetDocLink>();

            response = ClientDocument.ListS(clientUID, clientDocumentSetUID);
            
            return response;
        }

        /// <summary>
        /// Associate documents from selected document set to selected client
        /// </summary>
        /// <param name="clientUID"></param>
        /// <param name="clientDocumentSetUID"></param>
        /// <param name="documentSetUID"></param>
        public static void AssociateDocumentsToClient(
            ClientDocumentSet clientDocumentSet, 
            int documentSetUID,
            HeaderInfo headerInfo)
        {
            // It is a new client document set
            // It maybe a new client, the client document set MUST be new or empty
            // 1) Instantiate a TREE for the Client Document Set document
            // 2) Instantiate a second tree for the documents related to that document set
            // 3) Now the old copy all starts, all the nodes from the second tree are moved to the new tree
            //    following current process
            // 4) Save happens as per usual
            //

            TreeView tvFileList = new TreeView(); // This is the list of documents for a client, it should be EMPTY
            TreeView tvDocumentsAvailable = new TreeView(); // This is the list of documents for a client, it should be EMPTY
            string folderOnly = clientDocumentSet.FolderOnly; // Contains the folder location of the file

            // Add root folder
            //
            ClientDocument clientDocument = new ClientDocument();
            clientDocument.AddRootFolder(clientDocumentSet.FKClientUID, clientDocumentSet.ClientSetID, clientDocumentSet.FolderOnly);

            // List client document list !!!!!!! Important because the ROOT folder is loaded ;-)
            
            var documentSetList = new ClientDocument();
            documentSetList.List(clientDocumentSet.FKClientUID, clientDocumentSet.ClientSetID);

            tvFileList.Nodes.Clear();
            documentSetList.ListInTree(tvFileList, "CLIENT");
            if (tvFileList.Nodes.Count > 0)
                tvFileList.Nodes[0].Expand();

            // Load available documents
            //
            tvDocumentsAvailable.Nodes.Clear();

            // Get document list for a given document set
            //
            DocumentSet documentSet = new DocumentSet();
            documentSet.UID = documentSetUID;
            documentSet.Read(IncludeDocuments: 'Y');
            
            // Load document in the treeview
            //
            Document.Document root = new Document.Document();
            root.GetRoot(headerInfo);
            
            DocumentList.ListInTree(tvDocumentsAvailable, documentSet.documentList, root);

            while (tvDocumentsAvailable.Nodes[0].Nodes.Count > 0)
            {
                TreeNode tn = tvDocumentsAvailable.Nodes[0].Nodes[0];
                tn.Remove();

                tvFileList.Nodes[0].Nodes.Add(tn);
            }

            tvFileList.SelectedNode = tvFileList.Nodes[0];

            // -------------------------------------------------------------------
            // The documents have been moved from the available to client's tree
            // Now it is time to save the documents
            // -------------------------------------------------------------------
            Save(clientDocumentSet, documentSetUID, tvFileList);

            ClientDocumentLink cloneLinks = new ClientDocumentLink();
            cloneLinks.ReplicateDocSetDocLinkToClient(clientDocumentSet.FKClientUID, clientDocumentSet.ClientSetID, documentSetUID);

        }

        // ----------------------------------------------------------
        //                 Save client documents
        // ----------------------------------------------------------
        private static void Save(
                ClientDocumentSet clientDocumentSet,
                int documentSetUID,
                TreeView tvFileList

            )
        {
            ClientDocument cdsl = new ClientDocument();
            ClientDocumentSet docSet = new ClientDocumentSet();

            var lodsl = new ListOfscClientDocSetDocLink();
            lodsl.list = new List<scClientDocSetDocLink>();

            // Move data into views..

            int selUID = documentSetUID;

            docSet.Get(clientDocumentSet.FKClientUID, selUID);
            docSet.ClientSetID = selUID;
            docSet.Folder = clientDocumentSet.Folder;
            docSet.SourceFolder = clientDocumentSet.SourceFolder;
            docSet.Description = clientDocumentSet.Description;
            docSet.Update();

            // Save complete tree...

            SaveTreeViewToClient(tvFileList, 0, clientDocumentSet);

        }

        // -------------------------------------------------------------------
        //                Saves TreeView of a client tree
        // -------------------------------------------------------------------
        private static void SaveTreeViewToClient(TreeView treeView, int parentID, ClientDocumentSet clientDocumentSet)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                var documentLink = (scClientDocSetDocLink)node.Tag;

                SaveTreeNodeToClient(node, documentLink.clientDocument.UID, clientDocumentSet);
            }
        }


        // -------------------------------------------------------------------
        //                Saves TreeNode of a client tree
        // -------------------------------------------------------------------
        private static TreeNode SaveTreeNodeToClient(TreeNode treeNode, int parentID, ClientDocumentSet clientDocumentSet)
        {
            TreeNode ret = new TreeNode();
            ClientDocument cdsl = new ClientDocument();

            var t = treeNode.Tag.GetType();

            // If the type is not document, it is an existing document
            //
            // var documentLink = new FCMStructures.scClientDocSetDocLink();
            var documentLink = new scClientDocSetDocLink();

            if (t.Name == "scClientDocSetDocLink")
            {
                documentLink = (scClientDocSetDocLink)treeNode.Tag;
                documentLink.clientDocument.SequenceNumber = treeNode.Index;

            }

            //
            // If the type is Document, it means a new document added to the client
            // list
            //
            if (t.Name == "Document")
            #region Document
            {
                documentLink.document = new Document.Document();
                documentLink.document = (Document.Document)treeNode.Tag;

                documentLink.clientDocument = new ClientDocument();
                documentLink.clientDocumentSet = new ClientDocumentSet();

                // Fill in the extra details...
                //

                documentLink.clientDocument.EndDate = System.DateTime.MaxValue;
                documentLink.clientDocument.FKClientDocumentSetUID = clientDocumentSet.ClientSetID; // Utils.ClientSetID;
                documentLink.clientDocument.FKClientUID = clientDocumentSet.FKClientUID; //Utils.ClientID;
                if (clientDocumentSet.FKClientUID <= 0)
                {
                    MessageBox.Show("Client ID not supplied.");
                    return null;
                }
                documentLink.clientDocument.FKDocumentUID = documentLink.document.UID;
                documentLink.clientDocument.Generated = 'N';
                documentLink.clientDocument.SourceIssueNumber = documentLink.document.IssueNumber;
                documentLink.clientDocument.ClientIssueNumber = 00;

                // When the source is client, the name will have already all the numbers
                //
                //if (documentLink.document.SourceCode == Utils.SourceCode.CLIENT)
                //{
                //    documentLink.clientDocument.ComboIssueNumber = documentLink.document.CUID;
                //}
                //else
                //{

                //}

                if (documentLink.document.RecordType == Utils.RecordType.FOLDER)
                {
                    documentLink.clientDocument.ComboIssueNumber = documentLink.document.CUID;
                    documentLink.clientDocument.FileName = documentLink.document.SimpleFileName;
                }
                else
                {
                    documentLink.clientDocument.ComboIssueNumber =
                    ClientDocument.GetComboIssueNumber(documentLink.document.CUID,
                                                       documentLink.document.IssueNumber,
                                                       clientDocumentSet.FKClientUID);

                    documentLink.clientDocument.FileName = documentLink.clientDocument.ComboIssueNumber + " " +
                                                       documentLink.document.SimpleFileName;
                }
                documentLink.clientDocument.IsProjectPlan = documentLink.document.IsProjectPlan;
                documentLink.clientDocument.DocumentCUID = documentLink.document.CUID;
                documentLink.clientDocument.DocumentType = documentLink.document.DocumentType;
                // The client document location includes the client path (%CLIENTFOLDER%) plus the client document set id
                // %CLIENTFOLDER%\CLIENTSET201000001R0001\


                // How to identify the parent folder
                //
                // documentLink.clientDocument.ParentUID = destFolder.clientDocument.UID;
                documentLink.clientDocument.ParentUID = parentID;

                //  documentLink.clientDocument.Location = txtDestinationFolder.Text +
                //                                         Utils.GetClientPathInside(documentLink.document.Location);

                documentLink.clientDocument.Location = GetClientDocumentLocation(parentID);

                documentLink.clientDocument.RecordType = documentLink.document.RecordType;
                documentLink.clientDocument.SequenceNumber = treeNode.Index;
                documentLink.clientDocument.SourceFileName = documentLink.document.FileName;
                documentLink.clientDocument.SourceLocation = documentLink.document.Location;

                documentLink.clientDocument.StartDate = System.DateTime.Today;
                documentLink.clientDocument.UID = 0;

                documentLink.clientDocumentSet.UID = clientDocumentSet.ClientSetID; // clientDocumentSet.UID; // Utils.ClientSetID;
                documentLink.clientDocumentSet.SourceFolder = clientDocumentSet.SourceFolder;
                documentLink.clientDocumentSet.ClientSetID = clientDocumentSet.ClientSetID; // Utils.ClientSetID;
                documentLink.clientDocumentSet.FKClientUID = clientDocumentSet.FKClientUID;
                documentLink.clientDocumentSet.Folder = clientDocumentSet.Folder;
            }
            #endregion Document

            // Save link to database
            //
            documentLink.clientDocument.UID = cdsl.LinkDocumentToClientSet(documentLink);

            foreach (TreeNode children in treeNode.Nodes)
            {
                SaveTreeNodeToClient(children, documentLink.clientDocument.UID, clientDocumentSet);
            }


            return ret;
        }

        /// <summary>
        /// Retrieve the parent folder for a given document
        /// </summary>
        /// <param name="parentDocumentUID"></param>
        /// <returns></returns>
        private static string GetClientDocumentLocation(int clientDocumentUID)
        {
            string ret = "";
            var rs = BUSClientDocument.ClientDocumentReadS(clientDocumentUID: clientDocumentUID);
            ClientDocument clientDocument = new ClientDocument();


            if (rs.ReturnCode == 1 && rs.ReasonCode == 1)
            {
                clientDocument = (ClientDocument)rs.Contents;

                //  This is to prevent the first level from taking an extra \\ at the front
                // it was causing the folder to be like \\%CLIENTFOLDER%\\
                // At the end the client folder was replace by a physical path
                // and it appears like "\\c:\\fcm\\document\\"

                clientDocument.Location = clientDocument.Location.Trim();

                if (string.IsNullOrEmpty(clientDocument.Location))
                    ret = clientDocument.FileName;
                else
                    ret = clientDocument.Location + "\\" + clientDocument.FileName;
            }

            if (rs.ReturnCode == 1 && rs.ReasonCode == 2)
            {
                // Client document not found
            }

            return ret;
        }

    }
}
