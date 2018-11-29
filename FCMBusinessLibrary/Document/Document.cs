using System;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using FCMBusinessLibrary.ReferenceData;

namespace FCMBusinessLibrary.Document
{
    /// <summary>
    /// It represents a document, folder or appendix.
    /// </summary>
    public class Document
    {
        #region Properties
        public int UID { get; set; }
        public string CUID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int SequenceNumber { get; set; }
        public int IssueNumber { get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
        public string FileExtension { get; set; }
        /// <summary>
        /// It is the file name with extension
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Indicates the source of the document. It could be from FCM or a Client Document.
        /// </summary>
        public string SourceCode { get; set; }
        public int FKClientUID { get; set; }
        public int ParentUID { get; set; }
        /// <summary>
        /// Indicates if it is a Folder, Document or Appendix
        /// </summary>
        public string RecordType { get; set; }
        /// <summary>
        /// Indicates if the document is a project plan.
        /// Project plans are special and can hold other documents.
        /// </summary>
        public string IsProjectPlan { get; set; }
        /// <summary>
        /// Indicates the type of document: Word, Excel, PDF, Undefined etc.
        /// Use Utils.DocumentType
        /// </summary>
        public string DocumentType { get; set; }
        /// <summary>
        /// It includes the client and version number of the document
        /// </summary>
        // public string ComboIssueNumber { get { return _ComboIssueNumber; } set { _ComboIssueNumber = value; } }
        /// <summary>
        /// It does not include the prefix (CUID) or version number
        /// </summary>
        public string SimpleFileName { get; set; }
        /// <summary>
        /// Indicates whether the record is logically deleted.
        /// </summary>
        public string IsVoid { get; set; }
        /// <summary>
        /// Document Status (Draft, Finalised, Deleted)
        /// </summary>
        public string Status { get; set; }
        public int RecordVersion { get; set; }

        // Audit fields
        //
        public string UserIdCreatedBy { get; set; }
        public string UserIdUpdatedBy { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        #endregion Properties

        /// <summary>
        /// Database fields
        /// </summary>
        public struct FieldName
        {
            public const string UID = "UID";
            public const string SimpleFileName = "SimpleFileName";
            public const string CUID = "CUID";
            public const string Name = "Name";
            public const string DisplayName = "DisplayName";
            public const string SequenceNumber = "SequenceNumber";
            public const string IssueNumber = "IssueNumber";
            public const string Location = "Location";
            public const string Comments = "Comments";
            public const string FileName = "FileName";
            public const string SourceCode = "SourceCode";
            public const string FKClientUID = "FKClientUID";
            public const string IsVoid = "IsVoid";
            public const string ParentUID = "ParentUID";
            public const string RecordType = "RecordType";
            public const string FileExtension = "FileExtension";
            public const string IsProjectPlan = "IsProjectPlan";
            public const string DocumentType = "DocumentType";
            public const string Status = "Status";
            public const string RecordVersion = "RecordVersion";

            public const string CreationDateTime = "CreationDateTime";
            public const string UpdateDateTime = "UpdateDateTime";
            public const string UserIdCreatedBy = "UserIdCreatedBy";
            public const string UserIdUpdatedBy = "UserIdUpdatedBy";
        }

        // --------------------------------------------------
        // Retrieve name of the document
        // --------------------------------------------------
        public static string GetName(int documentUID)
        {
            string ret = "";
            // 
            // EA SQL database
            // 

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                " SELECT [Name] " +
                "  FROM [Document]" +
                " WHERE UID = " + documentUID;

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        try
                        {
                            ret = reader["Name"].ToString();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Get Document Details
        /// </summary>
        /// <returns></returns>
        public bool Read(bool includeVoid = false)
        {
            // 
            // EA SQL database
            // 
            bool ret = false;
            string commandString = "";

            string sincludeVoid = " AND DOC.IsVoid = 'N' ";

            if (includeVoid)
                sincludeVoid = "  ";

            if (this.UID > 0)
            {
                commandString = string.Format( 
                    " SELECT "+
                    Document.SQLDocumentConcat("DOC") +
                    "  FROM [Document] DOC" +
                    " WHERE " +
                    "      DOC.UID = {0} " +
                    sincludeVoid,
                    UID);
            }
            else
            {
                commandString = string.Format(
                    " SELECT " +
                    Document.SQLDocumentConcat("DOC") +
                    "  FROM [Document] DOC " +
                    " WHERE " +
                    "      DOC.IsVoid = 'N' " +
                    "  AND DOC.CUID = '{0}' ",
                    CUID );
            }

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                      connection.Open();
                      SqlDataReader reader = command.ExecuteReader();

                      if (reader.Read())
                      {
                          try
                          {
                              LoadDocumentFromReader(this, "DOC", reader);
                              ret = true;
                          }
                          catch (Exception)
                          {
                              CUID = "";
                              UID = 0;
                              this.Name = "Not found UID " + this.UID.ToString("0000") + " CUID: " + this.CUID;
                              this.FileName = "Not found UID " + this.UID.ToString("0000") + " CUID: " + this.CUID;
                              this.RecordType = ""; 
                          }
                      }
                  }
            }
            return ret;
        }

        public string GetDocumentLocationAndName()
        {
            string ret = "";

            Read();

            ret = Utils.getFilePathName(this.Location, this.Name);

            return ret;
        }

        //
        // Set void flag (Logical Delete)
        //
        public void SetToVoid(int DocumentUID)
        {
            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [Document] " +
                   " SET " +
                   " [IsVoid] = @IsVoid" +
                   " WHERE [UID] = @UID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@UID", SqlDbType.BigInt).Value = DocumentUID;
                    command.Parameters.Add("@IsVoid", SqlDbType.Char).Value = 'Y';

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return;
        }

        //
        // Physical Delete
        //
        public void Delete( int DocumentUID )
        {
            string ret = "Item updated successfully";

            using (var connection = new SqlConnection( ConnString.ConnectionString ))
            {

                var commandString =
                (
                   "DELETE [Document] " +
                   " WHERE [UID] = @UID "
                );



                using (var command = new SqlCommand(
                                            commandString, connection ))
                {
                    command.Parameters.Add( "@UID", SqlDbType.BigInt ).Value = DocumentUID;

                    connection.Open();
                    command.ExecuteNonQuery();
                }

            }
            return;
        }


        /// <summary>
        /// Add new Document
        /// </summary>
        /// <returns></returns>
        private int Add(HeaderInfo headerInfo)
        {

            string ret = "Item updated successfully";
            int _uid = 0;
            _uid = GetLastUID() + 1;

            this.UID = _uid;
            if (string.IsNullOrEmpty(this.Status))
                this.Status = "ACTIVE";
            this.UserIdCreatedBy = Utils.UserID;
            this.UserIdUpdatedBy = Utils.UserID;
            this.CreationDateTime = System.DateTime.Now;
            this.UpdateDateTime = System.DateTime.Now;
            this.IsVoid = "N";
            this.RecordVersion = 1;
            if (string.IsNullOrEmpty(this.DisplayName))
                this.DisplayName = "TBA";


            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {
                var commandString =
                (
                        "INSERT INTO [Document] " +
                        "( " + 
                        DocumentFieldString() +
                        ")" +
                            " VALUES " +
                        "( "+
                    "  @" + FieldName.UID +
                    ", @" + FieldName.CUID +
                    ", @" + FieldName.Name +
                    ", @" + FieldName.DisplayName +
                    ", @" + FieldName.SequenceNumber +
                    ", @" + FieldName.IssueNumber +
                    ", @" + FieldName.Location +
                    ", @" + FieldName.Comments +
                    ", @" + FieldName.FileName +
                    ", @" + FieldName.Status +
                    ", @" + FieldName.SimpleFileName +
                    ", @" + FieldName.SourceCode +
                    ", @" + FieldName.FKClientUID +
                    ", @" + FieldName.IsVoid  +
                    ", @" + FieldName.ParentUID +
                    ", @" + FieldName.RecordType +
                    ", @" + FieldName.FileExtension +
                    ", @" + FieldName.IsProjectPlan +
                    ", @" + FieldName.DocumentType +
                    ", @" + FieldName.RecordVersion +
                    ", @" + FieldName.UpdateDateTime +
                    ", @" + FieldName.CreationDateTime +
                    ", @" + FieldName.UserIdCreatedBy +
                    ", @" + FieldName.UserIdUpdatedBy +
                        " ) "

                        );

                using (var command = new SqlCommand(
                                          commandString, connection))
              
                                          {
                  command.Parameters.Add("@UID", SqlDbType.BigInt).Value = UID;
                  command.Parameters.Add("@CUID", SqlDbType.VarChar).Value = CUID;
                  command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Name;
                  command.Parameters.Add("@DisplayName", SqlDbType.VarChar).Value = DisplayName;
                  command.Parameters.Add("@SequenceNumber", SqlDbType.VarChar).Value = SequenceNumber;
                  command.Parameters.Add("@IssueNumber", SqlDbType.Decimal).Value = IssueNumber;
                  command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                  command.Parameters.Add("@Comments", SqlDbType.VarChar).Value = Comments;
                  command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;
                  command.Parameters.Add("@SimpleFileName", SqlDbType.VarChar).Value = SimpleFileName;
                  command.Parameters.Add("@SourceCode", SqlDbType.VarChar).Value = SourceCode;
                  command.Parameters.Add("@FKClientUID", SqlDbType.BigInt).Value = FKClientUID;
                  command.Parameters.Add("@IsVoid", SqlDbType.VarChar).Value = IsVoid;
                  command.Parameters.Add("@ParentUID", SqlDbType.VarChar).Value = ParentUID;
                  command.Parameters.Add("@RecordType", SqlDbType.VarChar).Value = RecordType;
                  command.Parameters.Add("@IsProjectPlan", SqlDbType.VarChar).Value = IsProjectPlan;
                  command.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = DocumentType;
                  command.Parameters.Add("@FileExtension", SqlDbType.VarChar).Value = FileExtension;
                  command.Parameters.Add("@Status", SqlDbType.VarChar).Value = Status;
                  command.Parameters.Add("@RecordVersion", SqlDbType.VarChar).Value = RecordVersion;
                  command.Parameters.Add("@UserIdCreatedBy", SqlDbType.VarChar).Value = UserIdCreatedBy;
                  command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = UserIdUpdatedBy;
                  command.Parameters.Add("@CreationDateTime", SqlDbType.DateTime).Value = CreationDateTime;
                  command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime).Value = UpdateDateTime;

                  connection.Open();

                  try
                  {
                      command.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                      LogFile.WriteToTodaysLogFile(
                          "Error adding new document." + ex.ToString(),
                          headerInfo.UserID,
                          "Document.cs"
                          );
                  }
                }
          }
          return _uid;
      }

