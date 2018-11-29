using System;
using System.Web.Mvc;
using fcmMVCfirst.Models;

namespace fcmMVCfirst.Common
{
    public class AllowAdminAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionInfo.CheckIfUserHasRole("ADMIN"))
                throw new UnauthorizedAccessException();
        }
    }

    public class AllowClientAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionInfo.CheckIfUserHasRole("CLIENT"))
                throw new UnauthorizedAccessException();
        }
    }


    public class AllowAdminClientAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionInfo.CheckIfUserHasRole("ADMIN"))
                if (!SessionInfo.CheckIfUserHasRole("CLIENT"))
                    throw new UnauthorizedAccessException();
        }
    }


    public class AllowAllAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionInfo.CheckIfUserHasAnyRole())
                throw new UnauthorizedAccessException();
        }
    }

    public class AllowAdminWorkerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!SessionInfo.CheckIfUserHasRole("ADMIN"))
                if (!SessionInfo.CheckIfUserHasRole("WORKER"))
                    throw new UnauthorizedAccessException();
        }
    }

}