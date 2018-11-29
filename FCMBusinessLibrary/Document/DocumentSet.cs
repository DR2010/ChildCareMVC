using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FCMBusinessLibrary.Document
{
    public class DocumentSet
    {

        public int UID { get; set; }
        public string Name { get; set; }
        public string TemplateType { get; set; }
        public string TemplateFolder { get; set; }
        public char IsVoid { get; set; }
        public DocumentList documentList { get; set; }
        public string UIDNameDisplay { get; set; }

        // -----------------------------------------------------
        //    Load document into document set
        // -----------------------------------------------------
        public void LoadAllDocuments()
        {

            // Retrieve all documents
            // For each document (order by parent uid)
                // check if it is already connected to current Document Set
                // If it is not, connect document
                // Link with parent document in the set
            // Replicate Document Links

            DocumentList dl = new DocumentList();
            dl.List();

            foreach (Document document in dl.documentList)
            {
                var found = DocumentSet.FindDocumentInSet(this.UID, document.UID);

                if (found.document.UID > 0)
                    continue;
                else
                {
                    DocumentSetDocument dsl = new DocumentSetDocument();

                    // Generate new UID
                    dsl.UID = this.GetLastUID() + 1;
                  
                    // Add document to set
                    //
                    dsl.FKDocumentSetUID = this.UID;
                    dsl.FKDocumentUID = document.UID;
                    dsl.Location = document.Location;
                    dsl.IsVoid = 'N';
                    dsl.StartDate = System.DateTime.Today;
                    dsl.EndDate = System.DateTime.MaxValue;
                    dsl.FKParentDocumentUID = document.ParentUID; // Uses the Document UID as the source (Has to be combined with Doc Set)
                    dsl.FKParentDocumentSetUID = dsl.FKDocumentSetUID; 
                    dsl.SequenceNumber = document.SequenceNumber;
                    
                    dsl.Add();

                }
            }

            // Replicate document links
            //
            foreach (Document document in dl.documentList)
            {
                var children = DocumentLinkList.ListRelatedDocuments(document.UID);

                foreach (var child in children.documentLinkList)
                {
                    // 
                    DocumentSetDocumentLink dsdl = new DocumentSetDocumentLink();
                    dsdl.FKParentDocumentUID = 0;
                    dsdl.FKChildDocumentUID = 0;
                    dsdl.IsVoid = 'N';
                    dsdl.LinkType = child.LinkType;
                    dsdl.UID = GetLastUID() + 1;

                    // Find parent

                    var parent1 = DocumentSet.FindDocumentInSet(this.UID, child.FKParentDocumentUID);

                    // Find child
                    var child1 = DocumentSet.FindDocumentInSet(this.UID, child.FKChildDocumentUID);

                    dsdl.FKParentDocumentUID = parent1.DocumentSetDocument.UID;
                    dsdl.FKChildDocumentUID = child1.DocumentSetDocument.UID;

                    dsdl.Add();

                }
            }

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
                    "SELECT MAX([UID]) LASTUID FROM [DocumentSet]";

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
        //    Get Document Details
        // -----------------------------------------------------
        public static scDocoSetDocumentLink FindDocumentInSet(int documentSetUID, int documentUID)
        {
            // 
            // EA SQL database
            // 
            scDocoSetDocumentLink ret = new scDocoSetDocumentLink();
            ret.document = new Document();
            ret.documentSet = new DocumentSet();
            ret.DocumentSetDocument = new DocumentSetDocument();

            string commandString = "";

            commandString = string.Format(
                " SELECT " +
                "       [Document].[UID] DocumentUID" +
                "      ,[Document].[CUID] DocumentCUID " +
                "      ,[Document].[Name] DocumentName " +
                "      ,[Document].[SequenceNumber] DocumentSequenceNumber " +
                "      ,[Document].[IssueNumber] DocumentIssueNumber " +
                "      ,[Document].[Location] DocumentLocation "+
                "      ,[Document].[Comments] DocumentComments" +
                "      ,[Document].[UID] DocumentUID" +
                "      ,[Document].[FileName] DocumentFileName" +
                "      ,[Document].[SourceCode] DocumentSourceCode" +
                "      ,[Document].[FKClientUID] DocumentFKClientUID" +
                "      ,[Document].[ParentUID] DocumentParentUID" +
                "      ,[Document].[RecordType] DocumentRecordType" +
                "      ,[Document].[IsProjectPlan] DocumentIsProjectPlan" +
                "      ,[Document].[DocumentType] DocumentDocumentType" +
                "      ,[DocSetDoc].[UID] DocSetDocUID" +
                "      ,[DocSetDoc].[FKDocumentUID] DocSetDocFKDocumentUID" +
                "      ,[DocSetDoc].[FKDocumentSetUID] DocSetDocFKDocumentSetUID" +
                "      ,[DocSetDoc].[Location] DocSetDocLocation" +
                "      ,[DocSetDoc].[IsVoid] DocSetDocIsVoid" +
                "      ,[DocSetDoc].[StartDate] DocSetDocStartDate" +
                "      ,[DocSetDoc].[EndDate] DocSetDocEndDate" +
                "      ,[DocSetDoc].[FKParentDocumentUID] DocSetDocFKParentDocumentUID" +
                "      ,[DocSetDoc].[FKParentDocumentSetUID] DocSetDocFKParentDocumentSetUID" +
                "      ,[DocSet].[UID] SetUID" +
                "      ,[DocSet].[TemplateType] SetTemplateType" +
                "      ,[DocSet].[TemplateFolder] SetTemplateFolder" +
                "  FROM  [Document] Document" +
                "       ,[DocumentSetDocument] DocSetDoc " +
                "       ,[DocumentSet] DocSet " +
                " WHERE " +
                "        [Document].[UID] = [DocSetDoc].[FKDocumentUID] "+
                "    AND [DocSetDoc].[FKDocumentSetUID] = [DocSet].[UID] " +
                "    AND [Document].[UID] = {0} " +
                "    AND [DocSetDoc].[FKDocumentSetUID] = {1}",
                documentUID,
                documentSetUID);

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            // Document
                            //
                            Document.LoadDocumentFromReader(ret.document, "", reader);
                            //ret.document.UID = Convert.ToInt32(reader["DocumentUID"].ToString());
                            //ret.document.CUID = reader["DocumentCUID"].ToString();
                            //ret.document.Name = reader["DocumentName"].ToString();
                            //ret.document.SequenceNumber = Convert.ToInt32(reader["DocumentSequenceNumber"].ToString());
                            //ret.document.IssueNumber = Convert.ToInt32(reader["DocumentIssueNumber"].ToString());
                            //ret.document.Location = reader["DocumentLocation"].ToString();
                            //ret.document.Comments = reader["DocumentComments"].ToString();
                            //ret.document.FileName = reader["DocumentFileName"].ToString();
                            //ret.document.SourceCode = reader["DocumentSourceCode"].ToString();
                            //ret.document.FKClientUID = Convert.ToInt32(reader["DocumentFKClientUID"].ToString());
                            //ret.document.ParentUID = Convert.ToInt32(reader["DocumentParentUID"].ToString());
                            //ret.document.RecordType = reader["DocumentRecordType"].ToString();
                            //ret.document.IsProjectPlan = Convert.ToChar(reader["DocumentIsProjectPlan"]);
                            //ret.document.DocumentType = reader["DocumentDocumentType"].ToString();

                            // Document Set
                            //
                            ret.documentSet.UID = Convert.ToInt32(reader["SetUID"].ToString());
                            ret.documentSet.TemplateType = reader["SetTemplateType"].ToString();
                            ret.documentSet.TemplateFolder = reader["SetTemplateFolder"].ToString();
                            ret.documentSet.UIDNameDisplay = ret.documentSet.UID.ToString() +
                                                             "; " + ret.documentSet.TemplateType;

                            // DocumentSetDocument
                            //
                            ret.DocumentSetDocument.UID = Convert.ToInt32(reader["DocSetDocUID"].ToString());
                            ret.DocumentSetDocument.FKDocumentUID = Convert.ToInt32(reader["DocSetDocFKDocumentUID"].ToString());
                            ret.DocumentSetDocument.FKDocumentSetUID = Convert.ToInt32(reader["DocSetDocFKDocumentSetUID"].ToString());
                            ret.DocumentSetDocument.Location = reader["DocSetDocLocation"].ToString();
                            ret.DocumentSetDocument.IsVoid = Convert.ToChar(reader["DocSetDocIsVoid"].ToString());
                            ret.DocumentSetDocument.StartDate = Convert.ToDateTime(reader["DocSetDocStartDate"].ToString());
                            ret.DocumentSetDocument.EndDate = Convert.ToDateTime(reader["DocSetDocEndDate"].ToString());
                            ret.DocumentSetDocument.FKParentDocumentUID = Convert.ToInt32(reader["DocSetDocFKParentDocumentUID"].ToString());
                            ret.DocumentSetDocument.FKParentDocumentSetUID = Convert.ToInt32(reader["DocSetDocFKParentDocumentSetUID"].ToString());

                        }
                        catch 
                        {
                        }
                    }
                }
            }
            return ret;
        }

        // -----------------------------------------------------
        //    Get Document Set Details
        // -----------------------------------------------------
        public bool Read(char IncludeDocuments)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;
            string commandString = "";

            commandString = string.Format(
                " SELECT [UID] " +
                "      ,[TemplateType] " +
                "      ,[TemplateFolder] " +
                "      ,[IsVoid] " +
                "  FROM [DocumentSet] " +
                " WHERE UID = {0} ",
                UID);

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

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
                            this.TemplateType = reader["TemplateType"].ToString();
                            this.TemplateFolder = reader["TemplateFolder"].ToString();
                            this.UIDNameDisplay = this.UID + "; " + this.TemplateType;
                            this.IsVoid = Convert.ToChar(reader["IsVoid"].ToString());
                            ret = true;
                        }
                        catch 
                        {
                        }
                    }
                }

                if (IncludeDocuments == 'Y')
                {
                    this.documentList = new DocumentList();
                    this.documentList.ListDocSet(this.UID);
                    
                }
            }
            return ret;
        }

        // -----------------------------------------------------
        //    Delete Document/ Folder node
        // -----------------------------------------------------
        public static void DeleteDocumentTreeNode(int documentSetUID, TreeNode documentSetNode)
        {

            if (documentSetNode == null)
                return;

            if (documentSetUID <= 0)
                return;

            foreach (TreeNode documentAsNode in documentSetNode.Nodes)
            {
                Document doc = (Document)documentAsNode.Tag;

                DocumentSetDocument.Delete(DocumentSetUID: documentSetUID, DocumentUID: doc.UID);

                if (documentAsNode.Nodes.Count > 0)
                {
                    foreach (TreeNode tn in documentAsNode.Nodes)
                    {
                        DeleteDocumentTreeNode(documentSetUID: documentSetUID, documentSetNode: tn);
                    }
                }
            }

            Document doc2 = (Document)documentSetNode.Tag;
            DocumentSetDocument.Delete(DocumentSetUID: documentSetUID, DocumentUID: doc2.UID);

            return;
        }

        // -----------------------------------------------------
        //    Add new Document Set
        // -----------------------------------------------------
        public int Add()
        {
            int _uid = 0;

            _uid = GetLastUID() + 1;

            this.UID = _uid;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [DocumentSet]" +
                   "( " +
                   " [UID] " +
                   ",[TemplateType] " +
                   ",[TemplateFolder]" +
                   ",[IsVoid]" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @UID    " +
                   ", @TemplateType    " +
                   ", @TemplateFolder " +
                   ", @IsVoid " +
                   " ) "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                    command.Parameters.Add("@TemplateType", SqlDbType.VarChar).Value = TemplateType;
                    command.Parameters.Add("@TemplateFolder", SqlDbType.VarChar).Value = TemplateFolder;
                    command.Parameters.Add("@IsVoid", SqlDbType.VarChar).Value = 'N';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return _uid;
        }


        // -----------------------------------------------------
        //    Add new Document Set
        // -----------------------------------------------------
        public void Update()
        {

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                     "UPDATE [DocumentSet] " +
                     " SET " +
                     " [TemplateType] =  @TemplateType " +
                     " WHERE [UID] = @UID "
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = this.UID;
                    command.Parameters.Add("@TemplateType", SqlDbType.VarChar).Value = this.TemplateType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

    }
}
