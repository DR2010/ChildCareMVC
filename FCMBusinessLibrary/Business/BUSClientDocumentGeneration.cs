using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary.Business
{
    public class BUSClientDocumentGeneration
    {
        public static ResponseStatus UpdateLocation(int clientID, int clientDocumentSetUID)
        {
            var response = new ResponseStatus();
            response = DocumentGeneration.UpdateDestinationFolder(clientID, clientDocumentSetUID);
            return response;
        }

    }
}
