using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MackkadoITFramework.ErrorHandling;
using MackkadoITFramework.Utils;
using MySql.Data.MySqlClient;

namespace MackkadoITFramework.ReferenceData
{
    public class CodeValue 
    {
        [Display(Name = "Code Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code Type must be supplied.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Code Type must be between 4 and 20 characters")]
        public string FKCodeType { get; set; }

        [Display(Name = "Code Value ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code Value must be supplied.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Code Value ID must be between 4 and 10 characters")]
        public string ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Description must be supplied.")]
        [Display(Name = "Description")]
        [StringLength(200, MinimumLength = 2)]
        public string Description { get; set; }

        [Display(Name = "Abbreviation")]
        [StringLength(10, MinimumLength = 3)]
        public string Abbreviation { get; set; }

        [Display(Name = "Value Extended")]
        [StringLength(200, MinimumLength = 0)]
        public string ValueExtended { get; set; }

        public bool FoundinDB { get; set; }

        public List<CodeValue> codeValueList;
        public List<string> codeValueIDList;
        public string ListForCodeType;

        // Public Methods
        //

        /// <summary>
        /// Add or Update a record
        /// </summary>
        public ResponseStatus Save()
        {
            // Check if code value exists.
            // If it exists, update
            // Else Add a new one

            Read(true);

            bool results = FoundinDB;

            if (results)
                Update();
            else
                Add();

            return new ResponseStatus();
        }

        // Delete record
        // 
        public void Delete()
        {

        }

        /// <summary>
        /// Return value extended
        /// </summary>
        /// <param name="iCodeType"></param>
        /// <param name="iCodeValueID"></param>
        /// <param name="headerInfo"> </param>
        /// <returns></returns>
        public static string GetCodeValueExtended(string iCodeType, string iCodeValueID, string constring = "")
        {
            if (string.IsNullOrEmpty(constring))
                constring = ConnString.ConnectionStringFramework;

            if (string.IsNullOrEmpty(constring))
            {
                LogFile.WriteToTodaysLogFile(
                    "GetCodeValueExtended Error. Connection string is empty.",
                     programName: "ProcessRequestCodeValues.cs"
                );

                return "";
            }

            string ret = "";

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = 
                                    " SELECT " +
                                    "       ValueExtended " +
                                    "  FROM rdCodeValue" +
                                    " WHERE FKCodeType = @FKCodeType " +
                                    "   AND ID         = @UID ";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {

                    command.Parameters.AddWithValue("@UID", iCodeValueID);
                    command.Parameters.AddWithValue("@FKCodeType", iCodeType);

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            ret = reader["ValueExtended"] as string;
                        }
                    }
                    catch (Exception ex)
                    {

                        LogFile.WriteToTodaysLogFile(
                            "GetCodeValueExtended error retrieving extended. " + ex,
                            programName: "ProcessRequestCodeValues.cs"
                            );

                        throw new Exception(ex.ToString());

                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Return value extended
        /// </summary>
        /// <param name="iCodeType"></param>
        /// <param name="iCodeValueID"></param>
        /// <param name="headerInfo"> </param>
        /// <returns></returns>
        public static string GetCodeValueExtraString( string iCodeType, string iCodeValueID, string constring = "" )
        {
            if ( string.IsNullOrEmpty( constring ) )
                constring = ConnString.ConnectionStringFramework;

            if ( string.IsNullOrEmpty( constring ) )
            {
                LogFile.WriteToTodaysLogFile(
                    "GetCodeValueExtraString Error. Connection string is empty.",
                     programName: "ProcessRequestCodeValues.cs"
                );

                return "";
            }

            string ret = "";

            using ( var connection = new MySqlConnection( ConnString.ConnectionStringFramework ) )
            {

                var commandString = 
                                    " SELECT " +
                                    "       ExtraString " +
                                    "  FROM rdCodeValue" +
                                    " WHERE FKCodeType = @FKCodeType " +
                                    "   AND ID         = @UID ";

                using ( var command = new MySqlCommand(
                                            commandString, connection ) )
                {

                    command.Parameters.AddWithValue( "@UID", iCodeValueID );
                    command.Parameters.AddWithValue( "@FKCodeType", iCodeType );

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        if ( reader.Read() )
                        {
                            ret = reader ["ExtraString"] as string;
                        }
                    }
                    catch ( Exception ex )
                    {

                        LogFile.WriteToTodaysLogFile(
                            "GetCodeValueExtraString error retrieving extended. " + ex,
                            programName: "ProcessRequestCodeValues.cs"
                            );

                        throw new Exception( ex.ToString() );

                    }
                }
            }

            return ret;
        }


