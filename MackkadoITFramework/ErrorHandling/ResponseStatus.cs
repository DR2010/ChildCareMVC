using System.Windows.Forms;
using FCMMySQLBusinessLibrary;
using MackkadoITFramework.Utils;

namespace MackkadoITFramework.ErrorHandling
{
    public class ResponseStatus
    {
        public int ReturnCode
        {
            set
            {
                returnCode = value;

                if (returnCode <= 0)
                {
                    Successful = false;
                    XMessageType = MessageType.Error;
                    Icon = MessageBoxIcon.Error;
                    UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000001;
                }
                else
                {
                    Successful = true;
                    XMessageType = MessageType.Informational;
                    Icon = MessageBoxIcon.Information;
                    UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;

                    if (ReasonCode > 1)
                    {
                        Successful = true;
                        XMessageType = MessageType.Warning;
                        Icon = MessageBoxIcon.Warning;
                        UniqueCode = ResponseStatus.MessageCode.Warning.FCMWAR00000001;
                    }
                }
            }
            get
            {
                return returnCode;
            }
        }

        public int ReasonCode
        {
            set
            {
                reasonCode = value;

                if (returnCode <= 0)
                {
                    Successful = false;
                    XMessageType = MessageType.Error;
                    Icon = MessageBoxIcon.Error;
                }
                else
                {
                    Successful = true;
                    XMessageType = MessageType.Informational;
                    Icon = MessageBoxIcon.Information;

                    if (ReasonCode > 1)
                    {
                        XMessageType = MessageType.Warning;
                        Icon = MessageBoxIcon.Warning;
                    }
                }
            }
            get
            {
                return reasonCode;
            }
        }
        public string UniqueCode
        {
            get
            {
                return uniqueCode;
            }
            set
            {
                uniqueCode = value;
            }
        }
        public string Message { get; set; }
        public MessageType XMessageType { get; set; }
        public object Contents { get; set; }
        public MessageBoxIcon Icon { get; set; }
        public bool Successful { get; set; }
        private int returnCode;
        private int reasonCode;
        private string uniqueCode;

        public ResponseStatus()
        {
            ReturnCode = 0001;
            ReasonCode = 0001;
            Message = "Successful";
            Successful = true;
            XMessageType = MessageType.Informational;
            UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;
            Icon = MessageBoxIcon.Information;
        }

        public ResponseStatus(MessageType messageType)
        {

            if (messageType == MessageType.Error)
            {
                ReturnCode = -10;
                ReasonCode = 1;
                Successful = false;
                Message = "Error";
                XMessageType = MessageType.Error;
                UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00009999;
                Icon = MessageBoxIcon.Error;
            }
            if (messageType == MessageType.Informational)
            {
                ReturnCode = 1;
                ReasonCode = 1;
                Message = "Successful";
                Successful = true;
                XMessageType = MessageType.Informational;
                UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;
                Icon = MessageBoxIcon.Information;
            }

            if (messageType == MessageType.Warning)
            {
                ReturnCode = 1;
                ReasonCode = 2;
                Message = "Warning.";
                Successful = true;
                XMessageType = MessageType.Warning;
                UniqueCode = ResponseStatus.MessageCode.Warning.FCMWAR00000001;
                Icon = MessageBoxIcon.Warning;
            }

        }

        public void WriteToLog(HeaderInfo headerInfo)
        {
            LogFile.WriteToTodaysLogFile(this.UniqueCode + this.Message, headerInfo.UserID);
        }
    
            /// <summary>
        /// List of error codes
        /// </summary>
        public struct MessageCode
        {

            public struct Informational
            {
                // ---------------------- //
                // INFORMATIONAL MESSAGES //
                // ---------------------- //
                /// <summary>
                /// Generic Successful.
                /// </summary>
                public const string FCMINF00000001 = "FCMINF00.00.0001";

                /// <summary>
                /// User role added successfully.
                /// </summary>
                public const string FCMINF00000002 = "FCMINF00.00.0002";

                /// <summary>
                /// User role deleted successfully.
                /// </summary>
                public const string FCMINF00000003 = "FCMINF00.00.0003";

                /// <summary>
                /// User added successfully.
                /// </summary>
                public const string FCMINF00000004 = "FCMINF00.00.0004";

                /// <summary>
                /// User updated successfully.
                /// </summary>
                public const string FCMINF00000005 = "FCMINF00.00.0005";

                /// <summary>
                /// User added successfully.
                /// </summary>
                public const string FCMINF00000006 = "FCMINF00.00.0006";

                /// <summary>
                /// Client deleted successfully.
                /// </summary>
                public const string FCMINF00000007 = "FCMINF00.00.0007";

            }

            public struct Error
            {
                // ---------------------- //
                // ERROR MESSAGES //
                // ---------------------- //
                /// <summary>
                /// Invalid logon.
                /// </summary>
                public const string FCMERR00000001 = "FCMERR00.00.0001";

                /// <summary>
                /// Error deleting User Role.
                /// </summary>
                public const string FCMERR00000002 = "FCMERR00.00.0002";

                /// <summary>
                /// User ID is mandatory.
                /// </summary>
                public const string FCMERR00000003 = "FCMINF00.00.0003";

                /// <summary>
                /// Password is mandatory.
                /// </summary>
                public const string FCMERR00000004 = "FCMINF00.00.0004";

                /// <summary>
                /// Salt is mandatory.
                /// </summary>
                public const string FCMERR00000005 = "FCMINF00.00.0005";

                /// <summary>
                /// Error creating new version. Source file not found.
                /// </summary>
                public const string FCMERR00000006 = "FCMINF00.00.0006";

                /// <summary>
                /// Client name is mandatory.
                /// </summary>
                public const string FCMERR00000007 = "FCMINF00.00.0007";

                /// <summary>
                /// Role Name is mandatory.
                /// </summary>
                public const string FCMERR00000008 = "FCMINF00.00.0008";

                /// <summary>
                /// Client logo or icon not found.
                /// </summary>
                public const string FCMERR00000009 = "FCMINF00.00.0009";



                /// <summary>
                /// Error.
                /// </summary>
                public const string FCMERR00009999 = "FCMINF00.00.9999";


            }

            public struct Warning
            {

                // ---------------------- //
                // WARNING MESSAGES //
                // ---------------------- //
                /// <summary>
                /// Generic Warning.
                /// </summary>
                public const string FCMWAR00000001 = "FCMWAR00.00.0001";

                /// <summary>
                /// No valid contract was found for client.
                /// </summary>
                public const string FCMWAR00000002 = "FCMWAR00.00.0002";
            }
        }
    }
}
