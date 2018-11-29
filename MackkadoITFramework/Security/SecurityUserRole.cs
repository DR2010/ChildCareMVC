using System;
using System.Collections.Generic;
using System.ComponentModel;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.Security
{
    public class SecurityUserRole
    {

        public int UniqueID { get; set; }
        public string FK_UserID { get; set; }
        public string FK_Role { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string IsActive { get; set; }
        public string IsVoid { get; set; }
        private HeaderInfo _headerInfo;
        public List<SecurityUserRole> securityUserRoles { get; set; }
        public string ListForUserID { get; set; }

        public struct FieldName
        {
            public const string UniqueID = "UniqueID";
            public const string FK_UserID = "FK_UserID";
            public const string FK_Role = "FK_Role";
            public const string IsActive = "IsActive";
        }

        public SecurityUserRole()
        {
        }

        public SecurityUserRole(HeaderInfo headerInfo)
        {
            _headerInfo = headerInfo;
        }

        public bool UserHasAccessToRole(string userid, string roleToCheck )
        {
            var roleList = UserRoleList(userid);
            foreach (var role in roleList)
            {
                if (role.FK_Role == roleToCheck)
                    return true;
            }

            return false;
        }

        public List<SecurityUserRole> UserRoleList( string userid )
        {
            List<SecurityUserRole> rolelist = new List<SecurityUserRole>();
            securityUserRoles = new List<SecurityUserRole>();

            using (var connection = new MySqlConnection( ConnString.ConnectionStringFramework ))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  UniqueID " +
                " ,FK_UserID   " +
                " ,FK_Role " +
                " ,StartDate " +
                " ,EndDate " +
                " ,IsActive " +
                " ,IsVoid " +
                "   FROM SecurityUserRole " +
                " WHERE FK_UserID = '{0}' AND IsActive='Y' ", userid );

                using (var command = new MySqlCommand(
                                      commandString, connection ))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SecurityUserRole SecurityUserRole = new SecurityUserRole(_headerInfo);
                            SecurityUserRole.UniqueID = Convert.ToInt32( reader["UniqueID"].ToString() );
                            SecurityUserRole.FK_UserID = reader["FK_UserID"].ToString();
                            SecurityUserRole.FK_Role = reader["FK_Role"].ToString();
                            SecurityUserRole.StartDate = Convert.ToDateTime( reader["StartDate"].ToString() );
                            SecurityUserRole.EndDate = Convert.ToDateTime( reader["EndDate"] );
                            SecurityUserRole.IsActive = reader["IsActive"].ToString();
                            SecurityUserRole.IsVoid = reader["IsVoid"].ToString();

                            rolelist.Add( SecurityUserRole );
                            securityUserRoles.Add( SecurityUserRole );
                        }
                    }
                }
            }

            return rolelist;
        }

        // -----------------------------------------------------
        //   Retrieve last Document Set id
        // -----------------------------------------------------
        private int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString =
                    "SELECT MAX(UniqueID) LASTUID FROM SecurityUserRole";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

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
        /// Add Role to User
        /// </summary>
        public ResponseStatus Add()
        {

            ResponseStatus ret = new ResponseStatus();

            ret.Message = "Item added successfully.";
            ret.ReturnCode = 0001;
            ret.ReasonCode = 0001;
            ret.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000002;

            int nextID = GetLastUID() + 1;
            StartDate = System.DateTime.Today;
            EndDate = System.DateTime.MaxValue;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO SecurityUserRole " +
                   "(UniqueID, FK_UserID, FK_Role, StartDate, EndDate, IsActive, IsVoid)" +
                        " VALUES " +
                   "( " +
                   "  @UniqueID  " +
                   ", @FK_UserID " +
                   ", @FK_Role   " +
                   ", @StartDate " +
                   ", @EndDate   " +
                   ", @IsActive  " +
                   ", @IsVoid    " +
                   " )"

                   );


                /* 
                 INSERT INTO SecurityUserRole 
                        (UniqueID, FK_UserID, FK_Role, StartDate, EndDate, IsActive, IsVoid ) 
                        VALUES (   1  , "DM0001", "ADMIN", '2012-02-19', '2012-12-31', "Y", "N" )

                 */

                using (var command = new MySqlCommand(commandString, connection))
                {
                    command.Parameters.AddWithValue("@UniqueID", nextID);
                    command.Parameters.AddWithValue("@FK_UserID", FK_UserID);
                    command.Parameters.AddWithValue("@FK_Role", FK_Role);
                    command.Parameters.AddWithValue("@StartDate", StartDate);
                    command.Parameters.AddWithValue("@EndDate", EndDate);
                    command.Parameters.AddWithValue("@IsActive", "Y");
                    command.Parameters.AddWithValue("@IsVoid", "N");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return ret;

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
            ret.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000003;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "DELETE FROM SecurityUserRole " +
                   "  WHERE  " +
                   "        UniqueID = @UniqueID    " +
                   "    "
                );


                try
                {
                    using (var command = new MySqlCommand(
                                                commandString, connection))
                    {

                        command.Parameters.Add("@UniqueID", MySqlDbType.Int32).Value = UniqueID;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                    LogFile.WriteToTodaysLogFile( ex.ToString(), _headerInfo.UserID, "", "SecurityUserRole.cs");
                    
                    return new ResponseStatus(MessageType.Error)
                               {
                                   ReturnCode = -0010, ReasonCode = 0001, 
                                   Message = "Error deleting user role.",
                                   UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000002
                               };
                }
            }
            return ret;
        }

        /// <summary>
        /// List user settings for a given user
        /// </summary>
        /// <returns></returns>
        public ResponseStatus ListRoleForUser(string userID, List<SecurityUserRole> roleList)
        {
            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString = string.Format(
                " SELECT " +
                FieldName.FK_UserID + "," +
                FieldName.FK_Role + " " +
                "   FROM SecurityUserRole " +
                "  WHERE FK_UserID = '{0}' "+
                "   ORDER BY FK_Role ASC ", userID
                );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userRole = new SecurityUserRole(_headerInfo);

                                userRole.FK_UserID = reader[FieldName.FK_UserID].ToString();
                                userRole.FK_Role = reader[FieldName.FK_Role].ToString();

                                // Check if document exists
                                //

                                roleList.Add(userRole);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(ex.ToString(), HeaderInfo.Instance.UserID);
                    }
                }
            }

            return new ResponseStatus();

        }

        /// <summary>  
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        private static string SQLConcat()
        {
            string ret = " " +
            FieldName.FK_UserID + "," +
            FieldName.FK_Role + " ";

            return ret;
        }

        

    }
}
