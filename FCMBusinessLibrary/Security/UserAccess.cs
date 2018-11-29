using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary
{
    public class UserAccess
    {

        #region Properties
        public string UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public int ClientUID
        {
            get { return _ClientUID; }
            set { _ClientUID = value; }
        }
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
        public string Password
        {
            //set
            //{
            //    if (string.IsNullOrWhiteSpace(_Salt))
            //    {
            //        throw new Exception("Salt has to be set first.");
            //    }

            //    _Password = EncryptX(_Salt, value);
            //}
            set { _Password = value; }
            get { return _Password; }
        }
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

        public UserAccess(string userID)
        {
            Read(userID);

            _UserID = userID;
        }
        
        /// <summary>
        /// List user users
        /// </summary>
        /// <returns></returns>
        public static List<UserAccess> List()
        {
            List<UserAccess> userAccessList = new List<UserAccess>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = string.Format(
                " SELECT " +
                SQLConcat() +
                "   FROM management.dbo.[FCMUser] " +
                "   ORDER BY UserID ASC "
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
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

        public string Salt
        {
            set { _Salt = value; }
            get { return _Salt; }
        }

        public bool IsUserAllowed()
        {
            return false;
        }

        public bool Read(string userID)
        {
            bool ret = false;
            // 
            // EA SQL database
            // 
            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {
                // TODO
                // SqlParameter useId = new 

                var commandString = string.Format(
                " SELECT [UserID] " +
                "      ,[UserName] " +
                "      ,[Password] " +
                "      ,[Salt] " +
                "      ,[LogonAttempts] " +
                "  FROM [FCMUser]" +
                " WHERE UserID = '{0}' ",
                userID
                );

                using (var command = new SqlCommand(
                                            commandString, connection ))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            this._UserID = reader["UserID"].ToString();
                            this._UserName = reader["UserName"].ToString();
                            this._Password = reader["Password"].ToString();
                            this._Salt = reader["Salt"].ToString();
                            this._LogonAttempts = Convert.ToInt32( reader["LogonAttempts"] );

                            Client.Client client = new Client.Client();
                            client.FKUserID = this._UserID;
                            client.ReadLinkedUser();
                            this._ClientUID = client.UID;

                            ret = true;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return ret;

        }

        public bool AuthenticateUser(string userID, string inputPassword)
        {
            var UserDB = new UserAccess( userID );

            if (UserDB.LogonAttempts > 4)
                return false;

            if (string.IsNullOrWhiteSpace( inputPassword ))
                return false;

            string passValue = EncryptX( UserDB.Salt, inputPassword );

            if (UserDB.Password == passValue)
            {
                UpdateLogonAttempts( "reset" );
                return true;
            }

            UpdateLogonAttempts( "add" );
            return false;
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
            
            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "INSERT INTO [FCMUser] " +
                   "([UserID], [UserName], [LogonAttempts],[Password],[Salt]" +
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

                using (var command = new SqlCommand(
                                            commandString, connection ))
                {
                    command.Parameters.Add("@UserID", SqlDbType.VarChar ).Value = _UserID;
                    command.Parameters.Add("@UserName", SqlDbType.VarChar ).Value = _UserName;
                    command.Parameters.Add("@LogonAttempts", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = "0";
                    command.Parameters.Add("@Salt", SqlDbType.Int).Value = 0;

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [FCMUser] " +
                   "SET  [UserName] = @UserName " +
                   "WHERE [UserID] = @UserID"
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = _UserID;
                    command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = _UserName;

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

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [FCMUser] " +
                   " SET  [Password] = @Password, [LogonAttempts] = @LogonAttempts " +
                   " WHERE [UserID] = @UserID"
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = _UserID;
                    command.Parameters.Add("@Password", SqlDbType.VarChar).Value = _EncryptedPassword;
                    command.Parameters.Add("@LogonAttempts", SqlDbType.VarChar).Value = 0;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return response;
        }


        public void UpdateLogonAttempts(string action)
        {
            if (action == "reset")
                this.LogonAttempts = 0;
            else
                this.LogonAttempts++;


            if (_UserID == null)
                return;

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "UPDATE [FCMUser] SET LogonAttempts = @LogonAttempts " +
                   "WHERE UserID = @UserID" 
                );

                using (var command = new SqlCommand(
                                            commandString, connection ))
                {
                    command.Parameters.Add( "@UserID", SqlDbType.VarChar ).Value = _UserID;
                    command.Parameters.Add( "@LogonAttempts", SqlDbType.Int ).Value = _LogonAttempts;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
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

        // Decrypt a string into a string using a password 

        //    Uses Decrypt(byte[], byte[], byte[]) 


        public static string Decrypt( string cipherText, string Password )
        {

            byte[] cipherBytes = Convert.FromBase64String( cipherText );

            PasswordDeriveBytes pdb = new PasswordDeriveBytes( Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 
        0x64, 0x76, 0x65, 0x64, 0x65, 0x76} );

            byte[] decryptedData = Decrypt( cipherBytes,
                pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );

            return System.Text.Encoding.Unicode.GetString( decryptedData );
        }

        // Decrypt bytes into bytes using a password 

        //    Uses Decrypt(byte[], byte[], byte[]) 


        public static byte[] Decrypt( byte[] cipherData, string Password )
        {


            PasswordDeriveBytes pdb = new PasswordDeriveBytes( Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
        0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76} );

            return Decrypt( cipherData, pdb.GetBytes( 32 ), pdb.GetBytes( 16 ) );
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
        private static UserAccess SetDocumentItem(SqlDataReader reader)
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
