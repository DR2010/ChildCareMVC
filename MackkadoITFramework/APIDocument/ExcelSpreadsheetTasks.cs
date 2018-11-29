using System;
using System.Collections.Generic;
using System.IO;
using MackkadoITFramework.APIDocument;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Interfaces;
using Excel=Microsoft.Office.Interop.Excel;
using System.Reflection;
using MackkadoITFramework.Utils;

namespace FCMMySQLBusinessLibrary
{
    public class ExcelSpreadsheetTasks
    {
        public void Test()
        {
            Excel._Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            Excel.Range oRng;

            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "First Name";
                oSheet.Cells[1, 2] = "Last Name";
                oSheet.Cells[1, 3] = "Full Name";
                oSheet.Cells[1, 4] = "Salary";

                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "D1").Font.Bold = true;
                oSheet.get_Range("A1", "D1").VerticalAlignment =
                    Excel.XlVAlign.xlVAlignCenter;

                // Create an array to multiple values at once.
                string[,] saNames = new string[5, 2];

                saNames[0, 0] = "John";
                saNames[0, 1] = "Smith";
                saNames[1, 0] = "Tom";
                saNames[1, 1] = "Brown";
                saNames[2, 0] = "Sue";
                saNames[2, 1] = "Thomas";
                saNames[3, 0] = "Jane";
                saNames[3, 1] = "Jones";
                saNames[4, 0] = "Adam";
                saNames[4, 1] = "Johnson";

                //Fill A2:B6 with an array of values (First and Last Names).
                oSheet.get_Range("A2", "B6").Value2 = saNames;

                //Fill C2:C6 with a relative formula (=A2 & " " & B2).
                oRng = oSheet.get_Range("C2", "C6");
                oRng.Formula = "=A2 & \" \" & B2";

                //Fill D2:D6 with a formula(=RAND()*100000) and apply format.
                oRng = oSheet.get_Range("D2", "D6");
                oRng.Formula = "=RAND()*100000";
                oRng.NumberFormat = "$0.00";

                //AutoFit columns A:D.
                oRng = oSheet.get_Range("A1", "D1");
                oRng.EntireColumn.AutoFit();

