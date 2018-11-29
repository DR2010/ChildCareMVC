using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Data.SqlClient;


namespace fcm
{
    class Metadata
    {
        public string RecordType;
        public string FieldCode;
        public string InformationType;
        public string Table;
        public string Field;
        public string Condition;
        public string CompareWith;

        public string Get()
        {
            string ret = "";

            string comp = "";
            string select = "";

            // Source Memory Information
            switch (this.CompareWith)
            {
                case "CLIENTUID":
                    comp = Utils.ClientID.ToString();
                    break;

            }


            if (this.InformationType == "FIELD")
            {
                 select = " SELECT " + this.Field +
                          " FROM " + this.Table +
                          " WHERE " + this.Condition + comp;

            }

            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString = select;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = reader[Field].ToString();
                    }
                }
            }

            return ret;
        }

    }
}
