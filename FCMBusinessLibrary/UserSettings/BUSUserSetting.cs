using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCMBusinessLibrary
{
    public class BUSUserSetting
    {

        /// <summary>
        /// Client document read
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus Save(UserSettings inUserSetting)
        {
            ResponseStatus response = new ResponseStatus();
            UserSettings userSetting = new UserSettings();
            userSetting.FKUserID = inUserSetting.FKUserID;
            userSetting.FKScreenCode = inUserSetting.FKScreenCode;
            userSetting.FKControlCode = inUserSetting.FKControlCode;
            userSetting.FKPropertyCode = inUserSetting.FKPropertyCode;
            userSetting.Value = inUserSetting.Value;

            userSetting.Save();

            Utils.RefreshCache();

            response.Contents = userSetting.ListOfUserSettings;
            return response;
        }
    }
}
