using System.Web;
using System.Web.Optimization;

namespace PharmaCoHealth
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            #region Template Designing

            bundles.Add(new ScriptBundle("~/template/js").Include(
                        "~/Scripts/jquery.min.js",
                        "~/Scripts/jquery.easing.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/custom.js",
                        "~/Scripts/contactform.js"));

            bundles.Add(new StyleBundle("~/template/css").Include(
                      "~/Content/css/downloaded.css",
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/style.css"
                      ));

            #endregion
        }
    }
}
