using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.ReferenceData
{
    public class RelatedCodeValue
    {
        public string FKRelatedCodeID { get; set; }
        public string FKCodeTypeFrom { get; set; }
        public string FKCodeValueFrom { get; set; }
        public string FKCodeTypeTo { get; set; }
        public string FKCodeValueTo { get; set; }

        public void Add()
        {

            DateTime _now = DateTime.Today;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [RelatedCodeValue] " +
                   "([FKRelatedCodeID], [FKCodeTypeFrom], [FKCodeValueFrom], [FKCodeTypeTo],[FKCodeValueTo]" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @FKRelatedCodeID      " +
                   ", @FKCodeTypeFrom   " +
                   ", @FKCodeValueFrom " +
                   ", @FKCodeTypeTo " +
                   ", @FKCodeValueTo " +
                   " )"

                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRelatedCodeID", SqlDbType.VarChar).Value = FKRelatedCodeID;
                    command.Parameters.Add("@FKCodeTypeFrom", SqlDbType.VarChar).Value = FKCodeTypeFrom;
                    command.Parameters.Add("@FKCodeValueFrom", SqlDbType.VarChar).Value = FKCodeValueFrom;
                    command.Parameters.Add("@FKCodeTypeTo", SqlDbType.VarChar).Value = FKCodeTypeTo;
                    command.Parameters.Add("@FKCodeValueTo", SqlDbType.VarChar).Value = FKCodeValueTo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public void Delete()
        {

            DateTime _now = DateTime.Today;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "DELETE FROM [RelatedCodeValue] " +
                   " WHERE " +
                   "  FKRelatedCodeID  = @FKRelatedCodeID  " +
                   ", FKCodeTypeFrom = @FKCodeTypeFrom   " +
                   ", FKCodeValueFrom  = @FKCodeValueFrom " +
                   ", FKCodeTypeTo = @FKCodeTypeTo " +
                   ", FKCodeValueTo = @FKCodeValueTo " +
                   " )"

                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRelatedCodeID", SqlDbType.VarChar).Value = FKRelatedCodeID;
                    command.Parameters.Add("@FKCodeTypeFrom", SqlDbType.VarChar).Value = FKCodeTypeFrom;
                    command.Parameters.Add("@FKCodeValueFrom", SqlDbType.VarChar).Value = FKCodeValueFrom;
                    command.Parameters.Add("@FKCodeTypeTo", SqlDbType.VarChar).Value = FKCodeTypeTo;
                    command.Parameters.Add("@FKCodeValueTo", SqlDbType.VarChar).Value = FKCodeValueTo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        /// <summary>
        /// Retrieve list of related codes for give related code type, code type, code value
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public static List<RelatedCodeValue> ListS(string RelatedCodeID, string CodeTypeFrom, string CodeValueFrom)
        {
            List<RelatedCodeValue> list = new List<RelatedCodeValue>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  [FKRelatedCodeID] " +
                " ,[FKCodeTypeFrom]   " +
                " ,[FKCodeValueFrom] " +
                " ,[FKCodeTypeTo] " +
                " ,[FKCodeValueTo] " +
                "   FROM [RelatedCodeValue] " +
                " WHERE FKRelatedCodeID = '{0}' " +
                "   AND CodeTypeFrom = '{1}' " +
                "   AND CodeValueFrom = '{2}' " 
                , RelatedCodeID
                , CodeTypeFrom
                , CodeValueFrom);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var _codeValue = new RelatedCodeValue();
                            _codeValue.FKRelatedCodeID = reader["FKRelatedCodeID"].ToString();
                            _codeValue.FKCodeTypeFrom = reader["FKCodeTypeFrom"].ToString();
                            _codeValue.FKCodeValueFrom = reader["FKCodeValueFrom"].ToString();
                            _codeValue.FKCodeTypeTo = reader["FKCodeTypeTo"].ToString();
                            _codeValue.FKCodeValueTo = reader["FKCodeValueTo"].ToString();

                            list.Add(_codeValue);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// Retrieve list of related codes for give related code type, code type, code value
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public static List<RelatedCodeValue> ListAllS()
        {
            List<RelatedCodeValue> list = new List<RelatedCodeValue>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  [FKRelatedCodeID] " +
                " ,[FKCodeTypeFrom]   " +
                " ,[FKCodeValueFrom] " +
                " ,[FKCodeTypeTo] " +
                " ,[FKCodeValueTo] " +
                "   FROM [RelatedCodeValue] " +
                " ORDER BY FKRelatedCodeID, FKCodeTypeFrom, FKCodeValueFrom " 
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var _codeValue = new RelatedCodeValue();
                            _codeValue.FKRelatedCodeID = reader["FKRelatedCodeID"].ToString();
                            _codeValue.FKCodeTypeFrom = reader["FKCodeTypeFrom"].ToString();
                            _codeValue.FKCodeValueFrom = reader["FKCodeValueFrom"].ToString();
                            _codeValue.FKCodeTypeTo = reader["FKCodeTypeTo"].ToString();
                            _codeValue.FKCodeValueTo = reader["FKCodeValueTo"].ToString();

                            list.Add(_codeValue);
                        }
                    }
                }
            }

            return list;

        }

    
    }
}
