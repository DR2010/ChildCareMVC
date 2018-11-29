using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FCMBusinessLibrary.Document;

namespace FCMBusinessLibrary
{

    public struct scDocoSetDocumentLink
    {
        public Document.Document document;
        public DocumentSet documentSet;
        public DocumentSetDocument DocumentSetDocument;
    }

    public struct ListOfscDocoSetDocumentLink
    {
        public List<scDocoSetDocumentLink> list;
    }

    public struct scClientDocSetDocLink
    {
        public Document.Document document;
        public ClientDocumentSet clientDocumentSet;
        public ClientDocument clientDocument;
    }

    public struct ListOfscClientDocSetDocLink
    {
        public List<scClientDocSetDocLink> list;
    }

    public struct scDocumentSetDocumentLink
    {
        public DocumentSetDocument dsdparent;
        public DocumentSetDocument dsdschild;
        public Document.Document documentChild;
        public DocumentSetDocumentLink dsdlink;

    }

}
