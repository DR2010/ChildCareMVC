using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;


namespace MackkadoITFramework.ReferenceData
{
    public class CodeType
    {
        [Display(Name = "Code Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code Type must be supplied.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Code Type must be between 4 and 20 characters")]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description must be supplied.")]
        [Display(Name = "Description")]
        [StringLength(50, MinimumLength = 4)]
        public string Description { get; set; }

        [Display(Name = "ShortCodeType")]
        [StringLength(3, MinimumLength = 3)]
        public string ShortCodeType { get; set; }

        public List<CodeType> codeTypeList;

        private HeaderInfo _headerInfo;

        /// <summary>
        /// Add code type
        /// </summary>
        public ResponseStatus Add()
        {
            // ConnString.ConnectionStringFramework
            // ConnString.ConnectionStringFramework
            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                    (
                        "INSERT INTO rdCodeType " +
                        "(CodeType, Description, ShortCodeType" +
                        ")" +
                        " VALUES " +
                        "( " +
                        "  @CodeType      " +
                        ", @Description   " +
                        ", @ShortCodeType " +
                        " )"

                    );

                using (var command = new MySqlCommand(
                    commandString, connection))
                {
                    command.Parameters.AddWithValue("@CodeType", Code);
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@ShortCodeType", MySqlDbType.VarChar).Value = ShortCodeType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return new ResponseStatus();
        }

        public ResponseStatus Read()
        {

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                    " SELECT CodeType " +
                    "      ,Description " +
                    "      ,ShortCodeType " +
                    "  FROM rdCodeType" +
                    " WHERE CodeType = '{0}' ",
                    Code);

                using (var command = new MySqlCommand(
                    commandString, connection))
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            Code = reader["CodeType"].ToString();
                            Description = reader["Description"].ToString();
                            ShortCodeType = reader["ShortCodeType"].ToString();

                        }
                        catch (Exception ex)
                        {
                            Description = ex.ToString();
                        }
                    }
                }
            }

            return new ResponseStatus();
        }

        public ResponseStatus Update()
        {

            var ret = new ResponseStatus {Message = "Item updated successfully"};

            if (string.IsNullOrEmpty( Code ))
            {
                ret.ReturnCode = -0010;
                ret.ReasonCode = 0001;
                ret.Message = "Update Error - Code field not supplied.";
                return ret;
            }

            if (string.IsNullOrEmpty( Description ))
            {
                ret.ReturnCode = -0010;
                ret.ReasonCode = 0002;
                ret.Message = "Update Error - Description field not supplied.";
                return ret;
            }

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format
                    (
                        "UPDATE rdCodeType  " +
                        "SET " +
                        "   Description  = @Description  " +
                        "  ,ShortCodeType = @ShortCodeType " +
                        "WHERE   " +
                        "      CodeType  = @CodeType ",
                        Description,
                        Code
                    );

                using (var command = new MySqlCommand(
                    commandString, connection))
                {

                    command.Parameters.Add("@CodeType", MySqlDbType.VarChar).Value = Code;
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@ShortCodeType", MySqlDbType.VarChar).Value = ShortCodeType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return ret;
        }

        public void Delete()
        {

            if (Code == null)
                return;

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = "DELETE FROM rdCodeType WHERE CodeType = @CodeType ";


                using (var command = new MySqlCommand( commandString, connection) )
                {
                    command.Parameters.Add("@CodeType", MySqlDbType.VarChar).Value = Code;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public ResponseStatus List(HeaderInfo headerInfo)
        {
            codeTypeList = new List<CodeType>();

            try
            {
                using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
                {

                    var commandString = string.Format(
                        " SELECT  " +
                        "  CodeType " +
                        " ,Description " +
                        "   FROM rdCodeType ");

                    using (var command = new MySqlCommand(
                        commandString, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CodeType _codeType = new CodeType();
                                _codeType.Code = reader["CodeType"].ToString();
                                _codeType.Description = reader["Description"].ToString();

                                codeTypeList.Add(_codeType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteToTodaysLogFile(
                    "Error adding new document." + ex,
                    headerInfo.UserID,
                    "Document.cs"
                    );

                return new ResponseStatus(MessageType.Error) {Message = ex.ToString()};

            }
            return new ResponseStatus();

        }

        public struct CodeTypeValue
        {
            public const string ContractStatus = "CONTRACTSTATUS";
            public const string ContractType = "CONTRACTTYPE";
            public const string ProposalType = "PROPTYPE";
            public const string ProposalStatus = "PROPSTATUS";
            public const string ClientOtherField = "CLIENTOTHERFIELD";

        }

        /// <summary>
        /// List Code Types
        /// </summary>
        public ResponseStatus List( List<CodeType> ctList )
        {

            try
            {
                using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
                {

                    var commandString = string.Format(
                    " SELECT  " +
                    "  CodeType " +
                    " ,Description " +
                    "   FROM rdCodeType ");

                    using (var command = new MySqlCommand(
                                          commandString, connection))
                    {
                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var _codeType = new CodeType();
                                _codeType.Code = reader["CodeType"].ToString();
                                _codeType.Description = reader["Description"].ToString();

                                // Instance Variable
                                codeTypeList.Add(_codeType);
 
                                // Input variable
                                ctList.Add(_codeType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteToTodaysLogFile(
                    "Error adding new document." + ex,
                    "",
                    "CodeType.cs"
                    );

                return new ResponseStatus(MessageType.Error)
                           {ReturnCode = -0020, ReasonCode = 0001, Message = ex.ToString()};
            }

            return new ResponseStatus();
        }

        //public void Redis_StoreCodeTypes()
        //{

        //    List<CodeType> list;
        //    list = new List<CodeType>();
        //    this.codeTypeList = new List<CodeType>();
        //    this.List(list);


        //    string host = "172.16.0.17";
                
        //    using (RedisClient redisClient = new RedisClient(host))
        //    {

        //        foreach (var codetype in list)
        //        {
        //            // store code type

        //            IRedisTypedClient<CodeType> codetypes = redisClient.As<CodeType>();
        //            redisClient.ChangeDb(0);
        //            codetypes.SetEntryIfNotExists(codetype.Code, codetype);

        //            CodeValue cv = new CodeValue();
        //            ResponseStatus rs = cv.List(codetype.Code);

        //            foreach (var codevalue in cv.codeValueList)
        //            {
        //                // store values

        //                IRedisTypedClient<CodeValue> codevalues = redisClient.As<CodeValue>();
        //                redisClient.ChangeDb(1);
        //                codevalues.SetEntryIfNotExists(codevalue.FKCodeType+codevalue.ID, codevalue);
    
        //            }
        //        }
        //    }
        //}
    }
}
