using System.Web;
using System.Web.Optimization;

namespace Project.Performance.WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                      "~/Scripts/FileSaver.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/fileinput.js",
                      "~/Scripts/fileinput.min.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/loadxmldoc.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/fileinput.css",
                      "~/Content/fileinput.min.css",
                      "~/Content/site.css"));

            // Add Pop Message plugin
            bundles.Add(new ScriptBundle("~/bundles/growl").Include(
                    "~/Scripts/growl/jquery.growl.js"));
            bundles.Add(new StyleBundle("~/growl/css").Include(
                    "~/Scripts/growl/jquery.growl.css"));

            // Add index to textarea
            bundles.Add(new ScriptBundle("~/bundles/toolScript").Include(
                    "~/Scripts/toolScript/textareaIndex.js"));
        }
    }
}
