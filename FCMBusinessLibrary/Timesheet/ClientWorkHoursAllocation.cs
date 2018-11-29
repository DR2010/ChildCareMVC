using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace FCMBusinessLibrary
{
    public class ClientWorkHoursAllocation
    {
        public int UID { get; set; }
        public DateTime Date { get; set; }
        public int FKCompanyUID { get; set; }
        public int FKEmployeeUID { get; set; }
        public int FKTaskUID { get; set; }
        public int Hours { get; set; }
        public int FKClientWorkIsFor { get; set; }
        public int FKInvoiceUID { get; set; }
        public int FKInvoiceItemID { get; set; }

        private static string tableName = "ClientWorkHoursAllocation";

        public enum FieldName
        {
            UID, 
            Date, 
            FKCompanyUID,
            FKEmployeeUID,
            FKTaskUID,
            Hours,
            FKClientWorkIsFor,
            FKInvoiceUID,
            FKInvoiceItemID 
        }

        /// <summary>
        /// Add new Employee
        /// </summary>
        /// <returns></returns>
        public ResponseStatus Add()
        {
            var rs = new ResponseStatus();
            rs.Message = "Client Added Successfully";
            rs.ReturnCode = 1;
            rs.ReasonCode = 1;

            int _uid = 0;

            _uid = GetLastUID() + 1;
            this.UID = _uid;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO " + tableName +
                   "( " +
                       ListOfFields(separator:",",prefixLine1:"") +
                   ")" +
                        " VALUES " +
                   "( " +
                       ListOfFields(separator: ", @", prefixLine1: " @") +
                   " )"
                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    AddSQLParameters(command);

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


            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [Employee] " +
                   " SET  " +
                   FieldName.Date.ToString() + " = @" + FieldName.Date.ToString() + ", " +
                   FieldName.FKClientWorkIsFor.ToString() + " = @" + FieldName.FKClientWorkIsFor.ToString() + ", " +
                   FieldName.FKCompanyUID.ToString() + " = @" + FieldName.FKCompanyUID.ToString() + ", " +
                   FieldName.FKEmployeeUID.ToString() + " = @" + FieldName.FKEmployeeUID.ToString() + ", " +
                   FieldName.FKInvoiceItemID.ToString() + " = @" + FieldName.FKInvoiceItemID.ToString() + ", " +
                   FieldName.FKInvoiceUID.ToString() + " = @" + FieldName.FKInvoiceUID.ToString() + ", " +
                   FieldName.FKTaskUID.ToString() + " = @" + FieldName.FKTaskUID.ToString() + ", " +
                   FieldName.Hours.ToString() + " = @" + FieldName.Hours.ToString() +
                   "   WHERE    UID = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {

                    AddSQLParameters(command);

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
            ret.Message = "Deleted successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = "DELETE " + tableName + " WHERE UID = @UID ";

                using (var command = new SqlCommand(commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;

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
        public static List<ClientWorkHoursAllocation> List(int clientID)
        {
            var workAllocationList = new List<ClientWorkHoursAllocation>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                ListOfFields(",","") +
                "   FROM " + tableName +
                "   WHERE  FKCompanyUID = {0}",
                clientID);

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientWorkHoursAllocation clientWork = new ClientWorkHoursAllocation();
                            LoadObject(reader, clientWork);

                            workAllocationList.Add(clientWork);
                        }
                    }
                }
            }

            return workAllocationList;
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
                var commandString = "SELECT MAX(UID) LASTUID FROM " + tableName;

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
        /// Return a string with fields separated by supplied string
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="prefixLine1"></param>
        /// <returns></returns>
        private static string ListOfFields(string separator, string prefixLine1)
        {
            return (

                   prefixLine1 + FieldName.UID.ToString() +
                   separator + FieldName.Date.ToString() +
                   separator + FieldName.FKClientWorkIsFor.ToString() +
                   separator + FieldName.FKCompanyUID.ToString() +
                   separator + FieldName.FKEmployeeUID.ToString() +
                   separator + FieldName.FKInvoiceItemID.ToString() +
                   separator + FieldName.FKInvoiceUID.ToString() +
                   separator + FieldName.FKTaskUID.ToString() +
                   separator + FieldName.Hours.ToString() 
                );
        }

        /// <summary>
        /// Add Employee parameters to the SQL Command.
        /// </summary>
        /// <param name="command"></param>
        private void AddSQLParameters(SqlCommand command)
        {

            command.Parameters.Add(FieldName.UID.ToString(), SqlDbType.BigInt).Value = UID;
            command.Parameters.Add(FieldName.Date.ToString(), SqlDbType.Date).Value = Date;
            command.Parameters.Add(FieldName.FKClientWorkIsFor.ToString(), SqlDbType.BigInt).Value = FKClientWorkIsFor;
            command.Parameters.Add(FieldName.FKCompanyUID.ToString(), SqlDbType.BigInt).Value = FKCompanyUID;
            command.Parameters.Add(FieldName.FKEmployeeUID.ToString(), SqlDbType.BigInt).Value = FKEmployeeUID;
            command.Parameters.Add(FieldName.FKInvoiceItemID.ToString(), SqlDbType.BigInt).Value = FKInvoiceItemID;
            command.Parameters.Add(FieldName.FKInvoiceUID.ToString(), SqlDbType.BigInt).Value = FKInvoiceUID;
            command.Parameters.Add(FieldName.FKTaskUID.ToString(), SqlDbType.BigInt).Value = FKTaskUID;
            command.Parameters.Add(FieldName.Hours.ToString(), SqlDbType.BigInt).Value = Hours;

        }


                /// <summary>
        /// This method loads the information from the sqlreader into the Employee object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="employee"></param>
        private static void LoadObject(SqlDataReader reader, ClientWorkHoursAllocation clientWorkAllocation)
        {
            clientWorkAllocation.UID = Convert.ToInt32(reader[FieldName.UID.ToString()]);
            clientWorkAllocation.Date = Convert.ToDateTime(reader[FieldName.Date.ToString()]);
            clientWorkAllocation.FKClientWorkIsFor = Convert.ToInt32(reader[FieldName.FKClientWorkIsFor.ToString()]);
            clientWorkAllocation.FKCompanyUID = Convert.ToInt32(reader[FieldName.FKCompanyUID.ToString()]);
            clientWorkAllocation.FKEmployeeUID = Convert.ToInt32(reader[FieldName.FKEmployeeUID.ToString()]);
            clientWorkAllocation.FKInvoiceItemID = Convert.ToInt32(reader[FieldName.FKInvoiceItemID.ToString()]);
            clientWorkAllocation.FKInvoiceUID = Convert.ToInt32(reader[FieldName.FKInvoiceUID.ToString()]);
            clientWorkAllocation.FKTaskUID = Convert.ToInt32(reader[FieldName.FKTaskUID.ToString()]);
            clientWorkAllocation.Hours = Convert.ToInt32(reader[FieldName.Hours.ToString()]);
        }
        
    }
}
