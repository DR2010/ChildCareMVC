using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using WordNet = Microsoft.Office.Interop.Word;
using fcm.Interfaces;


namespace FCMBusinessLibrary
{
    public class DocumentGeneration
    {
        private object vkFalse;
        private Word.Application vkWordApp;
        private Excel.Application vkExcelApp;
        private ReportMetadataList clientMetadata;
        private List<WordDocumentTasks.TagStructure> ts;
        private int clientID;
        private int clientDocSetID;
        private double fileprocessedcount;
        private double valueForProgressBar;
        private string startTime;
        private int filecount;
        private IOutputMessage uioutput;
        private string overrideDocuments;
        private ClientDocumentSet cds;
        private DateTime estimated;
        private double averageSpanInSec;
        private double acumulatedSpanInSec;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="ClientDocSetID"></param>
        public DocumentGeneration( int ClientID, int ClientDocSetID, IOutputMessage UIoutput = null, 
                                   string OverrideDocuments = null )
        {
            if (overrideDocuments == null)
                overrideDocuments = "N";

            // Assign to internal variables
            //
            // iconMessage = IconMessage;

            // Set private attributes
            clientID = ClientID;
            clientDocSetID = ClientDocSetID;
            uioutput = UIoutput;
            overrideDocuments = OverrideDocuments;

            // Instantiate Word
            //
            vkFalse = false;

            vkWordApp = new Word.Application();

            // Make it not visible
            vkWordApp.Visible = false;

            vkExcelApp = new Excel.Application();

            // Make it not visible
            vkExcelApp.Visible = false;

            // Get Metadata for client

            clientMetadata = new ReportMetadataList();

            // Daniel 31/12/2011
            //
            // clientMetadata.ListMetadataForClient(clientID);
            // The intention is to always use the full set of variables.
            // There is need to use all in order to replace the tags not used.
            //
            clientMetadata.ListDefault();

            ts = new List<WordDocumentTasks.TagStructure>();

            // Load variables/ metadata into memory
            //
            #region ClientMetadata
            foreach (ReportMetadata metadata in clientMetadata.reportMetadataList)
            {
                // Add client ID
                metadata.ClientUID = this.clientID;

                // Retrieve value for the field selected
                //
                string value =  metadata.GetValue();

                // If the field is not enabled, the program has to replace the value with spaces.
                // 01-Jan-2012 - No longer necessary.
                // All the variables have to be used

                // var valueOfTag = metadata.Enabled == 'Y' ? value : string.Empty;

                // Set to the value. If value is null, set to spaces.
                var valueOfTag = string.IsNullOrEmpty(value) ? string.Empty : value;
                
                // When the field is an image and it is not enable, do not include the "No image" icon in the list
                //
                //if (metadata.InformationType == Utils.InformationType.IMAGE && metadata.Enabled == 'N')
                //    continue;

                // If the field is an image but it has no value, no need to include.
                // Regular fields must be included because they need to be replaced.
                // Images uses bookmarks, no need to be replace. It is not displayed in the document.
                //
                if (metadata.InformationType == Utils.InformationType.IMAGE)
                {
                    if (string.IsNullOrEmpty(value))
                        continue;
                }

                // Add label before value to print.
                //
                if (metadata.UseAsLabel == 'Y')
                    valueOfTag = metadata.Description + " " + valueOfTag;

                ts.Add( new WordDocumentTasks.TagStructure()
                {
                    TagType = metadata.InformationType,
                    Tag = metadata.FieldCode,
                    TagValue = valueOfTag
                } );

            }
            #endregion ClientMetadata

            // Get Client Document Set Details 
            // To get the source and destination folders
            cds = new ClientDocumentSet();
            cds.Get( clientID, clientDocSetID );

            fileprocessedcount  = 0;
            valueForProgressBar = 0;
            startTime = System.DateTime.Now.ToString();

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ DocumentGeneration()
        { 
            
            // close word application
            if (vkWordApp != null)
            {
                try
                {
                    vkWordApp.Quit( ref vkFalse, ref vkFalse, ref vkFalse );
                }
                catch
                {
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject( vkWordApp );
                }
            }
            if (vkExcelApp != null)
            {
                try
                {
                    vkExcelApp.Quit();
                }
                catch
                { }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject( vkExcelApp );
                }
            }

        }

