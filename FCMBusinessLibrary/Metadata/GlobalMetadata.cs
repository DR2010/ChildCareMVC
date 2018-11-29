using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary
{
    class GlobalMetadata
    {
        // public Int32 UID;
        public string InformationType;
        public string FieldCode;
        public string TableName;
        public string FieldName;
        public string FilePath;
        private string _userID;
        private string _dbConnectionString;

        // -----------------------------------------------------
        //   Constructor using userId and connection string
        // -----------------------------------------------------
        public GlobalMetadata(string UserID, string DBConnectionString)
        {
            _userID = UserID;
            _dbConnectionString = DBConnectionString;

        }

    }
}
