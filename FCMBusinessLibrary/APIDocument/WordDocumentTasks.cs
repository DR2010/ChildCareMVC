using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using fcm.Interfaces;

namespace FCMBusinessLibrary
{
    public class WordDocumentTasks
    {

        // ---------------------------------------------
        //             Open Document
        // ---------------------------------------------
        public static void OpenDocument(object fromFileName, object vkReadOnly)
        {

            if (! File.Exists((string)fromFileName))
            {
                MessageBox.Show("File not found. " + fromFileName);
                return;
            }


            Word.Application vkWordApp =
                                 new Word.Application();

            // object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;


            object vkMissing = System.Reflection.Missing.Value;

            // Let's make the word application visible
            vkWordApp.Visible = true;
            vkWordApp.Activate();

            // Let's open the document
            Word.Document vkMyDoc = vkWordApp.Documents.Open(
                ref fromFileName, ref vkMissing, ref vkReadOnly,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkVisible);

            return;
        }

        // ---------------------------------------------
        //             Copy Documents
        // ---------------------------------------------
        public static object CopyDocumentReplaceContents(
                                           object fromFileName,
                                           object destinationFileName,
                                           List<WordDocumentTasks.TagStructure> tag
                                  )
        {

            Word.Application vkWordApp = 
                                 new Word.Application();

            object saveFile = destinationFileName;

            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;


            object vkMissing = System.Reflection.Missing.Value;

            // Let's make the word application visible
            vkWordApp.Visible = true;
            vkWordApp.Activate();

            // Let's open the document
            Word.Document vkMyDoc = vkWordApp.Documents.Open(
                ref fromFileName, ref vkMissing, ref vkReadOnly,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkVisible);

            // Let's create a new document
            Word.Document vkNewDoc = vkWordApp.Documents.Add(
                ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkVisible);

            // Select and Copy from the original document
            vkMyDoc.Select();
            vkWordApp.Selection.Copy();

            // Paste into new document as unformatted text
            vkNewDoc.Select();
            vkWordApp.Selection.PasteSpecial(ref vkMissing, ref vkFalse,
                                               ref vkMissing, ref vkFalse, ref vkDynamic, 
                                               ref vkMissing, ref vkMissing);

            // Save the new document
            vkNewDoc.SaveAs(ref saveFile, ref vkMissing,
                               ref vkMissing, ref vkMissing, ref vkMissing,
                               ref vkMissing, ref vkMissing, ref vkMissing,
                               ref vkMissing, ref vkMissing, ref vkMissing);


            // Copy elements
            // FromString => toString
            // 
            // underdevelopment
            //
            vkNewDoc.Select();

            foreach (var t in tag)
            {
                FindAndReplace(t.Tag, t.TagValue, 1, vkWordApp, vkMyDoc);
            }

            vkNewDoc.Save();

            // close the new document
            vkNewDoc.Close(ref vkFalse, ref vkMissing, ref vkMissing);

            // close the original document
            vkMyDoc.Close(ref vkFalse, ref vkMissing, ref vkMissing);

            // close word application
            vkWordApp.Quit(ref vkFalse, ref vkMissing, ref vkMissing);

            return saveFile;
        }

        
        // ---------------------------------------------
        //    
        // ---------------------------------------------
        /// <summary>
        /// Create client document and replace document tags
        /// </summary>
        /// <param name="fromFileName"></param>
        /// <param name="destinationFileName"></param>
        /// <param name="tag"></param>
        /// <param name="vkWordApp"></param>
        /// <returns></returns>
        public static ResponseStatus CopyDocument(
                                           object fromFileName,
                                           object destinationFileName,
                                           List<WordDocumentTasks.TagStructure> tag,
                                            Word.Application vkWordApp,
                                            IOutputMessage uioutput
                                  )
        {

            ResponseStatus ret = new ResponseStatus();

            object saveFile = destinationFileName;

            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;

            object vkMissing = System.Reflection.Missing.Value;

            // Let's make the word application not visible
            // vkWordApp.Visible = false;
            // vkWordApp.Activate();

            // Let's copy the document
            File.Copy(fromFileName.ToString(), destinationFileName.ToString(), true);

            // Let's open the DESTINATION document

            Word.Document vkMyDoc;
            try
            {

                vkMyDoc = vkWordApp.Documents.Open(
                    ref destinationFileName, ref vkMissing, ref vkReadOnly,
                    ref vkMissing, ref vkMissing, ref vkMissing,
                    ref vkMissing, ref vkMissing, ref vkMissing,
                    ref vkMissing, ref vkMissing, ref vkVisible );

            }
            catch (Exception ex)
            {

                ret.ReturnCode = -1;
                ret.ReasonCode = 1000;
                ret.Message = "Error copying file.";
                ret.Contents = ex;
                return ret;
            }

            foreach (var t in tag)
            {
                if (t.TagType == Utils.InformationType.FIELD || t.TagType == Utils.InformationType.VARIABLE)
                    FindAndReplace(t.Tag, t.TagValue, 1, vkWordApp, vkMyDoc);
                else
                {
                    insertPicture(vkMyDoc, t.TagValue, t.Tag);
                }
            }

            // 24/10/2010 - Modificado quando troquei a referencia do Word
            //
            //vkMyDoc.Sections.Item( 1 ).Headers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterPrimary ).Range.Fields.Update();
            //vkMyDoc.Sections.Item( 1 ).Footers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterPrimary ).Range.Fields.Update();


            try
            {
                vkMyDoc.Save();
            }
            catch (Exception ex)
            {
                uioutput.AddOutputMessage( "(Word) ERROR in file:  " + fromFileName + " --- Message: " + ex.ToString() );
            }

            // close the new document
            vkMyDoc.Close();

            // Trying to release COM object
            System.Runtime.InteropServices.Marshal.ReleaseComObject( vkMyDoc );

            return ret;
        }




