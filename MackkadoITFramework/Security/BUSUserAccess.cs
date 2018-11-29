using FCMMySQLBusinessLibrary;
using MackkadoITFramework.ErrorHandling;

namespace MackkadoITFramework.Security
{
    public class BUSUserAccess
    {
        /// <summary>
        /// Add role 
        /// </summary>
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
            var userRead = new UserAccess();

            var readuser = userRead.Read(inUser.UserID);

            // record found
            if (readuser.ReturnCode == 0001 && readuser.ReasonCode == 0001)
            {
                response = user.UpdateUser();
            }

            // Not found
            if (readuser.ReturnCode == 0001 && readuser.ReasonCode == 0002)
            {
                response = user.AddUser();
            }

            if (readuser.ReturnCode < 0000)
            {
                response = readuser;
            }

            return response;
        }


        /// <summary>
        /// Save password
        /// </summary>
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

            var readuser = userRead.Read(inUser.UserID);
            if (readuser.ReturnCode == 0001 && readuser.ReasonCode == 0001 )
            {
                response = user.UpdatePassword();
                return response;
            }
            
            if (readuser.ReturnCode == 0001 && readuser.ReasonCode == 0002)
            {
                response.ReturnCode = -0010;
                response.ReasonCode = 0001;
                response.Message = "User not found.";
                response.UniqueCode = ResponseStatus.MessageCode.Error.FCMERR00009999;
                return response;
            }

            if (readuser.ReturnCode < 0000 )
            {
                response = readuser;
            }            

            return response;
        }


        /// <summary>
        /// Add new role
        /// </summary>
        /// <param name="clientContract"></param>
        /// <returns></returns>
        public static ResponseStatus AddRole(SecurityRole inRole)
        {
            ResponseStatus response = new ResponseStatus();
            var role = new SecurityRole();
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
        public static ResponseStatus AddScreenToRole(SecurityRoleScreen inRole)
        {
            ResponseStatus response = new ResponseStatus();
            var role = new SecurityRoleScreen();
            role.FKRoleCode = inRole.FKRoleCode;
            role.FKScreenCode = inRole.FKScreenCode;

            response = role.Add();

            response.Contents = role;
            return response;
        }

        public static ResponseStatus ListByRole(string inRole)
        {
            ResponseStatus response = new ResponseStatus();
            var list = SecurityRoleScreen.List(inRole);
            response.Contents = list;

            return response;
        }

    }
}
