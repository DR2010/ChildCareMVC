using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using FCMBusinessLibrary.ReferenceData;

namespace FCMBusinessLibrary
{
    public class ReportMetadata
    {

        private SqlConnection _sqlConnection;
        private string _dbConnectionString;
        private string _userID;

        public int UID { get; set; }
        public string RecordType { get; set; } // DF = Default; CS = Company Specific
        public string FieldCode { get; set; }
        public string ClientType { get; set; }
        public int ClientUID { get; set; }
        public string Description { get; set; }
        public string InformationType { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Condition { get; set; }
        public string CompareWith { get; set; }
        public char UseAsLabel { get; set; }


        /// <summary>
        /// Indicates whether the metafield is used for the client. Only used for client specific field.
        /// </summary>
        public char Enabled;

        public bool FoundinDB;

        // -----------------------------------------------------
        //   Constructor using userId and connection string
        // -----------------------------------------------------
        public ReportMetadata()
        {
        
        }

        /// <summary>
        /// Returns the value of the metafield
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            string ret = "";
            string select = "";

            if (this.InformationType == Utils.InformationType.IMAGE)
            {
                select = this.Condition;
                // return select;
            }

            if (this.InformationType == Utils.InformationType.VARIABLE)
            {
                select = DateTime.Today.ToString().Substring(0,10);
                return select;
            }


            if (this.InformationType == Utils.InformationType.FIELD)
            {
                select = this.Condition;
            }

            if (string.IsNullOrEmpty(select))
            {
                return "";
            }

            // --------------------------------
            //          Get Variable
            // --------------------------------
            //if (this.CompareWith == "CLIENT.UID")
            //{
            //    // select += Utils.ClientID.ToString();
            //    select += this.ClientUID.ToString();
            //}
            
            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = select;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    if (this.CompareWith == "CLIENT.UID")
                    {
                        command.Parameters.Add("@UID", SqlDbType.BigInt).Value = this.ClientUID;
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = reader[0].ToString();
                    }
                }
            }