                //Make sure Excel is visible and give the user control
                //of Microsoft Excel's lifetime.
                oXL.Visible = true;
                oXL.UserControl = true;
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);
            }
        }

        public static ResponseStatus OpenDocument(string FileName)
        {
            ResponseStatus ret = new ResponseStatus();
            ret.ReasonCode = 1;
            ret.ReturnCode = 1;
            ret.Message = "Successful Execution";

            if (!File.Exists(FileName))
            {
                ret.ReturnCode = -10;
                ret.ReasonCode = 1;
                ret.Message ="File not found. " + FileName;
                return ret;
            }
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object missing = System.Reflection.Missing.Value;
            

            xlApp = new Excel.Application();

            xlApp.Visible = true;
            

            xlWorkBook = xlApp.Workbooks.Open(FileName, missing, false, missing, missing,
                missing, true, missing, "\t", missing,
                missing, missing, missing, missing, missing);

            return ret;
        }

        public static ResponseStatus PrintDocument( string FileName )
        {
            ResponseStatus ret = new ResponseStatus();
            ret.ReasonCode = 1;
            ret.ReturnCode = 1;
            ret.Message = "Successful Execution";

            if (!File.Exists( FileName ))
            {
                ret.ReturnCode = -10;
                ret.ReasonCode = 1;
                ret.Message = "File not found. " + FileName;
                return ret;
            }
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object missing = System.Reflection.Missing.Value;


            xlApp = new Excel.Application();

            xlApp.Visible = true;


            xlWorkBook = xlApp.Workbooks.Open( FileName, missing, false, missing, missing,
                missing, true, missing, "\t", missing,
                missing, missing, missing, missing, missing );

            xlWorkBook.PrintOutEx();

            xlWorkBook.Close();

            xlApp.Quit();

            return ret;
        }

        // ----------------------------------------------------
        //         Find and replace words in Excel
        // ----------------------------------------------------
        public static void FindAndReplace(
                                   object vkFind,
                                   object vkReplace,
                                   object vkNum,
                    Excel.Application vkExcelApp,
                           Excel.Workbook vkMyDoc
            )
        {

            string svkFind = vkFind.ToString();
            string svkReplace = vkReplace.ToString();


            // Retrieve workbooks
            Excel.Workbooks workbooks = vkExcelApp.Workbooks;

            // Switch off warnings
            vkExcelApp.ErrorCheckingOptions.UnlockedFormulaCells = false;
            vkExcelApp.ErrorCheckingOptions.EmptyCellReferences = false;
            vkExcelApp.ErrorCheckingOptions.Application.DisplayAlerts = false;

            // Replace Excel Document body
            //
            vkExcelApp.Cells.Replace(What: vkFind,
                                      Replacement: vkReplace,
                                      MatchCase: false,
                                      LookAt: Excel.XlLookAt.xlWhole);

            // Daniel 05-Aug-2013
            // Replace is replace the entire content of the cell
            // I can't find a way to replace only the specific text by another text.
            //

            int i = vkExcelApp.Worksheets.Count;

            // Excel.Worksheet wsX = (Excel.Worksheet)vkExcelApp.Worksheets[1];

            foreach (Excel.Worksheet ws in vkExcelApp.Worksheets)
            {
                // Daniel - Issue found here 
                //

                try
                {
                    ws.PageSetup.RightHeader = ws.PageSetup.RightHeader.Replace(svkFind, svkReplace);
                    ws.PageSetup.LeftHeader = ws.PageSetup.LeftHeader.Replace(svkFind, svkReplace);
                    ws.PageSetup.CenterHeader = ws.PageSetup.CenterHeader.Replace(svkFind, svkReplace);

                    ws.PageSetup.RightFooter = ws.PageSetup.RightFooter.Replace(svkFind, svkReplace);
                    ws.PageSetup.CenterFooter = ws.PageSetup.CenterFooter.Replace(svkFind, svkReplace);
                    ws.PageSetup.LeftFooter = ws.PageSetup.LeftFooter.Replace(svkFind, svkReplace);
                }
                catch (Exception ex)
                {
                    LogFile.WriteToTodaysLogFile(ex.ToString(), "", "", "ExcelSpreadSheetTasks.cs", "");
                    FCMEmail.SendEmailSimple("DanielLGMachado@gmail.com", "FCM Error ConsoleGenerate", ex.ToString());
                }
            }

            
            // vkExcelApp.WorksheetFunction.Substitute( Excel.HeaderFooter, vkFind.ToString(), vkReplace.ToString() ); 

            // Excel.HeaderFooter x;
            // x.Text.Replace( vkFind.ToString(), vkReplace.ToString() );

            // vkExcelApp.WorksheetFunction.Substitute( Excel.HeaderFooter, vkFind.ToString(), vkReplace.ToString() );

        }



        //// ---------------------------------------------
        ////             Copy Documents
        //// ---------------------------------------------
        //public static object CopyDocument(
        //                                   string fromFileName,
        //                                   string destinationFileName,
        //                                   List<WordDocumentTasks.TagStructure> tag,
        //                                   IOutputMessage uioutput, 
        //                                    string processName, 
        //                                    string userID
        //                          )
        //{

        //    var vkExcelApp = new Microsoft.Office.Interop.Excel.Application();
        //    vkExcelApp.Visible = false;

        //    // Excel.ApplicationClass vkExcelApp = new Excel.ApplicationClass();

        //    string saveFile = destinationFileName;

        //    object vkReadOnly = false;
        //    object vkVisible = true;
        //    object vkFalse = false;
        //    object vkTrue = true;
        //    object vkDynamic = 2;

        //    object vkMissing = System.Reflection.Missing.Value;

        //    // Let's make the excel application not visible
        //    // vkExcelApp.Visible = false;
        //    // vkExcelApp.Activate();

        //    // Let's copy the document
        //    File.Copy( fromFileName, destinationFileName, true );

        //    // Let's open the DESTINATION document
        //    //Word.Document vkMyDoc = vkExcelApp.Documents.Open(
        //    //    ref destinationFileName, ref vkMissing, ref vkReadOnly,
        //    //    ref vkMissing, ref vkMissing, ref vkMissing,
        //    //    ref vkMissing, ref vkMissing, ref vkMissing,
        //    //    ref vkMissing, ref vkMissing, ref vkVisible );

        //    Excel.Workbook vkMyDoc = vkExcelApp.Workbooks.Open(
        //        destinationFileName, 
        //        vkMissing, false, vkMissing, vkMissing,
        //        vkMissing, true, vkMissing, "\t", vkMissing,
        //        vkMissing, vkMissing, vkMissing, vkMissing, vkMissing );

        //    foreach (var t in tag)
        //    {
        //        // 17/02/2013
        //        // Ignore **MD** and other with ** because it is too risky
        //        //
        //        if (t.Tag == "**MD**" || t.Tag == "**PM**" || t.Tag == "**SM**" || t.Tag == "**ADDRESS**")
        //            continue;

        //        if (t.TagType == "IMAGE")
        //            continue;

        //        FindAndReplace( t.Tag, t.TagValue, 1, vkExcelApp, vkMyDoc );
        //    }

        //    try
        //    {
        //        vkMyDoc.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        uioutput.AddOutputMessage( "(Excel) ERROR in file:  " + fromFileName + " --- Message: " + ex.ToString(), processName, userID );
        //        uioutput.AddErrorMessage( "(Excel) ERROR in file:  " + fromFileName + " --- Message: " + ex.ToString(), processName, userID );

        //    }

        //    // close the new document
        //    vkMyDoc.Close();
        //    System.Runtime.InteropServices.Marshal.ReleaseComObject( vkMyDoc );

        //    // close excel application
        //    vkExcelApp.Quit();

        //    return saveFile;
        //}
    }
}
