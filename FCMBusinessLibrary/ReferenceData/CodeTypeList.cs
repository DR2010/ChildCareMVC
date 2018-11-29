using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.ReferenceData
{
    public class CodeTypeList
    {
        public List<CodeType> codeTypeList { get; set; }

        /// <summary>
        /// List
        /// </summary>
        public void List()
        {
            this.codeTypeList = new List<CodeType>();
            IEnumerator list = null;

            try
            {
                using (var connection = new SqlConnection(ConnString.ConnectionString))
                {

                    var commandString = string.Format(
                    " SELECT  " +
                    "  [CodeType] " +
                    " ,[Description] " +
                    "   FROM [CodeType] ");

                    using (var command = new SqlCommand(
                                          commandString, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            list = new DbEnumerator(reader);

                            while (reader.Read())
                            {
                                CodeType _codeType = new CodeType();
                                _codeType.Code = reader["CodeType"].ToString();
                                _codeType.Description = reader["Description"].ToString();

                                this.codeTypeList.Add(_codeType);
                                
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}