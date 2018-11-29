using System;
using System.Globalization;
using FCMBusinessLibrary.Document;
using MySql.Data.MySqlClient;

namespace FCMBusinessLibrary.Client
{
    public partial class Client
    {
        /// <summary>
        /// Get client details  
        /// </summary>
        internal void MySQLReadMySQL()
        {

            // ----------------------------
            // EA SQL database
            // ----------------------------

            using (var connection = new MySqlConnection(ConnString.ConnectionStringMySql))
            {
                var commandString =
                " SELECT " +
                ClientFieldString() +
                "  FROM [management].[dbo].[Client]" +
                " WHERE UID = @UID";

                using (var command = new MySqlCommand(commandString, connection))
                {
                    command.Parameters.Add("@UID", MySqlDbType.UInt32).Value = UID;

                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            LoadClientObject(reader, this);
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
        /// Add new Client
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus MySQLInsert(HeaderInfo headerInfo)
        {

            var response = new ResponseStatus();
            response.ReturnCode = 1;
            response.ReasonCode = 1;
            response.Message = "Client Added Successfully";


            int uid = 0;

            int nextUID = GetLastUID() + 1; // 2010100000

            if (nextUID == 1)
            {
                nextUID = DateTime.Now.Year * 100000 + 1;
            }

            uid = DateTime.Now.Year * 100000 + (Convert.ToInt32(nextUID.ToString(CultureInfo.InvariantCulture).Substring(4, 5)));

            UID = uid;

            RecordVersion = 1;

            if (String.IsNullOrEmpty(Name))
            {
                response.ReturnCode = -10;
                response.ReasonCode = 1;
                response.Message = "Error: Client Name is Mandatory.";
                return response;
            }
            if (Address == null)
                Address = "";

            if (MainContactPersonName == null)
                MainContactPersonName = "";

            var connection = new MySqlConnection(ConnString.ConnectionStringMySql);
            connection.Open();

            var commandString =
            (
                "INSERT INTO Client " +
                "(" +
                ClientFieldString() +
                ")" +
                    " VALUES " +
                ClientFieldValue()

                );

            using (var command = new MySqlCommand( commandString, connection))
            {
                RecordVersion = 1;
                AddSqlParameters(command, FCMConstant.SQLAction.CREATE, headerInfo);

                command.ExecuteNonQuery();
            }

            response.Contents = uid;

            return response;
        }


        /// <summary>
        /// MYSQL Load client object
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="client"> </param>
        private static void LoadClientObject(MySqlDataReader reader, Client client)
        {
            client.UID = Convert.ToInt32(reader[FieldName.UID]);
            client.RecordVersion = Convert.ToInt32(reader[FieldName.RecordVersion]);
            client.ABN = reader[FieldName.ABN].ToString();
            client.Name = reader[FieldName.Name].ToString();
            client.LegalName = reader[FieldName.LegalName].ToString();
            client.Address = reader[FieldName.Address].ToString();
            client.EmailAddress = reader[FieldName.EmailAddress].ToString();
            client.Phone = reader[FieldName.Phone].ToString();
            try { client.FKUserID = reader[FieldName.FKUserID].ToString(); }
            catch { client.FKUserID = ""; }
            try { client.FKDocumentSetUID = Convert.ToInt32(reader[FieldName.FKDocumentSetUID]); }
            catch { client.FKDocumentSetUID = 0; }
            try { client.Fax = reader[FieldName.Fax].ToString(); }
            catch { client.Fax = ""; }
            try { client.Mobile = reader[FieldName.Mobile].ToString(); }
            catch { client.Mobile = ""; }
            try { client.Logo1Location = reader[FieldName.Logo1Location].ToString(); }
            catch { client.Logo1Location = ""; }
            try { client.Logo2Location = reader[FieldName.Logo2Location].ToString(); }
            catch { client.Logo2Location = ""; }
            try { client.Logo3Location = reader[FieldName.Logo3Location].ToString(); }
            catch { client.Logo3Location = ""; }

            try { client.MainContactPersonName = reader[FieldName.MainContactPersonName].ToString(); }
            catch { client.MainContactPersonName = ""; }
            try { client.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString()); }
            catch { client.UpdateDateTime = DateTime.Now; }
            try { client.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString()); }
            catch { client.CreationDateTime = DateTime.Now; }
            try { client.IsVoid = reader[FieldName.IsVoid].ToString(); }
            catch { client.IsVoid = "N"; }
            try { client.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { client.UserIdCreatedBy = "N"; }
            try { client.UserIdUpdatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            catch { client.UserIdCreatedBy = "N"; }

            client.DocSetUIDDisplay = "0; 0";
            if (client.FKDocumentSetUID > 0)
            {
                var ds = new DocumentSet();
                ds.UID = client.FKDocumentSetUID;
                ds.Read('N');
                client.DocSetUIDDisplay = ds.UID + "; " + ds.TemplateType;
            }

        }


        /// <summary>
        /// Add SQL Parameters
        /// </summary>
        /// <param name="_uid"></param>
        /// <param name="command"></param>
        /// <param name="action"></param>
        /// <param name="headerInfo"> </param>
        private void AddSqlParameters(MySqlCommand command, string action, HeaderInfo headerInfo)
        {
            command.Parameters.Add("@UID", MySqlDbType.UInt32).Value = UID;
            command.Parameters.Add("@ABN", MySqlDbType.VarChar).Value = ABN;
            command.Parameters.Add("@Name", MySqlDbType.VarChar).Value = Name;
            command.Parameters.Add("@LegalName", MySqlDbType.VarChar).Value = LegalName;
            command.Parameters.Add("@Address", MySqlDbType.VarChar).Value = Address;
            command.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = Phone;
            command.Parameters.Add("@EmailAddress", MySqlDbType.VarChar).Value = EmailAddress;
            command.Parameters.Add("@Fax", MySqlDbType.VarChar).Value = Fax;
            command.Parameters.Add("@Mobile", MySqlDbType.VarChar).Value = Mobile;
            command.Parameters.Add("@Logo1Location", MySqlDbType.VarChar).Value = Logo1Location;
            command.Parameters.Add("@Logo2Location", MySqlDbType.VarChar).Value = Logo2Location;
            command.Parameters.Add("@Logo3Location", MySqlDbType.VarChar).Value = Logo3Location;
            command.Parameters.Add("@MainContactPersonName", MySqlDbType.VarChar).Value = MainContactPersonName;
            command.Parameters.Add("@DisplayLogo", MySqlDbType.VarChar).Value = DisplayLogo;
            command.Parameters.Add("@IsVoid", MySqlDbType.VarChar).Value = "N";
            command.Parameters.Add("@FKUserID", MySqlDbType.VarChar).Value = FKUserID;
            command.Parameters.Add("@FKDocumentSetUID", MySqlDbType.UInt32).Value = FKDocumentSetUID;
            command.Parameters.Add("@UpdateDateTime", MySqlDbType.DateTime, 8).Value = headerInfo.CurrentDateTime;
            command.Parameters.Add("@UserIdUpdatedBy", MySqlDbType.VarChar).Value = headerInfo.UserID;

            if (action == FCMConstant.SQLAction.CREATE)
            {
                command.Parameters.Add("@CreationDateTime", MySqlDbType.DateTime, 8).Value = headerInfo.CurrentDateTime;
                command.Parameters.Add("@UserIdCreatedBy", MySqlDbType.VarChar).Value = headerInfo.UserID;
            }

            command.Parameters.Add("@recordVersion", MySqlDbType.UInt32).Value = RecordVersion;

        }


    }
}