        /// <summary>
        /// Generate documents selected for a client
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="clientDocSetID"></param>
        /// <param name="uioutput"></param>
        /// <param name="overrideDocuments"></param>
        private void TBD_GenerateDocumentsForClient(
            int clientID, int clientDocSetID,
            string overrideDocuments )
        {


            uioutput.AddOutputMessage( "Start time: " + System.DateTime.Now.ToString() );

            // Instantiate Word
            //
            object vkFalse = false;

            Word.Application vkWordApp =
                                 new Word.Application();

            // Make it not visible
            vkWordApp.Visible = false;

            Excel.Application vkExcelApp = new Excel.Application();

            // Make it not visible
            vkExcelApp.Visible = false;

            // Get Metadata for client

            ReportMetadataList clientMetadata = new ReportMetadataList();
            clientMetadata.ListMetadataForClient( clientID );

            var ts = new List<WordDocumentTasks.TagStructure>();

            // Load variables/ metadata into memory
            //
            foreach (ReportMetadata metadata in clientMetadata.reportMetadataList)
            {
                // Retrieve value for the field selected
                //
                string value =  metadata.GetValue();

                // If the field is not enabled, the program has to replace the value with spaces.
                //
                var valueOfTag = metadata.Enabled == 'Y' ? value : string.Empty;

                // When the field is an image and it is not enable, do not include the "No image" icon in the list
                //
                if (metadata.InformationType == Utils.InformationType.IMAGE && metadata.Enabled == 'N')
                    continue;

                ts.Add( new WordDocumentTasks.TagStructure()
                {
                    TagType = metadata.InformationType,
                    Tag = metadata.FieldCode,
                    TagValue = valueOfTag
                } );

            }

            // Get Client Document Set Details 
            // To get the source and destination folders
            ClientDocumentSet cds = new ClientDocumentSet();
            cds.Get( clientID, clientDocSetID );

            // Get List of documents for a client
            //
            var cdl = new ClientDocument();
            cdl.List( Utils.ClientID, Utils.ClientSetID );


            bool fileNotFound = false;
            // ---------------------------------------------------------------------------
            //    Check if source files exist before generation starts
            // ---------------------------------------------------------------------------
            int filecount = 0;
            foreach (scClientDocSetDocLink doco in cdl.clientDocSetDocLink)
            {
                #region File Inspection 
                filecount++;

                // Ignore for now
                //
                if (doco.clientDocument.RecordType.Trim() == Utils.RecordType.FOLDER)
                {
                    string er = "Folder " + doco.document.Name;

                    uioutput.AddOutputMessage( er );
                    continue;
                }


                // Retrieve updated file name from source
                Document.Document document = new Document.Document();
                document.UID = doco.clientDocument.FKDocumentUID;
                document.Read();

                uioutput.AddOutputMessage( "Inspecting file: " + document.UID + " === " +  document.Name );

                // Client Document.SourceFileName is the name for the FCM File
                // Client Document.FileName is the client file name

                // Update client records with new file name
                //
                // Instantiate client document
                ClientDocument cd = new ClientDocument();
                cd.UID = doco.clientDocument.UID;
                // cd.FileName = document.FileName;
                cd.SourceFileName = document.FileName;
                cd.UpdateSourceFileName();

                // Update memory with latest file name
                // doco.clientDocument.SourceFileName = cd.FileName;
                doco.clientDocument.SourceFileName = cd.SourceFileName;

                string sourceFileLocationName = Utils.getFilePathName(
                       doco.clientDocument.SourceLocation,
                       doco.clientDocument.SourceFileName);

                // check if source folder/ file exists
                if (string.IsNullOrEmpty( doco.clientDocument.Location ))
                {
                    MessageBox.Show( "Document Location is empty." );
                    return;
                }

                if (string.IsNullOrEmpty( doco.clientDocument.FileName ))
                {
                    MessageBox.Show( "File Name is empty." );
                    return;
                }

                if (!File.Exists( sourceFileLocationName ))
                {
                    string er = "File does not exist " +
                        sourceFileLocationName + " - File Name: " + doco.clientDocument.SourceFileName;

                    uioutput.AddOutputMessage( er );
                    uioutput.AddErrorMessage( er );
                    fileNotFound = true;
                    continue;

                }
                #endregion File Inspection
            }


            // Can't proceed if file not found
            if (fileNotFound)
                return;

            // Check if destination folder exists
            //
            if (string.IsNullOrEmpty( cds.Folder ))
            {
                MessageBox.Show( "Destination folder not set. Generation stopped." );
                return;
            }
            string PhysicalCDSFolder = Utils.GetPathName( cds.Folder );
            if (!Directory.Exists( PhysicalCDSFolder ))
                Directory.CreateDirectory( PhysicalCDSFolder );


            // -----------------------------------------------------------------------
            //                          Generation starts here
            // -----------------------------------------------------------------------

            fileprocessedcount  = 0;
            valueForProgressBar = 0;
            startTime = System.DateTime.Now.ToString();
            estimated = System.DateTime.Now.AddSeconds( 5 * filecount);

            var previousTime = System.DateTime.Now;
            var agora = System.DateTime.Now;

            foreach (scClientDocSetDocLink doco in cdl.clientDocSetDocLink)
            {
                fileprocessedcount++;
                valueForProgressBar = (fileprocessedcount / filecount) * 100;
                
                // Get current time
                agora = System.DateTime.Now;

                // Get the time it took to process one file
                TimeSpan span = agora.Subtract( previousTime );

                // Calculate the estimated time to complete
                estimated = System.DateTime.Now.AddSeconds( span.TotalSeconds * filecount );

                uioutput.UpdateProgressBar( valueForProgressBar, estimated );

                previousTime = System.DateTime.Now;

                // Retrieve latest version
                //
                Document.Document document = new Document.Document();
                document.UID = doco.clientDocument.FKDocumentUID;
                document.Read();


                uioutput.AddOutputMessage( ">>> Generating file: " + document.UID + " === " + document.SimpleFileName );


                string sourceFileLocationName = Utils.getFilePathName(
                       doco.clientDocument.SourceLocation,
                       doco.clientDocument.SourceFileName);

                // This is the client file name
                //
                string clientFileLocation = cds.Folder.Trim() +
                    doco.clientDocument.Location.Trim();

                string clientFileLocationName = Utils.getFilePathName(
                    clientFileLocation,
                    doco.clientDocument.FileName.Trim() );



                // Check if file destination directory exists
                //
                string PhysicalLocation = Utils.GetPathName( clientFileLocation );

                if (string.IsNullOrEmpty( PhysicalLocation ))
                {
                    string er = "Location is empty " + doco.clientDocument.Location + "\n" +
                                "File Name: " + doco.document.Name;

                    uioutput.AddOutputMessage( er );
                    continue;
                }

                if (!Directory.Exists( PhysicalLocation ))
                    Directory.CreateDirectory( PhysicalLocation );

                if (File.Exists( clientFileLocationName ))
                {
                    // Proceed but report in list
                    //
                    if (overrideDocuments == "Yes")
                    {
                        // Delete file
                        try
                        {
                            File.Delete( clientFileLocationName );
                            uioutput.AddOutputMessage( "File replaced " +
                                              document.SimpleFileName );
                        }
                        catch (Exception)
                        {
                            uioutput.AddOutputMessage( "Error deleting file " +
                                              document.SimpleFileName );
                            uioutput.AddErrorMessage( "Error deleting file " +
                                              document.SimpleFileName );

                            continue;
                        }
                    }
                    else
                    {
                        uioutput.AddOutputMessage( "File already exists " +
                                          document.SimpleFileName );
                        continue;
                    }
                }

                // Copy and fix file
                //

                // Word Documents
                //
                if (doco.clientDocument.RecordType.Trim() == Utils.RecordType.FOLDER)
                {
                    // Update file - set as GENERATED.
                    //

                    uioutput.AddOutputMessage( "FOLDER: " + doco.clientDocument.SourceFileName );
                }
                else
                {

                    // If is is not a folder, it must be a regular file.
                    // Trying to copy it as well...
                    //

                    var currentDocumentPath = Path.GetExtension( doco.clientDocument.FileName );

                    if (doco.clientDocument.DocumentType == Utils.DocumentType.WORD)
                    {
                        #region Word
                        // ------------------------------------------------------------------------
                        // ------------------------------------------------------------------------
                        // Generate Document and replace tag values in new document generated
                        // ------------------------------------------------------------------------
                        // ------------------------------------------------------------------------
                        var results = WordDocumentTasks.CopyDocument( sourceFileLocationName, clientFileLocationName, ts, vkWordApp, uioutput );
                        if (results.ReturnCode < 0)
                        {
                            // Error has occurred
                            //
                            var er = (System.Exception)results.Contents;
                            uioutput.AddOutputMessage( "ERROR: " + er.ToString() );
                            uioutput.AddErrorMessage( "ERROR: " + er.ToString() );

                            continue;
                        }


                        //
                        // Instantiate client document
                        ClientDocument cd = new ClientDocument();
                        cd.UID = doco.clientDocument.UID;


                        // Update file - set as GENERATED.
                        //

                        cd.SetGeneratedFlagVersion( 'Y', document.IssueNumber );

                        uioutput.AddOutputMessage( "Document generated: " +
                                          clientFileLocationName );

                        #endregion Word
                    }
                    else if (doco.clientDocument.DocumentType == Utils.DocumentType.EXCEL)
                    {
                        // ------------------------------------------------------------------------
                        // ------------------------------------------------------------------------
                        // Generate Document and replace tag values in new document generated
                        // ------------------------------------------------------------------------
                        // ------------------------------------------------------------------------

                        ExcelSpreadsheetTasks.CopyDocument( sourceFileLocationName, clientFileLocationName, ts, vkExcelApp, uioutput );

                        //
                        // Instantiate client document
                        ClientDocument cd = new ClientDocument();
                        cd.UID = doco.clientDocument.UID;


                        // Update file - set as GENERATED.
                        //

                        cd.SetGeneratedFlagVersion( 'Y', document.IssueNumber );

                        uioutput.AddOutputMessage( "Document generated: " +
                                          clientFileLocationName );

                    }
                    else
                    {
                        File.Copy( sourceFileLocationName, clientFileLocationName );

                        uioutput.AddOutputMessage( "File copied but not modified: " +
                                 Path.GetExtension( doco.clientDocument.FileName ) + " == File: " + clientFileLocationName );
                    }

                }
            }

            // close word application
            vkWordApp.Quit( ref vkFalse, ref vkFalse, ref vkFalse );
            vkExcelApp.Quit();

            uioutput.AddOutputMessage( "End time: " + System.DateTime.Now.ToString() );

        }


