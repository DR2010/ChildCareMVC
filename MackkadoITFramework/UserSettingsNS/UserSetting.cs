using System;
using System.Collections.Generic;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.UserSettingsNS
{
    public class UserSettings
    {

        public List<UserSettings> ListOfUserSettings;

        #region Properties
        public string FKUserID { get { return _FKUserID; } set { _FKUserID = value; } }
        public string FKScreenCode { get { return _FKScreenCode; } set { _FKScreenCode = value; } }
        public string FKControlCode { get { return _FKControlCode; } set { _FKControlCode = value; } }
        public string FKPropertyCode { get { return _FKPropertyCode; } set { _FKPropertyCode = value; } }
        public string Value { get { return _Value; } set { _Value = value; } }


        #endregion Properties

        #region Attributes
        private string _FKUserID;             // Key
        private string _FKScreenCode;         // Key
        private string _FKControlCode;        // Key 
        private string _FKPropertyCode;       // Key
        private string _Value;

        #endregion Attributes

        #region FieldName
        public struct FieldName
        {
            public const string FKUserID = "FKUserID";
            public const string FKScreenCode = "FKScreenCode";
            public const string FKControlCode = "FKControlCode";
            public const string FKPropertyCode = "FKPropertyCode";
            public const string Value = "Value";
        }
        #endregion FieldName

        /// <summary>
        /// Constructor 
        /// </summary>
        public UserSettings()
        {
            ListOfUserSettings = new List<UserSettings>();
        }

        /// <summary>
        /// List user settings for a given user
        /// </summary>
        /// <returns></returns>
        public static List<UserSettings> List( string userID )
        {
            List<UserSettings> userSettingList = new List<UserSettings>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString =
                    " SELECT " +
                    SQLConcat() +
                    "   FROM UserSettings " +
                    "   WHERE " +
                    "       FKUserID = @FKUserID " +
                    "   ORDER BY FKUserID ASC, FKScreenCode ASC ";

                using (var command = new MySqlCommand( commandString, connection ))
                {

                    command.Parameters.Add("@FKUserID", MySqlDbType.VarChar).Value = userID.Trim();

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            var docItem = SetDocumentItem(reader);

                            // Check if document exists
                            //

                            userSettingList.Add(docItem);
                        }
                    }
                }
            }

            return userSettingList;
            
        }

        /// <summary>
        /// Save details. Create or Update accordingly.
        /// </summary>
        public void Save()
        {
            var checkOnly = new UserSettings();
            checkOnly._FKUserID = _FKUserID;
            checkOnly._FKScreenCode = _FKScreenCode;
            checkOnly._FKControlCode = _FKControlCode;
            checkOnly._FKPropertyCode  = _FKPropertyCode;
            
            if (checkOnly.Read())
            {
                Update();
            }
            else
            {
                Insert();
            }
        }

        /// <summary>
        /// Retrieve user setting details
        /// </summary>
        public bool Read()
        {
            // 
            // EA SQL database
            // 

            bool ret = false;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString =
                    " SELECT " +
                    UserSettingString() +
                    "  FROM UserSettings" +
                    "    WHERE " +
                    FieldName.FKUserID + " = @FKUserID AND " +
                    FieldName.FKScreenCode + " = @FKScreenCode AND " +
                    FieldName.FKControlCode + " = @FKControlCode AND " +
                    FieldName.FKPropertyCode + " = @FKPropertyCode ";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.Add("@FKUserID", MySqlDbType.VarChar).Value = FKUserID;
                    command.Parameters.Add("@FKScreenCode", MySqlDbType.VarChar).Value = FKScreenCode;
                    command.Parameters.Add("@FKControlCode", MySqlDbType.VarChar).Value = FKControlCode;
                    command.Parameters.Add("@FKPropertyCode", MySqlDbType.VarChar).Value = FKPropertyCode;

                    
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadClientObject(reader, this);

                            ret = true;
                        }
                        catch (Exception)
                        {
                            FKUserID = "";
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Load client object
        /// </summary>
        /// <param name="reader"></param>
        private static void LoadClientObject(MySqlDataReader reader, UserSettings client)
        {
            client.FKUserID = reader[FieldName.FKUserID].ToString();
            client.FKScreenCode = reader[FieldName.FKScreenCode].ToString();
            client.FKControlCode = reader[FieldName.FKControlCode].ToString();
            client.FKPropertyCode = reader[FieldName.FKPropertyCode].ToString();
            client.Value = reader[FieldName.Value].ToString();
        }
        
        /// <summary>
        /// Update user setting details
        /// </summary>
        /// <param name="client"></param>
        public void Update()
        {
            DateTime _now = DateTime.Today;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "UPDATE [UserSettings " +
                   " SET " +
                   FieldName.Value + " = @" + FieldName.Value +
                   "    WHERE " +
                   FieldName.FKUserID + " = @" + FieldName.FKUserID + " AND " +
                   FieldName.FKScreenCode + " = @" + FieldName.FKScreenCode + " AND " +
                   FieldName.FKControlCode + " = @" + FieldName.FKControlCode + " AND " +
                   FieldName.FKPropertyCode + " = @" + FieldName.FKPropertyCode  
                );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    AddSQLParameters(command, MakConstant.SQLAction.UPDATE);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        /// <summary>
        /// Add new user setting
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public void Insert()
        {

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO UserSettings " +
                   "(" +
                   UserSettingString() +
                   ")" +
                        " VALUES " +
                   "( @FKUserID     " +
                   ", @FKScreenCode    " +
                   ", @FKControlCode    " +
                   ", @FKPropertyCode " +
                   ", @Value " +
                   ")"
                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    AddSQLParameters(command, MakConstant.SQLAction.CREATE);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }


        /// <summary>
        /// Client string of fields.
        /// </summary>
        /// <returns></returns>
        private static string UserSettingString()
        {
            return (
                        FieldName.FKUserID
                + "," + FieldName.FKScreenCode
                + "," + FieldName.FKControlCode
                + "," + FieldName.FKPropertyCode
                + "," + FieldName.Value
            );

        }


        /// <summary>
        /// Add SQL Parameters
        /// </summary>
        /// <param name="_uid"></param>
        /// <param name="command"></param>
        /// <param name="action"></param>
        private void AddSQLParameters(MySqlCommand command, string action)
        {
            command.Parameters.Add("@Value", MySqlDbType.VarChar).Value = Value;
            command.Parameters.Add("@FKUserID", MySqlDbType.VarChar).Value = FKUserID;
            command.Parameters.Add("@FKScreenCode", MySqlDbType.VarChar).Value = FKScreenCode;
            command.Parameters.Add("@FKControlCode", MySqlDbType.VarChar).Value = FKControlCode;
            command.Parameters.Add("@FKPropertyCode", MySqlDbType.VarChar).Value = FKPropertyCode;
        }



        /// <summary>
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        public static string SQLConcat()
        {
            string ret = " " +
            FieldName.FKScreenCode+ "," +
            FieldName.FKControlCode + "," +
            FieldName.FKPropertyCode + "," +
            FieldName.FKUserID + "," +
            FieldName.Value + " " ; 

            return ret;
        }

        /// <summary>
        /// Set user setting values
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static UserSettings SetDocumentItem(MySqlDataReader reader)
        {
            var userSetting = new UserSettings();

            userSetting.FKUserID = reader[FieldName.FKUserID].ToString();
            userSetting.FKScreenCode = reader[FieldName.FKScreenCode].ToString();
            userSetting.FKControlCode = reader[FieldName.FKControlCode].ToString();
            userSetting.FKPropertyCode = reader[FieldName.FKPropertyCode].ToString();
            userSetting.Value = reader[FieldName.Value].ToString();

            return userSetting;
        }
    }
}
