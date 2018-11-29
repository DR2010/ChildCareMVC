using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FCMMySQLBusinessLibrary.Client;
using MackkadoITFramework.Security;
using MackkadoITFramework.Utils;
using ChildCareK;
using fcmMVCfirst.Models;

namespace ChildCareK.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(UserAccess model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //if (Membership.ValidateUser(model.UserName, model.Password))
                //{
                //    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                //    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                //        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                //    {
                //        return Redirect(returnUrl);
                //    }
                //    else
                //    {
                //        return RedirectToAction("Index", "Home");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                //}



                //  Daniel - My way of authentication
                //


                // Set the connection string
                ConnString.ConnectionString = ConnectionString.GetConnectionString();
                ConnString.ConnectionStringFramework = ConnectionString.GetConnectionString("makkframework");
              
                SessionInfo.StoreConnectionString(this, 
                                                     ConnString.ConnectionString,
                                                     ConnString.ConnectionStringFramework);

                SecurityUserRole fcmUserRole = new SecurityUserRole(HeaderInfo.Instance);
                var userRoleList = fcmUserRole.UserRoleList(model.UserID);

                string listOfRole = "";
                foreach (var ur in userRoleList)
                {
                    listOfRole += ur.FK_Role + ",";
                }

                UserAccess ua = new UserAccess();
                ua.UserID = model.UserID;
                ua.Read(model.UserID);

                if (ua.LogonAttempts > 4)
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                else
                {
                    var userAuthenticate = ua.AuthenticateUser(model.UserID, model.Password);

                    if ( userAuthenticate.ReturnCode == 0001 && userAuthenticate.ReasonCode == 0001 )
                    {
                        // Create a new ticket used for authentication
                        //
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                              version: 1, // Ticket version
                              name: model.UserID, // Username associated with ticket
                              issueDate: DateTime.Now, // Date/time issued
                              expiration: DateTime.Now.AddMinutes(30), // Date/time to expire
                              isPersistent: true, // "true" for a persistent user cookie
                              userData: listOfRole, // User-data, in this case the roles
                              cookiePath: FormsAuthentication.FormsCookiePath);// Path cookie valid for

                        // Encrypt the cookie using the machine key for secure transport
                        //
                        string hash = FormsAuthentication.Encrypt(ticket);
                        HttpCookie cookie = new HttpCookie(name: FormsAuthentication.FormsCookieName, // Name of auth cookie
                                                           value: hash); // Hashed ticket

                        // Set the cookie's expiration time to the tickets expiration time
                        if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

                        // Add the cookie to the list for outgoing response
                        Response.Cookies.Add(cookie);

                        // FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                        // Redirect to requested URL, or homepage if no previous page
                        // requested

                        // Commented out on 08 Feb 2012
                        //
                        //string returnUrl = Request.QueryString["ReturnUrl"];
                        //if (returnUrl == null) returnUrl = "/";
                        if (returnUrl == null) 
                            return RedirectToAction("Index", "Home");

                        // Don't call FormsAuthentication.RedirectFromLoginPage since it
                        // could
                        // replace the authentication ticket (cookie) we just added


                        // Store client in session
                        //if (ua.ClientUID > 0)
                        //{

                        //    var client = new Client(HeaderInfo.Instance);
                        //    client.UID = ua.ClientUID;
                        //    var response = client.Read();

                        //    if (client.UID > 0)
                        //    {
                        //        // 08 Feb 2012 - Daniel, come back here, session info may be different
                        //        //
                        //        SessionInfo.StoreClientInSession(client, this);

                        //    }
                        //}

                        // FormsAuthentication.RedirectFromLoginPage(UserEmail.Text, Persist.Checked);

                        //if (returnUrl == "/")
                        //    returnUrl = @"~/default.aspx";

                        //if (returnUrl == "/")
                        //    returnUrl = @"Index";
                        
                        
                        //returnUrl = @"/FCMWebSite/default.aspx";
                        // returnUrl = @"/default.aspx";

                        // Response.Redirect(returnUrl);
                        
                        // Enable/ Disable Menu options according to security
                        //



                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }

                // 
                //
                //
                //



            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(UserAccess model)
        {
            //if (ModelState.IsValid)
            //{
            //    // Attempt to register the user
            //    MembershipCreateStatus createStatus;
            //    Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

            //    if (createStatus == MembershipCreateStatus.Success)
            //    {
            //        FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
            //        return RedirectToAction("Index", "Home");
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", ErrorCodeToString(createStatus));
            //    }
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);

            UserAccess user = new UserAccess();
            user.UserID = model.UserID;
            user.UserName = model.UserName;
            user.Salt = DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture);
            user.Password = model.Password;
            user.PasswordRetype = model.PasswordRetype;

            if (! string.Equals(user.Password, user.PasswordRetype))
            {
                ModelState.AddModelError("Error", "Passwords mismatch.");
                return View();
            }

            // We need to create a second occurrence.
            UserAccess userRead = new UserAccess();

            var userReadResponse = userRead.Read(model.UserID);

            if (userReadResponse.ReturnCode == 0001 && userReadResponse.ReasonCode == 0001 )
            {
                userReadResponse = user.UpdateUser();
            }

            if (userReadResponse.ReturnCode == 0001 && userReadResponse.ReasonCode == 0002)
            {
                userReadResponse = user.AddUser();
            }

            if (userReadResponse.ReturnCode < 0001)
            {
                ModelState.AddModelError("Error", userReadResponse.Message);
                return View();
            }

            // all good
            return View("CreateUserSuccess");
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(UserAccess model)
        {
            if (ModelState.IsValid)
            {

                var uacnew = new UserAccess();
                var readuser = uacnew.Read(model.UserID);

                uacnew.UserID = model.UserID;
                uacnew.Salt = System.DateTime.Now.Hour.ToString();
                uacnew.Password = model.PasswordRetype;

                // Check if current password match
                //
                UserAccess ua = new UserAccess();
                ua.UserID = model.UserID;
                ua.Read(model.UserID);
                var userAuthenticate = ua.AuthenticateUser(model.UserID, model.Password);

                if (userAuthenticate.ReturnCode == 0001 && userAuthenticate.ReasonCode == 0001)
                {

                    if (model.PasswordRetype != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        return View(model);
                    }

                    var response = BUSUserAccess.SavePassword(uacnew);

                    return RedirectToAction("ChangePasswordSuccess");
                }

                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
