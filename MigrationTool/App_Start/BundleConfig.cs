using System.Web;
using System.Web.Optimization;

namespace MigrationTool
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.*",
                "~/Scripts/moment.js",
                "~/Scripts/bootstrap-datetimepicker.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include("~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/bootstrap-modifiers.css"));

            // select2
            bundles.Add(new ScriptBundle("~/bundles/select2").Include("~/Scripts/select2.js"));
            bundles.Add(new StyleBundle("~/Content/select2css").Include("~/Content/select2.css"));

            // Bonsai
            bundles.Add(new ScriptBundle("~/bundles/bonsai").Include(
                "~/Scripts/jquery.bonsai.js",
                "~/Scripts/jquery.qubit.js"));
            bundles.Add(new StyleBundle("~/Content/bonsaicss").Include("~/Content/jquery.bonsai.css"));

            // bootstrap-treeview
            bundles.Add(new ScriptBundle("~/bundles/treeview").Include("~/Scripts/bootstrap-treeview.js"));
            bundles.Add(new StyleBundle("~/Content/treeviewcss").Include("~/Content/bootstrap-treeview.css"));

            // spin.js
            bundles.Add(new ScriptBundle("~/bundles/spin").Include(
                "~/Scripts/spin.js",
                "~/Scripts/jquery.spin.js"));
        }
    }
}