        // -----------------------------------------------------
        //    Update Document Version
        // -----------------------------------------------------
        private void UpdateVersion(HeaderInfo headerInfo)
        {

            string ret = "Item updated successfully";

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "UPDATE [Document] " +
                   " SET " +
                   " [Name] =  @Name " +
                   ",[IssueNumber] = @IssueNumber" +
                   ",[Location] = @Location" +
                   ",[FileName] = @FileName" +
                   " WHERE [CUID] = @CUID "
                );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@CUID", SqlDbType.VarChar).Value = CUID;
                    command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Name;
                    command.Parameters.Add("@IssueNumber", SqlDbType.Decimal).Value = IssueNumber;
                    command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                    command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;

                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogFile.WriteToTodaysLogFile(
                            "Error updating document version." + ex.ToString(),
                            headerInfo.UserID,
                            "Document.cs"
                            );
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Get Root document. If it does not exits, create a root document.
        /// </summary>
        public void GetRoot( HeaderInfo headerInfo )
        {

            this.CUID = "ROOT";
            this.Name = "FCM Documents";
            this.RecordType = Utils.RecordType.FOLDER;
            this.UID = 0;

            if (!this.Read())
            {
                // Create root
                //
                this.CUID = "ROOT";
                this.RecordVersion = 1;
                this.Name = "FCM Documents";
                this.DisplayName = "FCM Documents";
                this.RecordType = Utils.RecordType.FOLDER;
                this.Comments = "Created automatically.";
                this.DocumentType = Utils.DocumentType.FOLDER;
                this.FileName = "ROOT";
                this.FKClientUID = 0;
                this.IsProjectPlan = "N";
                this.IssueNumber = 0;
                this.IsVoid = "N";
                this.Location = "ROOT";
                this.RecordType = Utils.RecordType.FOLDER;
                this.SequenceNumber = 0;
                this.SimpleFileName = "ROOT";
                this.UID = 0;
                this.SourceCode = Utils.SourceCode.FCM;
                this.FileExtension = "ROOT";

                this.Save(headerInfo);
            }
        }

