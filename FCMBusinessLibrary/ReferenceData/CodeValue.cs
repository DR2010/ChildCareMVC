using System;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.ReferenceData
{
    public class CodeValue
    {

        public string FKCodeType { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string ValueExtended { get; set; }
        public bool FoundinDB { get; set; }

        
        // -----------------------------------------------------
        //   Constructor using userId and connection string
        // -----------------------------------------------------
        public CodeValue()
        {
        }

        // Public Methods
        //

        // 
        // Add or Update a record
        //
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

        // Delete record
        // 
        public void Delete()
        {

        }

        public static string GetCodeValueExtended(string iCodeType, string iCodeValueID)
        {
            string ret = "";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "       [ValueExtended] " +
                "  FROM [CodeValue]" +
                " WHERE FKCodeType = '{0}' " +
                "   AND ID         = '{1}' ",
                iCodeType,
                iCodeValueID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            ret = reader["ValueExtended"].ToString();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return ret;
        }



        public static string GetCodeValueDescription(string iCodeType, string iCodeValueID)
        {
            string ret = "";
            
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "       [Description] " +
                "  FROM [CodeValue]" +
                " WHERE FKCodeType = '{0}' " +
                "   AND ID         = '{1}' ",
                iCodeType,
                iCodeValueID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            ret = reader["Description"].ToString();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return ret;
        }

        public CodeValue Read(bool checkOnly)
        {
            CodeValue ret = null;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT [FKCodeType] " +
                "      ,[ID] " +
                "      ,[Description] " +
                "      ,[Abbreviation] " +
                "      ,[ValueExtended] " +
                "  FROM [CodeValue]" +
                " WHERE FKCodeType = '{0}' " +
                "   AND ID         = '{1}' ",
                this.FKCodeType,
                this.ID);

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            if (!checkOnly)
                            {
                                this.FKCodeType = reader["FKCodeType"].ToString();
                                this.ID = reader["ID"].ToString();
                                this.Description = reader["Description"].ToString();
                                this.Abbreviation = reader["Abbreviation"].ToString();
                                this.ValueExtended = reader["ValueExtended"].ToString();
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

            return ret;

        }

        //
        // 
        //
        public void Add()
        {

            string ret = "Item updated successfully";
            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (FKCodeType == null)
                return;

            if (ID == null)
                return;

            if (Description == null)
                return;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [CodeValue] ( " +
                   "[FKCodeType], "+
                   "[ID], " + 
                   "[Description], " +
                   "[Abbreviation], " +
                   "[ValueExtended]" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @FKCodeType    " +
                   ", @ID            " +
                   ", @Description   " +
                   ", @Abbreviation  " +
                   ", @ValueExtended " +
                   " )"

                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKCodeType", SqlDbType.VarChar).Value = FKCodeType;
                    command.Parameters.Add("@ID", SqlDbType.VarChar).Value = ID;
                    command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@Abbreviation", SqlDbType.VarChar).Value = Abbreviation;
                    command.Parameters.Add("@ValueExtended", SqlDbType.VarChar).Value = ValueExtended;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public void Update()
        {
            string ret = "Item updated successfully";
            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (FKCodeType == null)
                return;

            if (ID == null)
                return;

            if (Description == null)
                return;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format
                (
                   "UPDATE [CodeValue]  " +
                   "SET " +
                   "   [Description]  = '{0}', " +
                   "   [Abbreviation] = '{1}', " +
                   "   [ValueExtended]= '{2}'  " +
                   "WHERE   " +
                   "      [FKCodeType]  = '{3}' " +
                   "  AND [ID]          = '{4}' ",
                   Description, 
                   Abbreviation ,
                   ValueExtended, 
                   FKCodeType,
                   ID
                 );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }
    }
}
