using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FCMBusinessLibrary
{
    /// <summary>
    /// The data contract for a document
    /// </summary>
    public class DCDocument
    {
        public int UID;
        /// <summary>
        /// CUID represents a character unique ID
        /// </summary>
        public string CUID;
        /// <summary>
        /// It is the main name of the document.
        /// DB Type: Varchar(100) 
        /// </summary>
        public string Name; // varchar(100)
        public int SequenceNumber;  
        public int IssueNumber; // 
        public string Location; // varchar(100)
        public string Comments; // varchar(100)
        public string FileName;// varchar(200)
        /// <summary>
        /// Indicates the source of the document. It could be from FCM or a Client Document.
        /// </summary>
        public string SourceCode; // varchar(20)
        public int FKClientUID;
        public int ParentUID;
        /// <summary>
        /// Indicates if it is a Folder, Document or Appendix
        /// </summary>
        public string RecordType;
        /// <summary>
        /// Indicates if the document is a project plan.
        /// Project plans are special and can hold other documents.
        /// </summary>
        public char IsProjectPlan;
        /// <summary>
        /// Indicates the type of document: Word, Excel, PDF, Undefined etc.
        /// Use Utils.DocumentType
        /// </summary>
        public string DocumentType;
        /// <summary>
        /// It includes the client and version number of the document
        /// </summary>
        public string ComboIssueNumber;
        /// <summary>
        /// It does not include the prefix (CUID) or version number
        /// </summary>
        public string SimpleFileName;
        /// <summary>
        /// Indicates whether the record is logically deleted.
        /// </summary>
        public char IsVoid;
        /// <summary>
        /// Document Status (Draft, Finalised, Deleted)
        /// </summary>
        public string Status;

        // Audit fields
        //
        public DateTime CreationDateTime;
        public DateTime UpdateDateTime;
        public string UserIdCreatedBy;
        public string UserIdUpdatedBy;

    }
}