        // -----------------------------------------------------
        //    Update Document
        // -----------------------------------------------------
        private int Update(HeaderInfo headerInfo)
        {

            string ret = "Item updated successfully";

            this.UserIdUpdatedBy = Utils.UserID;
            this.UpdateDateTime = System.DateTime.Now;

          using (var connection = new SqlConnection(ConnString.ConnectionString))
          {

              var commandString =
              (
                 "UPDATE [Document] "+
                 " SET " +
                 " [CUID] =  @CUID " +
                 ",[Name] =  @Name " +
                 ",[DisplayName] =  @DisplayName " +
                 ",[SequenceNumber] = @SequenceNumber  " +
                 ",[IssueNumber] = @IssueNumber" +
                 ",[Location] = @Location" +
                 ",[Comments] = @Comments " +
                 ",[FileName] = @FileName" +
                 ",[SimpleFileName] = @SimpleFileName" +
                 ",[SourceCode] = @SourceCode " +
                 ",[IsProjectPlan] = @IsProjectPlan " +
                 ",[FKClientUID] = @FKClientUID " +
                 ",[ParentUID] = @ParentUID " +
                 ",[DocumentType] = @DocumentType " +
                 ",[UserIdUpdatedBy] = @UserIdUpdatedBy " +
                 ",[UpdateDateTime] = @UpdateDateTime  " +
                 " WHERE [UID] = @UID "
              );

              using (var command = new SqlCommand(
                                          commandString, connection))
              {
                  command.Parameters.Add( "@UID", SqlDbType.VarChar ).Value = UID;
                  command.Parameters.Add( "@CUID", SqlDbType.VarChar ).Value = CUID;
                  command.Parameters.Add("@Name", SqlDbType.VarChar).Value = Name;
                  command.Parameters.Add("@DisplayName", SqlDbType.VarChar).Value = DisplayName;
                  command.Parameters.Add("@SequenceNumber", SqlDbType.VarChar).Value = SequenceNumber;
                  command.Parameters.Add("@IssueNumber", SqlDbType.Decimal).Value = IssueNumber;
                  command.Parameters.Add("@Location", SqlDbType.VarChar).Value = Location;
                  command.Parameters.Add("@IsProjectPlan", SqlDbType.VarChar).Value = IsProjectPlan;
                  command.Parameters.Add("@Comments", SqlDbType.VarChar).Value = Comments;
                  command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = FileName;
                  command.Parameters.Add("@SimpleFileName", SqlDbType.VarChar).Value = SimpleFileName;
                  command.Parameters.Add("@SourceCode", SqlDbType.VarChar).Value = SourceCode;
                  command.Parameters.Add("@FKClientUID", SqlDbType.Int).Value = FKClientUID;
                  command.Parameters.Add("@ParentUID", SqlDbType.Int).Value = ParentUID;
                  command.Parameters.Add("@DocumentType", SqlDbType.VarChar).Value = DocumentType;
                  command.Parameters.Add("@UserIdUpdatedBy", SqlDbType.VarChar).Value = UserIdUpdatedBy;
                  command.Parameters.Add("@UpdateDateTime", SqlDbType.DateTime).Value = UpdateDateTime;

                  connection.Open();

                  try
                  {
                      command.ExecuteNonQuery();
                  }
                  catch (Exception ex)
                  {
                      LogFile.WriteToTodaysLogFile(
                          "Error updating document." + ex.ToString(),
                          headerInfo.UserID,
                          "Document.cs"
                          );
                  }
              }
          }
          return this.UID;
        }

