using System;
using System.Collections.Generic;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.ReferenceData
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

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO rdRelatedCodeValue " +
                   "(FKRelatedCodeID, FKCodeTypeFrom, FKCodeValueFrom, FKCodeTypeTo,FKCodeValueTo" +
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

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRelatedCodeID", MySqlDbType.VarChar).Value = FKRelatedCodeID;
                    command.Parameters.Add("@FKCodeTypeFrom", MySqlDbType.VarChar).Value = FKCodeTypeFrom;
                    command.Parameters.Add("@FKCodeValueFrom", MySqlDbType.VarChar).Value = FKCodeValueFrom;
                    command.Parameters.Add("@FKCodeTypeTo", MySqlDbType.VarChar).Value = FKCodeTypeTo;
                    command.Parameters.Add("@FKCodeValueTo", MySqlDbType.VarChar).Value = FKCodeValueTo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public void Delete()
        {

            DateTime _now = DateTime.Today;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "DELETE FROM rdRelatedCodeValue " +
                   " WHERE " +
                   "  FKRelatedCodeID  = @FKRelatedCodeID  " +
                   ", FKCodeTypeFrom = @FKCodeTypeFrom   " +
                   ", FKCodeValueFrom  = @FKCodeValueFrom " +
                   ", FKCodeTypeTo = @FKCodeTypeTo " +
                   ", FKCodeValueTo = @FKCodeValueTo " +
                   " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRelatedCodeID", MySqlDbType.VarChar).Value = FKRelatedCodeID;
                    command.Parameters.Add("@FKCodeTypeFrom", MySqlDbType.VarChar).Value = FKCodeTypeFrom;
                    command.Parameters.Add("@FKCodeValueFrom", MySqlDbType.VarChar).Value = FKCodeValueFrom;
                    command.Parameters.Add("@FKCodeTypeTo", MySqlDbType.VarChar).Value = FKCodeTypeTo;
                    command.Parameters.Add("@FKCodeValueTo", MySqlDbType.VarChar).Value = FKCodeValueTo;

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

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  FKRelatedCodeID " +
                " ,FKCodeTypeFrom   " +
                " ,FKCodeValueFrom " +
                " ,FKCodeTypeTo " +
                " ,FKCodeValueTo " +
                "   FROM rdRelatedCodeValue " +
                " WHERE FKRelatedCodeID = '{0}' " +
                "   AND FKCodeTypeFrom = '{1}' " +
                "   AND FKCodeValueFrom = '{2}' " 
                , RelatedCodeID
                , CodeTypeFrom
                , CodeValueFrom);

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
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

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  FKRelatedCodeID " +
                " ,FKCodeTypeFrom   " +
                " ,FKCodeValueFrom " +
                " ,FKCodeTypeTo " +
                " ,FKCodeValueTo " +
                "   FROM rdRelatedCodeValue " +
                " ORDER BY FKRelatedCodeID, FKCodeTypeFrom, FKCodeValueFrom " 
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
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
