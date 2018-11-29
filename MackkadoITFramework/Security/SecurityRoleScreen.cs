using System;
using System.Collections.Generic;
using MackkadoITFramework.Utils;
using MackkadoITFramework.ErrorHandling;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.Security
{
    public class SecurityRoleScreen
    {
        public string FKRoleCode { get; set; }
        public string FKScreenCode { get; set; }

        public ResponseStatus Add()
        {

            ResponseStatus response = new ResponseStatus();
            response.Message = "Role Screen Linked Successfully.";
            response.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;

            DateTime _now = DateTime.Today;

            if (FKRoleCode == null && FKScreenCode == null)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "Role and Screen codes are mandatory.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000001;
                response.Contents = 0;
                return response;
            }


            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO SecurityRoleScreen " +
                   "(FKRoleCode, FKScreenCode" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @FKRoleCode        " +
                   ", @FKScreenCode " +
                   " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKRoleCode", MySqlDbType.VarChar).Value = FKRoleCode;
                    command.Parameters.Add("@FKScreenCode", MySqlDbType.VarChar).Value = FKScreenCode;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }

        /// <summary>
        /// Delete User Role
        /// </summary>
        /// <param name="userRoleUniqueID"></param>
        /// <returns></returns>
        public ResponseStatus Delete()
        {
            ResponseStatus ret = new ResponseStatus();

            ret.Message = "User role deleted successfully";
            ret.ReturnCode = 0001;
            ret.ReasonCode = 0001;
            ret.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000001;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "DELETE FROM SecurityRoleScreen " +
                   "  WHERE  " +
                   "        FKRoleCode = @FKRoleCode    " +
                   "    "
                );


                try
                {
                    using (var command = new MySqlCommand(
                                                commandString, connection))
                    {

                        command.Parameters.Add("@FKRoleCode", MySqlDbType.Int32).Value = FKRoleCode;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ret.Message = "Error deleting screen/role association.";
                    ret.ReturnCode = -0010;
                    ret.ReasonCode = 0001;
                    ret.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000001;
                }
            }
            return ret;
        }

        /// <summary>
        /// List employees
        /// </summary>
        /// <param name="clientID"></param>
        public static List<SecurityRoleScreen> List(string role)
        {
            List<SecurityRoleScreen> roleList = new List<SecurityRoleScreen>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT " +
                " FKRoleCode, " +
                " FKScreenCode " +
                "   FROM SecurityRoleScreen " +
                "   WHERE  FKRoleCode = '{0}'",
                role);

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SecurityRoleScreen rolescreen = new SecurityRoleScreen();
                            rolescreen.FKRoleCode = reader["FKRoleCode"].ToString();
                            rolescreen.FKScreenCode = reader["FKScreenCode"].ToString();

                            roleList.Add(rolescreen);
                        }
                    }
                }
            }

            return roleList;
        }
    }
}