        // -----------------------------------------------------
        //   Retrieve last Document Set id
        // -----------------------------------------------------
        private int GetLastUID()
        {
          int LastUID = 0;

          // 
          // EA SQL database
          // 

          using (var connection = new SqlConnection(ConnString.ConnectionString))
          {
              var commandString =
                  "SELECT MAX([UID]) LASTUID FROM [Document]";

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
        /// Add or Update a record
        /// </summary>
        /// <param name="type">In case of an update, checks the issue number
        /// </param>
        /// <returns></returns>
        public int Save(HeaderInfo headerInfo, string type = null)
        {
          // Check if code value exists.
          // If it exists, update
          // Else Add a new one
          int uidReturn = 0;

          Document find = new Document();
          // find.CUID = this.CUID; // CUID can be modified!
          find.UID = this.UID;

          if (find.Read())
          {
              // If it is a new issue, save the old issue in the issue table
              //
              if (find.IssueNumber == this.IssueNumber)
              {
                  if (type == Utils.SaveType.UPDATE)
                  {
                      uidReturn = this.Update(headerInfo);
                  }
              }
              else
              {
                  LogFile.WriteToTodaysLogFile(
                        "Document Save - issue number is different. CUID: " + this.CUID ,
                        headerInfo.UserID,
                        "Document.cs"
                        );
              }
          }
          else
          {
              uidReturn = this.Add(headerInfo);

          }
          return uidReturn;
        }

        // -----------------------------------------------------
        //    Create new document version
        // -----------------------------------------------------
        public ResponseStatus NewVersion(HeaderInfo headerInfo)
        {
            ResponseStatus response = new ResponseStatus();
            response.Message = "New version created successfully.";
            
            // Copy existing version to old folder version
            // Old folder comes from %VERSIONFOLDER%
            // 
            var versionFolder  =
                CodeValue.GetCodeValueExtended(FCMConstant.CodeTypeString.SYSTSET, FCMConstant.SYSFOLDER.VERSIONFOLDER);

            // Create a record to point to the old version
            var documentVersion = new DocumentVersion();
            documentVersion.FKDocumentCUID = this.CUID;
            documentVersion.FKDocumentUID = this.UID;

            // Generate the new version id
            documentVersion.IssueNumber = this.IssueNumber;
            documentVersion.Location = Utils.GetVersionPath(this.Location);
            documentVersion.FileName = this.FileName;
            documentVersion.Add();

            // Increments issue number
            this.IssueNumber++;

            // Create a new file name with version on it
            // POL-05-01 FILE NAME.doc
            //
            int version = Convert.ToInt32(this.IssueNumber);
            string tversion = version.ToString().PadLeft(2, '0');
            // string simpleFileName = this.Name.Substring(10).Trim();
            string simpleFileName = this.Name.Substring(07).Trim();

            string newFileName = this.CUID + '-' +
                                 tversion + '-' +
                                 simpleFileName;

            // string newFileNameWithExtension = newFileName + ".doc";

            // Well, the simple file name has extension already... so I have commented out the line above
            // 30/04/2011 - Testing to see if it works.
            //
            string newFileNameWithExtension = newFileName;
            
            // Copy the file with new name
            // Let's copy the document
            string realLocation = Utils.getFilePathName(this.Location, this.FileName);
            string realDestination = Utils.getFilePathName(documentVersion.Location, documentVersion.FileName);
            string realPathDestination = Utils.GetPathName(documentVersion.Location);

            if (!System.IO.Directory.Exists(realPathDestination))
                System.IO.Directory.CreateDirectory(realPathDestination);

            if (!System.IO.File.Exists(realLocation))
            {
                response.Message = "File to be copied was not found. " + realLocation;
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00000006;
                return response;
            }
            File.Copy(realLocation, realDestination, true);

            // Copy file to new name
            //
            string newFilePathName = Utils.getFilePathName(
                     this.Location,
                     newFileNameWithExtension);

            File.Copy(realLocation, newFilePathName, true);
            
            // Delete old version from main folder
            //
            File.Delete(realLocation);

            // Update document details - version, name, etc
            this.IssueNumber = version;
            // this.ComboIssueNumber = "C" + version.ToString("000");
            this.FileName = newFileNameWithExtension;
            this.Name = newFileName;
            this.UpdateVersion(headerInfo);

            // Build a screen to browse all versions of a file
            // Allow compare/ View
            // Check how to open a document read-only for users future

            response.Contents = version;
            return response;
        }

        /// <summary>
        /// Load a document from a given reader
        /// </summary>
        /// <param name="retDocument"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="reader"></param>
        public static void LoadDocumentFromReader(
                               Document retDocument,
                               string tablePrefix, 
                               SqlDataReader reader  )
        {

            retDocument.UID = Convert.ToInt32(reader[tablePrefix + FieldName.UID].ToString());
            retDocument.CUID = reader[tablePrefix + FieldName.CUID].ToString();
            retDocument.Name = reader[tablePrefix + FieldName.Name].ToString();
            retDocument.DisplayName = reader[tablePrefix + FieldName.DisplayName].ToString();
            retDocument.SequenceNumber = Convert.ToInt32(reader[tablePrefix + FieldName.SequenceNumber].ToString());
            retDocument.IssueNumber = Convert.ToInt32(reader[tablePrefix + FieldName.IssueNumber].ToString());
            retDocument.Location = reader[tablePrefix + FieldName.Location].ToString();
            retDocument.Comments = reader[tablePrefix + FieldName.Comments].ToString();
            retDocument.SourceCode = reader[tablePrefix + FieldName.SourceCode].ToString();
            retDocument.FileName = reader[tablePrefix + FieldName.FileName].ToString();
            retDocument.SimpleFileName = reader[tablePrefix + FieldName.SimpleFileName].ToString();
            retDocument.FKClientUID = Convert.ToInt32(reader[tablePrefix + FieldName.FKClientUID].ToString());
            retDocument.ParentUID = Convert.ToInt32(reader[tablePrefix + FieldName.ParentUID].ToString());
            retDocument.RecordType = reader[tablePrefix + FieldName.RecordType].ToString();
            retDocument.IsProjectPlan = reader[tablePrefix + FieldName.IsProjectPlan].ToString();
            retDocument.IsVoid = reader[tablePrefix + FieldName.IsVoid].ToString();
            retDocument.DocumentType = reader[tablePrefix + FieldName.DocumentType].ToString();
            retDocument.RecordVersion = Convert.ToInt32(reader[tablePrefix + FieldName.RecordVersion].ToString());


            //try { retDocument.UpdateDateTime = Convert.ToDateTime(reader[FieldName.UpdateDateTime].ToString()); }
            //catch { retDocument.UpdateDateTime = DateTime.Now; }
            //try { retDocument.CreationDateTime = Convert.ToDateTime(reader[FieldName.CreationDateTime].ToString()); }
            //catch { retDocument.CreationDateTime = DateTime.Now; }
            //try { retDocument.IsVoid = reader[FieldName.IsVoid].ToString(); }
            //catch { retDocument.IsVoid = "N"; }
            //try { retDocument.UserIdCreatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            //catch { retDocument.UserIdCreatedBy = "N"; }
            //try { retDocument.UserIdUpdatedBy = reader[FieldName.UserIdCreatedBy].ToString(); }
            //catch { retDocument.UserIdCreatedBy = "N"; }

            return;
        }
        /// <summary>
        /// Returns a string to be concatenated with a SQL statement
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <returns></returns>
        public static string SQLDocumentConcat(string tablePrefix)
        {
            string ret = " " +
            tablePrefix + ".[UID]            " + tablePrefix + "UID,            " +
            tablePrefix + ".[CUID]           " + tablePrefix + "CUID,           " +
            tablePrefix + ".[Name]           " + tablePrefix + "Name,           " +
            tablePrefix + ".[DisplayName]    " + tablePrefix + "DisplayName,    " +
            tablePrefix + ".[SequenceNumber] " + tablePrefix + "SequenceNumber, " +
            tablePrefix + ".[IssueNumber]    " + tablePrefix + "IssueNumber,    " +
            tablePrefix + ".[Location]       " + tablePrefix + "Location,       " +
            tablePrefix + ".[Comments]       " + tablePrefix + "Comments,       " +
            tablePrefix + ".[FileName]       " + tablePrefix + "FileName,       " +
            tablePrefix + ".[Status]         " + tablePrefix + "Status,         " +
            tablePrefix + ".[SimpleFileName] " + tablePrefix + "SimpleFileName, " +
            tablePrefix + ".[SourceCode]     " + tablePrefix + "SourceCode,     " +
            tablePrefix + ".[FKClientUID]    " + tablePrefix + "FKClientUID,    " +
            tablePrefix + ".[IsVoid]         " + tablePrefix + "IsVoid,         " +
            tablePrefix + ".[ParentUID]      " + tablePrefix + "ParentUID,      " +
            tablePrefix + ".[RecordType]     " + tablePrefix + "RecordType,     " +
            tablePrefix + ".[FileExtension]  " + tablePrefix + "FileExtension,  " +
            tablePrefix + ".[IsProjectPlan]  " + tablePrefix + "IsProjectPlan,  " +
            tablePrefix + ".[DocumentType]   " + tablePrefix + "DocumentType,    " +
            tablePrefix + ".[RecordVersion]   " + tablePrefix + "RecordVersion,    " +
            tablePrefix + ".[UpdateDateTime] " + tablePrefix + "UpdateDateTime,  " +
            tablePrefix + ".[CreationDateTime] " + tablePrefix + "CreationDateTime, " +
            tablePrefix + ".[UserIdCreatedBy]  " + tablePrefix + "UserIdCreatedBy, " +
            tablePrefix + ".[UserIdUpdatedBy]  " + tablePrefix + "UserIdUpdatedBy ";
           
            return ret;
        }

        /// <summary>
        /// Document string of fields.
        /// </summary>
        /// <returns></returns>
        private static string DocumentFieldString()
        {
            return (
                        FieldName.UID
                + "," + FieldName.CUID
                + "," + FieldName.Name
                + "," + FieldName.DisplayName
                + "," + FieldName.SequenceNumber
                + "," + FieldName.IssueNumber
                + "," + FieldName.Location
                + "," + FieldName.Comments
                + "," + FieldName.FileName
                + "," + FieldName.Status
                + "," + FieldName.SimpleFileName
                + "," + FieldName.SourceCode
                + "," + FieldName.FKClientUID
                + "," + FieldName.IsVoid
                + "," + FieldName.ParentUID
                + "," + FieldName.RecordType
                + "," + FieldName.FileExtension
                + "," + FieldName.IsProjectPlan
                + "," + FieldName.DocumentType
                + "," + FieldName.RecordVersion
                + "," + FieldName.UpdateDateTime
                + "," + FieldName.CreationDateTime
                + "," + FieldName.UserIdCreatedBy
                + "," + FieldName.UserIdUpdatedBy
            );
        }

    }
}
