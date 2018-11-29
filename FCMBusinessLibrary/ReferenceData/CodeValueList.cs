using System.Collections.Generic;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.ReferenceData
{
    public class CodeValueList
    {
        public List<CodeValue> codeValueList;
        public List<string> codeValueIDList;

        // -----------------------------------------------------
        //   Constructor using userId and connection string
        // -----------------------------------------------------
        public CodeValueList()
        {
        }

        public void ListInCombo(string codeType, System.Windows.Forms.ComboBox input)
        {
            this.List(codeType);

            foreach (CodeValue cv in this.codeValueList)
            {
                input.Items.Add(cv.ID);
            }

            return;

        }

        public void ListInCombo(bool IDDesc, string codeType, System.Windows.Forms.ComboBox input)
        {
            this.List(codeType);

            foreach (CodeValue cv in this.codeValueList)
            {
                input.Items.Add(cv.ID + ";" + cv.Description);
            }

            return;

        }

        //
        // Public Methods
        //
        public void List(string codeType)
        {
            this.codeValueList = new List<CodeValue>();
            this.codeValueIDList= new List<string>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  [FKCodeType] " +
                " ,[ID]   " +
                " ,[Description] " +
                " ,[Abbreviation] " +
                " ,[ValueExtended] " +
                "   FROM [CodeValue] " +
                " WHERE FKCodeType = '{0}'" , codeType  );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"].ToString();
                            _codeValue.ID = reader["ID"].ToString();
                            _codeValue.Description = reader["Description"].ToString();
                            _codeValue.Abbreviation = reader["Abbreviation"].ToString();
                            _codeValue.ValueExtended = reader["ValueExtended"].ToString();

                            this.codeValueList.Add(_codeValue);

                            this.codeValueIDList.Add(_codeValue.ID);

                        }
                    }
                }
            }

        }


        //
        // Public Methods
        //
        /// <summary>
        /// Return a list of codes for a given code type.
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public static List<CodeValue> ListS(string codeType)
        {
            List<CodeValue> list = new List<CodeValue>(); 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  [FKCodeType] " +
                " ,[ID]   " +
                " ,[Description] " +
                " ,[Abbreviation] " +
                " ,[ValueExtended] " +
                "   FROM [CodeValue] " +
                " WHERE FKCodeType = '{0}'", codeType);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"].ToString();
                            _codeValue.ID = reader["ID"].ToString();
                            _codeValue.Description = reader["Description"].ToString();
                            _codeValue.Abbreviation = reader["Abbreviation"].ToString();
                            _codeValue.ValueExtended = reader["ValueExtended"].ToString();

                            list.Add(_codeValue);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// Return a list of codes for a given code type.
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public static List<CodeValue> ListS()
        {
            List<CodeValue> list = new List<CodeValue>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  [FKCodeType] " +
                " ,[ID]   " +
                " ,[Description] " +
                " ,[Abbreviation] " +
                " ,[ValueExtended] " +
                "   FROM [CodeValue] " +
                " ORDER BY FKCodeType " ) ;

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"].ToString();
                            _codeValue.ID = reader["ID"].ToString();
                            _codeValue.Description = reader["Description"].ToString();
                            _codeValue.Abbreviation = reader["Abbreviation"].ToString();
                            _codeValue.ValueExtended = reader["ValueExtended"].ToString();

                            list.Add(_codeValue);
                        }
                    }
                }
            }

            return list;

        }

    }
}
