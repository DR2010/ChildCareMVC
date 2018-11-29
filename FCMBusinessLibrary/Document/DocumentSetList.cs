using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FCMBusinessLibrary.Document
{
    public class DocumentSetList
    {
        public List<DocumentSet> documentSetList;

        // -----------------------------------------------------
        //    List Documents
        // -----------------------------------------------------
        public void List()
        {
            this.documentSetList = new List<DocumentSet>();

            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString = string.Format(
                " SELECT [UID] " +
                " ,[TemplateType] " +
                " ,[TemplateFolder] " +
                " ,[IsVoid] " +

                "   FROM [DocumentSet] " +
                "  WHERE [IsVoid] = 'N' " 
                );

                using (var command = new SqlCommand(
                                      commandString, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DocumentSet documentSet = new DocumentSet();
                            documentSet.UID = Convert.ToInt32(reader["UID"].ToString());
                            documentSet.TemplateType = reader["TemplateType"].ToString();
                            documentSet.TemplateFolder = reader["TemplateFolder"].ToString();
                            documentSet.IsVoid = Convert.ToChar(reader["IsVoid"].ToString());
                            documentSet.UIDNameDisplay = documentSet.UID.ToString() + "; " + documentSet.TemplateType; 

                            this.documentSetList.Add(documentSet);
                        }
                    }
                }
            }
        }

        public void ListInComboBox(ComboBox cbxList)
        {
            this.List();

            foreach (DocumentSet docSet in documentSetList)
            {
                cbxList.Items.Add(docSet.UID + "; " + docSet.TemplateType);
            }
            
        }
    }
}
