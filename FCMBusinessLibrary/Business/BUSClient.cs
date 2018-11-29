using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Transactions;
using FCMBusinessLibrary.Client;

namespace FCMBusinessLibrary.Business
{
    public class BUSClient
    {


        /// <summary>
        /// Add client
        /// </summary>
        /// <param name="headerInfo"> </param>
        /// <param name="eventClient"></param>
        /// <param name="linkInitialSet"> </param>
        /// <returns></returns>
        public static ResponseStatus ClientAdd(HeaderInfo headerInfo,
                                                        Client.Client eventClient,
                                                        string linkInitialSet)
        {
            ResponseStatus response = new ResponseStatus();

            //
            // This is a new client.
            //
            if (string.IsNullOrEmpty(eventClient.Name))
            {
                response.ReturnCode = -0010;
                response.ReasonCode =  0001;
                response.Message = "Client Name is mandatory.";
                response.Contents = 0;
                return response;
            }

            // --------------------------------------------------------------
            // Check if user ID is already connected to a client
            // --------------------------------------------------------------
            #region Check if user is already connected to a client
            if ( ! string.IsNullOrEmpty(eventClient.FKUserID) )
            {
                var checkLinkedUser = new Client.Client {FKUserID = eventClient.FKUserID};
                var responseLinked = checkLinkedUser.ReadLinkedUser();

                if (!responseLinked.Successful)
                {
                    return responseLinked;
                }

                if (responseLinked.ReturnCode == 0001 && responseLinked.ReasonCode == 0001)
                {
                    response.ReturnCode = -0010;
                    response.ReasonCode = 0002;
                    response.Message = "User ID is already linked to another client.";
                    response.Contents = 0;

                    return response;
                }

            }
            #endregion

            var newClientUid = 0;

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                using (var tr = new TransactionScope(TransactionScopeOption.Required))
                {
                    connection.Open();

                    // -------------------------------
                    // Call method to add new client
                    // -------------------------------
                    var newClient = eventClient.Insert(headerInfo, connection);
             //       var newClientX = eventClient.MySQLInsert(headerInfo);

                    newClientUid = Convert.ToInt32(newClient.Contents);

                    // -------------------------------------------
                    // Call method to add client extra information
                    // -------------------------------------------
                    eventClient.clientExtraInformation.FKClientUID = eventClient.UID;
                    var cei = ClientExtraInformation.Insert(
                                    HeaderInfo.Instance,
                                    eventClient.clientExtraInformation, 
                                    connection);

                    if (cei.ReturnCode != 1)
                    {
                        // Rollback transaction
                        //
                        tr.Dispose();
                        return cei;
                    }

                    // --------------------------------------------
                    // Add first document set
                    // --------------------------------------------
                    var cds = new ClientDocumentSet();
                    cds.FKClientUID = newClientUid;

                    // cds.FolderOnly = "CLIENT" + newClientUID.ToString().Trim().PadLeft(4, '0');
                    cds.FolderOnly = "CLIENT" + newClientUid.ToString(CultureInfo.InvariantCulture).Trim().PadLeft(4, '0');

                    // cds.Folder = FCMConstant.SYSFOLDER.CLIENTFOLDER + "\\CLIENT" + newClientUID.ToString().Trim().PadLeft(4, '0');
                    cds.Folder = FCMConstant.SYSFOLDER.CLIENTFOLDER + @"\" + cds.FolderOnly;

                    cds.SourceFolder = FCMConstant.SYSFOLDER.TEMPLATEFOLDER;
                    cds.Add(headerInfo, connection);

                    // --------------------------------------------
                    // Apply initial document set
                    // --------------------------------------------
                    if (linkInitialSet == "Y")
                    {
                        BUSClientDocument.AssociateDocumentsToClient(
                                clientDocumentSet: cds,
                                documentSetUID: eventClient.FKDocumentSetUID,
                                headerInfo: headerInfo);

                        // Fix Destination Folder Location 
                        //
                        BUSClientDocumentGeneration.UpdateLocation(cds.FKClientUID, cds.ClientSetID);
                    }

                    // Commit transaction
                    //
                    tr.Complete();
                }
            }

            ClientList(headerInfo);

            // Return new client id
            response.Contents = newClientUid;
            return response;
        }