        /// <summary>
        /// Generate document for client (no treeview required)
        /// </summary>
        public void GenerateDocumentForClient()
        {

            // Load documents in tree 
            //
            TreeView tvFileList = new TreeView();

            // List client document list
            //
            var documentSetList = new ClientDocument();
            documentSetList.List(this.clientID, this.clientDocSetID);

            tvFileList.Nodes.Clear();
            documentSetList.ListInTree(tvFileList, "CLIENT");

            // Generate document
            //
            this.GenerateDocumentsForClient(tvFileList.Nodes[0]);
        }


        /// <summary>
        /// Generate documents based on tree
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="clientDocSetID"></param>
        /// <param name="uioutput"></param>
        /// <param name="overrideDocuments"></param>
        /// <param name="documentsTreeNode"></param>
        public void GenerateDocumentsForClient(TreeNode documentsTreeNode)       
        {

            if (uioutput != null) uioutput.AddOutputMessage( "Start time: " + System.DateTime.Now.ToString() );

            filecount = documentsTreeNode.GetNodeCount(includeSubTrees: true);

            estimated = System.DateTime.Now.AddSeconds( 5 * filecount );

            if (uioutput != null) uioutput.UpdateProgressBar( valueForProgressBar, estimated );

            GenerateDocumentsController(documentsTreeNode);

            if (uioutput != null) uioutput.AddOutputMessage( "End time: " + System.DateTime.Now.ToString() );

            // Set end bar
            if (uioutput != null) uioutput.UpdateProgressBar(100, estimated);


        }


