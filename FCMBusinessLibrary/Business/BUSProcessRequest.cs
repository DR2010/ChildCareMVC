using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary.Business
{
    public class BUSProcessRequest
    {
        /// <summary>
        /// Add Process Request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static ResponseStatus Add(ProcessRequest processRequest)
        {
            ResponseStatus response = new ResponseStatus();
            response = processRequest.Add();

            return response;
        }

        /// <summary>
        /// Add Document Generation for Client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static ResponseStatus GenerateDocumentClient(int clientUID, int clientSetID, string overrideDocuments )
        {
            ResponseStatus response = new ResponseStatus();

            ProcessRequest processRequest = new ProcessRequest();
            processRequest.Description = "Document Generation For Client " + clientUID.ToString() + " Set " + clientSetID.ToString();
            processRequest.Type = ProcessRequest.TypeValue.DOCUMENTGENERATION.ToString();
            processRequest.FKClientUID = clientUID;

            response = processRequest.Add();

            ProcessRequestArguments argument1 = new ProcessRequestArguments();
            argument1.FKRequestUID = processRequest.UID;
            argument1.Code = ProcessRequestArguments.CodeValue.CLIENTUID.ToString();
            argument1.ValueType = ProcessRequestArguments.ValueTypeValue.NUMBER.ToString();
            argument1.Value = clientUID.ToString();
            argument1.Add();

            ProcessRequestArguments argument2 = new ProcessRequestArguments();
            argument2.FKRequestUID = processRequest.UID;
            argument2.Code = ProcessRequestArguments.CodeValue.CLIENTSETID.ToString();
            argument2.ValueType = ProcessRequestArguments.ValueTypeValue.NUMBER.ToString();
            argument2.Value = clientSetID.ToString();
            argument2.Add();

            ProcessRequestArguments argument3 = new ProcessRequestArguments();
            argument3.FKRequestUID = processRequest.UID;
            argument3.Code = ProcessRequestArguments.CodeValue.OVERRIDE.ToString();
            argument3.ValueType = ProcessRequestArguments.ValueTypeValue.STRING.ToString();
            argument3.Value = overrideDocuments;
            argument3.Add();

            return response;
        }

        /// <summary>
        /// Add Process Request Argument
        /// </summary>
        /// <param name="processRequestArgument"></param>
        /// <returns></returns>
        public static ResponseStatus AddArgument(ProcessRequestArguments processRequestArgument)
        {
            ResponseStatus response = new ResponseStatus();
            response = processRequestArgument.Add();

            return response;
        }

        /// <summary>
        /// Add process request results
        /// </summary>
        /// <param name="processRequestResults"></param>
        /// <returns></returns>
        public static ResponseStatus AddArgument(ProcessRequestResults processRequestResults)
        {
            ResponseStatus response = new ResponseStatus();
            response = processRequestResults.Add();

            return response;
        }


        /// <summary>
        /// List Active Process Requests
        /// </summary>
        /// <param name="processRequestResults"></param>
        /// <returns></returns>
        public static List<ProcessRequest> ListActiveRequests()
        {
            var statusIn = ProcessRequest.StatusValue.OPEN;
            List<ProcessRequest> response = ProcessRequest.List(statusIn);

            return response;
        }
    }
}
