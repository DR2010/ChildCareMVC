using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using WordNet = Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using System.IO;
using fcm.Interfaces;
using FCMBusinessLibrary.Business;

namespace FCMBusinessLibrary
{

    public class WordReport
    {
        object oEndOfDoc = "\\endofdoc";
        private int clientID;
        private int clientDocSetID;
        private object vkFalse;
        private IOutputMessage uioutput;
        private Word.Application vkWordApp;
        private Excel.Application vkExcelApp;
        private ReportMetadataList clientMetadata;
        private List<WordDocumentTasks.TagStructure> ts;
        private ClientDocumentSet cds;
        private double valueForProgressBar;
        private string startTime;
        public string FileName;
        public string FullFileNamePath;

        int row;

        public WordReport(int ClientID, int ClientDocSetID, IOutputMessage UIoutput = null,
                                   string OverrideDocuments = null)
        {

            row = 1;

            // Set private attributes
            clientID = ClientID;
            clientDocSetID = ClientDocSetID;
            uioutput = UIoutput;

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
            clientMetadata.ListMetadataForClient(clientID);

            ts = new List<WordDocumentTasks.TagStructure>();

            // Load variables/ metadata into memory
            //
            #region ClientMetadata
            foreach (ReportMetadata metadata in clientMetadata.reportMetadataList)
            {
                // Retrieve value for the field selected
                //
                string value = metadata.GetValue();

                // If the field is not enabled, the program has to replace the value with spaces.
                //
                var valueOfTag = metadata.Enabled == 'Y' ? value : string.Empty;

                // When the field is an image and it is not enable, do not include the "No image" icon in the list
                //
                if (metadata.InformationType == Utils.InformationType.IMAGE && metadata.Enabled == 'N')
                    continue;

                ts.Add(new WordDocumentTasks.TagStructure()
                {
                    TagType = metadata.InformationType,
                    Tag = metadata.FieldCode,
                    TagValue = valueOfTag
                });

            }
            #endregion ClientMetadata

            // Get Client Document Set Details 
            // To get the source and destination folders
            cds = new ClientDocumentSet();
            cds.Get(clientID, clientDocSetID);

            valueForProgressBar = 0;
            startTime = System.DateTime.Now.ToString();


        }

        /// <summary>
        /// Generate word document with register of system documents
        /// </summary>
        /// <param name="tv"></param>
        public string RegisterOfSytemDocuments(TreeView tv, string clientFolder)
        {
            object oMissing = System.Reflection.Missing.Value;
            var pastPlannedActivities = string.Empty;

            //Start Word and create a new document.
            WordNet._Application oWord = new Application { Visible = false };
            WordNet._Document oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
                                                 ref oMissing, ref oMissing);

            oDoc.PageSetup.Orientation = WordNet.WdOrientation.wdOrientLandscape;

            PrintToWord(oDoc, "Register of System Documents", 16, 0, FCMWordAlign.CENTER);
            PrintToWord(oDoc, " ", 8, 0);

            // Locate client folder
            //
            string clientFileLocationName = Utils.getFilePathName(@"%TEMPLATEFOLDER%\ClientSource\", "ROS-001 Register Of System Documents.doc");
            FullFileNamePath = clientFileLocationName;
            FileName = "RegisterOfSystemDocuments.doc";

            if (File.Exists( clientFileLocationName ))
            {
                // Delete file
                try
                {
                    File.Delete( clientFileLocationName );
                    uioutput.AddOutputMessage("File replaced: " + clientFileLocationName);
                }
                catch (Exception)
                {
                    uioutput.AddOutputMessage("Error deleting file " + clientFileLocationName);
                    uioutput.AddErrorMessage("Error deleting file " + clientFileLocationName);
                    return clientFileLocationName;
                }
            }


            // string filename = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetTempFileName(), "doc"));
            oDoc.SaveAs(clientFileLocationName);

            string msg = ">>> Generating file... ";
            if (uioutput != null) uioutput.AddOutputMessage(msg);

            PrintToWord(oDoc, " ", 8, 1);

            WordNet.Range wrdRng;
            WordNet.Table oTable;

            wrdRng = oDoc.Bookmarks.get_Item(oEndOfDoc).Range;
            int rowCount = 30;

            // Get number of rows for a client document, client document set
            //
            var cds = new BUSClientDocumentSet(Utils.ClientID, Utils.ClientSetID);
            rowCount = cds.DocumentCount;

            if (rowCount < 1)
                return clientFileLocationName;

            oTable = oDoc.Tables.Add(wrdRng, rowCount, 6, ref vkFalse, ref vkFalse);

            //oTable.Borders.InsideLineWidth = WordNet.WdLineWidth.wdLineWidth050pt;
            //oTable.Borders.InsideColor = WordNet.WdColor.wdColorAutomatic;
            //oTable.Borders.OutsideLineWidth = WordNet.WdLineWidth.wdLineWidth050pt;
            //oTable.Borders.OutsideColor = WordNet.WdColor.wdColorAutomatic;

            oTable.Rows[1].HeadingFormat = -1;

            WordNet.Row headingRow = oTable.Rows[1];

            ApplyHeadingStyle(headingRow.Cells[1],40);
            headingRow.Cells[1].Range.Text = "";

            ApplyHeadingStyle(headingRow.Cells[2],30 );
            headingRow.Cells[2].Range.Text = "";

            ApplyHeadingStyle(headingRow.Cells[3],60);
            headingRow.Cells[3].Range.Text = "Document Number";

            ApplyHeadingStyle(headingRow.Cells[4],50);
            headingRow.Cells[4].Range.Text = "Version";

            ApplyHeadingStyle(headingRow.Cells[5],300);
            headingRow.Cells[5].Range.Text = "Document Name";

            ApplyHeadingStyle(headingRow.Cells[6],100);
            headingRow.Cells[6].Range.Text = "Comments";

            int line = 0;
            foreach (var treeNode in tv.Nodes)
            {
                line++;
                WriteLineToRoSD(tv.Nodes[0], oDoc, oTable,prefix: "",parent:"",seqnum:line);
            }


            msg = ">>> End ";
            if (uioutput != null) uioutput.AddOutputMessage(msg);

            PrintToWord(oDoc, " ", 12, 1);

            oDoc.Save();
            oDoc.Close();

            oWord.Visible = true;
            oWord.Documents.Open(FileName: clientFileLocationName);
            oWord.Activate();

            return clientFileLocationName;

        }



