using System;
using System.Collections.Generic;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.ReferenceData
{
    public class RelatedCode
    {
        public string RelatedCodeID { get; set; }
        public string Description { get; set; }
        public string FKCodeTypeFrom { get; set; }
        public string FKCodeTypeTo { get; set; }

        public void Add()
        {

            string ret = "Item updated successfully";
            int _uid = 0;

            DateTime _now = DateTime.Today;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO rdRelatedCode " +
                   "(RelatedCodeID, Description, FKCodeTypeFrom, FKCodeTypeTo" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @RelatedCodeID      " +
                   ", @Description   " +
                   ", @FKCodeTypeFrom " +
                   ", @FKCodeTypeTo " +
                   " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@RelatedCodeID", MySqlDbType.VarChar).Value = RelatedCodeID;
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@FKCodeTypeFrom", MySqlDbType.VarChar).Value = FKCodeTypeFrom;
                    command.Parameters.Add("@FKCodeTypeTo", MySqlDbType.VarChar).Value = FKCodeTypeTo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        public static List<RelatedCode> List()
        {
            List<RelatedCode> listRelcode = new List<RelatedCode>();

            try
            {
                using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
                {

                    var commandString = string.Format(
                    " SELECT  " +
                    "  RelatedCodeID " +
                    " ,Description " +
                    " ,FKCodeTypeFrom " +
                    " ,FKCodeTypeTo " +
                    "   FROM rdRelatedCode ");

                    using (var command = new MySqlCommand(
                                          commandString, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RelatedCode _codeType = new RelatedCode();
                                _codeType.RelatedCodeID = reader["RelatedCodeID"].ToString();
                                _codeType.Description = reader["Description"].ToString();
                                _codeType.FKCodeTypeFrom = reader["FKCodeTypeFrom"].ToString();
                                _codeType.FKCodeTypeTo = reader["FKCodeTypeTo"].ToString();

                                listRelcode.Add(_codeType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteToTodaysLogFile(
                    "Related Code List Error:  " +   ex.ToString(),
                    Helper.Utils.UserID,
                    "RelatedCode.cs");
            }

            return listRelcode;
        }
    }
}
