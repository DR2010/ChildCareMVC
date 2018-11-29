using System;
using System.IO;
using System.Threading;
using FCMMySQLBusinessLibrary;

namespace MackkadoITFramework.Utils
{
    /// <summary>
    /// Class to handle log files
    /// </summary>
    public static class LogFile
    {
        /// <summary>
        /// Constructor with the file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="userID"> </param>
        public static string WriteToTodaysLogFile(
                string what, 
                string userID = "User Not Supplied",
                string messageCode = "",
                string programName = "",
                string processname = "")
        {
            string _Location = MakConstant.SYSFOLDER.LOGFILEFOLDER;
            StreamWriter log;

            // Generate File Name
            // 
            string currYear = DateTime.Today.Year.ToString("0000");
            string currMonth = DateTime.Today.Month.ToString("00");
            string currDay = DateTime.Today.Day.ToString("00");
            string fileName = "FCM_" + currYear + currMonth + currDay + processname + ".txt";

            // interesting how I can code remotely
            //
            // get path
            //string filePathName = Utils.getFilePathName(_Location, fileName);
            string filePathName = XmlConfig.Read(MakConstant.ConfigXml.AuditLogPath) + "\\" + fileName; 


            if (! File.Exists(filePathName))
            {
                try
                {
                    log = new StreamWriter(filePathName);
                    
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.ToString();

                }
            }
            else
            {
                
                try
                {
                    log = File.AppendText( filePathName );
                }
                catch ( Exception ex1 )
                {
                    // Sleep a bit just to minimise conflict
                    Thread.Sleep( 1000 );

                    try
                    {
                        log = File.AppendText( filePathName );
                    }
                    catch ( Exception ex )
                    {
                        return "Error: " + ex.ToString();
                    }
                }

            }

            // Write to the file:
            log.WriteLine(
                    processname  + ": " +
                    DateTime.Now + ": " + 
                    userID + ": " + 
                    messageCode + ": " +
                    programName + ": " +
                    what 
                    );

            // Close the stream:
            log.Close();

            return "Ok";
        }

    }
}