        public static string GetCodeValueDescription(string iCodeType, string iCodeValueID, string constring = "")
        {
            string ret = "";

            if (string.IsNullOrEmpty(constring))
                constring = ConnString.ConnectionStringFramework;

            if ( string.IsNullOrEmpty(constring) )
            {
                LogFile.WriteToTodaysLogFile(
                    "GetCodeValueDescription Error. Connection string is empty.",
                     programName: "ProcessRequestCodeValues.cs"
                );

                return "";
            }

            using (var connection = new MySqlConnection(constring))
            {

                var commandString = 
                                    " SELECT " +
                                    "       Description " +
                                    "  FROM rdCodeValue" +
                                    " WHERE FKCodeType = @FKCodeType " +
                                    "   AND ID         = @UID ";

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", MySqlDbType.VarChar).Value = iCodeValueID;
                    command.Parameters.Add("@FKCodeType", MySqlDbType.VarChar).Value = iCodeType;

                    try
                    {
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            ret = reader["Description"] as string;

                        }
                    }
                    catch (Exception ex)
                    {

                        LogFile.WriteToTodaysLogFile(
                            "GetCodeValueExtended error retrieving extended. " + ex,
                            programName: "ProcessRequestCodeValues.cs"
                            );

                        throw new Exception(ex.ToString());

                    }

                }
            }

