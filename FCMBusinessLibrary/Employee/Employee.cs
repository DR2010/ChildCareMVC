using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using FCMBusinessLibrary.ReferenceData;

namespace FCMBusinessLibrary
{
    public class Employee
    {
        #region Properties

        public int FKCompanyUID { get; set; }
        public int UID { get; set; }
        public string Name { get; set; }
        public string RoleType { get; set; }
        public string RoleDescription { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string EmailAddress { get; set; }
        public char IsAContact { get; set; }
        public string UserIdCreatedBy { get; set; }
        public string UserIdUpdatedBy { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        #endregion Properties

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string FKCompanyUID = "FKCompanyUID";
            public const string UID = "UID";
            public const string Name = "Name";
            public const string RoleType = "RoleType";
            public const string RoleDescription = "RoleDescription";
            public const string Address = "Address";
            public const string Phone = "Phone";
            public const string Fax = "Fax";
            public const string EmailAddress = "EmailAddress";
            public const string IsAContact = "IsAContact";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
        }

        //? Missing constructor

        /// <summary>
        /// Get Employee details
        /// </summary>
        public void Read()
        {
            // 
            // EA SQL database
            // 
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT " +
                EmployeeFieldsString() 
                + "  FROM [Employee]" +
                " WHERE UID = " + this.UID;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadEmployeeObject(reader, this);

                        }
                        catch (Exception)
                        {
                            UID = 0;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Retrieve last Employee UID
        /// </summary>
        /// <returns></returns>
        public int GetLastUID()
        {
            int LastUID = 0;

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "SELECT MAX(UID) LASTUID FROM [Employee] ";

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
        /// Add new Employee
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Insert()
        {
            var rs = new ResponseStatus();
            rs.Message = "Client Added Successfully";
            rs.ReturnCode = 1;
            rs.ReasonCode = 1;

            int _uid = 0;

            _uid = GetLastUID() + 1;
            this.UID = _uid;

            DateTime _now = DateTime.Today;
            this.CreationDateTime = _now;
            this.UpdateDateTime = _now;

            if (Name == null)
                Name = "";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [Employee] " +
                   "( " + 
                    EmployeeFieldsString() +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @" + FieldName.FKCompanyUID +
                   ", @" + FieldName.UID +
                   ", @" + FieldName.Name +
                   ", @" + FieldName.RoleType +
                   ", @" + FieldName.Address +
                   ", @" + FieldName.Phone +
                   ", @" + FieldName.Fax +
                   ", @" + FieldName.EmailAddress +
                   ", @" + FieldName.IsAContact +
                   ", @" + FieldName.UserIdCreatedBy +
                   ", @" + FieldName.UserIdUpdatedBy +
                   ", @" + FieldName.CreationDateTime +
                   ", @" + FieldName.UpdateDateTime +
                   " )"
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    AddSQLParameters(command, "CREATE");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return rs;
        }

        /// <summary>
        /// Update Employee Details
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Update()
        {
            ResponseStatus ret = new ResponseStatus();      
            ret.Message = "Item updated successfully";

            if (Name == null)
                Name = "";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [Employee] " +
                   " SET  " +
                   FieldName.Name + " = @" + FieldName.Name + ", " +
                   FieldName.RoleType + " = @" + FieldName.RoleType + ", " +
                   FieldName.Fax + " = @" + FieldName.Fax + ", " +
                   FieldName.Address + " = @" + FieldName.Address + ", " +
                   FieldName.EmailAddress + " = @" + FieldName.EmailAddress + ", " +
                   FieldName.IsAContact + " = @" + FieldName.IsAContact + ", " +
                   FieldName.Phone + " = @" + FieldName.Phone + ", " +
                   FieldName.UpdateDateTime + " = @" + FieldName.UpdateDateTime + ", " +
                   FieldName.UserIdUpdatedBy + " = @" + FieldName.UserIdUpdatedBy +
                   "   WHERE    UID = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    AddSQLParameters(command, "UPDATE");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return ret;
        }

        /// <summary>
        /// Delete Employee 
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Delete()
        {

            var ret = new ResponseStatus();
            ret.Message = "Employee Deleted successfully";

            if (Name == null)
                Name = "";

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "DELETE [Employee] " +
                   "   WHERE    UID = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection ))
                {
                    command.Parameters.Add( "@UID", SqlDbType.VarChar ).Value = UID;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return ret;
        }

        /// <summary>
        /// List employees
        /// </summary>
        /// <param name="clientID"></param>
        public static List<Employee> List( int clientID )
        {
            List<Employee> employeeList = new List<Employee>();

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString = string.Format(
                " SELECT " +
                EmployeeFieldsString() +
                "   FROM [Employee] " +
                "   WHERE  FKCompanyUID = {0}",
                clientID );

                using (var command = new SqlCommand(
                                      commandString, connection ))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employee _employee = new Employee();
                            LoadEmployeeObject(reader, _employee);

                            employeeList.Add( _employee );
                        }
                    }
                }
            }

            return employeeList;
        }

