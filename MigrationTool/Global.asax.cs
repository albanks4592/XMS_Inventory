using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Threading;
using System.Web.Hosting;
using System.Security.Permissions;

namespace MigrationTool
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static Dictionary<string, DateTime> _roleCacheAges = new Dictionary<string, DateTime>();
        private static Dictionary<string, List<string>> _roleCache = new Dictionary<string, List<string>>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand)]
        protected void Application_AuthorizeRequest(object sender, EventArgs args)
        {
            // Even though we are using Windows Authentication, it looks like the authorization checks only recognize groups that you belong to which are in the same domain as your user.
            // Because we are working across domains, we need to get around this by manually looking up the full list of group memberships.
            // We also cache this data for five minutes at a time so that we don't bombard AD with requests.
            string identity = HttpContext.Current.User.Identity.Name;
            if (HttpContext.Current.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(identity) && identity.Contains('\\'))
            {
                string[] split = identity.Split('\\');
                string domain = split[0];
                string user = split[1];

                List<string> roles;
                if (_roleCacheAges.ContainsKey(user) && _roleCacheAges[user] > DateTime.Now.AddMinutes(-5))
                {
                    roles = _roleCache[user];
                }
                else
                {
                    List<string> newRoles = null;
                    try
                    {
                        // Active directory gives us timeout issues sometimes.
                        // If we can't update the user data, just keep using
                        // what we had before. Not the best solution, but the
                        // group memberships don't change often.
                        newRoles = this.GetGroups(domain, user);
                        roles = newRoles;
                        _roleCache[user] = newRoles;
                        _roleCacheAges[user] = DateTime.Now;
                    }
                    catch
                    {
                        if (_roleCacheAges.ContainsKey(user))
                        {
                            // User is in cache, keep using cached data.
                            roles = _roleCache[user];
                            _roleCacheAges[user] = DateTime.Now;
                        }
                        else
                        {
                            // User not in cache, we can't continue
                            throw;
                        }
                    }
                }

                GenericPrincipal principal = new GenericPrincipal(HttpContext.Current.User.Identity, roles.ToArray());

                Thread.CurrentPrincipal = HttpContext.Current.User = principal;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand)]
        protected List<string> GetGroups(string domain, string userName)
        {
            List<string> result = new List<string>();

            using (HostingEnvironment.Impersonate())
            {
                // establish domain context
                using (PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain, domain))
                {
                    //PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
                    //PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain, "novartis.net:3268", "DC=novartis,DC=net");

                    // find your user
                    using (PrincipalContext novartisnet = new PrincipalContext(ContextType.Domain, "gluseh-sp400004"))
                    {
                        var x = novartisnet.Options;
                        UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);

                        // if found - grab its groups
                        if (user != null)
                        {
                            //PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
                            var groups = user.GetGroups(novartisnet).ToList();

                            // iterate over all groups
                            foreach (Principal p in groups)
                            {
                                // make sure to add only group principals
                                if (p is GroupPrincipal)
                                {
                                    result.Add(p.Name);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}