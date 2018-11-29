using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FCMBusinessLibrary
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
        public static string WriteToTodaysLogFile(
                string what, 
                string userID,
                string messageCode = "",
                string programName = "")
        {
            string _Location = FCMConstant.SYSFOLDER.LOGFILEFOLDER;
            StreamWriter log;

            // Generate File Name
            // 
            string currYear = DateTime.Today.Year.ToString("0000");
            string currMonth = DateTime.Today.Month.ToString("00");
            string currDay = DateTime.Today.Day.ToString("00");
            string fileName = "FCM_" + currYear + currMonth + currDay + ".txt";

            // get path
            string filePathName = Utils.getFilePathName(_Location, fileName);

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
                log = File.AppendText(filePathName);
            }

            // Write to the file:
            log.WriteLine(
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
