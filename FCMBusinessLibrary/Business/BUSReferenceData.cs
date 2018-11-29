using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FCMBusinessLibrary.ReferenceData;

namespace FCMBusinessLibrary.Business
{
    public static class BUSReferenceData
    {
        /// <summary>
        /// List code value 
        /// </summary>
        /// <param name="eventClient"></param>
        /// <returns></returns>
        public static List<CodeValue> ListCodeValue()
        {
            var listOfCodeValues = CodeValueList.ListS();

            return listOfCodeValues;
        }

        /// <summary>
        /// List code value 
        /// </summary>
        /// <param name="eventClient"></param>
        /// <returns></returns>
        public static List<CodeValue> ListCodeValue(string codeType)
        {
            var listOfCodeValues = CodeValueList.ListS(codeType);

            return listOfCodeValues;
        }

        /// <summary>
        /// Get List of screens of user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<CodeValue> GetListScreensForUser(string userID)
        {

            List<CodeValue> list = new List<CodeValue>();

            // Retrieve list of roles for user
            var listOfRoles = FCMUserRole.ListRoleForUser(userID);

            foreach (var role in listOfRoles)
            { 
                // get list of screen for role
                //
                var listOfScreen = FCMRoleScreen.List(role.FK_Role);

                foreach (var cvScreen in listOfScreen)
                {
                    var screenAsCodeValue = new CodeValue();
                    screenAsCodeValue.ID = cvScreen.FKScreenCode;
                    screenAsCodeValue.Description = CodeValue.GetCodeValueDescription(
                        FCMConstant.CodeTypeString.SCREENCODE, cvScreen.FKScreenCode);

                    bool found = false;
                    foreach (var alreadyInListScreen in list)
                    {
                        if (cvScreen.FKScreenCode == alreadyInListScreen.ID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found) continue;

                    list.Add(screenAsCodeValue);

                }
            }
            
            return list;
        }

        public static ResponseStatus DeleteRelatedCodeValue(RelatedCodeValue relatedCodeValue)
        {
            ResponseStatus response = new ResponseStatus();

            relatedCodeValue.Delete();

            return response;
        }

        public static ResponseStatus AddRelatedCodeValue(RelatedCodeValue relatedCodeValue)
        {
            ResponseStatus response = new ResponseStatus();

            relatedCodeValue.Add();

            return response;
        }

        public static ResponseStatus AddRelatedCodeType(RelatedCode relatedCode)
        {
            ResponseStatus response = new ResponseStatus();

            relatedCode.Add();

            return response;
        }

        /// <summary>
        /// List code value 
        /// </summary>
        /// <param name="eventClient"></param>
        /// <returns></returns>
        public static List<RelatedCode> ListRelatedCode()
        {
            var listOfCodeValues = RelatedCode.List();

            return listOfCodeValues;
        }

        /// <summary>
        /// List complete set of related code value.
        /// </summary>
        /// <param name="eventClient"></param>
        /// <returns></returns>
        public static List<RelatedCodeValue> ListRelatedCodeValue()
        {
            var listOfRelatedValues = RelatedCodeValue.ListAllS();

            return listOfRelatedValues;
        }

    }
}