            return ret;
        }

        public CodeValue Read(bool checkOnly)
        {
            CodeValue ret = null;

            // 
            // EA SQL database
            // da

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT FKCodeType " +
                "      ,ID " +
                "      ,Description " +
                "      ,Abbreviation " +
                "      ,ValueExtended " +
                "  FROM rdCodeValue" +
                " WHERE FKCodeType = '{0}' " +
                "   AND ID         = '{1}' ",
                FKCodeType,
                this.ID);

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {

                    try
                    {
                        connection.Open();


                        if (connection.State != System.Data.ConnectionState.Open)
                        {
                            LogFile.WriteToTodaysLogFile(connection.ServerThread.ToString());
                            return null;
                        }

                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            if (!checkOnly)
                            {
                                FKCodeType = reader["FKCodeType"] as string;
                                ID = reader["ID"] as string;
                                Description = reader["Description"] as string;
                                Abbreviation = reader["Abbreviation"] as string;
                                ValueExtended = reader["ValueExtended"] as string;
                            }

                            FoundinDB = true;
                        }
                        else
                        {
                            FoundinDB = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Description = ex.ToString();

                        LogFile.WriteToTodaysLogFile(this.Description, Helper.Utils.UserID, null,"ProcessRequestCodeValues.cs");

                        throw new Exception(ex.ToString());
                    }
                }
            }

            return ret;

        }

        //
        // 
        //
        public ResponseStatus Add()
        {

            string ret = "Item updated successfully";
            int _uid = 0;

            DateTime _now = DateTime.Today;

            if (FKCodeType == null)
                return new ResponseStatus(MessageType.Error);

            if (ID == null)
                return new ResponseStatus(MessageType.Error);

            if (Description == null)
                return new ResponseStatus(MessageType.Error);

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                (
                   "INSERT INTO rdCodeValue ( " +
                   "FKCodeType, "+
                   "ID, " + 
                   "Description, " +
                   "Abbreviation, " +
                   "ValueExtended" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @FKCodeType    " +
                   ", @ID            " +
                   ", @Description   " +
                   ", @Abbreviation  " +
                   ", @ValueExtended " +
                   " )"

                   );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@FKCodeType", MySqlDbType.VarChar).Value = FKCodeType;
                    command.Parameters.Add("@ID", MySqlDbType.VarChar).Value = ID;
                    command.Parameters.Add("@Description", MySqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@Abbreviation", MySqlDbType.VarChar).Value = Abbreviation;
                    command.Parameters.Add("@ValueExtended", MySqlDbType.VarChar).Value = ValueExtended;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return new ResponseStatus();
        }

        public ResponseStatus Update()
        {

            if (string.IsNullOrEmpty(FKCodeType))
                return new ResponseStatus { ReturnCode = -0010, ReasonCode = 0001, Message = "Code Type is mandatory." };

            if (string.IsNullOrEmpty( ID ))
                return new ResponseStatus { ReturnCode = -0010, ReasonCode = 0002, Message = "ID not supplied." };

            if (string.IsNullOrEmpty( Description ))
                return new ResponseStatus { ReturnCode = -0010, ReasonCode = 0003, Message = "Description not supplied." };

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format
                (
                   "UPDATE rdCodeValue  " +
                   "SET " +
                   "   Description  = '{0}', " +
                   "   Abbreviation = '{1}', " +
                   "   ValueExtended= '{2}'  " +
                   "WHERE   " +
                   "      FKCodeType  = '{3}' " +
                   "  AND ID          = '{4}' ",
                   Description, 
                   Abbreviation ,
                   ValueExtended, 
                   FKCodeType,
                   ID
                 );

                using (var command = new MySqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }


            return new ResponseStatus();
        }


        // -----------------------------------------------------
        //   Listing code values
        // -----------------------------------------------------

        
        public void ListInCombo(string codeType, System.Windows.Forms.ComboBox input)
        {
            this.List(codeType);

            foreach (CodeValue cv in this.codeValueList)
            {
                input.Items.Add(cv.ID);
            }

            return;

        }

        public void ListInCombo(bool IDDesc, string codeType, System.Windows.Forms.ComboBox input)
        {
            this.List(codeType);

            foreach (CodeValue cv in this.codeValueList)
            {
                input.Items.Add(cv.ID + ";" + cv.Description);
            }

            return;

        }

        //
        // Public Methods
        //
        public ResponseStatus List(string codeType)
        {

            ListForCodeType = codeType;

            this.codeValueList = new List<CodeValue>();
            this.codeValueIDList= new List<string>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  FKCodeType " +
                " ,ID   " +
                " ,Description " +
                " ,Abbreviation " +
                " ,ValueExtended " +
                "   FROM rdCodeValue " +
                " WHERE FKCodeType = '{0}'" , codeType  );

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"] as string;
                            _codeValue.ID = reader["ID"] as string;
                            _codeValue.Description = reader["Description"] as string;
                            _codeValue.Abbreviation = reader["Abbreviation"] as string;
                            _codeValue.ValueExtended = reader["ValueExtended"] as string;

                            this.codeValueList.Add(_codeValue);

                            this.codeValueIDList.Add(_codeValue.ID);

                        }
                    }
                }
            }

            return new ResponseStatus();

        }

        //
        // Public Methods
        //

        public static IEnumerable<CodeValue> ListCodeValues(string codeType)
        {
            List<CodeValue> listOfCodeValues = new List<CodeValue>();
            
            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                    " SELECT  " +
                    "  FKCodeType " +
                    " ,ID   " +
                    " ,Description " +
                    " ,Abbreviation " +
                    " ,ValueExtended " +
                    "   FROM rdCodeValue " +
                    " WHERE FKCodeType = @FKCodeType ";

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {

                    command.Parameters.AddWithValue("@FKCodeType", codeType);

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"] as string;
                            _codeValue.ID = reader["ID"] as string;
                            _codeValue.Description = reader["Description"] as string;
                            _codeValue.Abbreviation = reader["Abbreviation"] as string;
                            _codeValue.ValueExtended = reader["ValueExtended"] as string;

                            listOfCodeValues.Add(_codeValue);

                        }
                    }
                }
            }

            return listOfCodeValues;

        }

        public static bool IsValueInCodeType(string value, string codeType)
        {
            var listOfCodeValues = CodeValue.ListCodeValuesString(codeType);

            if (listOfCodeValues.Contains(value))
                return true;

            return false;
        }


        public static List<string> ListCodeValuesString(string codeType)
        {
            List<string> listOfCodeValues = new List<string>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString =
                    " SELECT  " +
                    "  FKCodeType " +
                    " ,ID   " +
                    " ,Description " +
                    " ,Abbreviation " +
                    " ,ValueExtended " +
                    "   FROM rdCodeValue " +
                    " WHERE FKCodeType = @FKCodeType ";

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {

                    command.Parameters.AddWithValue("@FKCodeType", codeType);

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"] as string;
                            _codeValue.ID = reader["ID"] as string;
                            _codeValue.Description = reader["Description"] as string;
                            _codeValue.Abbreviation = reader["Abbreviation"] as string;
                            _codeValue.ValueExtended = reader["ValueExtended"] as string;

                            listOfCodeValues.Add(_codeValue.ID);

                        }
                    }
                }
            }

            return listOfCodeValues;

        }

        //
        // Public Methods
        //
        /// <summary>
        /// Return a list of codes for a given code type.
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public ResponseStatus ListS(string codeType, List<CodeValue> list)
        {

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  FKCodeType " +
                " ,ID   " +
                " ,Description " +
                " ,Abbreviation " +
                " ,ValueExtended " +
                "   FROM rdCodeValue " +
                " WHERE FKCodeType = '{0}'", codeType);

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"].ToString();
                            _codeValue.ID = reader["ID"].ToString();
                            _codeValue.Description = reader["Description"].ToString();
                            _codeValue.Abbreviation = reader["Abbreviation"].ToString();
                            _codeValue.ValueExtended = reader["ValueExtended"].ToString();

                            list.Add(_codeValue);
                        }
                    }
                }
            }

            return new ResponseStatus();

        }

        /// <summary>
        /// Return a list of codes for a given code type.
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public List<CodeValue> ListS()
        {
            List<CodeValue> list = new List<CodeValue>();

            using (var connection = new MySqlConnection(ConnString.ConnectionStringFramework))
            {

                var commandString = string.Format(
                " SELECT  " +
                "  FKCodeType " +
                " ,ID   " +
                " ,Description " +
                " ,Abbreviation " +
                " ,ValueExtended " +
                "   FROM rdCodeValue " +
                " ORDER BY FKCodeType " ) ;

                using (var command = new MySqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CodeValue _codeValue = new CodeValue();
                            _codeValue.FKCodeType = reader["FKCodeType"].ToString();
                            _codeValue.ID = reader["ID"].ToString();
                            _codeValue.Description = reader["Description"].ToString();
                            _codeValue.Abbreviation = reader["Abbreviation"].ToString();
                            _codeValue.ValueExtended = reader["ValueExtended"].ToString();

                            list.Add(_codeValue);
                        }
                    }
                }
            }

            return list;

        }

    }


    public class ListCodeValuesRequest
    {
        public HeaderInfo XHeaderInfo;
        public CodeType XCodeType;
    }
    public class ListCodeValuesResponse
    {
        public ResponseStatus XResponseStatus;
        public List<CodeValue> ListOfCodeValues;
    }


}
