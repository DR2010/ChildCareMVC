using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace FCMBusinessLibrary
{
    public class FCMUserRole
    {
        private int _UniqueID;
        private string _FK_UserID;
        private string _FK_Role;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private char _IsActive;
        private char _IsVoid;

        public int UniqueID { get{ return _UniqueID; } set  { _UniqueID= value;} }
        public string FK_UserID { get { return _FK_UserID; } set { _FK_UserID = value; } }
        public string FK_Role { get { return _FK_Role; } set { _FK_Role = value; } }
        public DateTime StartDate { get { return _StartDate; } set { _StartDate = value; } }
        public DateTime EndDate { get { return _EndDate; } set { _EndDate = value; } }
        public char IsActive { get { return _IsActive; } set { _IsActive = value; } }
        public char IsVoid { get { return _IsVoid; } set { _IsVoid = value; } }

        public struct FieldName
        {
            public const string UniqueID = "UniqueID";
            public const string FK_UserID = "FK_UserID";
            public const string FK_Role = "FK_Role";
            public const string IsActive = "IsActive";
        }


        public List<FCMUserRole> UserRoleList( string userid )
        {
            List<FCMUserRole> rolelist = new List<FCMUserRole>();

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  [UniqueID] " +
                " ,[FK_UserID]   " +
                " ,[FK_Role] " +
                " ,[StartDate] " +
                " ,[EndDate] " +
                " ,[IsActive] " +
                " ,[IsVoid] " +
                "   FROM [FCMUserRole] " +
                " WHERE FK_UserID = '{0}'", userid );

                using (var command = new SqlCommand(
                                      commandString, connection ))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FCMUserRole fcmUserRole = new FCMUserRole();
                            fcmUserRole.UniqueID = Convert.ToInt32( reader["UniqueID"].ToString() );
                            fcmUserRole.FK_UserID = reader["FK_UserID"].ToString();
                            fcmUserRole.FK_Role = reader["FK_Role"].ToString();
                            fcmUserRole.StartDate = Convert.ToDateTime( reader["StartDate"].ToString() );
                            fcmUserRole.EndDate = Convert.ToDateTime( reader["EndDate"] );
                            fcmUserRole.IsActive = Convert.ToChar( reader["IsActive"]);
                            fcmUserRole.IsVoid = Convert.ToChar( reader["IsVoid"]);

                            rolelist.Add( fcmUserRole );
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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                    "SELECT MAX([UniqueID]) LASTUID FROM [FCMUserRole]";

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [FCMUserRole] " +
                   "([UniqueID], [FK_UserID], [FK_Role], [StartDate], [EndDate], [IsActive], [IsVoid] " +
                   ")" +
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

                using (var command = new SqlCommand(commandString, connection))
                {
                    command.Parameters.Add("@UniqueID", SqlDbType.VarChar).Value = nextID;
                    command.Parameters.Add("@FK_UserID", SqlDbType.VarChar).Value = _FK_UserID;
                    command.Parameters.Add("@FK_Role", SqlDbType.VarChar).Value = _FK_Role;
                    command.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = _StartDate;
                    command.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = _EndDate;
                    command.Parameters.Add("@IsActive", SqlDbType.Char).Value = _IsActive;
                    command.Parameters.Add("@IsVoid  ", SqlDbType.Char).Value = _IsVoid;

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "DELETE FROM [FCMUserRole] " +
                   "  WHERE  " +
                   "        UniqueID = @UniqueID    " +
                   "    "
                );


                try
                {
                    using (var command = new SqlCommand(
                                                commandString, connection))
                    {

                        command.Parameters.Add("@UniqueID", SqlDbType.BigInt).Value = _UniqueID;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    ret.Message = "Error deleting user role.";
                    ret.ReturnCode = -0010;
                    ret.ReasonCode = 0001;
                    ret.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000002;
                }
            }
            return ret;
        }

        /// <summary>
        /// List user settings for a given user
        /// </summary>
        /// <returns></returns>
        public static List<FCMUserRole> ListRoleForUser(string userID)
        {
            var roleList = new List<FCMUserRole>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT " +
                FieldName.FK_UserID + "," +
                FieldName.FK_Role + " " +
                "   FROM management.dbo.[FCMUserRole] " +
                "  WHERE FK_UserID = '{0}' "+
                "   ORDER BY FK_Role ASC ", userID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userRole = new FCMUserRole();

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

            return roleList;

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
