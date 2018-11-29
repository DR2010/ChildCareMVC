using System;
using System.Collections.Generic;
using MackkadoITFramework.Utils;
using MackkadoITFramework.ErrorHandling;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.Security
{
    public class SecurityRole
    {

        #region Properties
        public string Role {get;set;}
        public string Description{get;set;}
        #endregion Properties

        #region FieldName
        public struct FieldName
        {
            public const string Role = "Role";
            public const string Description = "Description";
        }
        #endregion FieldName

        /// <summary>
        /// List user settings for a given user
        /// </summary>
        /// <returns></returns>
        public static List<SecurityRole> List()
        {
            List<SecurityRole> roleList = new List<SecurityRole>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString = string.Format(
                " SELECT " +
                SQLConcat() +
                "   FROM SecurityRole " +
                "   ORDER BY Role ASC "
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            var docItem = SetDocumentItem(reader);

                            // Check if document exists
                            //

                            roleList.Add(docItem);
                        }
                    }
                }
            }

            return roleList;

        }

        /// <summary>
        /// Set user setting values
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static SecurityRole SetDocumentItem(MySqlDataReader reader)
        {
            var role = new SecurityRole();

            role.Role = reader[FieldName.Role].ToString();
            role.Description = reader[FieldName.Description].ToString();

            return role;
        }

        /// <summary>  
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        private static string SQLConcat()
        {
            string ret = " " +
            FieldName.Role + "," +
            FieldName.Description + " ";

            return ret;
        }

        /// <summary>
        /// Add role
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Add()
        {

            ResponseStatus response = new ResponseStatus();
            response.Message = "Role Added Successfully.";
            response.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;

            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (Role == null)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "Role name is mandatory.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000008;
                response.Contents = 0;
                return response;
            }


            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO SecurityRole " +
                   "(Role, Description" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @Role        " +
                   ", @Description " +
                   " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@Role", MySqlDbType.VarChar).Value = Role;
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Description;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }

    }
}