            // If it is an image, get the final folder
            //
            if (this.InformationType == Utils.InformationType.IMAGE)
            {
                string logoPathName = Utils.GetPathName(ret);
                ret = logoPathName;
            }
            return ret;
        }


        
        /// <summary>
        /// Retrieve last Report Metadata UID
        /// </summary>
        /// <returns></returns>
        public static int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX([UID]) LASTUID FROM [ReportMetadata]";

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

        /// <summary>
        /// Retrieve last Report Metadata UID
        /// </summary>
        /// <returns></returns>
        public static int GetLastUIDSubTransaction(
                    SqlConnection connection, 
                    SqlTransaction sqltransaction, 
                    HeaderInfo headerInfo)
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            var commandString = "SELECT MAX([UID]) LASTUID FROM [ReportMetadata]";

            // var command = new SqlCommand(commandString, connection, sqltransaction);

            var command = connection.CreateCommand();
            command.CommandText = commandString;
            command.Transaction = sqltransaction;

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
            return LastUID;
        }


        /// <summary>
        /// Add or update record.
        /// </summary>
        public void Save()
        {
            // Check if code value exists.
            // If it exists, update
            // Else Add a new one

            this.Read(true);

            bool results = this.FoundinDB;

            if (results)
            {
                this.Update();
            }
            else
            {
                this.Add();
            }

        }

        /// <summary>
        /// Add or update record. Sub Transactional
        /// </summary>
        public void SaveSubTransaction( SqlConnection connection, 
                                        SqlTransaction sqltransaction, 
                                        HeaderInfo headerInfo)
        {
            // Check if code value exists.
            // If it exists, update
            // Else Add a new one

            this.Read(true);

            bool results = this.FoundinDB;

            if (results)
            {
                this.UpdateSubTransaction(connection, sqltransaction, headerInfo);
            }
            else
            {
                this.AddSubTransaction(connection, sqltransaction, headerInfo);
            }

        }



        /// <summary>
        /// Add new report metadata
        /// </summary>
        private void Add()
        {

            string ret = "Item updated successfully";
            int _uid = 0;

            _uid = GetLastUID() + 1;

            DateTime _now = DateTime.Today;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (

           "INSERT INTO [ReportMetadata] " +
           "( " +
           "  [UID] " +
           " ,[RecordType] " +
           " ,[Description] " +
           " ,[FieldCode] " +
           " ,[ClientType] " +
           " ,[ClientUID] " + 
           " ,[InformationType] " +
           " ,[Condition] " +
           " ,[CompareWith] " +
           " ,[Enabled] " +
           " ,[UseAsLabel] " +
           " ) " +
           " VALUES " +
           " ( " +
           "  @UID " +
           " ,@RecordType " +
           " ,@Description " +
           " ,@FieldCode " +
           " ,@ClientType " +
           " ,@ClientUID " + 
           " ,@InformationType " +
           " ,@Condition " +
           " ,@CompareWith " +
           " ,@Enabled " +
           " ,@UseAsLabel" +
           " )"            
           );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = _uid;
                    command.Parameters.Add("@RecordType", SqlDbType.VarChar).Value = RecordType;
                    command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@FieldCode", SqlDbType.VarChar).Value = FieldCode;
                    command.Parameters.Add("@ClientType", SqlDbType.VarChar).Value = ClientType;
                    command.Parameters.Add("@ClientUID", SqlDbType.BigInt).Value = ClientUID;
                    command.Parameters.Add("@InformationType", SqlDbType.VarChar).Value = InformationType;
                    command.Parameters.Add("@Condition", SqlDbType.VarChar).Value = Condition;
                    command.Parameters.Add("@CompareWith", SqlDbType.VarChar).Value = CompareWith;
                    command.Parameters.Add("@Enabled", SqlDbType.Char).Value = Enabled;
                    command.Parameters.Add("@UseAsLabel", SqlDbType.Char).Value = UseAsLabel;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        /// <summary>
        /// Add new report metadata
        /// </summary>
        private void AddSubTransaction(
                SqlConnection connection, 
                SqlTransaction sqltransaction,
                HeaderInfo headerInfo)
        {

            string ret = "Item updated successfully";
            int _uid = 0;

            _uid = GetLastUIDSubTransaction( connection, sqltransaction, headerInfo ) + 1;

            DateTime _now = DateTime.Today;

            var commandString =
            (
                "INSERT INTO [ReportMetadata] " +
                "( " +
                "  [UID] " +
                " ,[RecordType] " +
                " ,[Description] " +
                " ,[FieldCode] " +
                " ,[ClientType] " +
                " ,[ClientUID] " +
                " ,[InformationType] " +
                " ,[Condition] " +
                " ,[CompareWith] " +
                " ,[Enabled] " +
                " ) " +
                " VALUES " +
                " ( " +
                "  @UID " +
                " ,@RecordType " +
                " ,@Description " +
                " ,@FieldCode " +
                " ,@ClientType " +
                " ,@ClientUID " +
                " ,@InformationType " +
                " ,@Condition " +
                " ,@CompareWith " +
                " ,@Enabled " +
                " )"
                );

            var command = new SqlCommand(commandString, connection, sqltransaction);
            command.Parameters.Add("@UID", SqlDbType.BigInt).Value = _uid;
            command.Parameters.Add("@RecordType", SqlDbType.VarChar).Value = RecordType;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
            command.Parameters.Add("@FieldCode", SqlDbType.VarChar).Value = FieldCode;
            command.Parameters.Add("@ClientType", SqlDbType.VarChar).Value = ClientType;
            command.Parameters.Add("@ClientUID", SqlDbType.BigInt).Value = ClientUID;
            command.Parameters.Add("@InformationType", SqlDbType.VarChar).Value = InformationType;
            command.Parameters.Add("@Condition", SqlDbType.VarChar).Value = Condition;
            command.Parameters.Add("@CompareWith", SqlDbType.VarChar).Value = CompareWith;
            command.Parameters.Add("@Enabled", SqlDbType.Char).Value = Enabled;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogFile.WriteToTodaysLogFile(ex.ToString(), headerInfo.UserID);
            }

            return;
        }


        /// <summary>
        /// Update metadata table
        /// </summary>
        private void Update()
        {

            string ret = "Item updated successfully";
            
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (

           "UPDATE [ReportMetadata] " +
           "SET " +
           "  [FieldCode]        = @FieldCode " + // not setting record type...
           " ,[RecordType]       = @RecordType " +
           " ,[Description]      = @Description " +
           " ,[ClientType]       = @ClientType " +
           " ,[ClientUID]        = @ClientUID " +
           " ,[InformationType]  = @InformationType " +
           " ,[Condition]        = @Condition " +
           " ,[CompareWith]      = @CompareWith " +
           " ,[Enabled]          = @Enabled" +
           " ,[UseAsLabel]       = @UseAsLabel" +

           " WHERE UID  = @UID ");

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FieldCode", SqlDbType.VarChar ).Value = FieldCode;
                    command.Parameters.Add("@RecordType", SqlDbType.VarChar ).Value = RecordType;
                    command.Parameters.Add("@Description", SqlDbType.VarChar ).Value = Description;
                    command.Parameters.Add("@ClientType", SqlDbType.VarChar ).Value = ClientType;
                    command.Parameters.Add("@InformationType", SqlDbType.VarChar ).Value = InformationType;
                    command.Parameters.Add("@ClientUID", SqlDbType.VarChar ).Value = ClientUID;
                    command.Parameters.Add("@UID", SqlDbType.VarChar ).Value = UID;
                    command.Parameters.Add("@Condition", SqlDbType.VarChar ).Value = Condition;
                    command.Parameters.Add("@CompareWith", SqlDbType.VarChar ).Value = CompareWith;
                    command.Parameters.Add("@Enabled", SqlDbType.Char).Value = Enabled;
                    command.Parameters.Add("@UseAsLabel", SqlDbType.Char).Value = UseAsLabel;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        /// <summary>
        /// Update metadata table
        /// </summary>
        private void UpdateSubTransaction(
                    SqlConnection connection, 
                    SqlTransaction sqltransaction,
                    HeaderInfo headerInfo)
        {

            string ret = "Item updated successfully";

            var commandString =
            (
                "UPDATE [ReportMetadata] " +
                "SET " +
                "  [FieldCode]        = @FieldCode " + // not setting record type...
                " ,[RecordType]       = @RecordType " +
                " ,[Description]      = @Description " +
                " ,[ClientType]       = @ClientType " +
                " ,[ClientUID]        = @ClientUID " +
                " ,[InformationType]  = @InformationType " +
                " ,[Condition]        = @Condition " +
                " ,[CompareWith]      = @CompareWith " +
                " ,[Enabled]          = @Enabled" +

                " WHERE UID  = @UID ");

            var command = new SqlCommand(commandString, connection, sqltransaction);
            command.Parameters.Add("@FieldCode", SqlDbType.VarChar).Value = FieldCode;
            command.Parameters.Add("@RecordType", SqlDbType.VarChar).Value = RecordType;
            command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
            command.Parameters.Add("@ClientType", SqlDbType.VarChar).Value = ClientType;
            command.Parameters.Add("@InformationType", SqlDbType.VarChar).Value = InformationType;
            command.Parameters.Add("@ClientUID", SqlDbType.VarChar).Value = ClientUID;
            command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;
            command.Parameters.Add("@Condition", SqlDbType.VarChar).Value = Condition;
            command.Parameters.Add("@CompareWith", SqlDbType.VarChar).Value = CompareWith;
            command.Parameters.Add("@Enabled", SqlDbType.Char).Value = Enabled;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogFile.WriteToTodaysLogFile(ex.ToString(), headerInfo.UserID);
            }


            
            return;
        }


        /// <summary>
        /// Read metadata
        /// </summary>
        /// <param name="CheckOnly"></param>
        private void Read(bool CheckOnly)
        {
            CodeValue ret = null;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = 
                " SELECT [UID] " +
                "      ,[RecordType] " +
                "      ,[FieldCode] " +
                "      ,[Description] " +
                "      ,[ClientType] " +
                "      ,[ClientUID] " +
                "      ,[InformationType] " +
                "      ,[Condition] " +
                "      ,[CompareWith] " +
                "      ,[Enabled] " +
                "      ,[UseAsLabel] " +
                "  FROM [ReportMetadata] " +
                " WHERE UID = @UID ";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();

                    command.Parameters.Add(new SqlParameter("UID", this.UID));

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            if (!CheckOnly)
                            {
                                this.UID = Convert.ToInt32( reader["UID"].ToString() );
                                this.RecordType = reader["RecordType"].ToString();
                                this.FieldCode = reader["FieldCode"].ToString();
                                this.Description = reader["Description"].ToString();
                                this.ClientType = reader["ClientType"].ToString();
                                this.ClientUID = Convert.ToInt32( reader["ClientUID"]);
                                this.InformationType = reader["InformationType"].ToString();
                                this.Condition = reader["Condition"].ToString();
                                this.CompareWith = reader["CompareWith"].ToString();
                                this.Enabled = Convert.ToChar(reader["CompareWith"]);
                                this.UseAsLabel = Convert.ToChar(reader["UseAsLabel"]);
                            }

                            this.FoundinDB = true;
                        }
                        catch (Exception ex)
                        {
                            this.Description = ex.ToString();
                        }
                    }
                    else
                    {
                        this.FoundinDB = false;
                    }
                }
            }

        }


        // ---------------------------------------------
        //            Read
        // ---------------------------------------------
        public bool Read(int clientUID, string fieldCode)
        {

            bool ret = false;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = 
                " SELECT [UID] " +
                "      ,[RecordType] " +
                "      ,[FieldCode] " +
                "      ,[Description] " +
                "      ,[ClientType] " +
                "      ,[ClientUID] " +
                "      ,[InformationType] " +
                "      ,[Condition] " +
                "      ,[CompareWith] " +
                "      ,[Enabled] " +
                "      ,[UseAsLabel] " +
                "  FROM [ReportMetadata] " +
                " WHERE ClientUID = @CLIENTUID " +
                "   AND FieldCode = @FIELDCODE ";

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();

                    command.Parameters.Add(new SqlParameter("@CLIENTUID", clientUID));
                    command.Parameters.Add(new SqlParameter("@FIELDCODE", fieldCode));

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            this.UID = Convert.ToInt32(reader["UID"].ToString());
                            this.RecordType = reader["RecordType"].ToString();
                            this.FieldCode = reader["FieldCode"].ToString();
                            this.Description = reader["Description"].ToString();
                            this.ClientType = reader["ClientType"].ToString();
                            this.ClientUID = Convert.ToInt32(reader["ClientUID"]);
                            this.InformationType = reader["InformationType"].ToString();
                            this.Condition = reader["Condition"].ToString();
                            this.CompareWith = reader["CompareWith"].ToString();
                            this.Enabled = Convert.ToChar(reader["Enabled"]);
                            this.UseAsLabel = Convert.ToChar(reader["UseAsLabel"]);

                            ret = true;
                        }
                        catch (Exception ex)
                        {
                            this.Description = ex.ToString();
                        }
                    }
                }

                return ret;

            }

        }

        // ---------------------------------------------
        //            Read
        // ---------------------------------------------
        public bool ReadSubTransaction(
            int clientUID,
            string fieldCode,
            SqlConnection connection,
            SqlTransaction sqltransaction,
            HeaderInfo headerInfo)
        {

            bool ret = false;

            // 
            // EA SQL database
            // 

            var commandString = string.Format(
            " SELECT [UID] " +
            "      ,[RecordType] " +
            "      ,[FieldCode] " +
            "      ,[Description] " +
            "      ,[ClientType] " +
            "      ,[ClientUID] " +
            "      ,[InformationType] " +
            "      ,[Condition] " +
            "      ,[CompareWith] " +
            "      ,[Enabled] " +
            "      ,[UseAsLabel] " +
            "  FROM [ReportMetadata] " +
            " WHERE ClientUID = {0} " +
            "   AND FieldCode = '{1}' ",
            clientUID,
            fieldCode);

            var command = new SqlCommand(commandString, connection, sqltransaction);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                try
                {
                    this.UID = Convert.ToInt32(reader["UID"].ToString());
                    this.RecordType = reader["RecordType"].ToString();
                    this.FieldCode = reader["FieldCode"].ToString();
                    this.Description = reader["Description"].ToString();
                    this.ClientType = reader["ClientType"].ToString();
                    this.ClientUID = Convert.ToInt32(reader["ClientUID"]);
                    this.InformationType = reader["InformationType"].ToString();
                    this.Condition = reader["Condition"].ToString();
                    this.CompareWith = reader["CompareWith"].ToString();
                    this.Enabled = Convert.ToChar(reader["Enabled"]);
                    this.UseAsLabel = Convert.ToChar(reader["UseAsLabel"]);

                    ret = true;
                }
                catch (Exception ex)
                {
                    this.Description = ex.ToString();
                }
            }

            return ret;

        }

        // ---------------------------------------------
        //            Read
        // ---------------------------------------------
        public void FindMatch(int clientUID, string rmdItem)
        {
            CodeValue ret = null;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT [UID] " +
                "      ,[RecordType] " +
                "      ,[FieldCode] " +
                "      ,[Description] " +
                "      ,[ClientType] " +
                "      ,[ClientUID] " +
                "      ,[InformationType] " +
                "  FROM [ReportMetadata] " +
                " WHERE " + 
                "       ClientUID =  {0} " +
                "   AND FieldCode = '{1}' ",
                clientUID,
                rmdItem);

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
                            this.RecordType = reader["RecordType"].ToString();
                            this.FieldCode = reader["FieldCode"].ToString();
                            this.Description = reader["Description"].ToString();
                            this.ClientType = reader["ClientType"].ToString();
                            this.ClientUID = Convert.ToInt32(reader["ClientUID"]);
                            this.InformationType = reader["InformationType"].ToString();

                            this.FoundinDB = true;
                        }
                        catch (Exception ex)
                        {
                            this.Description = ex.ToString();
                        }
                    }
                    else
                    {
                        this.FoundinDB = false;
                    }
                }
            }

        }


        // ---------------------------------------------
        //            Update metadata table
        // ---------------------------------------------
        public bool Delete()
        {

            if (UID <= 0)
                return false;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (

           "DELETE [ReportMetadata] " +
           " WHERE UID  = @UID ");

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.VarChar).Value = UID;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public struct MetadataFieldCode
        {
            public const string COMPANYLOGO = "COMPANYLOGO";
        }

    }
}