        /// <summary>
        /// This operation updates the destination folder following the
        /// hierarchy of the tree instead of the initial location.
        /// </summary>
        /// <param name="documentsTreeNode"></param>
        public static ResponseStatus UpdateDestinationFolder(int clientID, int clientDocumentSetUID)
        {
            ResponseStatus response = new ResponseStatus();
            response.Contents = "Destination folder updated successfully.";

            var listOfDocuments = ClientDocument.ListS(clientID, clientDocumentSetUID);

            foreach (var doco in listOfDocuments)
            {
                // Update location
                //
                ResponseStatus destLocationDerivedClient = ClientDocument.GetDocumentPath(doco.clientDocument);
                string destLocationDerived = destLocationDerivedClient.Contents.ToString();

                ClientDocument.UpdateFieldString(doco.clientDocument.UID, "Location", destLocationDerived);

            }
            return new ResponseStatus();
        }

        /// <summary>
        /// Generate Documents Controller (Word must be previously initialised
        /// </summary>
        private void GenerateDocumentsController(TreeNode documentsTreeNode)
        {
            scClientDocSetDocLink documentTN = new scClientDocSetDocLink();

            // Get List of documents for a client
            //
            if (documentsTreeNode.Tag.GetType().Name == "scClientDocSetDocLink")
            {
                documentTN = (scClientDocSetDocLink)documentsTreeNode.Tag;
            }
            else
            {
                if (documentsTreeNode.Tag.GetType().Name == "Document")
                {
                    if (documentTN.clientDocument == null)
                    {
                        if (uioutput != null) uioutput.AddOutputMessage("Error CDISNULL019202 - client document is null.");
                        if (uioutput != null) uioutput.AddErrorMessage("Error CDISNULL019202 - client document is null. Generation stopped.");
                        return;
                    }

                    documentTN.clientDocument.RecordType = Utils.RecordType.DOCUMENT;
                    documentTN.document = (Document.Document)documentsTreeNode.Tag;
                }
            }

            // If it is a document, generate
            if (documentTN.clientDocument.RecordType.Trim() != Utils.RecordType.FOLDER)
            {
                #region Generate Document
                // Generate

                // Retrieve updated file name from source
                Document.Document document = new Document.Document();
                document.UID = documentTN.clientDocument.FKDocumentUID;
                document.Read();

                if (uioutput != null) uioutput.AddOutputMessage( "Inspecting file: " + document.UID + " === " + document.Name );
                // if (iconMessage != null) iconMessage.Text = "Start time: " + System.DateTime.Now.ToString();

                // Update client records with new file name
                //
                // Instantiate client document
                ClientDocument cd = new ClientDocument();
                cd.UID = documentTN.clientDocument.UID;
                cd.SourceFileName = document.FileName;
                // cd.Read();

                // Set comboIssueNumber and File Name
                //
                ClientDocument.SetClientDestinationFile(
                            clientDocument: cd,
                            clientUID: documentTN.clientDocument.FKClientUID,
                            documentCUID: document.CUID,
                            sourceFileName: document.FileName,
                            sourceVersionNumber: document.IssueNumber,
                            simpleFileName: document.SimpleFileName);

                
                cd.UpdateSourceFileName();

                // Update memory with latest file name
                documentTN.clientDocument.SourceFileName = cd.SourceFileName;

                string sourceFileLocationName = Utils.getFilePathName(
                       documentTN.clientDocument.SourceLocation,
                       documentTN.clientDocument.SourceFileName );

                // check if source folder/ file exists
                if (string.IsNullOrEmpty( documentTN.clientDocument.Location ))
                {
                    if (uioutput != null) uioutput.AddOutputMessage( "Document Location is empty." );
                    return;
                }

                if (string.IsNullOrEmpty( documentTN.clientDocument.FileName ))
                {
                    if (uioutput != null) uioutput.AddOutputMessage( "File Name is empty." );
                    return;
                }

                if (!File.Exists( sourceFileLocationName ))
                {
                    string er = "File does not exist " +
                        sourceFileLocationName + " - File Name: " + documentTN.clientDocument.SourceFileName;

                    if (uioutput != null) uioutput.AddOutputMessage( er );
                    if (uioutput != null) uioutput.AddErrorMessage( er );
                    return;

                }


                // Check if destination folder exists
                //
                if (string.IsNullOrEmpty( cds.Folder ))
                {
                    string er = "Destination folder not set. Generation stopped.";
                    if (uioutput != null) uioutput.AddOutputMessage( er );
                    return;
                }
                string PhysicalCDSFolder = Utils.GetPathName( cds.Folder );
                if (!Directory.Exists( PhysicalCDSFolder ))
                    Directory.CreateDirectory( PhysicalCDSFolder );

                // Generate one document (Folder will also be created in this call)
                //

                // Get time before file
                var previousTime = System.DateTime.Now;

                GenerateDocument( documentTN );
                
                // Get time after file
                var agora = System.DateTime.Now;

                fileprocessedcount++;
                valueForProgressBar = (fileprocessedcount / filecount) * 100;
                int leftToProcess = filecount - Convert.ToInt32( fileprocessedcount );

                // Get the time it took to process one file
                TimeSpan span = agora.Subtract( previousTime );

                // Average span in seconds
                acumulatedSpanInSec += span.Seconds;
                averageSpanInSec = acumulatedSpanInSec / fileprocessedcount;

                // Calculate the estimated time to complete
                estimated = System.DateTime.Now.AddSeconds( averageSpanInSec * leftToProcess );

                if (uioutput != null) uioutput.UpdateProgressBar( valueForProgressBar, estimated, leftToProcess );

                return;
                #endregion Generate Document

                // Processing for one file ends here
            }
            else
            {
                // If item imported is a FOLDER

                // This is the client destination folder (and name)
                //
                string clientDestinationFileLocation = documentTN.clientDocument.Location.Trim();

                if (!string.IsNullOrEmpty(clientDestinationFileLocation))
                {

                    string clientDestinationFileLocationName = Utils.getFilePathName(
                    clientDestinationFileLocation, documentTN.clientDocument.FileName.Trim());

                    // string PhysicalLocation = Utils.GetPathName(clientDestinationFileLocation);
                    string PhysicalLocation = clientDestinationFileLocationName;

                    if (uioutput != null) uioutput.AddOutputMessage("Processing Folder: " + PhysicalLocation);

                    if (!string.IsNullOrEmpty(PhysicalLocation))
                    {
                        if (!Directory.Exists(PhysicalLocation))
                        {
                            if (uioutput != null) uioutput.AddOutputMessage("Folder Created: " + clientDestinationFileLocationName);

                            Directory.CreateDirectory(PhysicalLocation);
                        }
                        else
                        {
                            if (uioutput != null) uioutput.AddOutputMessage("Folder Already Exists: " + clientDestinationFileLocationName);
                        }
                    }
                }
                else
                {
                    if (uioutput != null) uioutput.AddOutputMessage("Folder Ignored: " + documentTN.document.Name);
                }

                // Process each document in folder
                //
                foreach (TreeNode tn in documentsTreeNode.Nodes)
                {
                    scClientDocSetDocLink doco = (scClientDocSetDocLink)tn.Tag;

                    GenerateDocumentsController( tn );
                }
            }
        }



