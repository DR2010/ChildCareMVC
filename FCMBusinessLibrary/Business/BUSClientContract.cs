using System;
using FCMBusinessLibrary.DataAccess;

namespace FCMBusinessLibrary.Business
{
    public static class BUSClientContract
    {
        /// <summary>
        /// Client contract add
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus ClientContractAdd(ClientContract clientContract)
        {
            var response = clientContract.Insert();
            return response;
        }

        /// <summary>
        /// Client contract list
        /// </summary>
        /// <param name="clientContractUID"> </param>
        /// <returns></returns>
        public static ResponseStatus ClientContractList(int clientContractUID)
        {
            var response = new ResponseStatus
                               {
                                   Contents = ClientContract.List(clientContractUID)
                               };
            return response;
        }

        /// <summary>
        /// Client contract read
        /// </summary>
        /// <param name="clientContractUID"> </param>
        /// <returns></returns>
        public static ResponseStatus Read(int clientContractUID)
        {
            var response = new ResponseStatus
            {
                Contents = ClientContract.Read(clientContractUID)
            };
            return response;
        }


        /// <summary>
        /// Update details of a client's contract
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus ClientContractUpdate(ClientContract clientContract)
        {

            var response = new ResponseStatus
                               {
                                   Contents = clientContract.Update()
                               };
            return response;
        }

        /// <summary>
        /// Delete a client's contract
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus ClientContractDelete(ClientContract clientContract)
        {
            var response = new ResponseStatus
                               {
                                   Contents = clientContract.Delete()
                               };

            return response;
        }

        /// <summary>
        /// Client contract list
        /// </summary>
        /// <returns></returns>
        public static ResponseStatus GetValidContractOnDate(int clientContractUID, DateTime date)
        {
            var response = ClientContract.GetValidContractOnDate(clientContractUID, date);
            return response;
        }
    }
}
