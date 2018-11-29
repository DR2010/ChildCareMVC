using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace FCMBusinessLibrary.ReferenceData
{
    public class CodeType
    {
        [Display(Name = "Code Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = @"Code Type must be supplied.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = @"Code Type must be between 4 and 20 characters")]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = @"Description must be supplied.")]
        [Display(Name = "Description")]
        [StringLength(50, MinimumLength = 4)]
        public string Description { get; set; }

        [Display(Name = "ShortCodeType")]
        [StringLength(3, MinimumLength = 3)]
        public string ShortCodeType { get; set; }
        
        /// <summary>
        /// Add code type
        /// </summary>
        public void Create()
        {
            string errorMessage;

            ValidationContext context = new ValidationContext(this, null, null);
            IList<ValidationResult> errors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, context, errors, true))
            {
                foreach (ValidationResult result in errors)
                    errorMessage = result.ErrorMessage;
                return;
            }
           
            using (var connection = new SqlConnection(ConnString.ConnectionString))
            {

                var commandString =
                (
                   "INSERT INTO [CodeType] " +
                   "([CodeType], [Description], [ShortCodeType]" +
                   ")" +
                        " VALUES " +
                   "( " +
                   "  @CodeType      " +
                   ", @Description   " +
                   ", @ShortCodeType " +
                   " )"

                   );

                using (var command = new SqlCommand(
                                            commandString, connection))
                {
                    command.Parameters.Add("@CodeType", SqlDbType.VarChar).Value = Code;
                    command.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                    command.Parameters.Add("@ShortCodeType", SqlDbType.VarChar).Value = ShortCodeType;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public struct CodeTypeValue
        {
            public const string ContractStatus = "CONTRACTSTATUS";
            public const string ContractType = "CONTRACTTYPE";
            public const string ProposalType = "PROPTYPE";
            public const string ProposalStatus = "PROPSTATUS";
            public const string ClientOtherField = "CLIENTOTHERFIELD";

        }
    }
}