        /// <summary>
        /// Update client details
        /// </summary>
        /// <param name="headerInfo"> </param>
        /// <param name="eventClient"> </param>
        public static ResponseStatus ClientUpdate(HeaderInfo headerInfo, Client.Client eventClient)
        {
            var response = new ResponseStatus();

            // --------------------------------------------------------------
            // Check if user ID is already connected to a client
            // --------------------------------------------------------------
            var checkLinkedUser = new Client.Client
                                      {
                                          FKUserID = eventClient.FKUserID, 
                                          UID = eventClient.UID
                                      };

            if ( ! string.IsNullOrEmpty(eventClient.FKUserID))
            {
                var responseLinked = checkLinkedUser.ReadLinkedUser();

                if (responseLinked.ReturnCode == 0001 && responseLinked.ReasonCode == 0001)
                {
                    response.ReturnCode = 0001;
                    response.ReasonCode = 0002;
                    response.Message = "User ID is already linked to another client.";
                    response.Contents = 0;

                    return response;
                }

                if (responseLinked.ReturnCode == 0001 && responseLinked.ReasonCode == 0003)
                {
                    // All good. User ID is connected to Client Supplied.
                }


                if (responseLinked.ReturnCode == 0001 && responseLinked.ReasonCode == 0001)
                {
                    response.ReturnCode = -0010;
                    response.ReasonCode = 0002;
                    response.Message = "User ID is already linked to another client.";
                    response.Contents = 0;

                    return response;
                }
            }


            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                using (var tr = new TransactionScope(TransactionScopeOption.Required))
                {


                    connection.Open();

                    var responseClientUpdate = eventClient.Update(headerInfo);
                    if (!responseClientUpdate.Successful)
                    {
                        // Rollback
                        tr.Dispose();
                        return responseClientUpdate;
                    }
                    // -------------------------------------------
                    // Call method to add client extra information
                    // -------------------------------------------

                    var ceiResponse = ClientExtraInformation.Read(headerInfo, eventClient.UID);
                    if (ceiResponse.ReturnCode != 1)
                    {
                        // Rollback
                        tr.Dispose();
                        return ceiResponse;
                    }
                    var ceiRead = (ClientExtraInformation) ceiResponse.Contents;

                    eventClient.clientExtraInformation.RecordVersion = ceiRead.RecordVersion;

                    var cei = ClientExtraInformation.Update(
                        headerInfo,
                        eventClient.clientExtraInformation);
                    if (!cei.Successful)
                        return cei;

                    tr.Complete();
                }
            }

            return response;
        }

        /// <summary>
        /// Retrieve client details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="clientUID"> </param>
        public static ResponseStatus ClientRead(int clientUID)
        {
            var response = new ResponseStatus();

            var client = new Client.Client {UID = clientUID};
            client.Read();

            client.clientExtraInformation = new ClientExtraInformation {FKClientUID = client.UID};
            client.clientExtraInformation.Read();

            if (client.UID > 0)
            {
                ReadLogoStatus(client);
            }

            response.Contents = client;
            return response;
        }

        /// <summary>
        /// Client Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="eventClient"> </param>
        public static ResponseStatus ClientDelete(Client.Client eventClient)
        {
            var response = eventClient.Delete();

            return response;
        }

        /// <summary>
        /// List of clients
        /// </summary>
        public static ResponseStatus ClientList(HeaderInfo headerInfo)
        {
            var response = new ResponseStatus { Contents = Client.Client.List(headerInfo) };


            return response;
        }

        /// <summary>
        /// Read specified field from client table.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="clientUID"></param>
        /// <returns></returns>
        public static ResponseStatus ReadFieldClient(string field, int clientUID)
        {
            var response = new ResponseStatus();

            var responseReadFieldClient = Client.Client.ReadFieldClient(field, clientUID);

            response.Contents = responseReadFieldClient;

            return response;
        }

        /// <summary>
        /// Update logo display flag
        /// </summary>
        private static void UpdateLogoStatus(Client.Client eventClient)
        {
            // Update Logo Display Status
            //
            var rmd = new ReportMetadata
                          {
                              ClientUID = Utils.ClientID,
                              RecordType = Utils.MetadataRecordType.CLIENT,
                              FieldCode = Utils.FieldCode.COMPANYLOGO
                          };


            if (rmd.Read(clientUID: Utils.ClientID, fieldCode: Utils.FieldCode.COMPANYLOGO))
            {
                rmd.Enabled = eventClient.DisplayLogo;
                rmd.Save();
            }

            return;
        }



        /// <summary>
        /// Read logo display flag
        /// </summary>
        private static void ReadLogoStatus(Client.Client eventClient)
        {
            // Update Logo Display Status
            //
            ReportMetadata rmd = new ReportMetadata();
            rmd.ClientUID = Utils.ClientID;
            rmd.RecordType = Utils.MetadataRecordType.CLIENT;
            rmd.FieldCode = Utils.FieldCode.COMPANYLOGO;

            if (rmd.Read(clientUID: Utils.ClientID, fieldCode: Utils.FieldCode.COMPANYLOGO))
            {
                eventClient.DisplayLogo = rmd.Enabled;
            }
        }
   
    }
}