        private void WriteLineToRoSD(TreeNode tn, WordNet._Document oDoc, WordNet.Table oTable, string prefix="", string parent="", int seqnum=0)
        {
            if (tn.Tag == null || tn.Tag.GetType().Name != "scClientDocSetDocLink") 
            {
                // still need to check subnodes
            }
            else
            {

                int x = 0;
                foreach (TreeNode node in tn.Nodes)
                {
                    x++;
                    row++;

                    scClientDocSetDocLink documentClient = (scClientDocSetDocLink)node.Tag;

                    // First column

                    string currentParent = "";
                    if (string.IsNullOrEmpty(parent))
                    {
                        currentParent = x.ToString();
                    }
                    else
                    {
                        currentParent = parent + "." + x.ToString();
                    }

                    oTable.Cell(row, 1).Width = 30;
                    oTable.Cell(row, 1).Range.Text = currentParent;

                    oTable.Cell(row, 2).Width = 30;

                    System.Drawing.Bitmap bitmap1 = Properties.Resource1.FolderIcon;

                    if (documentClient.document.RecordType.Trim() == Utils.RecordType.FOLDER)
                    {
                        bitmap1 = Properties.Resource1.FolderIcon;
                    }
                    else 
                    {
                        bitmap1 = Properties.Resource1.WordIcon;

                        if (documentClient.document.DocumentType == Utils.DocumentType.EXCEL)
                            bitmap1 = Properties.Resource1.ExcelIcon;

                        if (documentClient.document.DocumentType == Utils.DocumentType.PDF)
                            bitmap1 = Properties.Resource1.PDFIcon;

                    }

                    Clipboard.SetImage(bitmap1);
                    oTable.Cell(row, 2).Range.Paste();


                    oTable.Cell(row, 3).Width = 60;
                    oTable.Cell(row, 3).Range.Text = prefix + documentClient.document.CUID;

                    oTable.Cell(row, 4).Width = 50;
                    oTable.Cell(row, 4).Range.Text = prefix + documentClient.document.IssueNumber.ToString("000");

                    oTable.Cell(row, 5).Width = 300;
                    oTable.Cell(row, 5).Range.Text = prefix + documentClient.document.Name;

                    oTable.Cell(row, 6).Width = 100;
                    oTable.Cell(row, 6).Range.Text = "???";

                    if (uioutput != null) uioutput.AddOutputMessage(documentClient.document.Name);

                    if (node.Nodes.Count > 0)
                        WriteLineToRoSD(node, oDoc, oTable, prefix: "", parent: currentParent, seqnum: x);

                }
            }
        }


        private static void ApplyHeadingStyle(WordNet.Cell cell, int width = 300)
        {
            cell.Width = width;
            
            cell.Range.Font.Name = "Arial";
            cell.Range.Font.Size = 10;
            cell.Range.Font.Bold = 1;

        }
        private static void ApplyContentsStyle(WordNet.Cell cell, WordNet.WdCellVerticalAlignment verticalAlignment = WordNet.WdCellVerticalAlignment.wdCellAlignVerticalCenter)
        {
            cell.VerticalAlignment = verticalAlignment;
            cell.Range.Font.Name = "Arial";
            cell.Range.Font.Size = 8;
            cell.Range.Font.Bold = 0;
        }


        private void PrintToWord(WordNet._Document oDoc, string toPrint, int fontSize, int bold,
                                 string align = FCMWordAlign.LEFT)
        {
            WordNet.WdParagraphAlignment walign = WordNet.WdParagraphAlignment.wdAlignParagraphLeft;

            if (align == FCMWordAlign.CENTER)
                walign = WordNet.WdParagraphAlignment.wdAlignParagraphCenter;
            if (align == FCMWordAlign.RIGHT)
                walign = WordNet.WdParagraphAlignment.wdAlignParagraphRight;

            object oRng =
                   oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            WordNet.Paragraph oPara = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara.Range.Font.Name = "Arial";
            oPara.Range.Font.Bold = bold;
            oPara.Range.Font.Size = fontSize;
            oPara.Range.Text = toPrint;
            oPara.Range.InsertParagraphAfter();
            oPara.Alignment = walign;

        }

        public struct FCMWordAlign
        {
            public const string LEFT = "LEFT";
            public const string RIGHT = "RIGHT";
            public const string CENTER = "CENTER";
        }
    }
}
