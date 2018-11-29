using System;
using System.Web;
using System.Web.Security;
using FCMMySQLBusinessLibrary.Client;

namespace fcmMVCfirst.Models
{
    /// <summary>
    /// Summary description for SessionInfo
    /// </summary>
    public class SessionInfo
    {
        public static void StoreClientInSession(Client client, System.Web.Mvc.Controller controller)
        {
            controller.Session[Client.FieldName.UID] = client.UID;
            controller.Session[Client.FieldName.ABN] = client.ABN;
            controller.Session[Client.FieldName.Name] = client.Name;
            controller.Session[Client.FieldName.Address] = client.Address;
            controller.Session[Client.FieldName.Phone] = client.Phone;
            controller.Session[Client.FieldName.Fax] = client.Fax;
            controller.Session[Client.FieldName.EmailAddress] = client.EmailAddress;
            controller.Session[Client.FieldName.MainContactPersonName] = client.MainContactPersonName;
            controller.Session[Client.FieldName.IsVoid] = client.IsVoid;
            controller.Session[Client.FieldName.DisplayLogo] = client.DisplayLogo;
        }

        public static bool CheckIfUserHasRole(string roleToCheck)
        {

            if (!(HttpContext.Current.User.Identity is FormsIdentity))
            {
                return false;
            }

            FormsIdentity id =
                (FormsIdentity) HttpContext.Current.User.Identity;

            FormsAuthenticationTicket ticket = id.Ticket;

            // Get the stored user-data, in this case, our roles
            //
            string userData = ticket.UserData;
            string[] roles = userData.Split(',');

            foreach (var ur in roles)
            {
                if (String.Equals(roleToCheck, ur))
                {
                    return true;
                }
            }

            return false;
        }

        public static string UserIDLogged
        {
            get
            {
                if (!(HttpContext.Current.User.Identity is FormsIdentity))
                {
                    return "";
                }

                FormsIdentity id =
                    (FormsIdentity) HttpContext.Current.User.Identity;

                FormsAuthenticationTicket ticket = id.Ticket;

                string userID = ticket.Name;

                return userID;
            }
        }

        public static void StoreConnectionString(System.Web.Mvc.Controller controller, string connectionString, string connectionStringFramework)
        {
            controller.Session["ConnectionString"] = connectionString;
            controller.Session["ConnectionStringFramework"] = connectionStringFramework;
        }

        public static bool CheckIfUserHasAnyRole()
        {

            if (!(HttpContext.Current.User.Identity is FormsIdentity))
            {
                return false;
            }

            FormsIdentity id =
                (FormsIdentity)HttpContext.Current.User.Identity;

            FormsAuthenticationTicket ticket = id.Ticket;

            // Get the stored user-data, in this case, our roles
            //
            string userData = ticket.UserData;
            string[] roles = userData.Split(',');

            foreach (var ur in roles)
            {
                return true;
            }

            return false;
        }


    }
}