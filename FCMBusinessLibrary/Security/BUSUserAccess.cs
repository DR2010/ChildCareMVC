using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FCMBusinessLibrary.DataAccess;

namespace FCMBusinessLibrary.Business
{
    public class BUSUserAccess
    {

        /// <summary>
        /// Add role 
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus Save(UserAccess inUser)
        {
            ResponseStatus response = new ResponseStatus();

            UserAccess user = new UserAccess();
            user.UserID = inUser.UserID;
            user.UserName = inUser.UserName;
            user.Salt = inUser.Salt;
            user.Password = inUser.Password;

            // We need to create a second occurrence.
            UserAccess userRead = new UserAccess();
            if (userRead.Read(inUser.UserID))
            {
                response = user.UpdateUser();
            }
            else
            {
                response = user.AddUser();
            }
            return response;
        }


        /// <summary>
        /// Save password
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus SavePassword(UserAccess inUser)
        {
            ResponseStatus response = new ResponseStatus();

            UserAccess user = new UserAccess();
            user.UserID = inUser.UserID;
            user.Salt = inUser.Salt;
            user.Password = inUser.Password;

            // Check if user exists
            // We need to create a second occurrence.

            UserAccess userRead = new UserAccess();
            if (userRead.Read(inUser.UserID))
            {
                response = user.UpdatePassword();
            }
            else
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "User not found.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00009999;
            }

            return response;
        }


        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus Add(FCMUserRole inUserRole)
        {
            ResponseStatus response = new ResponseStatus();
            FCMUserRole userRole = new FCMUserRole();
            userRole.FK_UserID = inUserRole.FK_UserID;
            userRole.FK_Role = inUserRole.FK_Role;
            userRole.IsActive = inUserRole.IsActive;

            response = userRole.Add();

            Utils.RefreshCache();

            response.Contents = userRole.UniqueID;
            return response;
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus Delete(FCMUserRole inUserRole)
        {
            ResponseStatus response = new ResponseStatus();

            FCMUserRole userRole = new FCMUserRole();
            userRole.FK_UserID = inUserRole.FK_UserID;
            userRole.FK_Role = inUserRole.FK_Role;
            userRole.UniqueID = inUserRole.UniqueID;

            response = userRole.Delete();

            Utils.RefreshCache();

            response.Contents = userRole.UniqueID;
            return response;
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus AddRole(FCMRole inRole)
        {
            ResponseStatus response = new ResponseStatus();
            var role = new FCMRole();
            role.Role = inRole.Role;
            role.Description = inRole.Description;

            response = role.Add();

            response.Contents = role;
            return response;
        }


        /// <summary>
        /// Add new screen to role
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus AddScreenToRole(FCMRoleScreen inRole)
        {
            ResponseStatus response = new ResponseStatus();
            var role = new FCMRoleScreen();
            role.FKRoleCode = inRole.FKRoleCode;
            role.FKScreenCode = inRole.FKScreenCode;

            response = role.Add();

            response.Contents = role;
            return response;
        }

        public static ResponseStatus ListByRole(string inRole)
        {
            ResponseStatus response = new ResponseStatus();
            var list = FCMRoleScreen.List(inRole);
            response.Contents = list;

            return response;
        }

    }
}