        public static string EmployeeFieldsString()
        {
            return (
                        FieldName.FKCompanyUID
                + "," + FieldName.UID
                + "," + FieldName.Name
                + "," + FieldName.RoleType
                + "," + FieldName.Address
                + "," + FieldName.Phone
                + "," + FieldName.Fax
                + "," + FieldName.EmailAddress
                + "," + FieldName.IsAContact
                + "," + FieldName.UserIdUpdatedBy
                + "," + FieldName.UserIdCreatedBy
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UpdateDateTime
                );

        }

        /// <summary>
        /// Add Employee parameters to the SQL Command.
        /// </summary>
        /// <param name="command"></param>
        private void AddSQLParameters(SqlCommand command, string action)
        {
            command.Parameters.Add(FieldName.FKCompanyUID, SqlDbType.BigInt).Value = FKCompanyUID;
            command.Parameters.Add(FieldName.UID, SqlDbType.BigInt).Value = UID;
            command.Parameters.Add(FieldName.Name, SqlDbType.VarChar).Value = Name;
            command.Parameters.Add(FieldName.RoleType, SqlDbType.VarChar).Value = RoleType;
            command.Parameters.Add(FieldName.Address, SqlDbType.VarChar).Value = Address;
            command.Parameters.Add(FieldName.Phone, SqlDbType.VarChar).Value = Phone;
            command.Parameters.Add(FieldName.Fax, SqlDbType.VarChar).Value = Fax;
            command.Parameters.Add(FieldName.EmailAddress, SqlDbType.VarChar).Value = EmailAddress;
            command.Parameters.Add(FieldName.IsAContact, SqlDbType.Char).Value = IsAContact;
            command.Parameters.Add(FieldName.UserIdUpdatedBy, SqlDbType.Char).Value = UserIdUpdatedBy;
            command.Parameters.Add(FieldName.UpdateDateTime, SqlDbType.DateTime).Value = UpdateDateTime;

            if (action == "CREATE")
            {
                command.Parameters.Add(FieldName.UserIdCreatedBy, SqlDbType.Char).Value = UserIdCreatedBy;
                command.Parameters.Add(FieldName.CreationDateTime, SqlDbType.DateTime).Value = CreationDateTime;
            }

        }
        
        /// <summary>
        /// This method loads the information from the sqlreader into the Employee object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="employee"></param>
        private static void LoadEmployeeObject(SqlDataReader reader, Employee employee)
        {
            employee.FKCompanyUID = Convert.ToInt32(reader[FieldName.FKCompanyUID]);
            employee.UID = Convert.ToInt32(reader[FieldName.UID].ToString());
            employee.Name = reader[FieldName.Name].ToString();
            employee.RoleType = reader[FieldName.RoleType].ToString();
            employee.Address = reader[FieldName.Address].ToString();
            employee.Phone = reader[FieldName.Phone].ToString();
            employee.Fax = reader[FieldName.Fax].ToString();
            employee.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString();
            employee.UserIdUpdatedBy = reader[FieldName.UserIdCreatedBy].ToString();
            employee.RoleDescription = CodeValue.GetCodeValueDescription(FCMConstant.CodeTypeString.RoleType, employee.RoleType);

            try  { employee.IsAContact = Convert.ToChar(reader[FieldName.IsAContact].ToString()); }
            catch { employee.IsAContact = 'N'; }
            try { employee.EmailAddress = reader[FieldName.EmailAddress].ToString(); }
            catch { employee.EmailAddress = ""; }
            try
            {
                employee.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString());
            }
            catch
            {
                employee.UpdateDateTime = DateTime.Now;
            }
            try
            {
                employee.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString());
            }
            catch
            {
                employee.CreationDateTime = DateTime.Now;
            }

        }

    }
}
