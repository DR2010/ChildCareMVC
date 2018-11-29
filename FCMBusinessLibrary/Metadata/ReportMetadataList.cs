using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;

namespace FCMBusinessLibrary
{
    public class ReportMetadataList
    {

        public List<ReportMetadata> reportMetadataList;
        private int _clientUID;

        // -----------------------------------------------------
        //   Constructor using userId and connection string
        // -----------------------------------------------------
        public ReportMetadataList()
        {
        }

        // -----------------------------------------------------
        //    List Global Fields
        // -----------------------------------------------------
        public void ListDefault()
        {
            this.reportMetadataList = new List<ReportMetadata>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "  [UID] " +
                " ,[Description] " +
                " ,[RecordType] " +
                " ,[FieldCode] " +
                " ,[ClientType] " +
                " ,[ClientUID] " +
                " ,[InformationType] " +
                " ,[Condition] " +
                " ,[CompareWith] " +
                " ,[Enabled] " +
                " ,[UseAsLabel] " +
                "   FROM [ReportMetadata] " +
                "  WHERE RecordType = '{0}'",
                "DF");

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           
                            ReportMetadata _reportMetadata = new ReportMetadata();
                            _reportMetadata.UID = Convert.ToInt32(reader["UID"].ToString());
                            _reportMetadata.Description = reader["Description"].ToString();
                            _reportMetadata.RecordType = reader["RecordType"].ToString();
                            _reportMetadata.FieldCode = reader["FieldCode"].ToString();
                            _reportMetadata.ClientType = reader["ClientType"].ToString();
                            _reportMetadata.Condition = reader["Condition"].ToString();
                            _reportMetadata.CompareWith = reader["CompareWith"].ToString();
                            _reportMetadata.Enabled = Convert.ToChar(reader["Enabled"]);
                            try
                            {
                                _reportMetadata.UseAsLabel = Convert.ToChar(reader["UseAsLabel"]);
                            }
                            catch (Exception ex)
                            {
                                _reportMetadata.UseAsLabel = 'N';
                            }
                            try
                            {
                                _reportMetadata.ClientUID = Convert.ToInt32(reader["ClientUID"]);
                            }
                            catch (Exception ex)
                            {
                                _reportMetadata.ClientUID = 0;
                            }
                            
                            _reportMetadata.InformationType = reader["InformationType"].ToString();

                            this.reportMetadataList.Add(_reportMetadata);
                        }
                    }
                }
            }
        }

        // -----------------------------------------------------
        //    List available report metadata
        // -----------------------------------------------------
        public void ListAvailableForClient(int clientUID)
        {
            this.reportMetadataList = new List<ReportMetadata>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "  [UID] " +
                " ,[Description] " +
                " ,[RecordType] " +
                " ,[FieldCode] " +
                " ,[ClientType] " +
                " ,[ClientUID] " +
                " ,[InformationType] " +
                " ,[Condition] " +
                " ,[CompareWith] " +
                "   FROM [ReportMetadata] " +
                "  WHERE RecordType = 'DF' " +
                "    AND FieldCode not in " +
                " ( select FieldCode from reportmetadata where ClientUID = {0}) ",
                clientUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            ReportMetadata _reportMetadata = new ReportMetadata();
                            _reportMetadata.UID = Convert.ToInt32(reader["UID"].ToString());
                            _reportMetadata.Description = reader["Description"].ToString();
                            _reportMetadata.RecordType = reader["RecordType"].ToString();
                            _reportMetadata.FieldCode = reader["FieldCode"].ToString();
                            _reportMetadata.ClientType = reader["ClientType"].ToString();

                            try
                            {
                                _reportMetadata.ClientUID = Convert.ToInt32(reader["ClientUID"]);
                            }
                            catch (Exception ex)
                            {
                                _reportMetadata.ClientUID = 0;
                            }

                            _reportMetadata.InformationType = reader["InformationType"].ToString();
                            _reportMetadata.Condition = reader["Condition"].ToString();
                            _reportMetadata.CompareWith = reader["CompareWith"].ToString();

                            this.reportMetadataList.Add(_reportMetadata);
                        }
                    }
                }
            }
        }

        // -----------------------------------------------------
        //    List metadata for a given client
        // -----------------------------------------------------
        public void ListMetadataForClient(int clientUID, bool onlyEnabled = false)
        {
            this.reportMetadataList = new List<ReportMetadata>();

            var enabledOnlyCriteria = "";
            if (onlyEnabled)
            {
                enabledOnlyCriteria = " AND Enabled = 'Y' ";
            }


            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT " +
                "  [UID] " +
                " ,[Description] " +
                " ,[RecordType] " +
                " ,[FieldCode] " +
                " ,[ClientType] " +
                " ,[ClientUID] " +
                " ,[InformationType] " +
                " ,[Condition] " +
                " ,[CompareWith] " +
                " ,[Enabled] " +
                "   FROM [ReportMetadata] " +
                "  WHERE RecordType = 'CL' " +
                enabledOnlyCriteria + 
                "    AND ClientUID = {0} ",
                clientUID
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            ReportMetadata _reportMetadata = new ReportMetadata();
                            _reportMetadata.UID = Convert.ToInt32(reader["UID"].ToString());
                            _reportMetadata.Description = reader["Description"].ToString();
                            _reportMetadata.RecordType = reader["RecordType"].ToString();
                            _reportMetadata.FieldCode = reader["FieldCode"].ToString();
                            _reportMetadata.ClientType = reader["ClientType"].ToString();
                            _reportMetadata.InformationType = reader["InformationType"].ToString();
                            _reportMetadata.Enabled = Convert.ToChar(reader["Enabled"]);

                            try
                            {
                                _reportMetadata.ClientUID = Convert.ToInt32(reader["ClientUID"]);
                            }
                            catch (Exception ex)
                            {
                                _reportMetadata.ClientUID = 0;
                            }

                            _reportMetadata.InformationType = reader["InformationType"].ToString();
                            _reportMetadata.Condition = reader["Condition"].ToString();
                            _reportMetadata.CompareWith = reader["CompareWith"].ToString();

                            this.reportMetadataList.Add(_reportMetadata);
                        }
                    }
                }
            }
        }

    }
}
