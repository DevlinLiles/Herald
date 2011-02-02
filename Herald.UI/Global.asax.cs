using System;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using Herald.Core;
using Herald.Core.Domain;
using Herald.Infrastructure;

namespace Herald.UI
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("account show", "Account/{id}/{name}", new {controller = "Account", action = "Show", name = ""}, new {id = @"\d+"});
            routes.MapRoute("speakers show", "Speakers/{id}/{name}", new {controller = "Speakers", action = "Show", name = ""}, new {id = @"\d+"});
            routes.MapRoute("sessions show", "Sessions/{id}/{title}", new {controller = "Sessions", action = "Show", title = ""}, new {id = @"\d+"});

            routes.MapRoute("about", "About", new {controller = "Home", action = "About"});

            routes.MapRoute("Default", "{controller}/{action}/{id}", new {controller = "Home", action = "Index", id = UrlParameter.Optional});
        }

        protected void Application_Start()
        {
            Ioc.Initialize(new StructureMapContainer());

            ControllerBuilder.Current.SetControllerFactory(new AppControllerFactory());
            ModelBinders.Binders[typeof(PresentationCategory)] = new PresentationCategoryBinder();

            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            EnsureSpecialPagesExist();

            SetVersionInfo();
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            CheckForWWW();
            SetSystemPath();
        }

        /****************************************************************/
        /****************************************************************/

        private static readonly Regex _wwwRegex = new Regex("(http|https)://www\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private bool _systemPathAlreadySet;

        private void CheckForWWW()
        {
            Uri url = HttpContext.Current.Request.Url;

            if (!_wwwRegex.IsMatch(url.ToString())) return;

            string newUrl = _wwwRegex.Replace(url.ToString(), string.Format("{0}://", url.Scheme));
            HttpContext.Current.Response.Redirect(newUrl);
        }

        private void SetSystemPath()
        {
            if (_systemPathAlreadySet)
                return;

            _systemPathAlreadySet = true;

            var uri = HttpContext.Current.Request.Url;
            var baseUri = "{0}://{1}".F(uri.Scheme, uri.Authority);
            SystemPath.BaseUri = () => baseUri;

            var applicationPath = HttpContext.Current.Request.ApplicationPath;
            SystemPath.ApplicationPath = () => applicationPath;
        }

        private void EnsureSpecialPagesExist()
        {
            var repository = Ioc.Resolve<IRepository>();

            // just have to try to open each page. If it doesn't exist, the default will be created and saved
            Enum
                .GetValues(typeof(SpecialPage))
                .Cast<SpecialPage>()
                .Each(specialPage => repository.GetSpecialPage(specialPage));
        }

        private void SetVersionInfo()
        {
            Application["revision"] = ConfigurationManager.AppSettings["changeset"];
            Application["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}