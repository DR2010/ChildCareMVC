using System.IO;
using Microsoft.Office.Interop.Word;

namespace FCMBusinessLibrary.FCMWord
{
    public class WordReportEngine
    {

        object missing = System.Reflection.Missing.Value;
        private object oFalse = false;
        object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */
        _Application oWord;
        _Document oDoc;

        public string CreateReport()
        {
            if (oWord == null)
            {
                StartupWord();
            }
            Clean();

            var tempFile = Path.GetTempFileName();

            Paste();

            SaveAsRtf(tempFile);

            return tempFile;
        }

        private void Clean()
        {
            if (oDoc != null)
            {
                oDoc.Close(ref oFalse, ref missing, ref missing);
            }
            oDoc = oWord.Documents.Add(ref missing, ref missing,
                                       ref missing, ref missing);
        }

        private void StartupWord()
        {
            oWord = new Application();
            oWord.Visible = false;
            oDoc = oWord.Documents.Add(ref missing, ref missing,
                                       ref missing, ref missing);
        }

        public void CloseWord()
        {
            if (oDoc != null)
            {
                oDoc.Close(ref oFalse, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDoc);
            
            }

            if (oWord != null)
            {
                oWord.Quit(ref missing, ref missing, ref missing);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oWord);
            }

            oDoc = null;
            oWord = null;
            
        }

        private void addText(string toPrint, int fontSize, int bold)
        {
            Paragraph oPara;
            object oRng =
                oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara.Range.Font.Bold = bold;
            oPara.Range.Font.Size = fontSize;
            oPara.Range.Text = toPrint;
            oPara.Range.InsertParagraphAfter();
        }

        public void Paste()
        {
            Paragraph oPara;
            object oRng =
                oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara.Range.Paste();
            oPara.Range.InsertParagraphAfter();
 
        }

        internal void SaveAsRtf(object fileName)
        {
            object format = WdSaveFormat.wdFormatRTF;

            oDoc.SaveAs(ref fileName, ref format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        }
    }
}