        /// <summary>
        /// Generate one document
        /// </summary>
        /// <param name="doco"></param>
        public void GenerateDocument( scClientDocSetDocLink doco )
        {

            // Retrieve latest version
            //
            Document.Document document = new Document.Document();
            document.UID = doco.clientDocument.FKDocumentUID;
            document.Read();

            string msg = ">>> Generating file: " + document.UID + " === " + document.SimpleFileName;
            if (uioutput != null) uioutput.AddOutputMessage( msg );
            // if (iconMessage != null) iconMessage.Text = ">>> Generating file: " + document.UID;


            // Locate source file
            //
            string sourceFileLocationName = Utils.getFilePathName(
                   doco.clientDocument.SourceLocation,
                   doco.clientDocument.SourceFileName );

            // Find the parent folder location
            //
            int parentUID = doco.clientDocument.ParentUID;
            ClientDocument cdParent = new ClientDocument();
            cdParent.UID = parentUID;
            cdParent.Read();

            // This is the client destination folder (and name)
            //
            //ResponseStatus destLocationDerivedClient = ClientDocument.GetDocumentPath(doco.clientDocument);
            //string destLocationDerived = destLocationDerivedClient.Contents.ToString();

            string clientDestinationFileLocation = doco.clientDocument.Location.Trim();

            string clientDestinationFileLocationName = Utils.getFilePathName(
                clientDestinationFileLocation, doco.clientDocument.FileName.Trim());

            // This is the source client file name
            //
            string clientSourceFileLocation = doco.clientDocument.Location.Trim();

            string clientSourceFileLocationName = Utils.getFilePathName(
                clientSourceFileLocation,
                doco.clientDocument.FileName.Trim() );

            // Source location and destination may be different.
            // The destination of the file must be the one where it lives on the actual tree
            // To determine where the file should be located, we need to get the parent folder
            // The only way to determine the parent folder is walking through the entire tree from the root.
            // The root is the only thing that can be trusted. Every other location is dependent on the root.

            // The determination of the file location occurs in 2 parts. The one here is the second part.
            // At this point we are not going to use the source location of the original document, instead
            // we are going to use the client document location
            //

            // Check if destination folder directory exists
            //
            string PhysicalLocation = Utils.GetPathName(clientDestinationFileLocation);

            if (string.IsNullOrEmpty( PhysicalLocation ))
            {
                string er = "Location is empty " + clientDestinationFileLocation + "\n" +
                            "File Name: " + doco.document.Name;

                uioutput.AddOutputMessage( er );
                return;
            }


            // 02/04/2011
            // This step should be done when the "FOLDER" record is created and not when the document
            // is generated
            //

            //if (!Directory.Exists( PhysicalLocation ))
            //    Directory.CreateDirectory( PhysicalLocation );


            // However the folder existence must be checked
            //
            if (!Directory.Exists(PhysicalLocation))
            {
                Directory.CreateDirectory(PhysicalLocation);

                string er = "Destination folder has been created with File! " + PhysicalLocation + "\n" +
                "File Name: " + doco.document.Name;

                uioutput.AddOutputMessage(er);
                //return;
            }

            if (File.Exists( clientDestinationFileLocationName ))
            {
                // Proceed but report in list
                //
                if (overrideDocuments == "Yes")
                {
                    // Delete file
                    try
                    {
                        File.Delete(clientDestinationFileLocationName);
                        uioutput.AddOutputMessage( "File replaced: " +
                                          document.SimpleFileName );
                    }
                    catch (Exception)
                    {
                        uioutput.AddOutputMessage( "Error deleting file " +
                                          document.SimpleFileName );
                        uioutput.AddErrorMessage( "Error deleting file " +
                                          document.SimpleFileName );

                        return;
                    }
                }
                else
                {
                    uioutput.AddOutputMessage( "File already exists " +
                                      document.SimpleFileName );
                    return; 
                }
            }

            // Copy and fix file
            //

            // Word Documents
            //
            if (doco.clientDocument.RecordType.Trim() == Utils.RecordType.FOLDER)
            {
                // Update file - set as GENERATED.
                //

                // This is the moment where the folder destination has to be created
                // and the folder db record has to be updated with the location
                //

                if (!Directory.Exists(PhysicalLocation))
                    Directory.CreateDirectory(PhysicalLocation);

                uioutput.AddOutputMessage( "FOLDER: " + doco.clientDocument.SourceFileName );
            }
            else
            {

                // If is is not a folder, it must be a regular file.
                // Trying to copy it as well...
                //

                var currentDocumentPath = Path.GetExtension( doco.clientDocument.FileName );

                if (doco.clientDocument.DocumentType == Utils.DocumentType.WORD)
                {
                    #region Word
                    // ------------------------------------------------------------------------
                    // ------------------------------------------------------------------------
                    // Generate Document and replace tag values in new document generated
                    // ------------------------------------------------------------------------
                    // ------------------------------------------------------------------------
                    var results = WordDocumentTasks.CopyDocument(sourceFileLocationName, clientSourceFileLocationName, ts, vkWordApp, uioutput);
                    if (results.ReturnCode < 0)
                    {
                        // Error has occurred
                        //
                        var er = (System.Exception)results.Contents;
                        uioutput.AddOutputMessage( "ERROR: " + er.ToString() );
                        uioutput.AddErrorMessage( "ERROR: " + er.ToString() );

                        return;
                    }


                    #endregion Word
                }
                else if (doco.clientDocument.DocumentType == Utils.DocumentType.EXCEL)
                {
                    // ------------------------------------------------------------------------
                    // ------------------------------------------------------------------------
                    // Generate Document and replace tag values in new document generated
                    // ------------------------------------------------------------------------
                    // ------------------------------------------------------------------------

                    ExcelSpreadsheetTasks.CopyDocument(sourceFileLocationName, clientSourceFileLocationName, ts, vkExcelApp, uioutput);

                }
                else
                {
                    File.Copy(sourceFileLocationName, clientSourceFileLocationName);

                    uioutput.AddOutputMessage( "File copied but not modified: " +
                             Path.GetExtension(doco.clientDocument.FileName) + " == File: " + clientSourceFileLocationName);
                
                }

                //
                // Instantiate client document
                ClientDocument cd = new ClientDocument();
                cd.UID = doco.clientDocument.UID;

                // Update file - set as GENERATED.
                //
                cd.SetGeneratedFlagVersion( 'Y', document.IssueNumber );

                uioutput.AddOutputMessage( "Document generated: " +
                                  clientDestinationFileLocationName);
            }
            return;
        }

    }
}
