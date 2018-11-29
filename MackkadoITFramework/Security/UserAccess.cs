using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.Security
{
    public class UserAccess
    {

        #region Properties
        [Required( ErrorMessage = "User ID is mandatory. You can use your email address." )]
        [Display( Name = "Enter User ID" )]
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        [Display( Name = "Enter Name" )]
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public int LogonAttempts
        {
            get { return _LogonAttempts; }
            set { _LogonAttempts = value; }
        }

        [Required( ErrorMessage = "Password is required." )]
        [Display( Name = "Enter Password" )]
        public string Password
        {
            set { _Password = value; }
            get { return _Password; }
        }

        public int ClientUID
        {
            set { _ClientUID = value; }
            get { return _ClientUID; }
        }

        [Display( Name = "Re-enter Password" )]
        public string PasswordRetype { get; set; }
        public string ConfirmPassword { get; set; }

        public string Salt
        {
            set { _Salt = value; }
            get { return _Salt; }
        }


        public List<UserAccess> ListOfUsers;
        
        #endregion Properties

        #region Attributes
        private string _UserID;
        private int _ClientUID;
        private string _UserName;
        private string _Password;
        private string _EncryptedPassword;
        private string _Salt;
        private int _LogonAttempts;
        #endregion Attributes

        #region FieldName
        public struct FieldName
        {
            public const string UserID = "UserID";
            public const string Password = "Password";
            public const string Salt = "Salt";
            public const string UserName = "UserName";
            public const string LogonAttempts = "LogonAttempts";
        }
        #endregion FieldName

        public UserAccess()
        {
        }

      
        /// <summary>
        /// List user users
        /// </summary>
        /// <returns></returns>
        public static List<UserAccess> List(string xConnectionStringUsed = null)
        {

            if (string.IsNullOrEmpty(xConnectionStringUsed))
            {
                if (string.IsNullOrEmpty(ConnString.ConnectionStringFramework))
                {
                    return null;
                }
                
                xConnectionStringUsed = ConnString.ConnectionStringFramework;
            }

            List<UserAccess> userAccessList = new List<UserAccess>();

            using (var connection = new MySqlConnection(xConnectionStringUsed))
            {
                var commandString = string.Format(
                " SELECT " +
                SQLConcat() +
                "   FROM SecurityUser " +
                "   ORDER BY UserID ASC "
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

                            userAccessList.Add(docItem);
                        }
                    }
                }
            }

            return userAccessList;

        }

        /// <summary>
        /// List user users
        /// </summary>
        /// <returns></returns>
        public ResponseStatus ListUsers()
        {

            ListOfUsers = new List<UserAccess>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {
                var commandString = string.Format(
                " SELECT " +
                SQLConcat() +
                "   FROM SecurityUser " +
                "   ORDER BY UserID ASC "
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

                            ListOfUsers.Add(docItem);
                        }
                    }
                }
            }

            return new ResponseStatus();

        }



        public bool IsUserAllowed()
        {
            return false;
        }

        public ResponseStatus Read(string userID)
        {

            var ret = new ResponseStatus();
            // 
            // EA SQL database
            // 
            using (var connection = new MySqlConnection( ConnString.ConnectionStringFramework ))
            {
                // TODO
                // SqlParameter useId = new 

                var commandString = string.Format(
                " SELECT UserID " +
                "      ,UserName " +
                "      ,Password " +
                "      ,Salt " +
                "      ,LogonAttempts " +
                "  FROM SecurityUser" +
                " WHERE UserID = '{0}' ",
                userID
                );

                using (var command = new MySqlCommand(
                                            commandString, connection ))
                {

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            _UserID = reader["UserID"].ToString();
                            _UserName = reader["UserName"].ToString();
                            _Password = reader["Password"].ToString();
                            _Salt = reader["Salt"].ToString();
                            _LogonAttempts = Convert.ToInt32(reader["LogonAttempts"]);

                            //var client = new Client.Client(HeaderInfo.Instance);
                            //client.FKUserID = _UserID;
                            //client.ReadLinkedUser();
                            //_ClientUID = client.UID;

                            // 
                            ret.ReturnCode = 0001;
                            ret.ReasonCode = 0001;
                            ret.Message = "Record found.";
                            return ret;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(
                            "GetCodeValueExtended error retrieving extended value. " + ex,
                            programName: "ProcessRequestCodeValues.cs"
                            );

                        return new ResponseStatus(MessageType.Error) {Message = ex.ToString()};
                    }
                }
            }

            ret.ReturnCode = 0001;
            ret.ReasonCode = 0002;
            ret.Message = "Record not found.";
            return ret;

        }

        public ResponseStatus AuthenticateUser(string userID, string inputPassword)
        {

            var UserDB = new UserAccess();
            var readuser = UserDB.Read(userID);

            if (readuser.ReturnCode == 0001 && readuser.ReasonCode == 0001)
            {
                // Ok
            }
            if (readuser.ReturnCode == 0001 && readuser.ReasonCode == 0002)
            {
                return new ResponseStatus(MessageType.Error) {Message = "Credentials are not correct."};
            }
            if (readuser.ReturnCode <= 0000 )
            {
                return readuser;
            }


            if (UserDB.LogonAttempts > 4)
            {
                return new ResponseStatus(MessageType.Error) { Message = "User locked due to logon attempts. Please contact system support." };
            }

            if (string.IsNullOrWhiteSpace( inputPassword ))
            {
                return new ResponseStatus(MessageType.Error) { Message = "Credentials are not correct. Spaces or Nulls." };
            }

            string passValue = EncryptX( UserDB.Salt, inputPassword );

            if (UserDB.Password == passValue)
            {   
                //
                // Logon successfull
                //
                UpdateLogonAttempts( "reset" );
                return new ResponseStatus();
            }

            UpdateLogonAttempts( "add" );

            return new ResponseStatus(MessageType.Error) { Message = "Credentials are not correct. Spaces or Nulls." };
        }

        public void test()
        {
            String s;
            // s.GetHashCode(); binary 20 posicoes
            // Daniel - back here 

        }

        public string EncryptX( string salt, string password )
        {
            int result = 0;
            // int salt = System.DateTime.Now.Hour;
            List <int> passwordChar = new List<int>();
            int desc = password.Length;
            double finalResult = 0.00;

            foreach (char c in password)
            {
                int value = Convert.ToInt32( c );
                passwordChar.Add( value );

                finalResult += Math.Sqrt( Convert.ToDouble( value * desc ) );
                desc--;
            }

            return Convert.ToInt32( finalResult ).ToString();
        }

        public string DecryptX( string salt, string password )
        {
            string ret = "";
            int result = 0;
            // int salt = System.DateTime.Now.Hour;
            List <int> passwordChar = new List<int>();
            int desc = password.Length;
            double finalResult = 0.00;

            foreach( char c in password)
            {
                int value = Convert.ToInt32( c ) ;
                passwordChar.Add( value );

                finalResult += Math.Sqrt( Convert.ToDouble( value * finalResult ) );
            }

            result = Convert.ToInt32( finalResult );

            ret = Password;

            return ret;
        }

        public ResponseStatus AddUser()
        {

            ResponseStatus response = new ResponseStatus();
            response.Message = "User Added Successfully.";
            response.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000004;

            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (_UserID == null)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "User ID is mandatory.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000003;
                response.Contents = 0;
                return response;
            }

            if (_Salt == null)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0003;
                response.Message = "Salt is mandatory.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000005;
                response.Contents = 0;
                return response;
            }

            _EncryptedPassword = EncryptX(_Salt, Password);


            using (var connection = new MySqlConnection( ConnString.ConnectionStringFramework ))
            {

                var commandString =
                (
                   "INSERT INTO SecurityUser " +
                   "(UserID, UserName, LogonAttempts, Password, Salt" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @UserID      " +
                   ", @UserName      " +
                   ", @LogonAttempts " +
                   ", @Password " +
                   ", @Salt " +
                   " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection ))
                {
                    command.Parameters.Add("@UserID", MySqlDbType.VarChar ).Value = _UserID;
                    command.Parameters.Add("@UserName", MySqlDbType.VarChar ).Value = _UserName;
                    command.Parameters.Add("@LogonAttempts", MySqlDbType.Int32).Value = 0;
                    command.Parameters.Add("@Password", MySqlDbType.VarChar).Value = _EncryptedPassword;
                    command.Parameters.Add("@Salt", MySqlDbType.Int32).Value = Salt;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }

        public ResponseStatus UpdateUser()
        {

            ResponseStatus response = new ResponseStatus();
            response.Message = "User Updated Successfully.";
            response.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000005;

            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (_UserID == null)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "User ID is mandatory.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000003;
                response.Contents = 0;
                return response;
            }

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "UPDATE SecurityUser " +
                   "SET  UserName = @UserName " +
                   "WHERE UserID = @UserID"
                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UserID", MySqlDbType.VarChar).Value = _UserID;
                    command.Parameters.Add("@UserName", MySqlDbType.VarChar).Value = _UserName;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }

        public ResponseStatus UpdatePassword()
        {

            ResponseStatus response = new ResponseStatus();
            response.Message = "Password changed successfully.";
            response.UniqueCode = ResponseStatus.MessageCode.Informational.FCMINF00000005;

            // Encrypt password
            _EncryptedPassword = EncryptX(_Salt, Password);

            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (_UserID == null)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "User ID is mandatory.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000003;
                response.Contents = 0;
                return response;
            }

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "UPDATE SecurityUser " +
                   " SET  Password = @Password, LogonAttempts = @LogonAttempts " +
                   " WHERE UserID = @UserID"
                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UserID", MySqlDbType.VarChar).Value = _UserID;
                    command.Parameters.Add("@Password", MySqlDbType.VarChar).Value = _EncryptedPassword;
                    command.Parameters.Add("@LogonAttempts", MySqlDbType.VarChar).Value = 0;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }


        public ResponseStatus UpdateLogonAttempts(string action)
        {
            if (action == "reset")
                this.LogonAttempts = 0;
            else
                this.LogonAttempts++;


            if (_UserID == null)
                return new ResponseStatus(MessageType.Error);


            using (var connection = new MySqlConnection( ConnString.ConnectionStringFramework ))
            {

                var commandString =
                (
                   "UPDATE SecurityUser SET LogonAttempts = @LogonAttempts " +
                   "WHERE UserID = @UserID" 
                );

                using (var command = new MySqlCommand(
                                            commandString, connection ))
                {
                    command.Parameters.Add( "@UserID", MySqlDbType.VarChar ).Value = _UserID;
                    command.Parameters.Add( "@LogonAttempts", MySqlDbType.Int32 ).Value = _LogonAttempts;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return new ResponseStatus();
        }

        // Encrypt a byte array into a byte array using a key and an IV 

        public static byte[] Encrypt( byte[] clearData, byte[] Key, byte[] IV )
        {
            // Create a MemoryStream to accept the encrypted bytes 

            MemoryStream ms = new MemoryStream();

            Rijndael alg = Rijndael.Create();

            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream( ms,
                alg.CreateEncryptor(), CryptoStreamMode.Write );

            cs.Write( clearData, 0, clearData.Length );
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        // Encrypt a string into a string using a password 

        //    Uses Encrypt(byte[], byte[], byte[]) 


        public static string Encrypt( string clearText, string Password )
        {
            // First we need to turn the input string into a byte array. 

            byte[] clearBytes = 
        System.Text.Encoding.Unicode.GetBytes( clearText );

            PasswordDeriveBytes pdb = new PasswordDeriveBytes( Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
        0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76} );


            byte[] encryptedData = Encrypt( clearBytes,
                        pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );


            return Convert.ToBase64String( encryptedData );

        }

        // Encrypt bytes into bytes using a password 

        //    Uses Encrypt(byte[], byte[], byte[]) 


        public static byte[] Encrypt( byte[] clearData, string Password )
        {

            PasswordDeriveBytes pdb = new PasswordDeriveBytes( Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
        0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76} );


            return Encrypt( clearData, pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );

        }

        // Decrypt a byte array into a byte array using a key and an IV 

        public static byte[] Decrypt( byte[] cipherData,
                                    byte[] Key, byte[] IV )
        {


            MemoryStream ms = new MemoryStream();

            Rijndael alg = Rijndael.Create();

            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream( ms,
                alg.CreateDecryptor(), CryptoStreamMode.Write );

            // Write the data and make it do the decryption 

            cs.Write( cipherData, 0, cipherData.Length );

            cs.Close();


            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }


        /// <summary>
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        private static string SQLConcat()
        {
            string ret = " " +
            FieldName.UserID + "," +
            FieldName.UserName + ", " +
            FieldName.Password + ", " +
            FieldName.Salt + ", " +
            FieldName.LogonAttempts + " ";

            return ret;
        }


        /// <summary>
        /// Set user setting values
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static UserAccess SetDocumentItem(MySqlDataReader reader)
        {
            var user = new UserAccess();

            user.UserID = reader[FieldName.UserID].ToString();
            user.UserName = reader[FieldName.UserName].ToString();
            user.Salt = reader[FieldName.Salt].ToString();
            user.LogonAttempts = Convert.ToInt32(reader[FieldName.LogonAttempts].ToString());
            user.Password = reader[FieldName.Password].ToString();

            return user;
        }

    }
}