        // ---------------------------------------------
        //             Copy Documents
        // ---------------------------------------------
        public static object CopyDocument(
                                           object fromFileName,
                                           object destinationFileName,
                                           List<WordDocumentTasks.TagStructure> tag
                                  )
        {

            Word.Application vkWordApp =
                                 new Word.Application();

            object saveFile = destinationFileName;

            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;

            object vkMissing = System.Reflection.Missing.Value;

            // Let's make the word application visible
            vkWordApp.Visible = true;
            vkWordApp.Activate();

            // Let's copy the document
            File.Copy(fromFileName.ToString(), destinationFileName.ToString(), true);

            // Let's open the DESTINATION document
            Word.Document vkMyDoc = vkWordApp.Documents.Open(
                ref destinationFileName, ref vkMissing, ref vkReadOnly,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkVisible);

            foreach (var t in tag)
            {
                FindAndReplace(t.Tag, t.TagValue, 1, vkWordApp, vkMyDoc);
            }

            vkMyDoc.Save();

            // close the new document
            vkMyDoc.Close(ref vkFalse, ref vkMissing, ref vkMissing);

            // close word application
            vkWordApp.Quit(ref vkFalse, ref vkMissing, ref vkMissing);

            return saveFile;
        }


        // ----------------------------------------------------
        //         Find and replace words in MS Word
        // ----------------------------------------------------
        public static void FindAndReplace(
                                   object vkFind, 
                                   object vkReplace, 
                                   object vkNum, 
                    Word.Application vkWordApp,
                            Word.Document vkMyDoc
            )
        {
            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object missing = false;
            object vkTrue = true;
            object vkDynamic = 2;
            object replaceAll = Word.WdReplace.wdReplaceAll; 

            // Replace Word Document body
            //
            
            // 05/09/2010 - Testando a passagem de paramentros com nome do parametro... nao remover codigo abaixo.

            // Working... Forward = false;

            //vkWordApp.Selection.Find.Execute(
            //    ref vkFind, ref vkFalse, ref vkFalse,
            //    ref vkFalse, ref vkFalse, ref vkFalse,
            //    ref vkTrue, ref vkNum, ref vkFalse,
            //    ref vkReplace, ref vkDynamic, ref vkFalse,
            //    ref vkFalse, ref vkFalse, ref vkFalse );

            //vkWordApp.Selection.Find.Execute(
            //          FindText: vkFind,
            //          ReplaceWith: vkReplace,
            //          MatchCase: vkFalse,
            //          MatchWholeWord: vkTrue,
            //          MatchAllWordForms: vkTrue,
            //          Replace: vkDynamic);

            vkWordApp.Selection.Find.Execute(
                MatchCase: vkFalse,
                MatchWholeWord: vkTrue,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                FindText: vkFind,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );

            // Replace in the primary header/ footer
            //
            //vkMyDoc.Sections.Item(1).Headers.Item(Word.WdHeaderFooterIndex.wdHeaderFooterPrimary).Range.Find.Execute(
            //    ref vkFind, ref vkFalse, ref vkFalse,
            //    ref vkFalse, ref vkFalse, ref vkFalse,
            //    ref vkTrue, ref vkNum, ref vkFalse,
            //    ref vkReplace, ref vkDynamic, ref vkFalse,
            //    ref vkFalse, ref vkFalse, ref vkFalse);

            vkMyDoc.Sections.Item( 1 ).Headers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterPrimary ).Range.Find.Execute(
                FindText: vkFind,
                MatchCase: vkFalse,
                MatchWholeWord: vkFalse,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );

            vkMyDoc.Sections.Item( 1 ).Headers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage).Range.Find.Execute(
                FindText: vkFind,
                MatchCase: vkFalse,
                MatchWholeWord: vkFalse,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );

            vkMyDoc.Sections.Item( 1 ).Headers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterEvenPages).Range.Find.Execute(
                FindText: vkFind,
                MatchCase: vkFalse,
                MatchWholeWord: vkFalse,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );
    

            // Replace in the first page footer
            //
            //vkMyDoc.Sections.Item( 1 ).Footers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterPrimary).Range.Find.Execute(
            //    ref vkFind, ref vkFalse, ref vkFalse,
            //    ref vkFalse, ref vkFalse, ref vkFalse,
            //    ref vkTrue, ref vkNum, ref vkFalse,
            //    ref vkReplace, ref vkDynamic, ref vkFalse,
            //    ref vkFalse, ref vkFalse, ref vkFalse );


            vkMyDoc.Sections.Item( 1 ).Footers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterPrimary).Range.Find.Execute(
                FindText: vkFind,
                MatchCase: vkFalse,
                MatchWholeWord: vkFalse,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );


            vkMyDoc.Sections.Item( 1 ).Footers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterFirstPage ).Range.Find.Execute(
                FindText: vkFind,
                MatchCase: vkFalse,
                MatchWholeWord: vkFalse,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );

            vkMyDoc.Sections.Item( 1 ).Footers.Item( Word.WdHeaderFooterIndex.wdHeaderFooterEvenPages ).Range.Find.Execute(
                FindText: vkFind,
                MatchCase: vkFalse,
                MatchWholeWord: vkFalse,
                MatchWildcards: vkFalse,
                MatchSoundsLike: vkFalse,
                MatchAllWordForms: vkFalse,
                Forward: vkFalse,
                Wrap: vkNum,
                Format: vkFalse,
                ReplaceWith: vkReplace,
                Replace: vkDynamic,
                MatchKashida: vkFalse,
                MatchDiacritics: vkFalse,
                MatchAlefHamza: vkFalse,
                MatchControl: vkFalse );

        }


        // ----------------------------------------------------
        //         Insert picture 
        // ----------------------------------------------------
        public static void insertPicture(Word.Document oDoc, 
                                         string pictureFile,
                                         object bookMarkName
                                         )
        {
            object oMissing = System.Reflection.Missing.Value;
            
            // oDoc.ActiveWindow.Selection.Range.InlineShapes.AddPicture(
            //     pictureFile, ref oMissing, ref oMissing, ref oMissing);

            // Object bookMarkName = "COMPANY_LOGO";

            if (oDoc.Bookmarks.Exists(bookMarkName.ToString()))
            {
                oDoc.Bookmarks.Item(ref bookMarkName).Range.InlineShapes.AddPicture(
                    pictureFile, ref oMissing, ref oMissing, ref oMissing);
            }
            // Object oMissed = doc.Paragraphs[2].Range; //the position you want to insert
            // doc.InlineShapes.AddPicture(

        }


        // ----------------------------------------------------
        //            Copy folder structure including files
        // ----------------------------------------------------
        static public void CopyFolder( string sourceFolder, string destFolder )
        {
            if (!Directory.Exists( destFolder ))
                Directory.CreateDirectory( destFolder );

            string[] files = Directory.GetFiles( sourceFolder );
            foreach (string file in files)
            {
                string name = Path.GetFileName( file );

                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileExtension = Path.GetExtension(file);

                string dest = Path.Combine( destFolder, 
                               fileName + "v01" + fileExtension );

                File.Copy( file, dest );
            }

            string[] folders = Directory.GetDirectories( sourceFolder );
            foreach (string folder in folders)
            {
                string name = Path.GetFileName( folder );
                string dest = Path.Combine( destFolder, name );
                CopyFolder( folder, dest );
            }
        }

        // ----------------------------------------------------
        //       Copy folder structure including files
        // ----------------------------------------------------
        static public ResponseStatus LoadFolder(string sourceFolder,
                     IOutputMessage uioutput,
                     int parentUID, int sequenceNumber, HeaderInfo headerInfo)
        {

            ResponseStatus response = new ResponseStatus();
            response.Message = "Folder loaded successfully.";

            if (!Directory.Exists(sourceFolder))
            {
                response.ReturnCode = -0010;
                response.ReasonCode = -0001;
                response.Message = "Source folder does not exist.";
                response.UniqueCode = "E00.00.0001";
                response.Icon = MessageBoxIcon.Error;
                return response;
            }

            string[] folderNameSplit = sourceFolder.Split('\\');
            string folderName = folderNameSplit[folderNameSplit.Length - 1];

            uioutput.Activate();

            string[] files = Directory.GetFiles(sourceFolder);

            // Create folder that contains files and keep the parent
            //
            // ...
            Document.Document folder = new Document.Document();

            if (folderName.Length >= 7)
                folder.CUID = folderName.Substring( 0, 7 );
            else
                folder.CUID = folderName;

            folder.FileName = folderName;
            folder.Comments = "Loaded by batch";
            folder.Name = folderName;
            folder.DisplayName = folderName;
            folder.FKClientUID = 0;
            folder.IssueNumber = 0;
            string refPath =
                    Utils.getReferenceFilePathName(sourceFolder);
            if (string.IsNullOrEmpty(refPath))
            {
                response.ReturnCode = -0010;
                response.ReasonCode = -0002;
                response.Message = "Folder selected is not under managed template folder.";
                response.UniqueCode = "E00.00.0001";
                return response;
            }

            folder.Location = refPath;
            // Store the folder being loaded at the root level
            //
            folder.Location = FCMConstant.SYSFOLDER.TEMPLATEFOLDER;
            folder.ParentUID = parentUID;
            folder.SequenceNumber = 0;
            folder.SourceCode = "FCM";
            folder.UID = 0;
            folder.RecordType = Utils.RecordType.FOLDER;
            folder.DocumentType = Utils.DocumentType.FOLDER;
            folder.SimpleFileName = folder.Name;
            folder.FileExtension = "FOLDER";
            folder.IsProjectPlan = "N";

            parentUID = folder.Save(headerInfo, Utils.SaveType.NEWONLY);

            // Store each file
            //
            foreach (string file in files)
            {
                #region File Processing
                string name = Path.GetFileName(file);

                string fileName = Path.GetFileNameWithoutExtension(file);
                string fileExtension = Path.GetExtension(file);

                string validExtensions = ".doc .docx .xls . xlsx .pdf";

                // Not every extension will be loaded
                //
                if (!validExtensions.Contains( fileExtension ))
                    continue;

 
                string fileNameExt = Path.GetFileName(file);

                string simpleFileName = fileNameExt;
                if (fileNameExt.Length > 10)
                    simpleFileName = fileNameExt.Substring(10).Trim();

                Document.Document document = new Document.Document();
                document.CUID = fileName.Substring(0, 6);
                document.FileName = fileNameExt;

                //string refPath =
                //        Utils.getReferenceFilePathName(sourceFolder);

                document.Location = refPath;
                string issue = "1";
                document.IssueNumber = Convert.ToInt32(issue);

                try
                {
                    issue = fileName.Substring(7, 2);
                    document.IssueNumber = Convert.ToInt32(issue);
                }
                catch (Exception ex)
                {
                }
                document.Name = fileName;
                document.SimpleFileName = simpleFileName;
                document.DisplayName = simpleFileName;
                document.SequenceNumber = sequenceNumber;
                document.ParentUID = parentUID;

                document.Comments = "Loaded via batch";
                document.SourceCode = "FCM";
                document.FKClientUID = 0;
                document.RecordType = Utils.RecordType.DOCUMENT;
                document.FileExtension = fileExtension;
                document.Status = Utils.DocumentStatus.ACTIVE;
                document.IsProjectPlan = "N";
               
                switch (fileExtension)
                {
                    case ".doc":
                        document.DocumentType = Utils.DocumentType.WORD;
                        break;

                    case ".docx":
                        document.DocumentType = Utils.DocumentType.WORD;
                        break;

                    case ".xls":
                        document.DocumentType = Utils.DocumentType.EXCEL;
                        break;

                    case ".xlsx":
                        document.DocumentType = Utils.DocumentType.EXCEL;
                        break;

                    case ".pdf":
                        document.DocumentType = Utils.DocumentType.PDF;
                        break;

                    default:
                        document.DocumentType = Utils.DocumentType.UNDEFINED;
                        break;
                }

                document.Save(headerInfo, Utils.SaveType.NEWONLY);

                uioutput.AddOutputMessage(document.Name);

                sequenceNumber++;
                #endregion File Processing
            }

            // Recursion removed
            //
            string[] folders = Directory.GetDirectories( sourceFolder );
            foreach (string directory in folders)
            {
                string name = Path.GetFileName( directory );
                LoadFolder(directory, uioutput, parentUID, 0, headerInfo);
            }

            return response;
        }


        // ----------------------------------------------------
        //            Copy folder structure
        // ----------------------------------------------------
        static public void ReplicateFolderStructure(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                ReplicateFolderStructure(folder, dest);
            }
        }

        // ----------------------------------------------------
        //         Replace strings in structure
        // ----------------------------------------------------
        static public void ReplaceStringInAllFiles(
                   string originalFolder, 
                   List<TagStructure> tagList,
                   Word.Application vkWordApp)
        {
            object vkMissing = System.Reflection.Missing.Value;
            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;

            if (Directory.Exists(originalFolder))
            {
                string[] files = Directory.GetFiles(originalFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    object xFile = file;

                    // Let's open the document
                    Word.Document vkMyDoc = vkWordApp.Documents.Open(
                        ref xFile, ref vkMissing, ref vkReadOnly,
                        ref vkMissing, ref vkMissing, ref vkMissing,
                        ref vkMissing, ref vkMissing, ref vkMissing,
                        ref vkMissing, ref vkMissing, ref vkVisible);

                    vkMyDoc.Select();

                    if (tagList.Count > 0)
                    {
                        for (int i = 0; i < tagList.Count; i++)
                        {
                            FindAndReplace(
                                tagList[i].Tag,
                                tagList[i].TagValue,
                                1,
                                vkWordApp,
                                vkMyDoc);
                        }
                    }

                    WordDocumentTasks.insertPicture( vkMyDoc, "C:\\Research\\fcm\\Resources\\FCMLogo.jpg","");
                    vkMyDoc.Save();
                    vkMyDoc.Close(ref vkFalse, ref vkMissing, ref vkMissing);

                }

                //
                //  Replace in all folders
                //
                string[] folders = Directory.GetDirectories(originalFolder);
                foreach (string folder in folders)
                {
                    ReplaceStringInAllFiles(folder, 
                                            tagList,
                                            vkWordApp);
                }
            }
        }

        public struct TagStructure : IEnumerable
        {
            public string TagType;
            public string Tag;
            public string TagValue;
            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        // ---------------------------------------------
        //             Print Document
        // ---------------------------------------------
        public static void PrintDocument( object fromFileName )
        {

            if (!File.Exists( (string)fromFileName ))
            {
                MessageBox.Show( "File not found. " + fromFileName );
                return;
            }


            Word.Application vkWordApp =
                                 new Word.Application();

            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;


            object vkMissing = System.Reflection.Missing.Value;

            // Let's make the word application visible
            vkWordApp.Visible = true;
            vkWordApp.Activate();

            // Let's open the document
            Word.Document vkMyDoc = vkWordApp.Documents.Open(
                ref fromFileName, ref vkMissing, ref vkReadOnly,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkVisible );

            vkMyDoc.PrintOut();

            vkMyDoc.Close();
            System.Runtime.InteropServices.Marshal.ReleaseComObject( vkMyDoc );

            vkWordApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject( vkWordApp );

            return;
        }


        /*

 http://www.c-sharpcorner.com/UploadFile/amrish_deep/WordAutomation05102007223934PM/WordAutomation.aspx
         * 9.1 Embedding Pictures in Document Header:
 
//EMBEDDING LOGOS IN THE DOCUMENT
//SETTING FOCUES ON THE PAGE HEADER TO EMBED THE WATERMARK
oWord.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekCurrentPageHeader;
 
//THE LOGO IS ASSIGNED TO A SHAPE OBJECT SO THAT WE CAN USE ALL THE
//SHAPE FORMATTING OPTIONS PRESENT FOR THE SHAPE OBJECT
Word.Shape logoCustom = null;
 
//THE PATH OF THE LOGO FILE TO BE EMBEDDED IN THE HEADER
String logoPath = "C:\\Document and Settings\\MyLogo.jpg";
logoCustom = oWord.Selection.HeaderFooter.Shapes.AddPicture(logoPath,
    ref oFalse, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
 
logoCustom.Select(ref oMissing);
logoCustom.Name = "CustomLogo";
logoCustom.Left = (float)Word.WdShapePosition.wdShapeLeft;
 
//SETTING FOCUES BACK TO DOCUMENT
oWord.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekMainDocument;
        
        */


        public static void CompareDocuments(object fromFileName, string toFileName)
        {

            if (!File.Exists((string)fromFileName))
            {
                MessageBox.Show("File not found. " + fromFileName);
                return;
            }


            Word.Application vkWordApp =
                                 new Word.Application();

            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;


            object vkMissing = System.Reflection.Missing.Value;

            // Let's make the word application visible
            vkWordApp.Visible = true;
            vkWordApp.Activate();

            // Let's open the document
            Word.Document vkMyDoc = vkWordApp.Documents.Open(
                ref fromFileName, ref vkMissing, ref vkReadOnly,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkMissing,
                ref vkMissing, ref vkMissing, ref vkVisible);

            vkMyDoc.Compare(toFileName);

            vkMyDoc.Close();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(vkMyDoc);

            vkWordApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(vkWordApp);

            return;
        }

        /// <summary>
        /// Generate master of system documents
        /// </summary>
        /// <param name="tv"></param>
        private ResponseStatus GenerateMasterOfSystemDocuments(TreeView tv)
        {
            ResponseStatus ret = new ResponseStatus();
            object destinationFileName = "temp01.doc";
            object saveFile = destinationFileName;

            object vkReadOnly = false;
            object vkVisible = true;
            object vkFalse = false;
            object vkTrue = true;
            object vkDynamic = 2;

            object vkMissing = System.Reflection.Missing.Value;
            Word.Application vkWordApp = new Word.Application();

            Word.Document vkMyDoc;
            try
            {
                vkMyDoc = vkWordApp.Documents.Open(
                    ref destinationFileName, ref vkMissing, ref vkReadOnly,
                    ref vkMissing, ref vkMissing, ref vkMissing,
                    ref vkMissing, ref vkMissing, ref vkMissing,
                    ref vkMissing, ref vkMissing, ref vkVisible);

            }
            catch (Exception ex)
            {

                ret.ReturnCode = -1;
                ret.ReasonCode = 1000;
                ret.Message = "Error creating file.";
                ret.Contents = ex;
                return ret;
            }





            return ret;
        }

    }
}
